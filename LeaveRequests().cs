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
using System.Xml.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;

namespace LeaveManagementSYstem
{
    public partial class LeaveRequests : Form

    {
        //leave types
        int anl = 0;
        int  csl=0;
        int shl= 0;


        // leaves date calculated
        private DateTime startDate; 
        private DateTime endDate;   
        private int totalDays;       

        // remaining leaves
        private int remainingAnnualLeaves;
        private int remainingCasualLeaves;
        private int remainingShortLeaves;

        public LeaveRequests()
        {
            
            InitializeComponent();
            leaveupdateinemployeetbl();
            RefreshGrid();
            GetLeaves(txtEmpID.Text);
            
        }
        private string connectionString = ""; /* Your Databse link*/ 
        private DataTable LeaveData = new DataTable();
        private void LeaveRequests_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet11.LeaveReqs' table. You can move, or remove it, as needed.
           // this.leaveReqsTableAdapter2.Fill(this.dataSet11.LeaveReqs);
            // TODO: This line of code loads data into the 'dataSet10.LeaveReqs' table. You can move, or remove it, as needed.
           // this.leaveReqsTableAdapter1.Fill(this.dataSet10.LeaveReqs);
            // TODO: This line of code loads data into the 'dataSet9.EmployeeTBL' table. You can move, or remove it, as needed.
          //  this.employeeTBLTableAdapter.Fill(this.dataSet9.EmployeeTBL);

            this.leaveReqsTableAdapter.Fill(this.dataSet2.LeaveReqs);



            
            
                //this.leaveReqsTableAdapter.Fill(this.dataSet2.LeaveReqs);

                // get leaves 
                if (!string.IsNullOrEmpty(txtEmpID.Text))
                {
                    GetLeaves(txtEmpID.Text);
                }
            // calenderst tooo timey things
                dtpSleaveTime.Format= DateTimePickerFormat.Custom;
                dtpSleaveTime.CustomFormat = "HH:mm:ss";
                dtpSleaveTime.ShowUpDown = true;
                dtpSleaveTime2.Format = DateTimePickerFormat.Custom;
                dtpSleaveTime2.CustomFormat = "HH:mm:ss";
                dtpSleaveTime2.ShowUpDown = true;

        }

        private void rdbShort_CheckedChanged(object sender, EventArgs e)
        {
            if ( rdbShort.Checked){
            dtpSleaveTime.Enabled = true;
            dtpSleaveTime2.Enabled = true;

            }

            else
            {

                dtpSleaveTime.Enabled= false;
                dtpSleaveTime2.Enabled= false;
            }
        }








        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (
            string.IsNullOrEmpty(txtEmpID.Text) ||
            string.IsNullOrEmpty(txtLeaveID.Text) ||
            string.IsNullOrEmpty(dtpStartdate.Text) ||
            string.IsNullOrEmpty(dtpEnddate.Text) ||
            string.IsNullOrEmpty(txtReason.Text)
                )
            {
                MessageBox.Show("Please fill in all required fields .");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString)) 
            {
                try
                {
                    con.Open();
                    string query_LeaveAppply = "INSERT INTO LeaveReqs (LeaveID,EmployeeID,StartDate,EndDate,LeaveType,Status,Reason,AppliedDate,TotalDays,SLDuration)" +
                                          "VALUES (@LeaveID,@EmployeeID,@StartDate,@EndDate,@LeaveType,@Status,@Reason,@AppliedDate,@TotalDays,@SLDuration)";

                    using (SqlCommand cmd = new SqlCommand(query_LeaveAppply, con))
                    {
                        cmd.Parameters.AddWithValue("@LeaveID",txtLeaveID.Text);
                        cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text);
                        if (dtpStartdate.Value <= DateTime.Now) 
                        {
                            MessageBox.Show("Start date cant be a previous date or today date", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@StartDate", dtpStartdate.Value);
                        }

                        cmd.Parameters.AddWithValue("EndDate",dtpEnddate.Value);
                        
                        string leave="";
                        GetLeaves(txtEmpID.Text);
                        if (rdbAnnual.Checked)
                        {
                            if (remainingAnnualLeaves > totalDays)
                            {
                                remainingAnnualLeaves -= totalDays;
                                leave = "Annual";
                                cmd.Parameters.AddWithValue("@SLDuration", "00:00:00");

                            }
                            else {
                                MessageBox.Show("Not enough remaining casual leaves.", "Error While Applying", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                         else if (rdbCasual.Checked )
                        {
                            if (remainingCasualLeaves > totalDays)
                            {
                                 remainingCasualLeaves -= totalDays;
                                leave = "Casual";
                                cmd.Parameters.AddWithValue("@SLDuration", "00:00:00");

                            }
                            else {
                                MessageBox.Show("Not enough remaining short leaves.", "Error While Applying", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                         else if (rdbShort.Checked)
                        {
                            TimeSpan duration = dtpSleaveTime2.Value - dtpSleaveTime.Value;

                            if (duration != TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(30)))
                            {
                                MessageBox.Show("Short leave duration must be exactly 1 hour and 30 minutes.", "Invalid Duration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }


                            if (remainingShortLeaves > totalDays)
                            {
                                remainingShortLeaves -= totalDays;
                                leave = "Short";

                                // Set SLDuration to the calculated duration in the database
                                cmd.Parameters.AddWithValue("@SLDuration", duration.ToString());
                            }


                        }
                        else
                        {

                            MessageBox.Show("Not enough remaining leaves of the selected type.", "Error While Applying", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;


                        }


                        cmd.Parameters.AddWithValue("@LeaveType",leave);
                        string status = "Pending";
                        cmd.Parameters.AddWithValue("Status",status);
                        cmd.Parameters.AddWithValue("@Reason",txtReason.Text);
                        cmd.Parameters.AddWithValue("@AppliedDate",DateTime.Now);
                        CalculateTotalDays();
                        if (totalDays <= 0) return;
                        cmd.Parameters.AddWithValue("@TotalDays",totalDays);

                        cmd.ExecuteNonQuery();
                        
                        MessageBox.Show("Leave Applied Successfully","Apply Leave",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);



                        leaveupdateinemployeetbl();
                    }
                    
                    RefreshGrid();
                }
                catch (Exception ex){
                    MessageBox.Show("Error:" + ex.Message);
                }
            }

        }









        // get leaves
        private void GetLeaves(string EmployeeID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Anual_leave, Casual_leave, Short_leave FROM EmployeeTBL WHERE EmployeeID = @EmployeeID";  
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        remainingAnnualLeaves = reader.GetInt32(0);  // 0 for Anual_leave
                        remainingCasualLeaves = reader.GetInt32(1);  // 1 for Casual_leave
                        remainingShortLeaves = reader.GetInt32(2);   // 2 for Short_leave
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }


        private void txtEmpID_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmpID.Text))
            {
                GetLeaves(txtEmpID.Text);
            }
        }

        //calculate total dayz of holidyzs

        private void CalculateTotalDays()
        {
            if (endDate < startDate)
            {
                totalDays = 0; 
            }
            else
            {
                totalDays = (int)(endDate - startDate).TotalDays; 
            }
        }


        private void dtpStartdate_ValueChanged(object sender, EventArgs e)
        {
             startDate = dtpStartdate.Value;
             CalculateTotalDays();
        }

        private void dtpEnddate_ValueChanged(object sender, EventArgs e)
        {
            endDate = dtpEnddate.Value;
            CalculateTotalDays();
        }


        // update leaves in employeetable
        private void leaveupdateinemployeetbl()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query_updateempTBL = "UPDATE EmployeeTBL SET Anual_leave = @AnnualLeave, Casual_leave = @CasualLeave, Short_leave = @ShortLeave WHERE EmployeeID = @EmployeeID";
                    SqlCommand cmd = new SqlCommand(query_updateempTBL, con);

                    // Pass the updated leave balances
                    cmd.Parameters.AddWithValue("@AnnualLeave", remainingAnnualLeaves);
                    cmd.Parameters.AddWithValue("@CasualLeave", remainingCasualLeaves);
                    cmd.Parameters.AddWithValue("@ShortLeave", remainingShortLeaves);
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        //Refresh grid
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // Retrieve the leave request  
                    string query_select = "SELECT LeaveType, TotalDays, EmployeeID FROM LeaveReqs WHERE LeaveID = @LeaveID";
                    string leaveType = "";
                    int totalDays = 0;
                    string employeeID = "";

                    //   get the details of the leave request
                    using (SqlCommand cmdSelect = new SqlCommand(query_select, con))
                    {
                        cmdSelect.Parameters.AddWithValue("@LeaveID", txtLeaveID.Text);
                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                leaveType = reader["LeaveType"].ToString();
                                totalDays = reader.GetInt32(reader.GetOrdinal("TotalDays"));
                                employeeID = reader["EmployeeID"].ToString();
                            }
                        }
                    }

                    //  delete the leave request
                    string query_delete = "DELETE FROM LeaveReqs WHERE LeaveID = @LeaveID";
                    using (SqlCommand cmdDelete = new SqlCommand(query_delete, con))
                    {
                        cmdDelete.Parameters.AddWithValue("@LeaveID", txtLeaveID.Text);
                        cmdDelete.ExecuteNonQuery();
                        MessageBox.Show("Leave Deleted Successfully", "Delete Leave", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    // Reverse the deducted leave balance
                    if (!string.IsNullOrEmpty(leaveType) && totalDays > 0)
                    {
                        // Update the employee's leave balance based on the leave type
                        if (leaveType == "Annual")
                        {
                            remainingAnnualLeaves += totalDays;
                        }
                        else if (leaveType == "Casual")
                        {
                            remainingCasualLeaves += totalDays;
                        }
                        else if (leaveType == "Short")
                        {
                            remainingShortLeaves += totalDays;
                        }

                        // Update the employee table with the reversed leave balance
                        leaveupdateinemployeetbl();
                    }

                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtEmpID.Clear();
            txtLeaveID.Clear();
            txtReason.Clear();
            dtpStartdate.Value = DateTime.Now;
            dtpEnddate.Value = DateTime.Now;
            rdbAnnual.Checked = false;
            rdbCasual.Checked = false;
            rdbShort.Checked = false;   
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try {
                    con.Open();
                    string empid = txtEmpID.Text;
                    string query_search = "SELECT LeaveID , EmployeeID FROM LeaveReqs  WHERE EmployeeID= '" + empid + "' ";
                    SqlCommand cmd = new SqlCommand(query_search, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read()) {
                        txtLeaveID.Text=rdr["LeaveID"].ToString();
                        txtEmpID.Text= rdr["EmployeeID"].ToString();
                         

                        
                        
                    }
                }
                catch (Exception ex)  
                {
                    MessageBox.Show("Error While Searching... " + ex.Message ); 
                    con.Close();
                }
            }
        }
    }   
}
