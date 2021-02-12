using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Windows.Forms;

namespace SaveRedirection
{
    class Redirector
    {
        public static void Redirect(Redirection redirection, System.Windows.Controls.TextBox ReportBox)
        {
            // Show user what's going on
            ReportBox.IsEnabled = true;
            ReportBox.Text = "Copying files to new location";
            // Copy files so that when user cancels operation the source folder will still be there
            // Use VB FileSystem to show pretty GUI when copying large files
            // TODO: Somehow keep track of which files were copied and remove them in destination folder when it errors to undo
            try
            {
                if (Directory.Exists(redirection.DestinationPath))
                    FileSystem.CopyDirectory(redirection.SourcePath, redirection.DestinationPath);
                else
                    FileSystem.MoveDirectory(redirection.SourcePath, redirection.DestinationPath);
            }
            catch
            {
                ReportBox.Text = "Error!";
                switch (MessageBox.Show($"Could not move folder, try again with overwrite enabled? This will overwrite files!", "Error detected", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Cancel:
                        return;
                    default:
                        ReportBox.Text = "Copying files to new location";
                        // Don't catch this if it errors, something is seriously wrong
                        FileSystem.CopyDirectory(redirection.SourcePath, redirection.DestinationPath, true);
                        break;
                }
            }
            // Add redirection to the list to be saved
            SettingsLoader.Instance.Settings.redirections.Add(redirection);
            // Remove original folder since copying to new location worked
            ReportBox.Text = "Removing original folder";
            if (Directory.Exists(redirection.SourcePath))
                FileSystem.DeleteDirectory(redirection.SourcePath, DeleteDirectoryOption.DeleteAllContents);
            // Create junction
            ReportBox.Text = "Linking new location";
            CreateMaps.JunctionPoint.Create(redirection.SourcePath, redirection.DestinationPath, false);
            // Hide junction
            ReportBox.Text = "Hiding link";
            File.SetAttributes(redirection.SourcePath, FileAttributes.Hidden | FileAttributes.System);
            ReportBox.Text = "Done!";
        }
        public static void Straighten(Redirection redirection)
        {
            // Delete junction
            CreateMaps.JunctionPoint.Delete(redirection.SourcePath);
            // Move folder back
            FileSystem.MoveDirectory(redirection.DestinationPath, redirection.SourcePath);
        }
    }
}
