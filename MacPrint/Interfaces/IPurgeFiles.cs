namespace MacPrint.Interfaces
{
    public interface IPurgeFiles
    {
        void FolderCleanup(int numberOfDays = 1);
    }
}
