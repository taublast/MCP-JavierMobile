using MobileDevMcpServer.Helpers;
using MobileDevMcpServer.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidDeviceTool
    {
        /// <summary>
        /// Retrieves a list of connected Android devices.
        /// </summary>
        /// <returns>
        /// A string containing the list of connected devices and their details, such as serial numbers.
        /// </returns>
        [McpServerTool("android_list_devices")]
        [Description("Lists all available Android devices.")]
        public string ListDevices()
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo("Starting device listing operation using ADB command...");

                var devices = new List<AdbDevice>();
                string result = Process.ExecuteCommand("adb devices -l");

                Logger.LogInfo("ADB command executed successfully. Parsing device list...");

                string[] lines = result.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

                // Skip the first line (header)
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Parse each line to extract device details
                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var device = new AdbDevice
                        {
                            Product = GetPropertyFromParts(parts, "product:"),
                            Model = GetPropertyFromParts(parts, "model:"),
                            Device = GetPropertyFromParts(parts, "device:"),
                            SerialNumber = parts[0], // Assuming the serial number is the first part
                        };
                        devices.Add(device);
                        Logger.LogInfo($"Device added: {device.SerialNumber}, Model: {device.Model}");
                    }
                }

                if (devices is null || devices.Count == 0)
                {
                    Logger.LogWarning("No devices found.");
                    return "No devices found.";
                }

                // Format the result as a table
                Logger.LogInfo($"Found {devices.Count} device(s). Formatting the device list...");
                var devicesStr = "# Devices\n\n";
                devicesStr += "| Serial          | Device           | Product          | Model            |\n";
                devicesStr += "|-----------------|------------------|------------------|------------------|\n";

                foreach (var device in devices)
                {
                    devicesStr += $"| `{device.SerialNumber}` | `{device.Device}` | `{device.Product}` | `{device.Model}` |\n";
                }

                Logger.LogInfo("Device list formatted successfully.");
                return devicesStr;
            }
            catch (Exception ex)
            {
                Logger.LogException("An error occurred while listing devices.", ex);
                return $"Error retrieving device list: {ex.Message}";
            }
        }

        /// <summary>
        /// Boots up an Android Virtual Device (AVD) emulator with the specified name.
        /// </summary>
        /// <param name="avdName">The name of the Android Virtual Device (AVD) to be booted.</param>
        [McpServerTool("android_boot_device")]
        [Description("Boots the specified Android device.")]
        public void BootDevice(string avdName)
        {
            try
            {
                Logger.LogInfo($"Attempting to shut down the Android Virtual Device (AVD) with name: {avdName}");

                if (string.IsNullOrEmpty(avdName))
                {
                    Logger.LogError("AVD name is missing or invalid. Cannot proceed with shutdown.");
                    throw new ArgumentNullException(nameof(avdName), "Error: Device name is missing or invalid.");
                }

                // Execute the adb command to kill the emulator
                Process.ExecuteCommand($"adb -s {avdName} emu kill");

                Logger.LogInfo($"AVD {avdName} shut down successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error booting the device: {ex.Message}");
            }
        }

        /// <summary>
        /// Shuts down an Android Virtual Device (AVD) emulator with the specified name.
        /// </summary>
        /// <param name="avdName">The name of the Android Virtual Device (AVD) to be shut down.</param>
        [McpServerTool("android_shutdown_device")]
        [Description("Shuts down the specified Android device.")]
        public void ShutdownDevice(string avdName)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo($"Attempting to shut down the Android Virtual Device (AVD) with name: {avdName}");

                if (string.IsNullOrEmpty(avdName))
                {
                    Logger.LogError("AVD name is missing or invalid. Cannot proceed with shutdown.");
                    throw new ArgumentNullException(nameof(avdName), "Error: Device name is missing or invalid.");
                }

                // Execute the adb command to kill the emulator
                Process.ExecuteCommand($"adb -s {avdName} emu kill");

                Logger.LogInfo($"AVD {avdName} shut down successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error shutting down the device: {ex.Message}");
            }
        }

        // Extracts the value of a specific property from an array of strings.
        private string GetPropertyFromParts(string[] parts, string propertyKey)
        {
            foreach (var part in parts)
            {
                if (part.StartsWith(propertyKey, StringComparison.OrdinalIgnoreCase))
                {
                    return part.Substring(propertyKey.Length);
                }
            }

            return string.Empty;
        }
    }
}
