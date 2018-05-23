using System.Collections.Generic;

namespace SonarSolutionAnalyzer
{
    public sealed class SolutionDescription: ItemDescription
    {
        public IEnumerable<ProjectDescription> Projects { get; set; }
        public IEnumerable<ProjectDescription> Tests { get; set; }
        public string Root { get; set; }
    }
}
