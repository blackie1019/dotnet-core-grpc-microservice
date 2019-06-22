import { useMemo } from 'react'
import useReduxState from '@app/Common/Hooks/useReduxState'
import hasPermission from './hasPermission'

export default (policy = false) => {
  const credential = useReduxState(state => state.authentication.credential)
  return useMemo(() => hasPermission(credential, policy), [credential])
}
