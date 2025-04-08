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


// this cam when clicked clean run too scared too remove this 
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LeaveManagementSYstem;
using Microsoft.Reporting.WinForms;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Reporting.Map.WebForms.BingMaps;


namespace LeaveManagementSystem
{
    public partial class LeaveHistoryViewAdmin : Form
    {
        private string connectionString = ""; /* Your Databse link*/
        public LeaveHistoryViewAdmin()
        {
            InitializeComponent();
            LoadLeaves();

        }


        // just too scared to remove this never touch this dont know why it worked at first now it wont
        private void LeaveHistoryViewAdmin_Load(object sender, EventArgs e)
        {
            // tried to connect the whole thing
            reportViewer1.LocalReport.ReportPath = "";
            RefreshReport();
        }







        private void btnSearch_Click(object sender, EventArgs e)
        {
            string empId = txtEmpID.Text.Trim();

            
            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Please enter an Employee ID.");
                return;
            }

            DateTime startDate = dtpStartdate.Value;
            DateTime endDate = dtpEnddate.Value;

            // Date Ranges
            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be later than end date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                
                LoadLeavesByDateRange(startDate, endDate, empId);
            }

        }

        private void LoadLeaves(string empId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {

            }
        }


        private void LoadLeavesByDateRange(DateTime startDate, DateTime endDate, string empId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string empId = txtEmpID.Text.Trim();
                // accidenatlly added this just keep it in case

                try
                {
                    con.Open();
                    
                    string query = "SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason FROM LeaveReqs WHERE EmployeeID = @EmployeeID AND StartDate >= @StartDate AND EndDate <= @EndDate";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EmployeeID", empId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable leaveData = new DataTable();
                        adapter.Fill(leaveData);

                        // Viewer code 
                        reportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource ds = new ReportDataSource("leave", leaveData);
                        reportViewer1.LocalReport.ReportPath = " ";/* location of yourreport + LHAV.rdlc*/

                        reportViewer1.LocalReport.DataSources.Add(ds);
                        reportViewer1.RefreshReport();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading leave requests: " + ex.Message);
                }
            }
        }

       

        

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshReport();
        }

        private void RefreshReport()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM LeaveReqs", con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    // Fill the DataTable with data from the database
                    adapter.Fill(dt);

                    
                    reportViewer1.LocalReport.DataSources.Clear();

                    ReportDataSource ds = new ReportDataSource("leave", dt);
                    reportViewer1.LocalReport.ReportPath = ""; /*location of yourreport +LHAV.rdlc */
                    reportViewer1.LocalReport.DataSources.Add(ds);
                    // Refresh thsi 
                    reportViewer1.RefreshReport();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error refreshing the report: " + ex.Message);
                }
            }
        }


        

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            ADashBoard aDashBoard = new ADashBoard();
            aDashBoard.Show();
        }

        private void LeaveHistoryViewAdmin_Load_2(object sender, EventArgs e)
        {

        }

        private void btnSearchAll_Click_1(object sender, EventArgs e)
        {
            Refresh();
            RefreshReport();
        }
    }
}
