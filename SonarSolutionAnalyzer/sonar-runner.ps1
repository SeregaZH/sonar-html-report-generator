function IfNullOrEmpty($a, $b) { if ($a -eq $null -or $a -eq "") { $b } else { $a } }

$config=Get-Content -Path .\configuration.json -Raw | ConvertFrom-Json
$solutionsConfig=Get-Content -Path .\solutions-config.json -Raw | ConvertFrom-Json

$login= $config.SoanarLogin
$url= IfNullOrEmpty $config.SonarUrl "http://localhost:9000"
$base= IfNullOrEmpty $config.BasePath (Get-Item -Path ".\").FullName
$sonarPath= IfNullOrEmpty $config.SonarScannerPath "SonarQube.Scanner.MSBuild.exe"
$nugetPath= IfNullOrEmpty $config.NugetPath "nuget"
$msBuildPath= IfNullOrEmpty $config.BuildToolPath "MsBuild.exe"
$dotCoverPath= IfNullOrEmpty $config.DotCoverPath "dotCover"
$vsTestPath=IfNullOrEmpty $config.TestExecutorPath $env:VSINSTALLDIR\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe

for ($i=0; $i -lt $solutionsConfig.Length; $i++) {
    $item = $solutionsConfig[$i]
    $path = $item.Path
    $rootPath = $item.Root
    cd $rootPath
    $name=$item.Name
    & $sonarPath begin /k:$name /d:sonar.host.url=$url /d:sonar.login=$login /d:sonar.cs.dotcover.reportsPaths="%CD%/dotCover.html"
    & $nugetPath restore
    & $msBuildPath /t:Rebuild
    
    If($item.Tests.Length -gt 0) {
        foreach ($test in $item.Tests) {
            $tname=$test.Name
            & $dotCoverPath cover /Output="%CD%/$tname.snapshot" "/TargetExecutable=$vsTestPath" /TargetWorkingDir=. "/TargetArguments=$tname\bin\Debug\$tname.dll"
        }
        $snapfiles = $item.Tests | % {$_.Name} | % {join-path %CD%/ $_".snapshot" }
        $snaphots = $snapfiles -join ";"
        & $dotCoverPath merge /Source="$snaphots" /Output="%CD%/dotCover.snapshot"
        & $dotCoverPath report /Source="%CD%/dotCover.snapshot" /ReportType=HTML /Output="%CD%/dotCover.html"
    }   
    & $sonarPath end /d:sonar.login=$login
}
