/**
 * Утилита для работы со статусами в приложении
 * Возвращает путь к переводу и класс для подсветки статуса
 */

// Типы статусов
export enum StatusType {
  Positive = 'positive',
  Info = 'info',
  Warning = 'warning',
  Negative = 'negative',
  Default = 'default',
}

// Интерфейс для результата обработки статуса
export interface StatusInfo {
  translationPath: string // Путь к переводу
  cssClass: string // CSS класс для подсветки
  type: StatusType // Тип статуса
}

// Карта соответствия статусов их нормализованным значениям
const statusMap: Record<string, string> = {
  // Заказы
  pending: 'pending',
  'ждет обработки': 'pending',
  processing: 'processing',
  обрабатывается: 'processing',
  intransit: 'inTransit',
  'в пути': 'inTransit',
  shipped: 'inTransit',
  отправлен: 'inTransit',
  waiting: 'pending',
  ожидание: 'pending',
  delivered: 'delivered',
  доставлен: 'delivered',
  cancelled: 'cancelled',
  canceled: 'cancelled',
  отменен: 'cancelled',
  completed: 'completed',
  завершен: 'completed',

  // Продукты
  active: 'active',
  активен: 'active',
  inactive: 'inactive',
  неактивен: 'inactive',
  published: 'published',
  опубликован: 'published',
  draft: 'draft',
  черновик: 'draft',

  // Платежи
  paid: 'paid',
  оплачен: 'paid',
  unpaid: 'unpaid',
  'не оплачен': 'unpaid',
  refunded: 'refunded',
  возвращен: 'refunded',
  failed: 'failed',
  ошибка: 'failed',
}

// Карта соответствия типов статусов
const statusTypeMap: Record<string, StatusType> = {
  pending: StatusType.Warning,
  processing: StatusType.Info,
  inTransit: StatusType.Info,
  delivered: StatusType.Positive,
  cancelled: StatusType.Negative,
  completed: StatusType.Positive,
  active: StatusType.Positive,
  inactive: StatusType.Negative,
  published: StatusType.Positive,
  draft: StatusType.Warning,
  paid: StatusType.Positive,
  unpaid: StatusType.Warning,
  refunded: StatusType.Info,
  failed: StatusType.Negative,
}

// Карта соответствия типов статусов CSS классам
const statusCssMap: Record<StatusType, string> = {
  [StatusType.Positive]: 'bg-green-100 text-green-800',
  [StatusType.Info]: 'bg-blue-100 text-blue-800',
  [StatusType.Warning]: 'bg-yellow-100 text-yellow-800',
  [StatusType.Negative]: 'bg-red-100 text-red-800',
  [StatusType.Default]: 'bg-gray-100 text-gray-800',
}

/**
 * Нормализует строку статуса
 * @param status Исходная строка статуса
 * @returns Нормализованная строка статуса
 */
export function normalizeStatus(status: string): string {
  if (!status) return ''

  const normalizedStatus = status.toLowerCase().trim()
  return statusMap[normalizedStatus] || normalizedStatus
}

/**
 * Определяет тип статуса
 * @param normalizedStatus Нормализованный статус
 * @returns Тип статуса
 */
export function getStatusType(normalizedStatus: string): StatusType {
  return statusTypeMap[normalizedStatus] || StatusType.Default
}

/**
 * Возвращает CSS класс для статуса
 * @param type Тип статуса
 * @returns CSS класс
 */
export function getStatusCssClass(type: StatusType): string {
  return statusCssMap[type]
}

/**
 * Возвращает путь к переводу статуса
 * @param normalizedStatus Нормализованный статус
 * @param context Контекст использования (orders, products, etc)
 * @returns Путь к переводу
 */
export function getStatusTranslationPath(
  normalizedStatus: string,
  context: string = 'common',
): string {
  // Первая буква статуса всегда заглавная
  const capitalizedStatus = normalizedStatus.charAt(0).toUpperCase() + normalizedStatus.slice(1)
  return `status.${context}.${capitalizedStatus}`
}

/**
 * Главная функция для получения информации о статусе
 * @param status Исходный статус
 * @param context Контекст использования (orders, products, etc)
 * @returns Объект с информацией о статусе
 */
export function getStatusInfo(status: string, context: string = 'common'): StatusInfo {
  const normalizedStatus = normalizeStatus(status)
  const type = getStatusType(normalizedStatus)
  const cssClass = getStatusCssClass(type)
  const translationPath = getStatusTranslationPath(normalizedStatus, context)

  return {
    translationPath,
    cssClass,
    type,
  }
}
