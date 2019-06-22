import { useEffect, useState, useRef } from 'react'

const utils = {
  defaultModels: [],
  defaultConverter: (name, value) => value,
  defaultVerifier: () => true,
  defaultSubmitHandler: () => {},
  getDataKey: (data, dataKey) =>
    typeof dataKey === 'function' ? dataKey(data) : data[dataKey]
}

export default ({
  editing = false,
  editableKeys = [],
  dataKey,
  dataSource,
  toInputValue = utils.defaultConverter,
  toModelData = utils.defaultConverter,
  verifyField = utils.defaultVerifier,
  onSubmit = utils.defaultSubmitHandler
}) => {
  const [models, setModels] = useState(utils.defaultModels)

  useEffect(() => {
    if (!editing) return setModels(utils.defaultModels)
    setModels(
      dataSource.map(data => ({
        key: utils.getDataKey(data, dataKey),
        data,
        fields: Object.keys(data)
          .filter(name => editableKeys.includes(name))
          .reduce((fields, name) => {
            const value = toInputValue(name, data[name])
            return {
              ...fields,
              [name]: {
                name,
                pristine: true,
                valid: false,
                invalid: false,
                rawValue: value,
                value
              }
            }
          }, {}),
        pristine: true,
        valid: false,
        invalid: false
      }))
    )
  }, [editing, dataSource])

  const stateRef = useRef()
  stateRef.current = { models, toModelData, verifyField, onSubmit }

  const changeFieldFnRef = useRef()
  if (!changeFieldFnRef.current) {
    changeFieldFnRef.current = (key, name, value) => {
      const { models, verifyField } = stateRef.current
      const index = models.findIndex(model => model.key === key)
      const model = models[index]
      const field = model.fields[name]
      const valid = verifyField(name, value, model)
      const nextFields = {
        ...model.fields,
        [name]: {
          ...field,
          pristine: value === field.rawValue,
          valid,
          invalid: !valid,
          value
        }
      }
      const nextModels = [...models]
      nextModels[index] = {
        ...model,
        fields: nextFields,
        ...Object.values(nextFields).reduce(
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
      }
      setModels(nextModels)
    }
  }

  const submitFnRef = useRef()
  if (!submitFnRef.current) {
    submitFnRef.current = () => {
      const { models, toModelData, onSubmit } = stateRef.current
      const updates = []
      for (let model of models) {
        if (model.invalid) return false
        if (model.pristine) continue
        const update = {}
        for (let [name, field] of Object.entries(model.fields)) {
          update[name] = toModelData(name, field.value, model)
        }
        updates.push({ ...model.data, ...update })
      }
      onSubmit(updates)
      return true
    }
  }

  const getModelRef = useRef()
  if (!getModelRef.current) {
    getModelRef.current = data => {
      const { models } = stateRef.current
      const key = utils.getDataKey(data, dataKey)
      const model = models.find(model => model.key === key)
      return model
    }
  }

  const getFieldRef = useRef()
  if (!getFieldRef.current) {
    getFieldRef.current = (data, name) => {
      const model = getModelRef.current(data)
      return model.fields[name]
    }
  }

  return {
    models,
    ...models.reduce(
      (status, model) => ({
        pristine: status.pristine && model.pristine,
        valid: status.valid && model.valid,
        invalid: status.invalid || model.invalid
      }),
      {
        pristine: true,
        valid: true,
        invalid: false
      }
    ),
    getModel: getModelRef.current,
    getField: getFieldRef.current,
    changeField: changeFieldFnRef.current,
    submit: submitFnRef.current
  }
}
