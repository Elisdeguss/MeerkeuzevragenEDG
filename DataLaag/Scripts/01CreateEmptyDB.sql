
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MeerkeuzevragentestsEDG')
BEGIN
    CREATE DATABASE MeerkeuzevragentestsEDG;
END
GO

USE MeerkeuzevragentestsEDG;
GO


IF OBJECT_ID('StudentAntwoorden', 'U') IS NOT NULL DROP TABLE StudentAntwoorden;
IF OBJECT_ID('TestResultaten', 'U') IS NOT NULL DROP TABLE TestResultaten;
IF OBJECT_ID('TestVragen', 'U') IS NOT NULL DROP TABLE TestVragen;
IF OBJECT_ID('TestOnderwerpen', 'U') IS NOT NULL DROP TABLE TestOnderwerpen;
IF OBJECT_ID('VraagOnderwerpen', 'U') IS NOT NULL DROP TABLE VraagOnderwerpen;
IF OBJECT_ID('Antwoorden', 'U') IS NOT NULL DROP TABLE Antwoorden;
IF OBJECT_ID('Vragen', 'U') IS NOT NULL DROP TABLE Vragen;
IF OBJECT_ID('Testen', 'U') IS NOT NULL DROP TABLE Testen;
IF OBJECT_ID('Onderwerpen', 'U') IS NOT NULL DROP TABLE Onderwerpen;
IF OBJECT_ID('Studenten', 'U') IS NOT NULL DROP TABLE Studenten;



CREATE TABLE Onderwerpen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Naam NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Studenten (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Naam NVARCHAR(100) NOT NULL
);

CREATE TABLE Vragen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InhoudTekst NVARCHAR(MAX) NOT NULL,
    Moeilijkheidsgraad NVARCHAR(50) NULL,
    IsActief BIT NOT NULL DEFAULT 1 
);

CREATE TABLE Testen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(100) NOT NULL, 
    Datum DATETIME NOT NULL DEFAULT GETDATE()
);



CREATE TABLE Antwoorden (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VraagId INT NOT NULL,
    Inhoud NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL, 
    CONSTRAINT FK_Antwoorden_Vragen FOREIGN KEY (VraagId) REFERENCES Vragen(Id)
);


CREATE TABLE VraagOnderwerpen (
    VraagId INT NOT NULL,
    OnderwerpId INT NOT NULL,
    PRIMARY KEY (VraagId, OnderwerpId),
    CONSTRAINT FK_VraagOnderwerpen_Vragen FOREIGN KEY (VraagId) REFERENCES Vragen(Id),
    CONSTRAINT FK_VraagOnderwerpen_Onderwerpen FOREIGN KEY (OnderwerpId) REFERENCES Onderwerpen(Id)
);


CREATE TABLE TestOnderwerpen (
    TestId INT NOT NULL,
    OnderwerpId INT NOT NULL,
    PRIMARY KEY (TestId, OnderwerpId),
    CONSTRAINT FK_TestOnderwerpen_Testen FOREIGN KEY (TestId) REFERENCES Testen(Id),
    CONSTRAINT FK_TestOnderwerpen_Onderwerpen FOREIGN KEY (OnderwerpId) REFERENCES Onderwerpen(Id)
);


CREATE TABLE TestVragen (
    TestId INT NOT NULL,
    VraagId INT NOT NULL,
    PRIMARY KEY (TestId, VraagId),
    CONSTRAINT FK_TestVragen_Testen FOREIGN KEY (TestId) REFERENCES Testen(Id),
    CONSTRAINT FK_TestVragen_Vragen FOREIGN KEY (VraagId) REFERENCES Vragen(Id)
);



CREATE TABLE TestResultaten (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL,
    TestId INT NOT NULL,
    Score INT NULL,
    DatumUitvoering DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TestResultaten_Studenten FOREIGN KEY (StudentId) REFERENCES Studenten(Id),
    CONSTRAINT FK_TestResultaten_Testen FOREIGN KEY (TestId) REFERENCES Testen(Id)
);

CREATE TABLE StudentAntwoorden (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TestResultaatId INT NOT NULL,
    VraagId INT NOT NULL,
    GekozenAntwoordId INT NOT NULL, 
    CONSTRAINT FK_StudentAntwoorden_Resultaat FOREIGN KEY (TestResultaatId) REFERENCES TestResultaten(Id),
    CONSTRAINT FK_StudentAntwoorden_Vragen FOREIGN KEY (VraagId) REFERENCES Vragen(Id),
    CONSTRAINT FK_StudentAntwoorden_Antwoorden FOREIGN KEY (GekozenAntwoordId) REFERENCES Antwoorden(Id)
);
