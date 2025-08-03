using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SmartHomeElderly
{
    public partial class Form2 : Form
    {
        private int timer, id;
        private int t = 0;
        TimeSpan timeSpan;
        Thread th;
        Query query = new Query();
        readonly SQLiteConnection conn = new SQLiteConnection("Data Source=DBElderly.db;Version=3;");

        public Form2()
        {
            InitializeComponent();
            new Form4().timer1.Enabled = true;
            RefreshTask();
            Scale();
            label1.Text = "Καλώς ορίσατε, " + query.GetFullName();            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer > 0)
            {
                timer--;
                timeSpan = TimeSpan.FromSeconds(timer);
                label4.Text = string.Format("{0}:{1:00}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
                label4.Height = label3.Height + 10;                
            }
            else
            {
                t++;                
                if (t % 2 == 0) label4.Visible = true;
                else label4.Visible = false;
                label4.ForeColor = Color.Red;
                if (t == 1)
                {
                    MessageBoxTimesUp();
                    RefreshTask();
                    Scale();
                    label4.ForeColor = Color.Black;
                }
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (pictureBox6.ImageLocation.Equals("images/next.png"))
            {                
                DialogResult result = MessageBox.Show("Ολοκληρώσατε την δραστηριότητα και θέλετε να πάτε στην επόμενη;", "Επιβεβαίωση", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CompletedTask();
                    Scale();
                }
            }
            else { RefreshTask(); Scale(); }            
        }

        private void CompletedTask()
        {
            conn.Open();
            string updateQuery = "Update Task set Bool=1 where ID=@id and AMKA=@amka";
            SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@amka", query.GetAMKA());
            cmd.ExecuteNonQuery();
            conn.Close();
            RefreshTask();
        }

        private void RefreshTask()
        {
            conn.Open();
            string selectQuery = "Select * from Task where AMKA=@amka";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", query.GetAMKA());
            SQLiteDataReader reader = command.ExecuteReader();
            List<int> ids = new List<int>();
            List<String> time = new List<String>(); ;
            double nowMilliseconds, milliseconds = 0;
            while (reader.Read())
            {
                if (reader.GetInt16(5).Equals(0))
                {
                    (nowMilliseconds, milliseconds) = TimeToMilliseconds(reader.GetString(2));
                    if (nowMilliseconds <= milliseconds + 300000 && milliseconds <= nowMilliseconds + 300000)
                    {
                        timer1.Start();
                        pictureBox6.ImageLocation = "images/next.png";
                        label3.Text = reader.GetString(3);
                        timer = reader.GetInt32(4) * 60;
                        label4.Visible = true;
                        label4.Enabled = true;
                        id = reader.GetInt16(0);
                        reader.Dispose();
                        break;
                    }
                    else
                    {
                        label3.Text = "Δεν υπάρχει κάποια δραστηριότητα" + Environment.NewLine + "αυτήν την στιγμή.";
                        label4.Visible = false;
                        label4.Enabled = false;
                        pictureBox6.ImageLocation = "images/reload.png";
                        timer1.Stop();
                    }
                }
                else
                {
                    ids.Add(reader.GetInt32(0));
                    time.Add(reader.GetString(2));
                    label3.Text = "Δεν υπάρχει κάποια δραστηριότητα" + Environment.NewLine + "αυτήν την στιγμή.";
                    label4.Visible = false;
                    label4.Enabled = false;
                    pictureBox6.ImageLocation = "images/reload.png";
                    timer1.Stop();
                }                
            }
            conn.Close();
            for (int i = 0; i < ids.Count(); i++)
            {
                (nowMilliseconds, milliseconds) = TimeToMilliseconds(time[i]);
                if (nowMilliseconds > milliseconds + 600000 || nowMilliseconds < milliseconds - 600000)
                {
                    conn.Open();
                    string updateQuery = "Update Task set Bool=0 where ID=@id";
                    SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@id", ids[i]);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        private void Scale()
        {
            if (label3.Width > label3.MaximumSize.Width)
            {
                int height = (int)label3.CreateGraphics().MeasureString(label3.Text, label3.Font, label3.Width).Height;
                if(height<50) height += 3;
                label3.Height = height;
                label4.Location = new Point(label4.Left, label3.Bottom);
            }
            label4.ForeColor = SystemColors.ControlText;
            timeSpan = TimeSpan.FromSeconds(timer);
            label4.Text = string.Format("{0}:{1:00}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
        }

        private (double, double) TimeToMilliseconds(string time)
        {
            string[] hour_min = time.Split(':');
            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            TimeSpan specificTime = new TimeSpan(int.Parse(hour_min[0]), int.Parse(hour_min[1]), 0);
            return (nowTime.TotalMilliseconds, specificTime.TotalMilliseconds);
        }

        public void MessageBoxTimesUp()
        {
            DialogResult result = MessageBox.Show("Είναι όλα καλά;", "Επιβεβαίωση", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                result = MessageBox.Show("Χρειάζεστε βοήθεια; Θέλετε να σταλθεί μήνημα στο 166;", "Αποστολή μηνύματος", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show(query.SelectElderly() + "\nSOS.Χρειάζομαι βοήθεια!", "Στάλθηκε το παρακάτω μήνυμα");
                }
            }
            t = 0;
        }

        private void openNewForm(object obj)
        {
            Application.Run(new Form1());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void openNewForm2(object obj)
        {
            Application.Run(new Form3());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm2);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void openNewForm3(object obj)
        {
            Application.Run(new Form4());
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm3);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void openNewForm4(object obj)
        {
            Application.Run(new Form5());
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "40");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm4);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }
    }
}
