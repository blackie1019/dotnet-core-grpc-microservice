import AppAPI from '@app/Common/Utils/http/AppAPI'

const currencyPath = 'Currency'

export const fetchCurrencyListAPI = () =>
  AppAPI.get(`${currencyPath}/GetCurrencyAll`).useAuth()

export const fetchCurrencyAPI = currencyCode =>
  AppAPI.get(`${currencyPath}/GetCurrency/${currencyCode}`).useAuth()

export const createCurrencyAPI = form => {
  const requestBody = { ...form, ifExistUpdateRate: true }

  return AppAPI.post(`${currencyPath}/CreateCurrency`)
    .useAuth()
    .body(requestBody)
}

export const updateCurrencyAPI = data =>
  AppAPI.post(`${currencyPath}/UpdateCurrency`)
    .useAuth()
    .body(data)

export const deleteCurrenciesAPI = currencyCode =>
  AppAPI.post(`${currencyPath}/DeleteCurrency/${currencyCode[0]}`).useAuth()
