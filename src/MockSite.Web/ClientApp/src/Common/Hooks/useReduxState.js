import { identity } from 'ramda'
import { useContext, useEffect, useState } from 'react'
import { ReactReduxContext } from 'react-redux'

export default (selector = identity) => {
  const { store, subscription: sub } = useContext(ReactReduxContext)
  const [state, setState] = useState(selector(store.getState()))

  useEffect(
    () => sub.addNestedSub(() => setState(selector(store.getState()))),
    []
  )

  return state
}
