# MockSite

Mock Project for external members

## Step 1 - DB Restroe

Please see [database preparation README file](Database/README.md) on DataBase folder.

---

## Step 2 - Build Code

 `dotnet buils` command of project is depand on **cake** and **gitversion** with dotnet tools.

### 1. Installation

1. .NET Code

    Download Package from
    [https://www.microsoft.com/net/download](https://www.microsoft.com/net/download)

2. intsall `dotnet tool cake`

    ```bash
    dotnet tool install -g Cake.Tool --version 0.30.0
    ```

3. install `dotnet tool gitversion`

    ```bash
    dotnet tool install --global GitVersion.Tool --version 4.0.1-beta1-47
    ```

---

### 2. Try Run

Default Build will trigger **Restore**, **Build** and **Test**. So need to start up DB first.

#### Build with Cake

Execute :

```bash
dotnet cake build.cake --target="Default-Build"
```

#### Get Version with gitversion

Excute :

```bash
dotnet gitversion > version.json
```

#### Default build-task to define flow of publish

1. Default-Build

   ```csharp
    Task("Task-Publish-Nuget")
        .IsDependentOn("Clean-Folder")
        .IsDependentOn("Restore")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("Versioning")
        .IsDependentOn("Package-NuGet")
        .IsDependentOn("Deploy-Nuget")
        .Does(() =>
        {
            Information("Publish-Nuget");
        });
   ```

2. Default-Nuget-Deployment

   ```csharp
    Task("Task-Publish-Domain")
        .IsDependentOn("Clean-Folder")
        .IsDependentOn("Restore")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("Publish-Domain")
        .Does(() =>
        {
            Information("Publish-Domain");
        });
   ```

3. Default-Publish

    ```csharp
    Task("Task-Publish-Web")
        .IsDependentOn("Clean-Folder")
        .IsDependentOn("Restore")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("Publish-Web")
        .Does(() =>
        {
            Information("Publish-Web");
        });
    ```

---

## Development Step

---

![System_Layer_HLD-MockSite.png](images/System_Layer_HLD-MockSite.png)

---

### Prerequisite ###

Install [node](https://nodejs.org/en/download/) in your system.

Path of front-end : `/src/MockSite.Web/ClientApp`

And do `npm install`.

### 1. Host gRPC Domain Service ###

path of Domain Service: `/src/MockSite.DomainService`

Please run below command to run console for Domain Service:

    dotnet run -f netcoreapp2.1

### 2. Front-end Development

[Front-end UI framework development README file](/src/MockSite.Web/ClientApp/README.md)

#### 3. React Development Only

1. Path of ASP.NET Core solution: `/MockSite.sln`

Run or Debug from Rider/Dotnet CLI/vscode to host `SPA Service` for React Development

2. Path of front-end : `/src/MockSite.Web/ClientApp`

   prerequisite steps: do `npm install` first and run below command to start front-end:

        npm start

---

### Additional: gRPC development

Please reference [gRPC development README file](protos/README.md) if you try to add new method in Domain Servcie and explore to Application to calling to support front-end CRUD of data.

---

## Docker and Docker-compose(Still fix problem...)

Using `deploy.sh` to build entire envionment(exclude Database) with `docker-compose`

---
