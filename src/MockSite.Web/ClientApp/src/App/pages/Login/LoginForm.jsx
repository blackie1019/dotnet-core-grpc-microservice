import { Card, Icon, Input } from 'antd'
import React from 'react'
import { func } from 'prop-types'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'

const form = {
  defaultData: {
    username: '',
    password: ''
  },
  verifyField: (name, value) => {
    switch (name) {
      case 'username':
      case 'password':
        return !!value
    }
  }
}

export default function LoginForm({ className, onSubmit }) {
  return (
    <Card className={className} title="Login">
      <Form
        defaultData={form.defaultData}
        verifyField={form.verifyField}
        onSubmit={onSubmit}
      >
        <Form.Item>
          <Form.Field name="username">
            <Input prefix={<Icon type="user" />} placeholder="Username" />
          </Form.Field>
        </Form.Item>
        <Form.Item>
          <Form.Field name="password">
            <Input.Password
              prefix={<Icon type="lock" />}
              placeholder="Password"
            />
          </Form.Field>
        </Form.Item>
        <Form.Item>
          <Button.Submit style={{ width: '100%' }} />
        </Form.Item>
      </Form>
    </Card>
  )
}

LoginForm.propTypes = {
  onSubmit: func.isRequired
}
