using System;
using System.IO;
using Microsoft.Build.Construction;
using Newtonsoft.Json;

namespace SonarSolutionAnalyzer
{
    class SolutionsAnalyzer
    {
        static void Main(string[] args)
        {
            var sourcePathParamIndex = Array.FindIndex(args, x => "-s".Equals(x) || "--source-path".Equals(x));
            var destPathParamIndex = Array.FindIndex(args, x => "-d".Equals(x) || "--dest-path".Equals(x));
            var sourcePath = args.Length >= sourcePathParamIndex + 1 ? args[sourcePathParamIndex + 1] : string.Empty;
            var destBasePath = args.Length >= destPathParamIndex + 1 ? args[destPathParamIndex + 1] : string.Empty;
            var configPath = Path.Combine(sourcePath, "configuration.json");
            var destPath = Path.Combine(destBasePath, "solutions-config.json");
            Console.WriteLine("Configuration file path: {0}",configPath);
            Console.WriteLine("Output json file path: {0}", destPath);
            var configContent = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Configuration>(configContent);
            var rootPath = !Path.IsPathRooted(config.RootPath) 
                ? Path.Combine(Environment.CurrentDirectory, config.RootPath) 
                : config.RootPath;
            Console.WriteLine("Project root path: {0}", rootPath);
            var processor = new SolutionProcessor(config, new SolutionDecriptorBuilder(SolutionFile.Parse), () => Directory
                .EnumerateFiles(config.RootPath, "*.sln", SearchOption.AllDirectories));
            var json = JsonConvert.SerializeObject(processor.Process());
            File.WriteAllText(destPath, json);
        }
    }
}
