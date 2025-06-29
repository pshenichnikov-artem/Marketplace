import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { CategoryResponse } from '@/types/category/CategoryResponse'
import type { CategoryCreateRequest } from '@/types/category/CategoryCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const categoryService = {
  async getAll(
    search: string | null = null,
    parentId: number | null = null,
  ): Promise<ApiResponse<CategoryResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<CategoryResponse[]>>(
        `${API_BASE_URL}/v1/categories`,
        {
          params: { search, parentId },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<CategoryResponse[]>()
    }
  },

  async getById(categoryId: number): Promise<ApiResponse<CategoryResponse>> {
    try {
      const response = await axios.get<ApiResponse<CategoryResponse>>(
        `${API_BASE_URL}/v1/categories/${categoryId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<CategoryResponse>()
    }
  },

  async create(request: CategoryCreateRequest): Promise<ApiResponse<CategoryResponse>> {
    try {
      const response = await axios.post<ApiResponse<CategoryResponse>>(
        `${API_BASE_URL}/v1/categories`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<CategoryResponse>()
    }
  },

  async update(categoryId: number, request: CategoryCreateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.put<ApiResponse<null>>(
        `${API_BASE_URL}/v1/categories/${categoryId}`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async deleteCategory(categoryId: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(
        `${API_BASE_URL}/v1/categories/${categoryId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },
}

export default categoryService
