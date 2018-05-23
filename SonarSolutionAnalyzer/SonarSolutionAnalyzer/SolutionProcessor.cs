using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SonarSolutionAnalyzer
{
    public sealed class SolutionProcessor: ISolutionsProcessor
    {
        private readonly Configuration _configuration;
        private readonly ISolutionDescriptorBuilder _descriptorBuilder;
        private readonly Func<IEnumerable<string>> _solutionListProvider;

        public SolutionProcessor(
            Configuration configuration, 
            ISolutionDescriptorBuilder descriptorBuilder,
            Func<IEnumerable<string>> solutionListProvider)
        {
            _configuration = configuration;
            _descriptorBuilder = descriptorBuilder;
            _solutionListProvider = solutionListProvider;
        }

        public IEnumerable<SolutionDescription> Process()
        {
            var solutionPaths = _solutionListProvider.Invoke();
            var includedPaths = NormalizePath(_configuration.IncludedPaths);
            var excludedPaths = NormalizePath(_configuration.ExcludedPaths);
            
            if (includedPaths.Any())
            {
                solutionPaths = solutionPaths.Where(x => includedPaths.Any(y => x.StartsWith(y + @"\")));
            }

            if (excludedPaths.Any())
            {
                solutionPaths = solutionPaths.Where(x => !excludedPaths.Any(y => x.StartsWith(y + @"\")));
            }

            return solutionPaths.Select(_descriptorBuilder.Build);
        }

        private List<string> NormalizePath(IEnumerable<string> paths)
        {
            var basePath = _configuration.RootPath;
            return paths.Select(x => !x.StartsWith(basePath + @"\") ? Path.Combine(basePath, x) : x).ToList();
        }
    }
}
