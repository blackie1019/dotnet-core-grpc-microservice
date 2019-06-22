import { Select as AntdSelect } from 'antd'
import { any, arrayOf, bool, object, string } from 'prop-types'
import React from 'react'

export default function Select({
  options,
  valueKey,
  labelKey = valueKey,
  value = null,
  showDefaultOption,
  defaultOptionLabel = 'All',
  defaultOptionValue = null,
  ...props
}) {
  return (
    <AntdSelect value={value} {...props}>
      {showDefaultOption && (
        <AntdSelect.Option key="$$default" value={defaultOptionValue}>
          {defaultOptionLabel}
        </AntdSelect.Option>
      )}
      {options.map((option, index) => (
        <AntdSelect.Option key={index} value={option[valueKey]}>
          {option[labelKey]}
        </AntdSelect.Option>
      ))}
    </AntdSelect>
  )
}

Select.propTypes = {
  options: arrayOf(object.isRequired).isRequired,
  valueKey: string.isRequired,
  labelKey: string,
  value: any,
  showDefaultOption: bool,
  defaultOptionLabel: string,
  defaultOptionValue: any
}
