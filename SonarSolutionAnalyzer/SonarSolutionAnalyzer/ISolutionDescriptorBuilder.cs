namespace SonarSolutionAnalyzer
{
    public interface ISolutionDescriptorBuilder
    {
        SolutionDescription Build(string path);
    }
}
