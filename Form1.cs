using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeaveManagementSYstem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // this is user button
            this.Hide();
            AdminLogin adminLogin = new AdminLogin();
            adminLogin.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this is admin button
            this.Hide();
            UserLogin userLogin = new UserLogin();
            userLogin.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
