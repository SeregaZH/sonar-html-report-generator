using System;
using System.IO;
using Microsoft.Build.Construction;
using SonarSolutionAnalyzer;
using Xunit;

namespace SonarSolutionsAnalyzer.Tests
{
    public class SolutionDescriptorBuilderTests
    {
        [Fact(DisplayName = "Should parse solution file without tests")]
        public void ShouldParseSolutionFileWithoutTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "test.sln");
            var testFile = SolutionFile.Parse(path);
            var target = new SolutionDecriptorBuilder((s) => testFile);
            var result = target.Build(path);
            Assert.Collection(result.Projects,
                p =>
                {
                    Assert.Equal(ProjectType.ClassLibrary, p.Type);
                    Assert.Equal("Entity", p.Name);
                },
                p =>
                {
                    Assert.Equal(ProjectType.ClassLibrary, p.Type);
                    Assert.Equal("API", p.Name);
                });
            Assert.Empty(result.Tests);
            Assert.Equal("test.sln", result.Name);
            Assert.Equal(Environment.CurrentDirectory, result.Root);
            Assert.Equal(path, result.Path);
        }

        [Fact(DisplayName = "Should parse solution file with tests")]
        public void ShouldParseSolutionFileWithTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "test-wt.sln");
            var testFile = SolutionFile.Parse(path);
            var target = new SolutionDecriptorBuilder(s => testFile);
            var result = target.Build(path);
            Assert.Collection(result.Tests,
                p =>
                {
                    Assert.Equal(ProjectType.Tests, p.Type);
                    Assert.Equal("Entity.Tests", p.Name);
                },
                p =>
                {
                    Assert.Equal(ProjectType.Tests, p.Type);
                    Assert.Equal("API.Test", p.Name);
                });
            Assert.Equal("test-wt.sln", result.Name);
        }
    }
}