import { ref } from 'vue'
import { useI18n } from 'vue-i18n'

/**
 * Хук для выполнения API запросов с обработкой состояний
 */
export default function useApiRequest() {
  const loading = ref(false)
  const error = ref(null)
  const data = ref(null)

  const { t } = useI18n()

  /**
   * Выполнение API запроса
   * @param {Function} apiCall - Функция запроса к API
   * @param {Object} options - Дополнительные опции
   * @returns {Object} Результат запроса
   */
  const execute = async (apiCall, options = {}) => {
    const {
      showErrorNotification = false,
      notificationRef = null,
      resetData = true,
      transformResponse = null,
      onSuccess = null,
      onError = null,
    } = options

    if (resetData) {
      data.value = null
    }

    loading.value = true
    error.value = null

    try {
      const response = await apiCall()

      if (response.status === 'success') {
        data.value = transformResponse ? transformResponse(response.data) : response.data
        if (onSuccess) onSuccess(data.value)
        return { success: true, data: data.value }
      } else {
        error.value = response.message || t('errorOccurred')

        if (showErrorNotification && notificationRef?.value) {
          notificationRef.value.error(error.value)
        }

        if (onError) onError(error.value)
        return { success: false, error: error.value }
      }
    } catch (err) {
      const errorMessage = err.message || t('unexpectedError')
      error.value = errorMessage

      if (showErrorNotification && notificationRef?.value) {
        notificationRef.value.error(errorMessage)
      }

      if (onError) onError(errorMessage)
      return { success: false, error: errorMessage }
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    data,
    execute,
  }
}
