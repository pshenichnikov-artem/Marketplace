export interface ReviewResponse {
  id: number;
  userId: number;
  firstName: string;
  lastName: string;
  productId: number;
  rating: number;
  comment: string;
  createdAt: string; // Используем string для даты
}
