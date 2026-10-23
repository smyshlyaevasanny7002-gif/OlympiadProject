using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OlympiadProject
{
    public class SubjectsForm : Form
    {
        private DataGridView grid;
        private TextBox txtName;
        private CheckBox chkProfile;
        private int selectedId = -1;

        public SubjectsForm()
        {
            this.Text = "Управление предметами";
            this.Size = new Size(500, 400);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            grid = new DataGridView { Dock = DockStyle.Top, Height = 250, BackgroundColor = Color.FromArgb(40, 40, 40), AllowUserToAddRows = false, ReadOnly = true };
            grid.CellClick += Grid_CellClick;

            txtName = new TextBox { Location = new Point(20, 280), Width = 200, Text = "Название" };
            chkProfile = new CheckBox { Text = "Профилирующий", Location = new Point(230, 280), Width = 150 };

            Button btnAdd = new Button { Text = "Добавить", Location = new Point(20, 320), BackColor = Color.Green };
            Button btnUpdate = new Button { Text = "Изменить", Location = new Point(120, 320), BackColor = Color.Orange };
            Button btnDelete = new Button { Text = "Удалить", Location = new Point(220, 320), BackColor = Color.Red };

            btnAdd.Click += (s, e) => SaveSubject(false);
            btnUpdate.Click += (s, e) => SaveSubject(true);
            btnDelete.Click += BtnDelete_Click;

            this.Controls.AddRange(new Control[] { grid, txtName, chkProfile, btnAdd, btnUpdate, btnDelete });
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
                grid.DataSource = db.Subjects.ToList();
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = (int)grid.Rows[e.RowIndex].Cells["Id"].Value;
                txtName.Text = grid.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                chkProfile.Checked = (bool)grid.Rows[e.RowIndex].Cells["IsProfile"].Value;
            }
        }

        private void SaveSubject(bool isUpdate)
        {
            using (var db = new AppDbContext())
            {
                if (isUpdate)
                {
                    var subj = db.Subjects.Find(selectedId);
                    if (subj != null) { subj.Name = txtName.Text; subj.IsProfile = chkProfile.Checked; }
                }
                else
                {
                    db.Subjects.Add(new Subject { Name = txtName.Text, IsProfile = chkProfile.Checked });
                }
                db.SaveChanges();
            }
            LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            using (var db = new AppDbContext())
            {
                var subj = db.Subjects.Find(selectedId);
                if (subj != null) { db.Subjects.Remove(subj); db.SaveChanges(); }
            }
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "SubjectsForm";
            this.Load += new System.EventHandler(this.SubjectsForm_Load);
            this.ResumeLayout(false);

        }

        private void SubjectsForm_Load(object sender, EventArgs e)
        {

        }
    }
}