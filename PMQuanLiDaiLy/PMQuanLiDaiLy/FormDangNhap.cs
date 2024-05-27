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

namespace PMQuanLiDaiLy
{
    public partial class FormDangNhap : Form
    {
        string chuoiketnoi = @"Data Source=LAPTOP-98F0GEC3;Initial Catalog=QUANLIDAILY;Integrated Security=True";
        SqlConnection ketnoi;
        string sql;
        SqlCommand thuchien;
        SqlDataReader docdulieu;

        public FormDangNhap()
        {
            InitializeComponent();
        }

        private void btDangNhap_Click(object sender, EventArgs e)
        {
            string tenTaiKhoan = txTKDangNhap.Text;
            string matKhau = txMKDangNhap.Text;

            try
            {
                using (SqlConnection connection = new SqlConnection(chuoiketnoi))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM TAIKHOAN WHERE TenTaiKhoan = @TenTaiKhoan AND MatKhau = @MatKhau";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenTaiKhoan", tenTaiKhoan);
                        command.Parameters.AddWithValue("@MatKhau", matKhau);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            // Đăng nhập thành công
                            SqlCommand queryLoaiTaiKhoan = new SqlCommand("SELECT LoaiTaiKhoan FROM TAIKHOAN WHERE TenTaiKhoan = @TenTaiKhoan", connection);
                            queryLoaiTaiKhoan.Parameters.AddWithValue("@TenTaiKhoan", tenTaiKhoan);
                            string loaiTaiKhoan = queryLoaiTaiKhoan.ExecuteScalar().ToString();

                            this.Hide();
                            Form1 form1 = new Form1();

                            // Ẩn các nút tương ứng với loại tài khoản
                            if (loaiTaiKhoan == "Nhân viên")
                            {
                                form1.HideButtonsForNhanVien();
                            }

                            form1.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            // Sai thông tin đăng nhập
                            MessageBox.Show("Tên tài khoản hoặc mật khẩu không đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            
            FormDangKi formDangKi = new FormDangKi();
            formDangKi.ShowDialog();
            
        }
    }
}
