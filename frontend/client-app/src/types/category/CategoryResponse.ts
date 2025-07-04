export interface CategoryResponse {
  id: number;
  categoryName: string;
  parentCategoryId: number | null;
  subCategories: CategoryResponse[];
}
