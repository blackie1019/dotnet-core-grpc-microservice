import getCurrentRoutes from './getCurrentRoutes'

export default function getBreadcrumbs(path, routes) {
  return getCurrentRoutes(path, routes).filter(
    route => route.path && !route.path.match(/^\/[^/]+$/)
  )
}
