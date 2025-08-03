using System;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartHomeElderly
{
    public partial class Form5 : Form
    {
        Thread th;
        Query query = new Query();

        public Form5()
        {
            InitializeComponent();
        }

        private void openNewForm(object obj)
        {
            Application.Run(new Form1());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.register = true;
            this.Close();
            th = new Thread(openNewForm);
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void openNewForm2(object obj)
        {
            Application.Run(new Form2());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool login = query.Login(textBox1.Text);
            if (login)
            {
                Form1.register = false;
                this.Close();
                th = new Thread(openNewForm2);
                th.TrySetApartmentState(ApartmentState.STA);
                th.Start();
            }
            else
            {
                MessageBox.Show("Λανθασμένο ΑΜΚΑ!" + Environment.NewLine + "Παρακαλώ προσπαθήστε ξανά.", "Error", 0, MessageBoxIcon.Error);
                textBox1.Text = string.Empty;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "SmartHomeHelp.chm", HelpNavigator.TopicId, "20");
        }
    }
}
