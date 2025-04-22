using System.Reflection;

namespace Telegami.Example.Advanced
{
    internal static class Utils
    {
        public static class Assets
        {
            public static string[] Cats()
            {
                var currentPath = Assembly.GetEntryAssembly()!.Location;

                var currentDirectory = Path.GetDirectoryName(currentPath)!;
                var assetsDirectory = Path.Combine(currentDirectory, "Assets");
                var catsDirectory = Path.Combine(assetsDirectory, "Cats");

                var files = Directory.GetFiles(catsDirectory, "*.jpg");
                return files;
            }
        }
    }
}
