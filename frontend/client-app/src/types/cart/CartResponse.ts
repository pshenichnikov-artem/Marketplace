import type { CartItemResponse } from './CartItemResponse';
import type { UserSelfResponse } from '@/types/user/UserSelfResponse';

export interface CartResponse {
  id: number;
  items: CartItemResponse[];
  user: UserSelfResponse;
}
