<template>
    <div class="bg-white rounded-xl shadow-md border border-gray-200 overflow-hidden">
        <!-- Верхняя панель с заголовком -->
        <div class="px-6 py-4 bg-gradient-to-r from-indigo-50 to-blue-50 border-b border-gray-200">
            <div class="flex justify-between items-center">
                <div class="flex items-center">
                    <button @click="goBack"
                        class="mr-3 p-1 rounded-full hover:bg-white text-indigo-600 hover:text-indigo-800 transition-colors duration-200"
                        :aria-label="$t('entityEditor.back')">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                    </button>
                    <h2 class="text-xl font-bold text-gray-800">{{ title }}</h2>
                </div>

                <div class="flex space-x-3">
                    <slot name="actions"></slot>
                    <button @click="save"
                        class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md"
                        :disabled="isSaving">
                        <svg v-if="isSaving" class="animate-spin -ml-1 mr-2 h-5 w-5 text-white"
                            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                        <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none"
                            viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4" />
                        </svg>
                        {{ isSaving ? $t('entityEditor.saving') : $t('entityEditor.save') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Основное содержимое -->
        <div class="p-6">
            <div class="overflow-hidden">
                <slot></slot>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

export default {
    name: 'EntityEditor',
    props: {
        title: {
            type: String,
            required: true
        },
        entityId: {
            type: [String, Number],
            default: null
        },
        isSaving: {
            type: Boolean,
            default: false
        },
        showFooter: {
            type: Boolean,
            default: true
        },
        backLink: {
            type: String,
            default: null
        }
    },
    emits: ['save', 'back', 'cancel'],
    setup(props, { emit }) {
        const { t } = useI18n();
        const router = useRouter();

        const goBack = () => {
            router.push({ path: props.backLink || '/dashboard' });
            emit('back');
        };

        // Функция сохранения
        const save = () => {
            console.log('Saving...');
            emit('save');

        };

        const handlePopState = (event) => {
            if (event.state !== null) {
                emit('back');
            }
        };

        onMounted(() => {
            window.addEventListener('popstate', handlePopState);
        });

        onBeforeUnmount(() => {
            window.removeEventListener('popstate', handlePopState);
        });

        return {
            goBack,
            save,
        };
    }
};
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

:deep(table) {
    width: 100%;
    min-width: 100%;
    border-collapse: collapse;
}

:deep(.table-container) {
    overflow-x: auto;
    width: 100%;
    max-width: 100%;
    margin-bottom: 1rem;
}

:deep(th),
:deep(td) {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

:deep(th.actions-column),
:deep(td.actions-column) {
    width: 160px;
    min-width: 160px;
}


:deep(td.expandable-cell) {
    max-width: 300px;
    white-space: normal;
}
</style>
