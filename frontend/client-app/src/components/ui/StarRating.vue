<template>
    <div class="flex items-center">
        <div v-for="i in 5" :key="i" class="flex-shrink-0">
            <svg :width="size" :height="size" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg"
                :class="i <= fullStars ? 'text-yellow-400' : i <= fullStars + 0.5 ? 'text-yellow-400' : 'text-gray-300'">
                <path
                    d="M9.04894 2.92707C9.3483 2.00576 10.6517 2.00576 10.9511 2.92707L12.0206 6.21886C12.1545 6.63089 12.5385 6.90983 12.9717 6.90983H16.4329C17.4016 6.90983 17.8044 8.14945 17.0207 8.71886L14.2205 10.7533C13.87 11.0079 13.7234 11.4593 13.8572 11.8713L14.9268 15.1631C15.2261 16.0844 14.1717 16.8506 13.388 16.2812L10.5878 14.2467C10.2373 13.9921 9.7627 13.9921 9.41221 14.2467L6.61204 16.2812C5.82833 16.8506 4.77385 16.0844 5.0732 15.1631L6.14277 11.8713C6.27665 11.4593 6.12999 11.0079 5.7795 10.7533L2.97933 8.71886C2.19562 8.14945 2.59839 6.90983 3.56712 6.90983H7.02832C7.46154 6.90983 7.84548 6.63089 7.97936 6.21886L9.04894 2.92707Z"
                    :fill="i <= fullStars ? 'currentColor' : i <= fullStars + 0.5 ? 'currentColor' : 'none'"
                    stroke="currentColor" stroke-width="1.5" />
            </svg>
        </div>
        <span v-if="showValue" class="ml-1 text-sm font-medium text-gray-700">{{ rating.toFixed(1) }}</span>
    </div>
</template>

<script lang="ts">
import { computed, defineComponent } from 'vue';

export default defineComponent({
    name: 'StarRating',
    props: {
        rating: {
            type: Number,
            required: true
        },
        size: {
            type: Number,
            default: 24
        },
        showValue: {
            type: Boolean,
            default: false
        }
    },
    setup(props) {
        const fullStars = computed(() => Math.floor(props.rating));

        return {
            fullStars
        };
    }
});
</script>
