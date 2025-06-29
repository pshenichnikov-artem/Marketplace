<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="w-full max-w-md p-8 bg-white rounded-lg shadow-md">
      <div class="flex justify-end mb-4">
        <LanguageSwitcher />
      </div>
      <h2 class="text-2xl font-bold text-gray-800 mb-6">{{ $t('login.title') }}</h2>
      <form @submit.prevent="handleLogin">
        <ValidationInput
            id="email"
            :label="$t('login.email')"
            type="email"
            v-model="email"
            validationRules="required|email"
            ref="emailInput"
            @validateInput="updateValidationState('email', $event)"
        />

        <ValidationInput
            id="password"
            :label="$t('login.password')"
            type="password"
            v-model="password"
            validationRules="required|password"
            :error-messages="{
            required: $t('validation.password.required'),
            password: $t('validation.password.pattern')
          }"
            ref="passwordInput"
            @validateInput="updateValidationState('password', $event)"
        />

        <div v-if="errorMessage" class="mb-4 mt-2 text-red-600 text-sm p-2 bg-red-50 border border-red-200 rounded">
          {{ errorMessage }}
        </div>

        <div class="flex items-center justify-between mt-6">
          <button type="submit"
            class="w-full px-4 py-2 bg-indigo-600 text-white font-medium text-sm rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
            {{ $t('login.loginButton') }}
          </button>
        </div>
      </form>
      <div class="mt-4 text-center">
        <button @click="$router.push('/register')"
          class="w-full px-4 py-2 bg-gray-600 text-white font-medium text-sm rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500">
          {{ $t('login.registerButton') }}
        </button>
      </div>
    </div>
  </div>
  <AppFooter />
</template>

<script>
import authService from '@/services/api/authService';
import ValidationInput from '@/components/ui/ValidationInput.vue';
import LanguageSwitcher from '@/components/ui/LanguageSwitcher.vue';
import AppFooter from "@/components/layout/AppFooter.vue";

export default {
  name: 'LoginPage',
  components: {
    AppFooter,
    ValidationInput,
    LanguageSwitcher
  },
  data() {
    return {
      email: '',
      password: '',
      errorMessage: '',
      validationState: {
        email: false,
        password: false
      }
    };
  },
  methods: {
    updateValidationState(field, isValid) {
      this.validationState[field] = isValid;
    },
    
    validateForm() {
      // Валидируем все поля
      const emailValid = this.$refs.emailInput.validate();
      const passwordValid = this.$refs.passwordInput.validate();
      
      // Проверяем состояние валидации
      return Object.values(this.validationState).every(state => state === true);
    },

    async handleLogin() {
      // Очищаем предыдущие сообщения об ошибках
      this.errorMessage = '';

      // Проверяем валидацию перед отправкой
      const isValid = this.validateForm();
      if (!isValid) {
        return;
      }

      console.log(this.email)

        const response = await authService.login({email: this.email, password: this.password});
        if (response.status === 'success') {
          this.$router.push('/');
        } else if (response.error?.code === 401) {
          this.errorMessage = this.$t('serverErrors.invalidCredentials');
        } 
        else if (response.error?.code === 500){
          this.errorMessage = response.message
        }
        else {
          this.errorMessage = this.$t('login.loginError');
        }
    },
  },
};
</script>
