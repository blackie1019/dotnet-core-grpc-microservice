import React from 'react';
import { render } from 'react-dom';

import 'assets/scss/material-dashboard-pro-react.css?v=1.4.0';
import * as serviceWorker from './serviceWorker';

const rootElement = document.getElementById('root');

const renderApp = element => {
  const App = require('./App.jsx').default;
  render(<App />, element);
};

renderApp(rootElement);

if (process.env.NODE_ENV !== 'production' && module.hot) {
  module.hot.accept('./App.jsx', () => renderApp(rootElement));
}

serviceWorker.unregister();
