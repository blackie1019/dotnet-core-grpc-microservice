import React from 'react';
import { Router } from 'react-router-dom';
import Routes from 'routes/Routes';
import history from 'utility/router/history.js';

const IndexRoute = () => (
  <Router history={history}>
    <Routes />
  </Router>
);

IndexRoute.displayName = 'IndexRoute';

export default IndexRoute;
