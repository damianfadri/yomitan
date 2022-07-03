using System;
using System.IO;

namespace Yomitan.Constants
{
    public static class YomitanFilepaths
    {
        public static readonly string YomitanDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Yomitan");
        public static readonly string ConfigurationDirectoryPath = Path.Combine(YomitanDirectoryPath, "Configurations");
        public static readonly string DictionaryDirectoryPath = Path.Combine(YomitanDirectoryPath, "Dictionaries");
        public static readonly string DeinflectRulesFilePath = Path.Combine(ConfigurationDirectoryPath, "deinflect.json");
        public static readonly string TagColorsFilePath = Path.Combine(ConfigurationDirectoryPath, "tag_colors.json");
        public static readonly string UserPreferencesFilePath = Path.Combine(ConfigurationDirectoryPath, "preferences.json");

        public static readonly string DeinflectRulesDefaultFilePath = "Yomitan.Assets.deinflect.json";
        public static readonly string TagColorsDefaultFilePath = "Yomitan.Assets.tag_colors.json";
    }
}
