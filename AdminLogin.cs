using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeaveManagementSYstem
{
    public partial class AdminLogin : Form
    {
        private string connectionString = "" ; /* Your Databse link*/ 

        public AdminLogin()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 start = new Form1();
            start.Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAdminId.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("AdminId and Password cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query_get = "SELECT AdminId FROM adminTBL WHERE AdminID= @AdminID AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(query_get, con);
                    cmd.Parameters.AddWithValue("@AdminId", txtAdminId.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string EmployeeID = result.ToString();
                        MessageBox.Show("Welcome", "Login Sucessful", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.Hide();
                        ADashBoard aDashBoard = new ADashBoard();
                        aDashBoard.Show();


                    }
                    else {
                        MessageBox.Show("Wrong credentials pls try again", "Error while logging", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void AdminLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
