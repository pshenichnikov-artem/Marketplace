<template>
  <div>
    <!-- Экран загрузки -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
    </div>

    <!-- Отображение данных профиля -->
    <div v-else>
      <!-- Режим просмотра -->
      <div v-if="!editMode && !changePasswordMode" class="bg-white rounded-lg">
        <!-- Верхняя секция с основной информацией -->
        <div class="flex flex-col sm:flex-row items-start justify-between gap-6 mb-8">
          <!-- Основная информация -->
          <div class="flex-1">
            <h2 class="text-2xl font-bold text-gray-800 mb-4">{{ user.firstName }} {{ user.lastName }}</h2>
            <div class="flex items-center text-gray-600 mb-1">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-gray-500" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              {{ user.email }}
            </div>
            <div class="flex items-center text-gray-600">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-gray-500" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
              </svg>
              {{ user.phone || $t('profile.noPhone') }}
            </div>
          </div>

          <!-- Кнопки редактировать и изменить пароль -->
          <div class="flex flex-col sm:flex-row gap-3">
            <button
              class="px-6 py-2.5 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition shadow-md hover:shadow-lg flex items-center"
              @click="editMode = true">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
              </svg>
              {{ $t('profile.editButton') }}
            </button>
            <button
              class="px-6 py-2.5 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition shadow flex items-center"
              @click="changePasswordMode = true">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
              {{ $t('profile.changePasswordButton') }}
            </button>
          </div>
        </div>

        <!-- Блок с более подробной информацией -->
        <div class="p-6 bg-gray-50 rounded-lg shadow-inner mb-8">
          <h3 class="text-lg font-semibold text-gray-700 border-b border-gray-200 pb-2 mb-4">
            {{ $t('profile.personalInfo') }}
          </h3>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-y-4 gap-x-6">
            <div class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.firstName') }}</span>
              <span class="font-medium">{{ user.firstName }}</span>
            </div>

            <div class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.lastName') }}</span>
              <span class="font-medium">{{ user.lastName }}</span>
            </div>

            <div class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.email') }}</span>
              <span class="font-medium">{{ user.email }}</span>
            </div>

            <div class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.phone') }}</span>
              <span class="font-medium">{{ user.phone || $t('profile.noPhone') }}</span>
            </div>

            <div v-if="user.country" class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.country') }}</span>
              <span class="font-medium">{{ user.country }}</span>
            </div>

            <div v-if="user.city" class="flex flex-col">
              <span class="text-sm text-gray-500 mb-1">{{ $t('profile.city') }}</span>
              <span class="font-medium">{{ user.city }}</span>
            </div>
          </div>
        </div>

        <!-- Баннер-карусель после информации -->
        <div class="mt-8">
          <h3 class="text-lg font-semibold text-gray-700 mb-4">{{ $t('profile.recommendations') }}</h3>
          <BannerCarousel />
        </div>
      </div>

      <!-- Режим редактирования профиля -->
      <div v-else-if="editMode" class="bg-white rounded-lg p-6 shadow-md transition duration-300">
        <h2 class="text-2xl font-bold text-gray-800 mb-6 flex items-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7 mr-2 text-indigo-600" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
          </svg>
          {{ $t('profile.editProfile') }}
        </h2>

        <form @submit.prevent="saveProfile" class="space-y-6">
          <!-- Персональная информация -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <ValidationInput id="firstName" :label="$t('profile.firstName')" type="text" v-model="editUser.firstName"
              validationRules="required|name" ref="firstNameInput"
              @validateInput="updateValidationState('firstName', $event)" />
            <ValidationInput id="lastName" :label="$t('profile.lastName')" type="text" v-model="editUser.lastName"
              validationRules="required|name" ref="lastNameInput"
              @validateInput="updateValidationState('lastName', $event)" />
            <ValidationInput id="email" :label="$t('profile.email')" type="email" v-model="editUser.email"
              :readonly="true" validationRules="required|email" ref="emailInput"
              @validateInput="updateValidationState('email', $event)" />
            <ValidationInput id="phone" :label="$t('profile.phone')" type="tel" v-model="editUser.phone"
              validationRules="phone" ref="phoneInput" @validateInput="updateValidationState('phone', $event)" />
          </div>

          <!-- Сообщения -->
          <div class="mt-6">
            <div v-if="errorMessage" class="p-3 bg-red-100 border border-red-200 text-red-700 rounded-lg mb-4">
              <div class="flex">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                <span>{{ errorMessage }}</span>
              </div>
            </div>

            <div v-if="successMessage" class="p-3 bg-green-100 border border-green-200 text-green-700 rounded-lg mb-4">
              <div class="flex">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <span>{{ successMessage }}</span>
              </div>
            </div>
          </div>

          <!-- Кнопки действий -->
          <div class="flex flex-col sm:flex-row gap-3 justify-end mt-8">
            <button type="button"
              class="px-6 py-2.5 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition shadow"
              @click="cancelEdit" :disabled="saving">
              {{ $t('profile.cancelButton') }}
            </button>
            <button type="submit"
              class="px-6 py-2.5 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition shadow-md hover:shadow-lg flex items-center justify-center"
              :disabled="saving">
              <span v-if="saving"
                class="inline-block w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span>
              <span v-if="saving">{{ $t('profile.saving') }}</span>
              <span v-else>{{ $t('profile.saveButton') }}</span>
            </button>
          </div>
        </form>
      </div>

      <!-- Режим изменения пароля -->
      <div v-else-if="changePasswordMode" class="bg-white rounded-lg p-6 shadow-md transition duration-300">
        <h2 class="text-2xl font-bold text-gray-800 mb-6 flex items-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7 mr-2 text-indigo-600" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
          </svg>
          {{ $t('profile.changePassword') }}
        </h2>

        <form @submit.prevent="savePassword" class="space-y-6">
          <div class="grid grid-cols-1 gap-6 max-w-md">
            <ValidationInput id="currentPassword" :label="$t('profile.currentPassword')" type="password"
              v-model="currentPassword" validationRules="required" ref="currentPasswordInput"
              @validateInput="updateValidationState('currentPassword', $event)" />
            <ValidationInput id="newPassword" :label="$t('profile.newPassword')" type="password" v-model="password"
              validationRules="password" ref="passwordInput"
              @validateInput="updateValidationState('password', $event)" />
            <ValidationInput id="confirmPassword" :label="$t('profile.confirmPassword')" type="password"
              v-model="confirmPassword" validationRules="match" :compareWith="password" ref="confirmPasswordInput"
              @validateInput="updateValidationState('confirmPassword', $event)" />
          </div>

          <div class="text-sm text-gray-500 mt-2">
            {{ $t('profile.passwordHint') }}
          </div>

          <!-- Сообщения -->
          <div class="mt-6">
            <div v-if="passwordError" class="p-3 bg-red-100 border border-red-200 text-red-700 rounded-lg mb-4">
              <div class="flex">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                <span>{{ passwordError }}</span>
              </div>
            </div>

            <div v-if="passwordSuccess" class="p-3 bg-green-100 border border-green-200 text-green-700 rounded-lg mb-4">
              <div class="flex">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <span>{{ passwordSuccess }}</span>
              </div>
            </div>
          </div>

          <!-- Кнопки действий -->
          <div class="flex flex-col sm:flex-row gap-3 justify-end mt-8">
            <button type="button"
              class="px-6 py-2.5 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 transition shadow"
              @click="cancelPasswordChange" :disabled="passwordSaving">
              {{ $t('profile.cancelButton') }}
            </button>
            <button type="submit"
              class="px-6 py-2.5 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition shadow-md hover:shadow-lg flex items-center justify-center"
              :disabled="passwordSaving">
              <span v-if="passwordSaving"
                class="inline-block w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span>
              <span v-if="passwordSaving">{{ $t('profile.saving') }}</span>
              <span v-else>{{ $t('profile.saveButton') }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import ValidationInput from '@/components/ui/ValidationInput.vue';
import userService from '@/services/api/userService';
import { removeToken, removeUserRole } from '@/utils/localStorage';
import BannerCarousel from '@/components/BannerCarousel.vue';

export default {
  name: 'ProfileInfo',
  components: { ValidationInput, BannerCarousel },
  data() {
    return {
      loading: true,
      saving: false,
      passwordSaving: false,
      editMode: false,
      changePasswordMode: false,
      userData: null,
      editUser: {
        firstName: '',
        lastName: '',
        email: '',
        phone: ''
      },
      // Данные для смены пароля
      currentPassword: '',
      password: '',
      confirmPassword: '',
      // Сообщения для профиля
      errorMessage: '',
      successMessage: '',
      // Сообщения для пароля
      passwordError: '',
      passwordSuccess: '',
      // Валидация
      validationState: {
        firstName: false,
        lastName: false,
        email: false,
        phone: true,
        currentPassword: false,
        password: true,
        confirmPassword: true
      }
    };
  },
  computed: {
    user() {
      return this.userData || {};
    }
  },
  watch: {
    user: {
      handler(newVal) {
        this.editUser = { ...newVal };
      },
      immediate: true,
      deep: true
    }
  },
  methods: {
    updateValidationState(field, isValid) {
      this.validationState[field] = isValid;
    },

    async fetchUser() {
      this.loading = true;
      this.errorMessage = '';
      const response = await userService.getSelf();
      if (response.status === 'success' && response.data) {
        this.userData = response.data;
        this.editUser = { ...response.data };
      } else if (response.error?.code === 401) {
        removeToken();
        removeUserRole && removeUserRole();
        this.$router.replace('/login');
        return;
      } else {
        this.errorMessage = this.$t('profile.loadError');
      }
      this.loading = false;
    },

    async saveProfile() {
      this.errorMessage = '';
      this.successMessage = '';
      this.$refs.firstNameInput.validate();
      this.$refs.lastNameInput.validate();
      this.$refs.emailInput.validate();
      this.$refs.phoneInput.validate();

      const profileValid = this.validationState.firstName &&
        this.validationState.lastName &&
        this.validationState.email &&
        this.validationState.phone;

      if (!profileValid) {
        this.errorMessage = this.$t('validation.validationError');
        return;
      }

      this.saving = true;
      const updateRequest = {
        firstName: this.editUser.firstName,
        lastName: this.editUser.lastName,
        email: this.editUser.email,
        phone: this.editUser.phone
      };

      const response = await userService.updateUserSelf(updateRequest);

      if (response.status === 'success') {
        this.successMessage = this.$t('profile.saveSuccess');
        await this.fetchUser();
        this.editMode = false;
      } else if (response.error?.code === 409) {
        this.errorMessage = this.$t('serverErrors.emailAlreadyExists');
      } else {
        this.errorMessage = this.$t('serverErrors.unexpectedError');
      }
      this.saving = false;
    },

    async savePassword() {
      this.passwordError = '';
      this.passwordSuccess = '';

      this.$refs.currentPasswordInput.validate();
      this.$refs.passwordInput.validate();
      this.$refs.confirmPasswordInput.validate();

      const passwordValid = this.validationState.currentPassword &&
        this.validationState.password &&
        this.validationState.confirmPassword;

      if (!passwordValid) {
        this.passwordError = this.$t('validation.validationError');
        return;
      }

      this.passwordSaving = true;

      const updateRequest = {
        currentPassword: this.currentPassword,
        newPassword: this.password
      };

      const response = await userService.changePassword(updateRequest);

      if (response.status === 'success') {
        this.passwordSuccess = this.$t('profile.passwordChangeSuccess');
        setTimeout(() => {
          this.currentPassword = '';
          this.password = '';
          this.confirmPassword = '';
          this.changePasswordMode = false;
        }, 1500);
      } else if (response.error?.code === 400) {
        this.passwordError = this.$t('serverErrors.invalidCurrentPassword');
      } else {
        this.passwordError = this.$t('serverErrors.unexpectedError');
      }

      this.passwordSaving = false;
    },

    cancelEdit() {
      this.editUser = { ...this.user };
      this.editMode = false;
      this.errorMessage = '';
      this.successMessage = '';
    },

    cancelPasswordChange() {
      this.currentPassword = '';
      this.password = '';
      this.confirmPassword = '';
      this.changePasswordMode = false;
      this.passwordError = '';
      this.passwordSuccess = '';
    }
  },
  mounted() {
    this.fetchUser();
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

/* Анимация перехода между режимами редактирования */
.bg-white {
  transition: all 0.3s ease;
}

/* Эффект наведения для карточки с информацией */
.bg-gray-50 {
  transition: all 0.2s ease;
}

.bg-gray-50:hover {
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}
</style>
