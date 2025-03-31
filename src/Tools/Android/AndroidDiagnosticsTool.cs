using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidDiagnosticsTool
    {
        /// <summary>
        /// Captures a comprehensive bug report from a connected Android device.
        /// A bug report includes diagnostic information such as system logs, running processes, and device state.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="outputPath">The file path where the bug report should be saved. If not provided, a temporary path will be used.</param>
        /// <param name="timeoutSeconds">The maximum duration allowed for the bug report process, in seconds (default is 500 seconds).</param>
        /// <returns>
        /// A string indicating the success or failure of the bug report operation, including the output file path or relevant error message.
        /// </returns>
        [McpServerTool("android_diagnostics_bug_report")]
        [Description("Captures a comprehensive bug report from a connected Android device.")]
        public string CaptureBugReport(string deviceSerial, string outputPath = "", int timeoutSeconds = 500)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    Logger.LogError("Device serial number is missing or invalid. Cannot proceed.");
                    return "Error: Invalid or missing device serial number.";
                }

                // Log operation details
                Logger.LogInfo($"Starting bug report capture for device: {deviceSerial}");
                Logger.LogInfo("The process might take a few minutes based on the device's current state.");

                // If output path is not provided, generate a temporary file
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    string tempDir = Path.GetTempPath();
                    outputPath = Path.Combine(tempDir, $"bugreport_{deviceSerial}.zip");
                    Logger.LogInfo($"Output path not provided. Using temporary file: {outputPath}");
                }

                // Ensure the output directory exists
                string? outputDir = Path.GetDirectoryName(outputPath);
                if (outputDir == null)
                {
                    Logger.LogError("Unable to determine the output directory from the specified path.");
                    return "Error: Invalid output path provided for bug report.";
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                    Logger.LogInfo($"Created missing output directory: {outputDir}");
                }

                // Run bug report command using ADB
                Logger.LogInfo($"Executing ADB command to capture bug report: adb -s {deviceSerial} bugreport {outputPath}");
                var process = Process.StartProcess($"adb -s {deviceSerial} bugreport {outputPath}");

                // Wait for process to complete or timeout
                if (!process.WaitForExit(timeoutSeconds * 1000))
                {
                    process.Kill();
                    Logger.LogError($"Bug report capture timed out after {timeoutSeconds} seconds.");
                    return $"Error: Bug report capture exceeded the time limit of {timeoutSeconds} seconds.";
                }

                // Check process exit code
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    Logger.LogError($"Bug report failed. Exit code: {process.ExitCode}");
                    Logger.LogError($"ADB Error Message: {error}");
                    return $"Error: Failed to capture bug report. ADB message: {error}";
                }

                // Verify if output file exists
                if (!File.Exists(outputPath))
                {
                    Logger.LogError("Bug report file was not created. ADB operation did not produce the expected file.");
                    return "Error: Bug report could not be saved because the output file was not generated.";
                }

                // Log file size and success
                long fileSize = new FileInfo(outputPath).Length;
                double fileSizeMb = fileSize / (1024.0 * 1024.0);
                Logger.LogInfo($"Bug report successfully captured! File saved at: {outputPath}");
                Logger.LogInfo($"File size: {fileSizeMb:F2} MB");

                return $"Bug report completed successfully. Saved to: {outputPath} ({fileSizeMb:F2} MB)";
            }
            catch (Exception ex)
            {
                Logger.LogException("An unexpected error occurred during bug report capture.", ex);
                return $"Error: Failed to capture bug report. Details: {ex.Message}";
            }
        }
    }
}