#addin nuget:?package=Cake.Json
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

/// <summary>
/// - Import Setting from other Cake.
/// </summary>
#load build/paths.cake
#load build/version.cake
#load build/urls.cake

var target = Argument("target", "Default-Build");
var configuration = Argument("configuration", "Release");
var packageOutputPath = Argument<DirectoryPath>("packageOutputPath", "packages");
var domainPublishOutputPath = Argument<DirectoryPath>("domainPublishOutputPath", "domainpublish");
var webPublishOutputPath = Argument<DirectoryPath>("webPublishOutputPath", "webpublish");
var runtime = Argument("runtime", "linux-x64");

// // Version
Version versionInfo = null;

Task("Versioning")
    .Does(() => 
    {
        var file = new FilePath ("version.json");
        versionInfo = DeserializeJsonFromFile<Version> (file);
        Information("MajorMinorPatch version: " + versionInfo.MajorMinorPatch);
        Information("SemVersion version: " + versionInfo.SemVer);
        Information("InformationalVersion version: " + versionInfo.InformationalVersion);
        Information("NugetVersion version: " + versionInfo.NuGetVersion);
        Information("Build version: " + versionInfo.FullBuildMetaData);
    });

Task("Restore")
    .Does(() =>
    {
        Information("Restore Start");
        DotNetCoreRestore(Paths.SolutionFile.GetDirectory().FullPath);
    });

Task("Build")
    .Does(() =>
    {
        Information("Build Start");
        var buildSettings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        };

        DotNetCoreBuild(
            Paths.SolutionFile.GetDirectory().FullPath, 
            buildSettings
        );
    });
Task("Publish-Domain")
    .Does(() =>
    {
        Information("Publish-Domian Start");
        var publishSettings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = domainPublishOutputPath,
            Runtime = runtime
        };

        DotNetCorePublish(
            Paths.DomainProjectFile.GetDirectory().FullPath, 
            publishSettings
        );
    });

Task("Publish-Web")
    .Does(() =>
    {
        Information("Publish-Web Start");
        var publishSettings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = webPublishOutputPath,
            Runtime = runtime
        };

        DotNetCorePublish(
            Paths.WebProjectFile.GetDirectory().FullPath, 
            publishSettings
        );
    });

Task("Test")
    .Does(() =>
    {
        Information("Test Start");
        DotNetCoreTest(Paths.TestProjectFile.GetDirectory().FullPath, new DotNetCoreTestSettings
            {
                NoBuild = true,
                NoRestore = true,
                Configuration = configuration
            });
    });

Task("Clean-Folder")
    .Does(() =>
    {
        CleanDirectory(packageOutputPath);
        CleanDirectory(domainPublishOutputPath);
        CleanDirectory(webPublishOutputPath);
    });

Task("Package-NuGet")
    .Does(() =>
    {    
        var packSettings = new DotNetCorePackSettings 
        {
            Configuration = configuration,
            OutputDirectory = packageOutputPath,
            ArgumentCustomization = (args) => {
                return args
                    .Append("/p:Version={0}", versionInfo.SemVer); //versionInfo.SemVer
            }
        };
        DotNetCorePack(
            Paths.InfraProjectFile.GetDirectory().FullPath,
            packSettings);
    });

Task("Deploy-Nuget")
    .Does(() =>
    {
        var settings = new DotNetCoreNuGetPushSettings()
        {
            Source = Urls.DeploymentUrl,
            ApiKey = Urls.ApiKey
        };

        var nugetFiles = GetFiles(packageOutputPath+"/*.nupkg");
        foreach(var file in nugetFiles)
        {				
            // Mock Project no need to push.
            // DotNetCoreNuGetPush(file.FullPath, settings);
        }
    });

Task("Default-Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    // .IsDependentOn("Test")
    .Does(() =>
    {
        Information("This is Default Build.");
    });

Task("Task-Publish-Nuget")
    .IsDependentOn("Clean-Folder")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    // .IsDependentOn("Test")
    .IsDependentOn("Versioning")
    .IsDependentOn("Package-NuGet")
    .IsDependentOn("Deploy-Nuget")
    .Does(() =>
    {
        Information("Publish-Nuget");
    });

Task("Task-Publish-Domain")
    .IsDependentOn("Clean-Folder")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    // .IsDependentOn("Test")
    .IsDependentOn("Publish-Domain")
    .Does(() =>
    {
        Information("Publish-Domain");
    });

Task("Task-Publish-Web")
    .IsDependentOn("Clean-Folder")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    // .IsDependentOn("Test")
    .IsDependentOn("Publish-Web")
    .Does(() =>
    {
        Information("Publish-Web");
    });

RunTarget(target);
