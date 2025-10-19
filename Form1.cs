using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRG282_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string path = "superheros.txt";

        private void SaveHero(Hero hero)
        {
            StreamWriter sw = new StreamWriter(path);
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

            SaveHero(newHero);
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

        private void ShowHeroes()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID",typeof(string));
            dt.Columns.Add("Name",typeof(string));
            dt.Columns.Add("Age",typeof(string));
            dt.Columns.Add("Superpower",typeof(string));
            dt.Columns.Add("Exam Score",typeof(string));
            dt.Columns.Add("Rank",typeof(string));
            dt.Columns.Add("Threat Level",typeof(string));

            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();
            while(line != null)
            {
                string[] result = line.Split(';');
                //no validation required since it happens in add and update forms
                Hero newHero = new Hero(Int32.Parse(result[0]), result[1], Int32.Parse(result[2]), result[3], Int32.Parse(result[4]));
                dt.Rows.Add(newHero.ToStringArray());
                line = sr.ReadLine();
            }

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[2].Width = 30;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[6].Width = 400;
            dataGridView1.Refresh();
        }
    }
}
