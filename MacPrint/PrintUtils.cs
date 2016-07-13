namespace Print
{
    using System;
    using PrintLogger;
    using System.Text;

    public class PrintUtils
    {
        #region Initialization
        private INLogger logger { get; set; }
        private IPrintUtilsHelper PrintUtilsHelper { get; set; }
        private string[] args { get; set; }

        private string filePath { get; set; }       //Arg[0]
        private bool isLandscape { get; set; }      //Arg[1]
        private string printerPath { get; set; }    //Arg[2]
        private bool printWithAdobe { get; set; }   //Arg[2PDF]
        private string pageSize { get; set; }       //Arg[3]
        private int fontSize { get; set; }          //Arg[4]
        private string source { get; set; }         //Arg[5]
        private bool isDuplex { get; set; }         //Arg[6]
        private bool printBlankLines { get; set; }  //Arg[7]
        private int startPage { get; set; }         //Arg[8]
        private int endPage { get; set; }           //Arg[9]

        private StringBuilder sb = new StringBuilder();

        public PrintUtils(INLogger logger, IPrintUtilsHelper PrintUtilsHelper, string[] args)
        {
            this.logger = logger;
            this.PrintUtilsHelper = PrintUtilsHelper;
            this.args = args;
            DefineArguments(args);
        }

        private void DefineArguments(string[] args)
        {
            int i = 0;
            foreach(string param in args)
            {
                switch (i)
                {
                    case 0:
                        args[0] = args[0].TrimEnd(new char[] { '\r', '\n' });
                        filePath = args[0];
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 0 (FilePath): {0}", args[0]).ToString());
                        break;
                    case 1:
                        if (IsPdf(filePath))
                        {
                            printerPath = args[1];
                            sb.Clear();
                            logger.Debug(sb.AppendFormat("Parameter 1 (printerPath): {0}", args[1]).ToString());
                        }
                        else
                        {
                            isLandscape = ArgIsTrue(args[1]);
                            sb.Clear();
                            logger.Debug(sb.AppendFormat("Parameter 1 (isLandscape): {0}", args[1]).ToString());
                        }
                        break;
                    case 2:
                        if (IsPdf(filePath))
                        {
                            printWithAdobe = ArgIsTrue(args[2]);
                            sb.Clear();
                            logger.Debug(sb.AppendFormat("Parameter 2 (printWithAdobe): {0}", args[2]).ToString());
                        }
                        else
                        {
                            printerPath = args[2];
                            sb.Clear();
                            logger.Debug(sb.AppendFormat("Parameter 2 (printerPath): {0}", args[2]).ToString());
                        }
                        break;
                    case 3:
                        pageSize = args[3];
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 3 (pageSize): {0}", args[3]).ToString());
                        break;
                    case 4:
                        int numeric;
                        bool isNumeric = int.TryParse(args[4], out numeric);
                        if(isNumeric)
                        {
                            fontSize = numeric;
                        }
                        else
                        {
                            fontSize = 10;
                        }
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 4 (fontSize): {0}", args[4]).ToString());
                        break;
                    case 5:
                        source = args[5];
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 5 (Sourse): {0}", args[5]).ToString());
                        break;
                    case 6:
                        isDuplex = ArgIsTrue(args[6]);
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 6 (isDuplex): {0}", args[6]).ToString());
                        break;
                    case 7:
                        printBlankLines = (args.Length > 7) && (ArgIsTrue(args[7]));
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 7 (printBlankLines): {0}", (args.Length > 7) && (ArgIsTrue(args[7]))).ToString());
                        break;
                    case 8:
                        numeric = 0;
                        isNumeric = int.TryParse(args[8], out numeric);
                        startPage = numeric;
                        break;
                    case 9:
                        numeric = 9999;
                        isNumeric = int.TryParse(args[9], out numeric);
                        endPage = numeric;
                        break;
                }
                i++;
            }
        }
        #endregion

        #region Main Functions
        public void Print()
        {
            PurgeOldFiles();

            try
            {
                if (IsPdf(filePath))
                {
                    PrintPdf();
                }
                else if (IsLable(filePath))
                {
                    logger.Warning("This is a miricle that this works. check PrintUtils.cs Ln140.");
                    PrintUtilsHelper.SendFileToPrinter(filePath.Replace(".lable", ""), args[1]);
                }
                else
                {
                    PrintFlatFile();
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                logger.Debug(sb.AppendFormat("Application FAILED\nMessage: {0}\nStack Trace: {1}", ex.Message,ex.StackTrace.ToString()).ToString());
                logger.Info("filePath: " + filePath);
            }

            logger.Info("Application Ended");
        }


        private void PrintPdf()
        {
            logger.Info("Printing PDF started");
            if (ArgIsTrue(args[2]))
            {
                try
                {
                    PrintUtilsHelper.PrintWithAdobe(filePath, args[1]);
                    logger.Info("Printing with adobe PDF Ended");
                }
                catch (Exception ex)
                {
                    logger.Info("Application FAILED" + ex.Message);
                }
            }
            else
            {
                if (PrintUtilsHelper.SendFileToPrinter(filePath, args[1]))
                {
                    logger.Info("Printing PDF Ended");
                }
                else
                {
                    logger.Info("Printing PDF Failed");
                }
            }
        }

        private void PrintFlatFile()
        {
            
            logger.Info("Printing Flat File started");
            bool printBlankLines = (args.Length > 7) && (ArgIsTrue(args[7]));

            PrintUtilsHelper.PrintNoextentionFile(filePath, isLandscape, printerPath, pageSize, fontSize.ToString(), source, isDuplex, printBlankLines);
            
            logger.Info("Printing Flat File Ended");
        }

        private void PurgeOldFiles()
        {
            try
            {
                PurgeFiles.FolderCleanup(7);
            }
            catch (Exception ex)
            {
                logger.Error("Folder Cleanup FAILED" + ex.Message);
            }
        }
        #endregion

        #region Utilities
        private static bool IsPdf(string filename)
        {
            return filename.ToLower().Contains(".pdf");
        }

        private static bool IsLable(string filename)
        {
            return filename.ToLower().Contains(".lable");
        }

        private static bool ArgIsTrue(string arg)
        {
            return arg.ToLower() == "true";
        }
        #endregion
    }
}
