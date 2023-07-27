# Innafor 2023 - Hensetting - API

Dette prosjektet er ment som et mulig utgangspunkt for backend i utviklingen av en prototype til den tekniske delen av oppgaven i Innafor 2023. Prosjektet bruker C# som språk, ASP.NET Core som rammeverk, SQLite EF Core Database Provider for databasetilkobling og Swashbuckle for oversikt over endepunkter.

Start med å se på `Program.cs` for å få oversikt over prosjektet.

## Formål

Dette prosjektet er både ment å kunne brukes som den er for de som har lyst til å fokusere på brukergrensesnitt og som ikke trenger mer enn det som allerede er tilgjengelig her (selv om det ikke er ideelt representert og inndelt), og som utgangspunkt for de som har lyst til å utvikle backend i .NET som en del av løsningen sin.

Prosjektet er strukturert for å gjøre det lettest mulig å få oversikt og gjøre endringer i den korte tiden tilgjengelig til utvikling av prototype, selv for de som ikke har tidligere erfaring med C# og .NET.

Prosjektet tilgjengeligjør data hentet fra Excelarkene nevnt i oppgaven gjennom disse endepunktene:

- GET /spaces: Henter en oversikt over alle hensettingsplasser
- GET /spaces/{id}: Henter all data om en hensettingsplass
- GET /spaces/{id}/reservations: Henter en oversikt over alle reservasjoner på en gitt hensettingsplass
- GET /locations: Henter en oversikt over alle lokasjoner til hensettingsplasser
- GET /locations/{id}: Henter all data om en lokasjon med hensettingsplasser
- GET /reservations: Henter en oversikt over alle reservasjoner av hensettingsplasser
- POST /reservations: Oppretter en ny reservasjon
- GET /reservations/{id}: Henter all data om en reservasjon

## Kjøre prosjektet

Prosjektet krever dotnet 7 for å kjøres. Dotnet blir automatisk lastet ned sammen med Visual Studio og Rider (og VSCode skal også hjelpe med å installere det når man åpner et dotnet prosjekt). Eventuelt så kan dotnet lastes ned fra microsoft sine sider https://dotnet.microsoft.com/en-us/download

Plasser databasefilen `innanor.db` i rotmappen til prosjektet for å bruke eksempeldata hentet fra excelarkene. Eventuelt så kan kommandoen `dotnet ef database update`[^1] kjøres i mappen `InnaNor.API` for å generere en tom `innanor.db` database.

Når databasen er på plass er det bare å kjøre prosjektet gjennom valgt IDE eller bare kjøre

```bash
dotnet run
```

i `InnaNor.API` mappen.

[^1]: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
