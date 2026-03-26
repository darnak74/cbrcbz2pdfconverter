using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;
using CbrConverter.Helpers;

namespace CbrConverter
{
    public partial class MainWindow : Window
    {
        private Extract _extract;
        private bool _isRunning;
        private string _sourcePathForCurrentRun;

        public MainWindow()
        {
            InitializeComponent();
            TxtDest.Text = "Identique à la source";
            SetStatus("Prêt", "#107C10");
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            BrowseSource();
        }

        private void BorderSource_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BrowseSource();
        }

        private void BrowseSource()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Sélectionner un fichier source",
                Filter = "Archives|*.cbr;*.cbz;*.rar;*.zip|Tous les fichiers|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (fileDialog.ShowDialog() == true)
            {
                TxtSource.Text = fileDialog.FileName;
                SetStatus("Fichier sélectionné.", "#107C10");
                return;
            }

            using (var folderDialog = new WinForms.FolderBrowserDialog())
            {
                folderDialog.Description = "Sélectionner un dossier source";
                if (folderDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    TxtSource.Text = folderDialog.SelectedPath;
                    SetStatus("Dossier sélectionné.", "#107C10");
                }
            }
        }

        private void BtnBrowseDest_Click(object sender, RoutedEventArgs e)
        {
            BrowseDestination();
        }

        private void BorderDest_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BrowseDestination();
        }

        private void BrowseDestination()
        {
            using (var folderDialog = new WinForms.FolderBrowserDialog())
            {
                folderDialog.Description = "Choisir le dossier de destination";
                if (folderDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    TxtDest.Text = folderDialog.SelectedPath;
                    TxtDest.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));
                }
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                DataAccess.Instance.g_Processing = false;
                SetStatus("Annulation…", "#BA7517");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtSource.Text))
            {
                MessageBox.Show("Veuillez sélectionner un fichier ou dossier source.", "Source manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!File.Exists(TxtSource.Text) && !Directory.Exists(TxtSource.Text))
            {
                MessageBox.Show("La source sélectionnée n'existe pas.", "Source invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            StartConversion();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                DataAccess.Instance.g_Processing = false;
            }

            ProgressFile.Value = 0;
            ProgressTotal.Value = 0;
            TxtProgressFile.Text = "—";
            TxtProgressTotal.Text = "—";
            TxtStatus.Text = "Prêt";
            BtnReset.IsEnabled = true;
            BtnStart.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4));
            TxtBtnStart.Text = "Démarrer";
            IconStart.Data = Geometry.Parse("M3,2 L11,6.5 L3,11 Z");
        }

        private void StartConversion()
        {
            _isRunning = true;
            BtnReset.IsEnabled = false;
            BtnStart.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x5A, 0x9E));
            TxtBtnStart.Text = "Annuler";
            IconStart.Data = Geometry.Parse("M3,3 L9,3 L9,9 L3,9 Z");
            SetStatus("Conversion en cours…", "#0078D4");

            string sourcePath = TxtSource.Text.Trim();
            string destination = ResolveDestination(sourcePath, TxtDest.Text.Trim());
            _sourcePathForCurrentRun = sourcePath;

            DataAccess.Instance.g_WorkingDir = sourcePath;
            DataAccess.Instance.g_Output_dir = destination;
            DataAccess.Instance.g_Processing = true;
            DataAccess.Instance.g_curProgress = 0;
            DataAccess.Instance.g_totProgress = 0;

            _extract = new Extract();
            _extract.evnt_UpdateCurBar += OnUpdateCurrentBar;
            _extract.evnt_UpdatTotBar += OnUpdateTotalBar;
            _extract.evnt_UpdateFileName += OnUpdateFileName;
            _extract.evnt_ErrorNotify += OnError;

            _extract.BeginExtraction(true, ChkCompress.IsChecked == true, false, ChkFusion.IsChecked == true, true);
        }

        private string ResolveDestination(string sourcePath, string userDestination)
        {
            if (!string.IsNullOrWhiteSpace(userDestination) && userDestination != "Identique à la source")
            {
                return userDestination;
            }

            if (File.Exists(sourcePath))
                return Path.GetDirectoryName(sourcePath);

            return sourcePath;
        }

        private void OnUpdateCurrentBar()
        {
            Dispatcher.Invoke(() =>
            {
                var value = Math.Max(0, Math.Min(100, DataAccess.Instance.g_curProgress));
                ProgressFile.Value = value;
                TxtProgressFile.Text = value.ToString("0") + "%";
            });
        }

        private void OnUpdateTotalBar(Extract _, EventArgs __)
        {
            Dispatcher.Invoke(() =>
            {
                var value = Math.Max(0, Math.Min(100, DataAccess.Instance.g_totProgress));
                ProgressTotal.Value = value;
                TxtProgressTotal.Text = value.ToString("0") + "%";
            });
        }

        private void OnUpdateFileName(Extract _, EventArgs __)
        {
            Dispatcher.Invoke(() =>
            {
                var current = DataAccess.Instance.g_WorkingFile;
                if (string.IsNullOrWhiteSpace(current))
                {
                    CompleteConversion();
                    return;
                }

                TxtStatus.Text = Path.GetFileName(current);
            });
        }

        private void OnError(Extract _, string message)
        {
            Dispatcher.Invoke(() =>
            {
                SetStatus("Erreur: " + message, "#C42B1C");
            });
        }

        private void CompleteConversion()
        {
            _isRunning = false;
            BtnReset.IsEnabled = true;
            BtnStart.Background = new SolidColorBrush(Color.FromRgb(0x10, 0x7C, 0x10));
            TxtBtnStart.Text = "Terminé";
            IconStart.Data = Geometry.Parse("M2,6.5 L5.5,10 L11,4");
            SetStatus("Conversion terminée.", "#107C10");

            string sourceFolder = File.Exists(_sourcePathForCurrentRun)
                ? Path.GetDirectoryName(_sourcePathForCurrentRun)
                : _sourcePathForCurrentRun;

            if (!string.IsNullOrWhiteSpace(sourceFolder) && Directory.Exists(sourceFolder))
            {
                PathHelper.OpenOrFocusDirectory(sourceFolder);
            }
        }

        private void SetStatus(string message, string hexColor)
        {
            TxtStatus.Text = message;
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            StatusDot.Fill = new SolidColorBrush(color);
        }
    }
}
