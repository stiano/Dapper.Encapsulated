USE [master]
GO

/****** Object:  Database [dapper.encapsulated.tests]    Script Date: 03-Apr-21 01:42:58 PM ******/
CREATE DATABASE [dapper.encapsulated.tests]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'dapper.encapsulated.tests', FILENAME = N'/var/opt/mssql/data/dapper.encapsulated.tests.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'dapper.encapsulated.tests_log', FILENAME = N'/var/opt/mssql/data/dapper.encapsulated.tests_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dapper.encapsulated.tests].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ARITHABORT OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET  DISABLE_BROKER 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET RECOVERY FULL 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET  MULTI_USER 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [dapper.encapsulated.tests] SET DB_CHAINING OFF 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [dapper.encapsulated.tests] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [dapper.encapsulated.tests] SET QUERY_STORE = OFF
GO

ALTER DATABASE [dapper.encapsulated.tests] SET  READ_WRITE 
GO


