<template>
  <div class="mb-4">
    <label v-if="label" class="block text-sm font-medium text-gray-700">{{ label }}</label>
    <div class="relative flex items-center">
      <input
          class="w-full border rounded px-3 py-2 mt-1 text-sm focus:outline-none focus:ring-2 pr-10"
          :class="{
          'border-gray-300 focus:ring-blue-500': !error && !isValid,
          'border-green-500 focus:ring-green-500': isValid,
          'border-red-500 focus:ring-red-500': error,
        }"
          :id="id"
          :type="computedInputType"
          :placeholder="placeholder"
          :value="modelValue"
          @input="handleInput"
          @blur="validate"
      />
      <!-- Глазик -->
      <button
          v-if="type === 'password'"
          type="button"
          class="absolute inset-y-0 right-2 text-gray-500 text-sm flex items-center justify-center"
          @click="togglePasswordVisibility"
      >
        <img :src="showPassword ? '/images/open_eye.png' : 'images/close_eye.png'" alt="eye icon" class="w-6 h-6" />
      </button>
    </div>
    <p v-if="error" class="text-xs text-red-500 mt-1">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();

const props = defineProps({
  id: String,
  modelValue: {
    type: String,
    default: '',
  },
  label: String,
  type: {
    type: String,
    default: 'text',
  },
  placeholder: String,
  validationRules: {
    type: [String, Array],
    default: '',
  },
  errorMessages: {
    type: Object,
    default: () => ({}),
  },
  compareWith: {
    type: String,
    default: null,
  },
});

const emit = defineEmits([
  'update:modelValue',
  'input',
  'update:validationErrors',
  'validateInput',
]);

const error = ref(null);
const isValid = ref(false);
const showPassword = ref(false);

const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value;
};

const computedInputType = computed(() => {
  if (props.type !== 'password') return props.type;
  return showPassword.value ? 'text' : 'password';
});

// Вычисляемое свойство, которое возвращает текущий статус валидации
const isValidated = computed(() => {
  return isValid.value && !error.value;
});

// Кастомные паттерны
const customPatterns = {
  email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  phone: /^\+?[0-9]{10,15}$/,

  // Имя и фамилия — одна раскладка, первая буква заглавная, остальные маленькие, длина 3–50
  name: /^(?:[A-Z][a-z]{2,49}|[А-ЯЁ][а-яё]{2,49})$/,
  lastName: /^(?:[A-Z][a-z]{2,49}|[А-ЯЁ][а-яё]{2,49})$/,

  // Пароль — минимум 8 символов, хотя бы одна заглавная и спецсимвол
  password: /^(?=.*[a-zа-яё])(?=.*[A-ZА-ЯЁ])(?=.*\d)(?=.*[!@#$%^&*]).{8,100}$/,
  onlyRussian: /^[А-ЯЁа-яё\s]+$/,
  onlyEnglish: /^[A-Za-z\s]+$/,
};

function parseRules(rules) {
  if (!rules) return [];

  if (Array.isArray(rules)) return rules;

  return rules.split('|').map((rule) => {
    const [name, param] = rule.split(':');
    return { name, param };
  });
}

function getValidators() {
  return {
    required: v => !!v || props.errorMessages.required || t('validation.required'),
    minLength: (v, p) => (v.length >= +p) || props.errorMessages.minLength || t('validation.minLength', { length: p }),
    maxLength: (v, p) => (v.length <= +p) || props.errorMessages.maxLength || t('validation.maxLength', { length: p }),
    pattern: (v, p) => new RegExp(p).test(v) || props.errorMessages.pattern || t('validation.pattern'),
    match: v => {
      if (props.compareWith == null) return props.errorMessages.match || 'No reference value for comparison';
      if (v !== props.compareWith) return props.errorMessages.match || t('validation.password.mismatch');
      return true;
    },
    name: v => {
      if (v.length < 3) return props.errorMessages.nameMinLength || t('validation.name.minLength');
      if (v.length > 50) return props.errorMessages.nameMaxLength || t('validation.name.maxLength');
      if (!/^[A-ZА-ЯЁ]/.test(v)) return props.errorMessages.nameUppercase || t('validation.name.uppercase');
      if (!customPatterns.name.test(v)) return props.errorMessages.namePattern || t('validation.name.pattern');
      return true;
    },
    lastName: v => {
      if (v.length < 3) return props.errorMessages.lastNameMinLength || t('validation.minLength', { length: 3 });
      if (v.length > 50) return props.errorMessages.lastNameMaxLength || t('validation.name.maxLength');
      if (!/^[A-ZА-ЯЁ]/.test(v)) return props.errorMessages.lastNameUppercase || t('validation.lastName.uppercase');
      if (!customPatterns.lastName.test(v)) return props.errorMessages.lastNamePattern || t('validation.lastName.pattern');
      return true;
    },
    email: v => customPatterns.email.test(v) || props.errorMessages.email || t('validation.email.invalid'),
    phone: v => customPatterns.phone.test(v) || props.errorMessages.phone || t('validation.phone.pattern'),
    password: v => {
      if (v.length < 8) return props.errorMessages.passwordMinLength || t('validation.password.minLength', { length: 8 });
      if (!/[A-ZА-ЯЁ]/.test(v)) return props.errorMessages.passwordUppercase || t('validation.password.uppercase');
      if (!/[!@#$%^&*]/.test(v)) return props.errorMessages.passwordSpecial || t('validation.password.special');
      if (!/\d/.test(v)) return props.errorMessages.passwordDigit || t('validation.password.digit');
      if (!customPatterns.password.test(v)) return props.errorMessages.passwordPattern || t('validation.password.pattern');
      return true;
    },
    onlyRussian: (v) => customPatterns.onlyRussian.test(v) || t('validation.onlyRussian'),
    onlyEnglish: (v) => customPatterns.onlyEnglish.test(v) || t('validation.onlyEnglish'),
  };
}

function validate() {
  error.value = null;
  isValid.value = false;
  
  // Если значение пустое и нет правила required, считаем валидным
  if (!props.modelValue) {
    const rules = parseRules(props.validationRules);
    if (!rules.some(r => r.name === 'required')) {
      isValid.value = true;
      emit('update:validationErrors', null);
      emit('validateInput', true);
      return true;
    }
  }
  
  const rules = parseRules(props.validationRules);
  const validators = getValidators();

  for (const { name, param } of rules) {
    const validator = validators[name];
    if (validator) {
      const result = validator(props.modelValue, param);
      if (result !== true) {
        // Используем переданные сообщения об ошибках или берем из локализации
        error.value = result;
        emit('update:validationErrors', { [props.id]: error.value });
        emit('validateInput', false);
        return false;
      }
    }
  }

  isValid.value = true;
  emit('update:validationErrors', null);
  emit('validateInput', true);
  return true;
}

function handleInput(e) {
  const value = e.target.value;
  emit('update:modelValue', value);
  emit('input', value);
  validate();
}

// Экспортируем метод validate для использования извне
defineExpose({
  validate,
  isValidated
});

onMounted(() => {
  if (props.modelValue) validate();
});

watch(() => props.modelValue, (newVal) => {
  validate();
});
</script>
