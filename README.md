# MockSite

Mock Project for external members

MockSite
- Src
  - MockSite.Common.Core
    - 放置 Constant 設定
    - 放置狀態碼列舉值
    - 放置 Request & Response Base Model
    - 放置設定檔讀取的 Helper 類別
  - MockSite.Common.Data
    - 資料存取層的封裝
  - MockSite.Common.Logging
    - Logging Framework 封裝
  - MockSite.Core
    - 放置 MockSite 會用到的 Entity & DTO
    - 放置 Repository 用 MockSite.Common.Data 從資料存取層讀取資料
    - User Service 接口與實作
  - MockSite.DomainService
    - gRPC Service 實作
    - gRPC service binding
    - 用 Unity 解析 UserService 
    - 用 Unity 定義解析時使用 UserService/Performanceinterceptor/UserRepository
    - 用 Unity 定義如何用 AOP 監測執行時間
  - MockSite.Message
    - gRPC 產生的類別
  - MockSite.Test
  - MockSite.Web
    - 使用 AddScoped 設定 UserService 的注入
    - 定義 UserController 透過 gRPC 調用 gRPC server 的 UserService 
    - 定義 AuthorizedController 透過建構子注入 MockSite.Web 內的 UserService
- database
  - 從 Dockhub 抓取 mariadb I mage
  - 設定 Port 導向
  - 設定 mariadb password
  - 設定容器卷
  - dump 目錄內的 script 會自動執行初始資料

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

4. install dotnet core https dev cert

    ```bash
    dotnet dev-certs https --trust
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

    dotnet run

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
