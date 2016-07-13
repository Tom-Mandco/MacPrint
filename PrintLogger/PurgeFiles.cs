namespace PrintLogger
{
    using System;
    using System.IO;

    public static class PurgeFiles
    {
        public static void FolderCleanup(int numberOfDays = 1)
        {
            string[] files = Directory.GetFiles(@"c:\temp\printlog\");

            var lockObject = new object();

            lock (lockObject)
            {
                foreach (string file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.CreationTimeUtc < DateTime.Now.AddDays(-1*numberOfDays))
                        fi.Delete();
                }
            }
        }
    }
}