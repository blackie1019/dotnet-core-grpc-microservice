import React from 'react';
import { Provider } from 'react-redux';
import ErrorBoundary from './ErrorBoundary.jsx';
import store from 'utility/redux/store';
import IndexRoute from 'routes/IndexRoute';

const App = () => (
  <Provider store={store}>
    <ErrorBoundary>
      <IndexRoute />
    </ErrorBoundary>
  </Provider>
);

App.displayName = 'App';

export default App;
