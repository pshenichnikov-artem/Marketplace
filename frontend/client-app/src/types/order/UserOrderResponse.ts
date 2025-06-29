import type { PaymentResponse } from '@/types/payment/PaymentResponse'
import type { OrderItemResponse } from '@/types/order/OrderItemResponse'
import type { UserSelfResponse } from '../user/UserSelfResponse'

export interface UserOrderResponse {
  id: number
  orderDate: string
  status: string
  customer: UserSelfResponse
  payment?: PaymentResponse
  items: OrderItemResponse[]
}
