import { ConnectedRouter } from 'connected-react-router'
import { array, object } from 'prop-types'
import React from 'react'
import SwitchRoute from './SwitchRoute'

export default function AppRouter({ history, routes }) {
  return (
    <ConnectedRouter history={history}>
      <SwitchRoute routes={routes} />
    </ConnectedRouter>
  )
}

AppRouter.propTypes = {
  history: object.isRequired,
  routes: array.isRequired
}
