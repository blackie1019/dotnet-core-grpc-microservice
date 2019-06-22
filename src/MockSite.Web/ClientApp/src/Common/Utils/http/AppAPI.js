import Request from './Request'
import {
  getCredential,
  saveCredential,
  restoreCredential,
  lockCredential,
  clearCredential
} from './session'

const API_URL = app.env.API_URL
const authPath = 'Authentication'

const RESPONSE_CODE = {
  SUCCESS: 0
}

const handleResponse = res => {
  const { code, data, msg } = res.data
  if (code !== RESPONSE_CODE.SUCCESS) {
    throw new Error(`Code: ${code}, Message: ${msg}`)
  }
  return data
}

export default class AppAPI extends Request {
  static get = path => new AppAPI(path)
  static post = path => new AppAPI(path, 'POST')
  static put = path => new AppAPI(path, 'PUT')
  static delete = path => new AppAPI(path, 'DELETE')

  static initAuth = () => Promise.resolve(restoreCredential())
  static lockAuth = () => Promise.resolve(lockCredential())
  static login = data =>
    AppAPI.post(`${authPath}/Login`)
      .body(data)
      .then(saveCredential)
  static logout = () =>
    AppAPI.post(`${authPath}/Logout`)
      .useAuth()
      .then(clearCredential, clearCredential)
  static renewToken = data =>
    AppAPI.post(`${authPath}/RenewToken`)
      .useAuth()
      .body(data)
      .then(saveCredential)

  constructor(path, method) {
    super(`${API_URL}/${path.replace(/^\//, '')}`, method)
  }

  useAuth() {
    return this.bearer(getCredential()?.token)
  }

  then(onFulfill, onReject) {
    return super.then(handleResponse).then(onFulfill, onReject)
  }
}
