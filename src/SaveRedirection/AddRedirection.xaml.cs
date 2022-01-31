using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SaveRedirection
{
    /// <summary>
    /// Interaction logic for AddGame.xaml
    /// </summary>
    public partial class AddRedirection : Window
    {
        public AddRedirection()
        {
            InitializeComponent();

            DocumentTextBox.Text = SettingsLoader.Instance.Settings.DocumentsFolder;
            try
            {
                SaveGamesTextBox.Text =
                    Path.Combine(SettingsLoader.Instance.Settings.SavedGamesFolder,
                    // Unescape returned path
                    Uri.UnescapeDataString(
                    // Makes a Uri from the home folder
                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                    // And then returns the relative Uri to the home folder
                    .MakeRelativeUri(new Uri(DocumentTextBox.Text)).OriginalString)
                    // Convert / to \
                    .Replace('/', '\\').Remove(0, new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name.Length + 1));
            }
            catch
            {
                SaveGamesTextBox.Text = SettingsLoader.Instance.Settings.SavedGamesFolder;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if user entered values (should always have a value but just in case)
            if (string.IsNullOrWhiteSpace(DocumentTextBox.Text) || string.IsNullOrWhiteSpace(SaveGamesTextBox.Text))
            {
                switch (MessageBox.Show($"You haven't entered all paths, without these the program cannot function. Do you want to close the program or try again", "Missing settings", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        System.Windows.Application.Current.Shutdown();
                        break;
                    default:
                        return;
                }
            }
        }

        private void DocumentButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentTextBox.Text = BrowserDialog() ?? DocumentTextBox.Text;
            try
            {
                string RelativePath =
                    // Unescape returned path
                    Uri.UnescapeDataString(
                    // Makes a Uri from the home folder
                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                    // And then returns the relative Uri to the home folder
                    .MakeRelativeUri(new Uri(DocumentTextBox.Text)).OriginalString)
                    // Convert / to \
                    .Replace('/', '\\');
                if (!RelativePath.StartsWith(".."))
                    // Set text and remove username
                    SaveGamesTextBox.Text = Path.Combine(SettingsLoader.Instance.Settings.SavedGamesFolder, RelativePath.Remove(0, new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Name.Length + 1));
            }
            catch { }
        }

        private void SaveGamesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveGamesTextBox.Text = BrowserDialog() ?? SaveGamesTextBox.Text;
        }

        private static string BrowserDialog()
        {
            VistaFolderBrowserDialog browserDialog = new();
            browserDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(browserDialog.SelectedPath))
                return browserDialog.SelectedPath;
            else
                return null;
        }

        private static string FileDialog(string title = null)
        {
            VistaOpenFileDialog fileDialog = new();
            if (title != null)
                fileDialog.Title = title;
            fileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
                return fileDialog.FileName;
            else
                return null;
        }

        private void GameImageButton_Click(object sender, RoutedEventArgs e)
        {
            RedirectionImageTextBox.Text = FileDialog("Select Icon");
            if (!File.Exists(RedirectionImageTextBox.Text))
            {
                RedirectionImageTextBox.Text = string.Empty;
                return;
            }
            try
            {
                RedirectionIcon.Source = new BitmapImage(new Uri(RedirectionImageTextBox.Text));
            }
            catch
            {
                RedirectionIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(
            System.Drawing.Icon.ExtractAssociatedIcon(RedirectionImageTextBox.Text).ToBitmap().GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {

            #region CheckFolder
            // Check if user entered values (should always have a value but just in case)
            if (string.IsNullOrWhiteSpace(DocumentTextBox.Text) || string.IsNullOrWhiteSpace(SaveGamesTextBox.Text))
            {
                switch (MessageBox.Show($"You haven't entered the paths, without these the program cannot function. Do you want to cancel or try again?", "Missing paths", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        Close();
                        break;
                    default:
                        return;
                }
            }
            // Check if user has selected a special folder, and if so warn them and give directions on how to move special folders through Windows's settings
            bool ShouldShowWarning = false;
            DirectoryInfo directoryInfo = new(DocumentTextBox.Text);
            foreach (Environment.SpecialFolder suit in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                if (directoryInfo.FullName == Environment.GetFolderPath(suit))
                {
                    ShouldShowWarning = true;
                    break;
                }
            }
            if (ShouldShowWarning)
            {
                switch (MessageBox.Show($"You're about to redirect a \"special\" folder, these are important Windows folders and it's recommended to change them through the system settings, only continue if you know what you're doing.", "Special folder detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        return;
                    default:
                        break;
                }
            }
            // Check if folder is inside of the Windows directory
            if (DocumentTextBox.Text.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows), StringComparison.OrdinalIgnoreCase))
            {
                switch (MessageBox.Show($"You're about to redirect a system folder, these are important Windows folders and it's recommended to not move them, only continue if you know what you're doing. This program is not to blame if your system breaks!", "Windows folder detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        return;
                    default:
                        break;
                }
            }
            #endregion
            Redirection redirection = new()
            {
                SourcePath = DocumentTextBox.Text,
                DestinationPath = SaveGamesTextBox.Text,
                Name = GameNameTextBox.Text,
                IconPath = RedirectionImageTextBox.Text
            };
            Redirector.Redirect(redirection, StatusReport);
            SettingsLoader.SaveSettings();
            MessageBox.Show("Success!", "Redirection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
