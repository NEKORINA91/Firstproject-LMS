using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LeaveManagementSYstem
{
    public partial class AdminRegister : Form
    {
        private string connectionString = ""; /* Your Databse link*/

        public AdminRegister()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                string query_add = " INSERT INTO adminTBL (AdminID,Password)VALUES (@AdminID,@Password)";
                SqlCommand cmd = new SqlCommand(query_add, con);

                cmd.Parameters.AddWithValue("@AdminID",txtAdminId.Text);
                cmd.Parameters.AddWithValue("@Password",txtPassword.Text);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery ();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Registration successful.!!");

                    this.Hide();
                    ADashBoard aDashBoard = new ADashBoard();
                    aDashBoard.Show();
                }
                else
                {
                    MessageBox.Show("Registration failed.!!!","Register error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            ADashBoard aDashBoard = new ADashBoard();   
            aDashBoard.Show();  
        }

        private void AdminRegister_Load(object sender, EventArgs e)
        {

        }
    }
}
