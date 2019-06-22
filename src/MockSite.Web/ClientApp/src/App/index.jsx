import React from 'react'
import { Provider } from 'react-redux'
import store from './configs/store'
import AppInit from './AppInit'

export default function App() {
  return (
    <Provider store={store}>
      <AppInit />
    </Provider>
  )
}
