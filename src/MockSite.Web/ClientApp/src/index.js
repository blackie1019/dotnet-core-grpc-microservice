import React from 'react';
import { render } from 'react-dom';
import { AppContainer } from 'react-hot-loader';

import 'assets/scss/material-dashboard-pro-react.css?v=1.4.0';
import * as serviceWorker from './serviceWorker';

const rootElement = document.getElementById('root');

const renderApp = element => {
  const App = require('./App.jsx').default;
  render(
    <AppContainer>
      <App />
    </AppContainer>,
    element
  );
};

renderApp(rootElement);

if (process.env.NODE_ENV !== 'production' && module.hot) {
  module.hot.accept('./App.jsx', () => renderApp(rootElement));
}

serviceWorker.unregister();
