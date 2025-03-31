using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidScreenshotTool
    {
        /// <summary>
        /// Captures a screenshot from the specified Android device and retrieves the image data as a byte array.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device. This is required if multiple devices are connected.</param>
        /// <returns>
        /// A byte array containing the screenshot image data, or null if the operation fails or the device is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="deviceSerial"/> is null or empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the screenshot capturing process or ADB command execution.
        /// </exception>
        [McpServerTool("android_screenshot")]
        [Description("Captures a screenshot from the specified Android device.")]
        public static byte[]? TakeScreenshot(string deviceSerial)
        {
            try
            {
                Logger.LogInfo($"Initializing shell command execution for device: {deviceSerial}");

                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    Logger.LogError($"Device not found.");
                    throw new Exception($"Error: Device not found.");
                }

                // Define the temporary file path on the device
                string deviceTempFilePath = "/sdcard/screenshot.png";

                // Take the screenshot on the device
                Logger.LogInfo("Taking screenshot...");
                Process.ExecuteCommand($"shell -s {deviceSerial} screencap -p {deviceTempFilePath}");

                // Define a temporary file path to save the pulled screenshot locally
                string localTempFilePath = Path.GetTempFileName();

                // Pull the screenshot from the device to the local machine
                Logger.LogInfo("Pulling screenshot to local machine...");
                Process.ExecuteCommand($"adb pull {deviceTempFilePath} \"{localTempFilePath}\"");

                // Delete the screenshot file from the device
                Logger.LogInfo("Deleting screenshot from device...");
                Process.ExecuteCommand($"shell rm {deviceTempFilePath}");

                // Read the screenshot image data into a byte array
                byte[] imageData = File.ReadAllBytes(localTempFilePath);

                // Delete the temporary file after reading
                File.Delete(localTempFilePath);

                Logger.LogInfo("Screenshot captured and image data retrieved successfully.");
                return imageData;
            }
            catch (Exception ex)
            {
                Logger.LogException("Error taking screenshot", ex);
    
                return null;
            }
        }
    }
}
