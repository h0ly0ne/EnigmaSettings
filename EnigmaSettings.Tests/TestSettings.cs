using System;
using System.IO;
using Krkadoni.EnigmaSettings;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings.Tests
{
    /// <summary>Shared helpers for building a SettingsIO and isolated temp directories.</summary>
    public static class TestSettings
    {
        public static ISettingsIO NewIO()
        {
            return new SettingsIO(new InstanceFactory());
        }

        /// <summary>Absolute path to a fixture folder under the test output directory.</summary>
        public static string FixtureDir(string version)
        {
            return Path.Combine(AppContext.BaseDirectory, "Fixtures", version);
        }

        /// <summary>Copies a fixture folder to a fresh temp directory and returns its path.</summary>
        public static string CopyFixtureToTemp(string version)
        {
            string src = FixtureDir(version);
            string dst = Path.Combine(Path.GetTempPath(), "es_tests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dst);
            foreach (var f in Directory.GetFiles(src))
                File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), true);
            return dst;
        }
    }
}
