import actions from './action'
import reducer from './reducer'

describe('Localization Reducer', () => {
  it('should handle localizationActions.fetchListSuccess', () => {
    const initialState = { list: [] }
    const expectedState = [
      {
        displayKey: 'BO:Button:Submit',
        'en-gb': 'submit',
        'zh-cn': '提交'
      },
      {
        displayKey: 'BO:Button:Delete',
        'en-gb': 'delete',
        'zh-cn': '刪除'
      }
    ]

    const actualState = reducer(
      initialState,
      actions.fetchListSuccess(expectedState)
    )
    expect(actualState.list).toEqual(expectedState)
  })
  it('should handle localizationActions.fetchCurrentSuccess', () => {
    const initialState = {
      current: {
        displayKey: 'BO:Button:Submit',
        'en-gb': 'submit',
        'zh-cn': '提交'
      }
    }
    const actualState = reducer(initialState, actions.fetchCurrentSuccess())
    expect(actualState.current).toBeUndefined()
  })
  it('should handle localizationActions.deleteSuccess', () => {
    const initialState = {
      list: [
        {
          displayKey: 'BO:Button:Submit',
          'en-gb': 'submit',
          'zh-cn': '提交'
        },
        {
          displayKey: 'BO:Button:Create',
          'en-gb': 'create',
          'zh-cn': '創建'
        },
        {
          displayKey: 'BO:Button:Delete',
          'en-gb': 'delete',
          'zh-cn': '刪除'
        }
      ]
    }
    const deletedDisplayKeys = ['BO:Button:Create', 'BO:Button:Submit']
    const expectedState = [
      {
        displayKey: 'BO:Button:Delete',
        'en-gb': 'delete',
        'zh-cn': '刪除'
      }
    ]
    const actualState = reducer(
      initialState,
      actions.deleteSuccess(deletedDisplayKeys)
    )
    expect(actualState.list).toEqual(expectedState)
  })
})
