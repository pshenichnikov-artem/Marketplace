export interface ProductStats {
  month: string;
  count: number;
}

export interface EarningsStats {
  month: string;
  earnings: string;
}

export interface ProductDetailStats {
  month: string;
  productId: string;
  productName: string;
  soldQuantity: number;
  totalProfit: string;
}
