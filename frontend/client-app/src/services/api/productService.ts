import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { ProductResponse } from '@/types/product/ProductResponse'
import type { ProductCreateRequest } from '@/types/product/ProductCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const productService = {
  async getProducts(
    page = 0,
    search?: string,
    orderBy?: string,
    categoryId?: number,
    sellerId?: number,
    pageSize?: number,
    minPrice?: number,
    maxPrice?: number,
  ): Promise<ApiResponse<ProductResponse>> {
    try {
      const params: Record<string, any> = {}

      if (search) params.search = search
      if (orderBy) params.orderBy = orderBy
      if (categoryId) params.categoryId = categoryId
      if (sellerId) params.sellerId = sellerId
      if (page) params.page = page
      if (pageSize) params.pageSize = pageSize
      if (minPrice) params.minPrice = minPrice
      if (maxPrice) params.maxPrice = maxPrice

      const response = await axios.get<ApiResponse<ProductResponse>>(
        `${API_BASE_URL}/v1/products`,
        { params },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ProductResponse>()
    }
  },

  async getProductById(id: number): Promise<ApiResponse<ProductResponse>> {
    try {
      const response = await axios.get<ApiResponse<ProductResponse>>(
        `${API_BASE_URL}/v1/products/${id}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ProductResponse>()
    }
  },

  async createProduct(request: ProductCreateRequest): Promise<ApiResponse<ProductResponse>> {
    try {
      const formData = new FormData()

      // Простые текстовые поля
      formData.append('Name', request.name)
      formData.append('Description', request.description)
      formData.append('Price', String(request.price))
      formData.append('StockQuantity', String(request.stockQuantity))
      formData.append('CategoryId', String(request.categoryId))
      if (request.sellerId != null) {
        formData.append('SellerId', String(request.sellerId))
      }
      formData.append('PrimaryPhotoIndex', String(request.primaryPhotoIndex))

      // Фото (файлы)
      request.photos.forEach((file) => {
        formData.append('Photos', file) // Ключ Photos повторяется для каждого файла
      })

      // Старые фото (строки)
      request.oldPhotos.forEach((url) => {
        formData.append('OldPhotos', url)
      })

      // ⚠️ Не указываем Content-Type вручную
      const response = await axios.post<ApiResponse<ProductResponse>>(
        `${API_BASE_URL}/v1/products`,
        formData,
      )

      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ProductResponse>()
    }
  },

  async updateProduct(id: number, request: ProductCreateRequest): Promise<ApiResponse<null>> {
    try {
      const formData = new FormData()
      Object.entries(request).forEach(([key, value]) => {
        if (Array.isArray(value)) {
          value.forEach((item) => formData.append(key, item))
        } else {
          formData.append(key, value as string | Blob)
        }
      })

      const response = await axios.put<ApiResponse<null>>(
        `${API_BASE_URL}/v1/products/${id}`,
        formData,
        {
          headers: { 'Content-Type': 'multipart/form-data' },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async deleteProduct(id: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(`${API_BASE_URL}/v1/products/${id}`)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },
}

export default productService
