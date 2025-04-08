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
using System.Transactions;
using System.Collections;

namespace LeaveManagementSYstem
{

    public partial class ManageLeave : Form

    {

        // remaining leaves
        private int remainingAnnualLeaves;
        private int remainingCasualLeaves;
        private int remainingShortLeaves;

        public ManageLeave()
        {

            LoadPendingLeaves();
            InitializeComponent();
            RefreshGrid();
             

        }
        private string connectionString = ""; /* Your Databse link*/
        DataTable LeaveData = new DataTable();

        private void ManageLeave_Load(object sender, EventArgs e)
        {
            
            this.leaveReqsTableAdapter.Fill(this.dataSet4.LeaveReqs);

        }
        private void LoadPendingLeaves()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason FROM LeaveReqs WHERE Status = 'Pending'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    DataTable pendingLeaves = new DataTable();
                    adapter.Fill(pendingLeaves);
                    dataGridView1.DataSource = pendingLeaves;
                }
                catch (Exception )
                {
                    
                }
            }
        }




        private void btnApprove_Click(object sender, EventArgs e)
        {
            string empid = txtEmpID.Text; 

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    
                    string updateStatusQuery = "UPDATE LeaveReqs SET Status = 'Approved' WHERE EmployeeID = @EmployeeID";
                    using (SqlCommand updateCmd = new SqlCommand(updateStatusQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@EmployeeID",txtEmpID.Text );
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Leave request approved successfully.", "Approval Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshGrid(); 
                        }
                        else
                        {
                            MessageBox.Show("Leave request not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }




        private void btnReject_Click(object sender, EventArgs e)
        {
            string employeeId = txtEmpID.Text; // Get the Employee ID from the text box

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Retrieve the leave request details from a table i guess
                    string query = "SELECT LeaveType, TotalDays FROM LeaveReqs WHERE EmployeeID = @EmployeeID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId); 

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string leaveType = reader.GetString(0);
                        int totalDays = reader.GetInt32(1);

                        // Update the employee's leave balances plzzz work
                        ReverseLeaveDeduction(employeeId, leaveType, totalDays);

                        // Update the status dooo "Rejected"
                        reader.Close(); 
                        string updateStatusQuery = "UPDATE LeaveReqs SET Status = 'Rejected' WHERE EmployeeID = @EmployeeID";
                        using (SqlCommand updateCmd = new SqlCommand(updateStatusQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@EmployeeID", employeeId); 
                            updateCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Leave request rejected and leave balance updated.", "Rejection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Leave request not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }








        private void LoadLeaveRequests(string status)
        {
            string query = "SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status FROM LeaveReqs WHERE Status = @Status";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", status);

                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable; // view
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading requests: " + ex.Message);
                }
            }
        }






        private void btnSearch_Click(object sender, EventArgs e)
        {
            string empId = txtEmpID.Text;

            // ID should not be empty
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
                    string query = "SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason FROM LeaveReqs WHERE EmployeeID = @EmployeeID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmployeeID", empId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable leaveData = new DataTable();
                        adapter.Fill(leaveData);
                        dataGridView1.DataSource = leaveData;
                    }

                    // Check for leave is there ot not
                    if (dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("No leave requests found for this Employee ID.");
                    }

                    else
                    {
                        // Highlighter i guess
                        bool rowFound = false;

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["EmployeeID"].Value != null &&
                                row.Cells["EmployeeID"].Value.ToString() == empId)
                            {
                                row.Selected = true;
                                dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                                rowFound = true;
                                 
                            }
                        }


                    }
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving leave requests: " + ex.Message);
                }
            }
        }

        private void RefreshGrid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query_refresh = "SELECT * FROM LeaveReqs";
                using (SqlCommand CMD = new SqlCommand(query_refresh, con))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(CMD))
                    {
                        LeaveData.Clear();
                        adapter.Fill(LeaveData);
                        dataGridView1.DataSource = LeaveData;
                    }
                }
            }
        }

        private void ReverseLeaveDeduction(string employeeId, string leaveType, int totalDays)
        {
            // remove reduced atoms(Leavezzz)
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE EmployeeTBL SET ";

                switch (leaveType)
                {
                    case "Annual":
                        query += "Anual_leave = Anual_leave + @TotalDays ";
                        break;
                    case "Casual":
                        query += "Casual_leave = Casual_leave + @TotalDays ";
                        break;
                    case "Short":
                        query += "Short_leave = Short_leave + @TotalDays ";
                        break;
                    default:
                        throw new ArgumentException("Invalid leave type.");
                }

                query += "WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TotalDays", totalDays);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            ADashBoard aDashBoard = new ADashBoard();
            aDashBoard.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason FROM LeaveReqs";
                    SqlCommand cmd = new SqlCommand(query, con);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable leaveData = new DataTable();
                        adapter.Fill(leaveData);
                        dataGridView1.DataSource = leaveData;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving leave requests: " + ex.Message);
                }
            }
            Refresh();
        }

        // Rejections happens what to do  
        private void ReverseAppliedLeavesifRejected(string employeeId, string leaveType, int totalDays)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();


                    string columnName = leaveType == "Annual" ? "Anual_leave" :
                                        leaveType == "Casual" ? "Casual_leave" : "Short_leave";

                    // Update the   leave balance
                    string updateQuery = $"UPDATE EmployeeTBL SET {columnName} = {columnName} + @TotalDays WHERE EmployeeID = @EmployeeID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@TotalDays", totalDays);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while updating leave balance: " + ex.Message);
                }
            }
        }

        private void btnPLeaves_Click(object sender, EventArgs e)
        {
            try {

                using (SqlConnection con = new SqlConnection(connectionString))
                {


                    string query_pending = "Select * from LeaveReqs where status = 'Pending'";
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query_pending, con);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind DataTable to DataGridView
                    dataGridView1.DataSource = dataTable;
                }

            }


            catch { }


        }
    }



}







