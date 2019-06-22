import { LOCATION_CHANGE } from 'connected-react-router'
import { matchPath } from 'react-router-dom'
import { takeEvery } from 'redux-saga/effects'

export function takeRouteActivate(path, handleRouteActivate) {
  return takeEvery(LOCATION_CHANGE, function*(action) {
    const { location } = action.payload
    const { pathname } = location
    if (matchPath(pathname, { path, exact: true })) {
      yield handleRouteActivate(action)
    }
  })
}
