import { Input, Typography } from 'antd'
import { func, shape, string } from 'prop-types'
import React from 'react'
import useAsyncAction from '@app/Common/AsyncTask/useAsyncAction'
import { isValidEmail, isValidPassword } from '@app/Common/Utils/validation'
import Box from '@app/Components/Box/Box'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'

const form = {
  getDefaultData: user =>
    user
      ? { name: user.name, email: user.email }
      : {
          code: '',
          name: '',
          email: '',
          password: '',
          confirmPassword: ''
        },
  verifyField: (name, value, fields) => {
    switch (name) {
      case 'code':
      case 'name':
        return !!value
      case 'email':
        return isValidEmail(value)
      case 'password':
        return isValidPassword(value)
      case 'confirmPassword':
        return value && value === fields.password.value
    }
  }
}

function UserForm({ user, onSubmit }) {
  const createMode = !user
  const [{ loading }, onSubmitAsync] = useAsyncAction(onSubmit)
  return (
    <Form
      defaultData={form.getDefaultData(user)}
      verifyField={form.verifyField}
      onSubmit={onSubmitAsync}
    >
      {createMode && (
        <Form.Item label="Code" required>
          <Form.Field name="code">
            <Input />
          </Form.Field>
        </Form.Item>
      )}
      <Form.Item label="Name" required>
        <Form.Field name="name">
          <Input />
        </Form.Field>
      </Form.Item>
      <Form.Item label="Email" required>
        <Form.Field name="email">
          <Input />
        </Form.Field>
      </Form.Item>
      {createMode && (
        <>
          <Form.Item label="Password" required>
            <Form.Field name="password">
              <Input.Password />
            </Form.Field>
          </Form.Item>
          <Form.Item label="Confirm Password" required>
            <Form.Field name="confirmPassword">
              <Input.Password />
            </Form.Field>
          </Form.Item>
          <Box marginBottom={24}>
            <Typography.Text strong>Password Requirements</Typography.Text>
            <Typography.Paragraph>
              <ul>
                <li>Length between 8 and 32 characters.</li>
                <li>One or more uppercase letters.</li>
                <li>One or more lowercase letters.</li>
                <li>One or more numbers.</li>
                <li>
                  One or more special characters (ASCII punctuation or space
                  characters).
                </li>
              </ul>
            </Typography.Paragraph>
          </Box>
        </>
      )}
      <Form.Item>
        <Button.Submit loading={loading} />
      </Form.Item>
    </Form>
  )
}

UserForm.propTypes = {
  user: shape({
    name: string.isRequired,
    email: string.isRequired
  }),
  onSubmit: func.isRequired
}

export default UserForm
