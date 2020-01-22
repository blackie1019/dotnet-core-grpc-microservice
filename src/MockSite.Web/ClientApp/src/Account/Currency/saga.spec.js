import * as routerActions from 'connected-react-router'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import createSagaTester from '@app/Common/Utils/test/createSagaTester'
import actions from './action'
import {
  fetchCurrencyListAPI,
  fetchCurrencyAPI,
  createCurrencyAPI,
  updateCurrencyAPI,
  deleteCurrenciesAPI
} from './api'
import reducer from './reducer'
import saga from './saga'

jest.mock('./api')

describe('Currency Saga', () => {
  describe('currencyActions.fetchListRequest handler', () => {
    it('should handle success response', async () => {
      const currencies = [
        { currencyCode: 'JYP', currencyRate: 3.2 },
        { currencyCode: 'TWD', currencyRate: 4.3 }
      ]

      const successAction = actions.fetchListSuccess(currencies)
      const expectedState = reducer(undefined, successAction)
      fetchCurrencyListAPI.mockResolvedValue(currencies)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchCurrencyListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const error = new Error(401)
      fetchCurrencyListAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchCurrencyListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchListFailure(error)
      )
    })
  })

  describe('currencyActions.fetchCurrentRequest handler', () => {
    it('should handle success response', async () => {
      const currency = { currencyCode: 'JYP', currencyRate: 3.2 }
      const successAction = actions.fetchCurrentSuccess(currency)
      const expectedState = reducer(undefined, successAction)
      fetchCurrencyAPI.mockResolvedValue(currency)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(currency.currencyCode))

      expect(fetchCurrencyAPI).toBeCalledWith(currency.currencyCode)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const currencyCode = 1
      const error = new Error(401)
      fetchCurrencyAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(currencyCode))

      expect(fetchCurrencyAPI).toBeCalledWith(currencyCode)
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchCurrentFailure(error)
      )
    })
  })

  describe('currencyActions.createRequest handler', () => {
    it('should handle success response', async () => {
      const currency = { currencyCode: 'JYP', currencyRate: 3 }
      createCurrencyAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(currency))

      const calledActions = tester.getCalledActions()
      expect(createCurrencyAPI).toBeCalledWith(currency)
      expect(calledActions).toContainEqual(actions.createSuccess())
      expect(calledActions).toContainEqual(
        routerActions.push(paths.CURRENCY_LIST)
      )
    })

    it('should handle failure response', async () => {
      const currency = { currencyCode: 'JYP', currencyRate: 3 }
      const error = new Error(401)
      createCurrencyAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(currency))

      expect(createCurrencyAPI).toBeCalledWith(currency)
      expect(tester.getCalledActions()).toContainEqual(
        actions.createFailure(error)
      )
    })
  })

  describe('currencyActions.updateRequest handler', () => {
    it('should handle success response', async () => {
      const currency = { currencyCode: 'CYN', currencyRate: 1.2 }
      updateCurrencyAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(currency))

      const calledActions = tester.getCalledActions()
      expect(updateCurrencyAPI).toBeCalledWith(currency)
      expect(calledActions).toContainEqual(actions.updateSuccess())
      expect(calledActions).toContainEqual(
        routerActions.push(paths.CURRENCY_LIST)
      )
    })

    it('should handle failure response', async () => {
      const currency = { currencyCode: 'CYN', currencyRate: 3.2 }
      const error = new Error(401)
      updateCurrencyAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(currency))

      expect(updateCurrencyAPI).toBeCalledWith(currency)
      expect(tester.getCalledActions()).toContainEqual(
        actions.updateFailure(error)
      )
    })
  })

  describe('currencyActions.deleteRequest handler', () => {
    it('should handle success response', async () => {
      const currencyCodes = ['JYP', 'TWD']
      const currencies = [
        { currencyCode: 'JPY', currencyRate: 3.2 },
        { currencyCode: 'TWD', currencyRate: 4.3 }
      ]
      const successAction = actions.deleteSuccess(currencyCodes)
      const initialState = reducer(
        undefined,
        actions.fetchListSuccess(currencies)
      )
      const expectedState = reducer(initialState, successAction)
      deleteCurrenciesAPI.mockResolvedValue()

      const tester = await createSagaTester({ initialState, reducer, saga })
      await tester.next(actions.deleteRequest(currencyCodes))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteCurrenciesAPI).toBeCalledWith(currencyCodes)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const currencyCodes = ['JYP', 'TWD']
      const error = new Error(401)
      deleteCurrenciesAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(currencyCodes))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteCurrenciesAPI).toBeCalledWith(currencyCodes)
      expect(tester.getCalledActions()).toContainEqual(
        actions.deleteFailure(error)
      )
    })

    it('should cancel request', async () => {
      const currencyCodes = ['JYP', 'TWD']
      deleteCurrenciesAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(currencyCodes))
      await tester.dispatch(dialogActions.cancel())

      expect(deleteCurrenciesAPI).toBeCalledTimes(0)
      expect(tester.getCalledActions()).toContainEqual(actions.deleteCancel())
    })
  })
})
