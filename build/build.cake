//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Package");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// CONSTANTS
//////////////////////////////////////////////////////////////////////

var version = "0.0.5";
var mittenServerId = "Mitten.Server"; 

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var sourceDir = Directory("../src");
var buildDir = sourceDir + Directory("bin") + Directory(configuration);
var packageDir = sourceDir + Directory("nuget");

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildDir);
    });

Task("Update-Solution-Info")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var file = "../src/Mitten.Server.SolutionInfo.cs";

        CreateAssemblyInfo(file, new AssemblyInfoSettings()
        {
            Version = version,
            FileVersion = version,
            Copyright = "Copyright (c) " + DateTime.Now.Year + " Jeremy Vainavicz"
        });
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore("../src/Mitten.Server.sln");
    });

Task("Build")
    .IsDependentOn("Update-Solution-Info")
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

Task("Build-NuGet-Packages")
    .IsDependentOn("Build")
    .Does(() =>
    {
        CleanDirectory(packageDir);

        BuildPackage(
            "Mitten.Server",
            GetDependency("../src/Mitten.Server/packages.config", "Newtonsoft.Json"),
            GetDependency("../src/Mitten.Server/packages.config", "NodaTime"));

        BuildPackage(
            "Mitten.Server.Commands",
            new NuSpecDependency() { Id = mittenServerId, Version = version },
            GetDependency("../src/Mitten.Server.Commands/packages.config", "Newtonsoft.Json"),
            GetDependency("../src/Mitten.Server.Commands/packages.config", "NodaTime"),
            GetDependency("../src/Mitten.Server.Commands/packages.config", "System.Reactive.Core"),
            GetDependency("../src/Mitten.Server.Commands/packages.config", "System.Reactive.Interfaces"),
            GetDependency("../src/Mitten.Server.Commands/packages.config", "System.Reactive.Linq"));

        BuildPackage(
            "Mitten.Server.Notifications",
            new NuSpecDependency() { Id = mittenServerId, Version = version },
            GetDependency("../src/Mitten.Server.Notifications/packages.config", "Newtonsoft.Json"),
            GetDependency("../src/Mitten.Server.Notifications/packages.config", "NodaTime"),
            GetDependency("../src/Mitten.Server.Notifications/packages.config", "PushSharp"));
    });

//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

void BuildPackage(string projectName, params NuSpecDependency[] dependencies)
{
    var baseDir = packageDir + Directory("bin/" + projectName);
    var libDir = baseDir + Directory("net-4.5");

    CreateDirectory(libDir);	

    CopyFileToDirectory(buildDir + File(projectName + ".dll"), libDir);
    CopyFileToDirectory(buildDir + File(projectName + ".xml"), libDir);

    NuGetPack(projectName + ".nuspec", new NuGetPackSettings()
    {
        Version = version,
        Dependencies = new List<NuSpecDependency>(dependencies),
        BasePath = baseDir,
        OutputDirectory = packageDir
    });
}

NuSpecDependency GetDependency(string packageConfig, string dependencyId)
{
    var dependencyVersion = XmlPeek(File(packageConfig), "/packages/package[@id='" + dependencyId + "']/@version");
    
    return new NuSpecDependency()
    {
        Id = dependencyId,
        Version = dependencyVersion
    };
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
    .IsDependentOn("Build-NuGet-Packages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
