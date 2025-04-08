using LeaveManagementSystem;
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
    public partial class ADashBoard : Form
    {
        public ADashBoard()
        {
            InitializeComponent();
        }

        private void txtMemployee_Click(object sender, EventArgs e)
        {
            this.Hide();
            Employeeregister employeeregister = new Employeeregister();
            employeeregister.Show();
        }

        private void btnMleave_Click(object sender, EventArgs e)
        {
            this.Hide();
            ManageLeave manageLeave = new ManageLeave();
            manageLeave.Show();
        }
        // back button
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminLogin adminLogin = new AdminLogin();   
            adminLogin.Show();
        }

        private void btnhistory_Click(object sender, EventArgs e)
        {
            this.Hide();
            LeaveHistoryViewAdmin leaveHistoryViewAdmin = new LeaveHistoryViewAdmin();  
            leaveHistoryViewAdmin.Show();
        }

        private void btnRadmin_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminRegister adminRegister = new AdminRegister();
            adminRegister.Show();
        }

        private void ADashBoard_Load(object sender, EventArgs e)
        {

        }
    }
}
