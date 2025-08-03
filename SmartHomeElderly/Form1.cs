using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SmartHomeElderly
{
    public partial class Form1 : Form
    {
        private string phones;
        private TextBox lastTextBox;
        Thread th;
        Query query = new Query();
        public static bool register = false;

        public Form1()
        {            
            InitializeComponent();
            if (register)
            {
                this.Text = "Εγγραφή Χρήστη";
                button1.Text = "ΕΓΓΡΑΦΗ";
            }
            else
            {
                new Form4().timer1.Enabled = true;
                this.Text = "Στοιχεία Χρήστη";
                button1.Text = "ΕΝΗΜΕΡΩΣΗ";
                textBox1.Text = Query.name;
                textBox1.ReadOnly = true;
                textBox2.Text = Query.surname;
                textBox2.ReadOnly = true;
                textBox3.Text = Query.AMKA;
                textBox3.ReadOnly = true;
                dateTimePicker1.Text = Query.dateofbirth;
                dateTimePicker1.Enabled = false;
                textBox4.Text = Query.weight.ToString();
                textBox5.Text = Query.region;
                textBox6.Text = Query.address;
                textBox7.Text = Query.phoneNumbers[0];
                for(int i = 1; i < Query.phoneNumbers.Length; i++) phoneTextboxes(Query.phoneNumbers[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Controls.OfType<TextBox>().Take(7).All(tb => !string.IsNullOrEmpty(tb.Text)))
            {
                try
                {
                    phones = "";
                    int c = 0;
                    foreach (TextBox textBox in this.Controls.OfType<TextBox>())
                    {
                        c++;
                        if (c == 1)
                        {                            
                            if (!Int64.TryParse(textBox.Text, out long i) || textBox.TextLength < 11)
                            {
                                MessageBox.Show("Το πεδίο ΑΜΚΑ δέχεται μόνο 11 ψηφία!", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        if (c == 2 || c > 7)
                        {
                            if(Int64.TryParse(textBox.Text, out long i))
                            {
                                if (textBox.TextLength == 10)
                                {
                                    if (phones.Equals("")) phones = textBox.Text;
                                    else phones += ',' + textBox.Text;
                                }
                                else
                                {
                                    MessageBox.Show("Κάθε αριθμός τηλεφώνου πρέπει να έχει 10 ψηφία.", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }else if (string.IsNullOrEmpty(textBox.Text))
                            {
                                continue;
                            }
                            else
                            {
                                MessageBox.Show("Το πεδίο Τηλ.Επικοινωνίας δέχεται μόνο αριθμούς!", "Εισαγωγή αριθμού", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }                        
                    }
                    if (register)
                    {
                        query.Register(textBox1.Text,textBox2.Text,textBox3.Text,dateTimePicker1.Text,int.Parse(textBox4.Text),textBox5.Text,textBox6.Text,phones);
                        MessageBox.Show("Εγγραφήκατε με επιτυχία!", "Επιτυχία", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        th = new Thread(openNewForm);
                        th.TrySetApartmentState(ApartmentState.STA);
                        th.Start();
                    }
                    else
                    {
                        query.UpdateData(int.Parse(textBox4.Text), textBox5.Text, textBox6.Text, phones);
                        MessageBox.Show("Τα δεδομένα ενημερώθηκαν επιτυχώς!", "Επιτυχία", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }                  
                }
                catch (Exception)
                {
                    if (!int.TryParse(textBox4.Text,out int i))
                    {
                        MessageBox.Show("Το πεδίο Βάρος δέχεται μόνο αριθμούς!", "Εισαγωγή αριθμού", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Ο χρήστης είναι ήδη καταχωρημένος!", "Σφάλμα: Υπάρχουσα στοιχεία", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Κάποιο πεδίο είναι κενό!", "Συμπλήρωση πεδίων", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            phoneTextboxes(null);
        }

        private void phoneTextboxes(string phone)
        {
            TextBox newTextBox = new TextBox
            {
                Font = textBox7.Font,
                MaxLength = 10,
                Size = new Size(textBox7.Width, textBox7.Height),
                Location = new Point(textBox7.Left, textBox7.Bottom + 3)
            };
            if (lastTextBox != null)
            {
                newTextBox.Location = new Point(lastTextBox.Left, lastTextBox.Bottom + 3);
                button1.Location = new Point(button1.Left, lastTextBox.Bottom + 50);
            }
            else button1.Location = new Point(button1.Left, newTextBox.Bottom + 15);
            Height += 32;
            if (phone != null)  newTextBox.Text = phone;
            pictureBox1.Location = new Point(pictureBox1.Left, pictureBox1.Bottom + 3);
            this.Controls.Add(newTextBox);
            lastTextBox = newTextBox;
        }

        private void openNewForm(object obj)
        {
            if (register) Application.Run(new Form5());
            else Application.Run(new Form2());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(openNewForm);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(register) Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "30");
            else Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "50");
        }
    }
}