namespace MacPrint.Classes
{
    using System;
    using System.IO;
    using Interfaces;

    public class PurgeFiles : IPurgeFiles
    {
        public void FolderCleanup(int numberOfDays = 1)
        {
            string[] files = Directory.GetFiles(@"c:\temp\printlog\");

            var lockObject = new object();

            lock (lockObject)
            {
                foreach (string file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.CreationTimeUtc < DateTime.Now.AddDays(-1 * numberOfDays))
                        fi.Delete();
                }
            }
        }
    }
}
