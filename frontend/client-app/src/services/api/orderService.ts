import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { UserOrderResponse } from '@/types/order/UserOrderResponse'
import type { OrderCreateRequest } from '@/types/order/OrderCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'
import type { UpdateOrderRequest } from '@/types/order/UpdateOrderRequest'
import type {
  ProductStats,
  EarningsStats,
  ProductDetailStats,
} from '@/types/order/StatisticsResponse'

const orderService = {
  async getAll(
    page = 1,
    pageSize = 20,
    userId = null,
    productId = null,
    status = null,
    orderBy = null,
    dateTo = null,
    dateFrom = null,
    sellerId = null,
  ): Promise<ApiResponse<UserOrderResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse[]>>(
        `${API_BASE_URL}/v1/orders`,
        {
          params: {
            page,
            pageSize,
            userId,
            productId,
            status,
            orderBy,
            dateTo,
            dateFrom,
            sellerId,
          },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse[]>()
    }
  },

  async getById(orderId: number): Promise<ApiResponse<UserOrderResponse>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse>>(
        `${API_BASE_URL}/v1/orders/${orderId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse>()
    }
  },

  async getMyOrders(): Promise<ApiResponse<UserOrderResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse[]>>(
        `${API_BASE_URL}/v1/orders/me/byUser`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse[]>()
    }
  },

  async getCurrentUserOrders(): Promise<ApiResponse<UserOrderResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse[]>>(
        `${API_BASE_URL}/v1/orders/me`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse[]>()
    }
  },

  async getOrdersByUserId(userId: number): Promise<ApiResponse<UserOrderResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse[]>>(
        `${API_BASE_URL}/v1/orders/users/${userId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse[]>()
    }
  },

  async createOrder(request: OrderCreateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.post<ApiResponse<null>>(`${API_BASE_URL}/v1/orders`, request)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async updateOrder(orderId: number, request: UpdateOrderRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.patch<ApiResponse<null>>(
        `${API_BASE_URL}/v1/orders/${orderId}`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async getOrdersForSeller(sellerId: number): Promise<ApiResponse<UserOrderResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<UserOrderResponse[]>>(
        `${API_BASE_URL}/v1/orders/seller/${sellerId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserOrderResponse[]>()
    }
  },

  async getMyStatistics(): Promise<
    ApiResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>
  > {
    try {
      const response = await axios.get<
        ApiResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>
      >(`${API_BASE_URL}/v1/orders/me/statistics`)
      return response.data
    } catch (error: any) {
      return (
        error.response?.data ||
        createDefaultErrorResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>()
      )
    }
  },

  async getSellerStatistics(
    sellerId: number,
  ): Promise<ApiResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>> {
    try {
      const response = await axios.get<
        ApiResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>
      >(`${API_BASE_URL}/v1/orders/statistics/${sellerId}`)
      return response.data
    } catch (error: any) {
      return (
        error.response?.data ||
        createDefaultErrorResponse<[ProductStats[], EarningsStats[], ProductDetailStats[]]>()
      )
    }
  },
}

export default orderService
