USE MeerkeuzevragentestsEDG;
GO


INSERT INTO Studenten (Naam) VALUES
    ('Bruce'),
    ('Brucifer'),
    ('Eli'),
    ('Elisa'),
    ('Nuno'),
    ('Evert');


INSERT INTO Onderwerpen (Naam) VALUES
    ('SQL'),
    ('Muziek');


INSERT INTO Vragen (InhoudTekst, Moeilijkheidsgraad, IsActief) VALUES
    ('Waar staat SQL voor?',                                        'Beginner',   1),
    ('Welke SQL-opdracht gebruik je om gegevens op te halen?',      'Beginner',   1),
    ('Welke clausule gebruik je om resultaten te filteren?',        'Beginner',   1),
    ('Welke SQL-opdracht voeg je een nieuwe rij toe met?',          'Beginner',   1),
    ('Welke artiest bracht het album Thriller uit?',                'Open level', 1),
    ('Welke band zong Bohemian Rhapsody?',                          'Open level', 1),
    ('Welke artiest staat bekend om het nummer Shape of You?',      'Open level', 1),
    ('Welke band maakte het nummer Hotel California?',              'Open level', 1);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (1, 'Structured Query Language', 1),
    (1, 'Simple Query Language',     0),
    (1, 'Sequential Query Language', 0),
    (1, 'System Query Logic',        0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (2, 'GET',    0),
    (2, 'SELECT', 1),
    (2, 'FETCH',  0),
    (2, 'FIND',   0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (3, 'FILTER', 0),
    (3, 'LIMIT',  0),
    (3, 'WHERE',  1),
    (3, 'SORT',   0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (4, 'ADD',    0),
    (4, 'INSERT', 1),
    (4, 'CREATE', 0),
    (4, 'PUT',    0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (5, 'Michael Jackson', 1),
    (5, 'Prince',          0),
    (5, 'Madonna',         0),
    (5, 'David Bowie',     0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (6, 'The Beatles', 0),
    (6, 'Queen',       1),
    (6, 'Led Zeppelin',0),
    (6, 'The Rolling Stones', 0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (7, 'Justin Bieber', 0),
    (7, 'Ed Sheeran',    1),
    (7, 'Bruno Mars',    0),
    (7, 'Harry Styles',  0);


INSERT INTO Antwoorden (VraagId, Inhoud, IsCorrect) VALUES
    (8, 'Fleetwood Mac', 0),
    (8, 'Eagles',        1),
    (8, 'The Doors',     0),
    (8, 'Creedence Clearwater Revival', 0);


INSERT INTO VraagOnderwerpen (VraagId, OnderwerpId) VALUES
    (1, 1), (2, 1), (3, 1), (4, 1);


INSERT INTO VraagOnderwerpen (VraagId, OnderwerpId) VALUES
    (5, 2), (6, 2), (7, 2), (8, 2);

GO


SELECT 'Studenten'  AS Tabel, COUNT(*) AS Aantal FROM Studenten  UNION ALL
SELECT 'Onderwerpen',          COUNT(*)           FROM Onderwerpen UNION ALL
SELECT 'Vragen',               COUNT(*)           FROM Vragen      UNION ALL
SELECT 'Antwoorden',           COUNT(*)           FROM Antwoorden  UNION ALL
SELECT 'VraagOnderwerpen',     COUNT(*)           FROM VraagOnderwerpen;
GO