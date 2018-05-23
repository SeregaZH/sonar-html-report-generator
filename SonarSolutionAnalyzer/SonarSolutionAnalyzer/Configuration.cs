using System;
using System.Collections.Generic;

namespace SonarSolutionAnalyzer
{
    public sealed class Configuration
    {
        public Configuration()
        {
            RootPath = string.Empty;
            ExcludedPaths = new List<string>();
            IncludedPaths = new List<string>();
            SonarScannerPath = string.Empty;
            SoanarLogin = string.Empty;
            CoverageReportPath = string.Empty;
            NugetPath = string.Empty;
            BuildToolPath = string.Empty;
            DotCoverPath = string.Empty;
        }

        public IEnumerable<string> ExcludedPaths { get; set; }
        public IEnumerable<string> IncludedPaths { get; set; }

        public string RootPath { get; set; }
        public Uri SonarUrl { get; set; }
        public string SoanarLogin { get; set; }
        public string SonarScannerPath { get; set; }
        public string CoverageReportPath { get; set; }
        public string NugetPath { get; set; }
        public string BuildToolPath { get; set; }
        public string DotCoverPath { get; set; }
        public string TestExecutorPath { get; set; }
    }
}
