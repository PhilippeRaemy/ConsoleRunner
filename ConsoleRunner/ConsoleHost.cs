namespace ConsoleRunner
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    class ConsoleHost
    {
        readonly RichTextBox _richTextBox ;
        readonly TabControl _tabs;

        public ConsoleHost(TabControl tabs, string command)
        {
            _tabs = tabs;
            var tabPage = new TabPage();
            _richTextBox = new RichTextBox {Font = new Font("Lucida Console", 8)};
            var ccols = InferColorFromText(command);
            _richTextBox.ForeColor = ccols.ForegroundColor;
            _richTextBox.BackColor = ccols.BackgroundColor;

            tabPage.Location = new Point(0, 0);
            tabPage.Name = "tabPage1";
            tabPage.Dock = DockStyle.Fill;
            tabPage.TabIndex = 0;
            tabs.TabPages.Add(tabPage);

            // 
            // tabPage
            // 
            tabPage.Controls.Add(_richTextBox);
            tabPage.Location = new Point(4, 22);
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(699, 426);
            tabPage.TabIndex = 0;
            tabPage.Text = command;
            tabPage.UseVisualStyleBackColor = true;
            // 
            // richTextBox
            // 
            _richTextBox.Dock=DockStyle.Fill;
            _richTextBox.TabIndex = 0;
            _richTextBox.Text = "";

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    // Arguments = "/c ping /t 127.0.0.1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            proc.OutputDataReceived += OutputHandler;
            proc.ErrorDataReceived += OutputHandler;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            var myStreamWriter = proc.StandardInput;


        }
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            _tabs.Parent.Invoke(TextAppender(_richTextBox, outLine.Data));
        }
        Action TextAppender(RichTextBox tb, string data)
        {
            return () =>
            {
                tb.Text += Environment.NewLine + data;
                try
                {
                    tb.SelectionStart = tb.TextLength;
                }
                catch (InvalidOperationException)
                {
                }
            };
        }

        class ConsoleColors
        {
            public Color BackgroundColor { get; set; }
            public Color ForegroundColor { get; set; }
        }

        ConsoleColors InferColorFromText(string text)
        {
            Func<int, int> colorComp = cmp =>
                 Encoding.Unicode.GetBytes(text)
                     .Where((c, i) => (i % 3) == cmp)
                     .Sum(c => c);
            var r = colorComp(0) % 256;
            var g = colorComp(1) % 256;
            var b = colorComp(2) % 256;
            var f = (r + 2 * g + b) / 3 >= 180 ? 0 : 255;
            return new ConsoleColors
            {
                BackgroundColor = Color.FromArgb(r, g, b),
                ForegroundColor = Color.FromArgb(f, f, f)
            };
        }
    }
}