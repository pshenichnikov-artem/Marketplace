export interface ProductCreateRequest {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  sellerId: number | null;
  photos: File[];
  oldPhotos: string[];
  primaryPhotoIndex: number;
}
