import * as routerActions from 'connected-react-router'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import createSagaTester from '@app/Common/Utils/test/createSagaTester'
import actions from './action'
import {
  fetchLocalizationListAPI,
  fetchLocalizationAPI,
  createLocalizationAPI,
  updateLocalizationAPI,
  deleteLocalizationsAPI
} from './api'
import reducer from './reducer'
import saga from './saga'

jest.mock('./api')

describe('Localization Saga', () => {
  describe('LocalizationActions.fetchListRequest handler', () => {
    it('should handle success response', async () => {
      const localizations = [
        {
          displaykey: 'BO:button:Submit',
          'en-gb': 'submit',
          'zh-cn': '提交'
        },
        {
          displaykey: 'BO:button:Create',
          'en-gb': 'create',
          'zh-cn': '新增'
        }
      ]
      const successAction = actions.fetchListSuccess(localizations)
      const expectedState = reducer(undefined, successAction)
      fetchLocalizationListAPI.mockResolvedValue(localizations)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchLocalizationListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const error = new Error(401)
      fetchLocalizationListAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchLocalizationListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchListFailure(error)
      )
    })
  })

  describe('LocalizationActions.fetchCurrentRequest handler', () => {
    it('should handle success response', async () => {
      const localization = {
        displayKey: 'BO:Button:Submit',
        'en-gb': 'submit',
        'zh-cn': '提交'
      }
      const successAction = actions.fetchCurrentSuccess(localization)
      const expectedState = reducer(undefined, successAction)
      fetchLocalizationAPI.mockResolvedValue(localization)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(localization.displayKey))

      expect(fetchLocalizationAPI).toBeCalledWith(localization.displayKey)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const displayKey = 'BO:Button:Create'
      const error = new Error(401)
      fetchLocalizationAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(displayKey))

      expect(fetchLocalizationAPI).toBeCalledWith(displayKey)
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchCurrentFailure(error)
      )
    })
  })

  describe('LocalizationActions.createRequest handler', () => {
    it('should handle success response', async () => {
      const localization = {
        displayKey: 'BO:Button:Submit',
        'en-gb': 'submit',
        'zh-cn': '提交'
      }
      createLocalizationAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(localization))

      const calledActions = tester.getCalledActions()
      expect(createLocalizationAPI).toBeCalledWith(localization)
      expect(calledActions).toContainEqual(actions.createSuccess())
      expect(calledActions).toContainEqual(
        routerActions.push(paths.LOCALIZATION_LIST)
      )
    })

    it('should handle failure response', async () => {
      const localization = {
        displayKey: 'BO:Button:Submit',
        'en-gb': 'submit',
        'zh-cn': '提交'
      }
      const error = new Error(401)
      createLocalizationAPI.mockRejectedValue(error)
      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(localization))

      expect(createLocalizationAPI).toBeCalledWith(localization)
      expect(tester.getCalledActions()).toContainEqual(
        actions.createFailure(error)
      )
    })
  })

  describe('LocalizationActions.updateRequest handler', () => {
    it('should handle success response', async () => {
      const localization = {
        displayKey: 'BO:Button:Delete',
        'en-gb': 'delete',
        'zh-cn': '刪除'
      }
      updateLocalizationAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(localization))

      const calledActions = tester.getCalledActions()
      expect(updateLocalizationAPI).toBeCalledWith(localization)
      expect(calledActions).toContainEqual(actions.updateSuccess())
      expect(calledActions).toContainEqual(
        routerActions.push(paths.LOCALIZATION_LIST)
      )
    })

    it('should handle failure response', async () => {
      const localization = {
        displayKey: 'BO:Button:Delete',
        'en-gb': 'delete',
        'zh-cn': '刪除'
      }
      const error = new Error(401)
      updateLocalizationAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(localization))

      expect(updateLocalizationAPI).toBeCalledWith(localization)
      expect(tester.getCalledActions()).toContainEqual(
        actions.updateFailure(error)
      )
    })
  })

  describe('LocalizationActions.deleteRequest handler', () => {
    it('should handle success response', async () => {
      const displayKeys = ['BO:Button:Submit']
      const localizations = [
        {
          displaykey: 'BO:button:Submit',
          'en-gb': 'submit',
          'zh-cn': '提交'
        },
        {
          displaykey: 'BO:button:Create',
          'en-gb': 'create',
          'zh-cn': '創建'
        }
      ]
      const initialState = reducer(
        undefined,
        actions.fetchListSuccess(localizations)
      )
      const successAction = actions.deleteSuccess(displayKeys)
      const expectedState = reducer(initialState, successAction)
      deleteLocalizationsAPI.mockResolvedValue()

      const tester = await createSagaTester({ initialState, reducer, saga })
      await tester.next(actions.deleteRequest(displayKeys))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteLocalizationsAPI).toBeCalledWith(displayKeys)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const displayKeys = ['BO:Button:Create', 'BO:Button:Submit']
      const error = new Error(401)
      deleteLocalizationsAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(displayKeys))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteLocalizationsAPI).toBeCalledWith(displayKeys)
      expect(tester.getCalledActions()).toContainEqual(
        actions.deleteFailure(error)
      )
    })

    it('should cancel request', async () => {
      const displayKeys = ['BO:Button:Create', 'BO:Button:Submit']
      deleteLocalizationsAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(displayKeys))
      await tester.dispatch(dialogActions.cancel())

      expect(deleteLocalizationsAPI).toBeCalledTimes(0)
      expect(tester.getCalledActions()).toContainEqual(actions.deleteCancel())
    })
  })
})
