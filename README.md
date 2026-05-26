# \# Meerkeuzevragen — Programmeren Gevorderd 1

# 

# Applicatie voor het opstellen, afleggen en beheren van meerkeuzevragen, gebouwd in C# met een 3-lagen architectuur.

# 

# \---

# 

# \## Vereisten

# 

# | Software | Versie |

# |---|---|

# | Visual Studio | 2022 of nieuwer |

# | .NET | 8.0 of nieuwer |

# | Microsoft SQL Server | 2019 of nieuwer |

# | SQL Server Management Studio (SSMS) | optioneel maar aangeraden |

# 

# \---

# 

# \## Projectstructuur

# 

# ```

# MeerkeuzevragenEDG/

# ├── BusinessLaag/          Managers, modellen, interfaces, exceptions

# ├── DataLaag/              Repositories, FileReaders, FileReaderFactory

# ├── MeerkeuzeUI/           WPF vensters, mappers, UI-modellen

# ├── TestProjectBL/         xUnit unit tests

# ├── Documentatie/          Architectuurschema, klassendiagram, databankschema

# └── MeerkeuzevragenEDG.sln

# ```

# 

# De documentatie (architectuurschema, klassendiagram en databankschema) bevindt zich in de map `Documentatie`, die naast het solution-bestand staat.

# 

# \---

# 

# \## Opzetten van de applicatie

# 

# \### Stap 1 — Database aanmaken

# 

# Open SSMS en voer het script `Scripts/01CreateEmptyDB.sql` uit. Dit maakt de database `MeerkeuzevragentestsEDG` aan met alle tabellen.

# 

# > Voer dit script uit op een lege SQL Server instantie om conflicten met bestaande data te vermijden.

# 

# \### Stap 2 — Seeddata toevoegen

# 

# Voer daarna het script `Scripts/SeedData.sql` uit. Dit vult de database met een beginset van studenten, onderwerpen, vragen en antwoorden.

# 

# > \*\*Belangrijk:\*\* voer de seeddata pas uit nadat de database volledig is aangemaakt (stap 1). Als er al data in de tabellen staat kunnen de hardcoded Id-waarden conflicteren met bestaande rijen.

# 

# \### Stap 3 — Connection string instellen

# 

# Pas in het project `MeerkeuzeUI` het bestand `appsettings.json` aan:

# 

# ```json

# {

# &#x20; "ConnectionStrings": {

# &#x20;   "SqlConnection": "Server=JOUW\_SERVERNAAM;Database=MeerkeuzevragentestsEDG;Trusted\_Connection=True;TrustServerCertificate=True;"

# &#x20; }

# }

# ```

# 

# Vervang `JOUW\_SERVERNAAM` door de naam van jouw SQL Server instantie (bv. `localhost` of `.\\SQLEXPRESS`).

# 

# \### Stap 4 — Startup project instellen

# 

# Stel `MeerkeuzeUI` in als startup project in Visual Studio:

# 

# \- Rechtsklik op `MeerkeuzeUI` in de Solution Explorer

# \- Kies \*\*Set as Startup Project\*\*

# 

# \### Stap 5 — Applicatie starten

# 

# Druk op \*\*F5\*\* of klik op de groene afspeelknop. De applicatie start op met een keuze tussen de docentenomgeving en de studentenomgeving.

# 

# \---

# 

# \## Gebruik

# 

# \*\*Als docent\*\* — klik op "Docent" in het startscherm om toegang te krijgen tot:

# \- Tests genereren en exporteren naar `.txt`

# \- Bestaande testen bekijken met oplossingssleutel

# \- Scores en gemiddeldes per student en per test raadplegen

# \- Vragen toevoegen of deactiveren via "Vragen Beheren"



# 

# \*\*Als student\*\* — klik op "Student" in het startscherm, voer je naam en de testcode in die je van de docent hebt ontvangen, en leg de test af. Na het indienen zie je je score en feedback per vraag.

# 

# \---

# 

# \## Importeren van vragenbestanden

# 

# De applicatie ondersteunt twee bestandsformaten voor het importeren van vragen:

# 

# \- \*\*Type 1\*\* — vragen met `Correct: A` als antwoordsleutel per vraag

# \- \*\*Type 2\*\* — vragen met een aparte `Antwoorden`-sectie onderaan het bestand

# 

# Plaats de `.txt` bestanden in de map `MeerkeuzeUI/Resources/`. Ze worden automatisch ingeladen bij het opstarten van de docentenomgeving.

# 

# \---

# 

# \## Unit tests uitvoeren

# 

# Open de \*\*Test Explorer\*\* via `View > Test Explorer` en klik op \*\*Run All\*\*. De tests bevinden zich in het project `TestProjectBL` en testen de businesslaag zonder databaseverbinding.

# 

# \---

# 

# \## Auteur

# 

# Elisabeth De Gusseme — Graduaat Programmeren, HoGent — 2025-2026

