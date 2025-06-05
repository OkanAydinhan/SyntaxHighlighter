namespace SyntaxHighlighter.Models
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }
        public int Length => Value.Length;
    }
}