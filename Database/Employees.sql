CREATE TABLE [dbo].[Employees]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PayrollNumber] VARCHAR(10) NOT NULL, 
    [Forenames] VARCHAR(25) NOT NULL, 
    [Surname] VARCHAR(25) NOT NULL, 
    [DateOfBirth] DATETIME NOT NULL, 
    [Telephone] VARCHAR(20) NOT NULL, 
    [Mobile] VARCHAR(20) NOT NULL, 
    [Adress] VARCHAR(100) NULL, 
    [Adress2] VARCHAR(100) NOT NULL, 
    [Postcode] VARCHAR(10) NULL, 
    [Email] VARCHAR(50) NULL, 
    [StartDate] DATETIME NOT NULL
)