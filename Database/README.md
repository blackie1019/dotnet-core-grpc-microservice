# Data 還原 #

## 開發上傳前的前置動作 ##

1. 找出當前使用的 MariaDB/MySQL image 版本與運行中的 ContainerID

2. 透過下方指令建立 *01_backup.sql* 至當前目錄下

    ```shell
    docker exec <ContainerID> /usr/bin/mysqldump --routines -B <Schema Name> -u root --password=<root password> <Schema Name> > dump/01_backup.sql
    ```

3. 準備 *00_init.sql* 內容去新增 User

4. 修改 *docker-compose* ，調整 MariaDB/MySQL image 版本

## 開發下載後的配置動作 ##

1. 至目錄內執行下方指令建立容器環境
    
        docker-compose up -d

2. 可以透過以下指令確認當前運行的環境狀態：

        docker-compose ps

    或是也可以透過下方指令查看所有 container 狀態:

        docker ps -a

3. 透過 *docker-compose* 所 porting 的連線位置取得 DB Instance 的操作