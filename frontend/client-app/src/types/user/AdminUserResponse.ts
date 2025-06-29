export interface AdminUserResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  role: string;
  address: string;
  createdAt: string; // Используем string для даты
}
