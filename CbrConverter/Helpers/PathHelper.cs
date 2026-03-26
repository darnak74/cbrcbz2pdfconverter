using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CbrConverter.Helpers
{
    public static class PathHelper
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        public static string BuildOutputFile(string outputDirectory, string sourceNameWithoutExtension, string extension)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory is required.", "outputDirectory");

            if (string.IsNullOrWhiteSpace(sourceNameWithoutExtension))
                throw new ArgumentException("Source name is required.", "sourceNameWithoutExtension");

            return Path.Combine(outputDirectory, sourceNameWithoutExtension + extension);
        }

        public static void OpenOrFocusDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
                return;

            if (TryFocusExistingExplorerWindow(directoryPath))
                return;

            Process.Start("explorer.exe", directoryPath);
        }

        private static bool TryFocusExistingExplorerWindow(string directoryPath)
        {
            try
            {
                var shellType = Type.GetTypeFromProgID("Shell.Application");
                if (shellType == null)
                    return false;

                dynamic shell = Activator.CreateInstance(shellType);
                dynamic windows = shell.Windows();

                foreach (var window in windows)
                {
                    try
                    {
                        string locationUrl = window.LocationURL as string;
                        if (string.IsNullOrWhiteSpace(locationUrl))
                            continue;

                        string localPath = Uri.UnescapeDataString(new Uri(locationUrl).LocalPath);
                        string normalizedWindowPath = localPath.TrimEnd('\\');
                        string normalizedTargetPath = directoryPath.TrimEnd('\\');

                        if (string.Equals(normalizedWindowPath, normalizedTargetPath, StringComparison.OrdinalIgnoreCase))
                        {
                            IntPtr hwnd = new IntPtr((int)window.HWND);
                            ShowWindowAsync(hwnd, 9);
                            SetForegroundWindow(hwnd);
                            return true;
                        }
                    }
                    catch
                    {
                        // Ignore explorer windows that are not filesystem folders.
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
