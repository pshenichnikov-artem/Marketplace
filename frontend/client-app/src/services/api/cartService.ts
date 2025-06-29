import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { CartResponse } from '@/types/cart/CartResponse'
import type { CartItemCreateRequest } from '@/types/cart/CartItemCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const cartService = {
  async getCart(
    userId: string | null = null,
    cartId: number | null = null,
  ): Promise<ApiResponse<CartResponse>> {
    try {
      const response = await axios.get<ApiResponse<CartResponse>>(`${API_BASE_URL}/v1/cart`, {
        params: { userId, cartId },
      })
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<CartResponse>()
    }
  },

  async addCartItem(request: CartItemCreateRequest): Promise<ApiResponse<CartResponse>> {
    try {
      const response = await axios.post<ApiResponse<CartResponse>>(
        `${API_BASE_URL}/v1/cart/items`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<CartResponse>()
    }
  },

  async updateCartItem(itemId: number, request: CartItemCreateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.patch<ApiResponse<null>>(
        `${API_BASE_URL}/v1/cart/items/${itemId}`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async removeCartItem(itemId: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(
        `${API_BASE_URL}/v1/cart/items/${itemId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  // Добавляем метод для удаления всех элементов корзины
  async clearCartItems(itemIds: number[]): Promise<boolean> {
    try {
      // Создаем массив промисов для параллельного удаления всех элементов
      const deletePromises = itemIds.map((id) => this.removeCartItem(id))

      // Ждем завершения всех запросов
      const results = await Promise.all(deletePromises)

      // Проверяем, что все запросы выполнились успешно
      const allSuccessful = results.every((result) => result.status === 'success')
      return allSuccessful
    } catch (error) {
      console.error('Ошибка при очистке корзины:', error)
      return false
    }
  },
}

export default cartService
