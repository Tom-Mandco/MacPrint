﻿namespace MacPrint.Classes
{
    using System;
    using Interfaces;
    using System.Text;

    public class App : IApp
    {
        #region Initialization
        private readonly ILog logger;
        private readonly IPurgeFiles purgeFiles;
        private readonly IPrintHandler printHandler;

        private string[] args { get; set; }

        private string filePath { get; set; }       //Arg[0]
        private bool isLandscape { get; set; }      //Arg[1]
        private string printerPath { get; set; }    //Arg[2][1PDF]
        private bool printWithAdobe { get; set; }   //Arg[2PDF]
        private string pageSize { get; set; }       //Arg[3]
        private int fontSize { get; set; }          //Arg[4]
        private string source { get; set; }         //Arg[5]
        private bool isDuplex { get; set; }         //Arg[6]
        private bool printBlankLines { get; set; }  //Arg[7]
        private int startPage { get; set; }         //Arg[8]
        private int endPage { get; set; }           //Arg[9]

        private StringBuilder sb = new StringBuilder();

        public App(ILog logger, IPurgeFiles purgeFiles, IPrintHandler printHandler)
        {
            this.logger = logger;
            this.purgeFiles = purgeFiles;
            this.printHandler = printHandler;
            this.args = args;
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
                        args[0] = args[0].Replace('/','\\');
                        filePath = args[0];
                        sb.Clear();
                        logger.Debug(sb.AppendFormat("Parameter 0 (FilePath): {0}", args[0]).ToString());
                        break;
                    case 1:
                        if (IsPdf(filePath))
                        {
                            if (args[1].Contains("\\"))
                                printerPath = args[1];
                            else
                                printerPath = args[2];

                            sb.Clear();
                            logger.Debug(sb.AppendFormat("Parameter 1 (printerPath): {0}", printerPath).ToString());
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
                            printWithAdobe = (args[2].Contains("\\") ? true : ArgIsTrue(args[2]));

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
                        logger.Debug(string.Format("Parameter 8 (startPage): {0}", startPage));
                        break;
                    case 9:
                        numeric = 9999;
                        isNumeric = int.TryParse(args[9], out numeric);
                        endPage = numeric;
                        logger.Debug(string.Format("Parameter 9 (endPage): {0}", endPage));
                        break;
                }
                i++;
            }
        }
        #endregion

        #region Main Functions
        public void Print(string[] args)
        {
            logger.Debug("MacPrint application started. Running Define Arguments.");
            DefineArguments(args);
            PurgeOldFiles();

            try
             {
                if (IsPdf(filePath))
                {
                    PrintPdf();
                }
                else if (filePath.Contains("testfile"))
                {
                    logger.Info("This is a miracle that this works. check PrintUtils.cs Ln140.");
                    printHandler.SendFileToPrinter(filePath, printerPath);
                }
                else
                {
                    if (printBlankLines == null)
                        printBlankLines = false;
                    PrintFlatFile(args.Length, printBlankLines);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }

            logger.Info(string.Format("Application Ended{0}",
                        Environment.NewLine));
        }

        private void PrintPdf()
        {
            logger.Info("Printing PDF started");
            try
            {
                printHandler.PrintWithAdobe(filePath, printerPath);
                logger.Info("Printing with adobe PDF Ended");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
            }
        }
    

        private void PrintFlatFile(int argsLength, bool printBlanks)
        {
            
            logger.Info("Printing Flat File started");
            bool printBlankLines = (argsLength > 7) && (printBlanks);

            printHandler.PrintNoExtensionFile(filePath, isLandscape, printerPath, pageSize, fontSize.ToString(), source, isDuplex, printBlankLines, startPage, endPage);
            
            logger.Info("Printing Flat File Ended");
        }

        private void PurgeOldFiles()
        {
            try
            {
                purgeFiles.FolderCleanup(7);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
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