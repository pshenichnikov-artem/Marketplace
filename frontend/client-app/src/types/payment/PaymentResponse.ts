export interface PaymentResponse {
  id: number;
  orderId: number;
  paymentDate: string;
  paymentMethod: string;
  amount: number;
}
