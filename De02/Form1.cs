using De02.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De02
{
    public partial class frmSanPham : Form
    {
        public frmSanPham()
        {
            InitializeComponent();
        }

        private int GetSelectedRow(string MaSP)
        {
            for (int i = 0; i < dgvSanPham.Rows.Count; i++)
            {
                if (dgvSanPham.Rows[i].Cells[0].Value.ToString() == MaSP)
                {
                    return i;
                }
            }
            return -1;
        }
        private void InsertUpdate(int selectedRow)
        {
            dgvSanPham.Rows[selectedRow].Cells[0].Value = txtMaSP.Text;
            dgvSanPham.Rows[selectedRow].Cells[1].Value = txtTenSP.Text;
            dgvSanPham.Rows[selectedRow].Cells[2].Value = dtNgaynhap.Text;
            dgvSanPham.Rows[selectedRow].Cells[3].Value = cboLoaiSP.Text;
        }

        private void btnThemSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMaSP.Text == " " || txtTenSP.Text == " " || cboLoaiSP.Text == " ")
                    throw new Exception("Vui lòng nhập đầy đủ thông tin sinh viên !");
                int selectedRow = GetSelectedRow(txtMaSP.Text);
                if (selectedRow == -1)
                {
                    selectedRow = dgvSanPham.Rows.Add();
                    InsertUpdate(selectedRow);
                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK);
                }
                else
                {
                    InsertUpdate(selectedRow);
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRow = GetSelectedRow(txtMaSP.Text);
                if (selectedRow == -1)
                {
                    throw new Exception("Không tìm thấy MaSP cần xóa!");
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Bạn có muốn xóa ?", "YES/NO", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        dgvSanPham.Rows.RemoveAt(selectedRow);
                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                SanPhamContextDB context = new SanPhamContextDB();
                List<LoaiSP> listLoaiSP = context.LoaiSPs.ToList();
                List<SanPham> listSanPham = context.SanPhams.ToList();
                FillFacultyCombobox(listLoaiSP);
                BindGrid(listSanPham);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<LoaiSP> listLoaiSP)
        {
            this.cboLoaiSP.DataSource = listLoaiSP;
            this.cboLoaiSP.DisplayMember = "TenLoai";
            this.cboLoaiSP.ValueMember = "MaLoai";
        }
        //Hàm binding gridView từ list Sản Phẩm
        private void BindGrid(List<SanPham> listSanPham)
        {
            dgvSanPham.Rows.Clear();
            foreach (var item in listSanPham)
            {
                int index = dgvSanPham.Rows.Add();
                dgvSanPham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanPham.Rows[index].Cells[1].Value = item.TenSP;
                dgvSanPham.Rows[index].Cells[2].Value = item.Ngaynhap;
                dgvSanPham.Rows[index].Cells[3].Value = item.MaLoai;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                SanPhamContextDB context = new SanPhamContextDB();
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.IsNewRow) continue;

                    string maSP = row.Cells[0].Value?.ToString();
                    if (maSP == null) continue;

                    SanPham sp = context.SanPhams.FirstOrDefault(s => s.MaSP == maSP);
                    if (sp == null)
                    {
                        sp = new SanPham
                        {
                            MaSP = row.Cells[0].Value.ToString(),
                            TenSP = row.Cells[1].Value.ToString(),
                            Ngaynhap = DateTime.Parse(row.Cells[2].Value.ToString()),
                            MaLoai = row.Cells[3].Value.ToString()
                        };
                        context.SanPhams.Add(sp);
                    }
                    else
                    {
                        sp.TenSP = row.Cells[1].Value.ToString();
                        sp.Ngaynhap = DateTime.Parse(row.Cells[2].Value.ToString());
                        sp.MaLoai = row.Cells[3].Value.ToString();
                    }
                }
                context.SaveChanges();
                MessageBox.Show("Lưu dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKluu_Click(object sender, EventArgs e)
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            cboLoaiSP.SelectedIndex = -1;
            dtNgaynhap.Value = DateTime.Now;

            frmSanPham_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            SanPhamContextDB context = new SanPhamContextDB();
            string timTenSP = txtTim.Text.Trim();
            string tensp = txtTenSP.Text.Trim();

            var query = context.SanPhams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timTenSP))
            {
                query = query.Where(sp => sp.TenSP.Contains(timTenSP));
            }

            if (!string.IsNullOrWhiteSpace(tensp))
            {
                query = query.Where(sp => sp.TenSP.Contains(tensp));
            }
            List<SanPham> results = query.ToList();
            BindGrid(results);
            if (results.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sản phẩm nào.");
            }
        }

    }
}
