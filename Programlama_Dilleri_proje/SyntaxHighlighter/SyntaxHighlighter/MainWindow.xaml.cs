using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using SyntaxHighlighter.Analyzer;
using SyntaxHighlighter.Syntax;

namespace SyntaxHighlighter
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            if (CodeTextBox == null)
            {
                MessageBox.Show("CodeTextBox is null! Please check the XAML file.");
                return;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            CodeTextBox.TextChanged += (s, e) => timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            string text = new TextRange(CodeTextBox.Document.ContentStart, CodeTextBox.Document.ContentEnd).Text;
            var lexer = new Lexer(text);
            var tokens = lexer.Tokenize();

            var parser = new Parser(tokens);
            var parseResult = parser.Parse();

            Highlighter.Highlight(CodeTextBox, tokens, parseResult);
        }
    }
}