using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class IosAppManagementTool
    {
        /// <summary>
        /// Installs an application on a specified simulator device.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the simulator device.</param>
        /// <param name="appPath">The file path to the application to be installed.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="deviceId"/> or <paramref name="appPath"/> is null or empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the installation process.
        /// </exception>
        [McpServerTool("ios_install_app")]
        [Description("Installs an application on the specified iOS simulator device.")]
        public void InstallApp(string deviceId, string appPath)
        {
            try
            {
                Logger.LogInfo($"Attempting to install application on simulator device with ID: {deviceId}, App Path: {appPath}");

                if (string.IsNullOrEmpty(deviceId))
                {
                    Logger.LogError("Device ID is missing or invalid. Cannot proceed with app installation.");
                    throw new ArgumentNullException(nameof(deviceId), "Error: Invalid or missing device ID.");
                }

                if (string.IsNullOrEmpty(appPath))
                {
                    Logger.LogError("Application path is missing or invalid. Cannot proceed with app installation.");
                    throw new ArgumentNullException(nameof(appPath), "Error: Invalid or missing application path.");
                }

                // Execute the command to install the application
                Logger.LogInfo($"Executing command: xcrun simctl install {deviceId} \"{appPath}\"");
                Process.ExecuteCommand($"xcrun simctl install {deviceId} \"{appPath}\"");
                Logger.LogInfo($"Application installed successfully on simulator device with ID: {deviceId}");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to install the application on device {deviceId}.", ex);
                throw new Exception($"Error installing application: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Launches an application on a specified simulator device.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the simulator device.</param>
        /// <param name="bundleId">The bundle identifier of the application to be launched.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <paramref name="deviceId"/> or <paramref name="bundleId"/> is null or empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the launch process.
        /// </exception>
        [McpServerTool("ios_launch_app")]
        [Description("Launches an application on the specified iOS simulator device.")]
        public void LaunchApp(string deviceId, string bundleId)
        {
            try
            {
                Logger.LogInfo($"Attempting to launch application with Bundle ID: {bundleId} on simulator device with ID: {deviceId}");

                if (string.IsNullOrEmpty(deviceId))
                {
                    Logger.LogError("Device ID is missing or invalid. Cannot proceed with launching the application.");
                    throw new ArgumentNullException(nameof(deviceId), "Error: Invalid or missing device ID.");
                }

                if (string.IsNullOrEmpty(bundleId))
                {
                    Logger.LogError("Bundle ID is missing or invalid. Cannot proceed with launching the application.");
                    throw new ArgumentNullException(nameof(bundleId), "Error: Invalid or missing Bundle ID.");
                }

                // Execute the command to launch the application
                Logger.LogInfo($"Executing command: xcrun simctl launch {deviceId} {bundleId}");
                Process.ExecuteCommand($"xcrun simctl launch {deviceId} {bundleId}");
                Logger.LogInfo($"Application with Bundle ID {bundleId} launched successfully on simulator device with ID {deviceId}");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to launch the application with Bundle ID {bundleId} on device {deviceId}.", ex);
                throw new Exception($"Error launching application: {ex.Message}", ex);
            }
        }
    }
}
