using MobileDevMcpServer.Helpers;
using MobileDevMcpServer.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class IosDeviceTool
    {
        /// <summary>
        /// Retrieves and lists all available simulator devices.
        /// </summary>
        /// <returns>
        /// A string formatted as a table, containing details of simulator devices such as name, ID, and runtime.
        /// If no devices are found or an error occurs, an appropriate message is returned.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the device retrieval process.
        /// </exception>
        [McpServerTool("ios_list_devices")]
        [Description("Lists all available iOS simulator devices.")]
        public string ListDevices()
        {
            try
            {
                Logger.LogInfo("Executing command to retrieve simulator devices as JSON...");
                string resultJson = Process.ExecuteCommand("xcrun simctl list devices --json");
                Logger.LogInfo("Command executed successfully. Parsing JSON response...");

                var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<SimulatorDevice>>>>(resultJson);
                var devices = new List<SimulatorDevice>();

                if (result is not null)
                {
                    Logger.LogInfo("Parsing simulator devices from the JSON response...");
                    foreach (var runtime in result["devices"])
                    {
                        foreach (var device in runtime.Value)
                        {
                            // Simplify runtime string and add device to the list
                            device.Runtime = runtime.Key.Replace("com.apple.CoreSimulator.SimRuntime.", string.Empty);
                            devices.Add(device);
                            Logger.LogInfo($"Device added: Name={device.Name}, Udid={device.Udid}, Runtime={device.Runtime}");
                        }
                    }
                    Logger.LogInfo($"Successfully parsed {devices.Count} simulator device(s).");
                }
                else
                {
                    Logger.LogWarning("No devices found in the JSON response.");
                    return "No devices found.";
                }

                // Format the result as a table
                Logger.LogInfo("Formatting devices into a table...");
                var devicesStr = "# Simulator Devices\n\n";
                devicesStr += "| Name            | Udid              | Runtime         |\n";
                devicesStr += "|-----------------|-----------------|-----------------|\n";

                foreach (var device in devices)
                {
                    devicesStr += $"| {device.Name} | {device.Udid} | {device.Runtime} |\n";
                }

                Logger.LogInfo("Devices formatted successfully.");
                return devicesStr;
            }
            catch (Exception ex)
            {
                Logger.LogException("An error occurred while listing simulator devices.", ex);
                return $"Error retrieving simulator devices: {ex.Message}";
            }
        }

        /// <summary>
        /// Retrieves the name and ID of the first booted simulator device.
        /// </summary>
        /// <returns>
        /// The name and ID of the booted simulator device in a formatted string.
        /// If no booted simulator is found, an exception is thrown.
        /// </returns>
        [McpServerTool("ios_booted_device")]
        [Description("Retrieves the name and ID of the first booted simulator device.")]
        public static string GetBootedDevice()
        {
            try
            {
                Logger.LogInfo("Executing command to retrieve simulator devices...");
                var process = Process.StartProcess("xcrun simctl list devices");
                Logger.LogInfo("Command executed successfully.");
                var stdOut = process.StandardOutput.ReadToEnd();
                var stdErr = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(stdOut))
                {
                    throw new Exception(stdErr);
                }

                var lines = stdOut.Split(["\n"], StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.Contains("Booted", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract the UUID - it's inside parentheses
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"\(([-0-9A-F]+)\)");

                        if (match.Success)
                        {
                            var deviceId = match.Groups[1].Value;
                            var deviceName = line.Split(['('], StringSplitOptions.None)[0].Trim();

                            // Format the result as a table
                            Logger.LogInfo("Formatting booted device into a table...");

                            var deviceStr = "# Booted Device\n\n";
                            deviceStr += "| Name            | Udid              |\n";
                            deviceStr += "|-----------------|-----------------|\n";
                            deviceStr += $"| {deviceName} | {deviceId} |\n";

                            Logger.LogInfo("Devices formatted successfully.");

                            return deviceStr;
                        }
                    }
                }

                return "No booted devices found.";
            }
            catch (Exception ex)
            {
                Logger.LogException("An error occurred while retrieving the booted device.", ex);
                return $"Error retrieving booted device: {ex.Message}";
            }
        }

        /// <summary>
        /// Boots a simulator device with the specified device ID.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the simulator device to be booted.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="deviceId"/> is null or empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the boot operation.
        /// </exception>
        [McpServerTool("ios_boot_device")]
        [Description("Boots the specified iOS simulator device.")]
        public void BootDevice(string deviceId)
        {
            try
            {
                Logger.LogInfo($"Attempting to boot simulator device with ID: {deviceId}");

                if (string.IsNullOrEmpty(deviceId))
                {
                    Logger.LogError("Device ID is missing or invalid. Cannot proceed with booting the device.");
                    throw new ArgumentNullException(nameof(deviceId), "Error: Invalid or missing device ID.");
                }

                // Execute the command to boot the simulator device
                Logger.LogInfo($"Executing command: xcrun simctl boot {deviceId}");
                Process.ExecuteCommand($"xcrun simctl boot {deviceId}");
                Logger.LogInfo($"Simulator device with ID {deviceId} booted successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to boot the simulator device with ID {deviceId}.", ex);
                throw new Exception($"Error booting the simulator device: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Shuts down a simulator device with the specified device ID.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the simulator device to be shut down.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="deviceId"/> is null or empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the shutdown operation.
        /// </exception>
        [McpServerTool("ios_shutdown_device")]
        [Description("Shuts down the specified iOS simulator device.")]
        public void ShutdownDevice(string deviceId)
        {
            try
            {
                Logger.LogInfo($"Attempting to shut down simulator device with ID: {deviceId}");

                if (string.IsNullOrEmpty(deviceId))
                {
                    Logger.LogError("Device ID is missing or invalid. Cannot proceed with shutting down the device.");
                    throw new ArgumentNullException(nameof(deviceId), "Error: Invalid or missing device ID.");
                }

                // Execute the command to shut down the simulator device
                Logger.LogInfo($"Executing command: xcrun simctl shutdown {deviceId}");
                Process.ExecuteCommand($"xcrun simctl shutdown {deviceId}");
                Logger.LogInfo($"Simulator device with ID {deviceId} shut down successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to shut down the simulator device with ID {deviceId}.", ex);
                throw new Exception($"Error shutting down the simulator device: {ex.Message}", ex);
            }
        }
    }
}
