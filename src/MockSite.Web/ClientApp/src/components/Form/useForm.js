import { equals } from 'ramda'
import { useReducer, useRef } from 'react'
import useDidUpdate from '@app/Common/Hooks/useDidUpdate'
import useSingleton from '@app/Common/Hooks/useSingleton'
import useWillMount from '@app/Common/Hooks/useWillMount'

const utils = {
  defaultFieldConverter: (name, value) => value,
  defaultVerifier: () => true,
  defaultEventListener: () => {},
  initialFieldStatus: {
    pristine: true,
    valid: false,
    invalid: false,
    message: undefined
  },
  createInitialState(model = {}, fields = {}) {
    return { status: utils.getFormStatus(fields), model, fields }
  },
  createField(name, value) {
    return {
      name,
      value,
      rawValue: value,
      ...utils.initialFieldStatus
    }
  },
  createFields(model, toInputValue) {
    const fields = {}
    for (let [name, data] of Object.entries(model)) {
      fields[name] = utils.createField(name, toInputValue(name, data))
    }
    return fields
  },
  getFieldStatus(fields, name, value, verifyFn, forced = false) {
    const field = fields[name]
    if (!field) return utils.initialFieldStatus
    if (!forced && value === field.rawValue) return utils.initialFieldStatus
    const result = verifyFn(name, value, fields)
    return {
      pristine: false,
      valid: result === true,
      invalid: result !== true,
      message: typeof result !== 'boolean' ? result : ''
    }
  },
  updateField(fields, name, value, verifyFn) {
    return {
      ...fields[name],
      ...utils.getFieldStatus(fields, name, value, verifyFn),
      value
    }
  },
  updateFields(fields, model, toInputValue, verifyFn) {
    const nextFields = {}
    for (let [name, field] of Object.entries(fields)) {
      const value = toInputValue(name, model[name])
      nextFields[name] = field
        ? { ...field, value }
        : utils.createField(name, value)
    }
    for (let [name, field] of Object.entries(nextFields)) {
      nextFields[name] = {
        ...field,
        ...utils.getFieldStatus(nextFields, name, field.value, verifyFn)
      }
    }
    return nextFields
  },
  verifyFields(fields, verifyFn) {
    const result = { valid: true, fields: {} }
    for (let [name, field] of Object.entries(fields)) {
      const { value } = field
      const status = utils.getFieldStatus(fields, name, value, verifyFn, true)
      result.fields[name] = { ...field, ...status }
      result.valid = result.valid && status.valid
    }
    return result
  },
  getFormModel(fields, toModelData) {
    const model = {}
    for (let [name, field] of Object.entries(fields)) {
      model[name] = toModelData(name, field.value)
    }
    return model
  },
  getFormStatus(fields) {
    return Object.values(fields).reduce(
      (status, field) => ({
        pristine: status.pristine && field.pristine,
        valid: status.valid && field.valid,
        invalid: status.invalid || field.invalid
      }),
      {
        pristine: true,
        valid: true,
        invalid: false
      }
    )
  },
  enhanceDispatch(dispatch, contextRef) {
    return action => {
      if (typeof action === 'function') dispatch(action(contextRef.current))
      else dispatch(action)
    }
  },
  bindDispatch(actionCreator, dispatch) {
    return (...args) => dispatch(actionCreator(...args))
  }
}

const ACTION_TYPE = {
  SET_FIELD: Symbol(),
  SET_FIELDS: Symbol()
}

const actions = {
  initField(name, value) {
    return ({ toModelData }) => ({
      type: ACTION_TYPE.SET_FIELD,
      name,
      data: toModelData(name, value),
      field: utils.createField(name, value)
    })
  },
  setField(name, value) {
    return ({ state, dispatch, toModelData, verifyField }) => {
      const { fields } = state
      const field = fields[name]
      if (!field) return dispatch(actions.initField(name, value))
      if (field.value === value) return
      return {
        type: ACTION_TYPE.SET_FIELD,
        name,
        data: toModelData(name, value),
        field: utils.updateField(fields, name, value, verifyField)
      }
    }
  },
  setFields(fields) {
    return ({ toModelData }) => ({
      type: ACTION_TYPE.SET_FIELDS,
      model: utils.getFormModel(fields, toModelData),
      fields
    })
  },
  updateForm(model) {
    return ({ state, dispatch, toInputValue, verifyField }) => {
      const fields = utils.updateFields(
        state.fields,
        model,
        toInputValue,
        verifyField
      )
      dispatch(actions.setFields(fields))
    }
  },
  resetForm() {
    return ({ dispatch, initialFields }) =>
      dispatch(actions.setFields(initialFields))
  },
  submitForm() {
    return ({ state, dispatch, toModelData, verifyField, onSubmit }) => {
      const result = utils.verifyFields(state.fields, verifyField)
      if (!result.valid) return dispatch(actions.setFields(result.fields))
      onSubmit(utils.getFormModel(result.fields, toModelData))
    }
  }
}

const reducer = (state, action) => {
  switch (action?.type) {
    case ACTION_TYPE.SET_FIELD: {
      const { name, data, field } = action
      const model = { ...state.model, [name]: data }
      const fields = { ...state.fields, [name]: field }
      return { status: utils.getFormStatus(fields), model, fields }
    }
    case ACTION_TYPE.SET_FIELDS: {
      const { model, fields } = action
      return { status: utils.getFormStatus(fields), model, fields }
    }
    default:
      return state
  }
}

export default ({
  model,
  defaultModel = model,
  toInputValue = utils.defaultFieldConverter,
  toModelData = utils.defaultFieldConverter,
  verifyField = utils.defaultVerifier,
  onChange = utils.defaultEventListener,
  onSubmit = utils.defaultEventListener
}) => {
  const contextRef = useRef()
  let initialFields
  let initialState
  useWillMount(() => {
    initialFields = utils.createFields(defaultModel, toInputValue)
    initialState = utils.createInitialState(defaultModel, initialFields)
    contextRef.current = { defaultModel, initialFields }
  })

  const [state, rawDispatch] = useReducer(reducer, initialState)
  const dispatch = useSingleton(utils.enhanceDispatch, rawDispatch, contextRef)
  contextRef.current = {
    ...contextRef.current,
    state,
    dispatch,
    toInputValue,
    toModelData,
    verifyField,
    onChange,
    onSubmit
  }

  const prevStateRef = useRef()
  const prevModel = prevStateRef.current?.model
  prevStateRef.current = state
  useDidUpdate(() => {
    if (!equals(prevModel, state.model)) {
      onChange(state.model)
    } else if (model && !equals(model, state.model)) {
      dispatch(actions.updateForm(model))
    }
  })

  const formRef = useRef()
  if (!formRef.current) {
    formRef.current = {
      setField: utils.bindDispatch(actions.setField, dispatch),
      reset: utils.bindDispatch(actions.resetForm, dispatch),
      submit: utils.bindDispatch(actions.submitForm, dispatch)
    }
  }
  return { ...formRef.current, fields: state.fields, status: state.status }
}
