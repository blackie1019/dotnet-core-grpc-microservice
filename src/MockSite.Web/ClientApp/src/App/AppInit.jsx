import { push as gotoPage } from 'connected-react-router'
import { bool, func } from 'prop-types'
import React, { useEffect } from 'react'
import Hotkeys from 'react-hot-keys'
import { connect } from 'react-redux'
import authActions from '@app/Common/Authentication/action'
import Dialog from '@app/Common/Dialog/Dialog'
import ErrorBoundary from '@app/Common/ErrorHandler/ErrorBoundary'
import AppRouter from '@app/Common/Router/AppRouter'
import Snackbar from '@app/Common/Snackbar/Snackbar'
import history from './configs/history'
import routes from './configs/routes'
import paths from './constants/paths'
import GlobalStyle from './styles/GlobalStyle'

function AppInit({ didAuthInit, initAuth, gotoLockScreenPage }) {
  useEffect(() => {
    initAuth()
  }, [])

  if (!didAuthInit) return null
  return (
    <ErrorBoundary>
      <GlobalStyle />
      <Dialog />
      <Snackbar />
      <Hotkeys keyName="⌘+⇧+l, ⌃+⇧+l" onKeyDown={gotoLockScreenPage} />
      <AppRouter history={history} routes={routes} />
    </ErrorBoundary>
  )
}

AppInit.propTypes = {
  didAuthInit: bool.isRequired,
  initAuth: func.isRequired,
  gotoLockScreenPage: func.isRequired
}

export default connect(
  state => ({
    didAuthInit: state.authentication.didInit
  }),
  {
    initAuth: authActions.init,
    gotoLockScreenPage: () => gotoPage(paths.LOCK_SCREEN)
  }
)(AppInit)
