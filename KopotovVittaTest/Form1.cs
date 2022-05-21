using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KopotovVittaTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var name = textBox1.Text;
            int index = 0;

            if (!Database.TryGetUserId(name, out index))
                index = Database.CreateUser(name);

            Form form = new MainForm(index);

            this.Hide();
            form.ShowDialog();
            this.Show();
            //form.Show();
        }
    }
}
