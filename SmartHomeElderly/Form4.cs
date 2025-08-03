using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SmartHomeElderly
{
    public partial class Form4 : Form
    {
        Thread th;
        Query query = new Query();
        string recipient = "";
        Random random;
        private static int countdown = 20 * 60;

        public Form4()
        {
            InitializeComponent();
            comboBox1.Text = "Επιλογή Τηλεφώνου:";
            comboBox1.Items.AddRange(query.GetAllPhones());
            textBox1.Text = query.GetFullName();
            random = new Random();
            timer1.Enabled = true;
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            if (countdown > 0) countdown--;
            else
            {
                timer1.Enabled = false;
                countdown = 20 * 60;
                new Form2().MessageBoxTimesUp();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (comboBox1.SelectedItem != null && !comboBox1.SelectedItem.Equals("Επιλογή Τηλεφώνου:") && !textBox2.Text.Contains(comboBox1.SelectedItem.ToString()))
            {
                if (recipient.Equals("")) recipient = comboBox1.SelectedItem.ToString();
                else recipient += ',' + comboBox1.SelectedItem.ToString();
            }
            textBox2.Text = recipient;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("Το μήνυμα εστάλη!!!", "Αποστολή", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboBox1.Text = "Επιλογή Τηλεφώνου:";
                recipient = "";
                textBox2.Text = string.Empty;
                richTextBox1.Text = string.Empty;                
            }
            else
            {
                MessageBox.Show("Κάποιο πεδίο είναι κενό!", "Συμπλήρωση πεδίων", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
                
        private void openNewForm(object obj)
        {
            Application.Run(new Form2());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            String[] messages = { "Έπεσα από το κρεβάτι.", "Έχει πάρει φωτιά το σπίτι.", "Κάποιοι κλέφτες με χτύπησαν και μου έκλεψαν τα λεφτά." };
            MessageBox.Show(query.SelectElderly() + Environment.NewLine + messages[random.Next(3)] + Environment.NewLine + "Χρειάζομαι βοήθεια!!!", "Αποστολή μηνύματος στο 166", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "70");
        }
    }
}
