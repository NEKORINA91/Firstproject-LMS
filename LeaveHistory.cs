using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace LeaveManagementSystem
{
    public partial class LeaveHistoryViewAdmin : Form
    {
        private string connectionString = @"Data Source=ABDULLAH\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Leave_Management_System";

        public LeaveHistoryViewAdmin()
        {
            InitializeComponent();
        }

        private void LeaveHistoryViewAdmin_Load(object sender, EventArgs e)
        {
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LHAV.rdlc");

            if (!File.Exists(reportPath))
            {
                MessageBox.Show("The report file (LHAV.rdlc) could not be found. Please ensure it is in the application directory.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            reportViewer1.LocalReport.ReportPath = reportPath;
            RefreshReport(); // Load all leave records initially
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string empId = txtEmpID.Text.Trim();
            DateTime startDate = dtpStartdate.Value.Date;
            DateTime endDate = dtpEnddate.Value.Date;

            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Please enter an Employee ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be later than the end date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadLeaves(empId, startDate, endDate);
        }

        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStartdate.Value.Date;
            DateTime endDate = dtpEnddate.Value.Date;

            if (startDate > endDate)
            {
                MessageBox.Show("Start date cannot be later than the end date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadLeavesByDateRange(startDate, endDate);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshReport(); // Reload all records
        }

        private void LoadLeaves(string empId, DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason 
                             FROM LeaveReqs 
                             WHERE EmployeeID = @EmployeeID 
                             AND StartDate >= @StartDate AND EndDate <= @EndDate";

            SqlParameter[] parameters = {
                new SqlParameter("@EmployeeID", empId),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            ExecuteQuery(query, parameters);
        }

        private void LoadLeavesByDateRange(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT LeaveID, EmployeeID, StartDate, EndDate, LeaveType, Status, Reason 
                             FROM LeaveReqs 
                             WHERE StartDate >= @StartDate AND EndDate <= @EndDate";

            SqlParameter[] parameters = {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            ExecuteQuery(query, parameters);
        }

        private void RefreshReport()
        {
            string query = "SELECT * FROM LeaveReqs";
            ExecuteQuery(query, null);
        }

        private void ExecuteQuery(string query, SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No records found for the specified criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            BindDataToReport(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error executing query: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BindDataToReport(DataTable data)
        {
            try
            {
                reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource dataSource = new ReportDataSource("LeaveDataSet", data); // Ensure "LeaveDataSet" matches the dataset name in RDLC
                reportViewer1.LocalReport.DataSources.Add(dataSource);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error binding data to the report: {ex.Message}", "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
