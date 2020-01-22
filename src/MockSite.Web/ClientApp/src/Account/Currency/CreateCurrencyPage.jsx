import { Input } from 'antd'
import { func } from 'prop-types'
import React from 'react'
import useAsyncAction from '@app/Common/AsyncTask/useAsyncAction'
import { connect } from 'react-redux'
import Section from '@app/Components/Section/Section'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'
import currencyActions from './action'

const form = {
  getDefaultData: () => ({
    currencyCode: '',
    currencyRate: ''
  }),
  verifyField: (name, value) => {
    switch (name) {
      case 'currencyCode':
      case 'currencyRate':
        return !!value
    }
  }
}

function CreateCurrencyPage({ createCurrency }) {
  const [{ loading }, createCurrencyAsync] = useAsyncAction(createCurrency)

  return (
    <Section>
      <Section.Back />
      <Section.Title text="Create Currency" />
      <Section.Body>
        <Form
          defaultData={form.getDefaultData()}
          verifyField={form.verifyField}
          onSubmit={createCurrencyAsync}
        >
          <Form.Item label="Currency Code" required>
            <Form.Field name="currencyCode">
              <Input />
            </Form.Field>
          </Form.Item>
          <Form.Item label="Currency Rate" required>
            <Form.Field name="currencyRate">
              <Input />
            </Form.Field>
          </Form.Item>
          <Form.Item>
            <Button.Submit loading={loading} />
          </Form.Item>
        </Form>
      </Section.Body>
    </Section>
  )
}

CreateCurrencyPage.propTypes = {
  createCurrency: func.isRequired
}

export default connect(
  null,
  { createCurrency: currencyActions.createRequest }
)(CreateCurrencyPage)
