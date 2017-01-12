//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Package");
var configuration = Argument("configuration", "Release");
var version = "0.0.2";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var projectDir = Directory("../src");
var buildDir = projectDir + Directory("bin") + Directory(configuration);
var packageDir = projectDir + Directory("nuget");

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore("../src/Mitten.Server.sln");
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        if(IsRunningOnWindows())
        {
            // Use MSBuild
            MSBuild("../src/Mitten.Server.sln", settings => settings.SetConfiguration(configuration));
        }
        else
        {
            // Use XBuild
            XBuild("../src/Mitten.Server.sln", settings => settings.SetConfiguration(configuration));
        }
    });

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

Task("Build-NuGet-Package")
    .IsDependentOn("Build")
    .Does(() =>
    {
	var baseDir = packageDir + Directory("bin/Mitten.Server");
        var libDir = baseDir + Directory("net-4.5");

	CleanDirectory(packageDir);
        CreateDirectory(libDir);	

	CopyFileToDirectory(buildDir + File("Mitten.Server.dll"), libDir);
        CopyFileToDirectory(buildDir + File("Mitten.Server.xml"), libDir);

        NuGetPack("Mitten.Server.nuspec", new NuGetPackSettings()
        {
            Version = version,
            BasePath = baseDir,
            OutputDirectory = packageDir
        });
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
    .IsDependentOn("Build-NuGet-Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
