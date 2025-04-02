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
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                // Execute the adb logcat command to capture logs from the device
                string result = Process.ExecuteCommand("adb logcat");

                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}