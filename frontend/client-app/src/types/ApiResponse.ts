import type { ApiError } from '@/types/ApiError';

export interface ApiResponse<T> {
  status: string;
  message: string;
  data?: T;
  error?: ApiError;
}
