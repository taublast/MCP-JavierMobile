using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidShellTool
    {
        /// <summary>
        /// Runs a shell command on the specified device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the device.</param>
        /// <param name="command">The shell command to execute.</param>
        /// <returns>A formatted string containing the command output.</returns>
        [McpServerTool("android_shell_command")]
        [Description("Runs a shell command on the specified device.")]
        public string ShellCommand(string deviceSerial, string command)
        {
            Logger.LogInfo($"Initializing shell command execution for device: {deviceSerial}");

            if (string.IsNullOrEmpty(deviceSerial))
            {
                Logger.LogError($"Device not found.");
                return $"Error: Device not found.";
            }

            Logger.LogInfo($"Device found: {deviceSerial}. Preparing to execute shell command: {command}");

            try
            {
                Logger.LogInfo($"Executing shell command on device: {deviceSerial}. Command: {command}");
                string result = Process.ExecuteCommand(command);

                // Check for security validation failure
                if (result.StartsWith("Error: Command rejected", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.LogError($"Command rejected on device {deviceSerial}: {command}");
                    return result;
                }

                Logger.LogInfo($"Command executed successfully on device: {deviceSerial}. Formatting output...");

                // Format the output
                string output = $"# Command Output from {deviceSerial}\n\n";
                output += $"```\n{result}\n```";

                Logger.LogInfo($"Output formatting completed for device: {deviceSerial}");
                return output;
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while executing shell command on device {deviceSerial}. Command: {command}", ex);
                return $"Error executing shell command: {ex.Message}";
            }
        }
    }
}