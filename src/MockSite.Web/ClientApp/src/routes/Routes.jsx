import React from 'react';
import { Route, Switch } from 'react-router-dom';
import Pages from 'layouts/Pages.jsx';
import RTL from 'layouts/RTL.jsx';
import Dashboard from 'layouts/Dashboard.jsx';

const routes = [
  { path: '/rtl', name: 'RTL', component: RTL },
  { path: '/pages', name: 'Pages', component: Pages },
  { path: '/', name: 'Home', component: Dashboard }
];

const Routes = () => (
  <Switch>
    {routes.map((prop, key) => {
      return <Route path={prop.path} component={prop.component} key={key} />;
    })}
  </Switch>
);

Routes.displayName = 'Routes';

export default Routes;
