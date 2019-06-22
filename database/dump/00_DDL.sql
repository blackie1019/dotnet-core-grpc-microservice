CREATE USER 'dbowner'@'%' IDENTIFIED BY 'pass.123';
GRANT All privileges ON *.* TO 'dbowner'@'%';

CREATE DATABASE IF NOT EXISTS TestDB;
use TestDB;

-- User --

CREATE TABLE `User` (
  `Id`            int AUTO_INCREMENT NOT NULL,
  `Code`          varchar(20)        NOT NULL,
  `Name`          varchar(40)        NOT NULL,
  `Email`         varchar(160)       NOT NULL,
  `Password`      varchar(70)        NOT NULL,
  UNIQUE (`Code`),
  PRIMARY KEY (`Id`)
);

CREATE INDEX IX_User_Code ON User (Code);

-- -- Create User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_CreateUser`(
  IN  IN_Code         varchar(20),
  IN  IN_Name         varchar(40),
  IN  IN_Email        varchar(160),
  IN  IN_Password     varchar(70),
  OUT OUT_ReturnValue int
)
BEGIN
  SET OUT_ReturnValue = 0;
  INSERT INTO User (Code, Name, Email, Password) values (IN_Code, IN_Name, IN_Email, IN_Password);
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Get Users --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetUsers`(OUT OUT_ReturnValue int)
BEGIN
  SET OUT_ReturnValue = 0;
  SELECT * FROM User;
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Get User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_GetUser`(
  IN  IN_Id           int,
  OUT OUT_ReturnValue int
)
BEGIN
    SET OUT_ReturnValue = 0;
    SELECT * FROM User WHERE Id = IN_Id LIMIT 1;
    SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;

-- Update User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_UpdateUser`(
  IN  IN_Id           int,
  IN  IN_Name         varchar(40),
  IN  IN_Email        varchar(160),
  OUT OUT_ReturnValue int
)
BEGIN
    SET OUT_ReturnValue = 0;
    UPDATE User SET Name = IN_Name, Email = IN_Email WHERE Id = IN_Id;
    SET OUT_ReturnValue = 1;
  END ;;
DELIMITER ;

-- Delete User --

DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `sp_User_DeleteUser`(
  IN  IN_Id           int,
  OUT OUT_ReturnValue int
)
BEGIN
  SET OUT_ReturnValue = 0;
  DELETE FROM User WHERE Id = IN_Id;
  SET OUT_ReturnValue = 1;
END ;;
DELIMITER ;
