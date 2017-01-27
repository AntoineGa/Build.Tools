#r "./fake/fakelib.dll"

#load "./Utils.fsx"

open Fake
open Utils
open Fake.SonarQubeHelper

let sonarDir = ""


let cleanSonar (config : Map<string, string>) _ =
    trace "Cleanup..."
    CleanDirs [sonarDir]


let sonarQubeBegin (config : Map<string, string>) _ =
    SonarQube Begin (fun p ->
        {p with
            Key = config.get "build:solution"
            Name = config.get "build:solution"
            Version = config.get "versioning:build"
        }
    )


let sonarQubeEnd (config : Map<string, string>) _ =
    SonarQubeEnd()
