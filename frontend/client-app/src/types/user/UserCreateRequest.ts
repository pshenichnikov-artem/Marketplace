export interface UserCreateRequest {
  firstName: string
  lastName: string
  email: string
  phone?: string
  address?: string
  role: string
  password?: string
}
