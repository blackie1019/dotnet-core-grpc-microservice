import getCurrentRoutes from './getCurrentRoutes'

export default function getCurrentNav(path, routes) {
  const current = getCurrentRoutes(path, routes)
  if (current.length <= 3) return
  return {
    group: current[1],
    item: current[2]
  }
}
