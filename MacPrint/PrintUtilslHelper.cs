namespace Print
{
    using PrintLogger;

    public class PrintUtilslHelper : IPrintUtilsHelper
    {
        private INLogger Logger { get; set; }

        public PrintUtilslHelper(INLogger logger)
        {
            Logger = logger;
        }

        public bool SendFileToPrinter(string szFileName, string szPrinterName)
        {
            return PrinterHelper.SendFileToPrinter(szFileName, szPrinterName);
        }

        public void PrintWithAdobe(string filePath, string printername)
        {
            PrinterHelper.PrintWithAdobe(filePath, printername);
        }

        public void PrintNoextentionFile(string filePath, bool isLandScape, string printerName, string sizeName, string fontSize,
            string sourceName = "D", bool isDublex = false, bool printBlankLines = false)
        {
            Logger.Info("Printing no extension file.");
            PrinterHelper.printNoextentionFile(filePath, isLandScape, printerName, sizeName, fontSize, sourceName, isDublex, printBlankLines);
        }
    }
}