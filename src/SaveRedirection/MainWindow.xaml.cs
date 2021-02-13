using SaveRedirection.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SaveRedirection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ItemViewModel();
#if DEBUG
            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection")))
            {
                switch (MessageBox.Show($"Debug running, reset app data for testing?\nThis will delete all files inside { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection") }.", "Debug install", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection"), true);
                        break;
                    default:
                        break;
                }
            }
#endif
            if ((int)SettingsLoader.LoadSettings(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection")) > 0)
            {
                Settings settings = new Settings
                {
                    Title = "First time opening"
                };
                settings.ShowDialog();
                SettingsLoader.SaveSettings();
            }
            RedirectionList.ItemsSource = SettingsLoader.Instance.Settings.redirections;
        }

        private void DockPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
            SettingsLoader.SaveSettings();
        }

        private void NewRedirectionButton_Click(object sender, RoutedEventArgs e)
        {
            AddRedirection addGame = new AddRedirection();
            addGame.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsLoader.SaveSettings();
        }
    }
}