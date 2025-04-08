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
    public partial class LeaveRemain : Form
    {
        private string connectionString = ""; /* Your Databse link*/
        public LeaveRemain()
        {
            InitializeComponent();
        }

        private void LeaveRemain_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet5.EmployeeTBL' table. You can move, or remove it, as needed.
            this.employeeTBLTableAdapter.Fill(this.dataSet5.EmployeeTBL);

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtEmpID.Text)) {
                MessageBox.Show("Employee ID cant be empty"," Error while searchings ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string empid = txtEmpID.Text;

                    // Emblooooos leaves da
                    string query_search = "SELECT * FROM EmployeeTBL WHERE EmployeeID = @EmployeeID";
                    SqlCommand cmd = new SqlCommand(query_search, con);
                    cmd.Parameters.AddWithValue("@EmployeeID", empid);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        
                        dataGridView1.DataSource = dt;

                        
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["EmployeeID"].Value != null && row.Cells["EmployeeID"].Value.ToString() == empid)
                            {
                                row.Selected = true;
                                dataGridView1.FirstDisplayedScrollingRowIndex = row.Index; 
                                break;
                            }
                        }

                        MessageBox.Show("Search Completed", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No record found for the given EmployeeID.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
