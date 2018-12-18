#! /bin/sh
docker exec 08fa /usr/bin/mysqldump --routines -B TestDB -u root --password=pass.123 TestDB > dump/01_backup.sql # export Instance DB

docker-compose up -d # 重建DB

docker-compose ps # 確認當前DB狀態
