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
    public partial class EmployeeLeaveView : Form
    {
        private string connectionString = ""; /* Your Databse link*/

        public EmployeeLeaveView()
        {
            InitializeComponent();
            
        }

        private void EmployeeLeaveView_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet3.LeaveReqs' table. You can move, or remove it, as needed.
            this.leaveReqsTableAdapter.Fill(this.dataSet3.LeaveReqs);

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string empId = txtEmpID.Text;

            // id should not be empty
            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Please enter your Employee ID.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT LeaveID,EmployeeID, StartDate, EndDate, LeaveType, Status, Reason FROM LeaveReqs WHERE EmployeeID = @EmployeeID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmployeeID", empId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable leaveData = new DataTable();
                        adapter.Fill(leaveData);
                        dataGridView1.DataSource = leaveData;
                    }

                    // Check for request
                    if (dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("No leave requests found for this Employee ID.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving leave requests: " + ex.Message);
                }
            }


        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString)) 
            {
                try {
                    con.Open();
                    string query_refresh ="SELECT * FROM LeaveReqs" ;
                    SqlDataAdapter adapter = new SqlDataAdapter(query_refresh,con);
                    DataTable leaveData = new DataTable();
                    adapter.Fill(leaveData);
                    dataGridView1.DataSource = leaveData;
                    dataGridView1.Refresh();
                }
                catch (Exception ex) {
                    MessageBox.Show("Error while refreshing...." + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show(); 
        }


       

    }
}
  

