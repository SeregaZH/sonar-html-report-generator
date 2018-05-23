using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;

namespace SonarSolutionAnalyzer
{
    public sealed class SolutionDecriptorBuilder: ISolutionDescriptorBuilder
    {
        private static readonly Predicate<ProjectDescription> PathPredicate =
            x => Path.GetFileName(x.Name).EndsWith("Test") || Path.GetFileName(x.Name).EndsWith("Tests");
        private readonly Func<string, SolutionFile> _fileParser;

        public SolutionDecriptorBuilder(Func<string, SolutionFile> fileParser)
        {
            _fileParser = fileParser;
        }

        public SolutionDescription Build(string path)
        {
            var solution = _fileParser.Invoke(path);
            return new SolutionDescription
            {
                Name = Path.GetFileName(path),
                Path = path,
                Root = Path.GetDirectoryName(path),
                Projects = solution.ProjectsInOrder
                    .Select(x => new ProjectDescription
                    {
                        Name = x.ProjectName,
                        Path = x.AbsolutePath,
                        Type = ProjectType.ClassLibrary
                    })
                    .Where(x => !PathPredicate(x)),
                Tests = solution.ProjectsInOrder
                    .Select(x => new ProjectDescription
                    {
                        Name = x.ProjectName,
                        Path = x.AbsolutePath,
                        Type = ProjectType.Tests
                    })
                    .Where(x => PathPredicate(x))
            };
        }
    }
}
