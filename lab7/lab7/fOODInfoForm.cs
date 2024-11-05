using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab7
{
    public partial class fOODInfoForm : Form
    {
        public fOODInfoForm()
        {
            InitializeComponent();
        }

        private void fOODInfoForm_Load(object sender, EventArgs e)
        {
            this.InitValues();
        }
        private void InitValues()
        {
            string connectionString = "server=PC821; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connectionString);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ID, Name FROM Category";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            // mở kết nối
            conn.Open();
            // Lấy dữ liệu từ cái đưa vào DataTable
            adapter.Fill(ds, "Category");
            // Hiển thị nhóm món ăn
            cbbCatName.DataSource = ds.Tables["Category"];
            cbbCatName.DisplayMember = "Name";
            cbbCatName.ValueMember = "ID";
            // đóng kết nối và giải phóng bộ nhớ
            conn.Close();
            conn.Dispose();
        }
        private void ResetText()
        {
            txtID.ResetText();
            txtName.ResetText();
            txtNotes.ResetText();
            txtUnit.ResetText();
            cbbCatName.ResetText();
            nudPrice.ResetText();
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "server=PC821; database = RestaurantManagement; Integrated Security = true; ";
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "EXECUTE InsertFood @id OUTPUT, @name, @unit, @foodCategoryID, @price, @notes";
                // Thêm tham số vào đối tượng Command
                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@foodCategoryId", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.VarChar, 3000);

                cmd.Parameters["@id"].Direction = ParameterDirection.Output;
                // Truyền giá trị vào thủ tục qua tham số
                cmd.Parameters["@name"].Value = txtName.Text;
                cmd.Parameters["@unit"].Value = txtUnit.Text;
                cmd.Parameters["@foodCategoryId"].Value = cbbCatName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;
                // mở kết nổi
                conn.Open();
                int numRowAffected = cmd.ExecuteNonQuery();
                if (numRowAffected > 0)
                {
                    string foodID = cmd.Parameters["@id"].Value.ToString();
                    MessageBox.Show("Successfully adding new food. Food ID=" + foodID, "Message");
                    this.ResetText();
                }
                else
                {
                    MessageBox.Show("Adding food failed");
                }
                // đóng kết nối
                conn.Close();
                conn.Dispose();
            }
            // Bắt lỗi SQL và các lỗi khác
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "SQL Eccer");
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message, "Error");
            }
        }
        public void DisplayFoodInfo(DataRowView rowView)
        {
            try
            {
                txtID.Text = rowView["ID"].ToString();
                txtName.Text = rowView["Name"].ToString();
                txtUnit.Text = rowView["Unit"].ToString();
                txtNotes.Text = rowView["Notes"].ToString();
                nudPrice.Text = rowView["Price"].ToString();
                cbbCatName.SelectedIndex = -1;
                // chọn nhóm món ăn tương ứng
                for (int index = 0; index < cbbCatName.Items.Count; index++)
                {
                    DataRowView cat = cbbCatName.Items[index] as DataRowView;
                    if (cat["ID"].ToString() == rowView["FoodCategoryID"].ToString())
                    {
                        cbbCatName.SelectedIndex = index;
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                this.Close();
            }
        }

        private void btnUpdateFood_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "server=PC821; database = RestaurantManagement; Integrated Security = true";
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "EXECUTE UpdateFood @id, @name, @unit, @foodCategoryID, @price, @notes";
                // Thêm tham số vào đối tượng Command
                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@unit", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@foodCategoryId", SqlDbType.Int);
                cmd.Parameters.Add("@price", SqlDbType.Int);
                cmd.Parameters.Add("@notes", SqlDbType.VarChar, 3000);
                // Truyền giá trị vào thủ tục qua tham số
                cmd.Parameters["@id"].Value = int.Parse(txtID.Text);
                cmd.Parameters["@name"].Value = txtName.Text;
                cmd.Parameters["@unit"].Value = txtUnit.Text;
                cmd.Parameters["@foodCategoryId"].Value = cbbCatName.SelectedValue;
                cmd.Parameters["@price"].Value = nudPrice.Value;
                cmd.Parameters["@notes"].Value = txtNotes.Text;
                // mà kết nổi
                conn.Open();
                int numRowAffected = cmd.ExecuteNonQuery();
                // Thông báo kết quả
                if (numRowAffected > 0)
                {
                    MessageBox.Show("Successfully updating food", "Message");
                    this.ResetText();
                }
                else
                {
                    MessageBox.Show("Updating food failed");
                }

                // đồng kết nối
                conn.Close();
                conn.Dispose();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "SQL Erroс");

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message, "Erroг");
            }
        }

        private void bynCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
