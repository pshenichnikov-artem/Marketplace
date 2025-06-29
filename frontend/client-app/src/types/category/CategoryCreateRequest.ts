export interface CategoryCreateRequest {
  categoryName: string;
  parentCategoryId?: number | null;
}
