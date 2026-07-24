import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// L'API .NET n'expose pas de CORS : on relaie donc les appels via le proxy de Vite.
// Le navigateur ne parle qu'au serveur Vite (même origine), qui transfère vers l'API.
// Surchargez la cible avec la variable d'environnement VITE_API_TARGET si besoin.
const API_TARGET = process.env.VITE_API_TARGET || 'http://localhost:5074'

export default defineConfig({
  plugins: [react()],
  server: {
    // Aspire injecte le port à utiliser via la variable PORT ; sinon 5173 en local.
    host: true,
    port: Number(process.env.PORT) || 5173,
    proxy: {
      '/funds': { target: API_TARGET, changeOrigin: true },
      '/auth': { target: API_TARGET, changeOrigin: true },
    },
  },
})
