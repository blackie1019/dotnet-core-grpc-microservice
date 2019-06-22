import { useMemo } from 'react'

export default (fn, ...args) => useMemo(() => fn(...args), args)
