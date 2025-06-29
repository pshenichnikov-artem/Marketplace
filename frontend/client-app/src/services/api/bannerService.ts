import axios from 'axios'
import { API_BASE_URL } from '@/constants/api'
import type { ApiResponse } from '@/types/ApiResponse'
import type { BannerResponse } from '@/types/banner/BannerResponse'
import type { BannerCreateRequest } from '@/types/banner/BannerCreateRequest'
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse'

const bannerService = {
  async getBanners(): Promise<ApiResponse<BannerResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<BannerResponse[]>>(
        `${API_BASE_URL}/v1/page/banner`,
      )
      return response.data
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<BannerResponse[]>()
    }
  },

  async createBanner(requests: BannerCreateRequest[]): Promise<ApiResponse<BannerResponse>> {
    try {
      const formData = new FormData()

      // Добавляем каждый баннер в формат List<AddBanners>
      requests.forEach((request, index) => {
        formData.append(`Title`, request.title)
        formData.append(`Description`, request.description)
        formData.append(`Link`, request.link)

        // Добавляем файл изображения, как это делается в ProductService
        if (request.image instanceof File) {
          formData.append(`Image`, request.image)
        }
      })

      // Важное отличие: не указываем Content-Type
      const response = await axios.post<ApiResponse<BannerResponse>>(
        `${API_BASE_URL}/v1/page/banner`,
        formData,
      )

      return response.data
    } catch (error: any) {
      console.error('Ошибка при отправке баннера:', error.response || error)
      return error.response?.data || createDefaultErrorResponse<BannerResponse>()
    }
  },
}

export default bannerService
