namespace MacPrint.Classes
{
    using Interfaces;
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    class PrintHandler : IPrintHandler
    {
        #region Initialisation
        private readonly ILog logger;
        private Font printFont;
        private StreamReader streamToPrint;
        private static bool printBlankLines;
        private static bool endOfPage = false;
        private static string topLine = "tpt";
        private int pageCount = 1;
        private int startPage = 0;
        private int endPage = 9999;

        public PrintHandler(ILog logger)
        {
            this.logger = logger;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);
        #endregion

        #region Printing Flat Files
        public void PrintNoExtensionFile(string filePath, bool isLandScape, string printerName, string sizeName, string fontSize, string sourceName = "D", bool isDublex = false, bool pPrintBlankLines = false, int pStartPage = 0, int pEndPage = 9999)
        {
            logger.Debug("PrintNoExtensionFile");
            try
            {
                logger.Debug("Opening Stream '" + filePath + "'");
                streamToPrint = new StreamReader(filePath);
                logger.Debug("Stream opened");
                FileInfo fileInfo = new FileInfo(filePath);
                logger.Debug("Size of file: " + fileInfo.Length);
                try
                {
                    printBlankLines = pPrintBlankLines;
                    startPage = pStartPage;
                    endPage = (pEndPage > 0) ? pEndPage : 9999;

                    printFont = new Font("Courier New", Convert.ToInt16(fontSize));
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                        (this.pd_PrintPage);

                    pd.PrinterSettings.PrinterName = printerName;
                    pd.PrinterSettings.PrintRange = PrintRange.SomePages;
                    pd.PrinterSettings.FromPage = startPage;
                    pd.PrinterSettings.ToPage = endPage;

                    pd.DocumentName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

                    if (isDublex)
                        if (isLandScape)
                            pd.DefaultPageSettings.PrinterSettings.Duplex = Duplex.Horizontal;
                        else
                            pd.DefaultPageSettings.PrinterSettings.Duplex = Duplex.Vertical;
                    else
                        pd.DefaultPageSettings.PrinterSettings.Duplex = Duplex.Simplex;

                    if (isLandScape)
                        pd.DefaultPageSettings.Landscape = true;

                    pd.DefaultPageSettings.PaperSize = getPaperSize(sizeName);

                    pd.Print();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
                }
                finally
                {
                    logger.Debug("Closing and disposing stream");
                    streamToPrint.Close();
                    streamToPrint.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = 0;
            float topMargin = -1;
            string line = null;



            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            logger.Info(string.Format("Printing page {0} mig ven.",
                            pageCount));

            linesPerPage += 11;

            logger.Info(string.Format("Linjer pr side: {0}",
                            linesPerPage));

            if (topLine != "tpt")
            {
                yPos = 0;
                if ((pageCount >= startPage) && (pageCount <= endPage))
                    ev.Graphics.DrawString(topLine, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                count++;
                topLine = "tpt";
            }
            // Print each line of the file. 
            while (count < linesPerPage &&
                (line = streamToPrint.ReadLine()) != null)
            {
                string dirtyLine = line;
                string cleanLine = dirtyLine;

                dirtyLine = dirtyLine.Replace("(s4B", "");
                dirtyLine = dirtyLine.Replace("(t2T", "");
                dirtyLine = dirtyLine.Replace("(s0B", "");
                cleanLine = RemoveControlCharacters(dirtyLine);

                line = cleanLine;
                yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));

                if (endOfPage)
                {
                    count = Convert.ToInt32(linesPerPage);
                    topLine = line;
                    pageCount++;
                }
                else
                {
                    if (printBlankLines || line != "")
                    {
                        if ((pageCount >= startPage) && (pageCount <= endPage))
                        {
                            ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                        }
                        count++;
                    }
                }
            }
            endOfPage = false;
            // If more lines exist, print another page. 
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;

        }
        #endregion

        #region Printing PDF Files
        public void PrintWithAdobe(string filePath, string printername)
        {
            try
            {
                string pdfArguments = string.Format(" /t "
                                                    + '"' + filePath + '"' + " "
                                                    + '"' + printername + '"'
                    );

                string pdfPrinterLocation = @"C:\Program Files (x86)\Adobe\Reader 10.0\Reader\AcroRd32.exe";

                var adobe =
                    Registry.LocalMachine.OpenSubKey("Software")
                        .OpenSubKey("Microsoft")
                        .OpenSubKey("Windows")
                        .OpenSubKey("CurrentVersion")
                        .OpenSubKey("App Paths")
                        .OpenSubKey("AcroRd32.exe");
                var path = adobe.GetValue("");

                if (!String.IsNullOrEmpty(path.ToString()))
                {
                    pdfPrinterLocation = path.ToString();
                }
                else
                {

                    var adobeOtherWay =
                        Registry.LocalMachine.OpenSubKey("Software")
                            .OpenSubKey("Classes")
                            .OpenSubKey("acrobat")
                            .OpenSubKey("shell")
                            .OpenSubKey("open")
                            .OpenSubKey("command");
                    var pathOtherWay = adobeOtherWay.GetValue("");

                    if (!String.IsNullOrEmpty(pathOtherWay.ToString()))
                    {
                        pdfPrinterLocation = pathOtherWay.ToString();
                    }
                }

                if (String.IsNullOrEmpty(pdfPrinterLocation))
                {
                    pdfPrinterLocation = @"C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe";
                }

                logger.Info(string.Format("pdfArugments: {0}{1}pdfPrinterLocation: {2}",
                                            pdfArguments,
                                            Environment.NewLine,
                                            pdfPrinterLocation));

                ProcessStartInfo newProcess = new ProcessStartInfo(pdfPrinterLocation, pdfArguments);
                newProcess.CreateNoWindow = true;
                newProcess.RedirectStandardOutput = true;
                newProcess.UseShellExecute = false;

                Process pdfProcess = new Process();
                pdfProcess.StartInfo = newProcess;
                pdfProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pdfProcess.Start();

                Thread.Sleep(30000);
                pdfProcess.Kill();
            }
            catch (InvalidOperationException invalidOp)
            {
                logger.Error(invalidOp.Message);
                logger.Error(invalidOp.StackTrace);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        public void PrintWithDefaultPDFPrinter(string filePath, string printername)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.Arguments = printername;
            info.FileName = filePath;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (!p.CloseMainWindow())
                p.Kill();
        }
        #endregion

        #region Printing PDFs without 3rd parties


        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        private bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            try
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();

                di.pDocName = "My C#.NET RAW Document";
                di.pDataType = "RAW";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
            return bSuccess;
        }

        public bool SendFileToPrinter(string szFileName, string szPrinterName)
        {
            logger.Debug("SendFileToPrinter");
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        #endregion

        #region Utilities
        private static string RemoveControlCharacters(string inString)
        {
            if (inString == null) return null;
            StringBuilder newString = new StringBuilder();
            char ch;
            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
                else
                {
                    if (ch == '\f')
                    {
                        endOfPage = true;
                    }
                }
            }
            return newString.ToString();
        }

        private static PaperSize getPaperSize(string sizeName)
        {
            PrintDocument pd = new PrintDocument();
            PaperSize size = new PaperSize();
            foreach (PaperSize item in pd.PrinterSettings.PaperSizes)
            {
                if (item.PaperName.Contains(sizeName.ToUpper()))
                {
                    size = item;
                    break;
                }
            }
            return size;
        }
        #endregion
    }
}
