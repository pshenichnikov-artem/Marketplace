<template>
    <div class="space-y-6">
        <!-- Проверяем, не находимся ли мы на странице редактирования или создания страницы -->
        <router-view v-if="isPageEditorRoute"></router-view>

        <!-- Если не находимся на странице редактирования, показываем список страниц -->
        <template v-else>
            <!-- Header with actions -->
            <div class="flex justify-between items-center">
                <h1 class="text-2xl font-bold text-gray-800">{{ $t('dashboard.pages.title') }}</h1>
                <button @click="createNewPage" class="btn-primary">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                            d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
                            clip-rule="evenodd" />
                    </svg>
                    {{ $t('dashboard.pages.createPage') }}
                </button>
            </div>

            <!-- Loading state -->
            <div v-if="isLoading" class="flex justify-center py-10">
                <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
            </div>

            <!-- Error state -->
            <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 text-center text-red-700">
                {{ error }}
                <button @click="loadPages" class="underline ml-2">{{ $t('dashboard.pages.tryAgain') }}</button>
            </div>

            <!-- Empty state -->
            <div v-else-if="pages.length === 0"
                class="text-center py-10 bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 mx-auto text-gray-400 mb-3" fill="none"
                    viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <p class="text-gray-600 font-medium">{{ $t('dashboard.pages.noPages') }}</p>
                <p class="text-gray-500 text-sm mt-1">{{ $t('dashboard.pages.createPageTip') }}</p>
                <button @click="createNewPage"
                    class="mt-4 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                    {{ $t('dashboard.pages.createFirstPage') }}
                </button>
            </div>

            <!-- Pages list -->
            <div v-else class="bg-white shadow-md rounded-lg overflow-hidden">
                <div class="overflow-x-auto">
                    <table class="min-w-full divide-y divide-gray-200">
                        <thead class="bg-gray-50">
                            <tr>
                                <th scope="col"
                                    class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    {{ $t('dashboard.pages.pageKey') }}
                                </th>
                                <th scope="col"
                                    class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    {{ $t('dashboard.pages.language') }}
                                </th>
                                <th scope="col"
                                    class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider w-32">
                                    {{ $t('dashboard.pages.actions') }}
                                </th>
                            </tr>
                        </thead>
                        <tbody class="bg-white divide-y divide-gray-200">
                            <tr v-for="page in pages" :key="page.pageKey" class="hover:bg-gray-50">
                                <td class="px-6 py-4 whitespace-nowrap">
                                    <div class="font-medium text-gray-900">{{ page.pageKey }}</div>
                                </td>
                                <td class="px-6 py-4 whitespace-nowrap">
                                    <span class="px-2.5 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                                        :class="getLanguageBadgeClass(page.language)">
                                        {{ getLanguageDisplay(page.language) }}
                                    </span>
                                </td>
                                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                    <button @click="editPage(page)" class="text-indigo-600 hover:text-indigo-900 mr-3">
                                        {{ $t('dashboard.pages.edit') }}
                                    </button>
                                    <button @click="confirmDelete(page)" class="text-red-600 hover:text-red-900">
                                        {{ $t('dashboard.pages.delete') }}
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Delete confirmation modal -->
            <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.pages.deletePageTitle')"
                :message="$t('dashboard.pages.deletePageMessage', { pageKey: pageToDelete?.pageKey || '' })"
                :confirm-text="$t('dashboard.pages.confirmDelete')" :cancel-text="$t('dashboard.pages.cancel')"
                @confirm="deletePage" @cancel="cancelDelete" />

            <Notification ref="toast" />
        </template>
    </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import pageService from '@/services/api/pageService';
import useApiRequest from '@/hooks/useApiRequest';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';

export default {
    name: 'DashboardPages',
    components: {
        ConfirmModal,
        Notification
    },
    setup() {
        const { t } = useI18n();
        const router = useRouter();
        const route = useRoute();
        const toast = ref(null);

        // State
        const pages = ref([]);
        const isLoading = ref(false);
        const error = ref(null);

        // Modal state
        const showDeleteModal = ref(false);
        const pageToDelete = ref(null);

        // Проверка находимся ли мы на странице редактора
        const isPageEditorRoute = computed(() => {
            return route.name === 'page-create' || route.name === 'page-edit';
        });

        const { loading, execute } = useApiRequest();

        // Load pages from API
        const loadPages = async () => {
            isLoading.value = true;
            error.value = null;

            try {
                const response = await pageService.getPages();
                if (response.status == "success") {
                    pages.value = response.data || [];
                } else {
                    error.value = response.message || t('dashboard.pages.loadError');
                }
            } catch (err) {
                error.value = t('dashboard.pages.loadError');
                console.error('Error loading pages:', err);
            } finally {
                isLoading.value = false;
            }
        };

        // Create new page
        const createNewPage = () => {
            router.push({ name: 'page-create' });
        };

        // Edit existing page
        const editPage = (page) => {
            router.push({
                name: 'page-edit',
                params: {
                    pageKey: page.pageKey,
                    language: page.language || 'ru'  // Передаем язык как параметр
                }
            });
        };

        // Confirm page deletion
        const confirmDelete = (page) => {
            pageToDelete.value = page;
            showDeleteModal.value = true;
        };

        // Cancel page deletion
        const cancelDelete = () => {
            showDeleteModal.value = false;
            pageToDelete.value = null;
        };

        // Delete page
        const deletePage = async () => {
            if (!pageToDelete.value) return;

            await execute(async () => {
                return await pageService.deletePage(pageToDelete.value.pageKey);
            }, {
                onSuccess: () => {
                    toast.value.success(t('dashboard.pages.deleteSuccess'));
                    loadPages(); // Reload pages
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            pageToDelete.value = null;
        };

        // Наблюдение за изменениями маршрута для обновления списка при возврате
        watch(() => route.name, (newName, oldName) => {
            if ((oldName === 'page-edit' || oldName === 'page-create') && newName === 'dashboard-pages') {
                loadPages();
            }
        });

        onMounted(() => {
            if (!isPageEditorRoute.value) {
                loadPages();
            }
        });

        // Функция для отображения языка в удобном для чтения формате
        const getLanguageDisplay = (langCode) => {
            switch (langCode) {
                case 'ru': return 'Русский';
                case 'en': return 'English';
                default: return langCode;
            }
        };

        // Функция для определения класса бейджа языка
        const getLanguageBadgeClass = (langCode) => {
            switch (langCode) {
                case 'ru': return 'bg-blue-100 text-blue-800';
                case 'en': return 'bg-green-100 text-green-800';
                default: return 'bg-gray-100 text-gray-800';
            }
        };

        return {
            pages,
            isLoading,
            error,
            showDeleteModal,
            pageToDelete,
            toast,
            loadPages,
            createNewPage,
            editPage,
            confirmDelete,
            cancelDelete,
            deletePage,
            isPageEditorRoute,
            getLanguageDisplay,
            getLanguageBadgeClass
        };
    }
};
</script>

<style scoped>
.btn-primary {
    @apply bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors flex items-center;
}

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
