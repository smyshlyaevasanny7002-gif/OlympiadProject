using System;
using System.Drawing;
using System.Windows.Forms;

namespace OlympiadProject
{
    public class EditForm : Form
    {
        private TableLayoutPanel layout;
        public bool IsSaved { get; private set; } = false;

        public EditForm(string title, string[] labels, string[] values)
        {
            this.Text = title;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Size = new Size(450, 80 + (labels.Length * 50) + 60);

            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = labels.Length + 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            for (int i = 0; i < labels.Length; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));

            for (int i = 0; i < labels.Length; i++)
            {
                Label lbl = new Label { Text = labels[i], Anchor = AnchorStyles.Left, AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                TextBox txt = new TextBox { Text = values[i], Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };

                layout.Controls.Add(lbl, 0, i + 1);
                layout.Controls.Add(txt, 1, i + 1);
            }

            Button btnSave = new Button { Text = "💾 Сохранить", Dock = DockStyle.Fill, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += (s, e) => {
                IsSaved = true;
                this.Close();
            };

            layout.Controls.Add(btnSave, 0, 0);
            layout.SetColumnSpan(btnSave, 2);

            this.Controls.Add(layout);
        }

        public string[] GetValues()
        {
            var result = new string[layout.RowCount - 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = layout.GetControlFromPosition(1, i + 1).Text;
            return result;
        }
    }
}