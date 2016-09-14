namespace Print
{
    public interface IPrintUtilsHelper
    {
        bool SendFileToPrinter(string szFileName, string szPrinterName);
        void PrintWithAdobe(string filePath, string printername);
        void PrintWithFoxit(string filePath, string printername);
        void PrintNoextentionFile(string filePath, bool isLandScape, string printerName, string paperSize, string fontSize, string sourceName = "D", bool isDublex = false, bool printBlankLines = false);
    }
}