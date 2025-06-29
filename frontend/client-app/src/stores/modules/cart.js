import { defineStore } from 'pinia'

export const useCartStore = defineStore('cart', {
  state: () => ({
    cartItemsCountDelta: 0,
  }),

  actions: {
    delta(delta) {
      this.cartItemsCountDelta += delta
    },
  },
})
