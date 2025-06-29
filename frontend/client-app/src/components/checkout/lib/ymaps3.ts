import * as Vue from 'vue'

// Глобальное объявление для доступа к API Яндекс Карт v3
declare global {
  interface Window {
    ymaps3: any
  }
}

// Типы для работы с картами
export interface MapLocation {
  center?: [number, number]
  zoom?: number
  bounds?: [[number, number], [number, number]]
  duration?: number
}

export interface SearchResult {
  geometry?: {
    coordinates: [number, number]
  }
  properties: {
    name: string
    description: string
  }
}

export type SearchResultCallback = (results: SearchResult[]) => void

// Константы для API геокодера
const GEOCODER_API_URL = 'https://geocode-maps.yandex.ru/1.x/'
const GEOCODER_API_KEY = '024ed6e8-f9dc-4952-aa70-a086fa35dc1a' // Используем тот же ключ для геокодера

// Константы для границ России
const RUSSIA_BOUNDS = [
  [19.484, 41.185], // Юго-западная граница
  [191.014, 81.858], // Северо-восточная граница
]

// Класс для работы с API Яндекс.Карт v3
export class YandexMaps3Service {
  private static _instance: YandexMaps3Service
  private _isLoaded = false
  private _isLoading = false
  private _loadPromise: Promise<any> | null = null
  private _vueComponents: any = null
  private _vuefy: any = null
  private _searchControl: any = null

  private constructor() {}

  public static getInstance(): YandexMaps3Service {
    if (!YandexMaps3Service._instance) {
      YandexMaps3Service._instance = new YandexMaps3Service()
    }
    return YandexMaps3Service._instance
  }

  public get isLoaded(): boolean {
    return this._isLoaded
  }

  public get vueComponents(): any {
    return this._vueComponents
  }

  // Загрузка API Яндекс.Карт v3
  public async loadApi(apiKey: string = '024ed6e8-f9dc-4952-aa70-a086fa35dc1a'): Promise<any> {
    // Если API уже загружен, возвращаем resolved Promise
    if (this._isLoaded && window.ymaps3) {
      return Promise.resolve(window.ymaps3)
    }

    // Если загрузка уже идет, возвращаем существующий Promise
    if (this._isLoading && this._loadPromise) {
      return this._loadPromise
    }

    this._isLoading = true

    // Создаем Promise для загрузки API
    this._loadPromise = new Promise<any>(async (resolve, reject) => {
      try {
        // Проверяем, загружен ли уже скрипт
        if (!window.ymaps3) {
          const script = document.createElement('script')
          script.src = `https://api-maps.yandex.ru/v3/?apikey=${apiKey}&lang=ru_RU`
          script.async = true
          script.onerror = (error) => {
            this._isLoading = false
            this._isLoaded = false
            reject(new Error(`Ошибка загрузки Яндекс.Карт API v3: ${error}`))
          }

          // Добавляем скрипт на страницу и ждем его загрузки
          document.head.appendChild(script)

          // Ожидаем загрузку скрипта
          await new Promise<void>((resolveScript) => {
            script.onload = () => resolveScript()
          })
        }

        // Ждем готовности API
        if (window.ymaps3) {
          await window.ymaps3.ready

          try {
            // Импортируем модуль vuefy для интеграции с Vue
            const ymaps3Vue = await window.ymaps3.import('@yandex/ymaps3-vuefy')
            this._vuefy = ymaps3Vue.vuefy.bindTo(Vue)

            // Получаем базовые компоненты карты
            const baseComponents = this._vuefy.module(window.ymaps3)

            // Загружаем UI-модуль с управляющими элементами
            try {
              const uiThemeModule = await window.ymaps3.import('@yandex/ymaps3-default-ui-theme')
              const uiComponents = this._vuefy.module(uiThemeModule)

              // Сохраняем компонент поиска для последующего использования
              this._searchControl = uiThemeModule.YMapSearchControl

              // Объединяем все компоненты
              this._vueComponents = {
                ...baseComponents,
                ...uiComponents,
              }
            } catch (uiError) {
              console.warn('Не удалось загрузить модуль UI-темы:', uiError)
              this._vueComponents = baseComponents
            }

            this._isLoaded = true
            this._isLoading = false
            resolve(window.ymaps3)
          } catch (importError) {
            this._isLoading = false
            reject(new Error(`Ошибка при импорте модулей Яндекс.Карт: ${importError}`))
          }
        } else {
          this._isLoading = false
          reject(new Error('API Яндекс.Карт v3 не доступно после загрузки скрипта'))
        }
      } catch (error) {
        this._isLoading = false
        reject(new Error(`Ошибка при инициализации Яндекс.Карт: ${error}`))
      }
    })

    return this._loadPromise
  }

  // Функция поиска, используемая для переопределения стандартного поиска в YMapSearchControl
  public createSearchFunction() {
    return async (query: string, options: any = {}) => {
      try {
        // Использовать встроенную функцию поиска из API
        return await window.ymaps3.search(query, {
          // Расширяем область поиска на всю Россию
          boundedBy: [
            [19.484, 41.185], // Юго-западная граница России
            [191.014, 81.858], // Северо-восточная граница России
          ],
          ...options,
        })
      } catch (error) {
        console.error('Ошибка при выполнении поиска:', error)
        return []
      }
    }
  }

  // Создаем функцию-обработчик результатов поиска для YMapSearchControl
  public createSearchResultHandler(
    onSelectMarker: (marker: SearchResult) => void,
    updateMapLocation: (location: MapLocation) => void,
  ) {
    return (searchResults: SearchResult[]) => {
      if (searchResults.length > 0) {
        // Если найден только один результат, выбираем его автоматически
        if (searchResults.length === 1) {
          onSelectMarker(searchResults[0])

          // Обновляем положение карты (центрируем на найденной точке)
          updateMapLocation({
            center: searchResults[0].geometry?.coordinates,
            zoom: 15,
            duration: 400,
          })
        }
        // Если результатов несколько, обновляем карту для отображения всех результатов
        else {
          const bounds = this.calculateBounds(searchResults)
          if (bounds) {
            updateMapLocation({
              bounds,
              duration: 400,
            })
          }
        }
      }
    }
  }

  // Выполнение AJAX-запроса к геокодеру Яндекса
  private async fetchGeocoderData(params: Record<string, string>): Promise<any> {
    // Добавляем обязательные параметры
    const queryParams = new URLSearchParams({
      apikey: GEOCODER_API_KEY,
      format: 'json',
      ...params,
      // Добавляем ограничение поиска территорией России
      bbox: `${RUSSIA_BOUNDS[0][0]},${RUSSIA_BOUNDS[0][1]}~${RUSSIA_BOUNDS[1][0]},${RUSSIA_BOUNDS[1][1]}`,
      rspn: '1', // Ограничить поиск указанной областью
      lang: 'ru_RU', // Локаль русская
    })

    try {
      const response = await fetch(`${GEOCODER_API_URL}?${queryParams.toString()}`)

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data = await response.json()
      return data
    } catch (error) {
      console.error('Ошибка при запросе к API геокодера:', error)
      throw error
    }
  }

  // Получение подсказок адресов через геокодер
  public async getSuggestions(query: string): Promise<
    Array<{
      title: string
      value: string
      coordinates?: [number, number]
    }>
  > {
    if (!this._isLoaded) {
      throw new Error('API Яндекс.Карт v3 не загружено')
    }

    if (!query || query.trim().length < 3) {
      return []
    }

    try {
      const params = {
        geocode: query,
        lang: 'ru_RU',
        results: '10',
        kinds: 'house,street,locality,metro,district', // Добавляем типы искомых объектов
      }

      const data = await this.fetchGeocoderData(params)

      // Проверяем ответ от геокодера
      const featureMembers = data?.response?.GeoObjectCollection?.featureMember || []

      // Преобразуем результаты в формат подсказок
      const suggestions = featureMembers.map((item: any) => {
        const geoObject = item.GeoObject
        const point = geoObject.Point?.pos
        let coordinates: [number, number] | undefined = undefined

        if (point) {
          const coords = point.split(' ').map(parseFloat)
          if (coords.length === 2) {
            // Геокодер возвращает координаты в формате [долгота, широта]
            coordinates = [coords[0], coords[1]]
          }
        }

        // Получаем наиболее релевантное имя объекта
        const name = geoObject.name || ''
        const description = geoObject.description || ''
        const text = geoObject.metaDataProperty?.GeocoderMetaData?.text || ''

        // Формируем заголовок и значение
        const title =
          text || (name && description ? `${name}, ${description}` : name || description || query)
        const value =
          text || (name && description ? `${name}, ${description}` : name || description || query)

        return {
          title,
          value,
          coordinates,
        }
      })

      return suggestions
    } catch (error) {
      console.error('Ошибка при получении подсказок адресов:', error)
      return []
    }
  }

  // Поиск координат по адресу (прямое геокодирование)
  public async geocode(address: string): Promise<[number, number] | null> {
    if (!this._isLoaded) {
      throw new Error('API Яндекс.Карт v3 не загружено')
    }

    try {
      const params = {
        geocode: address,
        lang: 'ru_RU',
        results: '1',
      }

      const data = await this.fetchGeocoderData(params)

      // Проверяем ответ от геокодера
      const featureMembers = data?.response?.GeoObjectCollection?.featureMember
      if (!featureMembers || featureMembers.length === 0) {
        return null
      }

      // Берем первый результат
      const result = featureMembers[0].GeoObject
      const point = result.Point?.pos

      if (!point) {
        return null
      }

      // Преобразуем строку координат "долгота широта" в массив [долгота, широта]
      const coordinates = point.split(' ').map(parseFloat)
      if (coordinates.length === 2) {
        return [coordinates[0], coordinates[1]]
      }

      return null
    } catch (error) {
      console.error('Ошибка геокодирования:', error)
      return null
    }
  }

  // Получение адреса по координатам (обратное геокодирование)
  public async reverseGeocode(coordinates: [number, number]): Promise<{
    address: string
    details?: any
  } | null> {
    if (!this._isLoaded) {
      throw new Error('API Яндекс.Карт v3 не загружено')
    }

    try {
      const [lng, lat] = coordinates
      const params = {
        geocode: `${lng},${lat}`,
        lang: 'ru_RU',
      }

      const data = await this.fetchGeocoderData(params)

      // Проверяем ответ от геокодера
      const featureMembers = data?.response?.GeoObjectCollection?.featureMember
      if (!featureMembers || featureMembers.length === 0) {
        return null
      }

      // Берем первый результат
      const result = featureMembers[0].GeoObject
      const address = result.metaDataProperty?.GeocoderMetaData?.text || `${lng},${lat}`

      // Получаем детали адреса
      const addressDetails =
        result.metaDataProperty?.GeocoderMetaData?.Address?.Components?.reduce(
          (acc: any, item: any) => {
            acc[item.kind] = item.name
            return acc
          },
          {},
        ) || {}

      return {
        address,
        details: addressDetails,
      }
    } catch (error) {
      console.error('Ошибка обратного геокодирования:', error)
      return null
    }
  }

  // Вычисление границ для группы маркеров
  private calculateBounds(markers: SearchResult[]): [[number, number], [number, number]] | null {
    if (!markers || markers.length === 0) return null

    let minLat = 90
    let maxLat = -90
    let minLng = 180
    let maxLng = -180

    markers.forEach((marker) => {
      if (marker.geometry && marker.geometry.coordinates) {
        const [lat, lng] = marker.geometry.coordinates
        minLat = Math.min(minLat, lat)
        maxLat = Math.max(maxLat, lat)
        minLng = Math.min(minLng, lng)
        maxLng = Math.max(maxLng, lng)
      }
    })

    // Добавляем отступ для лучшего отображения
    const latOffset = (maxLat - minLat) * 0.1
    const lngOffset = (maxLng - minLng) * 0.1

    return [
      [minLng - lngOffset, minLat - latOffset],
      [maxLng + lngOffset, maxLat + latOffset],
    ]
  }
}

// Экспортируем синглтон-экземпляр
export const ymaps3Service = YandexMaps3Service.getInstance()
