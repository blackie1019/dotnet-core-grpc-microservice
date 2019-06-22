import { useRef } from 'react'

export default (factory, ...params) => {
  const instance = useRef()
  if (!instance.current) instance.current = factory(...params)
  return instance.current
}
