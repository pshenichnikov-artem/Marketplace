import type { ProductResponse } from '@/types/product/ProductResponse';

export interface CartItemResponse {
  id: number;
  product: ProductResponse;
  quantity: number;
}
