import { Button as AntdButton, Icon } from 'antd'
import { array, bool, func, oneOfType, string } from 'prop-types'
import React from 'react'
import useAuthroizer from '@app/Common/Authentication/useAuthorizer'

export default function Button({
  policy,
  icon,
  iconTheme,
  iconRight,
  text,
  children,
  ...props
}) {
  const authorized = useAuthroizer(policy)
  if (!authorized) return null

  children = children
    ? React.Children.toArray(children)
    : [<span key="text">{text}</span>]
  if (icon) children.unshift(<Icon key="icon" type={icon} theme={iconTheme} />)
  if (iconRight) children.reverse()
  return <AntdButton {...props}>{children}</AntdButton>
}

Button.propTypes = {
  policy: oneOfType([array, bool, func]),
  icon: AntdButton.propTypes.icon,
  iconTheme: string,
  iconRight: bool,
  text: string
}

Button.Group = AntdButton.Group

Button.Submit = function SubmitButton(props) {
  return <Button {...props} type="primary" htmlType="submit" text="Submit" />
}

Button.Create = function CreateButton(props) {
  return <Button {...props} icon="plus" text="Create" />
}

Button.Actions = function CreateButton(props) {
  return <Button {...props} icon="down" iconRight text="Actions" />
}
