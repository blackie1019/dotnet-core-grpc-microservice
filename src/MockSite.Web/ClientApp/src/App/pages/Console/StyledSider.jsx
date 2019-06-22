import { Layout } from 'antd'
import styled from 'styled-components'

const Sider = styled(Layout.Sider).attrs({
  breakpoint: 'md',
  collapsible: true,
  collapsedWidth: 0,
  theme: 'dark',
  width: 256
})`
  background-color: #000c17;
  box-shadow: 0 0 200px -20px rgba(57, 55, 73, 0.4);

  > .ant-layout-sider-zero-width-trigger {
    position: fixed;
    top: 11px;
    left: 256px;
    background-color: #000c17;
    z-index: 2;
    transition: left 0.2s;
  }

  &.ant-layout-sider-collapsed > .ant-layout-sider-zero-width-trigger {
    left: 0;
  }
`

Sider.Header = styled.div`
  padding: 16px 24px;
  > * {
    margin: 0 !important;
  }
`

Sider.Body = styled.div`
  height: calc(100vh - 60px);
  overflow-y: auto;

  .ant-menu-dark {
    background-color: #000c17;
  }
  .ant-menu-root {
    padding: 16px 0;
  }
  .ant-menu-submenu:not(:last-child) {
    margin-bottom: 12px;
  }
  .ant-menu-submenu-open,
  .ant-menu-submenu-selected,
  .ant-menu-submenu:hover {
    > .ant-menu-submenu-title > .anticon:nth-child(2) {
      color: #fff;
    }
  }
  .ant-menu-submenu-title {
    display: flex;
    align-items: center;
    transition: none;
    > :first-child {
      flex: 1;
      font-size: 1rem;
    }
    > :nth-child(2) {
      position: absolute;
      top: calc((100% - 24px) / 2);
      right: 16px;
      margin: 0 !important;
      width: 24px;
      height: 24px;
      &.anticon {
        padding: 5px;
        color: #b8beca;
        > svg {
          width: 100%;
          height: 100%;
        }
      }
    }
    > :nth-child(3) {
      right: 48px !important;
    }
  }

  .ant-menu-sub .ant-menu-item {
    display: flex !important;
    align-items: center;
    min-height: 40px;
    height: auto !important;
    font-size: 1rem;
    line-height: 2 !important;
    white-space: normal !important;
    user-select: none;
    transition: none;
    &:after {
      left: 0;
      right: auto;
      border-right: 3px solid #fff !important;
    }
  }
`

export default Sider
