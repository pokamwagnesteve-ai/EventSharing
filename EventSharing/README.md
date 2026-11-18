# EventSharing

Application web .NET 10 permettant de publier des événements, de les classer par catégorie et de gérer les participations.

## Fonctionnalités

- inscription, connexion et gestion de compte avec ASP.NET Core Identity ;
- consultation des événements à venir et filtrage par catégorie ;
- création, modification et suppression d'un événement par son créateur ;
- inscription et désinscription à un événement ;
- gestion des catégories pour les utilisateurs authentifiés ;
- validation des dates et protection antiforgery des formulaires.

## Prérequis

- .NET SDK 10 ;
- SQL Server LocalDB (configuration par défaut) ou une instance SQL Server ;
- Visual Studio 2026 ou un éditeur compatible.

## Installation

```powershell
git clone <url-du-depot>
cd EventSharing
dotnet restore
dotnet build
```

La chaîne de connexion par défaut se trouve dans `appsettings.json`. Pour éviter de la versionner dans un environnement partagé, utilisez les User Secrets :

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<chaine-de-connexion>"
```

## Base de données

Appliquer les migrations avec :

```powershell
dotnet ef database update --project EventSharing\EventSharing.csproj --startup-project EventSharing\EventSharing.csproj
```

Créer une nouvelle migration après une modification des entités :

```powershell
dotnet ef migrations add NomDeLaMigration --project EventSharing\EventSharing.csproj --startup-project EventSharing\EventSharing.csproj
```

Le modèle Identity utilise `EventSharing.Models.User`. Les événements ont une catégorie, un créateur et une relation plusieurs-à-plusieurs avec leurs participants.

## Lancer l'application

```powershell
dotnet run --project EventSharing\EventSharing.csproj
```

Les URL locales sont indiquées par le profil de lancement Visual Studio. En développement, l'application expose la page de diagnostic des migrations ; cette fonctionnalité ne doit pas être activée en production.

## Architecture

- `Program.cs` : composition des services et pipeline HTTP ;
- `Data/ApplicationDbContext.cs` : contexte EF Core et relations ;
- `Models/` : entités `User`, `Event` et `Category` ;
- `Controllers/` : endpoints MVC et règles d'autorisation ;
- `Views/` : interface MVC Bootstrap ;
- `Areas/Identity/` : point d'intégration Razor Pages Identity ;
- `Data/Migrations/` : historique du schéma SQL Server.

L'interface principale est MVC. Razor Pages est conservé pour les écrans Identity fournis par ASP.NET Core.

## Règles de sécurité

- les opérations de création et de participation exigent une authentification ;
- seul le créateur d'un événement peut le modifier ou le supprimer ;
- les identifiants du créateur et des participants sont déterminés côté serveur ;
- les formulaires POST utilisent l'antiforgery token ;
- les exceptions détaillées ne doivent pas être exposées en production.

## Documentation complémentaire

Le parcours utilisateur est décrit dans [`docs/Guide-utilisateur.md`](docs/Guide-utilisateur.md). Les types et contrôleurs principaux disposent également de commentaires XML ; le fichier XML est généré dans le dossier de sortie lors de la compilation.
