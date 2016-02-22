#r "./packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.NuGet.Install
open System.IO

// Properties
let buildDir = "./build/"
let version = "0.0.2"
let wxsFile = "SetupTemplate.wxs"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
    DeleteFile wxsFile
    DeleteFile (wxsFile + ".wixobj")
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

(* Got started with script from here:
 * http://fsharp.github.io/FAKE/wix.html
 * *)
Target "BuildWixInstall" (fun _ ->
    (* This defines which files should be collected when running
     * bulkComponentCreation *)
    let fileFilter (file : FileInfo) =
        file.Extension = ".dll" || file.Extension = ".exe" || file.Extension = ".config"
        
    (* Collect Files which should be shipped. Pass directory with your
     * deployment output for buildDir *)
    let components = bulkComponentCreation fileFilter (DirectoryInfo buildDir)

    (* Collect component references for usage in features *)
    let componentRefs = components |> Seq.map (fun comp -> comp.ToComponentRef())

    let completeFeature = generateFeatureElement (fun f -> 
                                                    {f with  
                                                        Id = "Complete"
                                                        Title = "Complete Feature"
                                                        Level = 1 
                                                        Description = "Installs all features"
                                                        Components = componentRefs
                                                        Display = Expand 
                                                    })

    (* Generates a predefined WiX template with placeholders which will be
     * replaced in "FillInWiXScript" *)
    generateWiXScript wxsFile

    let WiXUIMondo = generateUIRef (fun f -> {f with Id = "WixUI_Mondo" })

    let WiXUIError = generateUIRef (fun f -> {f with Id = "WixUI_ErrorProgressText" })

    let MajorUpgrade = generateMajorUpgradeVersion(
                            fun f ->
                                {f with 
                                    Schedule = MajorUpgradeSchedule.AfterInstallExecute
                                    DowngradeErrorMessage = "A later version is already installed, exiting."
                                })

    FillInWiXTemplate "" (fun f ->
                                {f with
                                    ProductCode = System.Guid.NewGuid() // Guid which should be generated on every build
                                    ProductName = "Void"
                                    Description = "A fearless modern text editor in the spirit of Vim"
                                    ProductLanguage = 1033
                                    ProductVersion = version
                                    ProductPublisher = "Polyglot Symposium"
                                    UpgradeGuid = System.Guid.Parse "1b0f1b5e-90fd-4870-8a27-628f94f97d3b"
                                    MajorUpgrade = [MajorUpgrade]
                                    UIRefs = [WiXUIMondo; WiXUIError]
                                    WiXVariables = [{ Id = "WixUILicenseRtf"; Overridable = YesOrNo.No; Value="LICENSE.rtf" }]
                                    ProgramFilesFolder = ProgramFiles32
                                    Components = components
                                    BuildNumber = "0"
                                    Features = [completeFeature]
                                })
        

    (* Run the WiX tools *)
    WiX (fun p -> {p with ToolDirectory = "packages/WiX/tools"}) (buildDir + "Void.msi") wxsFile
)

Target "RebuildMsi" (fun _ ->
    trace "Build MSI completed!"
)

Target "WinCIBuild" (fun _ ->
    trace "Windows CI build completed!"
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

"Rebuild"
 ==> "RebuildMsi"

"BuildWixInstall"
 ==> "RebuildMsi"

"RebuildMsi"
 ==> "WinCIBuild"

// start build
RunTargetOrDefault "Rebuild"
