<template>
    <MainNavbar />
    <div class="page-container">

        <div v-if="isLoading" class="flex justify-center items-center py-20">
            <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>

        <div v-else-if="notFound" class="py-20 text-center">
            <div class="max-w-lg mx-auto">
                <h1 class="text-6xl font-bold text-indigo-600 mb-6">404</h1>
                <h2 class="text-3xl font-semibold text-gray-800 mb-4">{{ $t('pages.notFound.title') }}</h2>
                <p class="text-lg text-gray-600 mb-8">{{ $t('pages.notFound.message') }}</p>
                <router-link to="/"
                    class="inline-flex items-center px-6 py-3 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-colors">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" viewBox="0 0 20 20"
                        fill="currentColor">
                        <path fill-rule="evenodd"
                            d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z"
                            clip-rule="evenodd" />
                    </svg>
                    {{ $t('pages.notFound.returnHome') }}
                </router-link>
            </div>
        </div>

        <!-- Содержимое страницы -->
        <div v-else class="page-content full-height" v-html="sanitizedContent"></div>
    </div>
    <AppFooter />
</template>

<script>
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import pageService from '@/services/api/pageService';
import useApiRequest from '@/hooks/useApiRequest';
import DOMPurify from 'dompurify';
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';

export default {
    name: 'PageView',
    components: {
        MainNavbar,
        AppFooter
    },
    setup() {
        const { t, locale } = useI18n();
        const route = useRoute();
        const router = useRouter();

        const pageContent = ref('');
        const notFound = ref(false);
        const { loading: isLoading, execute } = useApiRequest();

        const getPageKeyFromUrl = () => {
            let pageKey = route.path.substring(1);

            if (!pageKey) {
                pageKey = 'home';
            }

            return pageKey;
        };

        const sanitizedContent = computed(() => {
            return DOMPurify.sanitize(pageContent.value);
        });

        const loadPage = async () => {
            const pageKey = getPageKeyFromUrl();
            const currentLocale = locale.value;

            await execute(async () => {
                return await pageService.getPageByKey(pageKey, currentLocale);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        pageContent.value = data.content;
                        notFound.value = false;

                        document.title = `${pageKey.charAt(0).toUpperCase() + pageKey.slice(1)} | Your App Name`;
                    } else {
                        notFound.value = true;
                        document.title = `${t('pages.notFound.title')} | Your App Name`;
                    }
                },
                onError: () => {
                    notFound.value = true;
                    document.title = `${t('pages.notFound.title')} | Your App Name`;
                }
            });
        };

        watch(locale, () => {
            loadPage();
        });

        onMounted(() => {
            loadPage();
        });

        return {
            pageContent,
            notFound,
            isLoading,
            sanitizedContent
        };
    }
};
</script>

<style scoped>
.page-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 2rem;
    min-height: calc(100vh - 64px);
    display: flex;
    flex-direction: column;
}

.page-content {
    background-color: white;
    border-radius: 0.5rem;
    box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
    padding: 2rem;
}

.full-height {
    flex: 1;
    min-height: calc(100vh - 160px);
}

.page-content :deep(h1) {
    font-size: 2rem;
    font-weight: 600;
    color: #1a202c;
    margin-bottom: 1rem;
}

.page-content :deep(h2) {
    font-size: 1.5rem;
    font-weight: 600;
    color: #2d3748;
    margin-bottom: 0.75rem;
    margin-top: 1.5rem;
}

.page-content :deep(h3) {
    font-size: 1.25rem;
    font-weight: 600;
    color: #4a5568;
    margin-bottom: 0.75rem;
    margin-top: 1.25rem;
}

.page-content :deep(p) {
    margin-bottom: 1rem;
    line-height: 1.625;
}

.page-content :deep(ul),
.page-content :deep(ol) {
    margin-left: 2rem;
    margin-bottom: 1rem;
}

.page-content :deep(li) {
    margin-bottom: 0.5rem;
}

.page-content :deep(a) {
    color: #4f46e5;
    text-decoration: underline;
}

.page-content :deep(img) {
    max-width: 100%;
    height: auto;
    margin: 1rem 0;
    border-radius: 0.25rem;
}

.page-content :deep(blockquote) {
    border-left: 4px solid #e2e8f0;
    padding-left: 1rem;
    margin-left: 0;
    margin-right: 0;
    font-style: italic;
    color: #4a5568;
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
