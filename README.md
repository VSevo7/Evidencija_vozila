# Evidencija vozila

ASP.NET Core MVC aplikacija za internu evidenciju korisnika, službenih vozila i naloga za korištenje vozila.

## Glavne funkcionalnosti

- prijava i odjava korisnika
- upravljanje korisnicima
- evidencija vozila i pregled detalja vozila
- kreiranje i završavanje naloga
- pretraga vozila po registracijskoj oznaci
- pretraga naloga po broju naloga

Korisnike u sustav dodaje administrator. Samostalna registracija nije omogućena.

## Tehnologije

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB

## Baza podataka

Connection string nalazi se u datoteci `EvidencijaVozila/appsettings.json`.

Projekt koristi EF Core migracije i primjenjuje ih automatski pri pokretanju aplikacije.

Ako želiš krenuti s praznom bazom, promijeni naziv baze u connection stringu ili obriši postojeću LocalDB bazu s tim nazivom.

## Pokretanje

```powershell
cd EvidencijaVozila
dotnet restore
dotnet run
```

Zadana adresa u razvojnom okruženju najčešće je `http://localhost:5226`.

## Početni administratorski račun

- korisničko ime: `admin`
- lozinka: `Admin123!`
