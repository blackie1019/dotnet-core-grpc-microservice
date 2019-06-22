import { func, number, object } from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import useDidMount from '@app/Common/Hooks/useDidMount'
import Section from '@app/Components/Section/Section'
import userActions from './action'
import UserForm from './UserForm'

function UpdateUserPage({ userId, user, fetchUser, updateUser }) {
  useDidMount(() => {
    fetchUser(userId)
  })
  return (
    <Section>
      <Section.Back />
      <Section.Title text="Update User" />
      <Section.Body>
        {user && (
          <UserForm
            user={user}
            onSubmit={form => updateUser({ ...form, id: user.id })}
          />
        )}
      </Section.Body>
    </Section>
  )
}

UpdateUserPage.propTypes = {
  userId: number.isRequired,
  user: object,
  fetchUser: func.isRequired,
  updateUser: func.isRequired
}

export default connect(
  (state, props) => ({
    userId: Number(props.match.params.id),
    user: state.user.current
  }),
  {
    fetchUser: userActions.fetchCurrentRequest,
    updateUser: userActions.updateRequest
  }
)(UpdateUserPage)
