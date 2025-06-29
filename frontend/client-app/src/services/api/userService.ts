import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { AdminUserResponse } from '@/types/user/AdminUserResponse'
import type { UserSelfResponse } from '@/types/user/UserSelfResponse'
import type { UserUpdateRequest } from '@/types/user/UserUpdateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse.ts'

const userService = {
  async getUsers(
    page = 0,
    search: string | null = null,
    role = 'user',
  ): Promise<ApiResponse<AdminUserResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<AdminUserResponse[]>>(
        `${API_BASE_URL}/v1/user`,
        {
          params: { page, search, role },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<AdminUserResponse[]>()
    }
  },

  async getSelf(): Promise<ApiResponse<UserSelfResponse>> {
    try {
      const response = await axios.get<ApiResponse<UserSelfResponse>>(
        `${API_BASE_URL}/v1/user/self`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserSelfResponse>()
    }
  },

  async getUserById(id: number): Promise<ApiResponse<UserSelfResponse>> {
    try {
      const response = await axios.get<ApiResponse<UserSelfResponse>>(
        `${API_BASE_URL}/v1/user/${id}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<UserSelfResponse>()
    }
  },

  async updateUser(id: number, request: UserUpdateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.put<ApiResponse<null>>(`${API_BASE_URL}/v1/user/${id}`, request)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async updateUserSelf(request: UserUpdateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.put<ApiResponse<null>>(`${API_BASE_URL}/v1/user`, request)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async deleteUser(id: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(`${API_BASE_URL}/v1/user/${id}`)
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },
}

export default userService
