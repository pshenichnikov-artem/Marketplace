<template>
    <div class="space-y-6">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-2xl font-bold text-gray-800">
                {{ isEditMode ? $t('dashboard.pages.editPage') : $t('dashboard.pages.createPage') }}
            </h1>
            <div class="flex space-x-3">
                <button @click="goBack" class="btn-secondary">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                            d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z"
                            clip-rule="evenodd" />
                    </svg>
                    {{ $t('dashboard.pages.back') }}
                </button>
                <button @click="savePage" class="btn-primary" :disabled="isSaving">
                    <svg v-if="!isSaving" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                            d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                            clip-rule="evenodd" />
                    </svg>
                    <svg v-else class="animate-spin h-5 w-5 mr-2" xmlns="http://www.w3.org/2000/svg" fill="none"
                        viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                        </circle>
                        <path class="opacity-75" fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                        </path>
                    </svg>
                    {{ isSaving ? $t('dashboard.pages.saving') : $t('dashboard.pages.save') }}
                </button>
            </div>
        </div>

        <!-- Form -->
        <div class="bg-white rounded-lg shadow p-6">
            <div class="space-y-6">
                <!-- Page key field -->
                <div>
                    <label for="pageKey" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ $t('dashboard.pages.pageKey') }} <span class="text-red-500">*</span>
                    </label>
                    <input type="text" id="pageKey" v-model="page.pageKey" :disabled="isEditMode"
                        class="w-full px-4 py-2 border rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                        :class="{ 'bg-gray-100': isEditMode }" :placeholder="$t('dashboard.pages.pageKeyPlaceholder')">
                    <p class="mt-1 text-sm text-gray-500">{{ $t('dashboard.pages.pageKeyDescription') }}</p>
                    <p v-if="errors.pageKey" class="mt-1 text-sm text-red-500">{{ errors.pageKey }}</p>
                </div>

                <!-- Language selector - только для новых страниц -->
                <div v-if="!isEditMode">
                    <label for="language" class="block text-sm font-medium text-gray-700 mb-1">
                        {{ $t('dashboard.pages.language') }} <span class="text-red-500">*</span>
                    </label>
                    <select id="language" v-model="page.language"
                        class="w-full px-4 py-2 border rounded-lg focus:ring-indigo-500 focus:border-indigo-500">
                        <option value="ru">Русский</option>
                        <option value="en">English</option>
                    </select>
                </div>

                <!-- Выбор языка для редактируемой страницы (только информация) -->
                <div v-else class="flex items-center">
                    <label class="block text-sm font-medium text-gray-700 mr-2">
                        {{ $t('dashboard.pages.language') }}:
                    </label>
                    <span class="px-3 py-1 bg-indigo-100 text-indigo-800 rounded-full text-sm">
                        {{ page.language === 'en' ? 'English' : 'Русский' }}
                    </span>
                </div>

                <!-- Editor mode toggle -->
                <div class="flex items-center justify-between">
                    <label class="block text-sm font-medium text-gray-700">
                        {{ $t('dashboard.pages.content') }} <span class="text-red-500">*</span>
                    </label>
                    <div class="flex items-center space-x-2">
                        <span class="text-sm text-gray-600">{{ $t('dashboard.pages.editorMode') }}:</span>
                        <button @click="editorMode = 'wysiwyg'" class="px-3 py-1 rounded text-sm"
                            :class="editorMode === 'wysiwyg' ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'">
                            {{ $t('dashboard.pages.visualEditor') }}
                        </button>
                        <button @click="editorMode = 'html'" class="px-3 py-1 rounded text-sm"
                            :class="editorMode === 'html' ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'">
                            HTML
                        </button>
                    </div>
                </div>

                <!-- Content Editor Component -->
                <ContentEditor v-model:value="htmlContent" v-model:mode="editorMode" :toast="toast" />

                <p v-if="errors.content" class="mt-1 text-sm text-red-500">{{ errors.content }}</p>
            </div>
        </div>

        <!-- Notification component -->
        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import pageService from '@/services/api/pageService';
import useApiRequest from '@/hooks/useApiRequest';
import Notification from '@/components/ui/Notification.vue';
import ContentEditor from '@/components/editor/ContentEditor.vue';

export default {
    name: 'PageEditor',
    components: {
        Notification,
        ContentEditor
    },
    setup() {
        const { t } = useI18n();
        const router = useRouter();
        const route = useRoute();
        const toast = ref(null);

        // Состояние
        const isSaving = ref(false);
        const isLoading = ref(false);
        const editorMode = ref('wysiwyg');
        const htmlContent = ref('');

        // Данные страницы
        const page = reactive({
            pageKey: '',
            content: '',
            language: 'ru'
        });

        // Ошибки валидации
        const errors = reactive({
            pageKey: '',
            content: ''
        });

        // Определяем режим (создание или редактирование)
        const isEditMode = computed(() => {
            return route.params.pageKey !== undefined;
        });

        // API хуки
        const { loading: loadingPage, execute: executeLoadPage } = useApiRequest();
        const { loading: savingPage, execute: executeSavePage } = useApiRequest();

        // Автоматически переключаться в режим HTML, если контент содержит div
        const autoSwitchEditorMode = (content) => {
            if (content && (content.includes('<div') || content.includes('</div>'))) {
                editorMode.value = 'html';
                htmlContent.value = content;

                setTimeout(() => {
                    toast.value.info(t('dashboard.pages.wysiwyg.autoHtmlMode'));
                }, 500);
            } else {
                editorMode.value = 'wysiwyg';
                htmlContent.value = content || '';
            }
        };

        // Загрузка данных страницы
        const loadPage = async () => {
            if (!isEditMode.value) return;

            isLoading.value = true;

            await executeLoadPage(async () => {
                return await pageService.getPageByKey(route.params.pageKey, route.params.language);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        page.pageKey = data.pageKey;
                        page.language = data.language || route.params.language;
                        htmlContent.value = data.content || '';

                        // Проверяем контент на сложность и выбираем подходящий редактор
                        autoSwitchEditorMode(data.content || '');
                    }
                },
                onError: (error) => {
                    toast.value.error(t('dashboard.pages.loadError'));
                    console.error('Error loading page:', error);
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            isLoading.value = false;
        };

        // Функция для определения пустого контента
        const isContentEmpty = (content) => {
            // Если содержимое полностью пустое или null/undefined
            if (!content || content.trim() === '') {
                return true;
            }

            // Создаем временный div для анализа HTML
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = content;

            // Проверяем текстовое содержимое
            const textContent = tempDiv.textContent.trim();

            // Если есть текст или изображения, контент не пустой
            if (textContent.length > 0 || tempDiv.querySelectorAll('img').length > 0) {
                return false;
            }

            // Проверяем на наличие пустых блоков с тегами br
            // Пустой контент может содержать только br теги или пустые блоки
            const onlyEmptyElements = Array.from(tempDiv.children).every(child => {
                // Получаем текстовое содержимое дочернего элемента
                const childText = child.textContent.trim();

                // Проверяем наличие изображений
                const hasImages = child.querySelectorAll('img').length > 0;

                // Проверяем, состоит ли элемент только из br тегов
                const onlyBrTags = child.innerHTML.replace(/<br\s*\/?>/gi, '').trim() === '';

                // Элемент пуст, если у него нет текста, изображений и он содержит только br теги
                return childText === '' && !hasImages && onlyBrTags;
            });

            return onlyEmptyElements;
        };

        // Валидация формы
        const validateForm = () => {
            let isValid = true;

            // Проверка ключа страницы
            if (!page.pageKey || page.pageKey.trim() === '') {
                errors.pageKey = t('dashboard.pages.validation.keyRequired');
                isValid = false;
            } else if (!/^[a-z0-9-_]+$/.test(page.pageKey)) {
                errors.pageKey = t('dashboard.pages.validation.keyFormat');
                isValid = false;
            } else {
                errors.pageKey = '';
            }

            // Проверка содержимого зависит от активного режима редактора
            const contentToCheck = htmlContent.value;
            console.log(`Validating content in ${editorMode.value} mode:`, contentToCheck);

            if (isContentEmpty(contentToCheck)) {
                errors.content = t('dashboard.pages.validation.contentRequired');
                isValid = false;
                console.log('Content is empty');
            } else {
                errors.content = '';
                console.log('Content is valid');
            }

            return isValid;
        };

        // Сохранение страницы
        const savePage = async () => {
            if (!validateForm()) {
                toast.value.error(t('dashboard.pages.validation.formErrors'));
                return;
            }

            isSaving.value = true;

            // Используем актуальное значение из активного редактора
            const contentToSave = htmlContent.value;
            console.log(`Saving content from ${editorMode.value} editor:`, contentToSave);

            await executeSavePage(async () => {
                if (isEditMode.value) {
                    return await pageService.updatePage(page.pageKey, contentToSave, page.language);
                } else {
                    return await pageService.createPage(page.pageKey, contentToSave, page.language);
                }
            }, {
                onSuccess: () => {
                    toast.value.success(
                        isEditMode.value
                            ? t('dashboard.pages.updateSuccess')
                            : t('dashboard.pages.createSuccess')
                    );

                    // Перенаправляем на страницу списка страниц
                    setTimeout(() => {
                        goBack();
                    }, 1000);
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            isSaving.value = false;
        };

        // Возврат назад
        const goBack = () => {
            router.push('/dashboard/pages');
        };

        // Инициализация
        onMounted(async () => {
            // Если редактирование, загружаем данные
            if (isEditMode.value) {
                await loadPage();
            }
        });

        return {
            page,
            isEditMode,
            isSaving,
            isLoading,
            errors,
            toast,
            editorMode,
            htmlContent,
            savePage,
            goBack,
            isContentEmpty
        };
    }
};
</script>

<style scoped>
.btn-primary {
    @apply bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors flex items-center disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-secondary {
    @apply bg-gray-200 text-gray-800 px-4 py-2 rounded-lg hover:bg-gray-300 transition-colors flex items-center;
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

/* Улучшаем визуальное восприятие кнопок переключения режимов */
.px-3.py-1.rounded.text-sm {
    transition: all 0.2s ease-in-out;
    min-width: 80px;
    text-align: center;
    font-weight: 500;
}

/* Добавим анимацию перехода между режимами редактирования */
.border.rounded-lg.overflow-hidden {
    transition: height 0.3s ease-in-out, opacity 0.2s ease-in-out;
}
</style>
