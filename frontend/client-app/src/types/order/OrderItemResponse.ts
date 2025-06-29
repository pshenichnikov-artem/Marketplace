import type { ProductResponse } from '../product/ProductResponse'

export interface OrderItemResponse {
  productId: number
  product: ProductResponse
  orderId: number
  price: number
  quantity: number
}
