NuGet.exe update -self

msbuild.exe ..\Dynamo.RazorTemplates.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

NuGet.exe pack Dynamo.RazorTemplates.nuspec

pause