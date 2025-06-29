<template>
  <div class="min-h-screen flex flex-col">
    <div class="flex-grow flex items-center justify-center bg-gray-100">
      <div class="w-full max-w-md p-8 bg-white rounded-lg shadow-md">
        <div class="flex justify-end mb-4">
          <LanguageSwitcher />
        </div>
        <h2 class="text-2xl font-bold text-gray-800 mb-6">{{ $t('register.title') }}</h2>
        <form @submit.prevent="handleRegister">
          <ValidationInput 
            id="firstName" 
            :label="$t('register.firstName')" 
            type="text" 
            v-model="firstName"
            validationRules="required|name" 
            :error-messages="{
              required: $t('validation.name.required'),
              name: $t('validation.name.pattern')
            }" 
            ref="firstNameInput" 
            @validateInput="updateValidationState('firstName', $event)"
          />

        <ValidationInput 
          id="lastName" 
          :label="$t('register.lastName')" 
          type="text" 
          v-model="lastName"
          validationRules="required|name" 
          :error-messages="{
            required: $t('validation.lastName.required'),
            name: $t('validation.lastName.pattern')
          }" 
          ref="lastNameInput" 
          @validateInput="updateValidationState('lastName', $event)"
        />

        <ValidationInput 
          id="email" 
          :label="$t('register.email')" 
          type="email" 
          v-model="email"
          validationRules="required|email"
          ref="emailInput" 
          @validateInput="updateValidationState('email', $event)"
        />

        <ValidationInput 
          id="phone" 
          :label="$t('register.phone')" 
          type="tel" 
          v-model="phone"
          validationRules="required|phone" 
          :error-messages="{
            required: $t('validation.phone.required'),
            phone: $t('validation.phone.pattern')
          }" 
          ref="phoneInput" 
          @validateInput="updateValidationState('phone', $event)"
        />

        <ValidationInput 
          id="password" 
          :label="$t('register.password')" 
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

        <ValidationInput 
          id="confirmPassword" 
          :label="$t('register.confirmPassword')" 
          type="password" 
          v-model="confirmPassword"
          validationRules="required|match" 
          :error-messages="{
            required: $t('validation.password.confirmRequired'),
            match: $t('validation.password.mismatch'),
          }"
          :compareWith="password"
          ref="confirmPasswordInput" 
          @validateInput="updateValidationState('confirmPassword', $event)"
        />
        
        <div class="mb-4">
          <label class="block text-sm font-medium text-gray-700">{{ $t('register.selectRole') }}</label>
          <select 
            v-model="role" 
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          >
            <option value="user">{{ $t('register.roleUser') }}</option>
            <option value="seller">{{ $t('register.roleSeller') }}</option>
          </select>
        </div>

        <div v-if="errorMessage" class="mb-4 mt-2 text-red-600 text-sm p-2 bg-red-50 border border-red-200 rounded">
          {{ errorMessage }}
        </div>

        <div class="flex items-center justify-between mt-6">
          <button 
            type="submit"
            class="w-full px-4 py-2 bg-indigo-600 text-white font-medium text-sm rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            {{ $t('register.registerButton') }}
          </button>
        </div>
      </form>
      <div class="mt-4 text-center">
        <button 
          @click="$router.push('/login')"
          class="w-full px-4 py-2 bg-gray-600 text-white font-medium text-sm rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
        >
          {{ $t('register.loginButton') }}
        </button>
      </div>
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
  name: 'RegisterPage',
  components: {
    AppFooter,
    ValidationInput,
    LanguageSwitcher
  },
  data() {
    return {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      password: '',
      confirmPassword: '',
      role: 'user',
      errorMessage: '',
      passwordMismatch: false,
      validationState: {
        firstName: false,
        lastName: false,
        email: false,
        phone: false, // Необязательное поле, поэтому по умолчанию true
        password: false,
        confirmPassword: false
      }
    };
  },
  methods: {
    updateValidationState(field, isValid) {
      this.validationState[field] = isValid;
    },
    
    validateForm() {
      // Сбрасываем флаг несовпадения паролей
      this.passwordMismatch = false;

      // Валидируем все поля
      this.$refs.firstNameInput.validate();
      this.$refs.lastNameInput.validate();
      this.$refs.emailInput.validate();
      this.$refs.phoneInput.validate();
      this.$refs.passwordInput.validate();
      this.$refs.confirmPasswordInput.validate();
      
      // Проверяем состояние валидации всех полей
      return Object.values(this.validationState).every(state => state === true) && !this.passwordMismatch;
    },

    async handleRegister() {
      // Очищаем предыдущие сообщения об ошибках
      this.errorMessage = '';

      // Проверяем валидацию перед отправкой
      const isValid = this.validateForm();
      if (!isValid) {
        return;
      }

      const userData = {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        phone: this.phone,
        password: this.password,
        role: this.role
      };

        const response = await authService.register(userData);
        if (response.status === 'success') {
          this.$router.push('/');
        } else if (response.error?.code === 409) {
          this.errorMessage = this.$t('serverErrors.emailAlreadyExists');
        }
        else if (response.error?.code === 500){
          this.errorMessage = response.message
        }
        else {
          this.errorMessage = this.$t('register.registerError');
        }
    },
  }
};
</script>
