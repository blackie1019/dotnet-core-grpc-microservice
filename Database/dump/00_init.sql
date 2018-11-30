CREATE USER 'dbowner'@'%' IDENTIFIED BY 'pass.123';
GRANT All privileges ON *.* TO 'dbowner'@'%';

CREATE DATABASE TestDB;