public static class Paths
{
    public static FilePath SolutionFile => "./MockSite.sln";
    public static FilePath TestProjectFile => "./src/MockSite.Test/MockSite.Test.csproj";
    public static FilePath InfraProjectFile => "./src/Infrastructure/Infrastructure.csproj";
    public static FilePath DomainProjectFile => "./src/MockSite.DomainService/MockSite.DomainService.csproj";
    public static FilePath WebProjectFile => "./src/MockSite.Web/MockSite.Web.csproj";
}

public static FilePath Combine(DirectoryPath directory, FilePath file)
{
    return directory.CombineWithFilePath(file);
}