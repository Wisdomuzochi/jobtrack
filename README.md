# JobTrack

Application de suivi personnel de candidatures — API REST .NET & interface Angular.

Projet d'entraînement réalisé en complément de MiniDoc, en préparation d'une
alternance Développeur .NET. Contrairement à MiniDoc (une seule entité), JobTrack
a été pensé pour pratiquer les **relations Entity Framework Core** (one-to-many)
sur un vrai besoin personnel : centraliser mes candidatures, les compétences
demandées, les contacts trouvés, et savoir quand relancer.

## Stack technique

**Backend**
- C# / .NET 8 — ASP.NET Core Web API
- Entity Framework Core 8 + SQLite
- xUnit — tests unitaires et d'intégration

**Frontend**
- Angular 21 (composants standalone)
- TypeScript, RxJS (Observables)
- Reactive Forms avec FormArray (champs dynamiques)

**Outils**
- Git / GitHub, workflow par branches et Pull Requests
- Swagger / OpenAPI

## Architecture

jobtrack/
├── src/
│ ├── JobTrack.Api/
│ │ ├── Controllers/ # CandidaturesController
│ │ ├── Services/ # CandidatureService (logique métier)
│ │ ├── Models/ # Candidature, CompetenceRequise, Contact
│ │ ├── Data/ # JobTrackDbContext
│ │ ├── Migrations/ # Historique du schéma de base
│ │ └── Program.cs
│ └── JobTrack.Web/
│ └── src/app/
│ ├── components/
│ │ ├── dashboard/ # Vue d'ensemble + liste
│ │ ├── candidature-create/ # Formulaire de création
│ │ └── shared/confirm-dialog/
│ ├── services/
│ │ └── candidatures.service.ts
│ └── models/
│ └── candidature.model.ts
└── tests/
└── JobTrack.Api.Tests/

## Modèle de données

Candidature (entité centrale)
├── Poste, Entreprise, LienOffre
├── DatePublicationOffre, DateCandidature
├── Statut (Envoyee | EnCours | Entretien | Refuse | Accepte)
├── RelanceConseillee (propriété calculée : Envoyee depuis plus de 15 jours)
│
├── CompetencesRequises (1 → N)
└── Contacts (1 → N)


## Fonctionnalités

- Création d'une candidature avec compétences et contacts ajoutés dynamiquement
- Tableau de bord avec statistiques (total, en cours, entretiens, relances conseillées)
- Changement de statut via sélecteur, limité aux transitions logiques
- Suppression avec confirmation (modale personnalisée)
- Calcul automatique des candidatures à relancer

## Lancer le projet

### Backend

```bash
cd src/JobTrack.Api
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend

Dans un second terminal :

```bash
cd src/JobTrack.Web
npm install
ng serve
```

Interface sur `http://localhost:4200`. CORS configuré côté API pour autoriser
uniquement cette origine.

## Lancer les tests

```bash
cd src/JobTrack.Api    # la solution référence les deux projets
dotnet test
```

18 tests (unitaires et d'intégration), écrits en TDD.

## Endpoints API

| Méthode | Route                     | Description                              |
|---------|-----------------------------|---------------------------------------------|
| POST    | /api/candidatures           | Créer une candidature avec compétences/contacts |
| GET     | /api/candidatures            | Lister toutes les candidatures              |
| PUT     | /api/candidatures/{id}       | Changer le statut                           |
| DELETE  | /api/candidatures/{id}       | Supprimer une candidature                   |

## Décisions techniques notables

- **Relations EF Core one-to-many** (`Candidature` → `CompetenceRequise`,
  `Candidature` → `Contact`) avec suppression en cascade — supprimer une
  candidature supprime automatiquement ses compétences et contacts liés.
- **`ReferenceHandler.IgnoreCycles`** sur la sérialisation JSON : nécessaire
  à cause de la relation bidirectionnelle (`CompetenceRequise.Candidature` ↔
  `Candidature.CompetencesRequises`), qui provoquait un cycle infini de
  sérialisation sans cette configuration.
- **`RelanceConseillee` en propriété calculée** (pas stockée en base) :
  toujours cohérente avec la date actuelle, sans колonne dédiée ni mise à jour
  manuelle nécessaire.
- **`Include`/`ThenInclude` systématique** sur les lectures : sans ça, EF Core
  ne charge jamais les collections liées par défaut (comportement volontaire
  du framework, pour éviter de charger inutilement des données).

## Ce qui n'est volontairement pas couvert

- Authentification (application personnelle mono-utilisateur)
- CI/CD (non mis en place, contrairement à MiniDoc, faute de temps)
- Édition d'une candidature existante (seule la création et le changement de statut sont supportés)