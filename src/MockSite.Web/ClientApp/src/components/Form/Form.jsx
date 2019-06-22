import { Form as AntdForm } from 'antd'
import { func, object } from 'prop-types'
import React, { useContext, useEffect, useRef } from 'react'
import useSingleton from '@app/Common/Hooks/useSingleton'
import useWillMount from '@app/Common/Hooks/useWillMount'
import useForm from './useForm'

const FormContext = React.createContext()

const findFieldElement = children => {
  for (let child of React.Children.toArray(children)) {
    if (child.type === FormField) return child
    if (!child.props) return
    if (child.props.children) {
      const field = findFieldElement(child.props.children)
      if (field) return field
    }
  }
}

function FormField({ name, value, children, ...props }) {
  const form = useContext(FormContext)
  const handleChange = useSingleton(() => e =>
    form.setField(name, e && e.target ? e.target.value : e)
  )
  useEffect(() => {
    if (value === undefined) return
    if (value === form.fields[name].value) return
    form.setField(name, value)
  }, [value])
  return React.cloneElement(children, {
    id: name,
    value: form.fields[name].value,
    onChange: handleChange,
    ...props
  })
}

function FormItem({ children, ...props }) {
  const form = useContext(FormContext)

  const fieldNameRef = useRef()
  const fieldNotExistsRef = useRef(false)
  useWillMount(() => {
    const fieldElement = findFieldElement(children)
    if (fieldElement) fieldNameRef.current = fieldElement.props.name
    else fieldNotExistsRef.current = true
  })

  if (fieldNotExistsRef.current) {
    return <AntdForm.Item {...props}>{children}</AntdForm.Item>
  }

  const fieldName = fieldNameRef.current
  const field = form.fields[fieldName]
  return (
    <AntdForm.Item
      {...props}
      htmlFor={fieldName}
      validateStatus={field.invalid ? 'error' : field.valid ? 'success' : null}
      help={field.message}
    >
      {children}
    </AntdForm.Item>
  )
}

const Form = React.forwardRef(
  (
    {
      data,
      defaultData,
      toInputValue,
      toModelData,
      verifyField,
      onChange,
      onSubmit,
      ...props
    },
    ref
  ) => {
    const form = useForm({
      model: data,
      defaultModel: defaultData,
      toInputValue,
      toModelData,
      verifyField,
      onChange,
      onSubmit
    })
    ref && (ref.current = form)
    const handleSubmit = useSingleton(() => e => {
      e.preventDefault()
      return form.submit()
    })
    return (
      <FormContext.Provider value={form}>
        <AntdForm {...props} onSubmit={handleSubmit} />
      </FormContext.Provider>
    )
  }
)

Form.propTypes = {
  data: object,
  defaultData: object,
  toInputValue: func,
  toModelData: func,
  verifyField: func,
  onChange: func,
  onSubmit: func
}
Form.Item = FormItem
Form.Field = FormField

export default Form
