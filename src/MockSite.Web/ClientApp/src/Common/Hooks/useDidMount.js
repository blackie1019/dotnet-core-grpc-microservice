import { useEffect } from 'react'

export default didMount =>
  useEffect(() => {
    didMount()
  }, [])
