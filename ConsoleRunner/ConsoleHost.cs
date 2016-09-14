namespace ConsoleRunner
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
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
                    CreateNoWindow = true
                }
            };
            proc.OutputDataReceived += OutputHandler;
            proc.ErrorDataReceived += OutputHandler;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();


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
    }
}