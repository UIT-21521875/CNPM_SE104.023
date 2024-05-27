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
    public partial class FormDangKi : Form
    {

        string chuoiketnoi = @"Data Source=LAPTOP-98F0GEC3;Initial Catalog=QUANLIDAILY;Integrated Security=True";
        SqlConnection ketnoi;
        string sql;
        SqlCommand thuchien;
        SqlDataReader docdulieu;

        public FormDangKi()
        {
            InitializeComponent();
        }

        private void btDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "insert into TAIKHOAN(TenTaiKhoan, MatKhau, LoaiTaiKhoan) values(N'" + txTKDangKy.Text + "', N'" + txMKDangKy.Text + "', " +
                    "N'" + cbcDangKy.Text + "')";
                thuchien.ExecuteNonQuery();
                ketnoi.Close();

                // Hiển thị thông báo đăng ký thành công
                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                FormDangNhap formDangNhap = new FormDangNhap();
                formDangNhap.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi đăng ký
                MessageBox.Show("Đăng ký thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
