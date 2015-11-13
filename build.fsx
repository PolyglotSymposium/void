#r "./packages/FAKE/tools/FakeLib.dll"
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

Target "Test" (fun _ ->
    !! (buildDir + "*.Spec.dll")
    |> NUnit (fun p ->
        { p with
            DisableShadowCopy = true
            OutputFile = buildDir + "TestResults.xml" })
)

Target "Rebuild" (fun _ ->
    trace "Rebuild completed!"
)

Target "JustRunTests" (fun _ ->
    trace "Test run completed!"
)

// Dependencies
"Test"
  ==> "JustRunTests"

"Clean"
  ==> "RestorePackages"
  ==> "Compile"
  ==> "Rebuild"

"Test"
  ==> "Rebuild"

// start build
RunTargetOrDefault "Rebuild"
