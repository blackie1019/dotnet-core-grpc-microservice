import hasPermission from '@app/Common/Authentication/hasPermission'

export default (routes, credential, gotoPage) =>
  routes
    .filter(r => r.icon && hasPermission(credential, r.policy))
    .map(r => ({
      icon: r.icon,
      displayKey: r.displayKey,
      navs: r.routes
        .filter(r => r.displayKey && hasPermission(credential, r.policy))
        .map(r => ({
          displayKey: r.displayKey,
          onClick: () => gotoPage(r.path)
        }))
    }))
