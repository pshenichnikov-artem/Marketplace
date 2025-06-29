import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const historyService = {
  async getRecentlyViewed(): Promise<ApiResponse<any[]>> {
    try {
      const response = await axios.get<ApiResponse<any[]>>(`${API_BASE_URL}/v1/history`)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<any[]>()
    }
  },
}

export default historyService
