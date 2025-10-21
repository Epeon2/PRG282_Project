using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRG282_Project
{
    internal class Hero
    {
        //Establishing Dictionary for heroes
        public static Dictionary<string, string> HeroRankThreat = new Dictionary<string, string>
        {
           {"S-Rank", "Finals Week (threat to the entire academy)"},
           {"A-Rank", "Midterm Madness (threat to a department)"},
           {"B-Rank", "Group Project Gone Wrong (threat to a study group)"},
           {"C-Rank", "Pop Quiz (potential threat to an individual student)"},
        };

        //Instantiating Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Superpower { get; set; }
        public int ExamScore { get; set; }
        public string Rank { get; set; }
        public string ThreatLevel { get; set; }

        //Establishing Hero Class
        public Hero(int id, string name, int age, string superpower, int examScore)
        {
            Id = id;
            Name = name;
            Age = age;
            Superpower = superpower;
            ExamScore = examScore;
            calcRank();
        }

        //Rank Calculation of heroes based on Exam score using Switch Case
        public void calcRank()
        {
            switch (ExamScore) {
                case int n when n <= 40:
                    this.Rank = HeroRankThreat.Keys.ElementAt(3);
                    break;
                case int n when n <= 60:
                    this.Rank = HeroRankThreat.Keys.ElementAt(2);
                    break;
                case int n when n <= 80:
                    this.Rank = HeroRankThreat.Keys.ElementAt(1);
                    break;
                case int n when n <= 100:
                    this.Rank = HeroRankThreat.Keys.ElementAt(0);
                    break;
                default:
                    MessageBox.Show("Bad Grade. Error.");
                    break;
            }
            calcThreatLevel(this.Rank);
        }
        
        //Overriding Threat Level based on Calculated Threat Level
        public void calcThreatLevel(string rank)
        {
            this.ThreatLevel = HeroRankThreat[Rank];
        }

        //Overriding To String Method
        public override string ToString()
        {
            return $"{Id};{Name};{Age};{Superpower};{ExamScore}";
        }

        //Establishing toString Array output
        public string[] ToStringArray()
        {
            string[] output = {Id.ToString(), Name, Age.ToString(), Superpower, ExamScore.ToString(), Rank, ThreatLevel};
            return output;
        }
    }
}
