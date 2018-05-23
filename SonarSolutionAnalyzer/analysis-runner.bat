dotnet build
cd SonarSolutionAnalyzer
dotnet run -- -s ../ -d ../
cd..
powershell.exe ./sonar-runner.ps1
