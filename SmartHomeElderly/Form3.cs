using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SmartHomeElderly
{
    public partial class Form3 : Form
    {        
        Thread th;
        Query query = new Query();

        public Form3()
        {
            InitializeComponent();
            new Form4().timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals("") && !richTextBox1.Text.Equals(""))
            {
                try
                {
                    if (int.Parse(textBox1.Text) < 0 || int.Parse(textBox1.Text) > 23 || int.Parse(textBox2.Text) < 0 || int.Parse(textBox2.Text) > 59)
                    {
                        MessageBox.Show("Η ώρα είναι λανθασμένη!", "Error", 0, MessageBoxIcon.Error);
                    }                    
                    else
                    {
                        string time = textBox1.Text + ':' + textBox2.Text;
                        (string c, _) = query.AlreadyRegisteredTask(time);
                        if (c.Equals("0"))
                        {
                            query.InsertTask(time, richTextBox1.Text, numericUpDown1.Text);
                            textBox1.Text = string.Empty;
                            textBox2.Text = string.Empty;
                            numericUpDown1.Value = 1;
                            richTextBox1.Text = string.Empty;
                            MessageBox.Show("Η δραστηριότητα αποθηκεύτηκε επιτυχώς!", "Επιτυχία", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Υπάρχει ήδη μια δραστηριότητα γύρω στις " + time + "." + Environment.NewLine + "Παρακαλώ εισάγετε τουλάχιστον 10' νωρίτερα ή αργότερα!", "Αλλαγή Ωρας", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Τα πεδία Ώρα και Αντιστροφη μέτρηση δέχονται μόνο αριθμούς!", "Εισαγωγή αριθμού", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }                
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

        private void openNewForm2(object obj)
        {
            Application.Run(new Form6());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm2);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "60");
        }
    }
}
