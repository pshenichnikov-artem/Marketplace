<template>
    <div class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 transition-opacity">
        <div class="bg-white rounded-lg shadow-xl max-w-md mx-auto w-full transform transition-all overflow-hidden"
            @click.stop>
            <div class="px-6 py-5 border-b border-gray-200">
                <h3 class="text-lg font-medium text-gray-900">{{ title }}</h3>
            </div>
            <div class="px-6 py-4">
                <p class="text-gray-700">{{ message }}</p>
            </div>
            <div class="px-6 py-4 bg-gray-50 flex justify-end space-x-3">
                <button @click="$emit('cancel')"
                    class="px-4 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition-colors focus:outline-none focus:ring-2 focus:ring-gray-400">
                    {{ cancelText }}
                </button>
                <button @click="$emit('confirm')"
                    class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors focus:outline-none focus:ring-2 focus:ring-red-500">
                    {{ confirmText }}
                </button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
defineProps({
    title: {
        type: String,
        required: true
    },
    message: {
        type: String,
        required: true
    },
    confirmText: {
        type: String,
        default: 'Подтвердить'
    },
    cancelText: {
        type: String,
        default: 'Отмена'
    }
});

defineEmits(['confirm', 'cancel']);
</script>

<style scoped>
.fixed {
    animation: fadeIn 0.2s ease-in-out;
}

@keyframes fadeIn {
    from {
        opacity: 0;
    }

    to {
        opacity: 1;
    }
}
</style>
