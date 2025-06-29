export interface ReviewCreateRequest {
  userId: number;
  productId: number;
  rating: number;
  comment: string;
}
