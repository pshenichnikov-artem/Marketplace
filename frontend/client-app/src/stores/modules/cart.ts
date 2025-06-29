import { defineStore } from 'pinia'

interface CartState {
  cartItemsCountDelta: number
}

export const useCartStore = defineStore('cart', {
  state: (): CartState => ({
    cartItemsCountDelta: 0,
  }),

  actions: {
    delta(delta: number): void {
      this.cartItemsCountDelta += delta
    },
  },
})
