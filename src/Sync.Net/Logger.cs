namespace Sync.Net
{
    public static class StaticLogger
    {
        public static ILogger Logger;

        public static void Log(string newLine)
        {
            Logger?.Log(newLine);
        }
    }
}