using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidAppManagementTool
    {
        /// <summary>
        /// Installs an application (APK file) on the specified Android device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="appPath">The file path to the APK file to be installed on the device.</param>
        [McpServerTool("android_install_app")]
        [Description("Installs an application on the specified Android emulator device.")]
        public void InstallApp(string deviceSerial, string appPath)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo($"Attempting to install application on device: {deviceSerial}, App Path: {appPath}");

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    Logger.LogError("Device serial number is missing or invalid. Cannot proceed with app installation.");
                    throw new ArgumentNullException(nameof(deviceSerial), "Error: Invalid or missing device serial number.");
                }

                if (string.IsNullOrEmpty(appPath))
                {
                    Logger.LogError("Application path is missing or invalid. Cannot proceed with app installation.");
                    throw new ArgumentNullException(nameof(appPath), "Error: Invalid or missing application path.");
                }

                // Execute the adb install command
                Logger.LogInfo($"Executing ADB command: adb -s {deviceSerial} install \"{appPath}\"");
                Process.ExecuteCommand($"adb -s {deviceSerial} install \"{appPath}\"");
                Logger.LogInfo("Application installed successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to install the application on device {deviceSerial}.", ex);
                throw new Exception($"Error installing application: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Launches an application on the specified Android device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <param name="packageName">The package name of the application to be launched.</param>
        [McpServerTool("android_launch_app")]
        [Description("Launches an application on the specified Android emulator device.")]
        public void LaunchApp(string deviceSerial, string packageName)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo($"Attempting to launch application on device: {deviceSerial}, Package Name: {packageName}");

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    Logger.LogError("Device serial number is missing or invalid. Cannot proceed with app launch.");
                    throw new ArgumentNullException(nameof(deviceSerial), "Error: Invalid or missing device serial number.");
                }

                if (string.IsNullOrEmpty(packageName))
                {
                    Logger.LogError("Package name is missing or invalid. Cannot proceed with app launch.");
                    throw new ArgumentNullException(nameof(packageName), "Error: Invalid or missing package name.");
                }

                // Execute the ADB command to launch the app
                Logger.LogInfo($"Executing ADB command: adb -s {deviceSerial} shell monkey -p {packageName} 1");
                Process.ExecuteCommand($"adb -s {deviceSerial} shell monkey -p {packageName} 1");
                Logger.LogInfo($"Application with package name {packageName} launched successfully on device {deviceSerial}.");
            }
            catch (Exception ex)
            {
                Logger.LogException($"An error occurred while attempting to launch the application on device {deviceSerial}.", ex);
                throw new Exception($"Error launching application: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of installed application packages from the specified Android device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the target Android device.</param>
        /// <returns>
        /// A string containing the list of installed application packages.
        /// </returns>
        [McpServerTool("android_list_packages")]
        [Description("Retrieves the list of installed package names from the specified Android device.")]
        public string ListPackages(string deviceSerial)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    Logger.LogError("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                Logger.LogInfo($"Attempting to list installed packages on device: {deviceSerial}");

                if (string.IsNullOrEmpty(deviceSerial))
                {
                    Logger.LogError("Device serial number is missing or invalid. Cannot proceed with listing packages.");
                    return $"Error: Invalid or missing device serial number.";
                }

                var packages = new List<string>();
                Logger.LogInfo($"Executing ADB command: adb -s {deviceSerial} shell pm list packages");

                string result = Process.ExecuteCommand($"adb -s {deviceSerial} shell pm list packages");

                Logger.LogInfo("ADB command executed successfully. Parsing package list...");

                string[] lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.StartsWith("package:"))
                    {
                        // Remove the "package:" prefix and add the package name to the list
                        string packageName = line.Replace("package:", "").Trim();
                        packages.Add(packageName);
                        Logger.LogInfo($"Package added: {packageName}");
                    }
                }

                if (packages is null || packages.Count == 0)
                {
                    Logger.LogWarning("No packages found on the device.");
                    return "No packages found on the device.";
                }

                // Format the result as a table
                Logger.LogInfo($"Found {packages.Count} package(s). Formatting the package list...");
                var packagesStr = "# Installed Packages\n\n";
                packagesStr += "| Package Name |\n";
                packagesStr += "|--------------|\n";

                foreach (var app in packages)
                {
                    packagesStr += $"| `{app}` |\n";
                }

                Logger.LogInfo("Package list formatted successfully.");
                return packagesStr;
            }
            catch (Exception ex)
            {
                Logger.LogException("An error occurred while retrieving the package list.", ex);
                return $"Error retrieving packages: {ex.Message}";
            }
        }
    }
}
