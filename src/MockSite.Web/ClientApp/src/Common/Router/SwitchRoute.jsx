import {
  array,
  arrayOf,
  bool,
  func,
  object,
  oneOfType,
  shape,
  string
} from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import { Redirect, Route, Switch } from 'react-router'
import hasPermission from '@app/Common/Authentication/hasPermission'

function callOrIdentity(fn, ...params) {
  if (typeof fn === 'function') return fn(...params)
  return fn
}

function SwitchRoute({ location, credential, parent, routes, fallback }) {
  if (!routes) return null
  fallback = fallback || parent?.notFoundFallback
  return (
    <Switch location={location}>
      {routes.map((route, i) => {
        const {
          key,
          path = parent?.path,
          exact = true,
          strict,
          policy,
          forbiddenFallback = '/',
          notFoundFallback = fallback,
          component: Component,
          render,
          routes
        } = route
        return (
          <Route key={key || i} path={path} exact={exact} strict={strict}>
            {props => {
              const { match } = props
              if (!match) return null

              if (!hasPermission(credential, policy)) {
                const to = callOrIdentity(forbiddenFallback, credential)
                return <Redirect to={to} />
              } else if (render) {
                return render({ ...props, route })
              } else if (Component) {
                return <Component {...props} route={route} />
              } else if (routes) {
                return (
                  <SwitchRoute
                    credential={credential}
                    parent={route}
                    routes={routes}
                    fallback={notFoundFallback}
                  />
                )
              }
              return null
            }}
          </Route>
        )
      })}
      {fallback && <Redirect to={fallback} />}
    </Switch>
  )
}

SwitchRoute.propTypes = {
  credential: object,
  parent: shape({
    path: string,
    notFoundFallback: string
  }),
  routes: arrayOf(
    shape({
      key: string,
      path: string,
      exact: bool,
      strict: bool,
      policy: oneOfType([array, bool, func]),
      forbiddenFallback: oneOfType([func, string]),
      notFoundFallback: string,
      render: func,
      component: oneOfType([func, object])
    }).isRequired
  ),
  fallback: string
}

export default connect(state => ({
  credential: state.authentication.credential,
  location: state.router.location
}))(SwitchRoute)
