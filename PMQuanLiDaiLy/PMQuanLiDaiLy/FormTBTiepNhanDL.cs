using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMQuanLiDaiLy
{
    public partial class FormTBTiepNhanDL : Form
    {
        
        public FormTBTiepNhanDL()
        {
            InitializeComponent();
        }

        private void btThemDLkhac_Click(object sender, EventArgs e)
        {
            this.Hide();

            Form1 form1 = (Form1)Application.OpenForms["Form1"]; // Lấy đối tượng Form1 hiện tại
            if (form1 != null)
            {
                form1.ThemDaiLyKhac(); // Gọi hàm ThemDaiLyKhac trong Form1
            }
        }

        private void btDLDS_Click(object sender, EventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"]; // Lấy đối tượng Form1 hiện tại
            if (form1 != null)
            {
                form1.ChuyenTabDanhSach(); // Gọi hàm ChuyenTabDanhSach trong Form1
                this.Close(); // Đóng FormTBTiepNhanDL
            }
        }

        private void btDLTC_Click(object sender, EventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"]; // Lấy đối tượng Form1 hiện tại
            if (form1 != null)
            {
                form1.ChuyenTabTrangChu(); // Gọi hàm ChuyenTabTrangChu trong Form1
                this.Close(); // Đóng FormTBTiepNhanDL
            }
        }

    }
}
