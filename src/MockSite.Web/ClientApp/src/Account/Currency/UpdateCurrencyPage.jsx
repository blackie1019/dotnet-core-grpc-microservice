import { Input } from 'antd'
import { func, number, object } from 'prop-types'
import React from 'react'
import useAsyncAction from '@app/Common/AsyncTask/useAsyncAction'
import { connect } from 'react-redux'
import useDidMount from '@app/Common/Hooks/useDidMount'
import Section from '@app/Components/Section/Section'
import Button from '@app/Components/Button/Button'
import Form from '@app/Components/Form/Form'
import currencyActions from './action'

const form = {
  getDefaultData: currency =>
    currency ? { currencyRate: currency.currencyRate } : { currencyRate: '' },
  verifyField: (name, value) => {
    switch (name) {
      case 'currencyRate':
        return !!value
    }
  }
}

function UpdateCurrencyPage({
  currencyCode,
  currency,
  fetchCurrency,
  updateCurrency
}) {
  const [{ loading }, updateCurrencyAsync] = useAsyncAction(form =>
    updateCurrency({ ...form, currencyCode })
  )
  useDidMount(() => {
    fetchCurrency(currencyCode)
  })

  return (
    <Section>
      <Section.Back />
      <Section.Title text="Update Currency" />
      <Section.Body>
        {currency && (
          <Form
            defaultData={form.getDefaultData(currency)}
            verifyField={form.verifyField}
            onSubmit={updateCurrencyAsync}
          >
            <Form.Item label={`${currencyCode} Currency Rate`} required>
              <Form.Field name="currencyRate">
                <Input />
              </Form.Field>
            </Form.Item>
            <Form.Item>
              <Button.Submit loading={loading} />
            </Form.Item>
          </Form>
        )}
      </Section.Body>
    </Section>
  )
}

UpdateCurrencyPage.propTypes = {
  currencyCode: number.isRequired,
  currency: object,
  fetchCurrency: func.isRequired,
  updateCurrency: func.isRequired
}

export default connect(
  (state, props) => ({
    currencyCode: props.match.params.currencyCode,
    currency: state.currency.current
  }),
  {
    fetchCurrency: currencyActions.fetchCurrentRequest,
    updateCurrency: currencyActions.updateRequest
  }
)(UpdateCurrencyPage)
