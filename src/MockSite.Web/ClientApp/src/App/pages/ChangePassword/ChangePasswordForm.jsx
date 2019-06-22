import { Avatar, Card, Icon, Input, Typography } from 'antd'
import React from 'react'
import { func, string } from 'prop-types'
import { isValidPassword } from '@app/Common/Utils/validation'
import Box from '@app/Components/Box/Box'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'

const form = {
  defaultData: {
    password: '',
    confirmPassword: ''
  },
  verifyField: (name, value, fields) => {
    switch (name) {
      case 'password':
        return isValidPassword(value)
      case 'confirmPassword':
        return value && value === fields.password.value
    }
  }
}

export default function ChangePasswordForm({ className, username, onSubmit }) {
  return (
    <Card className={className}>
      <Box display="flex" flexDirection="column" alignItems="center">
        <Box display="flex" alignItems="center">
          <Avatar size={80}>{username[0]}</Avatar>
        </Box>
        <Box display="flex" alignItems="center" marginTop={16}>
          <Typography.Title level={3}>{username}</Typography.Title>
        </Box>
      </Box>
      <Form
        defaultData={form.defaultData}
        verifyField={form.verifyField}
        onSubmit={onSubmit}
      >
        <Form.Item>
          <Form.Field name="password">
            <Input.Password
              prefix={<Icon type="lock" />}
              placeholder="Password"
            />
          </Form.Field>
        </Form.Item>
        <Form.Item>
          <Form.Field name="confirmPassword">
            <Input.Password
              prefix={<Icon type="lock" />}
              placeholder="Confirm Password"
            />
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
        <Form.Item>
          <Button.Submit style={{ width: '100%' }} />
        </Form.Item>
      </Form>
    </Card>
  )
}

ChangePasswordForm.propTypes = {
  username: string.isRequired,
  onSubmit: func.isRequired
}
