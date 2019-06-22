export default {
  INDEX: '/',
  ACCOUNT: '/account',
  LOGIN: '/login',
  LOCK_SCREEN: '/lock',
  CHANGE_PASSWORD: '/change-password',
  USER_LIST: '/account/users',
  CREATE_USER: '/account/users/create',
  UPDATE_USER: '/account/users/:id/update'
}

export function getPath(path, params = {}) {
  return Object.keys(params).reduce(
    (path, key) => path.replace(new RegExp(`:${key}`, 'g'), params[key]),
    path
  )
}
