using System.Text.RegularExpressions;

namespace AzureBuildsBrowser.Utils
{
    class PathGlob
    {
        private static readonly char[] Wildcards = new[] { '*' };

        public static Regex Create(string path) => new Regex("^" + Regex.Escape(path).Replace("\\*", "[^/]*") + "$", RegexOptions.IgnoreCase);

        public static bool HasWildcards(string path) => path.IndexOfAny(Wildcards) >= 0;
    }
}