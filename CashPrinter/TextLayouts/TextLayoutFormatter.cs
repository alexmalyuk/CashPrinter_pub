using CashPrinter.CashDocuments;
using CashPrinter.Core;
using Core.CashPrinter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace CashPrinter.TextLayouts
{
    public static class ExtensionMethods
    {
        public static string[] Wrap(this string text, int max, TextAlignment alignment)
        {
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var lines = new List<string>();
            var currentLine = new StringBuilder();
            int currentWidth = 0;

            foreach (var word in words)
            {
                if (currentWidth + word.Length + 1 <= max)
                {
                    // Add the word to the current line
                    currentLine.Append(word + " ");
                    currentWidth += word.Length + 1;
                }
                else
                {
                    // Start a new line with the current word
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear().Append(word + " ");
                    currentWidth = word.Length + 1;
                }
            }

            // Add the last line
            lines.Add(currentLine.ToString().Trim());

            // Replace with Alignment
            var result = new List<string>();
            foreach (string line in lines)
            {
                switch (alignment)
                {
                    case TextAlignment.Right:
                        result.Add(line.PadLeft(max));
                        break;
                    case TextAlignment.Center:
                        result.Add(string.Empty.PadLeft((max - line.Length) / 2, ' ') + line);
                        break;
                    default:
                        result.Add(line);
                        break;
                }
            }

            return result.ToArray();
        }

        public static void AppendLineWrap(this StringBuilder sb, string Text, int Max, TextAlignment Alignment)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            bool isDoubleWidth = false;
            string dblWidthPattern = @"(\\w)";
            if (Regex.IsMatch(Text, dblWidthPattern))
            {
                isDoubleWidth = true;
                Text = Regex.Replace(Text, dblWidthPattern, "");
            }
            bool isDoubleHeight = false;
            string dblHeightPattern = @"(\\h)";
            if (Regex.IsMatch(Text, dblHeightPattern))
            {
                isDoubleHeight = true;
                Text = Regex.Replace(Text, dblHeightPattern, "");
            }

            string[] wrappedText = Text.Wrap(Max, Alignment);
            foreach (string textline in wrappedText)
                sb.AppendFormat("{0}{1}{2}\n", isDoubleWidth ? @"\w" : "", isDoubleHeight ? @"\h" : "", textline);
        }
    }

    public static class TextLayoutFormatter
    {
        public static string GetText(CashDocument cashDoc)
        {
            TextFormatter textFormatter;

            if (cashDoc is Receipt)
            {
                if (AppSettings.TapeWidth == TapeWidthEnum.Tape57mm)
                    textFormatter = new ReceiptTextFormatter_w57();
                else if (AppSettings.TapeWidth == TapeWidthEnum.Tape80mm)
                    textFormatter = new ReceiptTextFormatter_w80();
                else
                    throw new NotSupportedException();
            }
            else if (cashDoc is FiscalReceipt)
            {
                if (AppSettings.TapeWidth == TapeWidthEnum.Tape57mm)
                    textFormatter = new FiscalReceiptTextFormatter_w57();
                else if (AppSettings.TapeWidth == TapeWidthEnum.Tape80mm)
                    textFormatter = new FiscalReceiptTextFormatter_w80();
                else
                    throw new NotSupportedException();
            }
            else if (cashDoc is WarrantyCard)
            {
                if (AppSettings.TapeWidth == TapeWidthEnum.Tape57mm)
                    textFormatter = new WarrantyCardTextFormatter_w57();
                else if (AppSettings.TapeWidth == TapeWidthEnum.Tape80mm)
                    textFormatter = new WarrantyCardTextFormatter_w80();
                else
                    throw new NotSupportedException();
            }
            else if (cashDoc is Invoice)
            {
                if (AppSettings.TapeWidth == TapeWidthEnum.Tape57mm)
                    textFormatter = new InvoiceTextFormatter_w57();
                else if (AppSettings.TapeWidth == TapeWidthEnum.Tape80mm)
                    textFormatter = new InvoiceTextFormatter_w80();
                else
                    throw new NotSupportedException();
            }
            else
                throw new NotSupportedException(String.Format("Неизвестный тип документа {0}", cashDoc.GetType().FullName));

            return textFormatter.GetTextFromReceipt(cashDoc);
        }

        public static string GetTextMemo(CashDocument cashDoc)
        {
            TextFormatter textFormatter;

            textFormatter = new MemoTextFormatter_wAll();

            return textFormatter.GetTextFromReceipt(cashDoc);
        }
    }

    public abstract class TextFormatter
    {
        public abstract string GetTextFromReceipt(CashDocument cashDocument);
    }

}
