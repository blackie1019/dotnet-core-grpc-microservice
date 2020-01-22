import AppAPI from '@app/Common/Utils/http/AppAPI'

const localizationPath = 'Localization'

const transToTableData = apiData => {
  return apiData.map(item => {
    const rowData = {}

    rowData.displayKey = item.displayKey

    rowData['en-gb'] = item.languageSets.find(
      languageSet => languageSet.langCode === 'en-gb'
    )?.displayValue
    rowData['zh-cn'] = item.languageSets.find(
      languageSet => languageSet.langCode === 'zh-cn'
    )?.displayValue

    return rowData
  })
}

const transToFormData = apiData => {
  const formData = {}

  formData.displayKey = apiData.displayKey
  formData['en-gb'] = apiData.languageSets.find(
    languageSet => languageSet.langCode === 'en-gb'
  )?.displayValue
  formData['zh-cn'] = apiData.languageSets.find(
    languageSet => languageSet.langCode === 'zh-cn'
  )?.displayValue

  return formData
}

const transToRequestBody = form => ({
  'en-gb': {
    displayKey: form.displayKey,
    langCode: 'en-gb',
    displayValue: form['en-gb'] ? form['en-gb'] : ''
  },
  'zh-cn': {
    displayKey: form.displayKey,
    langCode: 'zh-cn',
    displayValue: form['zh-cn'] ? form['zh-cn'] : ''
  }
})

export const fetchLocalizationListAPI = async () => {
  const apiData = await AppAPI.get(`${localizationPath}/GetLanguages`).useAuth()
  return transToTableData(apiData)
}

export const fetchLocalizationAPI = async displayKey => {
  const apiData = await AppAPI.get(
    `${localizationPath}/GetLanguage/${displayKey}`
  ).useAuth()

  return transToFormData(apiData)
}

export const createLocalizationAPI = async form => {
  const requestBodyData = transToRequestBody(form)

  await AppAPI.post(`${localizationPath}/ModifyLanguage`)
    .useAuth()
    .body(requestBodyData['en-gb'])
  await AppAPI.post(`${localizationPath}/ModifyLanguage`)
    .useAuth()
    .body(requestBodyData['zh-cn'])
}

export const updateLocalizationAPI = async form => {
  const requestBodyData = transToRequestBody(form)

  await AppAPI.post(`${localizationPath}/Modifylanguage`)
    .useAuth()
    .body(requestBodyData['en-gb'])
  await AppAPI.post(`${localizationPath}/Modifylanguage`)
    .useAuth()
    .body(requestBodyData['zh-cn'])
}

export const deleteLocalizationsAPI = displayKeys =>
  AppAPI.post(`${localizationPath}/DeleteLanguage/${displayKeys[0]}`).useAuth()
