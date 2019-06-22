import axios from 'axios'

function isNotEmpty(value) {
  return value != null && value !== ''
}

export default class Request {
  static get = url => new Request(url)
  static post = url => new Request(url, 'POST')
  static put = url => new Request(url, 'PUT')
  static delete = url => new Request(url, 'DELETE')

  constructor(url, method = 'GET') {
    if (!url) throw new TypeError('URL is required')
    this._url = url
    this._method = method
    this._headers = {}
    this._urlParams = {}
    this._bodyData = undefined
  }

  header(key, value) {
    if (value != null) this._headers[key] = value
    return this
  }

  auth(value) {
    return this.header('Authorization', value)
  }

  bearer(token) {
    return token ? this.auth(`Bearer ${token}`) : this
  }

  param(key, value) {
    if (isNotEmpty(value)) this._urlParams[key] = value
    return this
  }

  params(data) {
    if (isNotEmpty(data))
      Object.keys(data).forEach(key => this.param(key, data[key]))
    return this
  }

  body(data) {
    if (isNotEmpty(data)) this._bodyData = data
    return this
  }

  then(onFulfill, onReject) {
    return axios({
      url: this._url,
      method: this._method,
      headers: this._headers,
      params: this._urlParams,
      data: this._bodyData
    }).then(onFulfill, onReject)
  }
}
