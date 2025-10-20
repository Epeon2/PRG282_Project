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

        private void SaveHero(Hero hero, bool append)
        {
            StreamWriter sw = new StreamWriter(path, append);
            sw.WriteLine(hero.ToString());
            sw.Close();
        }

        private void btnAddHero_Click(object sender, EventArgs e)
        {
            //TODO validation

            int id = Int32.Parse(txbID.Text);
            string name = txbName.Text;

            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            int age = today.Year - dtpDOB.Value.Year;

            // If the birthdate hasn't arrived yet, minus one year.
            if (dtpDOB.Value.Date > today.AddYears(-age)) age--;

            string superpower = txbSuperpower.Text;

            int examScore = ((int)nudHeroScore.Value);

            Hero newHero = new Hero(id, name, age, superpower, examScore);

            MessageBox.Show($"Age: {newHero.Age}\nRank: {newHero.Rank}\nThreat Level: {newHero.ThreatLevel}");

            SaveHero(newHero, true);
        }

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

            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();
            while (! string.IsNullOrEmpty(line))
            {
                string[] result = line.Split(';');
                //no validation required since it happens in add and update forms
                Hero newHero = new Hero(Int32.Parse(result[0]), result[1], Int32.Parse(result[2]), result[3], Int32.Parse(result[4]));
                dt.Rows.Add(newHero.ToStringArray());
                line = sr.ReadLine();
            }
            sr.Close();

            return dt;
        }

        private void SaveDataAll(DataTable dt, bool append)
        {
            if (!append)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                Hero newHero = new Hero(Int32.Parse(row[0].ToString()), row[1].ToString(), Int32.Parse(row[2].ToString()), row[3].ToString(), Int32.Parse(row[4].ToString()));

                SaveHero(newHero, true);
                Thread.Sleep(100);
            }
        }

        private void ShowHeroes()
        {


            dataGridView1.DataSource = GetData();
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[2].Width = 30;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[6].Width = 400;
            dataGridView1.Refresh();
        }

        private void btnClearUpdateForm_Click(object sender, EventArgs e)
        {
            txbIDUpdate.Clear();
            txbNameUpdate.Clear();
            dtpDoBUpdate.Value = DateTime.Now;
            txbSuperpowerUpdate.Clear();
            nudScoreUpdate.Value = 0;
            txbIDUpdate.Focus();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txbIDUpdate.Text;
            //TODO validation

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("No ID");
                return;
            }

            DataTable dt = GetData();

            DataRow result = dt.Select($"ID = {id}")[0];
            if (result != null)
            {
                txbNameUpdate.Text = result[1].ToString();
                dtpDoBUpdate.Value = DateTime.Today.AddYears(-Int32.Parse(result[2].ToString()));
                txbSuperpowerUpdate.Text = result[3].ToString();
                nudScoreUpdate.Value = Int32.Parse(result[4].ToString());
                txbSuperpowerUpdate.Focus();
            }


        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Change Record?", "Warning!", MessageBoxButtons.OKCancel);
            if (!(result == DialogResult.OK))
            {
                return;
            }

            string id = txbIDUpdate.Text;
            //TODO validation

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("No ID");
                return;
            }

            DataTable dt = GetData();
            int index = dt.Rows.IndexOf(dt.Select($"ID = {id}")[0]);

            dt.Rows[index][1] = txbNameUpdate.Text;
            int age = DateTime.Today.Year - dtpDoBUpdate.Value.Year;
            if (dtpDoBUpdate.Value.Date > DateTime.Today.AddYears(-age)) age--;
            dt.Rows[index][2] = age;
            dt.Rows[index][3] = txbSuperpowerUpdate.Text;
            dt.Rows[index][4] = ((int)nudScoreUpdate.Value);

            SaveDataAll(dt, false);

            MessageBox.Show($"Updated hero");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow selected = dataGridView1.CurrentRow;

            int id = selected.Index;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            dt.Rows.RemoveAt(id);

            dataGridView1.DataSource = dt;

            dataGridView1.Refresh();
            SaveDataAll(dt, false);
        }
    }
}
