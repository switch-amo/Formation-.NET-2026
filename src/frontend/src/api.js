// Petit client d'accès à l'API PAS.
//
// L'API exige un JWT Keycloak sur tous les endpoints. En développement,
// l'endpoint /auth/token (mappé uniquement en environnement Development) échange
// des identifiants de test contre un token. On récupère donc un token, puis on
// appelle /funds avec l'en-tête Authorization.

const DEV_USER = 'testuser'
const DEV_PASSWORD = 'Test123!'

async function getToken() {
  const res = await fetch('/auth/token', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username: DEV_USER, password: DEV_PASSWORD }),
  })
  if (!res.ok) {
    throw new Error(
      `Authentification échouée (HTTP ${res.status}). Keycloak est-il démarré (via Aspire) ?`,
    )
  }
  const data = await res.json()
  if (!data.access_token) {
    throw new Error("Réponse d'authentification invalide : pas d'access_token.")
  }
  return data.access_token
}

export async function fetchFunds() {
  const token = await getToken()
  const res = await fetch('/funds', {
    headers: { Authorization: `Bearer ${token}` },
  })
  if (!res.ok) {
    throw new Error(`Impossible de récupérer les funds (HTTP ${res.status}).`)
  }
  return res.json()
}

export async function createFund({ name, isin, currency }) {
  const token = await getToken()
  const res = await fetch('/funds', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ name, isin, currency }),
  })
  if (!res.ok) {
    throw new Error(await extractError(res))
  }
  return res.json() // { id }
}

export async function addNav(fundId, { date, value }) {
  const token = await getToken()
  const res = await fetch(`/funds/${fundId}/nav`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ date, value }),
  })
  if (!res.ok) {
    throw new Error(await extractError(res))
  }
  // 204 No Content — pas de corps à lire
}

// Extrait un message lisible d'une réponse ProblemDetails (RFC 7807) :
// 400 => { errors: { Champ: [messages] } }, 422 => { detail: "..." }.
async function extractError(res) {
  try {
    const problem = await res.json()
    const fieldErrors = problem.errors ? Object.values(problem.errors).flat() : []
    if (fieldErrors.length) return fieldErrors.join(' ')
    if (problem.detail) return problem.detail
    if (problem.title) return problem.title
  } catch {
    // pas de corps JSON exploitable
  }
  return `Erreur (HTTP ${res.status}).`
}
