using System;
using System.IO;

namespace Yomitan.Constants
{
    public static class YomitanFilepaths
    {
        public static readonly string YomitanDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Yomitan");
        public static readonly string DictionaryDirectoryPath = Path.Combine(YomitanDirectoryPath, "Dictionaries");
        public static readonly string DeinflectRulesFilePath = Path.Combine(YomitanDirectoryPath, "deinflect.json");
        public static readonly string TagColorsFilePath = Path.Combine(YomitanDirectoryPath, "tag_colors.json");
    }
}
