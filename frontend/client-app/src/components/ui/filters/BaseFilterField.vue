<template>
  <div>
    <label class="block text-sm font-semibold text-gray-700 mb-2 flex items-center transition-colors duration-200">
      <!-- Иконка в зависимости от типа поля -->
      <svg v-if="icon === 'sort'" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-indigo-500" fill="none"
        viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12" />
      </svg>
      <svg v-else-if="icon === 'minPrice'" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-indigo-500"
        fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M15 12H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <svg v-else-if="icon === 'maxPrice'" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-indigo-500"
        fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M12 9v3m0 0v3m0-3h3m-3 0H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      {{ label }}
    </label>

    <div class="relative">
      <!-- Селект (выпадающий список) -->
      <div v-if="type === 'select'" class="relative">
        <select v-model="localValue"
          class="w-full appearance-none border border-gray-300 rounded-lg px-3 py-2.5 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:border-indigo-300 transition-all hover:bg-white hover:border-indigo-300"
          @change="onValueChange">
          <option v-for="opt in options" :key="opt.value === null ? 'null' : opt.value" :value="opt.value">
            {{ opt.label }}
          </option>
        </select>
        <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-500">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
          </svg>
        </div>
      </div>

      <!-- Обычное поле ввода -->
      <div v-else class="relative">
        <input :type="type === 'number' ? 'text' : type" :inputmode="type === 'number' ? 'numeric' : null"
          :pattern="type === 'number' ? '[0-9]*' : null" v-model="localValue" :placeholder="placeholder"
          class="w-full border border-gray-300 rounded-lg px-3 py-2.5 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:border-indigo-300 transition-all hover:bg-white hover:border-indigo-300"
          @blur="onValueChange" @keyup.enter="onValueChange" />
        <!-- Иконка рубля для числовых полей -->
        <div v-if="type === 'currency'"
          class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-3 text-gray-500">
          ₽
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    label: String,
    type: {
      type: String,
      default: 'text',
    },
    options: {
      type: Array,
      default: () => [],
    },
    modelValue: {
      default: null,
    },
    icon: {
      type: String,
      default: '',
    },
    placeholder: {
      type: String,
      default: '',
    }
  },
  data() {
    return {
      localValue: this.modelValue,
    };
  },
  watch: {
    modelValue: {
      handler(newVal) {
        console.log(`BaseFilterField ${this.label} received new modelValue:`, newVal);
        // Для чисел конвертируем в строку для отображения в input
        if (this.type === 'number' && newVal !== null && newVal !== undefined) {
          this.localValue = String(newVal);
        } else {
          this.localValue = newVal;
        }
      },
      immediate: true
    }
  },
  methods: {
    onValueChange() {
      // Если тип number, нам нужно преобразовать строку в число
      let value = this.localValue;
      if (this.type === 'number' && value !== null && value !== '' && value !== undefined) {
        value = Number(value);

        // Проверяем на NaN
        if (isNaN(value)) {
          value = null;
        }
      }

      console.log(`BaseFilterField ${this.label} value changed to:`, value);
      this.$emit('update:modelValue', value);
    },
  },
};
</script>

<style scoped>
/* Кастомные стили для фокуса */
.focus-visible:focus {
  outline: 2px solid #4f46e5;
  outline-offset: 2px;
}
</style>
