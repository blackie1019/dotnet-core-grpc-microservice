const initialState = {}

const STATUSES = ['REQUEST', 'SUCCESS', 'FAILURE', 'CANCEL']

export default (state = initialState, action) => {
  const matches = new RegExp(`^(.+)_(${STATUSES.join('|')})$`).exec(action.type)
  if (!matches) return state
  const [, name, status] = matches
  const index = STATUSES.indexOf(status)
  return {
    ...state,
    [action.taskId || name]: {
      loading: index === 0,
      success: index === 1,
      failure: index === 2
    }
  }
}

export const getTaskId = ({ type, taskId }) =>
  taskId || type.replace(new RegExp(`_(${STATUSES.join('|')})$`), '')
