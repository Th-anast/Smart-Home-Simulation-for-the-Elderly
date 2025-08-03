using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartHomeElderly
{
    public partial class Form6 : Form
    {
        Thread th;
        Query query = new Query();
        string id_tasks = string.Empty;

        public Form6()
        {
            InitializeComponent();
            new Form4().timer1.Enabled = true;
            (richTextBox1.Text, id_tasks) = query.AlreadyRegisteredTask(string.Empty);
            comboBox1.Text = "Επιλογή ID για διαγραφή:";
            comboBox1.Items.AddRange(id_tasks.ToString().Split(','));
            comboBox1.Items.RemoveAt(comboBox1.Items.Count - 1);
        }        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && !comboBox1.SelectedItem.Equals("Επιλογή ID για διαγραφή:"))
            {
                DialogResult result = MessageBox.Show("Θέλετε να διαγράψετε την δραστηριότητα με ID:" + comboBox1.SelectedItem.ToString() + ";", "Επιβεβαίωση", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    query.DeleteTask(Int32.Parse(comboBox1.SelectedItem.ToString()));
                    (richTextBox1.Text, id_tasks) = query.AlreadyRegisteredTask(string.Empty);
                    MessageBox.Show("Η δραστηριότητα διαγράφτηκε επιτυχώς!", "Επιτυχία", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    comboBox1.Text = "Επιλογή ID για διαγραφή:";
                }                
            }
        }

        private void openNewForm(object obj)
        {
            Application.Run(new Form3());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "60");
        }
    }
}
