using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JohnsonNet.Operation
{
    public class IOOperation
    {
        private static readonly List<string> units = new List<string>(5) { "B", "KB", "MB", "GB", "TB" }; // Not going further. Anything beyond MB is probably overkill anyway.
        private static string numberPattern = " ({0})";

        public string ToFriendlySizeString(long bytes)
        {
            var somethingMoreFriendly = TryForTheNextUnit(bytes, units[0]);
            var roundingPlaces = units[0] == somethingMoreFriendly.Item2 ? 0 : units.IndexOf(somethingMoreFriendly.Item2) - 1;
            return string.Format("{0} {1}", Math.Round(somethingMoreFriendly.Item1, roundingPlaces), somethingMoreFriendly.Item2);
        }

        private Tuple<double, string> TryForTheNextUnit(double size, string unit)
        {
            var indexOfUnit = units.IndexOf(unit);

            if (size > 1024 && indexOfUnit < units.Count - 1)
            {
                size = size / 1024;
                unit = units[indexOfUnit + 1];
                return TryForTheNextUnit(size, unit);
            }

            return new Tuple<double, string>(size, unit);
        }

        public string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public string ToFileName(string path)
        {
            foreach (string c in Path.GetInvalidFileNameChars().Select(p => p.ToString()))
                path = path.Replace(c, string.Empty);

            return path;
        }

        public static string GetResourceStream(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
