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
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using System.Security.Cryptography;


namespace PMQuanLiDaiLy
{
    public partial class Form1 : Form
    {
        string chuoiketnoi = @"Data Source=LAPTOP-98F0GEC3;Initial Catalog=QUANLIDAILY;Integrated Security=True";
        SqlConnection ketnoi;
        string sql;
        SqlCommand thuchien;
        SqlDataReader docdulieu;


        bool PanelMenuExpand;
        public Form1()
        {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.None;
        }

        //Trượt thanh Menu
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (PanelMenuExpand)
            {
                PanelMenu.Width -= 10;
                if (PanelMenu.Width == PanelMenu.MinimumSize.Width)
                {
                    PanelMenuExpand = false;
                    timer1.Stop();
                }
            }
            else
            {
                PanelMenu.Width += 10;
                if (PanelMenu.Width == PanelMenu.MaximumSize.Width)
                {
                    PanelMenuExpand = true;
                    timer1.Stop();
                }
            }
        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        //Nút ở thanh Menu
        private void btTrangChu_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabTrangChu;
        }

        private void btTiepNhanDL_Click(object sender, EventArgs e)
        {
            txTenDL.Text = "";
            txDienThoai.Text = "";
            txDiaChi.Text = "";
            txEmail.Text = "";

            tabAdmin.SelectedTab = tabTiepNhanDL;
        }

        private void btXuatNhapHang_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabXuatNhapHang;
        }

        private void btDanhSach_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();

                string sql = "select MaMH, TenMH, TenDVT, SoLuongTon, DonGiaNhap, DonGiaXuat " +
                             "from MATHANG, DVT " +
                             "where MATHANG.MaDVT = DVT.MaDVT";

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, ketnoi);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_MH.DataSource = dt;

                sql = "select MaLoaiDaiLy, TenLoaiDaiLy, SoNoToiDa " +
                      "from LOAIDAILY";
                adapter.SelectCommand.CommandText = sql;
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_LDL.DataSource = dt;

                sql = "select MaQuan, TenQuan " +
                      "from QUAN";
                adapter.SelectCommand.CommandText = sql;
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDSQuan.DataSource = dt;

                sql = "select MaDVT, TenDVT " +
                      "from DVT";
                adapter.SelectCommand.CommandText = sql;
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_DVT.DataSource = dt;
            }

            Form1_Load(sender, e);

            tabAdmin.SelectedTab = tabDanhSach;
        }

        private void btThuTien_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);

            tabAdmin.SelectedTab = tabThuTien;
        }

        private void btBaoCao_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);

            tabAdmin.SelectedTab = tabBaoCao;
        }

        private void btCaiDat_Click(object sender, EventArgs e)
        {
            

            tabAdmin.SelectedTab = tabCaiDat;
        }

        private void btTaiKhoan_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabTaiKhoan;
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát khỏi ứng dụng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                FormDangNhap formDangNhap = new FormDangNhap();
                formDangNhap.ShowDialog();
                this.Close();
            }
        }

        // PHÂN QUYỀN 
        public void HideButtonsForNhanVien()
        {
            btThuTien.Visible = false;
            btBaoCao.Visible = false;
            btCaiDat.Visible = false;
            btTaiKhoan.Visible = false;
        }



        //Tiếp nhận đại lý
        private void btLuuDL_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txTenDL.Text))
            {
                MessageBox.Show("Tên đại lý không được bỏ trống", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txDienThoai.Text, "^[0-9]{1,10}$"))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa ký tự số và không quá 10 ký tự", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();

            // Kiểm tra xem đã có đại lý khác cùng tên trong cùng một quận chưa
            string tenDaiLy = txTenDL.Text;
            string maQuan = cbcQuan.SelectedValue.ToString();
            thuchien.CommandText = "SELECT COUNT(*) FROM DAILY WHERE TenDaiLy = @TenDaiLy AND MaQuan = @MaQuan";
            thuchien.Parameters.AddWithValue("@TenDaiLy", tenDaiLy);
            thuchien.Parameters.AddWithValue("@MaQuan", maQuan);
            int countSameNameDL = Convert.ToInt32(thuchien.ExecuteScalar());

            if (countSameNameDL > 0)
            {
                MessageBox.Show("Trong cùng một quận không thể có hai đại lý cùng tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Tiến hành thêm dữ liệu
                thuchien.CommandText = "INSERT INTO DAILY (TenDaiLy, MaLoaiDaiLy, DienThoai, DiaChi, MaQuan, NgayTiepNhan, Email) " +
                    "VALUES (N'" + txTenDL.Text + "', N'" + cbcLoaiDL.SelectedValue.ToString() + "', '" + txDienThoai.Text + "', N'" + txDiaChi.Text + "'," +
                    "N'" + cbcQuan.SelectedValue.ToString() + "', '" + dateNgayTiepNhan.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', N'" + txEmail.Text + @"')";

                try
                {
                    thuchien.ExecuteNonQuery();
                    

                    sql = "select madaily, tendaily, tenloaidaily, dienthoai, diachi, tenquan, NgayTiepNhan, Email, TienNo " +
                        "from daily, loaidaily, quan " +
                        "where daily.MaLoaiDaiLy = LOAIDAILY.MaLoaiDaiLy and daily.MaQuan = quan.MaQuan";
                    thuchien = new SqlCommand(sql, ketnoi);
                    docdulieu = thuchien.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(docdulieu);
                    dgvDSDaiLy.DataSource = dt;

                    FormTBTiepNhanDL formTBTiepNhanDL=new FormTBTiepNhanDL();
                    formTBTiepNhanDL.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Thêm thất bại!\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ketnoi.Close();

        }


             //Thông báo tiếp nhận đại lý thành công
        public void ThemDaiLyKhac()
        {
            txTenDL.Clear();
            txDienThoai.Text = "";
            txDiaChi.Text = "";
            txEmail.Text = "";
        }

        public void ChuyenTabDanhSach()
        {
            tabAdmin.SelectedTab = tabDanhSach; // Chuyển đến tab "tabDanhSach"
        }

        public void ChuyenTabTrangChu()
        {
            tabAdmin.SelectedTab = tabTrangChu; // Chuyển đến tab "tabTrangChu"
        }



        //FORM LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            txSuaSDLTD.Visible = false;
            btLuuSDLTD.Visible = false;
            txSuaTLDG.Visible = false;
            btLuuTLDG.Visible = false;

            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();
                string sql = "select madaily, tendaily, tenloaidaily, dienthoai, diachi, tenquan, NgayTiepNhan, Email, TienNo " +
                    "from daily, loaidaily, quan " +
                    "where daily.MaLoaiDaiLy = LOAIDAILY.MaLoaiDaiLy and daily.MaQuan = quan.MaQuan";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDSDaiLy.DataSource = dt;

                sql = "select MaMH, TenMH, TenDVT, SoLuongTon, DonGiaNhap, DonGiaXuat" +
                    " from MATHANG, DVT " +
                    "where MATHANG.MaDVT = DVT.MaDVT";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_MH.DataSource = dt;

                sql = "select MaLoaiDaiLy, TenLoaiDaiLy, SoNoToiDa " +
                    "from LOAIDAILY";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_LDL.DataSource = dt;

                sql = "select MaQuan, TenQuan " +
                    "from QUAN";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDSQuan.DataSource = dt;

                sql = "select MaDVT, TenDVT " +
                    "from DVT";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_DVT.DataSource = dt;

                sql = "select MaPhieuNhap, NgayNhap, TongTien " +
                    "from PHIEUNHAPHANG";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvPNH.DataSource = dt;

                sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                       "FROM CT_PNH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                       "WHERE ct.id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PNH.DataSource = dt;

                sql = "select MaPhieuXuat, TenDaiLy, NgayXuat, TongTien, SoTienTra, ConLai" +
                    " from PHIEUXUATHANG, DAILY " +
                    "where PHIEUXUATHANG.MaDaiLy = DAILY.MaDaiLy";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvPXH.DataSource = dt;

                sql = "select MaDaiLy, TenDaiLy, MaQuan, TienNo from DAILY where TienNo > 0";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDSNo.DataSource = dt;

                sql = "select MaPhieuThuTien, TenDaiLy, NgayThuTien, SoTienThu " +
                    "from DAILY dl, PHIEUTHUTIEN ptt " +
                    "where dl.MaDaiLy = ptt.MaDaiLy";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvDSPTT.DataSource = dt;

                sql = "select * from TAIKHOAN ";    
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvTaiKhoan.DataSource = dt;

                sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                           "FROM CT_PXH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                           "WHERE ct.id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PXH.DataSource = dt;



                sql = "SELECT SoDaiLyToiDa, TiLeDonGia FROM THAMSO";
                SqlCommand command = new SqlCommand(sql, ketnoi);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int soDaiLyToiDa = reader.GetInt32(0);
                    decimal tiLeDonGia = reader.GetDecimal(1);

                    txSDLTD.Text = soDaiLyToiDa.ToString();
                    txTLDG.Text = tiLeDonGia.ToString();
                }

                reader.Close();

                ketnoi.Close();
            }

            //btLapBCDS.Click += btLapBCDSAnother.Click;
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.PHIEUXUATHANG' table. You can move, or remove it, as needed.
            this.pHIEUXUATHANGTableAdapter.Fill(this.qUANLIDAILYDataSet21.PHIEUXUATHANG);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.PHIEUNHAPHANG' table. You can move, or remove it, as needed.
            this.pHIEUNHAPHANGTableAdapter.Fill(this.qUANLIDAILYDataSet21.PHIEUNHAPHANG);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.DVT' table. You can move, or remove it, as needed.
            this.dVTTableAdapter.Fill(this.qUANLIDAILYDataSet21.DVT);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.MATHANG' table. You can move, or remove it, as needed.
            this.mATHANGTableAdapter.Fill(this.qUANLIDAILYDataSet21.MATHANG);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.QUAN' table. You can move, or remove it, as needed.
            this.qUANTableAdapter1.Fill(this.qUANLIDAILYDataSet21.QUAN);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.LOAIDAILY' table. You can move, or remove it, as needed.
            this.lOAIDAILYTableAdapter1.Fill(this.qUANLIDAILYDataSet21.LOAIDAILY);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet21.DAILY' table. You can move, or remove it, as needed.
            this.dAILYTableAdapter.Fill(this.qUANLIDAILYDataSet21.DAILY);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet1.QUAN' table. You can move, or remove it, as needed.
            this.qUANTableAdapter.Fill(this.qUANLIDAILYDataSet1.QUAN);
            // TODO: This line of code loads data into the 'qUANLIDAILYDataSet.LOAIDAILY' table. You can move, or remove it, as needed.
            this.lOAIDAILYTableAdapter.Fill(this.qUANLIDAILYDataSet.LOAIDAILY);

        }




        // LẬP PHIẾU NHẬP HÀNG
        private void btLapPNH_Click(object sender, EventArgs e)
        {

            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "insert into PHIEUNHAPHANG_PHU(NgayNhap) values (GETDATE()) " +
                "delete from CT_PNH_PHU";
            thuchien.ExecuteNonQuery();

            txTongTien_PNH.Text = "";
            txMaMH_PNH.Text = "";
            txTenMH_PNH.Text = "";
            txSLNhap.Text = "";
            txDonGiaNhap.Text = "";

            Form1_Load(sender, e);

            tabAdmin.SelectedTab = tabLapPNH;

            ketnoi.Close();
        }

        private void btThemMH_PNH_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();

            // Kiểm tra sự tồn tại của sản phẩm trong phiếu nhập
            thuchien.CommandText = "SELECT COUNT(*) FROM CT_PNH_PHU WHERE id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU) AND MaMH = N'" + cbcTK_MH_PNH.SelectedValue.ToString() + "'";
            int count = Convert.ToInt32(thuchien.ExecuteScalar());

            if (count > 0)
            {
                MessageBox.Show("Sản phẩm này đã được bạn thêm vào phiếu nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Thêm sản phẩm vào phiếu nhập
                thuchien.CommandText = "INSERT INTO CT_PNH_PHU (id, MaMH) " +
                    "VALUES ((SELECT MAX(id) FROM PHIEUNHAPHANG_PHU), N'" + cbcTK_MH_PNH.SelectedValue.ToString() + "' )";
                thuchien.ExecuteNonQuery();

                // Lấy dữ liệu chi tiết phiếu nhập và hiển thị trên DataGridView
                string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                    "FROM CT_PNH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                    "WHERE ct.id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PNH.DataSource = dt;
            }

            ketnoi.Close();
        }

        private void btLuuPNH_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "INSERT INTO PHIEUNHAPHANG (NgayNhap, TongTien)" +
                    " SELECT TOP 1 '" + dateLapPNH.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', TongTien " +
                    "FROM PHIEUNHAPHANG_PHU " +
                    "ORDER BY id DESC " +
                    "insert into CT_PNH(MaPhieuNhap, MaMH, SoLuongNhap, DonGiaNhap, ThanhTien) " +
                    "select(select top 1 MaPhieuNhap from PHIEUNHAPHANG order by id desc), MaMH, SoLuongNhap, DonGiaNhap, ThanhTien " +
                    "from CT_PNH_PHU " +
                    "delete from CT_PNH_PHU " +
                    "delete from PHIEUNHAPHANG_PHU";
                thuchien.ExecuteNonQuery();

                string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                       "FROM CT_PNH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                       "WHERE ct.id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PNH.DataSource = dt;

                sql = "select MaPhieuNhap, NgayNhap, TongTien " +
                    "from PHIEUNHAPHANG";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvPNH.DataSource = dt;

                ketnoi.Close();

                txMaMH_PNH.Text = "";
                txTenMH_PNH.Text = "";
                txSLNhap.Text = "";
                txDonGiaNhap.Text = "";
                txTongTien_PNH.Text = "";

                MessageBox.Show("Tạo phiếu nhập hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Chuyển về tab "tabNhapHang" trong tabXNH_CT
                tabXNH_CT.SelectTab("tabNhapHang");

                // Chuyển về tabXuatNhapHang trong tabAdmin
                tabAdmin.SelectTab("tabXuatNhapHang");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvLapCT_PNH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txMaMH_PNH.ReadOnly = true;
            txTenMH_PNH.ReadOnly = true;
            int i;
            i = dgvLapCT_PNH.CurrentRow.Index;
            txMaMH_PNH.Text = dgvLapCT_PNH.Rows[i].Cells[0].Value.ToString();
            txTenMH_PNH.Text = dgvLapCT_PNH.Rows[i].Cells[1].Value.ToString();
            txSLNhap.Text = dgvLapCT_PNH.Rows[i].Cells[2].Value.ToString();
            txDonGiaNhap.Text = dgvLapCT_PNH.Rows[i].Cells[3].Value.ToString();
        }

        private void btSuaMH_PNH_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txSLNhap.Text, out int soLuongNhap) && soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng việc thực hiện nếu số lượng nhập nhỏ hơn 0
            }
            if (decimal.TryParse(txDonGiaNhap.Text, out decimal donGiaNhap) && donGiaNhap < 0)
            {
                MessageBox.Show("Đơn giá nhập không được nhỏ hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng việc thực hiện nếu đơn giá nhập nhỏ hơn 0
            }
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "update CT_PNH_PHU set SoLuongNhap = '" + txSLNhap.Text + "', DonGiaNhap = '" + txDonGiaNhap.Text + "' " +
                "where MaMH = '" + txMaMH_PNH.Text + "' ";
            thuchien.ExecuteNonQuery();

            //Cập nhật lại CT_PNH_PHU
            string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                   "FROM CT_PNH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                   "WHERE ct.id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvLapCT_PNH.DataSource = dt;

            // Cập nhật giá trị TongTien
            string sqlTongTien = "SELECT TongTien FROM PHIEUNHAPHANG_PHU WHERE id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
            SqlCommand cmdTongTien = new SqlCommand(sqlTongTien, ketnoi);
            object result = cmdTongTien.ExecuteScalar();
            if (result != null)
            {
                string tongTien = result.ToString();
                txTongTien_PNH.Text = tongTien;
            }

            ketnoi.Close();
        }

        private void btXoaMH_PNH_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "delete from CT_PNH_PHU " +
                "where MaMH = '" + txMaMH_PNH.Text + "' ";
            thuchien.ExecuteNonQuery();

            //Cập nhật lại bảng CT_PNH_PHU
            string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                   "FROM CT_PNH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                   "WHERE ct.id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvLapCT_PNH.DataSource = dt;

            // Cập nhật giá trị TongTien
            string sqlTongTien = "SELECT TongTien FROM PHIEUNHAPHANG_PHU WHERE id = (SELECT MAX(id) FROM PHIEUNHAPHANG_PHU)";
            SqlCommand cmdTongTien = new SqlCommand(sqlTongTien, ketnoi);
            object result = cmdTongTien.ExecuteScalar();
            if (result != null)
            {
                string tongTien = result.ToString();
                txTongTien_PNH.Text = tongTien;
            }

            ketnoi.Close();
        }

        private void btResetLapPNH_Click(object sender, EventArgs e)
        {
            txMaMH_PNH.Text = "";
            txTenMH_PNH.Text = "";
            txSLNhap.Text = "";
            txDonGiaNhap.Text = "";
        }

        private void dgvPNH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvPNH.CurrentCell.RowIndex;
            string maPN = dgvPNH.Rows[rowIndex].Cells["MaPhieuNhap"].Value.ToString();

            string sql = "SELECT ct.MaPhieuNhap, ct.MaMH, mh.TenMH, ct.SoLuongNhap, ct.DonGiaNhap, ct.ThanhTien " +
                   "FROM CT_PNH ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                   "WHERE ct.MaPhieuNhap = @MaPhieuNhap";

            using (SqlConnection connection = new SqlConnection(chuoiketnoi))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@MaPhieuNhap", maPN);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCT_PNH.DataSource = dt;
            }
        }

        //Tìm kiếm phiếu nhập hàng
        private void btTKPNH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    ketnoi.Open();

                    string sql = "SELECT MaPhieuNhap, NgayNhap, TongTien FROM PHIEUNHAPHANG WHERE 1=1 ";

                    if (checkMaPN.Checked)
                    {
                        sql += "AND MaPhieuNhap = '" + cbcTKMaPNH.Text + "' ";
                    }
                    if (checkNgayNhap.Checked)
                    {
                        sql += "AND CONVERT(date, NgayNhap) = '" + dateTKPNH.Value.Date.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (checkKhoangNgayNhap.Checked)
                    {
                        sql += "AND CONVERT(date, NgayNhap) >= '" + dateTKPNHmin.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(date, NgayNhap) <= '" + dateTKPNHmax.Value.Date.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (checkTongTien.Checked)
                    {
                        sql += "AND TongTien = '" + txTKTongTien.Text + "' ";
                    }
                    if (checkKhoangTT.Checked)
                    {
                        if (!string.IsNullOrEmpty(txTKTongTienmin.Text))
                        {
                            sql += "AND TongTien >= " + txTKTongTienmin.Text.Replace("'", "''") + " ";
                        }
                        if (!string.IsNullOrEmpty(txTKTongTienmax.Text))
                        {
                            sql += "AND TongTien <= " + txTKTongTienmax.Text.Replace("'", "''") + " ";
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvPNH.DataSource = dt;

                    ketnoi.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btResetTKPNH_Click(object sender, EventArgs e)
        {
            checkMaPN.Checked = false;
            checkNgayNhap.Checked = false;
            checkKhoangNgayNhap.Checked = false;
            checkTongTien.Checked = false;
            checkKhoangTT.Checked = false;

            dateTKPNHmin.Value = DateTime.Now.Date;
            dateTKPNH.Value = DateTime.Now.Date;
            dateTKPNHmax.Value = DateTime.Now.Date;
            txTKTongTien.Text = "";
            txTKTongTienmin.Text = "";
            txTKTongTienmax.Text = "";

            Form1_Load(sender, e);
        }


        //LẬP PHIẾU XUẤT HÀNG
        //Nhấn phiếu xuất hàng hiện chi tiết của phiếu xuất hàng đó
        private void dgvPXH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvPXH.CurrentCell.RowIndex;
            string maPX = dgvPXH.Rows[rowIndex].Cells["MaPhieuXuat"].Value.ToString();

            string sql = "SELECT ct.MaPhieuXuat, ct.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                         "FROM CT_PXH ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                         "WHERE ct.MaPhieuXuat = @MaPhieuXuat";

            using (SqlConnection connection = new SqlConnection(chuoiketnoi))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@MaPhieuXuat", maPX);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCT_PXH.DataSource = dt;
            }
        }

        private void btLapPXH_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "insert into PHIEUXUATHANG_PHU(NgayXuat) values (GETDATE()) " +
                "delete from CT_PXH_PHU ";
            thuchien.ExecuteNonQuery();

            txMaMH_PXH.Text = "";
            txTenMH_PXH.Text = "";
            txSLXuat.Text = "";
            txDonGiaXuat.Text = "";
            txTongTien_PXH.Text = "";
            txSoTienTra.Text = "";
            txConLai.Text = "";

            Form1_Load(sender, e);

            tabAdmin.SelectedTab = tabLapPXH;

            ketnoi.Close();
        }

        // chuyển dữ liệu ở dgvLapCT_PXH sang nơi chỉnh sửa mặt hàng
        private void dgvLapCT_PXH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txMaMH_PXH.ReadOnly = true;
            txTenMH_PXH.ReadOnly = true;

            int rowIndex = dgvLapCT_PXH.CurrentRow.Index;
            txMaMH_PXH.Text = dgvLapCT_PXH.Rows[rowIndex].Cells["MaMH"].Value.ToString();
            txTenMH_PXH.Text = dgvLapCT_PXH.Rows[rowIndex].Cells["TenMH"].Value.ToString();
            txSLXuat.Text = dgvLapCT_PXH.Rows[rowIndex].Cells["SoLuongXuat"].Value.ToString();
            txDonGiaXuat.Text = dgvLapCT_PXH.Rows[rowIndex].Cells["DonGiaXuat"].Value.ToString();
        }

        private void btThemMH_PXH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();

                // Kiểm tra số lượng tồn của sản phẩm
                using (SqlCommand checkCommand = ketnoi.CreateCommand())
                {
                    checkCommand.CommandText = "SELECT SoLuongTon FROM MATHANG WHERE MaMH = N'" + cbcTK_MH_PXH.SelectedValue.ToString() + "'";
                    int soLuongTon = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (soLuongTon <= 0)
                    {
                        MessageBox.Show("Mặt hàng này đã hết hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Kiểm tra sự tồn tại của sản phẩm trong phiếu xuất
                        using (SqlCommand existsCommand = ketnoi.CreateCommand())
                        {
                            existsCommand.CommandText = "SELECT COUNT(*) FROM CT_PXH_PHU WHERE id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU) AND MaMH = N'" + cbcTK_MH_PXH.SelectedValue.ToString() + "'";
                            int count = Convert.ToInt32(existsCommand.ExecuteScalar());

                            if (count > 0)
                            {
                                MessageBox.Show("Sản phẩm này đã được bạn thêm vào phiếu xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                // Thêm sản phẩm vào phiếu xuất
                                using (SqlCommand insertCommand = ketnoi.CreateCommand())
                                {
                                    insertCommand.CommandText = "INSERT INTO CT_PXH_PHU (id, MaMH, DonGiaXuat) " +
                                        "VALUES ((SELECT MAX(id) FROM PHIEUXUATHANG_PHU), N'" + cbcTK_MH_PXH.SelectedValue.ToString() + "', " +
                                        "(SELECT DonGiaXuat FROM MATHANG WHERE MaMH = N'" + cbcTK_MH_PXH.SelectedValue.ToString() + "'))";
                                    insertCommand.ExecuteNonQuery();
                                }

                                // Lấy dữ liệu chi tiết phiếu xuất và hiển thị trên DataGridView
                                string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                                    "FROM CT_PXH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                                    "WHERE ct.id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";

                                using (SqlCommand selectCommand = new SqlCommand(sql, ketnoi))
                                {
                                    using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
                                    {
                                        DataTable dt = new DataTable();
                                        adapter.Fill(dt);
                                        dgvLapCT_PXH.DataSource = dt;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btSuaMH_PXH_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txSLXuat.Text, out int soLuongXuat) && soLuongXuat <= 0)
            {
                MessageBox.Show("Số lượng xuất phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng việc thực hiện nếu số lượng xuất nhỏ hơn 0
            }
            if (decimal.TryParse(txDonGiaXuat.Text, out decimal donGiaXuat) && donGiaXuat < 0)
            {
                MessageBox.Show("Đơn giá xuất không được nhỏ hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng việc thực hiện nếu đơn giá nhập nhỏ hơn 0
            }
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "update CT_PXH_PHU set SoLuongXuat = '" + txSLXuat.Text + "', DonGiaXuat = '" + txDonGiaXuat.Text + "' " +
                "where MaMH = '" + txMaMH_PXH.Text + "' ";

            try
            {
                thuchien.ExecuteNonQuery();

                //Cập nhật lại CT_PXH_PHU
                string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                       "FROM CT_PXH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                       "WHERE ct.id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PXH.DataSource = dt;

                // Cập nhật giá trị TongTien
                string sqlTongTien = "SELECT TongTien FROM PHIEUXUATHANG_PHU WHERE id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
                SqlCommand cmdTongTien = new SqlCommand(sqlTongTien, ketnoi);
                object result = cmdTongTien.ExecuteScalar();
                if (result != null)
                {
                    string tongTien = result.ToString();
                    txTongTien_PXH.Text = tongTien;
                }
            }
            catch (SqlException ex)
            {
                // Xử lý ngoại lệ SqlException để hiển thị thông báo từ trigger
                string triggerErrorMessage = "Không thể thực hiện thao tác. Số lượng xuất vượt quá số lượng tồn";
                if (ex.Message.Contains(triggerErrorMessage))
                {
                    MessageBox.Show(triggerErrorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Xử lý ngoại lệ khác nếu cần thiết
                }
            }
            finally
            {
                ketnoi.Close();
            }
        }

        private void btXoaMH_PXH_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "delete from CT_PXH_PHU " +
                "where MaMH = '" + txMaMH_PXH.Text + "' ";
            thuchien.ExecuteNonQuery();

            //Cập nhật lại bảng CT_PNH_PHU
            string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                   "FROM CT_PXH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                   "WHERE ct.id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvLapCT_PXH.DataSource = dt;

            // Cập nhật giá trị TongTien
            string sqlTongTien = "SELECT TongTien FROM PHIEUXUATHANG_PHU WHERE id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
            SqlCommand cmdTongTien = new SqlCommand(sqlTongTien, ketnoi);
            object result = cmdTongTien.ExecuteScalar();
            if (result != null)
            {
                string tongTien = result.ToString();
                txTongTien_PXH.Text = tongTien;
            }
        }

        private void btResetLapPXH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();

                // Lấy id lớn nhất từ bảng PHIEUXUATHANG_PHU
                using (SqlCommand maxIdCommand = ketnoi.CreateCommand())
                {
                    maxIdCommand.CommandText = "SELECT MAX(id) FROM PHIEUXUATHANG_PHU";
                    int maxId = Convert.ToInt32(maxIdCommand.ExecuteScalar());

                    // Cập nhật giá trị SoTienTra
                    using (SqlCommand updateCommand = ketnoi.CreateCommand())
                    {
                        updateCommand.CommandText = "UPDATE PHIEUXUATHANG_PHU SET SoTienTra = @SoTienTra WHERE id = @Id";
                        updateCommand.Parameters.AddWithValue("@SoTienTra", txSoTienTra.Text);
                        updateCommand.Parameters.AddWithValue("@Id", maxId);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Lấy giá trị ConLai và hiển thị lên txConLai
                    using (SqlCommand conLaiCommand = ketnoi.CreateCommand())
                    {
                        conLaiCommand.CommandText = "SELECT ConLai FROM PHIEUXUATHANG_PHU WHERE id = @Id";
                        conLaiCommand.Parameters.AddWithValue("@Id", maxId);
                        object result = conLaiCommand.ExecuteScalar();
                        if (result != null)
                        {
                            string conLai = result.ToString();
                            txConLai.Text = conLai;
                        }
                    }
                }
            }
        }

        private void btLuuPXH_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                SqlCommand thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "insert into PHIEUXUATHANG(MaDaiLy, NgayXuat, TongTien, SoTienTra, ConLai) " +
                    "select top 1 '" + cbcTK_DL_PXH.SelectedValue.ToString() + "', '" + dateLapPXH.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', TongTien, SoTienTra, ConLai " +
                    "from PHIEUXUATHANG_PHU " +
                    "order by id DESC " +
                    "insert into CT_PXH(MaPhieuXuat, MaMH, SoLuongXuat, DonGiaXuat, ThanhTien) " +
                    "select(select top 1 MaPhieuXuat from PHIEUXUATHANG order by id DESC), MaMH, SoLuongXuat, DonGiaXuat, ThanhTien " +
                    "from CT_PXH_PHU " +
                    "delete from CT_PXH_PHU " +
                    "delete from PHIEUXUATHANG_PHU";
                thuchien.ExecuteNonQuery();

                string sql = "SELECT mh.MaMH, mh.TenMH, ct.SoLuongXuat, ct.DonGiaXuat, ct.ThanhTien " +
                           "FROM CT_PXH_PHU ct INNER JOIN MATHANG mh ON ct.MaMH = mh.MaMH " +
                           "WHERE ct.id = (SELECT MAX(id) FROM PHIEUXUATHANG_PHU)";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvLapCT_PXH.DataSource = dt;

                sql = "select MaPhieuXuat, TenDaiLy, NgayXuat, TongTien, SoTienTra, ConLai" +
                        " from PHIEUXUATHANG, DAILY " +
                        "where PHIEUXUATHANG.MaDaiLy = DAILY.MaDaiLy";
                adapter = new SqlDataAdapter(sql, ketnoi);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvPXH.DataSource = dt;

                ketnoi.Close();

                txMaMH_PXH.Text = "";
                txTenMH_PXH.Text = "";
                txSLXuat.Text = "";
                txDonGiaXuat.Text = "";
                txTongTien_PXH.Text = "";
                txSoTienTra.Text = "";
                txConLai.Text = "";

                // Chuyển về tab "tabXuatHang" trong tabXNH_CT
                tabXNH_CT.SelectTab("tabXuatHang");

                // Chuyển về tabXuatNhapHang trong tabAdmin
                tabAdmin.SelectTab("tabXuatNhapHang");

                // Thông báo thành công
                MessageBox.Show("Tạo phiếu xuất hàng thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                string triggerErrorMessage = "Không thể tạo phiếu xuất hàng. Tiền Nợ vượt quá Số Nợ Tối Đa";
                if (ex.Message.Contains(triggerErrorMessage))
                {
                    // Lấy số tiền nợ hiện tại
                    SqlCommand cmdCurrentDebt = new SqlCommand("SELECT TienNo FROM DAILY WHERE MaDaiLy = @MaDaiLy", ketnoi);
                    cmdCurrentDebt.Parameters.AddWithValue("@MaDaiLy", cbcTK_DL_PXH.SelectedValue.ToString());
                    decimal currentDebt = (decimal)cmdCurrentDebt.ExecuteScalar();

                    // Hiển thị thông báo lỗi kèm số tiền nợ hiện tại
                    MessageBox.Show(triggerErrorMessage + ". Hiện đang nợ: " + currentDebt.ToString() + " VNĐ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }


        //Tìm kiếm phiếu xuất hàng
        private void btTKPXH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    ketnoi.Open();

                    string sql = "SELECT MaPhieuXuat, TenDaiLy, NgayXuat, TongTien, SoTienTra, ConLai " +
                        "FROM PHIEUXUATHANG, DAILY " +
                        "WHERE PHIEUXUATHANG.MaDaiLy = DAILY.MaDaiLy ";

                    if (checkMaPX.Checked)
                    {
                        sql += "AND MaPhieuXuat = '" + cbcTKMaPXH.Text + "' ";
                    }
                    if (checkTenDL_PX.Checked)
                    {
                        sql += "AND TenDaiLy = N'" + cbcTKTenDL.Text + "' ";
                    }
                    if (checkNgayXuat.Checked)
                    {
                        sql += "AND CONVERT(date, NgayXuat) = '" + dateTKNgayXuat.Value.Date.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (checkKhoangNgayXuat.Checked)
                    {
                        sql += "AND CONVERT(date, NgayXuat) >= '" + dateTKNgayXuatmin.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(date, NgayXuat) <= '" + dateTKNgayXuatmax.Value.Date.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (checkTongTien_PX.Checked)
                    {
                        sql += "AND TongTien = '" + txTKTongTienPXH.Text + "' ";
                    }
                    if (checkKhoangTongTien.Checked)
                    {
                        if (!string.IsNullOrEmpty(txTKTongTienPXHmin.Text))
                        {
                            sql += "AND TongTien >= '" + txTKTongTienPXHmin.Text.Replace("'", "''") + "' ";
                        }
                        if (!string.IsNullOrEmpty(txTKTongTienPXHmax.Text))
                        {
                            sql += "AND TongTien <= '" + txTKTongTienPXHmax.Text.Replace("'", "''") + "' ";
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvPXH.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btResetPXH_Click(object sender, EventArgs e)
        {
            checkMaPX.Checked = false;
            checkTenDL_PX.Checked = false;
            checkNgayXuat.Checked = false;
            checkKhoangNgayXuat.Checked = false;
            checkTongTien_PX.Checked = false;
            checkKhoangTongTien.Checked = false;

            dateTKNgayXuat.Value = DateTime.Now.Date;
            dateTKNgayXuatmax.Value = DateTime.Now.Date;
            dateTKNgayXuatmin.Value = DateTime.Now.Date;

            txTKTongTienPXH.Text = "";
            txTKTongTienPXHmax.Text = "";
            txTKTongTienPXHmin.Text = "";

            Form1_Load(sender, e);
        }




        //DANH SÁCH

        //Danh sách loại đại lý
        private void bt_TK_DSLDL_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();

            string sql = "SELECT * FROM LOAIDAILY WHERE 1 = 1 ";

            if (checkTenLDL.Checked)
            {
                sql += "AND TenLoaiDaiLy = @TenLoaiDaiLy ";
            }
            if (checkSNTD.Checked)
            {
                sql += "AND SoNoToiDa = @SoNoToiDa ";
            }
            if (checkKhoangSNTD.Checked)
            {
                if (!string.IsNullOrEmpty(txSoNoToiDamin_DS.Text))
                {
                    sql += "AND SoNoToiDa >= @SoNoToiDaMin ";
                }
                if (!string.IsNullOrEmpty(txSoNoToiDamax_DS.Text))
                {
                    sql += "AND SoNoToiDa <= @SoNoToiDaMax ";
                }
            }

            SqlCommand command = new SqlCommand(sql, ketnoi);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            if (checkTenLDL.Checked)
            {
                command.Parameters.AddWithValue("@TenLoaiDaiLy", cbcTenLDL_DS.Text);
            }
            if (checkSNTD.Checked)
            {
                command.Parameters.AddWithValue("@SoNoToiDa", txSoNoToiDa_DS.Text);
            }
            if (checkKhoangSNTD.Checked)
            {
                if (!string.IsNullOrEmpty(txSoNoToiDamin_DS.Text))
                {
                    command.Parameters.AddWithValue("@SoNoToiDaMin", txSoNoToiDamin_DS.Text);
                }
                if (!string.IsNullOrEmpty(txSoNoToiDamax_DS.Text))
                {
                    command.Parameters.AddWithValue("@SoNoToiDaMax", txSoNoToiDamax_DS.Text);
                }
            }

            try
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_LDL.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Điều kiện bạn nhập không hợp lệ. Chi tiết lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ketnoi.Close();
            }
        }

        private void btReset_DSLDL_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();
                string sql = "select * from loaidaily";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_LDL.DataSource = dt;
            }

            checkTenLDL.Checked = false;
            checkSNTD.Checked = false;
            checkKhoangSNTD.Checked = false;
            txSoNoToiDa_DS.Text = "";
            txSoNoToiDamin_DS.Text = "";
            txSoNoToiDamax_DS.Text = "";
        }

        private void dgvDS_LDL_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txMaLDL_DS.ReadOnly = true;

            int rowIndex = dgvDS_LDL.CurrentRow.Index;
            txMaLDL_DS.Text = dgvDS_LDL.Rows[rowIndex].Cells["MaLoaiDaiLy"].Value.ToString();
            txTenLDL_DS.Text = dgvDS_LDL.Rows[rowIndex].Cells["TenLoaiDaiLy"].Value.ToString();
            txSNTD_DS.Text = dgvDS_LDL.Rows[rowIndex].Cells["SoNoToiDa"].Value.ToString();
        }

        

        private void btXoaLDL_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "DELETE FROM LOAIDAILY WHERE MaLoaiDaiLy = @MaLoaiDaiLy";

            // Thêm tham số cho câu lệnh truy vấn
            thuchien.Parameters.AddWithValue("@MaLoaiDaiLy", txMaLDL_DS.Text);

            try
            {
                int rowsAffected = thuchien.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa loại đại lý thành công.");
                    Form1_Load(sender, e); // Tải lại dữ liệu sau khi xóa thành công
                }
                else
                {
                    MessageBox.Show("Không tìm thấy loại đại lý để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa loại đại lý: " + ex.Message);
            }
            finally
            {
                ketnoi.Close();
            }
        }

        private void btSuaLDL_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();

            // Kiểm tra tên đại lý đã tồn tại
            string checkQuery = "SELECT COUNT(*) FROM LOAIDAILY WHERE TenLoaiDaiLy = @TenLoaiDaiLy AND MaLoaiDaiLy <> @MaLoaiDaiLy";
            SqlCommand checkCommand = new SqlCommand(checkQuery, ketnoi);
            checkCommand.Parameters.AddWithValue("@TenLoaiDaiLy", txTenLDL_DS.Text);
            checkCommand.Parameters.AddWithValue("@MaLoaiDaiLy", txMaLDL_DS.Text);
            int count = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (count > 0)
            {
                MessageBox.Show("Tên đại lý đã tồn tại. Vui lòng chọn tên khác.");
                ketnoi.Close();
                return;
            }

            // Tiến hành sửa loại đại lý
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "UPDATE LOAIDAILY SET TenLoaiDaiLy = @TenLoaiDaiLy, SoNoToiDa = @SoNoToiDa WHERE MaLoaiDaiLy = @MaLoaiDaiLy";

            // Thêm tham số cho câu lệnh truy vấn
            thuchien.Parameters.AddWithValue("@TenLoaiDaiLy", txTenLDL_DS.Text);
            thuchien.Parameters.AddWithValue("@SoNoToiDa", txSNTD_DS.Text);
            thuchien.Parameters.AddWithValue("@MaLoaiDaiLy", txMaLDL_DS.Text);

            try
            {
                int rowsAffected = thuchien.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Sửa loại đại lý thành công.");
                    Form1_Load(sender, e); // Tải lại dữ liệu sau khi sửa thành công
                }
                else
                {
                    MessageBox.Show("Không tìm thấy loại đại lý để sửa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa loại đại lý: " + ex.Message);
            }
            finally
            {
                ketnoi.Close();
            }
        }

        private void btResetThaoTac_LDL_Click(object sender, EventArgs e)
        {
            txTenLDL_DS.Text = "";
            txSNTD_DS.Text = "";
            txMaLDL_DS.Text = "";
        }

        //Danh sách quận
        private void btTK_Quan_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sql = "SELECT * FROM QUAN WHERE TenQuan = @TenQuan";
                SqlCommand command = new SqlCommand(sql, ketnoi);
                command.Parameters.AddWithValue("@TenQuan", cbcDSTenQuan.Text);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDSQuan.DataSource = dt;

                ketnoi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btResetTK_Quan_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void dgvDSQuan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvDSQuan.CurrentRow.Index;
            txMaQuan.Text = dgvDSQuan.Rows[rowIndex].Cells["MaQuan"].Value.ToString();
            txTenQuan.Text = dgvDSQuan.Rows[rowIndex].Cells["TenQuan"].Value.ToString();
        }

       

        private void btXoaQuan_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sqlCheckExist = "SELECT COUNT(*) FROM QUAN WHERE MaQuan = @MaQuan";
                SqlCommand checkExistCommand = new SqlCommand(sqlCheckExist, ketnoi);
                checkExistCommand.Parameters.AddWithValue("@MaQuan", txMaQuan.Text);

                int existCount = (int)checkExistCommand.ExecuteScalar();
                if (existCount == 0)
                {
                    MessageBox.Show("Không tìm thấy quận có mã quận tương ứng. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sqlDelete = "DELETE FROM QUAN WHERE MaQuan = @MaQuan";
                SqlCommand deleteCommand = new SqlCommand(sqlDelete, ketnoi);
                deleteCommand.Parameters.AddWithValue("@MaQuan", txMaQuan.Text);
                deleteCommand.ExecuteNonQuery();

                ketnoi.Close();

                Form1_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa quận: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSuaQuan_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sqlCheckExist = "SELECT COUNT(*) FROM QUAN WHERE TenQuan = @TenQuan AND MaQuan != @MaQuan";
                SqlCommand checkExistCommand = new SqlCommand(sqlCheckExist, ketnoi);
                checkExistCommand.Parameters.AddWithValue("@TenQuan", txTenQuan.Text);
                checkExistCommand.Parameters.AddWithValue("@MaQuan", txMaQuan.Text);

                int existCount = (int)checkExistCommand.ExecuteScalar();
                if (existCount > 0)
                {
                    MessageBox.Show("Tên quận đã tồn tại. Vui lòng chọn tên quận khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sqlUpdate = "UPDATE QUAN SET TenQuan = @TenQuan WHERE MaQuan = @MaQuan";
                SqlCommand updateCommand = new SqlCommand(sqlUpdate, ketnoi);
                updateCommand.Parameters.AddWithValue("@TenQuan", txTenQuan.Text);
                updateCommand.Parameters.AddWithValue("@MaQuan", txMaQuan.Text);
                updateCommand.ExecuteNonQuery();

                ketnoi.Close();

                Form1_Load(sender, e);

                MessageBox.Show("Đã cập nhật thông tin quận thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình cập nhật thông tin quận: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btResetThaoTac_Quan_Click(object sender, EventArgs e)
        {
            txMaQuan.Text = "";
            txTenQuan.Text = "";
        }

        //Danh sách đơn vị tính
        private void btTKDVT_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sql = "SELECT * FROM DVT WHERE TenDVT = @TenDVT";
                SqlCommand command = new SqlCommand(sql, ketnoi);
                command.Parameters.AddWithValue("@TenDVT", cbcTK_TenDVT.Text);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDS_DVT.DataSource = dt;

                ketnoi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình tìm kiếm đơn vị tính: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btResetTKDVT_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void dgvDS_DVT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvDS_DVT.CurrentRow.Index;
            txMaDVT.Text = dgvDS_DVT.Rows[rowIndex].Cells["MaDVT"].Value.ToString();
            txTenDVT_DS.Text = dgvDS_DVT.Rows[rowIndex].Cells["TenDVT"].Value.ToString();
        }

        

        private void btXoaDVT_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sql = "DELETE FROM DVT WHERE MaDVT = @MaDVT";
                SqlCommand command = new SqlCommand(sql, ketnoi);
                command.Parameters.AddWithValue("@MaDVT", txMaDVT.Text);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa đơn vị tính thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ketnoi.Close();

                    Form1_Load(sender, e);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đơn vị tính có mã tương ứng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa đơn vị tính: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSuaDVT_Click(object sender, EventArgs e)
        {
            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra tên đơn vị tính đã tồn tại
                string checkSql = "SELECT COUNT(*) FROM DVT WHERE TenDVT = @TenDVT AND MaDVT <> @MaDVT";
                SqlCommand checkCommand = new SqlCommand(checkSql, ketnoi);
                checkCommand.Parameters.AddWithValue("@TenDVT", txTenDVT_DS.Text);
                checkCommand.Parameters.AddWithValue("@MaDVT", txMaDVT.Text);
                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Tên đơn vị tính đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Tiến hành sửa đơn vị tính
                    string sql = "UPDATE DVT SET TenDVT = N'" + txTenDVT_DS.Text + "' WHERE MaDVT = '" + txMaDVT.Text + "'";
                    SqlCommand updateCommand = new SqlCommand(sql, ketnoi);
                    updateCommand.ExecuteNonQuery();

                    MessageBox.Show("Sửa đơn vị tính thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form1_Load(sender, e);
                }

                ketnoi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình sửa đơn vị tính: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btResetThaoTac_DVT_Click(object sender, EventArgs e)
        {
            txTenDVT_DS.Text = "";
            txMaDVT.Text = "";
        }

        //Danh sách đại lý
        private void btTK_DSDL_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                {
                    ketnoi.Open();

                    string sql = "SELECT madaily, tendaily, tenloaidaily, dienthoai, diachi, tenquan, NgayTiepNhan, Email, TienNo " +
                                 "FROM daily, loaidaily, quan " +
                                 "WHERE daily.MaLoaiDaiLy = LOAIDAILY.MaLoaiDaiLy AND daily.MaQuan = quan.MaQuan ";

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (checkTenDL.Checked)
                    {
                        sql += "AND TenDaiLy = @TenDaiLy ";
                        parameters.Add(new SqlParameter("@TenDaiLy", cbcTenDL_DS.Text));
                    }
                    if (checkLDL.Checked)
                    {
                        sql += "AND TenLoaiDaiLy = @TenLoaiDaiLy ";
                        parameters.Add(new SqlParameter("@TenLoaiDaiLy", cbcTenLDL_DS.Text));
                    }
                    if (checkDienThoai.Checked)
                    {
                        sql += "AND DienThoai = @DienThoai ";
                        parameters.Add(new SqlParameter("@DienThoai", cbcDT_DS.Text));
                    }
                    if (checkTenQuan.Checked)
                    {
                        sql += "AND TenQuan = @TenQuan ";
                        parameters.Add(new SqlParameter("@TenQuan", cbcTenQuan_DS.Text));
                    }
                    if (checkDiaChi.Checked)
                    {
                        sql += "AND DiaChi = @DiaChi ";
                        parameters.Add(new SqlParameter("@DiaChi", cbcDiaChi_DS.Text));
                    }
                    if (checkDateTiepNhan.Checked)
                    {
                        sql += "AND CONVERT(DATE, NgayTiepNhan) = @NgayTiepNhan ";
                        parameters.Add(new SqlParameter("@NgayTiepNhan", dateNgayTiepNhan_DS.Value.Date));
                    }
                    if (checkTongNo.Checked)
                    {
                        sql += "AND TienNo = @TongNo ";
                        parameters.Add(new SqlParameter("@TongNo", Convert.ToDecimal(txTongNo_DS.Text)));
                    }
                    if (checkKhoangTN.Checked)
                    {
                        if (!string.IsNullOrEmpty(txTongNomin_DS.Text))
                        {
                            sql += "AND TienNo >= @TongNoMin ";
                            parameters.Add(new SqlParameter("@TongNoMin", Convert.ToDecimal(txTongNomin_DS.Text)));
                        }
                        if (!string.IsNullOrEmpty(txTongNomax_DS.Text))
                        {
                            sql += "AND TienNo <= @TongNoMax ";
                            parameters.Add(new SqlParameter("@TongNoMax", Convert.ToDecimal(txTongNomax_DS.Text)));
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                    adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvDSDaiLy.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại đây, ví dụ:
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btReset_DSDL_Click(object sender, EventArgs e)
        {
            checkDateTiepNhan.Checked = false;
            checkDiaChi.Checked = false;
            checkDienThoai.Checked = false;
            checkTenDL.Checked = false;
            checkLDL.Checked = false;
            checkTenQuan.Checked = false;
            checkTongNo.Checked = false;
            checkKhoangTN.Checked = false;
            txTongNo_DS.Text = "";
            txTongNomin_DS.Text = "";
            txTongNomax_DS.Text = "";

            Form1_Load(sender, e);
        }

        private void btThemDL_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabTiepNhanDL;
        }

        private void btXoaDL_Click(object sender, EventArgs e)
        {
            if (dgvDSDaiLy.SelectedRows.Count > 0)
            {
                // Lấy giá trị cột MaDaiLy từ hàng được chọn
                string maDaiLy = dgvDSDaiLy.SelectedRows[0].Cells["MaDaiLy"].Value.ToString();

                // Hỏi người dùng xác nhận xóa đại lý
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đại lý này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                        {
                            ketnoi.Open();

                            // Xây dựng câu lệnh SQL xóa đại lý dựa trên MaDaiLy
                            string sql = "DELETE FROM DaiLy WHERE MaDaiLy = @MaDaiLy";
                            SqlCommand command = new SqlCommand(sql, ketnoi);
                            command.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                            // Thực thi câu lệnh SQL
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Đã xóa đại lý thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Cập nhật lại dữ liệu trong DataGridView 
                                Form1_Load(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy đại lý có mã tương ứng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đại lý để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btSuaDL_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabSuaDL;
        }
        private void dgvDSDaiLy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvDSDaiLy.CurrentRow.Index;
            txMaDL.Text = dgvDSDaiLy.Rows[rowIndex].Cells["MaDaiLy"].Value.ToString();
            txSuaTenDL.Text = dgvDSDaiLy.Rows[rowIndex].Cells["TenDaiLy"].Value.ToString();
            cbSuaLDL.Text = dgvDSDaiLy.Rows[rowIndex].Cells["TenLoaiDaiLy"].Value.ToString();
            txSuaDT.Text = dgvDSDaiLy.Rows[rowIndex].Cells["DienThoai"].Value.ToString();
            txSuaDiaChi.Text = dgvDSDaiLy.Rows[rowIndex].Cells["DiaChi"].Value.ToString();
            cbSuaQuan.Text = dgvDSDaiLy.Rows[rowIndex].Cells["TenQuan"].Value.ToString();
            dateSuaNgayTN.Text = dgvDSDaiLy.Rows[rowIndex].Cells["NgayTiepNhan"].Value.ToString();
            txSuaEmail.Text = dgvDSDaiLy.Rows[rowIndex].Cells["Email"].Value.ToString();

        }

        private void btLuuSuaDL_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                {
                    ketnoi.Open();

                    string sql = "UPDATE DAILY SET TenDaiLy = @TenDaiLy, MaLoaiDaiLy = @MaLoaiDaiLy, DienThoai = @DienThoai, DiaChi = @DiaChi, MaQuan = @MaQuan, NgayTiepNhan = @NgayTiepNhan, Email = @Email " +
                        "WHERE MaDaiLy = '" + txMaDL.Text + "'";
                    SqlCommand command = new SqlCommand(sql, ketnoi);
                    command.Parameters.AddWithValue("@TenDaiLy", txSuaTenDL.Text);
                    command.Parameters.AddWithValue("@MaLoaiDaiLy", cbSuaLDL.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@DienThoai", txSuaDT.Text);
                    command.Parameters.AddWithValue("@DiaChi", txSuaDiaChi.Text);
                    command.Parameters.AddWithValue("@MaQuan", cbSuaQuan.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@NgayTiepNhan", dateSuaNgayTN.Value);
                    command.Parameters.AddWithValue("@Email", txSuaEmail.Text);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã cập nhật thông tin đại lý thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Chuyển về tab "tabDSDaiLy" trong tabTC_DanhSach
                        tabCT_DanhSach.SelectTab("tabDSDaiLy");

                        // Chuyển về tabDanhSach trong tabAdmin
                        tabAdmin.SelectTab("tabDanhSach");

                        Form1_Load(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy đại lý cần cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btHuySuaDL_Click(object sender, EventArgs e)
        {
            // Chuyển về tab "tabDSDaiLy" trong tabTC_DanhSach
            tabCT_DanhSach.SelectTab("tabDSDaiLy");

            // Chuyển về tabDanhSach trong tabAdmin
            tabAdmin.SelectTab("tabDanhSach");
        }

       


        // Danh sách mặt hàng
        private void btTK_DSMH_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                {
                    ketnoi.Open();

                    string sql = "SELECT MaMH, TenMH, TenDVT, SoLuongTon, DonGiaNhap, DonGiaXuat " +
                                 "FROM MATHANG, DVT " +
                                 "WHERE MATHANG.MaDVT = DVT.MaDVT ";

                    if (checkTenMH.Checked)
                    {
                        sql += "AND TenMH = @TenMH ";
                    }
                    if (checkTenDVT.Checked)
                    {
                        sql += "AND TenDVT = @TenDVT ";
                    }
                    if (checkSLT.Checked)
                    {
                        sql += "AND SoLuongTon = @SoLuongTon ";
                    }
                    if (checkKhoangSLT.Checked)
                    {
                        sql += "AND SoLuongTon >= @SoLuongTonMin AND SoLuongTon <= @SoLuongTonMax ";
                    }
                    if (checkDGN.Checked)
                    {
                        sql += "AND DonGiaNhap = @DonGiaNhap ";
                    }
                    if (checkKhoangDGN.Checked)
                    {
                        sql += "AND DonGiaNhap >= @DonGiaNhapMin AND DonGiaNhap <= @DonGiaNhapMax ";
                    }
                    if (checkDGX.Checked)
                    {
                        sql += "AND DonGiaXuat = @DonGiaXuat ";
                    }
                    if (checkKhoangDGX.Checked)
                    {
                        sql += "AND DonGiaXuat >= @DonGiaXuatMin AND DonGiaXuat <= @DonGiaXuatMax ";
                    }

                    SqlCommand command = new SqlCommand(sql, ketnoi);

                    if (checkTenMH.Checked)
                    {
                        command.Parameters.AddWithValue("@TenMH", cbcTenMH_DS.SelectedValue.ToString());
                    }
                    if (checkTenDVT.Checked)
                    {
                        command.Parameters.AddWithValue("@TenDVT", cbcTenDVT_DS.SelectedValue.ToString());
                    }
                    if (checkSLT.Checked)
                    {
                        command.Parameters.AddWithValue("@SoLuongTon", txSLTon_DS.Text);
                    }
                    if (checkKhoangSLT.Checked)
                    {
                        command.Parameters.AddWithValue("@SoLuongTonMin", txSLTonMin_DS.Text);
                        command.Parameters.AddWithValue("@SoLuongTonMax", txSLTonmax_DS.Text);
                    }
                    if (checkDGN.Checked)
                    {
                        command.Parameters.AddWithValue("@DonGiaNhap", txDGN_DS.Text);
                    }
                    if (checkKhoangDGN.Checked)
                    {
                        command.Parameters.AddWithValue("@DonGiaNhapMin", txDGNmin_DS.Text);
                        command.Parameters.AddWithValue("@DonGiaNhapMax", txDGNmax_DS.Text);
                    }
                    if (checkDGX.Checked)
                    {
                        command.Parameters.AddWithValue("@DonGiaXuat", txDGX_DS.Text);
                    }
                    if (checkKhoangDGX.Checked)
                    {
                        command.Parameters.AddWithValue("@DonGiaXuatMin", txDGXmin_DS.Text);
                        command.Parameters.AddWithValue("@DonGiaXuatMax", txDGXmax_DS.Text);
                    }

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dgvDS_MH.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin mặt hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btReset_DSMH_Click(object sender, EventArgs e)
        {
            checkTenMH.Checked = false;
            checkTenDVT.Checked = false;
            checkSLT.Checked = false;
            checkKhoangSLT.Checked = false;
            checkDGN.Checked = false;
            checkKhoangDGN.Checked = false;
            checkDGX.Checked = false;
            checkKhoangDGX.Checked = false;
            txSLTon_DS.Text = "";
            txSLTonMin_DS.Text = "";
            txSLTonmax_DS.Text = "";
            txDGN_DS.Text = "";
            txDGNmin_DS.Text = "";
            txDGNmax_DS.Text = "";
            txDGX_DS.Text = "";
            txDGXmin_DS.Text = "";
            txDGXmax_DS.Text = "";

            Form1_Load(sender, e);
        }

        private void btThemMH_Click(object sender, EventArgs e)
        {

            tabAdmin.SelectedTab = tabThemMH;
        }

        private void btSuaMH_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabSuaMH;
        }

        private void btHuyBoSuaMH_Click(object sender, EventArgs e)
        {
            tabCT_DanhSach.SelectTab("tabDSMatHang");
            tabAdmin.SelectTab("tabDanhSach");
        }

        private void btHuyThemMH_Click(object sender, EventArgs e)
        {
            txThemMH_TenMH.Text = "";
            tabCT_DanhSach.SelectTab("tabDSMatHang");
            tabAdmin.SelectTab("tabDanhSach");
        }

        private void btXoaMH_Click(object sender, EventArgs e)
        {
            if (dgvDS_MH.SelectedRows.Count > 0)
            {
                // Lấy giá trị cột MaMH từ hàng được chọn
                string maMH = dgvDS_MH.SelectedRows[0].Cells["MaMH"].Value.ToString();

                // Hiển thị hộp thoại xác nhận xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mặt hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                        {
                            ketnoi.Open();

                            // Xây dựng câu lệnh SQL xóa mặt hàng dựa trên MaMH
                            string sql = "DELETE FROM MATHANG WHERE MaMH = @MaMH";
                            SqlCommand command = new SqlCommand(sql, ketnoi);
                            command.Parameters.AddWithValue("@MaMH", maMH);

                            // Thực thi câu lệnh SQL
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Đã xóa mặt hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Form1_Load(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy mặt hàng có mã tương ứng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn mặt hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btLuuMH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    ketnoi.Open();

                    // Kiểm tra tên mặt hàng đã tồn tại
                    string sqlCheckExist = "SELECT COUNT(*) FROM MATHANG WHERE TenMH = @TenMH";
                    SqlCommand commandCheckExist = new SqlCommand(sqlCheckExist, ketnoi);
                    commandCheckExist.Parameters.AddWithValue("@TenMH", txThemMH_TenMH.Text);
                    int count = (int)commandCheckExist.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Tên mặt hàng đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Không thực hiện thêm mặt hàng nữa
                    }

                    // Kiểm tra tên mặt hàng không được bỏ trống
                    if (string.IsNullOrEmpty(txThemMH_TenMH.Text))
                    {
                        MessageBox.Show("Vui lòng nhập tên mặt hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Không thực hiện thêm mặt hàng nữa
                    }

                    // Thêm mặt hàng vào cơ sở dữ liệu
                    SqlCommand commandInsert = ketnoi.CreateCommand();
                    commandInsert.CommandText = "INSERT INTO MATHANG (TenMH, MaDVT) VALUES (@TenMH, @MaDVT)";
                    commandInsert.Parameters.AddWithValue("@TenMH", txThemMH_TenMH.Text);
                    commandInsert.Parameters.AddWithValue("@MaDVT", cbcThemMH_TenDVT.SelectedValue.ToString());

                    int rowsAffected = commandInsert.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã thêm mặt hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form1_Load(sender, e); // Cập nhật lại dữ liệu
                        // Chuyển về tab "tabDSMatHang" trong tabTC_DanhSach
                        tabCT_DanhSach.SelectTab("tabDSMatHang");

                        // Chuyển về tabDanhSach trong tabAdmin
                        tabAdmin.SelectTab("tabDanhSach");

                    }
                    else
                    {
                        MessageBox.Show("Thêm mặt hàng không thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvDS_MH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = dgvDS_MH.CurrentRow.Index;
            txSua_MaMH.Text = dgvDS_MH.Rows[rowIndex].Cells["MaMH"].Value.ToString();
            txSua_TenMH.Text = dgvDS_MH.Rows[rowIndex].Cells["TenMH"].Value.ToString();
            cbcSua_TenDVT.Text = dgvDS_MH.Rows[rowIndex].Cells["TenDVT"].Value.ToString();
            txSua_DGN.Text = dgvDS_MH.Rows[rowIndex].Cells["DonGiaNhap"].Value.ToString();
            txSua_DGX.Text = dgvDS_MH.Rows[rowIndex].Cells["DonGiaXuat"].Value.ToString();
            txSLTon.Text = dgvDS_MH.Rows[rowIndex].Cells["SoLuongTon"].Value.ToString();
        }

        private void btLuuSua_MH_Click(object sender, EventArgs e)
        {
            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    ketnoi.Open();

                    // Kiểm tra tên mặt hàng đã tồn tại
                    string sqlCheckExist = "SELECT COUNT(*) FROM MATHANG WHERE TenMH = @TenMH AND MaMH != @MaMH";
                    SqlCommand commandCheckExist = new SqlCommand(sqlCheckExist, ketnoi);
                    commandCheckExist.Parameters.AddWithValue("@TenMH", txSua_TenMH.Text);
                    commandCheckExist.Parameters.AddWithValue("@MaMH", txSua_MaMH.Text);
                    int count = (int)commandCheckExist.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Tên mặt hàng đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Không thực hiện cập nhật mặt hàng
                    }

                    if (string.IsNullOrEmpty(txSua_TenMH.Text))
                    {
                        MessageBox.Show("Tên mặt hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Không thực hiện cập nhật mặt hàng
                    }

                    string sql = "UPDATE MATHANG SET TenMH = @TenMH, MaDVT = @MaDVT, DonGiaNhap = @DonGiaNhap, DonGiaXuat = @DonGiaXuat, SoLuongTon = @SoLuongTon WHERE MaMH = @MaMH";
                    SqlCommand command = new SqlCommand(sql, ketnoi);
                    command.Parameters.AddWithValue("@TenMH", txSua_TenMH.Text);
                    command.Parameters.AddWithValue("@MaDVT", cbcSua_TenDVT.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@DonGiaNhap", txDonGiaNhap.Text);
                    command.Parameters.AddWithValue("@DonGiaXuat", txDonGiaXuat.Text);
                    command.Parameters.AddWithValue("@SoLuongTon", txSLTon.Text);
                    command.Parameters.AddWithValue("@MaMH", txSua_MaMH.Text);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã cập nhật thông tin mặt hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form1_Load(sender, e); // Cập nhật lại dữ liệu

                        // Chuyển về tab "tabDSMatHang" trong tabTC_DanhSach
                        tabCT_DanhSach.SelectTab("tabDSMatHang");

                        // Chuyển về tabDanhSach trong tabAdmin
                        tabAdmin.SelectTab("tabDanhSach");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mặt hàng cần cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btCongDVT_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabThemDVT2;
        }

        private void btHuyThemDVT2_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabSuaMH;
        }

        private void btHuyThemDVT1_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabThemMH;
        }

        private void btThemDVT1_Click(object sender, EventArgs e)
        {
            txDVT1.Text = "";
            tabAdmin.SelectedTab = tabThemDVT1;
        }



        //PHIẾU THU TIỀN
        private void btTKPTT_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();

            string sql = "SELECT MaPhieuThuTien, TenDaiLy, NgayThuTien, SoTienThu " +
                         "FROM DAILY dl, PHIEUTHUTIEN ptt " +
                         "WHERE dl.MaDaiLy = ptt.MaDaiLy ";

            if (checkTenDL_No.Checked)
            {
                sql += "AND TenDaiLy = N'" + cbcDSDL_No.Text + "' ";
            }
            if (checkDateTT.Checked)
            {
                sql += "AND CONVERT(date, NgayThuTien) = '" + dateTT.Value.Date.ToString("yyyy-MM-dd") + "' ";
            }
            if (checkKhoangSTT.Checked)
            {
                sql += "AND CONVERT(date, NgayThuTien) >= '" + dateTTmin.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(date, NgayThuTien) <= '" + dateTTmax.Value.Date.ToString("yyyy-MM-dd") + "' ";
            }
            if (checkSoTienThu.Checked)
            {
                sql += "AND SoTienThu = '" + txSoTienThu.Text + "' ";
            }
            if (checkKhoangTienThu.Checked)
            {
                if (!string.IsNullOrEmpty(txSoTienThumin.Text))
                {
                    sql += "AND SoTienThu >= '" + txSoTienThumin.Text.Replace("'", "''") + "' ";
                }
                if (!string.IsNullOrEmpty(txSoTienThumax.Text))
                {
                    sql += "AND SoTienThu <= '" + txSoTienThumax.Text.Replace("'", "''") + "' ";
                }
            }

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDSPTT.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ketnoi.Close();
        }

        private void btResetPTT_Click(object sender, EventArgs e)
        {
            checkTenDL_No.Checked = false;
            checkDateTT.Checked = false;
            checkKhoangSTT.Checked = false;
            checkSoTienThu.Checked = false;
            checkKhoangTienThu.Checked = false;

            dateTT.Value = DateTime.Now.Date;
            dateTTmin.Value = DateTime.Now.Date;
            dateTTmax.Value = DateTime.Now.Date;

            txSoTienThu.Text = "";
            txSoTienThumin.Text = "";
            txSoTienThumax.Text = "";

            Form1_Load(sender, e);
        }

        private void dgvDSNo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDSNo.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvDSNo.SelectedRows[0];
                string tenDaiLy = selectedRow.Cells["TenDaiLy"].Value.ToString();
                string maQuan = selectedRow.Cells["MaQuan"].Value.ToString();
                string tienNo = selectedRow.Cells["TienNo"].Value.ToString();

                txTenDL_TT.Text = tenDaiLy;
                txSoTT.Text = tienNo;
                txMaQuan.Text = maQuan;
            }
        }

        private void btLapPTT_Click(object sender, EventArgs e)
        {
            if (dgvDSNo.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một đại lý trong danh sách nợ trước khi lập phiếu thu tiền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tabAdmin.SelectedTab = tabLapPhieuThuTien;
        }

        private void btLuuPTT_Click(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();

            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "INSERT INTO PHIEUTHUTIEN (MaDaiLy, NgayThuTien, SoTienThu) " +
                "VALUES ((SELECT MaDaiLy FROM DAILY WHERE TenDaiLy = N'" + txTenDL_TT.Text + "' AND MaQuan = N'" + txMaQuan.Text + "' ), " +
                "'" + dateNgayTT.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txSoTT.Text + "')";

            try
            {
                thuchien.ExecuteNonQuery();
                MessageBox.Show("Lưu phiếu thu tiền thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                tabAdmin.SelectedTab = tabThuTien;

                Form1_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu phiếu thu tiền thất bại!\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ketnoi.Close();
        }

        private void btHuyPTT_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabThuTien;
        }

        //BÁO CÁO
        //Báo cáo doanh số
        private void btLapBCDS_Click(object sender, EventArgs e)
        {
            int thang = int.Parse(cbcThangDS.SelectedItem.ToString());
            int nam = int.Parse(txNamDS.Text);

            DateTime hienTai = DateTime.Now;
            DateTime thoiGianNhap = new DateTime(nam, thang, 1); // Giả sử ngày nhập là ngày đầu tiên của tháng

            if (thoiGianNhap.Year > hienTai.Year || (thoiGianNhap.Year == hienTai.Year && thoiGianNhap.Month >= hienTai.Month))
            {
                MessageBox.Show("Chưa tới thời gian lập báo cáo.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            // Thực hiện truy vấn để lấy dữ liệu từ CSDL
            string sql = "EXEC CalculateAndDisplayReport @Thang = " + thang + ", @Nam = " + nam + "; " +
                         "SELECT dl.TenDaiLy, ctbcds.TyLe " +
                         "FROM BAOCAODOANHSO bcds " +
                         "INNER JOIN CT_BCDS ctbcds ON bcds.MaBCDS = ctbcds.MaBCDS " +
                         "INNER JOIN DAILY dl ON ctbcds.MaDaiLy = dl.MaDaiLy " +
                         "WHERE bcds.Thang = " + thang + " AND bcds.Nam = " + nam;

            SqlConnection connection = new SqlConnection(chuoiketnoi);
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Thang", thang);
            command.Parameters.AddWithValue("@Nam", nam);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Xóa các series và titles hiện tại trong biểu đồ
                chartTiLeTriGia.Series.Clear();
                chartTiLeTriGia.Titles.Clear();

                // Tạo series mới cho biểu đồ
                Series series = new Series("Tỉ lệ trị giá");
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = true;
                series.LabelFormat = "#.###%";

                while (reader.Read())
                {
                    string tenDaiLy = reader["TenDaiLy"].ToString();
                    decimal tyLe = Convert.ToDecimal(reader["TyLe"]);

                    DataPoint dataPoint = new DataPoint();
                    dataPoint.AxisLabel = tenDaiLy;
                    dataPoint.YValues = new double[] { (double)tyLe };
                    series.Points.Add(dataPoint);
                }

                // Thêm series vào biểu đồ
                chartTiLeTriGia.Series.Add(series);

                // Thêm title cho biểu đồ
                chartTiLeTriGia.Titles.Add("Biểu đồ tỉ lệ tổng trị giá");

                // Hiển thị biểu đồ
                chartTiLeTriGia.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }

            // Thao tác ở dgvCT_BCDS
            using (connection = new SqlConnection(chuoiketnoi))
            {
                connection.Open();

                sql = "SELECT dl.MaDaiLy, dl.TenDaiLy, c.SoPhieuXuat, c.TongTriGia " +
                             "FROM CT_BCDS c, DAILY dl, BAOCAODOANHSO b " +
                             "WHERE b.MaBCDS = c.MaBCDS and c.MaDaiLy = dl.MaDaiLy and Thang = @Thang and Nam = @Nam";

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Thang", cbcThangDS.Text);
                command.Parameters.AddWithValue("@Nam", txNamDS.Text);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCT_BCDS.DataSource = dt;

                // Hiển thị thông tin ở các textbox
                sql = "SELECT * FROM BAOCAODOANHSO WHERE Thang = @Thang AND Nam = @Nam";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Thang", cbcThangDS.Text);
                command.Parameters.AddWithValue("@Nam", txNamDS.Text);

                adapter = new SqlDataAdapter(command);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txMaBCDS.Text = row.Field<string>("MaBCDS");
                    txThang.Text = row.Field<int>("Thang").ToString();
                    txNam.Text = row.Field<int>("Nam").ToString();

                    decimal tongDoanhThu = 0;
                    if (!row.IsNull("TongDoanhThu"))
                    {
                        tongDoanhThu = row.Field<decimal>("TongDoanhThu");
                    }
                    txTongDoanhThu.Text = tongDoanhThu.ToString("N0");
                }
                else
                {
                    txMaBCDS.Text = string.Empty;
                    txThang.Text = string.Empty;
                    txNam.Text = string.Empty;
                    txTongDoanhThu.Text = string.Empty;
                }



            }

            // Biểu đồ cột về doanh thu các tháng trong năm
            if (int.TryParse(txNamDS.Text, out nam))
            {
                connection = new SqlConnection(chuoiketnoi);
                sql = "SELECT Thang, ISNULL(TongDoanhThu, 0) AS TongDoanhThu FROM BAOCAODOANHSO WHERE Nam = @Nam";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Nam", nam);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Xóa dữ liệu cũ trong biểu đồ
                    chartDoanhSoThang.Series.Clear();

                    // Tạo series mới cho biểu đồ cột
                    Series series = new Series("Tổng doanh thu tháng");
                    series.ChartType = SeriesChartType.Column;

                    while (reader.Read())
                    {
                        thang = reader.GetInt32(0);
                        decimal tongDoanhThu = reader.GetDecimal(1);

                        // Thêm điểm dữ liệu vào series
                        DataPoint dataPoint = new DataPoint();
                        dataPoint.AxisLabel = thang.ToString();
                        dataPoint.YValues = new double[] { (double)tongDoanhThu };
                        series.Points.Add(dataPoint);
                    }

                    // Thêm series vào biểu đồ
                    chartDoanhSoThang.Series.Add(series);

                    // Hiển thị biểu đồ
                    chartDoanhSoThang.Visible = true;

                    // Hiển thị thông tin tổng doanh thu trên biểu đồ
                    decimal tongDoanhThuNam = series.Points.Sum(p => (decimal)p.YValues[0]);
                    chartDoanhSoThang.Titles.Clear();
                    chartDoanhSoThang.Titles.Add("Tổng doanh thu của năm " + nam + ": " + tongDoanhThuNam.ToString("N0"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi truy vấn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Năm không hợp lệ. Vui lòng nhập một giá trị số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //Báo cáo công nợ
        private void btLapBCCN_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy giá trị tháng và năm từ combobox và textbox
                int thang = Convert.ToInt32(cbcThangCN.SelectedItem);
                int nam = Convert.ToInt32(txNamBCCN.Text);

                DateTime hienTai = DateTime.Now;
                DateTime thoiGianNhap = new DateTime(nam, thang, 1); // Giả sử ngày nhập là ngày đầu tiên của tháng

                if (thoiGianNhap.Year > hienTai.Year || (thoiGianNhap.Year == hienTai.Year && thoiGianNhap.Month >= hienTai.Month))
                {
                    MessageBox.Show("Chưa tới thời gian lập báo cáo.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                using (SqlConnection connection = new SqlConnection(chuoiketnoi))
                {
                    connection.Open();

                    // Gọi stored procedure Report_BAOCAOCONGNO để tính toán dữ liệu
                    using (SqlCommand command = new SqlCommand("Report_BAOCAOCONGNO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Thang", thang);
                        command.Parameters.AddWithValue("@Nam", nam);
                        command.ExecuteNonQuery();
                    }

                    // Truy vấn dữ liệu từ database
                    string query = "SELECT d.TenDaiLy, c.NoCuoi FROM CT_BCCN c INNER JOIN DAILY d ON c.MaDaiLy = d.MaDaiLy WHERE c.MaBCCN = 'N' + RIGHT('00' + CAST(@Thang AS VARCHAR(2)), 2) + RIGHT(CAST(@Nam AS VARCHAR(4)), 2)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Thang", thang);
                        command.Parameters.AddWithValue("@Nam", nam);

                        SqlDataAdapter adapter;
                        using (adapter = new SqlDataAdapter(command))
                        {
                            DataTable data = new DataTable();
                            adapter.Fill(data);

                            // Xóa các dữ liệu cũ trên biểu đồ
                            chartBCCN.Series.Clear();

                            // Tạo một chuỗi dữ liệu mới trên biểu đồ
                            Series series = new Series("Nợ cuối");

                            // Thiết lập loại biểu đồ
                            series.ChartType = SeriesChartType.Column;

                            // Thêm dữ liệu vào chuỗi dữ liệu
                            foreach (DataRow row in data.Rows)
                            {
                                string tenDaiLy = row["TenDaiLy"].ToString();
                                decimal noCuoi = Convert.ToDecimal(row["NoCuoi"]);

                                series.Points.AddXY(tenDaiLy, noCuoi);
                            }

                            // Thêm chuỗi dữ liệu vào biểu đồ
                            chartBCCN.Series.Add(series);
                        }
                    }

                    connection.Close();
                }

                // Hiện dữ liệu ở dgvCT_BCCN
                SqlConnection ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                string sql = "SELECT ct.MaDaiLy, TenDaiLy, NoDau, PhatSinh, NoCuoi " +
                             "FROM CT_BCCN ct, DAILY d, BAOCAOCONGNO b " +
                             "WHERE ct.MaBCCN = b.MaBCCN AND d.MaDaiLy = ct.MaDaiLy AND Thang = " + thang + " AND Nam = " + nam;

                SqlDataAdapter adapter1 = new SqlDataAdapter(sql, ketnoi);
                DataTable dt = new DataTable();
                adapter1.Fill(dt);

                dgvCT_BCCN.DataSource = dt;

                ketnoi.Close();


                // Truy vấn để lấy thông tin BAOCAOCONGNO cụ thể
                sql = "SELECT * FROM BAOCAOCONGNO WHERE Thang = @Thang AND Nam = @Nam";
                SqlCommand command1 = new SqlCommand(sql, ketnoi);
                command1.Parameters.AddWithValue("@Thang", cbcThangCN.SelectedItem);
                command1.Parameters.AddWithValue("@Nam", txNamBCCN.Text);

                SqlDataAdapter adapter2 = new SqlDataAdapter(command1);
                
                dt = new DataTable();
                adapter2.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txMaCN.Text = row.Field<string>("MaBCCN");
                    txThangCN.Text = row.Field<int>("Thang").ToString();
                    txNamCN.Text = row.Field<int>("Nam").ToString();

                    decimal tongNo = 0;
                    if (!row.IsNull("TongNo"))
                    {
                        tongNo = row.Field<decimal>("TongNo");
                    }
                    txTongNo.Text = tongNo.ToString("N0");
                }
                else
                {
                    txMaCN.Text = string.Empty;
                    txThangCN.Text = string.Empty;
                    txNamCN.Text = string.Empty;
                    txTongNo.Text = string.Empty;
                }


            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //CÀI ĐẶT
        private void btSuaSDLTD_Click(object sender, EventArgs e)
        {
            if (txSuaSDLTD.Visible && btLuuSDLTD.Visible)
            {
                // Nếu control txSuaSDLTD và btLuuSDLTD đang hiển thị, thực hiện việc ẩn đi
                txSuaSDLTD.Visible = false;
                btLuuSDLTD.Visible = false;
            }
            else
            {
                // Nếu control txSuaSDLTD và btLuuSDLTD không hiển thị, thực hiện việc hiển thị
                txSuaSDLTD.Visible = true;
                btLuuSDLTD.Visible = true;
            }
        }

        private void btSuaTLDG_Click(object sender, EventArgs e)
        {
            if (txSuaTLDG.Visible && btLuuTLDG.Visible)
            {
                txSuaTLDG.Visible = false;
                btLuuTLDG.Visible = false;
            }
            else
            {
                txSuaTLDG.Visible = true;
                btLuuTLDG.Visible = true;
            }
        }

        private void btLuuSDLTD_Click(object sender, EventArgs e)
        {
            try
            {
                int soDaiLyToiDa;
                if (!int.TryParse(txSuaSDLTD.Text, out soDaiLyToiDa))
                {
                    MessageBox.Show("Vui lòng nhập số nguyên hợp lệ!");
                    return;
                }

                using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                {
                    ketnoi.Open();

                    // Lấy số đại lý tối đa trong mỗi quận
                    string sqlCountDL = "SELECT MaQuan, COUNT(*) AS SoDaiLy FROM DAILY GROUP BY MaQuan";
                    SqlCommand commandCountDL = new SqlCommand(sqlCountDL, ketnoi);
                    SqlDataReader readerCountDL = commandCountDL.ExecuteReader();

                    int maxSoDaiLy = 0;
                    while (readerCountDL.Read())
                    {
                        int soDaiLy = Convert.ToInt32(readerCountDL["SoDaiLy"]);
                        if (soDaiLy > maxSoDaiLy)
                        {
                            maxSoDaiLy = soDaiLy;
                        }
                    }
                    readerCountDL.Close();

                    // Kiểm tra xem số lượng đại lý tối đa của quận đang cập nhật có nhỏ hơn số lượng đại lý tối đa nhất không
                    if (maxSoDaiLy > soDaiLyToiDa)
                    {
                        MessageBox.Show("Số lượng đại lý tối đa trong quận đang cập nhật không thể nhỏ hơn số lượng đại lý tối đa trong một quận khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tiến hành cập nhật số đại lý tối đa nếu không có vấn đề gì
                    string sqlUpdate = "UPDATE THAMSO SET SoDaiLyToiDa = @SoDaiLyToiDa";
                    using (SqlCommand commandUpdate = new SqlCommand(sqlUpdate, ketnoi))
                    {
                        commandUpdate.Parameters.AddWithValue("@SoDaiLyToiDa", soDaiLyToiDa);
                        commandUpdate.ExecuteNonQuery();
                        MessageBox.Show("Lưu thành công!");
                    }

                    ketnoi.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btLuuTLDG_Click(object sender, EventArgs e)
        {
            string sql = "update THAMSO set TiLeDonGia = @TiLeDonGia";

            using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
            {
                ketnoi.Open();

                using (SqlCommand thuchien = new SqlCommand(sql, ketnoi))
                {
                    decimal tiLeDonGia;
                    if (decimal.TryParse(txSuaTLDG.Text, out tiLeDonGia))
                    {
                        thuchien.Parameters.AddWithValue("@TiLeDonGia", tiLeDonGia);
                        thuchien.ExecuteNonQuery();
                        MessageBox.Show("Lưu thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập số hợp lệ!");
                    }
                }

                // Gọi lại sự kiện Form1_Load để cập nhật giá trị mới
                Form1_Load(sender, e);

                ketnoi.Close();
            }
        }

        private void btXoaBCDS_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ dữ liệu liên quan tới doanh số của tất cả các tháng?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "delete from CT_BCDS; delete from BAOCAODOANHSO";
                thuchien.ExecuteNonQuery();

                Form1_Load(sender, e);
                ketnoi.Close();
            }
        }

        private void btXoaBCCN_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ dữ liệu liên quan tới công nợ của tất cả các tháng?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "delete from CT_BCCN; delete from BAOCAOCONGNO";
                thuchien.ExecuteNonQuery();

                Form1_Load(sender, e);
                ketnoi.Close();
            }

        }


        //BỔ SUNG

        //TIẾP NHẬN ĐẠI LÝ

               //Thêm loại đại lý -> Tiếp nhận đại lý
        private void btCongLDL_Click(object sender, EventArgs e)
        {
            txThemLDL2.Text = "";
            txSNTD2.Text = "";
            tabAdmin.SelectedTab = tabThemLDL1;
        }

        private void btLuuLDL2_Click(object sender, EventArgs e)
        {
            string tenLoaiDaiLy = txThemLDL2.Text;
            string soNoToiDa = txSNTD2.Text;

            if (string.IsNullOrEmpty(tenLoaiDaiLy))
            {
                MessageBox.Show("Vui lòng nhập tên loại đại lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(soNoToiDa, out int parsedSoNoToiDa))
            {
                MessageBox.Show("Số nợ tối đa chỉ được nhập số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tên loại đại lý đã tồn tại hay chưa
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "SELECT COUNT(*) FROM LOAIDAILY WHERE TenLoaiDaiLy = N'" + tenLoaiDaiLy + "'";
            int count = (int)thuchien.ExecuteScalar();
            ketnoi.Close();

            if (count > 0)
            {
                MessageBox.Show("Tên loại đại lý đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thêm loại đại lý vào cơ sở dữ liệu
                ketnoi.Open();
                thuchien.CommandText = "INSERT INTO LOAIDAILY(TenLoaiDaiLy, SoNoToiDa) VALUES (N'" + tenLoaiDaiLy + "', " + parsedSoNoToiDa + ")";
                thuchien.ExecuteNonQuery();
                ketnoi.Close();

                MessageBox.Show("Thêm loại đại lý thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                tabAdmin.SelectedTab = tabTiepNhanDL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm loại đại lý thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btHuyLDL2_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabTiepNhanDL;
        }

              //Thêm quận -> Tiếp nhận đại lý
        private void btCongQuan_Click(object sender, EventArgs e)
        {
            txThemQuan2.Text = "";
            tabAdmin.SelectedTab = tabThemQuan1;
        }

        private void btLuuQuan2_Click(object sender, EventArgs e)
        {
            string tenQuan = txThemQuan2.Text;

            if (string.IsNullOrEmpty(tenQuan))
            {
                MessageBox.Show("Vui lòng nhập tên quận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra tên quận đã tồn tại hay chưa
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM QUAN WHERE TenQuan = @TenQuan";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                int count = (int)thuchien.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Tên quận đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm quận vào cơ sở dữ liệu
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "INSERT INTO QUAN(TenQuan) VALUES (@TenQuan)";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                thuchien.ExecuteNonQuery();

                MessageBox.Show("Thêm quận thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                ketnoi.Close();

                tabAdmin.SelectedTab = tabTiepNhanDL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm quận thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btHuyQuan2_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabTiepNhanDL;
        }

        //XUẤT NHẬP HÀNG
        private void btXoaPNH_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn hàng trong DataGridView hay chưa
            if (dgvPNH.SelectedRows.Count > 0)
            {
                // Lấy mã phiếu nhập hàng từ hàng được chọn
                string maPhieuNhap = dgvPNH.SelectedRows[0].Cells["MaPhieuNhap"].Value.ToString();

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu nhập hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        ketnoi = new SqlConnection(chuoiketnoi);
                        ketnoi.Open();
                        // Xóa chi tiết phiếu nhập hàng (CT_PNH) dựa trên mã phiếu nhập
                        thuchien = ketnoi.CreateCommand();
                        thuchien.CommandText = "DELETE FROM CT_PNH WHERE MaPhieuNhap = @MaPhieuNhap";
                        thuchien.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                        thuchien.ExecuteNonQuery();

                        // Xóa phiếu nhập hàng (PHIEUNHAPHANG) dựa trên mã phiếu nhập
                        thuchien = ketnoi.CreateCommand();
                        thuchien.CommandText = "DELETE FROM PHIEUNHAPHANG WHERE MaPhieuNhap = @MaPhieuNhap";
                        thuchien.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                        thuchien.ExecuteNonQuery();

                        // Tải lại dữ liệu trong DataGridView sau khi xóa
                        Form1_Load(sender, e);

                        // Thông báo thành công
                        MessageBox.Show("Đã xóa phiếu nhập hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa phiếu nhập hàng:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Đóng kết nối sau khi hoàn thành công việc
                        ketnoi.Close();
                    }
                }
            }
            else
            {
                // Hiển thị thông báo khi chưa chọn hàng
                MessageBox.Show("Vui lòng chọn một phiếu nhập hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btXoaPXH_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn hàng trong DataGridView hay chưa
            if (dgvPXH.SelectedRows.Count > 0)
            {
                // Lấy mã phiếu xuất hàng từ hàng được chọn
                string maPhieuXuat = dgvPXH.SelectedRows[0].Cells["MaPhieuXuat"].Value.ToString();

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu xuất hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        ketnoi = new SqlConnection(chuoiketnoi);
                        ketnoi.Open();

                        // Xóa chi tiết phiếu xuất hàng (CT_PXH) dựa trên mã phiếu xuất
                        thuchien = ketnoi.CreateCommand();
                        thuchien.CommandText = "DELETE FROM CT_PXH WHERE MaPhieuXuat = @MaPhieuXuat";
                        thuchien.Parameters.AddWithValue("@MaPhieuXuat", maPhieuXuat);
                        thuchien.ExecuteNonQuery();

                        // Xóa phiếu xuất hàng (PHIEUXUATHANG) dựa trên mã phiếu xuất
                        thuchien = ketnoi.CreateCommand();
                        thuchien.CommandText = "DELETE FROM PHIEUXUATHANG WHERE MaPhieuXuat = @MaPhieuXuat";
                        thuchien.Parameters.AddWithValue("@MaPhieuXuat", maPhieuXuat);
                        thuchien.ExecuteNonQuery();

                        // Tải lại dữ liệu trong DataGridView sau khi xóa
                        Form1_Load(sender, e);

                        // Thông báo thành công
                        MessageBox.Show("Đã xóa phiếu xuất hàng thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa phiếu xuất hàng:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        ketnoi.Close();
                    }
                }
            }
            else
            {
                // Hiển thị thông báo khi chưa chọn hàng
                MessageBox.Show("Vui lòng chọn một phiếu xuất hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //LẬP PHIẾU XUẤT NHẬP HÀNG
        private void btHuyLapPNH_Click(object sender, EventArgs e)
        {
            tabXNH_CT.SelectedTab = tabNhapHang;
            tabAdmin.SelectedTab = tabXuatNhapHang;
        }

        private void btHuyLapPXH_Click(object sender, EventArgs e)
        {
            tabXNH_CT.SelectedTab = tabXuatHang;
            tabAdmin.SelectedTab = tabXuatNhapHang;
        }

                 //Lập phiếu nhập hàng -> thêm mặt hàng -> thêm đơn vị tính
        private void btCongMH_PNH_Click(object sender, EventArgs e)
        {
            txThemMH3.Text = "";
            tabAdmin.SelectedTab = tabThemMH3;   
        }

        private void btCongDVT3_Click(object sender, EventArgs e)
        {
            txDVT3.Text = "";
            tabAdmin.SelectedTab = tabDVT3;
        }

        private void bthuyMH3_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabLapPNH;
        }

        private void btHuyDVT3_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabThemMH3;
        }

        private void btLuuMH3_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;
            SqlCommand thuchien = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "insert into MATHANG(TenMH, MaDVT) " +
                    "values (N'" + txThemMH3.Text + "', N'" + cbcDVT3.SelectedValue.ToString() + "')";
                thuchien.ExecuteNonQuery();

                Form1_Load(sender, e);

                tabAdmin.SelectedTab = tabLapPNH;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (thuchien != null)
                {
                    thuchien.Dispose();
                }
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }

        private void btLuuDVT3_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;
            SqlCommand thuchien = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "insert into DVT(TenDVT) " +
                    "values (N'" + txDVT3.Text + "')";
                thuchien.ExecuteNonQuery();

                Form1_Load(sender, e);

                tabAdmin.SelectedTab = tabThemMH3;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (thuchien != null)
                {
                    thuchien.Dispose();
                }
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }


        //SỬA THÔNG TIN ĐẠI LÝ -> THÊM QUẬN, LOẠI ĐẠI LÝ
        private void btCongLDL_sua_Click(object sender, EventArgs e)
        {
            txThemLDL1.Text = "";
            txSoNoToiDa1.Text = "";

            tabAdmin.SelectTab("tabThemLDL");
        }

        private void btCongQuan_Sua_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectTab("tabThemQuan");
        }

        private void btHuyLDL1_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabSuaDL;
        }

        private void btLuuLDL1_Click(object sender, EventArgs e)
        {
            string tenLoaiDaiLy = txThemLDL1.Text;
            string soNoToiDa = txSoNoToiDa1.Text;

            if (string.IsNullOrEmpty(tenLoaiDaiLy))
            {
                MessageBox.Show("Vui lòng nhập tên loại đại lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(soNoToiDa, out int parsedSoNoToiDa))
            {
                MessageBox.Show("Số nợ tối đa chỉ được nhập số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tên loại đại lý đã tồn tại hay chưa
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "SELECT COUNT(*) FROM LOAIDAILY WHERE TenLoaiDaiLy = N'" + tenLoaiDaiLy + "'";
            int count = (int)thuchien.ExecuteScalar();
            ketnoi.Close();

            if (count > 0)
            {
                MessageBox.Show("Tên loại đại lý đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thêm loại đại lý vào cơ sở dữ liệu
                ketnoi.Open();
                thuchien.CommandText = "INSERT INTO LOAIDAILY(TenLoaiDaiLy, SoNoToiDa) VALUES (N'" + tenLoaiDaiLy + "', " + parsedSoNoToiDa + ")";
                thuchien.ExecuteNonQuery();
                ketnoi.Close();

                MessageBox.Show("Thêm loại đại lý thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                tabAdmin.SelectedTab = tabSuaDL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm loại đại lý thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btHuyQuan_Click(object sender, EventArgs e)
        {
            tabAdmin.SelectedTab = tabSuaDL;
        }

        private void btLuuQuan_Click(object sender, EventArgs e)
        {
            string tenQuan = txThemQuan.Text;

            if (string.IsNullOrEmpty(tenQuan))
            {
                MessageBox.Show("Vui lòng nhập tên quận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra tên quận đã tồn tại hay chưa
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM QUAN WHERE TenQuan = @TenQuan";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                int count = (int)thuchien.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Tên quận đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm quận vào cơ sở dữ liệu
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "INSERT INTO QUAN(TenQuan) VALUES (@TenQuan)";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                thuchien.ExecuteNonQuery();

                MessageBox.Show("Thêm quận thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                ketnoi.Close();

                tabAdmin.SelectedTab = tabSuaDL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm quận thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //DANH SÁCH MẶT HÀNG
        private void btLuuDVT1_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;
            SqlCommand thuchien = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra xem tên đơn vị tính đã tồn tại hay chưa
                string tenDVT = txDVT1.Text;
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM DVT WHERE TenDVT = @TenDVT";
                thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                int count = Convert.ToInt32(thuchien.ExecuteScalar());

                if (count > 0)
                {
                    // Tên đơn vị tính đã tồn tại, hiển thị thông báo lỗi
                    MessageBox.Show("Tên đơn vị tính đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Thêm đơn vị tính vào cơ sở dữ liệu
                    thuchien = ketnoi.CreateCommand();
                    thuchien.CommandText = "INSERT INTO DVT (TenDVT) VALUES (@TenDVT)";
                    thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                    thuchien.ExecuteNonQuery();

                    Form1_Load(sender, e);

                    MessageBox.Show("Thêm đơn vị tính thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tabAdmin.SelectedTab = tabThemMH;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (thuchien != null)
                {
                    thuchien.Dispose();
                }
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }

        private void btThemDVT2_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;
            SqlCommand thuchien = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra xem tên đơn vị tính đã tồn tại hay chưa
                string tenDVT = txDVT2.Text;
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM DVT WHERE TenDVT = @TenDVT";
                thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                int count = Convert.ToInt32(thuchien.ExecuteScalar());

                if (count > 0)
                {
                    // Tên đơn vị tính đã tồn tại, hiển thị thông báo lỗi
                    MessageBox.Show("Tên đơn vị tính đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Thêm đơn vị tính vào cơ sở dữ liệu
                    thuchien = ketnoi.CreateCommand();
                    thuchien.CommandText = "INSERT INTO DVT (TenDVT) VALUES (@TenDVT)";
                    thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                    thuchien.ExecuteNonQuery();

                    Form1_Load(sender, e);

                    MessageBox.Show("Thêm đơn vị tính thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tabAdmin.SelectedTab = tabSuaMH;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (thuchien != null)
                {
                    thuchien.Dispose();
                }
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }

        //DANH SÁCH LOẠI ĐẠI LÝ
        private void btThemLDL_Click(object sender, EventArgs e)
        {
            txLDL2.Text = "";
            txSoNoToiDa2.Text = "";
            tabAdmin.SelectedTab = tabThemLDL2;
        }

        private void btHuyLDL_Click(object sender, EventArgs e)
        {
            tabCT_DanhSach.SelectedTab = tabDSLoaiDL;
            tabAdmin.SelectedTab = tabDanhSach;
        }

        private void btLuuLDL_Click(object sender, EventArgs e)
        {
            string tenLoaiDaiLy = txLDL2.Text;
            string soNoToiDa = txSoNoToiDa2.Text;

            if (string.IsNullOrEmpty(tenLoaiDaiLy))
            {
                MessageBox.Show("Vui lòng nhập tên loại đại lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(soNoToiDa, out int parsedSoNoToiDa))
            {
                MessageBox.Show("Số nợ tối đa chỉ được nhập số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tên loại đại lý đã tồn tại hay chưa
            ketnoi = new SqlConnection(chuoiketnoi);
            ketnoi.Open();
            thuchien = ketnoi.CreateCommand();
            thuchien.CommandText = "SELECT COUNT(*) FROM LOAIDAILY WHERE TenLoaiDaiLy = N'" + tenLoaiDaiLy + "'";
            int count = (int)thuchien.ExecuteScalar();
            ketnoi.Close();

            if (count > 0)
            {
                MessageBox.Show("Tên loại đại lý đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thêm loại đại lý vào cơ sở dữ liệu
                ketnoi.Open();
                thuchien.CommandText = "INSERT INTO LOAIDAILY(TenLoaiDaiLy, SoNoToiDa) VALUES (N'" + tenLoaiDaiLy + "', " + parsedSoNoToiDa + ")";
                thuchien.ExecuteNonQuery();
                ketnoi.Close();

                MessageBox.Show("Thêm loại đại lý thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                tabCT_DanhSach.SelectedTab = tabDSLoaiDL;
                tabAdmin.SelectedTab = tabDanhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm loại đại lý thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //DANH SÁCH QUẬN
        private void btThemQuan_Click(object sender, EventArgs e)
        {
            txQuan1.Text = "";
            tabAdmin.SelectedTab = tabThemQuan2;
        }

        private void btHuyQuan1_Click(object sender, EventArgs e)
        {
            tabCT_DanhSach.SelectedTab = tabDSQuan;
            tabAdmin.SelectedTab = tabDanhSach;
        }

        private void btLuuQuan1_Click(object sender, EventArgs e)
        {
            string tenQuan = txQuan1.Text;

            if (string.IsNullOrEmpty(tenQuan))
            {
                MessageBox.Show("Vui lòng nhập tên quận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra tên quận đã tồn tại hay chưa
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM QUAN WHERE TenQuan = @TenQuan";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                int count = (int)thuchien.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Tên quận đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm quận vào cơ sở dữ liệu
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "INSERT INTO QUAN(TenQuan) VALUES (@TenQuan)";
                thuchien.Parameters.AddWithValue("@TenQuan", tenQuan);
                thuchien.ExecuteNonQuery();

                MessageBox.Show("Thêm quận thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form1_Load(sender, e);

                ketnoi.Close();

                tabCT_DanhSach.SelectedTab = tabDSQuan;
                tabAdmin.SelectedTab = tabDanhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm quận thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //DANH SÁCH ĐƠN VỊ TÍNH
        private void btThemDVT_Click(object sender, EventArgs e)
        {
            txThemDVT3.Text = "";
            tabAdmin.SelectedTab = tabThemDVT3;
        }

        private void btHuyDVT4_Click(object sender, EventArgs e)
        {
            tabCT_DanhSach.SelectedTab = tabDSDVT;
            tabAdmin.SelectedTab = tabDanhSach;
        }

        private void btLuuDVT4_Click(object sender, EventArgs e)
        {
            SqlConnection ketnoi = null;
            SqlCommand thuchien = null;

            try
            {
                ketnoi = new SqlConnection(chuoiketnoi);
                ketnoi.Open();

                // Kiểm tra xem tên đơn vị tính đã tồn tại hay chưa
                string tenDVT = txThemDVT3.Text;
                thuchien = ketnoi.CreateCommand();
                thuchien.CommandText = "SELECT COUNT(*) FROM DVT WHERE TenDVT = @TenDVT";
                thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                int count = Convert.ToInt32(thuchien.ExecuteScalar());

                if (count > 0)
                {
                    // Tên đơn vị tính đã tồn tại, hiển thị thông báo lỗi
                    MessageBox.Show("Tên đơn vị tính đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Thêm đơn vị tính vào cơ sở dữ liệu
                    thuchien = ketnoi.CreateCommand();
                    thuchien.CommandText = "INSERT INTO DVT (TenDVT) VALUES (@TenDVT)";
                    thuchien.Parameters.AddWithValue("@TenDVT", tenDVT);
                    thuchien.ExecuteNonQuery();

                    Form1_Load(sender, e);

                    MessageBox.Show("Thêm đơn vị tính thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tabCT_DanhSach.SelectedTab = tabDSDVT;
                    tabAdmin.SelectedTab = tabDanhSach;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (thuchien != null)
                {
                    thuchien.Dispose();
                }
                if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                {
                    ketnoi.Close();
                    ketnoi.Dispose();
                }
            }
        }


        //TÀI KHOẢN
        private void btXoaTK_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn hàng trong DataGridView hay chưa
            if (dgvTaiKhoan.SelectedRows.Count > 0)
            {
                // Lấy tên tài khoản từ hàng được chọn
                string tenTaiKhoan = dgvTaiKhoan.SelectedRows[0].Cells["TenTaiKhoan"].Value.ToString();

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản '" + tenTaiKhoan + "'?", "Xác nhận xóa", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Tạo kết nối đến cơ sở dữ liệu
                        using (SqlConnection ketnoi = new SqlConnection(chuoiketnoi))
                        {
                            ketnoi.Open();

                            // Xóa tài khoản dựa trên tên tài khoản
                            using (SqlCommand thuchien = ketnoi.CreateCommand())
                            {
                                thuchien.CommandText = "DELETE FROM TAIKHOAN WHERE TenTaiKhoan = @TenTaiKhoan";
                                thuchien.Parameters.AddWithValue("@TenTaiKhoan", tenTaiKhoan);
                                thuchien.ExecuteNonQuery();
                            }

                            // Tải lại dữ liệu trong DataGridView sau khi xóa
                            Form1_Load(sender, e);

                            // Thông báo thành công
                            MessageBox.Show("Đã xóa tài khoản thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa tài khoản:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // Hiển thị thông báo khi chưa chọn hàng
                MessageBox.Show("Vui lòng chọn một tài khoản để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //THU TIỀN
        private void btXoaPTT_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn hàng trong DataGridView hay chưa
            if (dgvDSPTT.SelectedRows.Count > 0)
            {
                // Lấy MaPhieuThuTien từ hàng được chọn
                string maPhieuThuTien = dgvDSPTT.SelectedRows[0].Cells["MaPhieuThuTien"].Value.ToString();

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu thu tiền này?", "Xác nhận xóa", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        ketnoi = new SqlConnection(chuoiketnoi);
                        ketnoi.Open();

                        // Xóa phiếu thu tiền dựa trên MaPhieuThuTien
                        thuchien = ketnoi.CreateCommand();
                        thuchien.CommandText = "DELETE FROM PHIEUTHUTIEN WHERE MaPhieuThuTien = @MaPhieuThuTien";
                        thuchien.Parameters.AddWithValue("@MaPhieuThuTien", maPhieuThuTien);
                        thuchien.ExecuteNonQuery();

                        // Tải lại dữ liệu trong DataGridView sau khi xóa
                        Form1_Load(sender, e);

                        // Thông báo thành công
                        MessageBox.Show("Đã xóa phiếu thu tiền thành công.");
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa phiếu thu tiền:\n" + ex.Message);
                    }
                    finally
                    {
                        if (ketnoi != null && ketnoi.State == ConnectionState.Open)
                        {
                            ketnoi.Close();
                            ketnoi.Dispose();
                        }
                    }
                }
            }
            else
            {
                // Hiển thị thông báo khi chưa chọn hàng
                MessageBox.Show("Vui lòng chọn một phiếu thu tiền để xóa.");
            }
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }
    }
}


