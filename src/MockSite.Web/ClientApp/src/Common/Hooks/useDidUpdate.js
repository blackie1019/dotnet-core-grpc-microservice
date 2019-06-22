import { useEffect, useRef } from 'react'

export default (didUpdate, deps) => {
  const didMount = useRef()
  useEffect(() => {
    if (didMount.current) didUpdate()
    else didMount.current = true
  }, deps)
}
