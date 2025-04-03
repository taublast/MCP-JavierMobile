using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidFilesTool
    {
        /// <summary>
        /// Pushes a local file to an Android device using ADB (Android Debug Bridge).
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target device.</param>
        /// <param name="localPath">The local file path to be uploaded.</param>
        /// <param name="devicePath">The destination file path on the device.</param>
        /// <returns>
        /// A string message indicating the result of the operation:
        /// - A success message if the file upload completes successfully.
        /// - An error message if an exception occurs during the operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if:
        /// - ADB is not installed or not in the system PATH.
        /// - The device serial is null or empty.
        /// - The specified local file does not exist.
        /// </exception>
        [McpServerTool(Name = "android_files_push")]
        [Description("Pushes a local file to an Android device.")]
        public string PushFile(string deviceSerial, string localPath, string devicePath)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    throw new Exception($"Error: Device not found.");
                }

                // Check if the local file exists
                if (!File.Exists(localPath))
                {
                    throw new Exception($"Error: Local file {localPath} does not exist.");
                }

                Process.ExecuteCommand($"adb push -s {deviceSerial} {localPath} {devicePath}");

                return $"File Uploaded Successfully. The file `{localPath}` has been uploaded to `{devicePath}` on device {deviceSerial}.";
            }
            catch (Exception ex)
            {
                return $"Error pushing file: {ex.Message}";
            }
        }

        /// <summary>
        /// Pulls a file from an Android device to the local machine using ADB (Android Debug Bridge).
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target device.</param>
        /// <param name="localPath">The destination path on the local machine where the file will be saved.</param>
        /// <param name="devicePath">The path of the file on the device that will be downloaded.</param>
        /// <returns>A message indicating the result of the file pull operation.</returns>
        [McpServerTool(Name = "android_files_push")]
        [Description("Pulls a file from an Android device to the local machine.")]
        public string PullFile(string deviceSerial, string localPath, string devicePath)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    throw new Exception($"Error: Device not found.");
                }

                // Check if the local file exists
                if (!File.Exists(localPath))
                {
                    throw new Exception($"Error: Local file {localPath} does not exist.");
                }

                Process.ExecuteCommand($"adb pull -s {deviceSerial} {devicePath} {localPath}");

                return $"File downloaded Successfully. The file `{devicePath}` has been downloaded to `{localPath}` from device {deviceSerial}.";
            }
            catch (Exception ex)
            {
                return $"Error pushing file: {ex.Message}";
            }
        }
    }
}