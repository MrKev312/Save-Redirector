using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SaveRedirection
{
    internal class SettingsLoader
    {
        public bool Loaded = false;
        public SettingsTemplate Settings;

        private static readonly Lazy<SettingsLoader>
        lazy =
        new Lazy<SettingsLoader>
            (() => new SettingsLoader());

        public static SettingsLoader Instance { get { return lazy.Value; } }

        public enum Result
        {
            Success,
            NotAllValuesSet,
            PathError
        }

        public static Result LoadSettings(string PathToSettings = null)
        {
            if (string.IsNullOrWhiteSpace(PathToSettings))
                PathToSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection");

            if (File.Exists(Path.Combine(PathToSettings, "settings.json")))
            {
                Instance.Settings = JsonConvert.DeserializeObject<SettingsTemplate>(File.ReadAllText(Path.Combine(PathToSettings, "settings.json")));
            }
            else
            {
                Instance.Settings = new SettingsTemplate();
            }
            Instance.Loaded = true;
            if (string.IsNullOrWhiteSpace(Instance.Settings.DocumentsFolder) || string.IsNullOrWhiteSpace(Instance.Settings.SavedGamesFolder))
                return Result.NotAllValuesSet;
            else
                return Result.Success;
        }

        public static void SaveSettings(string PathToSettings = null)
        {
            if (string.IsNullOrWhiteSpace(PathToSettings))
                PathToSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SaveRedirection");
            string SaveFile = JsonConvert.SerializeObject(Instance.Settings, Formatting.Indented);
            Directory.CreateDirectory(PathToSettings);
            File.WriteAllText(Path.Combine(PathToSettings, "settings.json"), SaveFile);
        }
    }

    internal class SettingsTemplate
    {
        // Keep track of Settings Version in case settings drastically change
        public int SettingsVersion;
        public string DocumentsFolder, SavedGamesFolder;
        public ObservableCollection<Redirection> redirections;
        public SettingsTemplate()
        {
            // Create list when a new instance of class is created
            redirections = new ObservableCollection<Redirection>();
            // Current SettingsVersion is 1
            SettingsVersion = 1;
        }
    }
}
