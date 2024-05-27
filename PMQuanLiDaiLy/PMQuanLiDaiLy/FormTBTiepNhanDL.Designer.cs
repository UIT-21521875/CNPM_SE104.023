namespace PMQuanLiDaiLy
{
    partial class FormTBTiepNhanDL
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btThemDLkhac = new System.Windows.Forms.Button();
            this.btDLDS = new System.Windows.Forms.Button();
            this.btDLTC = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Navy;
            this.panel1.Location = new System.Drawing.Point(24, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(12, 324);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(42, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "TIẾP NHẬN ĐẠI LÝ THÀNH CÔNG !!!";
            // 
            // btThemDLkhac
            // 
            this.btThemDLkhac.BackColor = System.Drawing.Color.Navy;
            this.btThemDLkhac.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btThemDLkhac.ForeColor = System.Drawing.Color.White;
            this.btThemDLkhac.Location = new System.Drawing.Point(42, 224);
            this.btThemDLkhac.Name = "btThemDLkhac";
            this.btThemDLkhac.Size = new System.Drawing.Size(154, 64);
            this.btThemDLkhac.TabIndex = 2;
            this.btThemDLkhac.Text = "THÊM ĐẠI LÝ KHÁC";
            this.btThemDLkhac.UseVisualStyleBackColor = false;
            this.btThemDLkhac.Click += new System.EventHandler(this.btThemDLkhac_Click);
            // 
            // btDLDS
            // 
            this.btDLDS.BackColor = System.Drawing.Color.Navy;
            this.btDLDS.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDLDS.ForeColor = System.Drawing.Color.White;
            this.btDLDS.Location = new System.Drawing.Point(202, 224);
            this.btDLDS.Name = "btDLDS";
            this.btDLDS.Size = new System.Drawing.Size(168, 64);
            this.btDLDS.TabIndex = 3;
            this.btDLDS.Text = "XEM DANH SÁCH";
            this.btDLDS.UseVisualStyleBackColor = false;
            this.btDLDS.Click += new System.EventHandler(this.btDLDS_Click);
            // 
            // btDLTC
            // 
            this.btDLTC.BackColor = System.Drawing.Color.Navy;
            this.btDLTC.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDLTC.ForeColor = System.Drawing.Color.White;
            this.btDLTC.Location = new System.Drawing.Point(376, 224);
            this.btDLTC.Name = "btDLTC";
            this.btDLTC.Size = new System.Drawing.Size(154, 64);
            this.btDLTC.TabIndex = 4;
            this.btDLTC.Text = "TRANG CHỦ";
            this.btDLTC.UseVisualStyleBackColor = false;
            this.btDLTC.Click += new System.EventHandler(this.btDLTC_Click);
            // 
            // FormTBTiepNhanDL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 322);
            this.Controls.Add(this.btDLTC);
            this.Controls.Add(this.btDLDS);
            this.Controls.Add(this.btThemDLkhac);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "FormTBTiepNhanDL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTBTiepNhanDL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btThemDLkhac;
        private System.Windows.Forms.Button btDLDS;
        private System.Windows.Forms.Button btDLTC;
    }
}