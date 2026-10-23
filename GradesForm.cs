using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace OlympiadProject
{
    public class GradesForm : Form
    {
        private DataGridView grid;
        private ComboBox cmbStudent, cmbSubject;
        private TextBox txtScore;
        private int selectedId = -1;

        public GradesForm()
        {
            this.Text = "Управление оценками";
            this.Size = new Size(700, 500);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            grid = new DataGridView { Dock = DockStyle.Top, Height = 300, BackgroundColor = Color.FromArgb(40, 40, 40), AllowUserToAddRows = false, ReadOnly = true };
            grid.CellClick += Grid_CellClick;

            cmbStudent = new ComboBox { Location = new Point(20, 320), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSubject = new ComboBox { Location = new Point(230, 320), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            txtScore = new TextBox { Location = new Point(390, 320), Width = 50, Text = "5" };

            Button btnAdd = new Button { Text = "Добавить", Location = new Point(20, 360), BackColor = Color.Green };
            Button btnUpdate = new Button { Text = "Изменить", Location = new Point(120, 360), BackColor = Color.Orange };
            Button btnDelete = new Button { Text = "Удалить", Location = new Point(220, 360), BackColor = Color.Red };

            btnAdd.Click += (s, e) => SaveGrade(false);
            btnUpdate.Click += (s, e) => SaveGrade(true);
            btnDelete.Click += BtnDelete_Click;

            this.Controls.AddRange(new Control[] { grid, cmbStudent, cmbSubject, txtScore, btnAdd, btnUpdate, btnDelete });
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
                var grades = db.Grades.Include(g => g.Student).Include(g => g.Subject)
                    .Select(g => new { g.Id, Студент = g.Student.FullName, Предмет = g.Subject.Name, Оценка = g.Score }).ToList();
                grid.DataSource = grades;

                cmbStudent.DataSource = db.Students.ToList();
                cmbStudent.DisplayMember = "FullName";
                cmbStudent.ValueMember = "Id";

                cmbSubject.DataSource = db.Subjects.ToList();
                cmbSubject.DisplayMember = "Name";
                cmbSubject.ValueMember = "Id";
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = (int)grid.Rows[e.RowIndex].Cells["Id"].Value;
                using (var db = new AppDbContext())
                {
                    var grade = db.Grades.Find(selectedId);
                    if (grade != null)
                    {
                        cmbStudent.SelectedValue = grade.StudentId;
                        cmbSubject.SelectedValue = grade.SubjectId;
                        txtScore.Text = grade.Score.ToString();
                    }
                }
            }
        }

        private void SaveGrade(bool isUpdate)
        {
            if (!int.TryParse(txtScore.Text, out int score) || score < 2 || score > 5)
            {
                MessageBox.Show("Оценка должна быть от 2 до 5!");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (isUpdate)
                {
                    var grade = db.Grades.Find(selectedId);
                    if (grade != null)
                    {
                        grade.StudentId = (int)cmbStudent.SelectedValue;
                        grade.SubjectId = (int)cmbSubject.SelectedValue;
                        grade.Score = score;
                    }
                }
                else
                {
                    db.Grades.Add(new Grade
                    {
                        StudentId = (int)cmbStudent.SelectedValue,
                        SubjectId = (int)cmbSubject.SelectedValue,
                        Score = score
                    });
                }
                db.SaveChanges();
            }
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
          
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "GradesForm";
            this.Load += new System.EventHandler(this.GradesForm_Load);
            this.ResumeLayout(false);

        }

        private void GradesForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            using (var db = new AppDbContext())
            {
                var grade = db.Grades.Find(selectedId);
                if (grade != null) { db.Grades.Remove(grade); db.SaveChanges(); }
            }
            LoadData();
        }
    }
}