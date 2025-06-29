export interface PaymentCreateRequest {
  orderId: number;
  amount: number;
  paymentMethod: string;
}
