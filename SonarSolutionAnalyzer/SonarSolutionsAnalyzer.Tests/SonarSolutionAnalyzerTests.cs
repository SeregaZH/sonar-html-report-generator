using System;
using System.IO;
using Newtonsoft.Json;
using SonarSolutionAnalyzer;
using Xunit;

namespace SonarSolutionsAnalyzer.Tests
{
    public class SonarSolutionAnalyzerTests
    {
        private const string TestPath = "C:\\";
        private readonly Uri _sonarUrl = new Uri("http://localhost:9000");

        [Fact(DisplayName = "Config should be serialized correctly")]
        public void ConfigShouldBeSirializedCorrectly()
        {
            var configContent = File.ReadAllText("configuration.json");
            var config = JsonConvert.DeserializeObject<Configuration>(configContent);
            Assert.Collection(config.ExcludedPaths, s => Assert.Equal(s, TestPath));
            Assert.Empty(config.IncludedPaths);
            Assert.Equal(config.SonarScannerPath, TestPath);
            Assert.Equal(config.SoanarLogin, TestPath);
            Assert.Equal(config.CoverageReportPath, TestPath);
            Assert.NotNull(config.SonarUrl);
            Assert.Equal(config.SonarUrl.AbsolutePath, _sonarUrl.AbsolutePath);
            Assert.Equal(config.NugetPath, string.Empty);
        }
    }
}
