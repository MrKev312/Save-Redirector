using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SaveRedirection
{
    /// <summary>
    /// Interaction logic for EditRedirection.xaml
    /// </summary>
    public partial class EditRedirection : Window
    {
        Redirection redirection;
        public EditRedirection(Redirection redirection)
        {
            InitializeComponent();

            this.redirection = redirection;
            GameNameTextBox.Text = redirection.Name;
            RedirectionImageTextBox.Text = redirection.IconPath;
            RedirectionIcon.Source = redirection.IconImage;
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
        private static string FileDialog(string title = null)
        {
            VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
            if (title != null)
                fileDialog.Title = title;
            fileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
                return fileDialog.FileName;
            else
                return null;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            redirection.IconPath = RedirectionImageTextBox.Text;
            redirection.Name = GameNameTextBox.Text;
            SettingsLoader.SaveSettings();
        }
    }
}
