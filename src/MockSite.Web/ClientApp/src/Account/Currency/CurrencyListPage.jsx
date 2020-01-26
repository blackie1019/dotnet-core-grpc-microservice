import { push as gotoPage } from 'connected-react-router'
import React, { useState } from 'react'
import paths, { getPath } from '@app/App/constants/paths'
import { arrayOf, shape, string, func } from 'prop-types'
import { connect } from 'react-redux'
import Button from '@app/Components/Button/Button'
import Section from '@app/Components/Section/Section'
import SearchInput from '@app/Components/Input/SearchInput'
import Table from '@app/Components/Table/Table'
import currencyActions from './action'

function CurrencyListPage({
  currencies,
  fetchList,
  batchDelete,
  gotoCreatePage,
  gotoUpdatePage
}) {
  const [keyword, setKeyword] = useState('')
  return (
    <Section>
      <Section.Title text="Currency List" />
      <Section.ActionBar>
        <Section.ButtonGroup>
          <Button.Create onClick={gotoCreatePage} />
        </Section.ButtonGroup>
        <SearchInput onSearch={setKeyword} />
      </Section.ActionBar>
      <Section.Body>
        <Table
          fetchAction={[fetchList]}
          searchKeys={['currencyCode']}
          searchKeyword={keyword}
          dataSource={currencies}
        >
          <Table.Column
            title="CurrencyCode"
            dataIndex="currencyCode"
            sortable
          />
          <Table.Column
            title="CurrencyRate"
            dataIndex="currencyRate"
            sortable
          />
          <Table.Action
            text="Edit"
            onClick={currency => gotoUpdatePage(currency.currencyCode)}
          />
          <Table.Action
            text="Delete"
            onClick={currency => batchDelete([currency.currencyCode])}
          />
        </Table>
      </Section.Body>
    </Section>
  )
}

CurrencyListPage.propTypes = {
  currencies: arrayOf(
    shape({
      currencyCode: string.isRequired,
      currencyRate: string.isRequired
    })
  ).isRequired,
  gotoCreatePage: func.isRequired,
  gotoUpdatePage: func.isRequired,
  fetchList: func.isRequired,
  batchDelete: func.isRequired
}

export default connect(
  state => ({ currencies: state.currency.list }),
  {
    fetchList: currencyActions.fetchListRequest,
    batchDelete: currencyActions.deleteRequest,
    gotoCreatePage: () => gotoPage(paths.CREATE_CURRENCY),
    gotoUpdatePage: currencyCode =>
      gotoPage(getPath(paths.UPDATE_CURRENCY, { currencyCode }))
  }
)(CurrencyListPage)
