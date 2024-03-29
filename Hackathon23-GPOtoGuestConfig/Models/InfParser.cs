﻿namespace Hackathon23_GPOtoGuestConfig.Models
{
    public class IniReader
    {
        private Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);

        public IniReader(string fileName, string file, bool fromFile)
        {
            string txt;
            if (fromFile)
            {
                txt = File.ReadAllText(fileName);
            }
            else
            {
                txt = file;
            }

            Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            ini[""] = currentSection;

            foreach (var line in txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                   .Where(t => !string.IsNullOrWhiteSpace(t))
                                   .Select(t => t.Trim()))
            {
                if (line.StartsWith(";"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    ini[line.Substring(1, line.LastIndexOf("]") - 1)] = currentSection;
                    continue;
                }

                //the lines may have extra spaces, so remove them all
                var thisLine = line.Replace(" ", string.Empty);
                var idx = thisLine.IndexOf("=");
                if (idx == -1)
                    currentSection[thisLine] = "";
                else
                    currentSection[thisLine.Substring(0, idx)] = thisLine.Substring(idx + 1);
            }
        }

        public string GetValue(string key)
        {
            return GetValue(key, "", "");
        }

        public string GetValue(string key, string section)
        {
            return GetValue(key, section, "");
        }

        public string GetValue(string key, string section, string @default)
        {
            if (!ini.ContainsKey(section))
                return @default;     

            if (!ini[section].ContainsKey(key))
                return @default;

            return ini[section][key];
        }

        public string[] GetKeys(string section)
        {
            if (!ini.ContainsKey(section))
                return new string[0];

            return ini[section].Keys.ToArray();
        }

        public string[] GetSections()
        {
            return ini.Keys.Where(t => t != "")
                .Where(t => t != "Unicode")
                .Where(t => t != "Version")
                .ToArray();
        }
    }
}