import { createGlobalStyle } from 'styled-components'

export default createGlobalStyle`
  html, body {
    font-size: 13px;
    overflow: hidden;
  }

  *::-webkit-scrollbar {
    width: 0;
    height: 0;
  }

  // ==========
  // Antd Input
  // ==========

  form .ant-input-number,
  td .ant-input-number {
    width: 100%;
  }
  .ant-input.readonly {
    border-style: dashed;
    border-color: #d9d9d9 !important;
    box-shadow: none !important;
    cursor: default;
  }

  // ==========
  // Antd Table
  // ==========

  .ant-table-body {
    overflow-x: auto;
  }
  .ant-table-thead > tr > th {
    padding: 8px 16px !important;
  }
  .ant-table-tbody > tr > td {
    padding: 6px 16px !important;
  }
  .ant-table-tbody > tr.ant-table-row-selected > td {
    background-color: #fff !important;
  }
  .ant-table-header-column,
  .ant-table-column-sorters {
    display: flex !important;
    align-items: center;
    flex: 1;
  }
  .ant-table-column-title {
    flex: 1;
    display: flex !important;
    font-size: 12px;
  }
  .ant-table-selection-column .ant-table-header-column {
    justify-content: center;
  }
  .ant-table-column-sorter-inner {
    margin-top: -4px !important;
    transform: translateX(8px);
  }

  // =============
  // Antd Collapse
  // =============

  .ant-collapse {
    border-radius: 8px;
  }
  .ant-collapse > .ant-collapse-item:last-child,
  .ant-collapse > .ant-collapse-item:last-child > .ant-collapse-header,
  .ant-collapse-item:last-child > .ant-collapse-content {
    border-radius: 0 0 8px 8px;
  }

  // =========
  // Antd Card
  // =========

  .ant-card {
    border-radius: 8px !important;
  }
`
