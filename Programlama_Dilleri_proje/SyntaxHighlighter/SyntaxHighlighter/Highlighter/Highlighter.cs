using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SyntaxHighlighter.Models;
using SyntaxHighlighter.Analyzer;

namespace SyntaxHighlighter.Syntax
{
    public static class Highlighter
    {
        public static void Highlight(RichTextBox textBox, List<Token> tokens, ParseResult parseResult)
        {
            // Preserve caret position
            TextPointer caretPosition = textBox.CaretPosition;
            int caretOffset = new TextRange(textBox.Document.ContentStart, caretPosition).Text.Length;

            // Get full text and normalize line endings
            string fullText = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text.Replace("\r\n", "\n");

            var paragraph = new Paragraph();
            int currentPos = 0;

            foreach (var token in tokens)
            {
                // Add text before the token (spaces, etc.)
                if (token.Position > currentPos && token.Position <= fullText.Length)
                {
                    int length = token.Position - currentPos;
                    if (length > 0 && currentPos + length <= fullText.Length)
                    {
                        string skippedText = fullText.Substring(currentPos, length);
                        paragraph.Inlines.Add(new Run(skippedText) { Foreground = Brushes.SlateGray });
                    }
                }

                // Process the token
                if (token.Position >= 0 && token.Position < fullText.Length)
                {
                    int tokenLength = token.Value.Length;
                    if (token.Position + tokenLength > fullText.Length)
                    {
                        tokenLength = fullText.Length - token.Position;
                    }
                    string tokenText = fullText.Substring(token.Position, tokenLength);

                    var run = new Run(tokenText);

                    switch (token.Type)
                    {
                        case "keyword":
                            if (token.Value == "int")
                                run.Foreground = Brushes.DeepSkyBlue; // Bright sky blue for 'int'
                            else if (token.Value == "string")
                                run.Foreground = Brushes.LightGreen; // Light green for 'string'
                            break;
                        case "number":
                            run.Foreground = Brushes.MediumSeaGreen; // Medium sea green for numbers
                            break;
                        case "identifier":
                            run.Foreground = Brushes.MediumSeaGreen; // Medium sea green for identifiers
                            break;
                        case "stringLiteral":
                            run.Foreground = Brushes.MediumSeaGreen; // Medium sea green for string literals
                            break;
                        case "operator":
                            run.Foreground = Brushes.HotPink; // Hot pink for operators
                            break;
                        case "comment":
                            run.Foreground = Brushes.Goldenrod; // Goldenrod for comments
                            break;
                        default:
                            run.Foreground = Brushes.SlateGray; // Slate gray for default
                            break;
                    }
                    paragraph.Inlines.Add(run);
                }

                currentPos = token.Position + token.Value.Length;
            }

            // Add remaining text
            if (currentPos < fullText.Length)
            {
                int remainingLength = fullText.Length - currentPos;
                if (remainingLength > 0)
                {
                    string remainingText = fullText.Substring(currentPos, remainingLength);
                    paragraph.Inlines.Add(new Run(remainingText) { Foreground = Brushes.SlateGray });
                }
            }

            // Update text
            textBox.Document.Blocks.Clear();
            textBox.Document.Blocks.Add(paragraph);

            // Restore caret position
            if (caretOffset > 0)
            {
                TextPointer newCaretPosition = textBox.Document.ContentStart;
                int offset = 0;
                foreach (var inline in paragraph.Inlines)
                {
                    if (inline is Run run)
                    {
                        int runLength = run.Text.Length;
                        if (offset + runLength >= caretOffset)
                        {
                            int relativeOffset = caretOffset - offset;
                            newCaretPosition = run.ContentStart.GetPositionAtOffset(relativeOffset);
                            break;
                        }
                        offset += runLength;
                    }
                }
                textBox.CaretPosition = newCaretPosition ?? textBox.Document.ContentEnd;
            }
        }
    }
}