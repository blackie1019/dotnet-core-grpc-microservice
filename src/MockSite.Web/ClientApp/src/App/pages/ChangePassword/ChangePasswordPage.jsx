import React from 'react'
import { connect } from 'react-redux'
import { func, string } from 'prop-types'
import styled from 'styled-components'
import action from '@app/Common/Authentication/action'
import Fading from '@app/Components/Animation/Fading'
import centeredFormStyle from '../FullScreen/centeredFormStyle'
import FullScreenPageContainer from '../FullScreen/FullScreenPageContainer'
import BaseChangePasswordForm from './ChangePasswordForm'

const ChangePasswordForm = styled(BaseChangePasswordForm)`
  ${centeredFormStyle};
`

function ChangePasswordPage({ username, changePassword }) {
  return (
    <FullScreenPageContainer>
      <Fading in down>
        <ChangePasswordForm username={username} onSubmit={changePassword} />
      </Fading>
    </FullScreenPageContainer>
  )
}

ChangePasswordPage.propTypes = {
  username: string.isRequired,
  changePassword: func.isRequired
}

export default connect(
  state => ({ username: state.authentication.credential?.name }),
  { changePassword: action.changePassword }
)(ChangePasswordPage)
