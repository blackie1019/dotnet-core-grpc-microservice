import { push as gotoPage } from 'connected-react-router'
import React, { useState } from 'react'
import paths, { getPath } from '@app/App/constants/paths'
import { arrayOf, string, shape, func } from 'prop-types'
import { connect } from 'react-redux'
import Button from '@app/Components/Button/Button'
import SearchInput from '@app/Components/Input/SearchInput'
import Section from '@app/Components/Section/Section'
import Table from '@app/Components/Table/Table'
import localizationActions from './action'

function LocalizationListPage({
  localizationItems,
  fetchList,
  batchDelete,
  gotoCreatePage,
  gotoUpdatePage
}) {
  const [keyword, setKeyword] = useState('')

  return (
    <Section>
      <Section.Title text="Localization List" />
      <Section.ActionBar>
        <Section.ButtonGroup>
          <Button.Create onClick={gotoCreatePage} />
        </Section.ButtonGroup>
        <SearchInput onSearch={setKeyword} />
      </Section.ActionBar>
      <Section.Body>
        <Table
          fetchAction={[fetchList]}
          dataSource={localizationItems}
          searchKeys={['displayKey', 'en-gb', 'zh-cn']}
          searchKeyword={keyword}
          rowKey="displayKey"
        >
          <Table.Column title="DisplayKey" dataIndex="displayKey" sortable />
          <Table.Column title="en-gb" dataIndex="en-gb" sortable />
          <Table.Column title="zh-cn" dataIndex="zh-cn" sortable />
          <Table.Action
            text="Edit"
            onClick={row => gotoUpdatePage(row.displayKey)}
          />
          <Table.Action
            text="Delete"
            onClick={row => batchDelete([row.displayKey])}
          />
        </Table>
      </Section.Body>
    </Section>
  )
}

LocalizationListPage.propTypes = {
  localizationItems: arrayOf(
    shape({
      displayKey: string.isRequired,
      'en-gb': string.isRequired,
      'zh-cn': string.isRequired
    })
  ).isRequired,
  gotoCreatePage: func.isRequired,
  gotoUpdatePage: func.isRequired,
  fetchList: func.isRequired,
  batchDelete: func.isRequired
}

export default connect(
  state => ({ localizationItems: state.localization.list }),
  {
    fetchList: localizationActions.fetchListRequest,
    batchDelete: localizationActions.deleteRequest,
    gotoCreatePage: () => gotoPage(paths.CREATE_LOCALIZATION),
    gotoUpdatePage: displayKey => {
      return gotoPage(getPath(paths.UPDATE_LOCALIZATION, { displayKey }))
    }
  }
)(LocalizationListPage)
