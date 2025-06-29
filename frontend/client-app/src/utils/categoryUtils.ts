export interface Category {
  id: number | string
  categoryName: string
  subCategories?: Category[]
}

export interface TreeNode {
  id: number | string | null
  label: string
  children: TreeNode[]
}

export function prepareCategoryOptions(categories: Category[], defaultText: string): TreeNode[] {
  const buildTree = (categories: Category[]): TreeNode[] => {
    return categories.map(
      (category): TreeNode => ({
        id: category.id,
        label: category.categoryName,
        children:
          category.subCategories && category.subCategories.length
            ? buildTree(category.subCategories)
            : [],
      }),
    )
  }

  const tree = buildTree(categories)
  return [{ id: null, label: defaultText, children: [] }, ...tree]
}
