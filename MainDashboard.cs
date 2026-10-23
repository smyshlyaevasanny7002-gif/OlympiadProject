using System;
using System.Data;
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

        private DataView dvStudents, dvSubjects, dvOlympiads;

        private Label lblCountStudents, lblCountOlympiads, lblCountGrades, lblCountSubjects;
        private Panel statsPanel;
        private Label lblResultSummary;

        public MainDashboard()
        {
            AppDbContext.EnsureDatabaseUpdated();
            SetupUI();
            LoadData();
            ShowPanel(pnlResults);
        }

        private void SetupUI()
        {
            this.Text = "Система отбора на олимпиаду";
            this.Size = new Size(1300, 800);
            this.MinimumSize = new Size(1200, 700);
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

        private void CreateStatsCards()
        {
            statsPanel = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.Transparent };
            int cardWidth = 180, cardSpacing = 15;

            statsPanel.Controls.Add(CreateStatCard("Студентов", Color.FromArgb(52, 152, 219), out lblCountStudents, 0, cardWidth));
            statsPanel.Controls.Add(CreateStatCard("Олимпиад", Color.FromArgb(155, 89, 182), out lblCountOlympiads, cardWidth + cardSpacing, cardWidth));
            statsPanel.Controls.Add(CreateStatCard("Предметов", Color.FromArgb(46, 204, 113), out lblCountSubjects, (cardWidth + cardSpacing) * 2, cardWidth));
            statsPanel.Controls.Add(CreateStatCard("Оценок", Color.FromArgb(241, 196, 15), out lblCountGrades, (cardWidth + cardSpacing) * 3, cardWidth));

            pnlMain.Controls.Add(statsPanel);
        }

        private Panel CreateStatCard(string title, Color color, out Label valueLabel, int xPos, int width)
        {
            Panel card = new Panel { Location = new Point(xPos, 0), Size = new Size(width, 80), BackColor = Color.White };
            card.Paint += (s, e) => { e.Graphics.FillRectangle(new SolidBrush(color), 0, 0, 8, card.Height); };

            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 9, FontStyle.Regular), ForeColor = Color.Gray, Location = new Point(15, 10), AutoSize = true };
            valueLabel = new Label { Text = "0", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(28, 35, 49), Location = new Point(15, 30), AutoSize = true };

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
            pnlResults = new Panel { Visible = false, Padding = new Padding(0, 10, 0, 0) };

            TableLayoutPanel topFilter = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10),
                ColumnCount = 5,
                RowCount = 1
            };
            topFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            topFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            topFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            topFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            topFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));

            Panel pnlOlimp = new Panel { Dock = DockStyle.Fill };
            pnlOlimp.Controls.Add(new Label { Text = "Олимпиада:", Location = new Point(0, 0), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            cmbFilterOlympiadsForResults = new ComboBox { Location = new Point(0, 25), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterOlympiadsForResults.SelectedIndexChanged += (s, e) => UpdateSubjectsCombo();
            pnlOlimp.Controls.Add(cmbFilterOlympiadsForResults);

            Panel pnlSubj = new Panel { Dock = DockStyle.Fill };
            pnlSubj.Controls.Add(new Label { Text = "Предмет (опц.):", Location = new Point(0, 0), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            cmbFilterSubjectsForResults = new ComboBox { Location = new Point(0, 25), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlSubj.Controls.Add(cmbFilterSubjectsForResults);

            Button btnCalc = new Button
            {
                Text = "Выполнить отбор",
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            btnCalc.Click += (s, e) => PerformCalculation();

            topFilter.Controls.Add(pnlOlimp, 0, 0);
            topFilter.Controls.Add(pnlSubj, 1, 0);
            topFilter.Controls.Add(new Panel(), 2, 0);
            topFilter.Controls.Add(new Panel(), 3, 0);
            topFilter.Controls.Add(btnCalc, 4, 0);

            Panel pnlSummary = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(235, 245, 255), Padding = new Padding(20, 0, 20, 0) };
            lblResultSummary = new Label
            {
                Text = "Выберите олимпиаду и нажмите 'Выполнить отбор'...",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            pnlSummary.Controls.Add(lblResultSummary);

            gridResults = CreateStyledGrid();
            gridResults.Dock = DockStyle.Fill;
            gridResults.Padding = new Padding(0, 10, 0, 0);

            gridResults.DataBindingComplete += (s, e) => {
                if (gridResults.Columns["FullName"] != null) gridResults.Columns["FullName"].HeaderText = "ФИО Студента";
                if (gridResults.Columns["GroupName"] != null) gridResults.Columns["GroupName"].HeaderText = "Группа";
                if (gridResults.Columns["AverageScore"] != null) gridResults.Columns["AverageScore"].HeaderText = "Средний балл";
                if (gridResults.Columns["ProfileSubjectsCount"] != null) gridResults.Columns["ProfileSubjectsCount"].HeaderText = "Профильных предметов";
            };

            pnlResults.Controls.Add(gridResults);
            pnlResults.Controls.Add(pnlSummary);
            pnlResults.Controls.Add(topFilter);
        }

        private void PerformCalculation()
        {
            if (cmbFilterOlympiadsForResults.SelectedItem == null) { MessageBox.Show("Создайте олимпиаду!"); return; }

            int olimpId = 0;
            if (cmbFilterOlympiadsForResults.SelectedItem is Olympiad selectedOlimp) olimpId = selectedOlimp.Id;
            else if (cmbFilterOlympiadsForResults.SelectedValue is int id) olimpId = id;

            int? subjectId = null;
            if (cmbFilterSubjectsForResults.SelectedValue != null && cmbFilterSubjectsForResults.SelectedValue is int subId && subId > 0) subjectId = subId;

            double threshold = 0;
            using (var db = new AppDbContext())
            {
                var olimp = db.Olympiads.Find(olimpId);
                if (olimp != null) threshold = olimp.PassingScore;
            }

            var service = new OlympiadService();
            var results = service.GetSelectedStudents(threshold, olimpId, subjectId);
            gridResults.DataSource = results;

            lblResultSummary.Text = $"Проходной балл: {threshold}. Успешно прошли отбор: {results.Count} чел.";
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
            txtSearchStudents.TextChanged += (s, e) => {
                if (dvStudents == null) return;
                if (txtSearchStudents.Text.Contains("Поиск")) dvStudents.RowFilter = "";
                else dvStudents.RowFilter = $"FullName LIKE '%{txtSearchStudents.Text}%'";
            };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(280, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(410, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(540, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новый студент", new[] { "ФИО", "Группа", "Факультет", "Email" }, new string[4]);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var v = form.GetValues();
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            db.Students.Add(new Student { FullName = v[0], GroupName = v[1], Faculty = v[2], Email = v[3] });
                            db.SaveChanges();
                        }
                        LoadData();
                        MessageBox.Show("Успешно добавлено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridStudents.CurrentRow == null) return;
                int id = Convert.ToInt32(((DataRowView)gridStudents.CurrentRow.DataBoundItem)["Id"]);
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var student = db.Students.Find(id);
                        if (student == null) return;
                        var form = new EditForm("Редактировать", new[] { "ФИО", "Группа", "Факультет", "Email" }, new[] { student.FullName, student.GroupName, student.Faculty, student.Email });
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            var v = form.GetValues();
                            student.FullName = v[0]; student.GroupName = v[1]; student.Faculty = v[2]; student.Email = v[3];
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Успешно изменено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            btnDelete.Click += (s, e) => {
                if (gridStudents.CurrentRow == null) return;
                if (MessageBox.Show("Удалить студента?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(((DataRowView)gridStudents.CurrentRow.DataBoundItem)["Id"]);
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var studentToDelete = db.Students.Find(id);
                            if (studentToDelete != null) { db.Students.Remove(studentToDelete); db.SaveChanges(); }
                        }
                        LoadData();
                        MessageBox.Show("Успешно удалено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchStudents, btnAdd, btnEdit, btnDelete });
            gridStudents = CreateStyledGrid();
            gridStudents.Dock = DockStyle.Fill;
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

            txtSearchSubjects = new TextBox { Location = new Point(10, 20), Width = 300, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Text = "🔍 Поиск по названию или преподавателю..." };
            txtSearchSubjects.Enter += (s, e) => { if (txtSearchSubjects.Text.Contains("Поиск")) { txtSearchSubjects.Text = ""; txtSearchSubjects.ForeColor = Color.Black; } };
            txtSearchSubjects.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearchSubjects.Text)) { txtSearchSubjects.Text = "🔍 Поиск по названию или преподавателю..."; txtSearchSubjects.ForeColor = Color.Gray; } };
            txtSearchSubjects.TextChanged += (s, e) => {
                if (dvSubjects == null) return;
                if (txtSearchSubjects.Text.Contains("Поиск")) dvSubjects.RowFilter = "";
                else dvSubjects.RowFilter = $"Name LIKE '%{txtSearchSubjects.Text}%' OR Teacher LIKE '%{txtSearchSubjects.Text}%'";
            };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(330, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(460, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(590, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новый предмет", new[] { "Название", "Преподаватель" }, new string[2]);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var v = form.GetValues();
                    try
                    {
                        using (var db = new AppDbContext()) { db.Subjects.Add(new Subject { Name = v[0], Teacher = v[1] }); db.SaveChanges(); }
                        LoadData();
                        MessageBox.Show("Успешно добавлено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridSubjects.CurrentRow == null) return;
                int id = Convert.ToInt32(((DataRowView)gridSubjects.CurrentRow.DataBoundItem)["Id"]);
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var subj = db.Subjects.Find(id);
                        if (subj == null) return;
                        var form = new EditForm("Редактировать", new[] { "Название", "Преподаватель" }, new[] { subj.Name, subj.Teacher });
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            var v = form.GetValues();
                            subj.Name = v[0]; subj.Teacher = v[1];
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Успешно изменено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            btnDelete.Click += (s, e) => {
                if (gridSubjects.CurrentRow == null) return;
                if (MessageBox.Show("Удалить предмет?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(((DataRowView)gridSubjects.CurrentRow.DataBoundItem)["Id"]);
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            bool isUsed = db.OlympiadSubjects.Any(os => os.SubjectId == id);
                            if (isUsed) { MessageBox.Show("Нельзя удалить предмет, он привязан к олимпиаде!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                            var subjectToDelete = db.Subjects.Find(id);
                            if (subjectToDelete != null) { db.Subjects.Remove(subjectToDelete); db.SaveChanges(); }
                        }
                        LoadData();
                        MessageBox.Show("Успешно удалено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchSubjects, btnAdd, btnEdit, btnDelete });
            gridSubjects = CreateStyledGrid();
            gridSubjects.Dock = DockStyle.Fill;
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

                try
                {
                    using (var db = new AppDbContext())
                    {
                        cmbSt.DataSource = db.Students.ToList(); cmbSt.DisplayMember = "FullName"; cmbSt.ValueMember = "Id";
                        cmbSb.DataSource = db.Subjects.ToList(); cmbSb.DisplayMember = "Name"; cmbSb.ValueMember = "Id";
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки списков", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                btnSave.Click += (s2, e2) => {
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            db.Grades.Add(new Grade { StudentId = (int)cmbSt.SelectedValue, SubjectId = (int)cmbSb.SelectedValue, Score = int.Parse(txtSc.Text), Date = DateTime.Now.ToShortDateString() });
                            db.SaveChanges();
                        }
                        form.Close(); LoadData();
                        MessageBox.Show("Оценка выставлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            txtSearchOlympiads.TextChanged += (s, e) => {
                if (dvOlympiads == null) return;
                if (txtSearchOlympiads.Text.Contains("Поиск")) dvOlympiads.RowFilter = "";
                else dvOlympiads.RowFilter = $"Name LIKE '%{txtSearchOlympiads.Text}%'";
            };

            Button btnAdd = new Button { Text = "+ Добавить", Location = new Point(280, 18), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnEdit = new Button { Text = "✏ Изменить", Location = new Point(410, 18), BackColor = Color.FromArgb(241, 196, 15), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            Button btnDelete = new Button { Text = "🗑 Удалить", Location = new Point(540, 18), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 120, Height = 32, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };

            btnAdd.Click += (s, e) => {
                var form = new EditForm("Новая олимпиада", new[] { "Название", "Уровень", "Проходной балл", "Дата", "Призовой фонд" }, new[] { "", "", "4,5", "", "" }, new[] { "Профильный предмет" });
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var subjects = db.Subjects.ToList();
                        form.ComboBoxes[0].DataSource = subjects; form.ComboBoxes[0].DisplayMember = "Name"; form.ComboBoxes[0].ValueMember = "Id"; form.ComboBoxes[0].SelectedIndex = -1;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка загрузки предметов", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var v = form.GetValues();
                    double.TryParse(v[2].Replace(".", ","), out double score);
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var newOlimp = new Olympiad { Name = v[0], Level = v[1], PassingScore = score, Date = v[3], PrizeFund = v[4] };
                            db.Olympiads.Add(newOlimp); db.SaveChanges();
                            if (form.ComboBoxes[0].SelectedValue != null && (int)form.ComboBoxes[0].SelectedValue > 0)
                                db.OlympiadSubjects.Add(new OlympiadSubject { OlympiadId = newOlimp.Id, SubjectId = (int)form.ComboBoxes[0].SelectedValue, IsProfile = true });
                            db.SaveChanges();
                        }
                        LoadData();
                        MessageBox.Show("Олимпиада добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            btnEdit.Click += (s, e) => {
                if (gridOlympiads.CurrentRow == null) return;
                int id = Convert.ToInt32(((DataRowView)gridOlympiads.CurrentRow.DataBoundItem)["Id"]);
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var olimp = db.Olympiads.Find(id);
                        if (olimp == null) return;
                        var form = new EditForm("Редактировать", new[] { "Название", "Уровень", "Проходной балл", "Дата", "Призовой фонд" }, new[] { olimp.Name, olimp.Level, olimp.PassingScore.ToString(), olimp.Date ?? "", olimp.PrizeFund ?? "" }, new[] { "Профильный предмет" });
                        var subjects = db.Subjects.ToList();
                        form.ComboBoxes[0].DataSource = subjects; form.ComboBoxes[0].DisplayMember = "Name"; form.ComboBoxes[0].ValueMember = "Id";
                        var linkedSubject = db.OlympiadSubjects.FirstOrDefault(os => os.OlympiadId == olimp.Id);
                        if (linkedSubject != null) form.ComboBoxes[0].SelectedValue = linkedSubject.SubjectId;

                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            var v = form.GetValues();
                            double.TryParse(v[2].Replace(".", ","), out double score);
                            olimp.Name = v[0]; olimp.Level = v[1]; olimp.PassingScore = score; olimp.Date = v[3]; olimp.PrizeFund = v[4];
                            var oldLinks = db.OlympiadSubjects.Where(os => os.OlympiadId == olimp.Id); db.OlympiadSubjects.RemoveRange(oldLinks);
                            if (form.ComboBoxes[0].SelectedValue != null && (int)form.ComboBoxes[0].SelectedValue > 0)
                                db.OlympiadSubjects.Add(new OlympiadSubject { OlympiadId = olimp.Id, SubjectId = (int)form.ComboBoxes[0].SelectedValue, IsProfile = true });
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Олимпиада изменена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            btnDelete.Click += (s, e) => {
                if (gridOlympiads.CurrentRow == null) return;
                if (MessageBox.Show("Удалить олимпиаду?", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(((DataRowView)gridOlympiads.CurrentRow.DataBoundItem)["Id"]);
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var olimpToDelete = db.Olympiads.Find(id);
                            if (olimpToDelete != null)
                            {
                                var links = db.OlympiadSubjects.Where(os => os.OlympiadId == olimpToDelete.Id);
                                db.OlympiadSubjects.RemoveRange(links); db.Olympiads.Remove(olimpToDelete); db.SaveChanges();
                            }
                        }
                        LoadData();
                        MessageBox.Show("Олимпиада удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            topBar.Controls.AddRange(new Control[] { txtSearchOlympiads, btnAdd, btnEdit, btnDelete });
            gridOlympiads = CreateStyledGrid();
            gridOlympiads.Dock = DockStyle.Fill;
            gridOlympiads.ReadOnly = true;

            gridOlympiads.DataBindingComplete += (s, e) => {
                if (gridOlympiads.Columns["Name"] != null) gridOlympiads.Columns["Name"].HeaderText = "Название олимпиады";
                if (gridOlympiads.Columns["Level"] != null) gridOlympiads.Columns["Level"].HeaderText = "Уровень";
                if (gridOlympiads.Columns["PassingScore"] != null) gridOlympiads.Columns["PassingScore"].HeaderText = "Проходной балл";
                if (gridOlympiads.Columns["Date"] != null) gridOlympiads.Columns["Date"].HeaderText = "Дата";
                if (gridOlympiads.Columns["PrizeFund"] != null) gridOlympiads.Columns["PrizeFund"].HeaderText = "Призовой фонд";
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
            try
            {
                using (var db = new AppDbContext())
                {
                    var studentsList = db.Students.ToList();
                    dvStudents = new DataView(ToDataTable(studentsList));
                    gridStudents.DataSource = dvStudents;

                    var subjectsList = db.Subjects.ToList();
                    dvSubjects = new DataView(ToDataTable(subjectsList));
                    gridSubjects.DataSource = dvSubjects;

                    var olympiadsList = db.Olympiads.ToList();
                    dvOlympiads = new DataView(ToDataTable(olympiadsList));
                    gridOlympiads.DataSource = dvOlympiads;

                    if (gridStudents.Columns["Id"] != null) gridStudents.Columns["Id"].Visible = false;
                    if (gridSubjects.Columns["Id"] != null) gridSubjects.Columns["Id"].Visible = false;
                    if (gridOlympiads.Columns["Id"] != null) gridOlympiads.Columns["Id"].Visible = false;

                    cmbFilterOlympiadsForResults.DataSource = db.Olympiads.ToList();
                    cmbFilterOlympiadsForResults.DisplayMember = "Name";
                    cmbFilterOlympiadsForResults.ValueMember = "Id";

                    var gradesView = db.Grades.Include(g => g.Student).Include(g => g.Subject)
                        .Select(g => new { g.Id, Студент = g.Student.FullName, Предмет = g.Subject.Name, Оценка = g.Score, Дата = g.Date }).ToList();
                    gridGrades.DataSource = gradesView;
                    if (gridGrades.Columns["Id"] != null) gridGrades.Columns["Id"].Visible = false;

                    lblCountStudents.Text = db.Students.Count().ToString();
                    lblCountOlympiads.Text = db.Olympiads.Count().ToString();
                    lblCountSubjects.Text = db.Subjects.Count().ToString();
                    lblCountGrades.Text = db.Grades.Count().ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private DataTable ToDataTable<T>(System.Collections.Generic.IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}