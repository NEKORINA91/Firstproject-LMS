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
    public partial class Employeeregister : Form
    {
        private string connectionString = ""; /* Your Databse link*/
        private DataTable employeeData = new DataTable();
        public Employeeregister()
        {
            InitializeComponent();
            RefreshGrid ();
        }
        private void RefreshGrid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query_select = "SELECT * FROM EmployeeTBL";   

                    using (SqlCommand cmd = new SqlCommand(query_select, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            employeeData.Clear();  
                            adapter.Fill(employeeData);

                             
                            dataGridView1.DataSource = employeeData;
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while refreshing data: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmpID.Text) ||
            string.IsNullOrEmpty(txtPassword.Text) ||
            string.IsNullOrEmpty(txtPosition.Text) ||
            string.IsNullOrEmpty(txtDepartment.Text) ||
            string.IsNullOrEmpty(txtAleave.Text) ||
            string.IsNullOrEmpty(txtFname.Text) ||
            string.IsNullOrEmpty(txtLname.Text)||
            string.IsNullOrEmpty(txtCleave.Text) ||
            string.IsNullOrEmpty(txtSleave.Text) ||
            string.IsNullOrEmpty(dtpAdate.Text) ||
            string.IsNullOrEmpty(dtpStartTime.Text) || 
            string.IsNullOrEmpty(dtpEndTime.Text)

)
            {
                MessageBox.Show("Please fill in all required fields .");
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query_insert = "INSERT INTO EmployeeTBL (EmployeeID,FirstName,LastName,Department,Position,App_Date,Password,Anual_leave,Casual_leave,Short_leave,S_Time,E_Time)" +
                        "VALUES(@EmployeeID,@FirstName,@LastName,@Department,@Position,@App_Date,@Password,@Anual_leave,@Casual_leave,@Short_leave,@S_Time,@E_Time)";
                    using (SqlCommand cmd = new SqlCommand(query_insert, con)) {
                        cmd.Parameters.AddWithValue("@EmployeeID",txtEmpID.Text);
                        cmd.Parameters.AddWithValue("@FirstName",txtFname.Text);
                        cmd.Parameters.AddWithValue("@LastName",txtLname.Text);
                        cmd.Parameters.AddWithValue("@Department",txtDepartment.Text);
                        cmd.Parameters.AddWithValue("@App_Date",dtpAdate.Value);
                        cmd.Parameters.AddWithValue("@Position",txtPosition.Text);
                        cmd.Parameters.AddWithValue("@Password",txtPassword.Text);
                        cmd.Parameters.AddWithValue("@Anual_leave",txtAleave.Text);
                        cmd.Parameters.AddWithValue("@Casual_leave",txtCleave.Text);
                        cmd.Parameters.AddWithValue("@Short_leave",txtSleave.Text);
                        cmd.Parameters.AddWithValue("@S_Time",dtpStartTime.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@E_Time",dtpEndTime.Value.TimeOfDay);
                        

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Recorded sucessfully", "Redord Insert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        RefreshGrid();
                        return;
                       
                    }
                 
                }
            }
            catch (Exception ex){
                MessageBox.Show("Error: " + ex.Message);


            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    
                    string query_update = "UPDATE EmployeeTBL SET EmployeeID ='"+ txtEmpID.Text + "',FirstName='" + txtFname.Text + "',LastName='" + txtLname.Text +"',Department='" +txtDepartment.Text+ "',Position='" +txtPosition.Text+"',App_Date='"+ dtpAdate.Value+"',Password='"+ txtPassword.Text +"',Anual_leave='"+ int.Parse(txtAleave.Text) +"',Casual_leave='"+ int.Parse(txtCleave.Text)+"',Short_leave='"+ int.Parse(txtSleave.Text)+"' where EmployeeID='"+txtEmpID.Text+"'";
                    SqlCommand cmd = new SqlCommand(query_update,con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data updated sucessfully", "Update Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                    RefreshGrid();
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: " + ex.Message);

                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {

                    con.Open();
                    string query_delete = "DELETE FROM EmployeeTBL WHERE EmployeeID= @EmployeeID";
                    using (SqlCommand cmd = new SqlCommand(query_delete, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Deleted sucessfully", "Delete details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while Deleting" + ex.Message);
                    con.Close();
                }

        }

        private void Employeeregister_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'leave_Management_SystemDataSet2.EmployeeTBL' table. You can move, or remove it, as needed.
            this.employeeTBLTableAdapter1.Fill(this.leave_Management_SystemDataSet2.EmployeeTBL);
            // TODO: This line of code loads data into the 'dataSet1.EmployeeTBL' table. You can move, or remove it, as needed.
            this.employeeTBLTableAdapter.Fill(this.dataSet1.EmployeeTBL);
            //dtp tooo time thingies
             dtpStartTime.Format=DateTimePickerFormat.Custom;
             dtpStartTime.CustomFormat = "HH:mm:ss";
             dtpStartTime.ShowUpDown = true; // no calender thingies

             dtpEndTime.Format=DateTimePickerFormat.Custom;
             dtpEndTime.CustomFormat = "HH:mm:ss";
             dtpEndTime.ShowUpDown = true;// like brefore no calenderz thingi


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string EID = txtEmpID.Text;
                    string query_search = "SELECT * FROM EmployeeTBL WHERE EmployeeID='" + txtEmpID.Text +"'";
                    SqlCommand cmd = new SqlCommand(query_search, con);
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read()) { 
                     txtFname.Text = r["FirstName"].ToString();
                     txtLname.Text= r["LastName"].ToString();
                     txtDepartment.Text=r["Department"].ToString();
                     dtpAdate.Text= r["App_Date"].ToString();
                     txtPassword.Text= r["Password"].ToString();
                     txtAleave.Text= r["Anual_leave"].ToString();
                     txtCleave.Text= r["Casual_leave"].ToString();    
                     txtSleave.Text= r["Short_leave"].ToString();
                     dtpStartTime.Text= r["S_Time"].ToString() ;
                     dtpEndTime.Text= r["E_Time"].ToString() ;
                            
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while Searching " + ex.Message);
                    con.Close();
                }

            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            ADashBoard aDashBoard = new ADashBoard();   
            aDashBoard.Show();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCleave.Clear();
            txtAleave.Clear();
            txtSleave.Clear();
            txtDepartment.Clear();
            txtPassword.Clear();
            txtFname.Clear();
            txtLname.Clear();
            txtPosition.Clear();
            dtpAdate.ResetText();
            txtEmpID.Clear();
            dtpStartTime.ResetText();
            dtpEndTime.ResetText();
        
        }
    }
}
