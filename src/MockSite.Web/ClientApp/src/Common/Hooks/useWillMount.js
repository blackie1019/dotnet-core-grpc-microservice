import { useRef } from 'react'

export default willMount => {
  const didMount = useRef()
  if (!didMount.current) {
    willMount()
    didMount.current = true
  }
}
