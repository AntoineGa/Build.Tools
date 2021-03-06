#r    "./fake/fakelib.dll"
#load "./Utils.fsx"
#load "./Packaging.fsx"
#load "./Versioning.fsx"
#load "./Solution.fsx"
#load "./Test.fsx"
#load "./Specflow.fsx"
#load "./Grunt.fsx"
#load "./Octopus.fsx"
#load "./Docker.fsx"
#load "./SonarQube.fsx"
#load "./SQLServer.fsx"

open System.IO
open Fake
open System

let config = 
    Map.ofList [
        "build:configuration",          environVarOrDefault "configuration"         "Release"
        "build:solution",               environVar          "solution"
        "core:tools",                   environVarOrDefault "tools"                 "tools"
        "grunt:environment",            environVarOrDefault "gruntenvironment"      "dev"
        "grunt:verbose",                environVarOrDefault "gruntverbose"          "false"
        "packaging:output",             environVarOrDefault "output"                (sprintf "%s\output" (Path.GetFullPath(".")))
        "packaging:deployoutput",       environVarOrDefault "deployoutput"          (sprintf "%s\deploy" (Path.GetFullPath(".")))
        "packaging:outputsubdirs",      environVarOrDefault "outputsubdirs"         "false"
        "packaging:updateid",           environVarOrDefault "updateid"              ""
        "packaging:pushto",             environVarOrDefault "pushto"                ""
        "packaging:pushdir",            environVarOrDefault "pushdir"               ""
        "packaging:pushurl",            environVarOrDefault "pushurl"               ""
        "packaging:apikey",             environVarOrDefault "apikey"                ""
        "packaging:deploypushto",       environVarOrDefault "deploypushto"          ""
        "packaging:deploypushdir",      environVarOrDefault "deploypushdir"         ""
        "packaging:deploypushurl",      environVarOrDefault "deploypushurl"         ""
        "packaging:importConnectionstring",      environVarOrDefault "importConnectionstring"         ""
        "packaging:exportConnectionstring",      environVarOrDefault "exportConnectionstring"         ""
        "packaging:localConnectionstring",       environVarOrDefault "localConnectionstring"          ""
        "packaging:deployapikey",       environVarOrDefault "deployapikey"          ""
        "packaging:packages",           environVarOrDefault "packages"              ""
        "versioning:build",             environVarOrDefault "build_number"          "0"
        "versioning:branch",            match environVar "teamcity_build_branch" with
                                            | "<default>" -> environVar "vcsroot_branch"
                                            | _ -> environVar "teamcity_build_branch"
        "vs:version",                   environVarOrDefault "vs_version"            "14.0" 
        "env:workspace",                environVarOrDefault "env:workspace"         (Environment.CurrentDirectory) 
        ]

// Target definitions
Target "Default"                       <| DoNothing
Target "Packaging:Package"             <| Packaging.package config
Target "Packaging:PackageDeploy"       <| Packaging.packageDeploy config
Target "Packaging:Restore"             <| Packaging.restore config
Target "Packaging:Update"              <| Packaging.update config
Target "Packaging:Push"                <| Packaging.push config
Target "Packaging:Constrain"           <| Packaging.constrain config
Target "Packaging:PushDeploy"          <| Packaging.pushDeploy config
Target "Solution:Build"                <| Solution.build config
Target "Solution:SonarQubeBegin"       <| SonarQube.sonarQubeBegin config
Target "Solution:SonarQubeEnd"         <| SonarQube.sonarQubeEnd config
Target "Solution:Clean"                <| Solution.clean config
Target "Versioning:Update"             <| Versioning.update config
Target "Versioning:UpdateDeployNuspec" <| Versioning.updateDeploy config
Target "Grunt:Install"                 <| Grunt.install config
Target "Grunt:Run"                     <| Grunt.run config
Target "Grunt:Karma"                   <| Grunt.karma config
Target "Grunt:Protractor"              <| Grunt.protractor config
Target "Test:Run"                      <| Test.run config
Target "SpecFlow:Run"                  <| Specflow.run config
Target "Docker:Package"                <| Docker.dockerize config

// Build order
"Solution:Clean"
    ==> "Packaging:Restore"
    ==> "Versioning:Update"
    ==> "Solution:SonarQubeBegin"
    ==> "Solution:Build"
    ==> "Solution:SonarQubeEnd"
    // ==> "Packaging:Package"
    // ==> "SpecFlow:Run"
    // ==> "Test:Run"
    // =?> ("Packaging:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"
