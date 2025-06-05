using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;
using SyntaxHighlighter.Models;

namespace SyntaxHighlighter.Analyzer
{
    public class ParseResult
    {
        // Empty class, no error-related properties
    }

    public class Parser
    {
        private readonly List<Token> tokens;
        private int position;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? new List<Token>();
            this.position = 0;
        }

        public ParseResult Parse()
        {
            while (position < tokens.Count)
                ParseStatement();

            return new ParseResult();
        }

        private void ParseStatement()
        {
            if (position >= tokens.Count)
                return;

            var token = tokens[position];
            if (token.Type == "keyword")
            {
                switch (token.Value)
                {
                    case "int":
                    case "string":
                        ParseAssignmentStatement();
                        break;
                    case "if":
                    case "while":
                        ParseConditional();
                        break;
                    case "for":
                        ParseForLoop();
                        break;
                    default:
                        position++; // Skip unknown keywords
                        break;
                }
            }
            else
            {
                position++; // Skip unexpected tokens
            }
        }

        private void ParseAssignmentStatement()
        {
            position++;
            if (position < tokens.Count && tokens[position].Type == "identifier")
                position++;

            if (position < tokens.Count && tokens[position].Value == "=")
                position++;

            ParseExpression();

            if (position < tokens.Count && tokens[position].Value == ";")
                position++;
        }

        private void ParseConditional()
        {
            position++;
            if (position < tokens.Count && tokens[position].Value == "(")
                position++;

            ParseExpression();

            if (position < tokens.Count && tokens[position].Value == ")")
                position++;

            if (position < tokens.Count && tokens[position].Value == "{")
            {
                position++;
                ParseBlock();
                if (position < tokens.Count && tokens[position].Value == "}")
                    position++;
            }
        }

        private void ParseForLoop()
        {
            position++;
            if (position < tokens.Count && tokens[position].Value == "(")
                position++;

            if (position < tokens.Count && tokens[position].Type == "keyword" && (tokens[position].Value == "int" || tokens[position].Value == "string"))
            {
                ParseAssignmentStatement();
            }
            else if (position < tokens.Count && tokens[position].Value == ";")
            {
                position++;
            }

            if (position < tokens.Count && tokens[position].Type != "operator" && tokens[position].Value != ";")
            {
                ParseExpression();
            }
            if (position < tokens.Count && tokens[position].Value == ";")
                position++;

            if (position < tokens.Count && tokens[position].Type != "operator" && tokens[position].Value != ")")
            {
                ParseExpression();
            }
            if (position < tokens.Count && tokens[position].Value == ")")
                position++;

            if (position < tokens.Count && tokens[position].Value == "{")
            {
                position++;
                ParseBlock();
                if (position < tokens.Count && tokens[position].Value == "}")
                    position++;
            }
        }

        private void ParseBlock()
        {
            while (position < tokens.Count && tokens[position].Value != "}")
            {
                ParseStatement();
            }
        }

        private void ParseExpression()
        {
            if (position >= tokens.Count)
                return;

            if (tokens[position].Value == "(")
            {
                position++;
                ParseExpression();
                if (position < tokens.Count && tokens[position].Value == ")")
                    position++;
            }
            else if (tokens[position].Type == "number" || tokens[position].Type == "identifier" || tokens[position].Type == "stringLiteral")
            {
                position++;
                if (position < tokens.Count && tokens[position].Type == "operator" && "+-*/><==".Contains(tokens[position].Value))
                {
                    position++;
                    ParseExpression();
                }
            }
            else
            {
                position++; // Skip invalid expressions
            }
        }
    }
}