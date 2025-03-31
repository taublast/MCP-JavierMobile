using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidUiTool
    {
        /// <summary>
        /// Simulates a tap gesture on the screen of an Android device.
        /// This method requires the device's serial number and the screen coordinates (X, Y) as inputs.
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="x">The X coordinate of the screen where the tap should occur.</param>
        /// <param name="y">The Y coordinate of the screen where the tap should occur.</param>
        /// <returns>
        /// A string indicating the result of the tap operation.
        /// </returns>
        [McpServerTool("android_ui_tap")]
        [Description("Simulates a tap gesture on the device screen at the specified coordinates (X, Y).")]
        public string Tap(string deviceSerial, int x, int y)
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
                    Logger.LogError($"Device {deviceSerial} not connected or not found.");
                    return $"Error: Device {deviceSerial} not connected or not found.";
                }

                // Log the tap operation start
                Logger.LogInfo($"Tapping at coordinates ({x}, {y}) on device {deviceSerial}...");

                // Perform the tap operation
                Process.ExecuteCommand($"adb shell input tap {x} {y}");

                // Log the successful completion
                Logger.LogInfo("Tap operation completed successfully.");
                return $"Successfully tapped at ({x}, {y})";
            }
            catch (Exception ex)
            {
                Logger.LogException("Error executing tap operation", ex);
                Logger.LogError($"Error executing tap operation: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Performs a swipe gesture on the screen of a connected Android device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="startX">The starting X coordinate of the swipe.</param>
        /// <param name="startY">The starting Y coordinate of the swipe.</param>
        /// <param name="endX">The ending X coordinate of the swipe.</param>
        /// <param name="endY">The ending Y coordinate of the swipe.</param>
        /// <param name="durationMs">The duration of the swipe in milliseconds (default is 500).</param>
        /// <returns>
        /// A string indicating the result of the swipe operation.
        /// </returns>
        [McpServerTool("android_ui_swipe")]
        [Description("Performs a swipe gesture on the screen of a connected Android device.")]
        public string Swipe(string deviceSerial, int startX, int startY, int endX, int endY, int durationMs = 500)
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
                    Logger.LogError($"Device {deviceSerial} not connected or not found.");
                    return $"Error: Device {deviceSerial} not connected or not found.";
                }

                // Log the swipe operation start
                Logger.LogInfo($"Swiping from ({startX}, {startY}) to ({endX}, {endY}) with duration {durationMs}ms on device {deviceSerial}...");

                // Perform the swipe operation
                Process.ExecuteCommand($"shell input touchscreen swipe {startX},{startY} {endX},{endY} {durationMs}");

                // Log the successful completion
                Logger.LogInfo("Swipe operation completed successfully.");
                return $"Successfully swiped from ({startX}, {startY}) to ({endX}, {endY})";
            }
            catch (Exception ex)
            {
                Logger.LogException("Error executing swipe operation", ex);
                return $"Error executing swipe operation: {ex.Message}";
            }
        }

        /// <summary>
        /// Inputs text into a connected Android device as if typed from a keyboard.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="text">The text to be input into the device.</param>
        /// <returns>
        /// A string indicating the result of the text input operation.
        /// </returns>
        [McpServerTool("android_ui_input_text")]
        [Description("Inputs text into a connected Android device as if typed from a keyboard.")]
        public string InputText(string deviceSerial, string text)
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
                    Logger.LogError($"Device {deviceSerial} not connected or not found.");
                    return $"Error: Device {deviceSerial} not connected or not found.";
                }

                // Log the text input operation start
                Logger.LogInfo($"Inputting text on device {deviceSerial}...");

                // Perform the text input operation
                Process.ExecuteCommand($"adb shell input text {text}");

                // Log the successful completion
                Logger.LogInfo("Text input completed successfully.");
                return "Successfully input text on device.";
            }
            catch (Exception e)
            {
                Logger.LogException("Error executing text input operation", e);
                return $"Error executing text input operation: {e.Message}";
            }
        }
    }
}
