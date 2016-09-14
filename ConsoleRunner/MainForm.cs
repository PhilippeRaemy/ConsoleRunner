namespace ConsoleRunner
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using MoreLinq;

    public partial class MainForm : Form
    {
        public MainForm(IEnumerable<string> args)
        {
            InitializeComponent();
            args.ForEach(s => new ConsoleHost(tabs, s));
        }
    }
}
