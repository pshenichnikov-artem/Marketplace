<template>
  <footer class="bg-gray-800 text-white py-8 mt-auto">
    <div class="container mx-auto px-4">
      <!-- Загрузка -->
      <div v-if="isLoading" class="flex justify-center items-center py-4">
        <div class="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-400"></div>
      </div>

      <!-- Содержимое футера -->
      <div v-else v-html="sanitizedContent" class="footer-content"></div>


    </div>
  </footer>
</template>

<script>
import { ref, onMounted, computed, watch } from 'vue'; // Добавляем импорт watch
import { useI18n } from 'vue-i18n'; // Добавляем импорт useI18n
import DOMPurify from 'dompurify';
import pageService from '@/services/api/pageService';
import useApiRequest from '@/hooks/useApiRequest';


export default {
  name: 'AppFooter',
  components: {

  },
  setup() {
    const { locale } = useI18n(); // Получаем текущую локаль
    const footerContent = ref('');
    const { loading: isLoading, execute } = useApiRequest();

    // Очистка HTML для защиты от XSS
    const sanitizedContent = computed(() => {
      return DOMPurify.sanitize(footerContent.value);
    });

    // Загрузка содержимого футера
    const loadFooterContent = async () => {
      await execute(async () => {
        // Передаем текущую локаль вместо фиксированного 'ru'
        return await pageService.getPageByKey('footer', locale.value);
      }, {
        onSuccess: (data) => {
          if (data && data.content) {
            footerContent.value = data.content;
          } else {
            console.error('Footer content not found');
          }
        },
        onError: () => {
          console.error('Error loading footer content');

        }
      });
    };
    // Получение текущего года
    const currentYear = computed(() => {
      return new Date().getFullYear();
    });

    // Добавляем слежение за изменением локали
    watch(locale, () => {
      loadFooterContent();
    });

    onMounted(() => {
      loadFooterContent();
    });

    return {
      sanitizedContent,
      isLoading,
      currentYear
    };
  }
}
</script>

<style scoped>
/* Стили для footer */
footer {
  flex-shrink: 0;
}

/* Стили для внутреннего содержимого */
.footer-content :deep(h3) {
  font-size: 1.125rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.footer-content :deep(ul) {
  margin-bottom: 1rem;
}

.footer-content :deep(a) {
  color: #9ca3af;
  transition: color 0.2s;
}

.footer-content :deep(a:hover) {
  color: #ffffff;
}

.footer-content :deep(.space-y-2 > *) {
  margin-top: 0;
  margin-bottom: 0.5rem;
}

.footer-content :deep(.space-y-2 > *:first-child) {
  margin-top: 0;
}

/* Анимация загрузки */
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
