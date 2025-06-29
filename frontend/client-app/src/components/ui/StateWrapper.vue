<template>
    <div>
        <!-- Состояние загрузки -->
        <div v-if="loading" class="flex justify-center items-center py-8">
            <slot name="loading">
                <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
            </slot>
        </div>

        <!-- Состояние ошибки -->
        <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
            <slot name="error" :error="error">
                <div class="flex items-center">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-red-500 mr-2 flex-shrink-0" fill="none"
                        viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <span class="text-red-600">{{ typeof error === 'string' ? error : $t('common.errorOccurred')
                        }}</span>
                </div>
            </slot>
        </div>

        <!-- Состояние "нет данных" -->
        <div v-else-if="isEmpty">
            <slot name="empty">
                <div
                    class="bg-gradient-to-r from-blue-50 to-indigo-50 text-blue-700 border border-blue-100 rounded-lg p-8 text-center">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto mb-4 text-blue-400 opacity-80"
                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                    </svg>
                    <p class="text-xl font-medium text-indigo-700 mb-3">{{ emptyMessage || $t('common.noData') }}</p>
                    <slot name="empty-action"></slot>
                </div>
            </slot>
        </div>

        <!-- Содержимое при загруженных данных -->
        <div v-else>
            <slot></slot>
        </div>
    </div>
</template>

<script>
export default {
    name: 'StateWrapper',
    props: {
        loading: {
            type: Boolean,
            default: false
        },
        error: {
            type: [String, Error, Object, Boolean],
            default: null
        },
        isEmpty: {
            type: Boolean,
            default: false
        },
        emptyMessage: {
            type: String,
            default: ''
        }
    }
}
</script>

<style scoped>
.animate-spin {
    animation: spin 1s linear infinite;
}

@keyframes spin {
    from {
        transform: rotate(0deg);
    }

    to {
        transform: rotate(360deg);
    }
}
</style>
