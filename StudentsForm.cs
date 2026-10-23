using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OlympiadProject
{
    public class StudentsForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtGroup;
        private int selectedId = -1;

        public StudentsForm()
        {
            this.Text = "Управление студентами";
            this.Size = new Size(600, 500);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            grid = new DataGridView { Dock = DockStyle.Top, Height = 250, BackgroundColor = Color.FromArgb(40, 40, 40), AllowUserToAddRows = false, ReadOnly = true };
            grid.CellClick += Grid_CellClick;

            txtName = new TextBox { Location = new Point(20, 280), Width = 200, Text = "ФИО" };
            txtGroup = new TextBox { Location = new Point(230, 280), Width = 100, Text = "Группа" };

            Button btnAdd = new Button { Text = "Добавить", Location = new Point(20, 320), BackColor = Color.Green };
            Button btnUpdate = new Button { Text = "Изменить", Location = new Point(120, 320), BackColor = Color.Orange };
            Button btnDelete = new Button { Text = "Удалить", Location = new Point(220, 320), BackColor = Color.Red };

            btnAdd.Click += (s, e) => SaveStudent(false);
            btnUpdate.Click += (s, e) => SaveStudent(true);
            btnDelete.Click += BtnDelete_Click;

            this.Controls.AddRange(new Control[] { grid, txtName, txtGroup, btnAdd, btnUpdate, btnDelete });
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
                grid.DataSource = db.Students.ToList();
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = (int)grid.Rows[e.RowIndex].Cells["Id"].Value;
                txtName.Text = grid.Rows[e.RowIndex].Cells["FullName"].Value.ToString();
                txtGroup.Text = grid.Rows[e.RowIndex].Cells["GroupName"].Value.ToString();
            }
        }

        private void SaveStudent(bool isUpdate)
        {
            using (var db = new AppDbContext())
            {
                if (isUpdate)
                {
                    var student = db.Students.Find(selectedId);
                    if (student != null) { student.FullName = txtName.Text; student.GroupName = txtGroup.Text; }
                }
                else
                {
                    db.Students.Add(new Student { FullName = txtName.Text, GroupName = txtGroup.Text });
                }
                db.SaveChanges();
            }
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
           
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "StudentsForm";
            this.Load += new System.EventHandler(this.StudentsForm_Load);
            this.ResumeLayout(false);

        }

        private void StudentsForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            using (var db = new AppDbContext())
            {
                var student = db.Students.Find(selectedId);
                if (student != null) { db.Students.Remove(student); db.SaveChanges(); }
            }
            LoadData();
        }
    }
}