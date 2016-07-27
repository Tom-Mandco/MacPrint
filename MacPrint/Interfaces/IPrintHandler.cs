namespace MacPrint.Interfaces
{
    public interface IPrintHandler
    {
        void PrintNoExtensionFile(string filePath, bool isLandScape, string printerName, string sizeName, string fontSize, string sourceName = "D", bool isDublex = false, bool pPrintBlankLines = false, int startPage = 0, int endPage = 9999);
        void PrintWithAdobe(string filePath, string printername);
        void PrintWithFoxit(string filePath, string printername);
        bool SendFileToPrinter(string szFileName, string szPrinterName);
    }
}
