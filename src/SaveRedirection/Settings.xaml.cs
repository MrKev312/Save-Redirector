using Ookii.Dialogs.Wpf;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SaveRedirection
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        // Use Win32API call to figue out where the Saved Games folder is located (why is there still no easy way for that Microsoft?)
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);
        public Settings()
        {
            InitializeComponent();
            // Set Documents folder, if settings has null then get it from the environment
            DocumentTextBox.Text = SettingsLoader.Instance.Settings.DocumentsFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Set Saved Games folder, manually check if setting has null
            if (!string.IsNullOrWhiteSpace(SettingsLoader.Instance.Settings.SavedGamesFolder))
                // It isn't so load it from the settings
                SaveGamesTextBox.Text = SettingsLoader.Instance.Settings.SavedGamesFolder;
            else
            {
                // Settings doesn't know so use this witchcraft to figure out where it is
                // Call to get the Saved Games folder (if this was a special folder this could've been done in one line)
                if (SHGetKnownFolderPath(new Guid("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}"), 0, IntPtr.Zero, out string Path) != -1)
                    SaveGamesTextBox.Text = Path;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if user entered values (should always have a value but just in case)
            if (string.IsNullOrWhiteSpace(DocumentTextBox.Text) || string.IsNullOrWhiteSpace(SaveGamesTextBox.Text))
            {
                switch (MessageBox.Show($"You haven't entered all paths, without these the program cannot function. Do you want to close the program or try again?", "Missing settings", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        System.Windows.Application.Current.Shutdown();
                        break;
                    default:
                        return;
                }
            }
            // Update settings with new values and save
            SettingsLoader.Instance.Settings.DocumentsFolder = DocumentTextBox.Text;
            SettingsLoader.Instance.Settings.SavedGamesFolder = SaveGamesTextBox.Text;
            SettingsLoader.SaveSettings();
        }

        private void DocumentButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentTextBox.Text = BrowserDialog() ?? DocumentTextBox.Text;
        }

        private void SaveGamesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveGamesTextBox.Text = BrowserDialog() ?? SaveGamesTextBox.Text;
        }

        private static string BrowserDialog()
        {
            VistaFolderBrowserDialog browserDialog = new VistaFolderBrowserDialog();
            browserDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(browserDialog.SelectedPath))
                return browserDialog.SelectedPath;
            else
                return null;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
