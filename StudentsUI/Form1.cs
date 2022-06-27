using StudentsUI.JsonTypes;
using System.Windows.Forms;
using System.Collections;

namespace StudentsUI
{
    public partial class Form1 : Form
    {
        private ListViewColumnSorter lvwColumnSorter;
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("First Name", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Last Name", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("School",-2, HorizontalAlignment.Left);
            listView1.Columns.Add("Discord", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Student ID", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Also known as...", -2, HorizontalAlignment.Left);
            textBox1.KeyUp += onTextBoxUpdated;
            listView1.ColumnClick += listView1_ColumnClick;
            InitDatabase();
            AddStudents();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            MaximizeBox = false;
            MinimizeBox = false;
            listView1.MouseUp += OnMouseUp;
            TxtEdit.Leave += TxtEdit_Leave;
            TxtEdit.KeyUp += TxtEdit_KeyUp;
            MouseDown += OnMouseDown;
            Scroll += OnScroll;
            button1.MouseClick += ButtonClick;
        }
        public void ButtonClick(object sender,MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Save();
            }
        }
        ListViewItem.ListViewSubItem SelectedLSI = null;
        ListViewItem SelectedItem = null;
        public void OnMouseUp(object sender,MouseEventArgs e)
        {
            ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);
            
            SelectedLSI = i.SubItem;
            SelectedItem = i.Item;
            if (SelectedLSI == null)
                return;
            int colnum = i.Item.SubItems.IndexOf(i.SubItem);
            if (colnum == 4 || colnum == 5)
            {
                SelectedLSI = null;
                SelectedItem = null;
                return;
            }

            int border = 0;
            switch (listView1.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = SelectedLSI.Bounds.Width;
            int CellHeight = SelectedLSI.Bounds.Height;
            int CellLeft = border + listView1.Left + i.SubItem.Bounds.Left;
            int CellTop = listView1.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = listView1.Columns[0].Width;
            TxtEdit.Location = new Point(CellLeft, CellTop);
            TxtEdit.Size = new Size(CellWidth, CellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = i.SubItem.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }
        public void OnMouseDown(object sender,MouseEventArgs e)
        {
            HideTextEditor();
        }
        public void OnScroll(object sender,EventArgs e)
        {

            HideTextEditor();
        }
        public void TxtEdit_Leave(object sender,EventArgs e)
        {
            HideTextEditor();
        }
        public void TxtEdit_KeyUp(object sender,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Escape)
            {
                HideTextEditor();
            }
        }
        private void HideTextEditor()
        {
            if(SelectedLSI == null)
            {

            }
            else
            {
                SelectedLSI.Text = TxtEdit.Text;
                JSONStorage.Instance.Load(SelectedItem);
            }
            TxtEdit.Visible = false;
            TxtEdit.Text = "";
            SelectedItem = null;
            SelectedLSI = null;
        }
        public void InitDatabase()
        {
            JSONStorage storage = new JSONStorage();
            //storage.Read();
            storage.Read();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
        internal void AddStudentToList(Student student)
        {
            ListViewItem item = new ListViewItem(student.FirstName,0);
            item.SubItems.Add(student.LastName);
            item.SubItems.Add(student.School);
            if (student.Discord == "Unknown") item.SubItems.Add("");
            else item.SubItems.Add(student.Discord);
            item.SubItems.Add(student.Id.ToString());
            item.SubItems.Add(student.currentDeadName);
            listView1.Items.Add(item);
        }
        internal void AddStudents()
        {
            JSONStorage storage = JSONStorage.Instance;
            foreach(Student student in storage.students)
            {
                AddStudentToList(student);
            }
        }
        internal void AddStudents(string str)
        {
            listView1.Items.Clear();
            JSONStorage storage = JSONStorage.Instance;
            for(int i = 0; i < storage.students.Length; i++)
            {
                Student student = storage.students[i];
                string name;
                if (ContainsString(student, str))
                {
                    student.currentDeadName = "";
                    AddStudentToList(student);
                }
                else if (TryGetDeadname(student, str, out name))
                {
                    student.currentDeadName = name;
                    AddStudentToList(student);
                }
                else
                {
                    student.currentDeadName = "";
                }
            }
        }
        public void listView1_ColumnClick(object sender,ColumnClickEventArgs e)
        {
            int Column = e.Column;
            if (Column == 5) return;
            // Determine if clicked column is already the column that is being sorted.
            if (Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listView1.Sort();
        }
        public void onTextBoxUpdated(object sender,KeyEventArgs e)
        {
            string text = textBox1.Text;
            AddStudents(text);
        }
        internal bool ContainsString(Student student,string str)
        {
            if (str.Length == 0) return true;
            str = str.ToLower();
            return
                student.FirstName.ToLower().Contains(str) ||
                student.LastName.ToLower().Contains(str) ||
                student.School.ToLower().Contains(str) ||
                student.Id.ToString().ToLower().Contains(str) ||
                student.Discord.ToLower().Contains(str);
        }
        public bool ContainsString(string[] strings,string str)
        {
            if (str == "") return true;
            if (strings == null || strings.Length == 0) return false;
            str = str.ToLower();
            foreach(string s in strings)
            {
                if(s.ToLower().Contains(str))
                {
                    return true;
                }
            }
            return false;
        }
        internal bool TryGetDeadname(Student student,string search,out string name)
        {
            search = search.ToLower();
            string[] names = student.DeadNames;
            foreach(string str in names)
            {
                if(str.ToLower().Contains(search))
                {
                    name = str;
                    return true;
                }
            }
            name = "";
            return false;
        }
        public bool ContainsString(List<string> strings,string str)
        {
            str = str.ToLower();
            foreach (string s in strings)
            {
                if (s.ToLower().Contains(str))
                {
                    return true;
                }
            }
            return false;
        }
        public void Save()
        {
            listView1_ColumnClick(null, new ColumnClickEventArgs(1));
            JSONStorage.Instance.Load(listView1);
            JSONStorage.Instance.Write();
            //CopyToClipboard();
        }
        public void CopyToClipboard()
        {
            string names = "";
            Student[] students = JSONStorage.Instance.students;
            for (int i = 0; i < students.Length;i++)
            {
                Student student = students[i];
                if(student.Discord == "Unknown" || student.Discord.Length == 0)
                {
                    if (i == 0) names += student.FirstName + " " + student.LastName;
                    else names += "\n" + student.FirstName + " " + student.LastName;
                }
            }
            if (names.Length == 0) return;
            Clipboard.SetText(names);
        }
    }
}