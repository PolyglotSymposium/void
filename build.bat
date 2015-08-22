@echo off
"src\.nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "tools\" "-ExcludeVersion"
"tools\packages\FAKE\tools\Fake.exe" build.fsx
