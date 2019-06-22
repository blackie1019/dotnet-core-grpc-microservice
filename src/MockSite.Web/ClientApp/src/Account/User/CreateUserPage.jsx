import { func } from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import Section from '@app/Components/Section/Section'
import userActions from './action'
import UserForm from './UserForm'

function CreateUserPage({ createUser }) {
  return (
    <Section>
      <Section.Back />
      <Section.Title text="Create User" />
      <Section.Body>
        <UserForm onSubmit={createUser} />
      </Section.Body>
    </Section>
  )
}

CreateUserPage.propTypes = {
  createUser: func.isRequired
}

export default connect(
  null,
  { createUser: userActions.createRequest }
)(CreateUserPage)
