#r "./tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.NuGet.Install

// Properties
let buildDir = "./build/"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "RestorePackages" (fun _ ->
    "src/Void.sln"
    |> RestoreMSSolutionPackages (fun p ->
        { p with OutputPath = "src/packages" })
)

Target "Compile" (fun _ ->
   !! "src/Void.sln"
   |> MSBuildRelease buildDir "Build"
   |> Log "Compile-Output: "
)

(* If the build ends up having more NuGet dependencies than this, I should
 * probably make a packages.config for it instead, and make this target
 * "AcquireBuildDependencies". "NUnit.Runners" can simply be replaced with
 * "packages.config". *)
Target "AcquireUnitTestRunner" (fun _ ->
    "NUnit.Runners"
    |> NugetInstall (fun p ->
        { p with
            ToolPath = "src/.nuget/NuGet.exe"
            OutputDirectory = "tools/"
            ExcludeVersion = true })
)

Target "Test" (fun _ ->
    !! (buildDir + "*.Spec.dll")
    |> NUnit (fun p ->
        { p with
            DisableShadowCopy = true
            OutputFile = buildDir + "TestResults.xml" })
)

Target "Default" (fun _ ->
    trace "Completed!"
)

// Dependencies
"Clean"
  ==> "RestorePackages"
  ==> "Compile"
  ==> "AcquireUnitTestRunner"
  ==> "Test"
  ==> "Default"

// start build
RunTargetOrDefault "Default"
