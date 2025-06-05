using System;
using System.Collections.Generic;
using System.Linq;
using SyntaxHighlighter.Models;

namespace SyntaxHighlighter.Analyzer
{
    public class Lexer
    {
        private readonly string input;
        private int position;
        private int line;
        private static readonly string[] keywords = { "if", "while", "int", "else", "for", "string" };

        public Lexer(string input)
        {
            this.input = input ?? string.Empty;
            this.position = 0;
            this.line = 1;
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();

            while (position < input.Length)
            {
                char current = input[position];

                if (current == '\n')
                {
                    line++;
                    position++;
                    continue;
                }
                else if (char.IsWhiteSpace(current))
                {
                    position++;
                    continue;
                }
                else if (current == '/' && position + 1 < input.Length && input[position + 1] == '/')
                {
                    tokens.Add(ReadComment());
                }
                else if (char.IsDigit(current))
                {
                    tokens.Add(ReadNumber());
                }
                else if (char.IsLetter(current) || current == '_')
                {
                    tokens.Add(ReadIdentifierOrKeyword());
                }
                else if (current == '"' && position < input.Length)
                {
                    tokens.Add(ReadStringLiteral());
                }
                else if ("+-*/=;(){}".Contains(current))
                {
                    tokens.Add(new Token { Type = "operator", Value = current.ToString(), Position = position, Line = line });
                    position++;
                }
                else
                {
                    position++; // Skip unrecognized characters
                }
            }

            return tokens;
        }

        private Token ReadNumber()
        {
            int start = position;
            while (position < input.Length && char.IsDigit(input[position]))
                position++;

            string value = input.Substring(start, position - start);
            return new Token { Type = "number", Value = value, Position = start, Line = line };
        }

        private Token ReadIdentifierOrKeyword()
        {
            int start = position;
            while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
                position++;

            string word = input.Substring(start, position - start);
            string type = keywords.Contains(word) ? "keyword" : "identifier";
            return new Token { Type = type, Value = word, Position = start, Line = line };
        }

        private Token ReadComment()
        {
            int start = position;
            while (position < input.Length && input[position] != '\n')
                position++;

            string comment = input.Substring(start, position - start);
            return new Token { Type = "comment", Value = comment, Position = start, Line = line };
        }

        private Token ReadStringLiteral()
        {
            int start = position;
            position++; // Skip opening quote
            while (position < input.Length && input[position] != '"')
            {
                if (input[position] == '\\' && position + 1 < input.Length) position++; // Handle escape sequences
                position++;
            }
            if (position < input.Length && input[position] == '"') position++; // Skip closing quote
            string value = input.Substring(start, position - start);
            return new Token { Type = "stringLiteral", Value = value, Position = start, Line = line };
        }
    }
}