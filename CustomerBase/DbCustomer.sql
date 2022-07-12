CREATE DATABASE [DbCustomer]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [DbCustomer].[dbo].[Customers](
  [Id] [int] NOT NULL,
  [Name] [varchar](50) NOT NULL,
  [Email] [varchar](50) NULL,
  [PhoneNumber] [varchar](14) NULL,
CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED
(
  [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [DbCustomer].[dbo].[Addresses](
  [CustomerId] [int] NOT NULL,
  [Street] [varchar](50) NOT NULL,
  [Number] [int] NOT NULL,
  [Complement] [varchar](30) NULL,
  [District] [varchar](30) NULL,
  [ZipCode] [varchar](9) NOT NULL,
CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED
(
  [CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [DbCustomer].[dbo].[Addresses] WITH CHECK ADD CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY([CustomerId])
REFERENCES [DbCustomer].[dbo].[Customers] ([Id])
GO

ALTER TABLE [DbCustomer].[dbo].[Addresses] CHECK CONSTRAINT [FK_Addresses_Customers]
GO

DELETE FROM [DbCustomer].[dbo].[Customers]
DELETE FROM [DbCustomer].[dbo].[Addresses]
