import { useEffect, useState } from 'react'

export default query => {
  const [mounted, setMounted] = useState(false)
  const [matches, setMatches] = useState(
    !mounted && window.matchMedia(query).matches
  )
  const handleMediaQueryStatusChange = e => setMatches(e.matches)
  useEffect(() => {
    setMounted(true)
    const matchQueryList = window.matchMedia(query)
    matchQueryList.addListener(handleMediaQueryStatusChange)
    return () => matchQueryList.removeListener(handleMediaQueryStatusChange)
  }, [query])
  return matches
}
