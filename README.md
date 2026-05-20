# Evidencija vozila

ASP.NET Core MVC aplikacija za internu evidenciju korisnika, službenih vozila i naloga za korištenje vozila.

## Funkcionalnosti

- prijava i odjava korisnika
- administracija korisnika
- evidencija vozila
- pregled detalja vozila
- kreiranje i završavanje naloga
- pretraga vozila po registracijskoj oznaci
- pretraga naloga po broju naloga

Korisnike u sustav unosi administrator. Ne postoji samostalna registracija korisnika.

## Tehnologije

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB

## Baza podataka

Veza prema bazi nalazi se u [appsettings.json](C:\Users\Korisnik\OneDrive\Radna povrsina\Nova mapa\EvidencijaVozila\appsettings.json).

Projekt koristi EF Core migracije. Pri pokretanju aplikacije migracije se primjenjuju automatski.

Ako želiš potpuno praznu bazu za novi unos podataka, promijeni naziv baze u connection stringu ili obriši postojeću LocalDB bazu s tim nazivom.

## Pokretanje

```powershell
cd "C:\Users\Korisnik\OneDrive\Radna povrsina\Nova mapa\EvidencijaVozila"
dotnet restore
dotnet run
```

Zadana adresa u razvojnom okruženju je najčešće `http://localhost:5226`.

## Početni administratorski račun

- korisničko ime: `admin`
- lozinka: `Admin123!`
