using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using SonarSolutionAnalyzer;
using Xunit;

namespace SonarSolutionsAnalyzer.Tests
{
    public sealed class SolutionProcessorTests
    {
        private const string BasePath = @"C:\repos";

        [Fact(DisplayName = "Process should produce list of solution files without excludes")]
        public void ProcessShouldProduceListOfSolutionFilesWithoutExcludes()
        {
            var testData = new List<(string path, SolutionDescription desc)>
            {
                (Path.Combine(BasePath, @"projects-finder\SonarSolutionsAnalyzer.Tests\test.sln"),
                 new SolutionDescription { Name = "test" }),
                (Path.Combine(BasePath, @"projects-finder\SonarSolutionsAnalyzer.Tests\test-wt.sln"),
                new SolutionDescription { Name = "test-wt" })
            };
            var solutionBuilderMock = new Mock<ISolutionDescriptorBuilder>();
            foreach (var (path, desc) in testData)
            {
                solutionBuilderMock.Setup(x => x.Build(path)).Returns(desc).Verifiable();
            }

            var target = new SolutionProcessor(new Configuration { RootPath = BasePath }, solutionBuilderMock.Object, () => testData.Select(x => x.path));
            var result = target.Process();
            Assert.Collection(
                result, 
                d => Assert.Equal(d.Name, testData[0].desc.Name),
                d => Assert.Equal(d.Name, testData[1].desc.Name)
            );
            solutionBuilderMock.Verify();
        }

        [Fact(DisplayName = "Process should produce list of solution files with excludes")]
        public void ProcessShouldProduceListOfSolutionFilesWithExcludes()
        {
            var testData = new List<(string path, SolutionDescription desc)>
            {
                (Path.Combine(BasePath, @"projects-finder\SonarSolutionsAnalyzer.Tests\test.sln"),
                    new SolutionDescription { Name = "test" }),
                (Path.Combine(BasePath, @"projects-finder\SonarSolutionsAnalyzer.Tests\test-wt.sln"),
                    new SolutionDescription { Name = "test-wt" }),
                (Path.Combine(BasePath, @"exclude\SonarSolutionsAnalyzer.Tests\test1.sln"),
                    new SolutionDescription { Name = "test1" }),
                (Path.Combine(BasePath, @"exclude2\exclude3\test2.sln"),
                    new SolutionDescription { Name = "test2" })
            };
            var solutionBuilderMock = new Mock<ISolutionDescriptorBuilder>();
            foreach (var (path, desc) in testData)
            {
                solutionBuilderMock.Setup(x => x.Build(path)).Returns(desc);
            }

            var target = new SolutionProcessor(
                new Configuration
                {
                    RootPath = BasePath,
                    ExcludedPaths = new List<string>
                        {
                            Path.Combine(BasePath, @"exclude"),
                            @"exclude2\exclude3"
                        }
                    },
                solutionBuilderMock.Object,
                () => testData.Select(x => x.path)
            );

            var result = target.Process().ToList();
            solutionBuilderMock.Verify(x => x.Build(testData[0].path), Times.Once);
            solutionBuilderMock.Verify(x => x.Build(testData[1].path), Times.Once);
            solutionBuilderMock.Verify(x => x.Build(testData[2].path), Times.Never);
            solutionBuilderMock.Verify(x => x.Build(testData[3].path), Times.Never);
            Assert.Collection(
                result,
                d => Assert.Equal(d.Name, testData[0].desc.Name),
                d => Assert.Equal(d.Name, testData[1].desc.Name)
            );
        }

        [Fact(DisplayName = "Process should produce list of solution files with includes")]
        public void ProcessShouldProduceListOfSolutionFilesWithIncludes()
        {
            var testData = new List<(string path, SolutionDescription desc)>
            {
                (Path.Combine(BasePath, @"projects-finder\SonarSolutionsAnalyzer.Tests\test.sln"),
                    new SolutionDescription { Name = "test" }),
                (Path.Combine(BasePath, @"exclude1\exlude2\SonarSolutionsAnalyzer.Tests\test-wt.sln"),
                    new SolutionDescription { Name = "test-wt" }),
                (Path.Combine(BasePath, @"exclude\SonarSolutionsAnalyzer.Tests\test1.sln"),
                    new SolutionDescription { Name = "test1" }),
                (Path.Combine(BasePath, @"include2\test2.sln"),
                    new SolutionDescription { Name = "test2" })
            };
            var solutionBuilderMock = new Mock<ISolutionDescriptorBuilder>();
            foreach (var (path, desc) in testData)
            {
                solutionBuilderMock.Setup(x => x.Build(path)).Returns(desc);
            }

            var target = new SolutionProcessor(
                new Configuration
                {
                    RootPath = BasePath,
                    IncludedPaths = new List<string>
                        {
                            Path.Combine(BasePath, @"projects-finder"),
                            @"include2"
                        }
                },
                solutionBuilderMock.Object,
                () => testData.Select(x => x.path)
            );

            var result = target.Process().ToList();
            solutionBuilderMock.Verify(x => x.Build(testData[0].path), Times.Once);
            solutionBuilderMock.Verify(x => x.Build(testData[1].path), Times.Never);
            solutionBuilderMock.Verify(x => x.Build(testData[2].path), Times.Never);
            solutionBuilderMock.Verify(x => x.Build(testData[3].path), Times.Once);
            Assert.Collection(
                result,
                d => Assert.Equal(d.Name, testData[0].desc.Name),
                d => Assert.Equal(d.Name, testData[3].desc.Name)
            );
        }
    }
}
