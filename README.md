# Evidencija sluzbenih vozila

Studentska web aplikacija za evidenciju sluzbenih vozila, korisnika i naloga za upotrebu vozila.

## Tehnologije

- C#
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap 5

## Struktura

- `EvidencijaVozila.sln` solution datoteka
- `EvidencijaVozila/` glavni MVC projekt
- `Controllers/` kontroleri aplikacije
- `Models/` entiteti baze
- `ViewModels/` modeli za forme i prikaze
- `Views/` Razor stranice
- `Data/` `DbContext`, inicijalizacija i pomoćne metode

## Funkcionalnosti

- prijava korisnika s ulogama `Administrator` i `Korisnik`
- dashboard sa stanjem sustava
- upravljanje korisnicima
- upravljanje vozilima
- kreiranje i zakljucivanje naloga
- pregled dostupnosti vozila po terminu
- izvjestaji o koristenju i servisnim intervalima

## Pokretanje

1. Otvori mapu ili solution `EvidencijaVozila.sln`.
2. U terminalu idi u `EvidencijaVozila/`.
3. Pokreni `dotnet build`.
4. Pokreni `dotnet run`.
5. Otvori adresu koju aplikacija ispise u terminalu.

## Baza

Aplikacija koristi `SQL Server LocalDB` i pri prvom pokretanju sama kreira bazu `EvidencijaVozilaDb` te unosi pocetne podatke.

Ako `LocalDB` nije instaliran, u `appsettings.json` promijeni connection string na svoju `SQL Server Express` instancu.

## Demo korisnici

- administrator: `admin`
- korisnik: `iivic`
- korisnik: `aanic`
- lozinka za sve demo racune: `Admin123!`
