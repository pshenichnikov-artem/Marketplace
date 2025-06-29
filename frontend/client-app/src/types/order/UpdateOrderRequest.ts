import type { OrderItemRequest } from '@/types/order/OrderItemRequest'

export interface UpdateOrderRequest {
  shippingAddress: string
  status: string
  items: OrderItemRequest[]
}
