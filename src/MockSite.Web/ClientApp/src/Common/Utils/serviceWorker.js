const isLocalhost = () => {
  const { hostname } = window.location
  return (
    hostname === 'localhost' ||
    hostname === '[::1]' ||
    !!hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/)
  )
}

const validateServiceWorker = async url => {
  const response = await fetch(url)
  const contentType = response.headers.get('content-type')
  if (response.status === 404 || !contentType?.includes('javascript')) {
    const registration = await navigator.serviceWorker.ready
    await registration.unregister()
    window.location.reload()
  } else {
    return true
  }
}

export const register = async ({ url }) => {
  if (!('serviceWorker' in navigator)) return
  await new Promise(resolve =>
    window.addEventListener('load', async () => {
      if (isLocalhost) {
        const isValid = await validateServiceWorker(url)
        if (!isValid) return resolve()
      }
      await navigator.serviceWorker.register(url)
      resolve()
    })
  )
}

export const unregister = async () => {
  if (!('serviceWorker' in navigator)) return
  const registration = await navigator.serviceWorker.ready
  await registration.unregister()
}
