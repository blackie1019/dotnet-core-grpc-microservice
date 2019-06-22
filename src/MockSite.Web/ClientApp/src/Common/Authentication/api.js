import AppAPI from '@app/Common/Utils/http/AppAPI'

export const initAuthAPI = AppAPI.initAuth
export const lockAuthAPI = AppAPI.lockAuth
export const loginAPI = AppAPI.login
export const logoutAPI = AppAPI.logout
export const renewTokenAPI = AppAPI.renewToken

export const changePasswordAPI = data =>
  AppAPI.post('Authentication/UpdatePassword')
    .useAuth()
    .body(data)
