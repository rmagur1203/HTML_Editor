using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTML_Editor
{
    public partial class Form1 : Form
    {
        private bool ctrl;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private TabPage untitle()
        {
            TabPage untitled = new TabPage();
            untitled.Text = "untitled";
            StatusStrip status = new StatusStrip();
            ToolStripStatusLabel statusLabel1 = new ToolStripStatusLabel();
            statusLabel1.Name = "toolStripStatusLabel1";
            status.Items.Add(statusLabel1);
            ToolStripStatusLabel statusLabel2 = new ToolStripStatusLabel();
            statusLabel2.Name = "";
            statusLabel2.Visible = false;
            status.Items.Add(statusLabel2);
            status.Dock = DockStyle.Bottom;
            status.Name = "statusStrip1";
            untitled.Controls.Add(status);
            RichTextBox richText = new RichTextBox();
            richText.Dock = DockStyle.Fill;
            richText.Name = "richTextBox1";
            richText.TextChanged += new EventHandler(this.richTextBox1_TextChanged);
            richText.KeyPress += new KeyPressEventHandler(this.richTextBox1_TextChanged);
            richText.KeyDown += new KeyEventHandler(this.richTextBox1_KeyDown);
            richText.KeyUp += new KeyEventHandler(this.richTextBox1_KeyUp);
            richText.MouseClick += new MouseEventHandler(this.richTextBox1_TextChanged);
            //untitled.MouseClick += new MouseEventHandler(this.untitled_MouseClick);
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Close");
            cms.Items.Add("Close Other Taps");
            cms.Items.Add("Close Taps to the Right");
            cms.Items.Add(new ToolStripSeparator());
            untitled.ContextMenuStrip = cms;
            cms.Items.Add("New File");
            cms.Items.Add("Open File");
            untitled.Controls.Add(richText);
            return untitled;
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //tabControl1.Controls.Add(untitled_page.Controls[0]);
            TabPage tp = untitle();
            tabControl1.Controls.Add(tp);
            stats_update(tp);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            stats_update((TabPage)rtb.Parent);
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    ctrl = true;
                    break;
                case Keys.S:
                    //if (!ctrl)
                    //    break;
                    
                    break;
            }
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    ctrl = false;
                    break;
                case Keys.S:
                    if (!ctrl)
                        break;
                    save((TabPage)((RichTextBox)sender).Parent);
                    break;
            }
        }

        private void untitled_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.ContextMenuStrip.Show();
            }
        }

        private void web_load(string html)
        {
            webBrowser1.DocumentText = html;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Document.OpenNew(true);
            webBrowser1.Document.Write(html);
            webBrowser1.Refresh();
        }

        private void stats_update(TabPage tp)
        {
            string line;
            RichTextBox rtb = (RichTextBox)tp.Controls[1];
            ToolStripStatusLabel status = (ToolStripStatusLabel)((StatusStrip)tp.Controls[0]).Items[0];
            ToolStripStatusLabel file = (ToolStripStatusLabel)((StatusStrip)tp.Controls[0]).Items[1];

            if (file.Text == "")
            {
                try
                {
                    if (rtb.Lines[0] == "") tp.Text = "untitled";
                    if (rtb.Lines[0].Length > 50) tp.Text = rtb.Lines[0].Substring(0, 50); else tp.Text = rtb.Lines[0];
                }
                catch (Exception) { tp.Text = "untitled"; }
            }
            else tp.Text = file.Text.Split('\\').Last();
            tp.Text += "*";

            int firstcharindex = rtb.GetFirstCharIndexOfCurrentLine();
            int currentline = rtb.GetLineFromCharIndex(firstcharindex);
            line = (currentline + 1).ToString();

            int position = rtb.SelectionStart;
            int col = position - rtb.GetFirstCharIndexOfCurrentLine() + 1;

            string column;
            column = rtb.SelectionStart.ToString();
            status.Text = string.Format("Line {0}, Column {1}", line, col);
        }

        private void save(TabPage tp)
        {
            web_load(((RichTextBox)tp.Controls[1]).Text);
            RichTextBox rtb = (RichTextBox)tp.Controls[1];
            ToolStripStatusLabel file = (ToolStripStatusLabel)((StatusStrip)tp.Controls[0]).Items[1];
            tp.Text = tp.Text.Remove(tp.Text.Length - 1);
            if (!(file.Text == ""))
            {
                System.IO.File.WriteAllLines(file.Text, rtb.Lines);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                file.Text = sfd.FileName;
                System.IO.File.WriteAllLines(sfd.FileName, rtb.Lines);
                tp.Text = file.Text.Split('\\').Last();
            }
        }

        private bool Exit()
        {
            bool issave = true;
            List<TabPage> tps = new List<TabPage>();
            foreach (TabPage search in tabControl1.TabPages)
            {
                if (((StatusStrip)search.Controls[0]).Items[1].Text == "")
                {
                    issave = false;
                    tabControl1.SelectTab(search);
                    switch (MessageBox.Show("저장 하시겠습니까?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            save(search);
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.Cancel:
                            return false;
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            if (issave)
                Application.ExitThread();
            return true;
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    TabPage tp = untitle();
                    ToolStripStatusLabel filetext = (ToolStripStatusLabel)((StatusStrip)tp.Controls[0]).Items[1];
                    StringBuilder sb = new StringBuilder();
                    foreach (string search in System.IO.File.ReadAllLines(file)) sb.AppendLine(search);
                    RichTextBox richText = (RichTextBox)tp.Controls[1];
                    richText.Text = sb.ToString();
                    filetext.Text = file;
                    tp.Text = file.Split('\\').Last();
                    tabControl1.Controls.Add(tp);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void openRecentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reopenWithEncodingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newViewIntoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save(tabControl1.SelectedTab);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (!Exit())
                e.Cancel = true;
        }
    }
}
