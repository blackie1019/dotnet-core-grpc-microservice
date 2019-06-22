import { push as gotoPage } from 'connected-react-router'
import { arrayOf, func, number, shape, string } from 'prop-types'
import React, { useState } from 'react'
import { connect } from 'react-redux'
import paths, { getPath } from '@app/App/constants/paths'
import policies from '@app/App/constants/policies'
import Button from '@app/Components/Button/Button'
import SearchInput from '@app/Components/Input/SearchInput'
import Section from '@app/Components/Section/Section'
import Table from '@app/Components/Table/Table'
import userActions from './action'

function UserListPage({
  users,
  fetchList,
  batchDelete,
  gotoCreatePage,
  gotoUpdatePage
}) {
  const [keyword, setKeyword] = useState('')
  const [selectedIds, setSelectedIds] = useState([])
  return (
    <Section>
      <Section.Title text="User List" />
      <Section.ActionBar>
        <Section.ButtonGroup>
          <Button.Create
            policy={[policies.USER_MODIFY]}
            onClick={gotoCreatePage}
          />
        </Section.ButtonGroup>
        <SearchInput onSearch={setKeyword} />
      </Section.ActionBar>
      <Section.Body>
        <Table
          fetchAction={[fetchList]}
          searchKeys={['code', 'name', 'email']}
          searchKeyword={keyword}
          selectedRowKeys={selectedIds}
          dataSource={users}
          onSelectedRowKeysChange={setSelectedIds}
        >
          <Table.Column title="Code" dataIndex="code" width={120} sortable />
          <Table.Column title="Name" dataIndex="name" width={120} sortable />
          <Table.Column
            title="Email"
            dataIndex="email"
            minWidth={240}
            sortable
          />
          <Table.Action
            policy={[policies.USER_MODIFY]}
            text="Edit"
            onClick={user => gotoUpdatePage(user.id)}
          />
          <Table.Action
            policy={[policies.USER_DELETE]}
            text="Delete"
            onClick={user => batchDelete([user.id])}
          />
        </Table>
      </Section.Body>
    </Section>
  )
}

UserListPage.propTypes = {
  users: arrayOf(
    shape({
      id: number.isRequired,
      code: string.isRequired,
      name: string.isRequired,
      email: string.isRequired
    }).isRequired
  ).isRequired,
  fetchList: func.isRequired,
  batchDelete: func.isRequired,
  gotoCreatePage: func.isRequired,
  gotoUpdatePage: func.isRequired
}

export default connect(
  state => ({ users: state.user.list }),
  {
    fetchList: userActions.fetchListRequest,
    batchDelete: userActions.deleteRequest,
    gotoCreatePage: () => gotoPage(paths.CREATE_USER),
    gotoUpdatePage: id => gotoPage(getPath(paths.UPDATE_USER, { id }))
  }
)(UserListPage)
