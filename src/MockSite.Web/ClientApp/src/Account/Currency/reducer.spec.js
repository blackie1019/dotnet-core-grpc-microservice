import actions from './action'
import reducer from './reducer'

describe('Currency Reducer', () => {
  it('should handle currencyActions.fetchListSuccess', () => {
    const initialState = { list: [] }
    const expectedCurrencies = [{ currencyCode: 'JPY', currencyRate: 6.9 }]
    const actualState = reducer(
      initialState,
      actions.fetchListSuccess(expectedCurrencies)
    )
    expect(actualState.list).toEqual(expectedCurrencies)
  })

  it('should handle currencyActions.fetchCurrentRequest', () => {
    const initialState = { current: { currencyCode: 'JPY', currencyRate: 6.9 } }
    const actualState = reducer(
      initialState,
      actions.fetchCurrentRequest('CYN')
    )
    expect(actualState.current).toBeUndefined()
  })

  it('should handle currencyActions.fetchCurrentSuccess', () => {
    const initialState = { current: undefined }
    const expectedCurrency = { currencyCode: 'JPY', currencyRate: 6.9 }
    const actualState = reducer(
      initialState,
      actions.fetchCurrentSuccess(expectedCurrency)
    )
    expect(actualState.current).toEqual(expectedCurrency)
  })

  it('should handle currencyActions.deleteSuccess', () => {
    const initialState = {
      list: [
        { currencyCode: 'JPY', currencyRate: 6.9 },
        { currencyCode: 'CYN', currencyRate: 9 },
        { currencyCode: 'TWD', currencyRate: 2.3 }
      ]
    }
    const deletedCurrencyCodes = ['JPY', 'TWD']
    const expectedCurrencies = [{ currencyCode: 'CYN', currencyRate: 9 }]
    const actualState = reducer(
      initialState,
      actions.deleteSuccess(deletedCurrencyCodes)
    )
    expect(actualState.list).toEqual(expectedCurrencies)
  })
})
