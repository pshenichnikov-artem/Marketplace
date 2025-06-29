import axios from 'axios';
import { API_BASE_URL } from '@/constants/api';
import type { ApiResponse } from '@/types/ApiResponse';
import type { PaymentResponse } from '@/types/payment/PaymentResponse';
import type { PaymentCreateRequest } from '@/types/payment/PaymentCreateRequest';
import { createDefaultErrorResponse } from '@/utils/createDefaultErrorResponse';

const paymentService = {
  async getPayments(): Promise<ApiResponse<PaymentResponse[]>> {
    try {
      const response = await axios.get<ApiResponse<PaymentResponse[]>>(`${API_BASE_URL}/v1/payments`);
      return response.data;
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PaymentResponse[]>();
    }
  },

  async getPaymentByOrderId(orderId: number): Promise<ApiResponse<PaymentResponse>> {
    try {
      const response = await axios.get<ApiResponse<PaymentResponse>>(`${API_BASE_URL}/v1/payments/order/${orderId}`);
      return response.data;
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<PaymentResponse>();
    }
  },

  async createPayment(request: PaymentCreateRequest): Promise<ApiResponse<null>> {
    try {
      const response = await axios.post<ApiResponse<null>>(`${API_BASE_URL}/v1/payments`, request);
      return response.data;
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>();
    }
  },

  async deletePayment(id: number): Promise<ApiResponse<null>> {
    try {
      const response = await axios.delete<ApiResponse<null>>(`${API_BASE_URL}/v1/payments/${id}`);
      return response.data;
    } catch (error: any) {
      return error.response?.data || createDefaultErrorResponse<null>();
    }
  },
};

export default paymentService;
