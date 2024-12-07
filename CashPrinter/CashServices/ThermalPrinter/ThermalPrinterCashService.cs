using CashPrinter.Core;
using GemBox.Pdf;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Printing;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CashPrinter.CashServices.ThermalPrinter
{
    class ThermalPrinterCashService : CashService
    {
        public override string DeviceName => "POS Thermal Printer";
        public override string SerialNumber => AppSettings.ThermalPrinterName;
        public override string ErrorMessage => "";
        public override int ResultCode => 0;
        public override bool DeviceEnabled => true;
        public override void Dispose() { }

        public override void PrintText(string Text, int Copies = 1)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FlowDocument flowDocument = CreateFlowDocument(Text);

                PrintDialog printDialog = new PrintDialog();
                try
                {
                    printDialog.PrintQueue = new PrintQueue(new PrintServer(), AppSettings.ThermalPrinterName);
                }
                catch (PrintQueueException ex)
                {
                    throw new CashPrinterException("Invalid printer name " + AppSettings.ThermalPrinterName, ex);
                }
                catch (Exception)
                {
                    throw;
                }

                flowDocument.PageHeight = printDialog.PrintableAreaHeight;
                flowDocument.PageWidth = printDialog.PrintableAreaWidth;
                flowDocument.PagePadding = new Thickness(0);
                flowDocument.ColumnGap = 0;
                flowDocument.ColumnWidth = flowDocument.PageWidth;

                for (int i = 0; i < Copies; i++)
                    printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "CashPrinter job");
            });
        }

        private Image GetBarCodeImage(string strBarCode)
        {
            const int barCodeWidth = 250;
            const int barCodeHeight = 50;

            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.Drawing.Image img = b.Encode(BarcodeLib.TYPE.CODE39, strBarCode, barCodeWidth, barCodeHeight);

            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Bmp);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();

                Image image = new Image
                {
                    Source = bitmapImage,
                    Width = barCodeWidth,
                    Height = barCodeHeight
                };

                return image;
            };
        }
        private BlockUIContainer GetBarCodeBlockUIContainer(string strBarCode)
        {
            BlockUIContainer blk = new BlockUIContainer(GetBarCodeImage(strBarCode));
            return blk;
        }
        private FlowDocument CreateFlowDocument(string Text)
        {

            FlowDocument flowDocument = new FlowDocument();
            flowDocument.PagePadding = new Thickness(0, 0, 0, 0);
            flowDocument.FontSize = 12.5; //8.5;
            flowDocument.FontFamily = new FontFamily("Consolas");   //Courier New

            Paragraph paragraph = new Paragraph();

            #region Bold Example
            //Bold bld = new Bold();
            //bld.Inlines.Add(new Run("First Paragraph"));
            //Italic italicBld = new Italic();
            //italicBld.Inlines.Add(bld);
            //Underline underlineItalicBld = new Underline();
            //underlineItalicBld.Inlines.Add(italicBld);
            //p1.Inlines.Add(underlineItalicBld);
            #endregion

            string[] lines = Text.Split('\n');
            foreach (string line in lines)
            {

                if (Regex.IsMatch(line, @"(\\bcl)"))
                {
                    Figure visual = new Figure();

                    string strBarCode = Regex.Replace(line, @"\\bcl", "");
                    visual.Blocks.Add(GetBarCodeBlockUIContainer(strBarCode));

                    visual.Width = new FigureLength(250, FigureUnitType.Pixel);
                    visual.Height = new FigureLength(50, FigureUnitType.Pixel);
                    visual.HorizontalAnchor = FigureHorizontalAnchor.ColumnLeft;
                    visual.HorizontalOffset = 0;
                    visual.Margin = new Thickness(0, 0, 0, 0);

                    paragraph.Inlines.Add(visual);
                    paragraph.Inlines.Add(new LineBreak());
                    paragraph.Inlines.Add(new LineBreak());
                }
                else
                {
                    TextBlock visual = new TextBlock();

                    Run run = new Run(line);
                    visual.Inlines.Add(run);

                    if (Regex.IsMatch(line, @"(\\w\\h|\\h\\w)"))
                        visual.LayoutTransform = new ScaleTransform(2, 3);
                    else if (Regex.IsMatch(line, @"(\\w)"))
                        visual.LayoutTransform = new ScaleTransform(2, 1);
                    else if (Regex.IsMatch(line, @"(\\h)"))
                        visual.LayoutTransform = new ScaleTransform(1, 3);

                    run.Text = Regex.Replace(run.Text, @"\\w|\\h", "");
                    run.Text = Regex.Replace(run.Text, @"\\logo", "");

                    paragraph.Inlines.Add(visual);
                    paragraph.Inlines.Add(new LineBreak());
                }
            }

            flowDocument.Blocks.Add(paragraph);

            return flowDocument;
        }

        public override void PrintPDF(byte[] Pdf, int Copies = 1)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PrintPDF_(Pdf, Copies);
            });
        }

        private void PrintPDF_(byte[] Pdf, int Copies = 1)
        {
            //GemBox.Pdf Free delivers the same performance and set of features as the Professional version.
            //However, the Free version is limited to two pages.
            //This limitation is enforced during reading or writing files.

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            using (MemoryStream stream = new MemoryStream(Pdf))
            using (PdfDocument document = PdfDocument.Load(stream))
            {
                PrintOptions printOptions = new PrintOptions()
                {
                    CopyCount = Copies,
                    DocumentName = "CashPrinter job"
                };

                document.Print(AppSettings.ThermalPrinterName, printOptions);
            }
        }

        public override bool Test()
        {
            return true;
        }
    }
}
