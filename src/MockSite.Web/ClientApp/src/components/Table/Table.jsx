/* eslint-disable react/display-name */

import { Button, Form, Input, Table as AntdTable } from 'antd'
import {
  array,
  arrayOf,
  bool,
  func,
  number,
  object,
  oneOfType,
  string
} from 'prop-types'
import { identity, is } from 'ramda'
import React, { useMemo, useRef } from 'react'
import styled from 'styled-components'
import useFetchActions from '@app/Common/AsyncTask/useFetchActions'
import useFuzzySearch from '@app/Common/Hooks/useFuzzySearch'
import Box from '@app/Components/Box/Box'
import Dropdown from '@app/Components/Dropdown/Dropdown'
import useBatchEditor from './useBatchEditor'

const TableFoot = styled.tfoot`
  background-color: #fafafa;
  > tr > td {
    border-right: 1px solid #e8e8e8;
    border-bottom: 1px solid #e8e8e8;
    padding: 16px;
  }
`

const utils = {
  findEditableKeys: children => {
    const elements = React.Children.toArray(children)
    const editableKeys = []
    for (let element of elements) {
      switch (element.type) {
        case AntdTable.Column: {
          const { editable, dataIndex } = element.props
          if (editable) editableKeys.push(dataIndex)
          break
        }
        case AntdTable.ColumnGroup: {
          editableKeys.push(...utils.findEditableKeys(element.props.children))
          break
        }
      }
    }
    return editableKeys
  },
  findFooters: children => {
    const elements = React.Children.toArray(children)
    const footers = []
    for (let element of elements) {
      switch (element.type) {
        case AntdTable.Column: {
          if (element.props.footer) footers.push(element)
          break
        }
        case AntdTable.ColumnGroup: {
          footers.push(...utils.findFooters(element.props.children))
          break
        }
      }
    }
    return footers
  },
  sortRow: cellName => (row1, row2) => {
    const cell1 = row1[cellName]
    const cell2 = row2[cellName]
    if (!isNaN(cell1) && !isNaN(cell2)) {
      return Math.sign(Number(cell1) - Number(cell2))
    } else {
      return `${cell1}`.localeCompare(`${cell2}`)
    }
  },
  renderClickableCell: (cellValue, rowData, onClick) => {
    return (
      <Button
        style={{ padding: 0 }}
        type="link"
        onClick={() => onClick(rowData)}
      >
        {cellValue}
      </Button>
    )
  },
  renderEditableCell: (editorElement, batchEditor, data, dataIndex) => {
    const model = batchEditor.getModel(data)
    const field = model.fields[dataIndex]
    const status = field.invalid ? 'error' : field.valid ? 'success' : null
    const onChange = e =>
      batchEditor.changeField(
        model.key,
        field.name,
        e.target ? e.target.value : e
      )
    return (
      <Form.Item style={{ margin: 0 }} validateStatus={status}>
        {React.cloneElement(editorElement, {
          value: field.value,
          onChange
        })}
      </Form.Item>
    )
  },
  renderColumn: ({ batchEditor }) => (column, i) => {
    if (column.type === AntdTable.ColumnGroup) {
      return (
        <AntdTable.ColumnGroup key={i} title={column.props.title}>
          {React.Children.map(
            column.props.children,
            utils.renderColumn({ batchEditor })
          )}
        </AntdTable.ColumnGroup>
      )
    }
    const {
      editable,
      editorElement = <Input />,
      sortable,
      title,
      dataIndex,
      render = identity,
      onClick
    } = column.props
    let { minWidth, width } = column.props
    const showEditor = editable && batchEditor.models.length
    const sorter = sortable && !showEditor ? utils.sortRow(dataIndex) : false

    const style = { wordWrap: 'break-word' }
    if (!minWidth && !width) style.minWidth = 48
    else if (minWidth) style.minWidth = minWidth - 32
    else style.width = width - 32

    const headerStyle = { ...style }
    if (sorter) {
      if (headerStyle.width) {
        headerStyle.width = Math.max(headerStyle.width - 20, 0)
      }
      if (headerStyle.minWidth) {
        headerStyle.minWidth = Math.max(headerStyle.minWidth - 20, 0)
      }
    }

    return (
      <AntdTable.Column
        key={dataIndex || column.key}
        sorter={sorter}
        {...column.props}
        title={<Box {...headerStyle}>{title}</Box>}
        render={(value, data, rowIndex) => {
          if (dataIndex === '$$index') value = rowIndex + 1
          let children
          if (showEditor) {
            children = utils.renderEditableCell(
              editorElement,
              batchEditor,
              data,
              dataIndex
            )
          } else {
            style.paddingTop = 10
            style.paddingBottom = 10
            children = render(value)
            if (onClick) {
              children = utils.renderClickableCell(children, data, onClick)
            }
          }
          return <Box {...style}>{children}</Box>
        }}
      />
    )
  },
  renderActionColumn: children => {
    const actions = React.Children.toArray(children).filter(
      child => child.type === TableAction
    )
    if (!actions.length) return null
    return (
      <AntdTable.Column
        key="$$action"
        width={128}
        title={<Box style={{ width: 93.1 }} />}
        render={item => (
          <Dropdown>{actions.map(utils.renderDropdownAction(item))}</Dropdown>
        )}
      />
    )
  },
  renderDropdownAction: item => {
    return (action, index) => {
      const { divider, hidden, onClick, ...props } = action.props
      if (divider) return <Dropdown.Divider key={index} />
      return (
        <Dropdown.Item
          key={index}
          {...props}
          hidden={is(Function, hidden) ? hidden(item) : hidden}
          onClick={onClick && (() => onClick(item))}
        />
      )
    }
  },
  createTableComponent: tableRef => ({ className, style, children }) => {
    const { dataSource, columns } = tableRef.current
    const footers = utils.findFooters(columns)
    const rows = React.Children.toArray(children).filter(
      child => child.type === 'tbody'
    )[0].props.children
    return (
      <table className={className} style={style}>
        {children}
        {!!footers.length && !!rows.length && (
          <TableFoot>
            <tr>
              {footers.map(column => {
                const {
                  colSpan = 1,
                  dataIndex,
                  footer,
                  footerColSpan = colSpan
                } = column.props
                return (
                  <td key={dataIndex || column.key} colSpan={footerColSpan}>
                    {typeof footer === 'function' ? footer(dataSource) : footer}
                  </td>
                )
              })}
            </tr>
          </TableFoot>
        )}
      </table>
    )
  }
}

function TableAction() {
  return null
}

export default function Table({
  fetchAction,
  rowKey = 'id',
  searchKeys = [],
  searchKeyword,
  selectedRowKeys,
  batchEditing,
  batchEditConfig = {},
  batchEditorRef,
  pagination,
  dataSource,
  children,
  onSelectedRowKeysChange,
  onBatchEditSubmit,
  ...props
}) {
  if (rowKey === '$$index') rowKey = (_, index) => index
  if (pagination !== false) {
    pagination = {
      showSizeChanger: true,
      pageSizeOptions: ['10', '20', '50', '100'],
      ...pagination
    }
  }

  const { loading } = useFetchActions(fetchAction)

  const columns = React.Children.toArray(children).filter(c =>
    [AntdTable.Column, AntdTable.ColumnGroup].includes(c.type)
  )
  const editableKeys = utils.findEditableKeys(children)

  const batchEditor = useBatchEditor({
    ...batchEditConfig,
    editing: batchEditing,
    editableKeys,
    dataKey: rowKey,
    dataSource,
    onSubmit: onBatchEditSubmit
  })
  if (batchEditorRef) batchEditorRef.current = batchEditor

  const filteredDataSource = useFuzzySearch({
    data: dataSource,
    keys: searchKeys,
    keyword: searchKeyword
  })

  const rowSelection = useMemo(() => {
    if (!selectedRowKeys || !onSelectedRowKeysChange) return
    return {
      selectedRowKeys,
      onChange: v => onSelectedRowKeysChange(v),
      getCheckboxProps: () => ({
        disabled: batchEditing === true
      })
    }
  }, [batchEditing, selectedRowKeys, onSelectedRowKeysChange])

  const antdColumns = [
    ...columns.map(utils.renderColumn({ batchEditor })),
    utils.renderActionColumn(children)
  ].filter(Boolean)

  const tableRef = useRef()
  tableRef.current = {
    dataSource: filteredDataSource,
    columns: antdColumns
  }
  const tableComponentRef = useRef()
  if (!tableComponentRef.current) {
    tableComponentRef.current = utils.createTableComponent(tableRef)
  }

  return (
    <AntdTable
      bordered
      loading={loading}
      rowKey={rowKey}
      rowSelection={rowSelection}
      dataSource={filteredDataSource}
      pagination={pagination}
      components={{ table: tableComponentRef.current }}
      {...props}
    >
      {antdColumns}
    </AntdTable>
  )
}

Table.ColumnGroup = AntdTable.ColumnGroup
Table.Column = AntdTable.Column
Table.Action = TableAction
Table.propTypes = {
  fetchAction: array,
  rowKey: oneOfType([func, string]),
  searchKeys: arrayOf(string.isRequired),
  searchKeyword: string,
  selectedRowKeys: arrayOf(oneOfType([number, string]).isRequired),
  batchEditing: bool,
  batchEditConfig: object,
  batchEditorRef: object,
  pagination: oneOfType([bool, object]),
  dataSource: array.isRequired,
  onSelectedRowKeysChange: func,
  onBatchEditSubmit: func
}
