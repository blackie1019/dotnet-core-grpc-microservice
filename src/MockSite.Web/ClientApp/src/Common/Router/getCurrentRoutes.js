import { matchPath } from 'react-router'

export default function getCurrentRoutes(path, routes) {
  const current = []
  for (let route of routes) {
    if (route.path && !matchPath(path, { path: route.path })) continue
    current.push({
      displayKey: route.displayKey,
      path: route.path
    })
    if (route.routes) {
      current.push(...getCurrentRoutes(path, route.routes))
    }
  }
  return current
}
