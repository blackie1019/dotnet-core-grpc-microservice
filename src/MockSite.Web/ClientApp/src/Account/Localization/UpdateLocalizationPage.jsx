import { Input } from 'antd'
import { func, string, object } from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import useDidMount from '@app/Common/Hooks/useDidMount'
import useAsyncAction from '@app/Common/AsyncTask/useAsyncAction'
import Section from '@app/Components/Section/Section'
import Form from '@app/Components/Form/Form'
import Button from '@app/Components/Button/Button'
import localizationActions from './action'

const form = {
  getDefaultData: localization =>
    localization
      ? { 'en-gb': localization['en-gb'], 'zh-cn': localization['zh-cn'] }
      : { 'en-gb': '', 'zh-cn': '' },
  verifyField: (name, value) => {
    switch (name) {
      case 'zh-cn':
      case 'en-gb':
        return !!value
    }
  }
}

function UpdateLocalizationPage({
  displayKey,
  localization,
  fetchLocalization,
  updateLocalization
}) {
  const [{ loading }, onSubmitAsync] = useAsyncAction(form =>
    updateLocalization({ ...form, displayKey })
  )
  useDidMount(() => {
    fetchLocalization(displayKey)
  })

  return (
    <Section>
      <Section.Back />
      <Section.Title text={`Update ${displayKey}`} />
      <Section.Body>
        {localization && (
          <Form
            defaultData={form.getDefaultData(localization)}
            verifyField={form.verifyField}
            onSubmit={onSubmitAsync}
          >
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
        )}
      </Section.Body>
    </Section>
  )
}

UpdateLocalizationPage.propTypes = {
  updateLocalization: func.isRequired,
  fetchLocalization: func.isRequired,
  displayKey: string.isRequired,
  localization: object
}

export default connect(
  (state, props) => ({
    displayKey: props.match.params.displayKey,
    localization: state.localization.current
  }),
  {
    updateLocalization: localizationActions.updateRequest,
    fetchLocalization: localizationActions.fetchCurrentRequest
  }
)(UpdateLocalizationPage)
