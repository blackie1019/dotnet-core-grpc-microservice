# Data 前置準備 #

## 在 Docker 使用 Container 前配置動作 ##

1. 至目錄內

    Path of Domain Service: `/database`
    
    執行下方指令建立容器環境
    
        docker-compose up -d

2. 可以透過以下指令確認當前運行的環境狀態：

        docker-compose ps

    或是也可以透過下方指令查看所有 container 狀態:

        docker ps -a

3. 透過 *docker-compose* 所 porting 的連線位置取得 DB Instance 的操作

## 匯入 Data 配置動作 ##

1. 找出當前使用的 MariaDB/MySQL image 版本與運行中的 ContainerID

2. 準備 *00_init.sql* 內容去新增 User

3. 透過下方指令建立 *01_backup.sql* 至當前目錄下

    ```shell
    docker exec <ContainerID> /usr/bin/mysqldump --routines -B <Schema Name> -u root --password=<root password> <Schema Name> > dump/01_backup.sql
    ```

4. 修改 *docker-compose* ，調整 MariaDB/MySQL image 版本