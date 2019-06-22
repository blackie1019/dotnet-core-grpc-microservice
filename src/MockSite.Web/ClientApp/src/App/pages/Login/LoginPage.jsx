import React from 'react'
import { connect } from 'react-redux'
import { func } from 'prop-types'
import styled from 'styled-components'
import action from '@app/Common/Authentication/action'
import Fading from '@app/Components/Animation/Fading'
import centeredFormStyle from '../FullScreen/centeredFormStyle'
import FullScreenPageContainer from '../FullScreen/FullScreenPageContainer'
import BaseLoginForm from './LoginForm'

const LoginForm = styled(BaseLoginForm)`
  ${centeredFormStyle};
`

function LoginPage({ login }) {
  return (
    <FullScreenPageContainer>
      <Fading in down>
        <LoginForm onSubmit={login} />
      </Fading>
    </FullScreenPageContainer>
  )
}

LoginPage.propTypes = {
  login: func.isRequired
}

export default connect(
  null,
  { login: action.login }
)(LoginPage)
