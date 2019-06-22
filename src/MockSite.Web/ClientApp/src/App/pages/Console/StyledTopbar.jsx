import { Layout } from 'antd'
import styled from 'styled-components'

const Topbar = styled(Layout.Header)`
  display: flex;
  align-items: center;
  padding: 0 24px 0 48px;
  border-bottom: 1px solid #e4e9f0;
  background-color: #fff;

  .ant-dropdown-trigger {
    padding: 0;
  }

  .ant-breadcrumb {
    font-weight: 700;
    > * a,
    > .ant-breadcrumb-separator:before {
      color: #c0bdd0;
    }
    > *:last-child a {
      color: #0e0b20;
    }
  }

  .ant-breadcrumb-separator:before {
    display: inline-block;
    content: '\\B7';
  }

  .ant-dropdown-trigger .anticon-caret-down svg {
    width: 10px;
    height: 10px;
    color: #d2d9e5;
  }
`

Topbar.Right = styled.div`
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: flex-end;

  > *:not(:last-child) {
    margin-right: 16px;
  }
`

export default Topbar
