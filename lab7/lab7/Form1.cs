using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab7
{
    public partial class Form1 : Form
    {
        private DataTable foodTable;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadCategory();
        }

        private void LoadCategory()
        {
            string connectionString = "server= PC821; database = RestaurantManagement; Integrated Security = true; ";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ID, Name FROM Category";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            // Mà kết nối
            conn.Open();
            // Lấy dữ liệu từ csdl đưa vào DataTable
            adapter.Fill(dt);
            // Đóng kết nối và giải phóng bộ nhớ
            conn.Close();
            conn.Dispose();
            // Đưa dữ liệu vào Combo Box
            cbbCategory.DataSource = dt;
            // Hiển thị tên nhóm sản phẩm
            cbbCategory.DisplayMember = "Name";
            // Nhưng khi lấy giá trị thì lấy ID của nhóm
            cbbCategory.ValueMember = "ID";
        }

        private void cbbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbCategory.SelectedIndex == -1) return;
            string connectionString = "server = PC821; database = RestaurantManagement; Integrated Security = true";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Food WHERE FoodCategoryID = @categoryId";
            // Truyền tham số
            cmd.Parameters.Add("@categoryId", SqlDbType.Int);
            if (cbbCategory.SelectedValue is DataRowView)
            {
                DataRowView rowView = cbbCategory.SelectedValue as DataRowView;
                cmd.Parameters["@categoryId"].Value = rowView["ID"];
            }
            else
            {
                cmd.Parameters["@categoryId"].Value = cbbCategory.SelectedValue;
            }
            // Tạo bộ điều phiều dữ liệu
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            foodTable = new DataTable();
            // Mở kết nối
            conn.Open();
            // Lấy dữ liệu từ csdĩ đưa vào DataTable
            adapter.Fill(foodTable);
            // Đóng kết nối và giải phóng bộ nhớ
            conn.Close();
            conn.Dispose();
            // Đưa dữ liệu vào data gridview
            dgvFoodList.DataSource = foodTable;
            // Tính số lượng mẫu tin
            lblQuantity.Text = foodTable.Rows.Count.ToString();
            lblCatName.Text = cbbCategory.Text;
        }

        private void tsmCalculateQuantity_Click(object sender, EventArgs e)
        {
            string connectionString = "server=PC821; database = RestaurantManagement; Integrated Security = true";
            SqlConnection conn = new SqlConnection(connectionString);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT @numSaleFood = sum(Quantity) FROM BillDetails WHERE FoodID = @foodId";
            // Lấy thông tin sản phẩm được chọn
            if (dgvFoodList.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];

                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;

                // Truyền tham số
                cmd.Parameters.Add("@foodId", SqlDbType.Int);
                cmd.Parameters["@foodId"].Value = rowView["ID"];

                cmd.Parameters.Add("@numSaleFood", SqlDbType.Int);
                cmd.Parameters["@numSaleFood"].Direction = ParameterDirection.Output;
                conn.Open();
                // Thực thì truy vẫn và lấy dữ liệu từ tham số
                cmd.ExecuteNonQuery();
                string result= cmd.Parameters["@numSaleFood"].Value.ToString();
                if(result=="")
                {
                    result = "0";
                }    
                MessageBox.Show("Tống sẽ lượng món"+ rowView["Name"]+ "đã bán là: " + result+ " " +rowView["Unit"]);
                // Đông kết nối csdl
                conn.Close();
            }
                cmd.Dispose();
                conn.Dispose();
            }

        private void tsmAddFood_Click(object sender, EventArgs e)
        {
            fOODInfoForm foodForm=new fOODInfoForm();
            foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);

            foodForm.Show(this);
        }
        void foodForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            int index = cbbCategory.SelectedIndex;
            cbbCategory.SelectedIndex = -1;
            cbbCategory.SelectedIndex = index; 
        }

            private void tsmUpdateFood_Click(object sender, EventArgs e)
        {
            if (dgvFoodList.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                fOODInfoForm foodForm = new fOODInfoForm();
                foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);
                foodForm.Show(this);
                foodForm.DisplayFoodInfo(rowView);
            }
        }

        private void txtSearchByName_TextChanged(object sender, EventArgs e)
        {
            if (foodTable == null) return;

            string filterExpression="Name like '%"+ txtSearchByName.Text+"%'";
            string sortExpression = "Price DESC";
            DataViewRowState rowStateFilter = DataViewRowState.OriginalRows;

            DataView foodView =new DataView(foodTable,filterExpression,sortExpression,rowStateFilter);

            dgvFoodList.DataSource = foodView;
        }
    }
    }
