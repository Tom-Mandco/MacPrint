namespace PrintLogger
{
    public interface INLogger
    {
        void Trace(string msg);
        void Error(string msg);
        void Info(string msg);
        void Debug(string msg);
        void Warning(string msg);
    }
}
