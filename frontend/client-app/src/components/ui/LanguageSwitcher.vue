<template>
  <div class="flex items-center gap-4">
    <span class="text-lg font-medium cursor-pointer hover:text-indigo-700"
      :class="{ 'text-indigo-600 font-bold': currentLanguage === 'en', 'text-gray-500': currentLanguage !== 'en' }"
      @click="setLanguage('en')">
      En
    </span>

    <div class="relative w-16 h-8 bg-gray-200 rounded-full flex items-center px-1 cursor-pointer"
      @click="toggleLanguage">
      <div class="w-6 h-6 absolute bg-indigo-600 rounded-full shadow-md transition-all duration-300"
        :style="{ left: currentLanguage === 'en' ? '6px' : 'calc(100% - 6px - 24px)' }"></div>
    </div>

    <span class="text-lg font-medium cursor-pointer hover:text-indigo-700"
      :class="{ 'text-indigo-600 font-bold': currentLanguage === 'ru', 'text-gray-500': currentLanguage !== 'ru' }"
      @click="setLanguage('ru')">
      Ru
    </span>
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n';
import { computed } from 'vue';
import { setLocale } from '@/i18n/i18n';

const { locale } = useI18n();

const currentLanguage = computed(() => locale.value);

function toggleLanguage() {
  const newLocale = currentLanguage.value === 'en' ? 'ru' : 'en';
  setLanguage(newLocale);
}

function setLanguage(lang) {
  setLocale(lang);
}
</script>

<style scoped>
.text-lg {
  font-size: 1.125rem;
  line-height: 1.75rem;
}

.text-indigo-600 {
  color: #4f46e5;
}

.text-gray-500 {
  color: #6b7280;
}

.hover\:text-indigo-700:hover {
  color: #4338ca;
}

.bg-indigo-600 {
  background-color: #4f46e5;
}

.shadow-md {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.cursor-pointer {
  cursor: pointer;
}

.gap-4 {
  gap: 1rem;
}
</style>
