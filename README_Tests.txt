# simple PACS Server orthanc -> docker-compose.yml

version: '3.1'  # Secrets are only available since this version of Docker Compose
services:
  orthanc:
    image: jodogne/orthanc-plugins:1.12.4
    command: /run/secrets/  # Path to the configuration files (stored as secrets)
    ports:
      - 4242:4242
      - 8042:8042
    secrets:
      - orthanc.json
    environment:
      - ORTHANC_NAME=HelloWorld
secrets:
  orthanc.json:



----------------
orthanc config file: orthanc.json

{
  "Name" : "${ORTHANC_NAME} in Docker Compose",
  "RemoteAccessAllowed" : true
}


----------------
create table script for database:

USE [dicomImport]
GO

/****** Object:  Table [dbo].[files]    Script Date: 25.08.2024 22:44:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[filepath] [nvarchar](max) NOT NULL,
	[fileSizeInBytes] [nvarchar](max) NULL,
	[patname] [nvarchar](max) NULL,
	[patbirthd] [nvarchar](max) NULL,
	[institutionName] [nvarchar](max) NULL,
	[error] [nvarchar](max) NULL,
 CONSTRAINT [PK__files__3214EC0758759EB2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



