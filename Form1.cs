using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace PRG282_Project
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        string path = "superheros.txt";

        // --- Save a single hero record to file ---
        private void SaveHero(Hero hero, bool append)
        {
            //attempt to write, catch error if necessary
            try
            {
                using (StreamWriter sw = new StreamWriter(path, append))
                {
                    sw.WriteLine(hero.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving hero: {ex.Message}", "File Error");
            }
        }

        // --- Add a new hero record ---
        private void btnAddHero_Click(object sender, EventArgs e)
        {
            try
            {
                //Validate WhiteSpace
                if (string.IsNullOrWhiteSpace(txbID.Text) || string.IsNullOrWhiteSpace(txbName.Text) ||
                    string.IsNullOrWhiteSpace(txbSuperpower.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error");
                    return;
                }
                //Validate Number
                if (!int.TryParse(txbID.Text, out int id))
                {
                    MessageBox.Show("Hero ID must be a valid number.", "Validation Error");
                    return;
                }
                //Validate Number Range
                int examScore = (int)nudHeroScore.Value;
                if (examScore < 0 || examScore > 100)
                {
                    MessageBox.Show("Exam Score must be between 0 and 100.", "Validation Error");
                    return;
                }

                // Calculate age
                var today = DateTime.Today;
                int age = today.Year - dtpDOB.Value.Year;
                if (dtpDOB.Value.Date > today.AddYears(-age)) age--;

                Hero newHero = new Hero(id, txbName.Text, age, txbSuperpower.Text, examScore);

                // Display hero info
                MessageBox.Show($"Age: {newHero.Age}\nRank: {newHero.Rank}\nThreat Level: {newHero.ThreatLevel}");

                // Save record
                SaveHero(newHero, true);
                MessageBox.Show("Hero added successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding hero: {ex.Message}", "Unexpected Error");
            }
        }

        //Clear input text boxes
        private void btnClear_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Clear all form inputs?", "Warning!", MessageBoxButtons.OKCancel);
            if (confirmResult == DialogResult.OK)
            {
                txbID.Clear();
                txbName.Clear();
                dtpDOB.Value = DateTime.Today;
                txbSuperpower.Clear();
                nudHeroScore.Value = 0;

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    //add hero
                    break;
                case 1:
                    //view hero
                    ShowHeroes();
                    break;
                case 2:
                    //Update hero
                    break;
                default:
                    //impossible
                    break;
            }

        }


        //Fetching Data from File and displaying in table
        private DataTable GetData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(string));
            dt.Columns.Add("Superpower", typeof(string));
            dt.Columns.Add("Exam Score", typeof(string));
            dt.Columns.Add("Rank", typeof(string));
            dt.Columns.Add("Threat Level", typeof(string));

            //Attempting to read data from File
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close(); // create empty file if missing
                    return dt;
                }
                //Read Data from File knowing it exists
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null && line.Trim() != "")
                    {
                        //Adding each Hero that exists to Array
                        string[] parts = line.Split(';');
                        if (parts.Length == 5)
                        {
                            Hero hero = new Hero(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                int.Parse(parts[4])
                            );
                            //Adding Hero to the Array for display
                            dt.Rows.Add(hero.ToStringArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "File Error");
            }

            return dt;
        }
        //adding data back in to the text file after changes
        private void SaveDataAll(DataTable dt)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                foreach (DataRow row in dt.Rows)
                {
                    Hero h = new Hero(
                        int.Parse(row[0].ToString()),
                        row[1].ToString(),
                        int.Parse(row[2].ToString()),
                        row[3].ToString(),
                        int.Parse(row[4].ToString())
                    );
                    SaveHero(h, true);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "File Error");
            }
        }

        //Showing all heroes
        private void ShowHeroes()
        {
            dataGridView1.DataSource = GetData();
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[2].Width = 30;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[6].Width = 400;
            dataGridView1.Refresh();
        }
        //clear data in the update Form
        private void btnClearUpdateForm_Click(object sender, EventArgs e)
        {
            txbIDUpdate.Clear();
            txbNameUpdate.Clear();
            dtpDoBUpdate.Value = DateTime.Now;
            txbSuperpowerUpdate.Clear();
            nudScoreUpdate.Value = 0;
            txbIDUpdate.Focus();

        }

        //Search for a hero
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //checks for WhiteSpace
            if (string.IsNullOrWhiteSpace(txbIDUpdate.Text))
            {
                MessageBox.Show("Enter a Hero ID to search.", "Validation Error");
                return;
            }
            //Attempt  to fetch Data, otherwise Catch Error
            try
            {
                DataTable dt = GetData();
                DataRow[] result = dt.Select($"ID = '{txbIDUpdate.Text}'");
                //Checking for adequate Data
                if (result.Length == 0)
                {
                    MessageBox.Show("No hero found with that ID.", "Not Found");
                    return;
                }

                DataRow row = result[0];
                txbNameUpdate.Text = row[1].ToString();
                dtpDoBUpdate.Value = DateTime.Today.AddYears(-int.Parse(row[2].ToString()));
                txbSuperpowerUpdate.Text = row[3].ToString();
                nudScoreUpdate.Value = int.Parse(row[4].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching hero: {ex.Message}", "Error");
            }
        }

        //Update Button and send data to Text File
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //Attempt to execute otherwise catch error
            try
            {
                //Check for WhiteSpace
                if (string.IsNullOrWhiteSpace(txbIDUpdate.Text))
                {
                    MessageBox.Show("Enter a valid Hero ID to update.", "Validation Error");
                    return;
                }

                DataTable dt = GetData();
                DataRow[] result = dt.Select($"ID = '{txbIDUpdate.Text}'");

                if (result.Length == 0)
                {
                    MessageBox.Show("No hero found to update.", "Error");
                    return;
                }
                //Takes Data and sends in to Text File
                DataRow row = result[0];
                int age = DateTime.Today.Year - dtpDoBUpdate.Value.Year;
                if (dtpDoBUpdate.Value.Date > DateTime.Today.AddYears(-age)) age--;

                row["Name"] = txbNameUpdate.Text;
                row["Age"] = age.ToString();
                row["Superpower"] = txbSuperpowerUpdate.Text;
                row["Exam Score"] = ((int)nudScoreUpdate.Value).ToString();

                SaveDataAll(dt);
                MessageBox.Show("Hero updated successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating hero: {ex.Message}", "Error");
            }
        }

        // --- Delete selected hero record ---
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Catching Errors
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please select a hero to delete.", "Error");
                    return;
                }

                if (MessageBox.Show("Delete selected hero?", "Confirm Delete", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
                //Execute Deletion
                int rowIndex = dataGridView1.CurrentRow.Index;
                DataTable dt = (DataTable)dataGridView1.DataSource;
                dt.Rows.RemoveAt(rowIndex);

                SaveDataAll(dt);
                dataGridView1.DataSource = dt;
                MessageBox.Show("Hero deleted successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting hero: {ex.Message}", "Error");
            }
        }
    }
}
