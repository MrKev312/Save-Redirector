using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SaveRedirection
{
    public partial class Redirection
    {
        private string name;
        private string sourcePath;
        private string destinationPath;
        private string iconPath;

        public string SourcePath { get => sourcePath; set { sourcePath = value; SendUpdate(); } }
        public string DestinationPath { get => destinationPath; set { destinationPath = value; SendUpdate(); } }
        public string IconPath { get => iconPath; set { iconPath = value; SendUpdate(); SendUpdate(nameof(IconImage)); } }
        [JsonIgnore]
        public ImageSource IconImage { get => GetIconBitmap(); }
        public string Name { get => name; set { name = value; SendUpdate(); } }

        private ImageSource GetIconBitmap()
        {
            if (string.IsNullOrWhiteSpace(iconPath))
                return null;
            if (!File.Exists(iconPath))
                return Imaging.CreateBitmapSourceFromHBitmap(
            SystemIcons.Warning.ToBitmap().GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
            try
            {
                return new BitmapImage(new Uri(iconPath));
            }
            catch
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
            Icon.ExtractAssociatedIcon(iconPath).ToBitmap().GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }

    public partial class Redirection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SendUpdate([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
