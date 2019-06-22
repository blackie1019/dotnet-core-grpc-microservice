import React from 'react'
import { render } from 'react-dom'
import App from '@app/App'
import * as serviceWorker from '@app/Common/Utils/serviceWorker'

const rootElement = document.getElementById('root')
const renderApp = () => render(<App />, rootElement)

renderApp()
if (app.env.ENABLE_HOT_RELOAD) {
  module.hot?.accept('@app/App', renderApp)
}

if (app.env.ENABLE_SERVICE_WORKER) {
  serviceWorker.register({ url: app.env.SERVICE_WORKER_URL })
}

/* eslint-disable-next-line no-console */
console.log(`Revision Number: ${app.env.REVISION_NUMBER}`)
