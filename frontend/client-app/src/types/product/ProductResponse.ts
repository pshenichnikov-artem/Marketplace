import type {SellerResponse} from "@/types/user/SellerResponse.ts";
import type {ProductImageResponse} from "@/types/Image/ProductImageResponse.ts";

export interface ProductResponse {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  seller: SellerResponse;
  rating: number;
  reviewCount: number;
  images: ProductImageResponse[];
}
