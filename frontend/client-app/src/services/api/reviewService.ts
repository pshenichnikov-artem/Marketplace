import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { ReviewResponse } from '@/types/review/ReviewResponse'
import type { ReviewCreateRequest } from '@/types/review/ReviewCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const reviewService = {
  async getReviews(
    productId: number,
    userId: number,
    search: string,
    page: number,
    pageSize: number,
    orderBy: string, //userId, productId, createdAt, comment, rating
    orderDirection: string, //asc, desc
  ): Promise<ApiResponse<{ comments: ReviewResponse[]; totalCount: number }>> {
    try {
      const response = await axios.get<
        ApiResponse<{ comments: ReviewResponse[]; totalCount: number }>
      >(`${API_BASE_URL}/v1/review`, {
        params: { productId, userId, search, page, pageSize, orderBy, orderDirection },
      })
      return response.data
    } catch (error: any) {
      return (
        error.response?.data ||
        createDefaultErrorResponse<{ comments: ReviewResponse[]; totalCount: number }>()
      )
    }
  },

  async getMyReviewByProduct(productId: number): Promise<ApiResponse<ReviewResponse>> {
    try {
      const response = await axios.get<ApiResponse<ReviewResponse>>(
        `${API_BASE_URL}/v1/review/me/byProduct`,
        {
          params: { productId },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ReviewResponse>()
    }
  },

  async getMyReview(): Promise<ApiResponse<ReviewResponse>> {
    try {
      const response = await axios.get<ApiResponse<ReviewResponse>>(
        `${API_BASE_URL}/v1/review/me`,
        {},
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ReviewResponse>()
    }
  },

  async getReviewByUserAndProductIds(
    userId: number,
    productId: number,
  ): Promise<ApiResponse<ReviewResponse>> {
    try {
      const response = await axios.get<ApiResponse<ReviewResponse>>(
        `${API_BASE_URL}/v1/review/byUserAndProduct`,
        {
          params: { userId, productId },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ReviewResponse>()
    }
  },

  async canLeaveReview(productId: number): Promise<ApiResponse<boolean>> {
    try {
      const response = await axios.get<ApiResponse<boolean>>(
        `${API_BASE_URL}/v1/review/me/can-leave`,
        {
          params: { productId },
        },
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<boolean>()
    }
  },

  async createReview(request: ReviewCreateRequest): Promise<ApiResponse<ReviewResponse>> {
    try {
      const response = await axios.post<ApiResponse<ReviewResponse>>(
        `${API_BASE_URL}/v1/review`,
        request,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<ReviewResponse>()
    }
  },

  async deleteReviewById(reviewId: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(
        `${API_BASE_URL}/v1/review/${reviewId}`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },

  async deleteMyReview(productId: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(`${API_BASE_URL}/v1/review/me`, {
        params: { productId },
      })
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>()
    }
  },
}

export default reviewService
