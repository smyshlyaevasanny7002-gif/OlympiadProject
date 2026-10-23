using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace OlympiadProject
{
    public class MainDashboard : Form
    {
        private Panel pnlSidebar, pnlMain, pnlResults, pnlStudents, pnlSubjects, pnlGrades, pnlOlympiads;
        private DataGridView gridResults, gridStudents, gridSubjects, gridGrades, gridOlympiads;
        private Label lblTitle;

        private TextBox txtSearchStudents, txtSearchSubjects, txtSearchOlympiads;
        private ComboBox cmbFilterOlympiadsForResults, cmbFilterSubjectsForResults;

        private BindingSource bsStudents = new BindingSource();
        private BindingSource bsSubjects = new BindingSource();
        private BindingSource bsOlympiads = new BindingSource();

        private Label lblCountStudents, lblCountOlympiads, lblCountGrades, lblCountSubjects;

        public MainDashboard()
        {
            SetupUI();
            SeedDatabaseIfEmpty();
            LoadData();
            ShowPanel(pnlResults);
        }

        private void SetupUI()
        {
            this.Text = "Система отбора на олимпиаду (v4.0)";
            this.Size = new Size(1350, 850);
            this.MinimumSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 10);

            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Color.FromArgb(28, 35, 49) };
            Label lblLogo = new Label { Text = "ОЛИМПИАДЫ", ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), Dock = DockStyle.Top, Height = 80, TextAlign = ContentAlignment.MiddleCenter };
            pnlSidebar.Controls.Add(lblLogo);

            Button btnResults = CreateMenuButton("📊 Расчет отбора", 80);
            Button btnStudents = CreateMenuButton("👨‍🎓 Студенты", 140);
            Button btnSubjects = CreateMenuButton("📚 Предметы", 200);
            Button btnGrades = CreateMenuButton("📝 Оценки", 260);
            Button btnOlympiads = CreateMenuButton("🏆 Олимпиады", 320);

            btnResults.Click += (s, e) => ShowPanel(pnlResults);
            btnStudents.Click += (s, e) => ShowPanel(pnlStudents);
            btnSubjects.Click += (s, e) => ShowPanel(pnlSubjects);
            btnGrades.Click += (s, e) => ShowPanel(pnlGrades);
            btnOlympiads.Click += (s, e) => ShowPanel(pnlOlympiads);

            pnlSidebar.Controls.AddRange(new Control[] { btnResults, btnStudents, btnSubjects, btnGrades, btnOlympiads });

            pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };
            lblTitle = new Label { Text = "Панель управления", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(28, 35, 49), Dock = DockStyle.Top, Height = 60 };
            pnlMain.Controls.Add(lblTitle);

            CreateStatsCards(); 
            CreateResultsPanel();
            CreateStudentsPanel();
            CreateSubjectsPanel();
            CreateGradesPanel();
            CreateOlympiadsPanel();

            pnlMain.Controls.AddRange(new Control[] { pnlResults, pnlStudents, pnlSubjects, pnlGrades, pnlOlympiads });
            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
        }

        private Button CreateMenuButton(string text, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(220, 50),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                BackColor = Color.FromArgb(28, 35, 49),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
        }

        private Panel statsPanel;
        private void CreateStatsCards()
        {
            statsPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.Transparent };

            int cardWidth = 200;
            int cardSpacing = 20;

            statsPanel.Controls.Add(CreateStatCard("Студентов", Color.FromArgb(52, 152, 219), out lblCountStudents, 0));
            statsPanel.Controls.Add(CreateStatCard("Олимпиад", Color.FromArgb(155, 89, 182), out lblCountOlympiads, cardWidth + cardSpacing));
            statsPanel.Controls.Add(CreateStatCard("Предметов", Color.FromArgb(46, 204, 113), out lblCountSubjects, (cardWidth + cardSpacing) * 2));
            statsPanel.Controls.Add(CreateStatCard("Оценок", Color.FromArgb(241, 196, 15), out lblCountGrades, (cardWidth + cardSpacing) * 3));

            pnlMain.Controls.Add(statsPanel);
        }

        private Panel CreateStatCard(string title, Color color, out Label valueLabel, int xPos)
        {
            Panel card = new Panel { Location = new Point(xPos, 0), Size = new Size(200, 90), BackColor = Color.White };
            card.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(color), 0, 0, 10, card.Height);
            };

            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 10, FontStyle.Regular), ForeColor = Color.Gray, Location = new Point(20, 15), AutoSize = true };
            valueLabel = new Label { Text = "0", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(28, 35, 49), Location = new Point(20, 35), AutoSize = true };

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);
            return card;
        }

        private void ShowPanel(Panel panelToShow)
        {
            pnlResults.Visible = false; pnlStudents.Visible = false; pnlSubjects.Visible = false;
            pnlGrades.Visible = false; pnlOlympiads.Visible = false;

            panelToShow.Visible = true; panelToShow.Dock = DockStyle.Fill; panelToShow.BringToFront();

            statsPanel.Visible = (panelToShow == pnlResults);

            if (panelToShow == pnlStudents) lblTitle.Text = "Управление студентами";
            else if (panelToShow == pnlSubjects) lblTitle.Text = "Управление предметами";
            else if (panelToShow == pnlGrades) lblTitle.Text = "Управление оценками";
            else if (panelToShow == pnlOlympiads) lblTitle.Text = "Управление олимпиадами";
            else lblTitle.Text = "Расчет проходного балла";

            LoadData();
        }

        private void CreateResultsPanel()
        {
            pnlResults = new Panel { Visible = false };
            Panel topFilter = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(20) };

            Label lblOlymp = new Label { Text = "Олимпиада:", Location = new Point(20, 25), AutoSize = true };
            cmbFilterOlympiadsForResults = new ComboBox { Location = new Point(110, 22), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterOlympiadsForResults.SelectedIndexChanged += (s, e) => UpdateSubjectsCombo();

            Label lblSubject = new Label { Text = "Предмет (опц.):", Location = new Point(330, 25), AutoSize = true };
            cmbFilterSubjectsForResults = new ComboBox { Location = new Point(440, 22), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblThreshold = new Label { Text = "Порог:", Location = new Point(640, 25), AutoSize = true };
            NumericUpDown numThreshold = new NumericUpDown { Location = new Point(700, 22), Width = 60, DecimalPlaces = 1, Minimum = 2, Maximum = 5, Value = 4.5M };

            Button btnCalc = new Button { Text = "Выполнить отбор", Location = new Point(790, 20), Size = new Size(160, 32), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnCalc.Click += (s, e) => {
                if (cmbFilterOlympiadsForResults.SelectedItem == null) { MessageBox.Show("Создайте олимпиаду!"); return; }

                int olimpId = 0;
                if (cmbFilterOlympiadsForResults.SelectedItem is Olympiad selectedOlimp) olimpId = selectedOlimp.Id;
                else if (cmbFilterOlympiadsForResults.SelectedValue is int id) olimpId = id;

                int? subjectId = null;
                if (cmbFilterSubjectsForResults.SelectedValue != null && cmbFilterSubjectsForResults.SelectedValue is int subId && subId > 0) subjectId = subId;

                var service = new OlympiadService();
                gridResults.DataSource = service.GetSelectedStudents((double)numThreshold.Value, olimpId, subjectId);

                if (gridResults.Columns.Count > 0)
                {
                    gridResults.Columns["FullName"].HeaderText = "ФИО Студента";
                    gridResults.Columns["GroupName"].HeaderText = "Группа";
                    gridResults.Columns["AverageScore"].HeaderText = "Средний балл";
                    gridResults.Columns["ProfileSubjectsCount"].HeaderText = "Профильных предметов";
                }
            };

            topFilter.Controls.AddRange(new Control[] { lblOlymp, cmbFilterOlympiadsForResults, lblSubject, cmbFilterSubjectsForResults, lblThreshold, numThreshold, btnCalc });

            gridResults = CreateStyledGrid();
            gridResults.Dock = DockStyle.Fill;
            gridResults.Padding = new Padding(0, 110, 0, 0);

            pnlResults.Controls.Add(gridResults);
            pnlResults.Controls.Add(topFilter);
        }

        private void UpdateSubjectsCombo()
        {
            if (cmbFilterOlympiadsForResults.SelectedItem == null) return;
            int olimpId = 0;
            if (cmbFilterOlympiadsForResults.SelectedItem is Olympiad selectedOlimp) olimpId = selectedOlimp.Id;
            else if (cmbFilterOlympiadsForResults.SelectedValue is int id) olimpId = id;
            else return;

            using (var db = new AppDbContext())
            {
                var subjects = db.OlympiadSubjects
                    .Include(os => os.Subject)
                    .Where(os => os.OlympiadId == olimpId && os.IsProfile)
                    .Select(os => os.Subject).ToList();

                var allSubjects = new[] { new Subject { Id = 0, Name = "Все предметы" } }.Concat(subjects).ToList();

                cmbFilterSubjectsForResults.DataSource = allSubjects;
                cmbFilterSubjectsForResults.DisplayMember = "Name";
                cmbFilterSubjectsForResults.ValueMember = "Id";
            }
        }

        private void CreateStudentsPanel()
        {
            pnlStudents = new Panel { Visible = false };
            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(15) };

            txtSearchStudents = new TextBox { Location = new Point(10, 20), Width = 250, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Text = "🔍 Поиск по ФИО..." };
            txtSearchStudents.Enter += (s, e) => { if (txtSearchStudents.Text.Contains("Поиск")) { txtSearchStudents.Text = ""; txtSearchStudents.ForeColor = Color.Black; } };
            txtSearchStudents.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchStudents.Text)) { txtSearchStudents.Text = "🔍 Поиск по ФИО..."; txtSearchStudents.ForeColor = Color.Gray; } };
            txtSearchStudents.TextChanged += (s, e) => { if (txtSearchStudents.Text != "🔍 Поиск по ФИО...") bsStudents.Filter = $"FullName LIKE '%{txtSearchStudents.Text}%'"; };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(280, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(410, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(540, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новый студент", new[] { "ФИО", "Группа", "Факультет", "Email" }, new string[4]);
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    using (var db = new AppDbContext()) { db.Students.Add(new Student { FullName = v[0], GroupName = v[1], Faculty = v[2], Email = v[3] }); db.SaveChanges(); }
                    LoadData();
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridStudents.CurrentRow == null) return;
                var student = (Student)gridStudents.CurrentRow.DataBoundItem;
                var form = new EditForm("Редактировать", new[] { "ФИО", "Группа", "Факультет", "Email" }, new[] { student.FullName, student.GroupName, student.Faculty, student.Email });
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    using (var db = new AppDbContext())
                    {
                        var studentToUpdate = db.Students.Find(student.Id);
                        studentToUpdate.FullName = v[0]; studentToUpdate.GroupName = v[1]; studentToUpdate.Faculty = v[2]; studentToUpdate.Email = v[3];
                        db.SaveChanges();
                    }
                    LoadData();
                }
            };

            btnDelete.Click += (s, e) => {
                if (gridStudents.CurrentRow == null) return;
                if (MessageBox.Show("Удалить студента?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var student = (Student)gridStudents.CurrentRow.DataBoundItem;
                    using (var db = new AppDbContext())
                    {
                        var studentToDelete = db.Students.Find(student.Id);
                        db.Students.Remove(studentToDelete); db.SaveChanges();
                    }
                    LoadData();
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchStudents, btnAdd, btnEdit, btnDelete });
            gridStudents = CreateStyledGrid();
            gridStudents.Dock = DockStyle.Fill;
            gridStudents.DataSource = bsStudents;
            gridStudents.ReadOnly = true;

            gridStudents.DataBindingComplete += (s, e) => {
                if (gridStudents.Columns["FullName"] != null) gridStudents.Columns["FullName"].HeaderText = "ФИО";
                if (gridStudents.Columns["GroupName"] != null) gridStudents.Columns["GroupName"].HeaderText = "Группа";
                if (gridStudents.Columns["Faculty"] != null) gridStudents.Columns["Faculty"].HeaderText = "Факультет";
                if (gridStudents.Columns["Email"] != null) gridStudents.Columns["Email"].HeaderText = "Email";
            };

            pnlStudents.Controls.Add(gridStudents);
            pnlStudents.Controls.Add(topBar);
        }

        private void CreateSubjectsPanel()
        {
            pnlSubjects = new Panel { Visible = false };
            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(15) };

            txtSearchSubjects = new TextBox { Location = new Point(10, 20), Width = 250, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Text = "🔍 Поиск по названию..." };
            txtSearchSubjects.Enter += (s, e) => { if (txtSearchSubjects.Text.Contains("Поиск")) { txtSearchSubjects.Text = ""; txtSearchSubjects.ForeColor = Color.Black; } };
            txtSearchSubjects.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchSubjects.Text)) { txtSearchSubjects.Text = "🔍 Поиск по названию..."; txtSearchSubjects.ForeColor = Color.Gray; } };
            txtSearchSubjects.TextChanged += (s, e) => { if (txtSearchSubjects.Text != "🔍 Поиск по названию...") bsSubjects.Filter = $"Name LIKE '%{txtSearchSubjects.Text}%'"; };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(280, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(410, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(540, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новый предмет", new[] { "Название", "Преподаватель" }, new string[2]);
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    using (var db = new AppDbContext()) { db.Subjects.Add(new Subject { Name = v[0], Teacher = v[1] }); db.SaveChanges(); }
                    LoadData();
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridSubjects.CurrentRow == null) return;
                var subj = (Subject)gridSubjects.CurrentRow.DataBoundItem;
                var form = new EditForm("Редактировать", new[] { "Название", "Преподаватель" }, new[] { subj.Name, subj.Teacher });
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    using (var db = new AppDbContext())
                    {
                        var subjectToUpdate = db.Subjects.Find(subj.Id);
                        subjectToUpdate.Name = v[0]; subjectToUpdate.Teacher = v[1];
                        db.SaveChanges();
                    }
                    LoadData();
                }
            };

            btnDelete.Click += (s, e) => {
                if (gridSubjects.CurrentRow == null) return;
                if (MessageBox.Show("Удалить предмет?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var subj = (Subject)gridSubjects.CurrentRow.DataBoundItem;
                    using (var db = new AppDbContext())
                    {
                        var subjectToDelete = db.Subjects.Find(subj.Id);
                        db.Subjects.Remove(subjectToDelete); db.SaveChanges();
                    }
                    LoadData();
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchSubjects, btnAdd, btnEdit, btnDelete });
            gridSubjects = CreateStyledGrid();
            gridSubjects.Dock = DockStyle.Fill;
            gridSubjects.DataSource = bsSubjects;
            gridSubjects.ReadOnly = true;

            gridSubjects.DataBindingComplete += (s, e) => {
                if (gridSubjects.Columns["Name"] != null) gridSubjects.Columns["Name"].HeaderText = "Название предмета";
                if (gridSubjects.Columns["Teacher"] != null) gridSubjects.Columns["Teacher"].HeaderText = "Преподаватель";
            };

            pnlSubjects.Controls.Add(gridSubjects);
            pnlSubjects.Controls.Add(topBar);
        }

        private void CreateGradesPanel()
        {
            pnlGrades = new Panel { Visible = false };
            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(15) };

            Button btnAdd = new Button { Text = "+ Выставить оценку", Location = new Point(10, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 180, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new Form { Text = "Выставить оценку", Size = new Size(420, 280), StartPosition = FormStartPosition.CenterParent, BackColor = Color.FromArgb(245, 247, 250), Font = new Font("Segoe UI", 10) };

                ComboBox cmbSt = new ComboBox { Location = new Point(20, 40), Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
                ComboBox cmbSb = new ComboBox { Location = new Point(20, 100), Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
                TextBox txtSc = new TextBox { Location = new Point(20, 160), Width = 100, Text = "5" };
                Button btnSave = new Button { Text = "Сохранить", Location = new Point(20, 200), Width = 150, Height = 35, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

                using (var db = new AppDbContext())
                {
                    cmbSt.DataSource = db.Students.ToList(); cmbSt.DisplayMember = "FullName"; cmbSt.ValueMember = "Id";
                    cmbSb.DataSource = db.Subjects.ToList(); cmbSb.DisplayMember = "Name"; cmbSb.ValueMember = "Id";
                }

                btnSave.Click += (s2, e2) => {
                    using (var db = new AppDbContext())
                    {
                        db.Grades.Add(new Grade { StudentId = (int)cmbSt.SelectedValue, SubjectId = (int)cmbSb.SelectedValue, Score = int.Parse(txtSc.Text), Date = DateTime.Now.ToShortDateString() });
                        db.SaveChanges();
                    }
                    form.Close(); LoadData();
                };

                form.Controls.AddRange(new Control[] {
                    new Label { Text = "Студент:", Location = new Point(20, 15) }, cmbSt,
                    new Label { Text = "Предмет:", Location = new Point(20, 75) }, cmbSb,
                    new Label { Text = "Оценка:", Location = new Point(20, 135) }, txtSc, btnSave
                });
                form.ShowDialog();
            };

            topBar.Controls.AddRange(new Control[] { btnAdd });
            gridGrades = CreateStyledGrid();
            gridGrades.Dock = DockStyle.Fill;

            gridGrades.DataBindingComplete += (s, e) => {
                if (gridGrades.Columns["Студент"] != null) gridGrades.Columns["Студент"].HeaderText = "ФИО Студента";
                if (gridGrades.Columns["Предмет"] != null) gridGrades.Columns["Предмет"].HeaderText = "Предмет";
                if (gridGrades.Columns["Оценка"] != null) gridGrades.Columns["Оценка"].HeaderText = "Оценка";
                if (gridGrades.Columns["Дата"] != null) gridGrades.Columns["Дата"].HeaderText = "Дата выставления";
            };

            pnlGrades.Controls.Add(gridGrades);
            pnlGrades.Controls.Add(topBar);
        }

        private void CreateOlympiadsPanel()
        {
            pnlOlympiads = new Panel { Visible = false };
            Panel topBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(15) };

            txtSearchOlympiads = new TextBox { Location = new Point(10, 20), Width = 250, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Text = "🔍 Поиск по названию..." };
            txtSearchOlympiads.Enter += (s, e) => { if (txtSearchOlympiads.Text.Contains("Поиск")) { txtSearchOlympiads.Text = ""; txtSearchOlympiads.ForeColor = Color.Black; } };
            txtSearchOlympiads.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchOlympiads.Text)) { txtSearchOlympiads.Text = "🔍 Поиск по названию..."; txtSearchOlympiads.ForeColor = Color.Gray; } };
            txtSearchOlympiads.TextChanged += (s, e) => { if (txtSearchOlympiads.Text != "🔍 Поиск по названию...") bsOlympiads.Filter = $"Name LIKE '%{txtSearchOlympiads.Text}%'"; };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(280, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(410, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(540, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новая олимпиада", new[] { "Название", "Уровень", "Проходной балл" }, new[] { "", "", "4,5" });
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    double.TryParse(v[2].Replace(".", ","), out double score);
                    using (var db = new AppDbContext()) { db.Olympiads.Add(new Olympiad { Name = v[0], Level = v[1], PassingScore = score }); db.SaveChanges(); }
                    LoadData();
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridOlympiads.CurrentRow == null) return;
                var olimp = (Olympiad)gridOlympiads.CurrentRow.DataBoundItem;
                var form = new EditForm("Редактировать", new[] { "Название", "Уровень", "Проходной балл" }, new[] { olimp.Name, olimp.Level, olimp.PassingScore.ToString() });
                if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                {
                    var v = form.GetValues();
                    double.TryParse(v[2].Replace(".", ","), out double score);
                    using (var db = new AppDbContext())
                    {
                        var olimpToUpdate = db.Olympiads.Find(olimp.Id);
                        olimpToUpdate.Name = v[0]; olimpToUpdate.Level = v[1]; olimpToUpdate.PassingScore = score;
                        db.SaveChanges();
                    }
                    LoadData();
                }
            };

            btnDelete.Click += (s, e) => {
                if (gridOlympiads.CurrentRow == null) return;
                if (MessageBox.Show("Удалить олимпиаду?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var olimp = (Olympiad)gridOlympiads.CurrentRow.DataBoundItem;
                    using (var db = new AppDbContext())
                    {
                        var olimpToDelete = db.Olympiads.Find(olimp.Id);
                        db.Olympiads.Remove(olimpToDelete); db.SaveChanges();
                    }
                    LoadData();
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchOlympiads, btnAdd, btnEdit, btnDelete });
            gridOlympiads = CreateStyledGrid();
            gridOlympiads.Dock = DockStyle.Fill;
            gridOlympiads.DataSource = bsOlympiads;
            gridOlympiads.ReadOnly = true;

            gridOlympiads.DataBindingComplete += (s, e) => {
                if (gridOlympiads.Columns["Name"] != null) gridOlympiads.Columns["Name"].HeaderText = "Название олимпиады";
                if (gridOlympiads.Columns["Level"] != null) gridOlympiads.Columns["Level"].HeaderText = "Уровень";
                if (gridOlympiads.Columns["PassingScore"] != null) gridOlympiads.Columns["PassingScore"].HeaderText = "Проходной балл";
            };

            pnlOlympiads.Controls.Add(gridOlympiads);
            pnlOlympiads.Controls.Add(topBar);
        }

        private DataGridView CreateStyledGrid()
        {
            var grid = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 230, 230),
                ColumnHeadersHeight = 45
            };
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(28, 35, 49);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 40;
            return grid;
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                bsStudents.DataSource = db.Students.ToList();
                if (gridStudents.Columns["Id"] != null) gridStudents.Columns["Id"].Visible = false;

                bsSubjects.DataSource = db.Subjects.ToList();
                if (gridSubjects.Columns["Id"] != null) gridSubjects.Columns["Id"].Visible = false;

                bsOlympiads.DataSource = db.Olympiads.ToList();
                if (gridOlympiads.Columns["Id"] != null) gridOlympiads.Columns["Id"].Visible = false;

                cmbFilterOlympiadsForResults.DataSource = db.Olympiads.ToList();
                cmbFilterOlympiadsForResults.DisplayMember = "Name";
                cmbFilterOlympiadsForResults.ValueMember = "Id";

                var gradesView = db.Grades.Include(g => g.Student).Include(g => g.Subject)
                    .Select(g => new {
                        g.Id,
                        Студент = g.Student.FullName,
                        Предмет = g.Subject.Name,
                        Оценка = g.Score,
                        Дата = g.Date
                    }).ToList();
                gridGrades.DataSource = gradesView;
                if (gridGrades.Columns["Id"] != null) gridGrades.Columns["Id"].Visible = false;

                lblCountStudents.Text = db.Students.Count().ToString();
                lblCountOlympiads.Text = db.Olympiads.Count().ToString();
                lblCountSubjects.Text = db.Subjects.Count().ToString();
                lblCountGrades.Text = db.Grades.Count().ToString();
            }
        }

        private void SeedDatabaseIfEmpty()
        {
            using (var db = new AppDbContext())
            {
                if (!db.Students.Any())
                {
                    var math = new Subject { Name = "Математика", Teacher = "Смирнов А.А." };
                    var prog = new Subject { Name = "Информатика", Teacher = "Козлов В.В." };
                    var rus = new Subject { Name = "Русский язык", Teacher = "Иванова Е.П." };
                    db.Subjects.AddRange(math, prog, rus);

                    var olimp1 = new Olympiad { Name = "Всероссийская олимпиада", Level = "Всероссийская", PassingScore = 4.5 };
                    var olimp2 = new Olympiad { Name = "Городская олимпиада", Level = "Городская", PassingScore = 4.0 };
                    db.Olympiads.AddRange(olimp1, olimp2);
                    db.SaveChanges();

                    db.OlympiadSubjects.AddRange(
                        new OlympiadSubject { OlympiadId = olimp1.Id, SubjectId = math.Id, IsProfile = true },
                        new OlympiadSubject { OlympiadId = olimp1.Id, SubjectId = prog.Id, IsProfile = true },
                        new OlympiadSubject { OlympiadId = olimp2.Id, SubjectId = math.Id, IsProfile = true }
                    );
                    db.SaveChanges();

                    var s1 = new Student { FullName = "Иванов Иван", GroupName = "П-23", Faculty = "ИТ", Email = "ivanov@mail.ru" };
                    var s2 = new Student { FullName = "Петрова Анна", GroupName = "П-23", Faculty = "ИТ", Email = "petrova@mail.ru" };
                    db.Students.AddRange(s1, s2);
                    db.SaveChanges();

                    db.Grades.AddRange(
                        new Grade { StudentId = s1.Id, SubjectId = math.Id, Score = 5, Date = "10.10.2023" },
                        new Grade { StudentId = s1.Id, SubjectId = prog.Id, Score = 4, Date = "11.10.2023" },
                        new Grade { StudentId = s2.Id, SubjectId = math.Id, Score = 5, Date = "10.10.2023" },
                        new Grade { StudentId = s2.Id, SubjectId = prog.Id, Score = 5, Date = "11.10.2023" }
                    );
                    db.SaveChanges();
                }
            }
        }
    } 
} 