IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105110546_InitialCreate')
BEGIN
    CREATE TABLE [Profile] (
        [ID] int NOT NULL IDENTITY,
        [etunimi] nvarchar(max) NULL,
        [sukunimi] nvarchar(max) NULL,
        [sposti] nvarchar(max) NULL,
        [puhelin] nvarchar(max) NULL,
        [katuosoite] nvarchar(max) NULL,
        [postinumero] nvarchar(max) NULL,
        [postitoimipaikka] nvarchar(max) NULL,
        CONSTRAINT [PK_Profile] PRIMARY KEY ([ID])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105110546_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211105110546_InitialCreate', N'3.1.20');
END;

GO

