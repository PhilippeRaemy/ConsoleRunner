using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleRunner
{
    using System.Diagnostics;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadTabs();
        }

        void LoadTabs()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c ping /t 127.0.0.1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.OutputDataReceived += OutputHandler;
            proc.ErrorDataReceived  += OutputHandler;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
        }

        Action TextAppender(RichTextBox tb, string data)
        {
            return () =>
            {
                richTextBox1.Text += Environment.NewLine + data;
                try
                {
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                }
                catch (InvalidOperationException)
                {
                }
            };
        }

        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Invoke(TextAppender(richTextBox1, outLine.Data));
        }
    }
}
