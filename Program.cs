using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LOCCounter
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine
            (
                "Format: <paths>; <extensions>; <recursive>; <exclude-extension>\n" +
                "paths: Pipe-separated list of targeted paths. Place \"?\" in front to exclude.\n" +
                "extensions: Comma-separated list of targeted file extensions. Leave blank for all extensions.\n" +
                "recursive: \"true\" or \"false\". Whether to recursively search for files in the specified paths.\n" +
                "exclude-extension: \"true\" or \"false\". Whether to exclude the extensions specified in \"extensions\" and search for all other extensions.\n" +
                "Example: C:\\Code | D:\\Programming | ?D:\\Programming\\Secret; .cs, .cpp, .c; true; false\n"
            );
            while (true)
            {
                string command = Console.ReadLine();
                string errorText = CheckError(command);
                if (!string.IsNullOrEmpty(errorText))
                {
                    Console.WriteLine(errorText);
                    continue;
                }
                string[] arguments = command.Split(';', StringSplitOptions.None);
                string[] paths = arguments[0].Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < paths.Length; i++)
                {
                    paths[i] = paths[i].Trim();
                }
                string[] extensions = arguments[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < extensions.Length; i++)
                {
                    extensions[i] = extensions[i].Trim();
                }
                if (Array.TrueForAll(extensions, extension => string.IsNullOrWhiteSpace(extension)))
                {
                    extensions = new string[1] { "" };
                }
                bool isRecursive;
                try
                {
                    isRecursive = bool.Parse(arguments[2]);
                }
                catch when (string.IsNullOrWhiteSpace(arguments[2]))
                {
                    isRecursive = true;
                }
                bool isExcludeExtension;
                try
                {
                    isExcludeExtension = bool.Parse(arguments[3]);
                }
                catch when (string.IsNullOrWhiteSpace(arguments[3]))
                {
                    isExcludeExtension = false;
                }
                List<string> includedPaths = new List<string>();
                List<string> excludedPaths = new List<string>();
                string LOCs = "";
                int totalLines = 0;
                foreach (string path in paths)
                {
                    if (path.StartsWith('?'))
                    {
                        excludedPaths.Add(path.Remove(0, 1));
                    }
                    else
                    {
                        includedPaths.Add(path);
                    }
                }
                foreach (string path in includedPaths)
                {
                    List<string> pathsToSearch = new List<string>();
                    if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    {
                        ScanDirectory(path, excludedPaths, extensions, isExcludeExtension, ref LOCs, ref totalLines, isRecursive);
                    }
                    else
                    {
                        if (!excludedPaths.Contains(path))
                        {
                            ScanFile(path, excludedPaths, extensions, isExcludeExtension, ref LOCs, ref totalLines);
                        }
                    }
                }
                Console.WriteLine($"{LOCs}\nTotal: {totalLines} lines\n");
            }
        }

        private static string CheckError(string command)
        {
            string errorText = "";
            string[] arguments = command.Split(';', StringSplitOptions.None);
            string[] paths = arguments[0].Split('|');
            if (arguments.Length != 4)
            {
                errorText += $"Incorrect argument count({arguments.Length} provided, should be 4)\n";
                return errorText;
            }
            if (paths.Length == 0 || Array.TrueForAll(paths, path => string.IsNullOrWhiteSpace(path)))
            {
                errorText += "paths: No paths provided\n";
            }
            try
            {
                bool.Parse(arguments[2]);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                {
                    errorText += "recursive: Argument is null\n";
                }
                if (e is FormatException)
                {
                    errorText += "recursive: Wrong format(must be \"true\" or \"false\")\n";
                }
            }
            try
            {
                bool.Parse(arguments[3]);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                {
                    errorText += "exclude-extension: Argument is null\n";
                }
                if (e is FormatException)
                {
                    errorText += "exclude-extension: Wrong format(must be \"true\" or \"false\")\n";
                }
            }
            foreach (string path in paths)
            {
                string trimed;
                if (path.StartsWith('?'))
                {
                    trimed = path.Remove(0, 1);
                }
                else
                {
                    trimed = path;
                }
                if (!File.Exists(trimed) && !Directory.Exists(trimed) && !string.IsNullOrWhiteSpace(trimed) && !trimed.StartsWith('?'))
                {
                    errorText += $"{trimed}: No such file or directory\n";
                }
            }
            return errorText;
        }

        private static void ScanDirectory(string path, List<string> excludedPaths, string[] extensions, bool isExcludeExtension, ref string LOCs, ref int totalLines, bool isRecursive)
        {
            if (excludedPaths.ContainsIgnoreCase(path))
            {
                return;
            }
            excludedPaths.Add(path);
            foreach (string file in Directory.EnumerateFiles(path))
            {
                ScanFile(file, excludedPaths, extensions, isExcludeExtension, ref LOCs, ref totalLines);
                excludedPaths.Add(file);
            }
            if (isRecursive)
            {
                foreach (string directory in Directory.EnumerateDirectories(path))
                {
                    ScanDirectory(directory, excludedPaths, extensions, isExcludeExtension, ref LOCs, ref totalLines, isRecursive);
                }
            }
        }

        private static void ScanFile(string file, List<string> excludedPaths, string[] extensions, bool isExcludeExtension, ref string LOCs, ref int totalLines)
        {
            if (!excludedPaths.ContainsIgnoreCase(file) && extensions.Any(s => file.EndsWith(s)) == !isExcludeExtension)
            {
                int lines = File.ReadLines(file).Count();
                LOCs += $"{file}: {lines} lines\n";
                totalLines += lines;
            }
        }
    }
}
