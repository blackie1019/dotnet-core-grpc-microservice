import { Avatar, Card, Icon, Input, Typography } from 'antd'
import React from 'react'
import { func, string } from 'prop-types'
import Box from '@app/Components/Box/Box'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'

const form = {
  defaultData: {
    password: ''
  },
  verifyField: (name, value) => {
    switch (name) {
      case 'password':
        return !!value
    }
  }
}

export default function LockScreenForm({ className, username, onSubmit }) {
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
          <Button.Submit style={{ width: '100%' }} />
        </Form.Item>
      </Form>
    </Card>
  )
}

LockScreenForm.propTypes = {
  username: string.isRequired,
  onSubmit: func.isRequired
}
