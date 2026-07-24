import { Fragment, useCallback, useEffect, useState } from 'react'
import { addNav, createFund, fetchFunds } from './api'

const STATUS_LABELS = {
  Active: 'Actif',
  Suspended: 'Suspendu',
  Closed: 'Clôturé',
}

function formatMoney(value, currency) {
  try {
    return new Intl.NumberFormat('fr-FR', { style: 'currency', currency }).format(value)
  } catch {
    return `${value} ${currency}`
  }
}

function formatDate(isoDate) {
  if (!isoDate) return '—'
  const d = new Date(isoDate)
  return Number.isNaN(d.getTime()) ? isoDate : d.toLocaleDateString('fr-FR')
}

function todayIso() {
  return new Date().toISOString().slice(0, 10)
}

// Mini graphe en ligne (SVG maison, sans dépendance). `navs` trié par date croissante.
function NavChart({ navs }) {
  if (!navs || navs.length === 0) return null

  const width = 600
  const height = 160
  const padX = 8
  const padY = 16
  const n = navs.length
  const values = navs.map((nav) => nav.value)
  const min = Math.min(...values)
  const max = Math.max(...values)
  const span = max - min || 1

  const x = (i) => (n === 1 ? width / 2 : padX + (i * (width - 2 * padX)) / (n - 1))
  const y = (v) => height - padY - ((v - min) / span) * (height - 2 * padY)

  const line = navs.map((nav, i) => `${x(i)},${y(nav.value)}`).join(' ')
  const area = `M ${x(0)},${height - padY} L ${line.split(' ').join(' L ')} L ${x(n - 1)},${height - padY} Z`

  return (
    <svg className="chart" viewBox={`0 0 ${width} ${height}`} role="img" aria-label="Évolution de la VL">
      <path className="chart__area" d={area} />
      <polyline className="chart__line" points={line} vectorEffect="non-scaling-stroke" />
      {navs.map((nav, i) => (
        <circle key={nav.date} className="chart__dot" cx={x(i)} cy={y(nav.value)} r="3">
          <title>{`${formatDate(nav.date)} · ${nav.value}`}</title>
        </circle>
      ))}
    </svg>
  )
}

function AddNavForm({ fundId, onAdded }) {
  const [date, setDate] = useState(todayIso())
  const [value, setValue] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState(null)

  async function handleSubmit(e) {
    e.preventDefault()
    const num = Number(value)
    if (!value.trim() || Number.isNaN(num) || num <= 0) {
      setError('La valeur doit être un nombre strictement positif.')
      return
    }
    if (!date) {
      setError('La date est requise.')
      return
    }
    if (date > todayIso()) {
      setError('La date ne peut pas être dans le futur.')
      return
    }
    setSubmitting(true)
    setError(null)
    try {
      await addNav(fundId, { date, value: num })
      setValue('')
      onAdded()
    } catch (err) {
      setError(err.message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form className="nav-form" onSubmit={handleSubmit}>
      <div className="nav-form__row">
        <label className="field">
          <span>Date</span>
          <input
            type="date"
            value={date}
            max={todayIso()}
            onChange={(e) => setDate(e.target.value)}
          />
        </label>
        <label className="field">
          <span>Valeur (VL)</span>
          <input
            type="number"
            min="0"
            step="0.0001"
            value={value}
            onChange={(e) => setValue(e.target.value)}
            placeholder="128.42"
          />
        </label>
        <button type="submit" className="btn btn--sm" disabled={submitting}>
          {submitting ? 'Ajout…' : 'Ajouter la VL'}
        </button>
      </div>
      {error && <p className="form-error">{error}</p>}
    </form>
  )
}

function NavHistory({ fund, onChanged }) {
  const desc = fund.navs ?? []
  const asc = [...desc].reverse()
  const isClosed = fund.status === 'Closed'

  return (
    <div className="detail">
      <div className="detail__top">
        {desc.length > 0 ? (
          <>
            <div className="detail__chart">
              <NavChart navs={asc} />
            </div>
            <table className="subtable">
              <thead>
                <tr>
                  <th>Date</th>
                  <th className="num">VL</th>
                  <th className="num">Variation</th>
                </tr>
              </thead>
              <tbody>
                {desc.map((nav, i) => {
                  const prev = desc[i + 1]
                  const variation = prev && prev.value !== 0 ? (nav.value - prev.value) / prev.value : null
                  const cls = variation == null ? '' : variation >= 0 ? 'up' : 'down'
                  return (
                    <tr key={nav.date}>
                      <td>{formatDate(nav.date)}</td>
                      <td className="num">{formatMoney(nav.value, fund.currency)}</td>
                      <td className={`num ${cls}`}>
                        {variation == null
                          ? '—'
                          : `${variation >= 0 ? '+' : ''}${(variation * 100).toFixed(2)} %`}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </>
        ) : (
          <p className="detail__empty">Aucune VL enregistrée.</p>
        )}
      </div>

      {isClosed ? (
        <p className="detail__empty">Fund clôturé : l'ajout de VL est désactivé.</p>
      ) : (
        <AddNavForm fundId={fund.id} onAdded={onChanged} />
      )}
    </div>
  )
}

// Validation côté client, alignée sur les règles du backend (Isin/Currency + validators).
function validateFund({ name, isin, currency }) {
  const trimmedName = name.trim()
  if (!trimmedName) return 'Le nom est requis.'
  if (trimmedName.length > 200) return 'Le nom ne peut pas dépasser 200 caractères.'

  const isinUpper = isin.trim().toUpperCase()
  if (isinUpper.length !== 12) return "L'ISIN doit faire exactement 12 caractères."
  if (!/^[A-Z]{2}[A-Z0-9]{10}$/.test(isinUpper))
    return "L'ISIN doit commencer par 2 lettres (code pays) puis être alphanumérique."

  if (!/^[A-Z]{3}$/.test(currency.trim().toUpperCase()))
    return 'La devise doit être un code ISO à 3 lettres (ex. EUR).'

  return null
}

function AddFundForm({ onCreated, onCancel }) {
  const [name, setName] = useState('')
  const [isin, setIsin] = useState('')
  const [currency, setCurrency] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState(null)

  async function handleSubmit(e) {
    e.preventDefault()
    const validationError = validateFund({ name, isin, currency })
    if (validationError) {
      setError(validationError)
      return
    }
    setSubmitting(true)
    setError(null)
    try {
      await createFund({
        name: name.trim(),
        isin: isin.trim().toUpperCase(),
        currency: currency.trim().toUpperCase(),
      })
      onCreated()
    } catch (err) {
      setError(err.message)
      setSubmitting(false)
    }
  }

  return (
    <form className="form-card" onSubmit={handleSubmit}>
      <h2 className="form-card__title">Nouveau fund</h2>
      <div className="form-grid">
        <label className="field">
          <span>Nom</span>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            maxLength={200}
            placeholder="Global Equity Fund"
            autoFocus
          />
        </label>
        <label className="field">
          <span>ISIN</span>
          <input
            value={isin}
            onChange={(e) => setIsin(e.target.value)}
            maxLength={12}
            placeholder="LU0000000001"
            className="input-mono"
          />
        </label>
        <label className="field field--sm">
          <span>Devise</span>
          <input
            value={currency}
            onChange={(e) => setCurrency(e.target.value)}
            maxLength={3}
            placeholder="EUR"
          />
        </label>
      </div>
      {error && <p className="form-error">{error}</p>}
      <div className="form-actions">
        <button type="submit" className="btn" disabled={submitting}>
          {submitting ? 'Création…' : 'Créer le fund'}
        </button>
        <button type="button" className="btn btn--ghost" onClick={onCancel} disabled={submitting}>
          Annuler
        </button>
      </div>
    </form>
  )
}

export default function App() {
  const [funds, setFunds] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [expandedId, setExpandedId] = useState(null)
  const [showForm, setShowForm] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      setFunds(await fetchFunds())
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    load()
  }, [load])

  const toggle = (id) => setExpandedId((current) => (current === id ? null : id))

  return (
    <div className="app">
      <header className="app__header">
        <div>
          <h1>Funds</h1>
          <p className="app__subtitle">Policy Administration System</p>
        </div>
        <div className="app__actions">
          <button className="btn btn--ghost" onClick={() => setShowForm((s) => !s)}>
            {showForm ? 'Fermer' : '+ Ajouter un fund'}
          </button>
          <button className="btn" onClick={load} disabled={loading}>
            {loading ? 'Chargement…' : 'Rafraîchir'}
          </button>
        </div>
      </header>

      {showForm && (
        <AddFundForm
          onCreated={() => {
            setShowForm(false)
            load()
          }}
          onCancel={() => setShowForm(false)}
        />
      )}

      {error && (
        <div className="alert">
          <strong>Erreur.</strong> {error}
        </div>
      )}

      {!error && loading && <p className="muted">Chargement des funds…</p>}

      {!error && !loading && funds.length === 0 && (
        <p className="muted">Aucun fund pour le moment.</p>
      )}

      {!error && funds.length > 0 && (
        <>
          <p className="muted">
            {funds.length} fund{funds.length > 1 ? 's' : ''} · cliquez sur une ligne pour l'historique
          </p>
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Nom</th>
                  <th>ISIN</th>
                  <th>Devise</th>
                  <th>Statut</th>
                  <th className="num">Dernière VL</th>
                  <th>Date VL</th>
                </tr>
              </thead>
              <tbody>
                {funds.map((fund) => {
                  const latest = fund.navs?.[0] ?? null
                  const open = expandedId === fund.id
                  return (
                    <Fragment key={fund.id}>
                      <tr
                        className="row row--clickable"
                        onClick={() => toggle(fund.id)}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault()
                            toggle(fund.id)
                          }
                        }}
                        role="button"
                        tabIndex={0}
                        aria-expanded={open}
                      >
                        <td className="strong">
                          <span className="caret">{open ? '▾' : '▸'}</span>
                          {fund.name}
                        </td>
                        <td className="mono">{fund.isin}</td>
                        <td>{fund.currency}</td>
                        <td>
                          <span className={`badge badge--${(fund.status || '').toLowerCase()}`}>
                            {STATUS_LABELS[fund.status] ?? fund.status}
                          </span>
                        </td>
                        <td className="num">
                          {latest ? formatMoney(latest.value, fund.currency) : '—'}
                        </td>
                        <td>{latest ? formatDate(latest.date) : '—'}</td>
                      </tr>
                      {open && (
                        <tr className="detail-row">
                          <td className="detail-cell" colSpan={6}>
                            <NavHistory fund={fund} onChanged={load} />
                          </td>
                        </tr>
                      )}
                    </Fragment>
                  )
                })}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}
