#addin "Cake.Incubator"

///////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////
// BUILD VARIABLES
///////////////////////////////////////////////////////////////////////////
var artifactsDir = Directory("./artifacts");

var preReleaseSuffix = 
    HasArgument("PreReleaseSuffix") ? Argument<string>("PreReleaseSuffix") :
    (AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.Repository.Tag.IsTag) ? null : 
    EnvironmentVariable("PreReleaseSuffix") != null ? EnvironmentVariable("PreReleaseSuffx") :
    "beta";

var buildNumber = 
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) :
    0;

///////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    var dirsToClean = GetDirectories("./**/**/bin").Concat(GetDirectories("./**/**/obj"));
    Information("Dirs count {0}", dirsToClean.Count());
    foreach(var dir in dirsToClean)
    {
        Information("Clean dir {0}", dir);
    }
    CleanDirectories(dirsToClean);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var projects = GetFiles("./**/**/*.csproj");
    foreach (var project in projects)
    {
        Information("Restoring project {0}", project);
        DotNetCoreRestore(project.FullPath);
    }
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    var projects = GetFiles("./**/**/*.csproj");
    foreach (var project in projects)
    {
        Information("Building project {0}", project);
        DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var path = "./test/**/*.Tests.csproj";
    Information("Path to tests {0}", path);
    var testProjects = GetFiles(path);
    foreach (var project in testProjects)
    {
        Information("Testing project {0}", project);
        DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true
        });
    }
});

Task("Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    string versionSuffix = null;
    if (!string.IsNullOrEmpty(preReleaseSuffix))
    {
        versionSuffix = preReleaseSuffix + "-" + buildNumber.ToString("D4");
    }
    foreach (var project in GetFiles("./src/**/*.csproj"))
    {
        DotNetCorePack(project.FullPath, new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = artifactsDir,
            VersionSuffix = versionSuffix
        });
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);