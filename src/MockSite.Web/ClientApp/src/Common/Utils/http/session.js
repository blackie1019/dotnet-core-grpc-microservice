import decodeJWT from 'jwt-decode'

const CREDENTIAL_KEY = 'credential'

let _credential

const setCredential = credential => {
  if (!credential) localStorage.removeItem(CREDENTIAL_KEY)
  else localStorage.setItem(CREDENTIAL_KEY, btoa(JSON.stringify(credential)))
  return (_credential = credential)
}

export const getCredential = () => _credential

export const saveCredential = credential => {
  try {
    const token = decodeJWT(credential.token)
    const expiryTime = token.exp * 1000
    const expired = expiryTime <= Date.now()
    if (!expired) return setCredential({ ...credential, expiryTime })
  } catch {
    /* eslint-disable no-empty */
  }
  return clearCredential()
}

export const restoreCredential = () => {
  const rawCredential = localStorage.getItem(CREDENTIAL_KEY)
  try {
    const credential = JSON.parse(atob(rawCredential))
    if (credential.expiryTime > Date.now()) return (_credential = credential)
  } catch {
    /* eslint-disable no-empty */
  }
  return clearCredential()
}

export const lockCredential = () =>
  setCredential({ ..._credential, locking: true })

export const clearCredential = () => setCredential()
