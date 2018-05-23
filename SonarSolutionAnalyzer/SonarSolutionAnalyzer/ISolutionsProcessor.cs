using System.Collections.Generic;

namespace SonarSolutionAnalyzer
{
    public interface ISolutionsProcessor
    {
        IEnumerable<SolutionDescription> Process();
    }
}
