#r "./fake/fakelib.dll"
#r "./fake/Fake.SQL.dll"
#r "System.Management.Automation"
#r "./SQL/Microsoft.SqlServer.SqlClrProvider.dll"
#r "./SQL/Microsoft.SqlServer.Smo.dll"

#load "./Utils.fsx"

open Fake
open System
open Fake.SQL.SqlServer
open Utils

let sqlLocalDbExe = @"C:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe"
let sqlPackageExe = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\130\sqlpackage.exe"
let workspace = ""
let bacpacName = ""
let fullBacpacPath = workspace + "\\" + bacpacName

let localConnectionString = ""
let importConnectionString = ""
let exportConnectionString = ""

Target "ExportSql" (fun _ ->
  ExecProcess (fun info ->
      info.FileName <- sqlPackageExe
      info.WorkingDirectory <- workspace
      info.Arguments <- "/Action:Export /tf:\""+ fullBacpacPath + "\" /scs:\"" + exportConnectionString + "\" ") TimeSpan.MaxValue
  |> ignore
)



Target "ImportSql" (fun _ ->
    
    let targetServerInfo = getServerInfo localConnectionString
    let targetDbName = getDBName targetServerInfo
    if not (existDBOnServer targetServerInfo targetDbName) then
        ExecProcess (fun info ->
            info.FileName <- sqlPackageExe
            info.WorkingDirectory <- workspace
            info.Arguments <- "/Action:Import /sf:\"" + fullBacpacPath + "\" /tcs:\""+importConnectionString+"\" ") TimeSpan.MaxValue
            |> ignore
    // else runScript targetServerInfo @"AH.eTLB.Database\Scripts\Post-deployment\Insert.Localization.sql"

)