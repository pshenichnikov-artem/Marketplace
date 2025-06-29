import type { OrderItemRequest } from '@/types/order/OrderItemRequest';

export interface OrderCreateRequest {
  shippingAddress: string;
  items: OrderItemRequest[];
}
