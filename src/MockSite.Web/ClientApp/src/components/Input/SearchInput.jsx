import { Input } from 'antd'
import { func } from 'prop-types'
import React from 'react'

export default function SearchInput({ onSearch }) {
  return <Input.Search placeholder="Search" onSearch={v => onSearch(v)} />
}

SearchInput.propTypes = {
  onSearch: func.isRequired
}
