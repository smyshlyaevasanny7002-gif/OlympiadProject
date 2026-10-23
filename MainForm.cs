using System;
using System.Drawing;
using System.Windows.Forms;

namespace OlympiadProject
{
    public class MainForm : Form
    {
        private TextBox txtThreshold;
        private DataGridView gridResults;

        public MainForm()
        {
            this.Text = "Олимпиада: Главное меню";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            Button btnStudents = CreateNavButton("Студенты", 20);
            btnStudents.Click += (s, e) => new StudentsForm().ShowDialog();

            Button btnSubjects = CreateNavButton("Предметы", 70);
            btnSubjects.Click += (s, e) => new SubjectsForm().ShowDialog();

            Button btnGrades = CreateNavButton("Оценки", 120);
            btnGrades.Click += (s, e) => new GradesForm().ShowDialog();

            Label lbl = new Label { Text = "Порог балла:", Location = new Point(200, 30), AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtThreshold = new TextBox { Location = new Point(320, 28), Width = 80, Text = "4.5", Font = new Font("Segoe UI", 12) };

            Button btnCalculate = new Button { Text = "Рассчитать", Location = new Point(420, 25), Size = new Size(120, 30), BackColor = Color.DodgerBlue, FlatStyle = FlatStyle.Flat };
            btnCalculate.Click += BtnCalculate_Click;

            gridResults = new DataGridView { Location = new Point(200, 80), Size = new Size(650, 450), BackgroundColor = Color.FromArgb(40, 40, 40), AllowUserToAddRows = false, ReadOnly = true };
            gridResults.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            gridResults.DefaultCellStyle.ForeColor = Color.White;
            gridResults.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSlateGray;

            this.Controls.AddRange(new Control[] { btnStudents, btnSubjects, btnGrades, lbl, txtThreshold, btnCalculate, gridResults });
        }

        private Button CreateNavButton(string text, int y)
        {
            return new Button { Text = text, Location = new Point(20, y), Size = new Size(150, 40), BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtThreshold.Text.Replace(",", "."), out double threshold))
            {
                var service = new OlympiadService();
                gridResults.DataSource = service.GetSelectedStudents(threshold);
                if (gridResults.Columns.Count > 0)
                {
                    gridResults.Columns["FullName"].HeaderText = "ФИО";
                    gridResults.Columns["GroupName"].HeaderText = "Группа";
                    gridResults.Columns["AverageScore"].HeaderText = "Средний балл";
                }
            }
            else MessageBox.Show("Введите корректное число!");
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}