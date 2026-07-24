# PAS · Funds — Frontend

Petite interface React (Vite) pour afficher la liste des funds exposée par `PAS.Api`.

## Prérequis

- Le backend doit tourner (via **Aspire** de préférence, pour que **Keycloak** et **SQL Server** soient démarrés), l'API écoutant sur `http://localhost:5074`.
- Node 18+.

## Démarrage

### Option A — via Aspire (recommandé)

Le frontend est déclaré comme ressource dans l'AppHost (`PAS.Aspire`). Il suffit de
lancer l'AppHost : Aspire démarre SQL Server, Keycloak, l'API **puis** le frontend,
et lui injecte automatiquement l'URL de l'API (`VITE_API_TARGET`) et le port à utiliser.

```bash
dotnet run --project "src/backend/PAS.Aspire"
```

Le frontend apparaît alors dans le dashboard Aspire (endpoint cliquable).
Note : Aspire lance `npm run dev` mais **n'installe pas** les dépendances — faites
`npm install` une fois dans `src/frontend` au préalable.

### Option B — en autonome

```bash
npm install
npm run dev
```

Puis ouvrez http://localhost:5173.

## Comment ça marche

- L'API impose un JWT Keycloak. En développement, le front récupère un token via
  l'endpoint `POST /auth/token` (identifiants de test `testuser` / `Test123!`),
  puis appelle `GET /funds` avec l'en-tête `Authorization: Bearer …`.
- L'API n'expose pas de CORS : les appels `/funds` et `/auth` passent par le
  **proxy de Vite** (voir `vite.config.js`), donc le navigateur reste en même origine.

## Configuration

Si l'API n'écoute pas sur le port par défaut, surchargez la cible du proxy :

```bash
VITE_API_TARGET=http://localhost:XXXX npm run dev
```
