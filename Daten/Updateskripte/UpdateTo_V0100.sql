﻿-- Fehler in Vorteil korrigieren
UPDATE [VorNachteil] SET [Setting] = 'Aventurien, Myranor', [Literatur] = 'WdH 258 / WnM 120' WHERE [Name] LIKE 'Wesen der Nacht%';

--Anpassungen an Literatur-Tabellen
ALTER TABLE [Literatur] ADD [Regelsystem] int NOT NULL DEFAULT 0;
ALTER TABLE [Literatur] ADD [Nummer] nvarchar(50) NULL;
ALTER TABLE [Literatur] ADD [Art] nvarchar(150) NULL;
ALTER TABLE [Literatur] ADD [Reihe] nvarchar(150) NULL;
ALTER TABLE [Literatur] ADD [Setting] nvarchar(150) NULL DEFAULT 'Aventurien';
ALTER TABLE [Literatur] ADD [Box] nvarchar(150) NULL;
