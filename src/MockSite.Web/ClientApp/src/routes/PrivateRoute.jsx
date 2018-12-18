import React from 'react';
import { connect } from 'react-redux';
import { Route, Redirect } from 'react-router-dom';

const PrivateRoute = ({ component: Component, authenticated }) => {
    return (
        <Route
            render={props =>
                authenticated ? (
                    <Component {...props} />
                ) : (
                    <Redirect to="/authorization" />
                )
            }
        />
    );
};

PrivateRoute.displayName = 'PrivateRoute';

export default connect(state => ({
    authenticated: !!state.authentication.token
}))(PrivateRoute);