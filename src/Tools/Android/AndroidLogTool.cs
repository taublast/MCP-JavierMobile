using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidLogTool
    {
        /// <summary>
        /// Retrieves the system logs from a connected Android device using the logcat tool.
        /// </summary>
        /// <returns>
        /// A string containing the log output captured from the device.
        /// </returns>
        [McpServerTool("android_device_logcat")]
        [Description("Retrieves the system logs from the connected Android device using logcat.")]
        public string GetDeviceLogcat()
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo("Starting retrieval of system logs using ADB logcat command...");

                // Execute the adb logcat command to capture logs from the device
                string result = Process.ExecuteCommand("adb logcat");

                Logger.LogInfo("System logs retrieved successfully.");
              
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogException("Error retrieving the system logs", ex);
                Logger.LogError($"Error retrieving system logs: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}