import { Input } from 'antd'
import { func } from 'prop-types'
import React from 'react'
import useAsyncAction from '@app/Common/AsyncTask/useAsyncAction'
import { connect } from 'react-redux'
import Section from '@app/Components/Section/Section'
import Form from '@app/Components/Form/Form'
import Button from '@app/Components/Button/Button'
import localizationActions from './action'

const form = {
  getDefaultData: () => ({
    displayKey: '',
    ['en-gb']: '',
    ['zh-cn']: ''
  }),
  verifyField: (name, value) => {
    switch (name) {
      case 'displayKey':
      case 'zh-cn':
      case 'en-gb':
        return !!value
    }
  }
}

function CreateLocalizationPage({ createLocalization }) {
  const [{ loading }, onSubmitAsync] = useAsyncAction(createLocalization)

  return (
    <Section>
      <Section.Back />
      <Section.Title text="Create Localization" />
      <Section.Body>
        <Form
          defaultData={form.getDefaultData()}
          verifyField={form.verifyField}
          onSubmit={onSubmitAsync}
        >
          <Form.Item label="DisplayKey" required>
            <Form.Field name="displayKey">
              <Input />
            </Form.Field>
          </Form.Item>
          <Form.Item label="en-gb" required>
            <Form.Field name="en-gb">
              <Input />
            </Form.Field>
          </Form.Item>
          <Form.Item label="zh-cn" required>
            <Form.Field name="zh-cn">
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

CreateLocalizationPage.propTypes = {
  createLocalization: func.isRequired
}

export default connect(
  null,
  { createLocalization: localizationActions.createRequest }
)(CreateLocalizationPage)
