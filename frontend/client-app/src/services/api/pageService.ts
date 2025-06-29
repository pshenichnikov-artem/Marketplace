import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { PageResponse } from '@/types/page/PageResponse'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const pageService = {
  async getPages(): Promise<ApiResponse<PageResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<PageResponse[]>>(`${API_BASE_URL}/v1/page`)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PageResponse[]>()
    }
  },

  async getPageByKey(pageKey: string, lang: string): Promise<ApiResponse<PageResponse>> {
    try {
      const response = await axios.get<ApiResponse<PageResponse>>(
        `${API_BASE_URL}/v1/page/${pageKey}`,
        {
          params: {
            lang,
          },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PageResponse>()
    }
  },

  async createPage(
    pageKey: string,
    content: string,
    language: string,
  ): Promise<ApiResponse<PageResponse>> {
    try {
      const response = await axios.post<ApiResponse<PageResponse>>(`${API_BASE_URL}/v1/page`, {
        pageKey,
        content,
        language,
      })
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PageResponse>()
    }
  },

  async updatePage(
    pageKey: string,
    content: string,
    language: string,
  ): Promise<ApiResponse<PageResponse>> {
    try {
      const response = await axios.put<ApiResponse<PageResponse>>(`${API_BASE_URL}/v1/page`, {
        pageKey,
        content,
        language,
      })
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PageResponse>()
    }
  },

  async deletePage(pageKey: string): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(`${API_BASE_URL}/v1/page/${pageKey}`)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },
}

export default pageService
