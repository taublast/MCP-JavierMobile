namespace MobileDevMcpServer.Helpers
{
    public static class Logger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public static void LogInfo(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public static void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        public static void LogError(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }

        /// <summary>
        /// Logs an exception with a custom message and exception details.
        /// </summary>
        /// <param name="message">The custom message to be logged alongside the exception.</param>
        /// <param name="e">The exception to be logged.</param>
        public static void LogException(string message, Exception e)
        {
            Console.WriteLine($"EXCEPTION: {message}, Details: {e}");
        }
    }
}
