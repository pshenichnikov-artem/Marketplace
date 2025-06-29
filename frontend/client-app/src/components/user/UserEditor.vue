<template>
    <div class="max-w-5xl mx-auto">
        <!-- Используем общий компонент EntityEditor в качестве оболочки -->
        <EntityEditor :title="isEditMode ? $t('entityEditor.user.title.edit') : $t('entityEditor.user.title.create')"
            :is-saving="isSaving" :back-link="'/dashboard/users'" @back="handleBack" @save="saveUser">
            <!-- Дополнительные действия в шапке -->
            <template v-if="isEditMode" #actions>
                <button @click="confirmDelete"
                    class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    {{ $t('entityEditor.user.deleteUser') }}
                </button>
            </template>

            <!-- Основное содержимое формы редактирования пользователя -->
            <div class="space-y-6">
                <!-- Индикатор загрузки при инициализации редактора -->
                <div v-if="isLoading" class="flex justify-center items-center py-8">
                    <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
                </div>

                <!-- Форма пользователя -->
                <div v-else class="flex flex-col md:flex-row gap-6">
                    <!-- Левая колонка -->
                    <div class="md:w-7/12 space-y-6">
                        <!-- 1. Секция основной информации -->
                        <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.user.basicInfo') }}</h3>
                            </div>
                            <div class="p-5">
                                <div class="space-y-4">
                                    <!-- Имя -->
                                    <ValidationInput id="firstName" :label="$t('entityEditor.user.firstName')"
                                        type="text" v-model="userData.firstName" validationRules="required"
                                        :error-messages="{
                                            required: $t('entityEditor.user.validation.firstNameRequired')
                                        }" :placeholder="$t('entityEditor.user.firstNamePlaceholder')"
                                        ref="firstNameInput"
                                        @validateInput="updateValidationState('firstName', $event)" />

                                    <!-- Фамилия -->
                                    <ValidationInput id="lastName" :label="$t('entityEditor.user.lastName')" type="text"
                                        v-model="userData.lastName" validationRules="required" :error-messages="{
                                            required: $t('entityEditor.user.validation.lastNameRequired')
                                        }" :placeholder="$t('entityEditor.user.lastNamePlaceholder')"
                                        ref="lastNameInput"
                                        @validateInput="updateValidationState('lastName', $event)" />

                                    <!-- Email -->
                                    <ValidationInput id="email" :label="$t('entityEditor.user.email')" type="email"
                                        v-model="userData.email" validationRules="required|email" :error-messages="{
                                            required: $t('entityEditor.user.validation.emailRequired'),
                                            email: $t('entityEditor.user.validation.emailInvalid')
                                        }" :placeholder="$t('entityEditor.user.emailPlaceholder')" ref="emailInput"
                                        @validateInput="updateValidationState('email', $event)" />

                                    <!-- Телефон -->
                                    <ValidationInput id="phone" :label="$t('entityEditor.user.phone')" type="tel"
                                        v-model="userData.phone" validationRules="required|phone" :error-messages="{
                                        }" :placeholder="$t('entityEditor.user.phonePlaceholder')" ref="phoneInput"
                                        @validateInput="updateValidationState('phone', $event)" />

                                    <!-- Роль -->
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-1">
                                            {{ $t('entityEditor.user.role') }} <span class="text-red-500">*</span>
                                        </label>
                                        <div class="relative">
                                            <select v-model="userData.role"
                                                class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                                                @change="validateField('role')">
                                                <option v-for="role in roles" :key="role.value" :value="role.value">
                                                    {{ role.label }}
                                                </option>
                                            </select>
                                            <p v-if="errors.role" class="mt-1 text-sm text-red-500">{{ errors.role }}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- 2. Секция адреса -->
                        <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.user.address') }}</h3>
                            </div>
                            <div class="p-5">
                                <!-- Адрес -->
                                <ValidationInput id="address" :label="$t('entityEditor.user.addressLine')"
                                    type="textarea" v-model="userData.address"
                                    :placeholder="$t('entityEditor.user.addressPlaceholder')" rows="3"
                                    ref="addressInput" />
                            </div>
                        </div>
                    </div>

                    <!-- Правая колонка -->
                    <div class="md:w-5/12">
                        <!-- Секция пароля (только для создания нового пользователя) -->
                        <div v-if="!isEditMode"
                            class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.user.credentials') }}</h3>
                            </div>
                            <div class="p-5">
                                <div class="space-y-4">
                                    <!-- Пароль -->
                                    <ValidationInput id="password" :label="$t('entityEditor.user.password')"
                                        type="password" v-model="userData.password" validationRules="required|min:8"
                                        :error-messages="{
                                            required: $t('entityEditor.user.validation.passwordRequired'),
                                            min: $t('entityEditor.user.validation.passwordMinLength')
                                        }" ref="passwordInput"
                                        @validateInput="updateValidationState('password', $event)" />

                                    <!-- Подтверждение пароля -->
                                    <ValidationInput id="confirmPassword"
                                        :label="$t('entityEditor.user.confirmPassword')" type="password"
                                        v-model="userData.confirmPassword" validationRules="required|confirmed:password"
                                        :error-messages="{
                                            required: $t('entityEditor.user.validation.confirmPasswordRequired'),
                                            confirmed: $t('entityEditor.user.validation.passwordsNotMatch')
                                        }" ref="confirmPasswordInput"
                                        @validateInput="updateValidationState('confirmPassword', $event)" />
                                </div>
                            </div>
                        </div>

                        <!-- Секция смены пароля (только для редактирования пользователя) -->
                        <div v-if="isEditMode"
                            class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.user.changePassword') }}
                                </h3>
                            </div>
                            <div class="p-5">
                                <div class="space-y-4">
                                    <!-- Новый пароль -->
                                    <ValidationInput id="password" :label="$t('entityEditor.user.newPassword')"
                                        type="password" v-model="userData.password" validationRules="min:8"
                                        :error-messages="{
                                            min: $t('entityEditor.user.validation.passwordMinLength')
                                        }" :placeholder="$t('entityEditor.user.newPasswordPlaceholder')"
                                        ref="passwordInput"
                                        @validateInput="updateValidationState('password', $event)" />

                                    <!-- Подтверждение нового пароля -->
                                    <ValidationInput id="confirmPassword"
                                        :label="$t('entityEditor.user.confirmNewPassword')" type="password"
                                        v-model="userData.confirmPassword" validationRules="confirmed:password"
                                        :error-messages="{
                                            confirmed: $t('entityEditor.user.validation.passwordsNotMatch')
                                        }" :placeholder="$t('entityEditor.user.confirmPasswordPlaceholder')"
                                        ref="confirmPasswordInput"
                                        @validateInput="updateValidationState('confirmPassword', $event)" />

                                    <p class="text-sm text-gray-500">
                                        {{ $t('entityEditor.user.passwordHint') }}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EntityEditor>

        <!-- Модальное окно подтверждения удаления -->
        <ConfirmModal v-if="showDeleteModal" :title="$t('entityEditor.user.deleteConfirm')"
            :message="$t('entityEditor.user.deleteConfirmText')"
            :confirm-text="$t('entityEditor.user.deleteConfirmButton')"
            :cancel-text="$t('entityEditor.user.deleteCancel')" @confirm="deleteUser" @cancel="cancelDelete" />

        <!-- Уведомления -->
        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import ValidationInput from '@/components/ui/ValidationInput.vue';
import userService from '@/services/api/userService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'UserEditor',
    components: {
        EntityEditor,
        ConfirmModal,
        Notification,
        ValidationInput
    },
    props: {
        id: {
            type: [String, Number],
            default: null
        }
    },
    emits: ['save', 'back', 'cancel'],
    setup(props) {
        const { t } = useI18n();
        const router = useRouter();
        const route = useRoute();

        const toast = ref(null);

        // Рефы для ValidationInput
        const firstNameInput = ref(null);
        const lastNameInput = ref(null);
        const emailInput = ref(null);
        const phoneInput = ref(null);
        const addressInput = ref(null);
        const passwordInput = ref(null);
        const confirmPasswordInput = ref(null);

        // Состояния
        const isLoading = ref(false);
        const isSaving = ref(false);
        const showDeleteModal = ref(false);

        // Данные пользователя
        const userData = reactive({
            id: null,
            firstName: '',
            lastName: '',
            email: '',
            phone: '',
            address: '',
            role: 'user',
            password: '',
            confirmPassword: ''
        });

        // Режим (создание или редактирование)
        const isEditMode = computed(() => !!props.id);

        // Список ролей
        const roles = [
            { label: t('dashboard.users.roles.user'), value: 'user' },
            { label: t('dashboard.users.roles.seller'), value: 'seller' },
            { label: t('dashboard.users.roles.admin'), value: 'admin' }
        ];

        // Валидация
        const validationState = reactive({
            firstName: false,
            lastName: false,
            email: false,
            phone: false,
            role: false,
            password: false,
            confirmPassword: false
        });

        const errors = reactive({
            firstName: '',
            lastName: '',
            email: '',
            role: '',
            password: '',
            confirmPassword: ''
        });

        // Метод для обновления состояния валидации
        const updateValidationState = (field, isValid) => {
            validationState[field] = isValid;
        };

        // API хуки
        const { loading: loadingUser, execute: executeLoadUser } = useApiRequest();
        const { loading: savingUser, execute: executeSaveUser } = useApiRequest();
        const { loading: deletingUser, execute: executeDeleteUser } = useApiRequest();

        // Отслеживание состояния загрузки
        const watchLoading = () => {
            isLoading.value = loadingUser.value;
            isSaving.value = savingUser.value;
        };

        // Загрузка данных пользователя при редактировании
        const loadUser = async () => {
            if (!props.id) return;

            await executeLoadUser(async () => {
                return await userService.getUserById(props.id);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        const user = data;
                        // Заполняем поля формы данными загруженного пользователя
                        userData.id = user.id;
                        userData.firstName = user.firstName;
                        userData.lastName = user.lastName;
                        userData.email = user.email;
                        userData.phone = user.phone || '';
                        userData.address = user.address || '';
                        userData.role = user.role || 'user';

                        // Запускаем валидацию полей после загрузки данных
                        validateAllFields();
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            watchLoading();
        };

        // Валидация полей формы
        const validateField = (field) => {
            switch (field) {
                case 'firstName':
                    errors.firstName = !userData.firstName
                        ? t('entityEditor.user.validation.firstNameRequired')
                        : '';
                    break;

                case 'lastName':
                    errors.lastName = !userData.lastName
                        ? t('entityEditor.user.validation.lastNameRequired')
                        : '';
                    break;

                case 'email':
                    if (!userData.email) {
                        errors.email = t('entityEditor.user.validation.emailRequired');
                    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(userData.email)) {
                        errors.email = t('entityEditor.user.validation.emailInvalid');
                    } else {
                        errors.email = '';
                    }
                    break;

                case 'role':
                    errors.role = !userData.role
                        ? t('entityEditor.user.validation.roleRequired')
                        : '';
                    break;

                case 'password':
                    if (!isEditMode.value && !userData.password) {
                        errors.password = t('entityEditor.user.validation.passwordRequired');
                    } else if (userData.password && userData.password.length < 8) {
                        errors.password = t('entityEditor.user.validation.passwordMinLength');
                    } else {
                        errors.password = '';
                    }
                    break;

                case 'confirmPassword':
                    if (!isEditMode.value && !userData.confirmPassword) {
                        errors.confirmPassword = t('entityEditor.user.validation.confirmPasswordRequired');
                    } else if (userData.password !== userData.confirmPassword) {
                        errors.confirmPassword = t('entityEditor.user.validation.passwordsNotMatch');
                    } else {
                        errors.confirmPassword = '';
                    }
                    break;
            }

            // Обновляем состояние валидации
            validationState[field] = !errors[field];
        };

        // Валидация всех полей
        const validateAllFields = () => {
            firstNameInput.value?.validate();
            lastNameInput.value?.validate();
            emailInput.value?.validate();
            validateField('role');

            if (!isEditMode.value) {
                passwordInput.value?.validate();
                confirmPasswordInput.value?.validate();
            } else if (userData.password) {
                passwordInput.value?.validate();
                confirmPasswordInput.value?.validate();
            }
        };

        const validateForm = () => {
            validateAllFields();

            // Собираем все ошибки валидации
            const errorMessages = [];

            if (!validationState.firstName) {
                errorMessages.push(t('entityEditor.user.validation.firstNameRequired'));
            }

            if (!validationState.lastName) {
                errorMessages.push(t('entityEditor.user.validation.lastNameRequired'));
            }

            if (!validationState.email) {
                if (!userData.email) {
                    errorMessages.push(t('entityEditor.user.validation.emailRequired'));
                } else {
                    errorMessages.push(t('entityEditor.user.validation.emailInvalid'));
                }
            }

            if (!validationState.role) {
                errorMessages.push(t('entityEditor.user.validation.roleRequired'));
            }

            if (!validationState.phone) {
                errorMessages.push(t('entityEditor.user.validation.phoneRequired'));
            }

            if (!isEditMode.value) {
                if (!validationState.password) {
                    if (!userData.password) {
                        errorMessages.push(t('entityEditor.user.validation.passwordRequired'));
                    } else {
                        errorMessages.push(t('entityEditor.user.validation.passwordMinLength'));
                    }
                }

                if (!validationState.confirmPassword) {
                    if (!userData.confirmPassword) {
                        errorMessages.push(t('entityEditor.user.validation.confirmPasswordRequired'));
                    } else {
                        errorMessages.push(t('entityEditor.user.validation.passwordsNotMatch'));
                    }
                }
            } else if (userData.password) {
                // Если при редактировании указан пароль, проверяем его требования
                if (!validationState.password) {
                    errorMessages.push(t('entityEditor.user.validation.passwordMinLength'));
                }

                if (!validationState.confirmPassword) {
                    errorMessages.push(t('entityEditor.user.validation.passwordsNotMatch'));
                }
            }

            // Если есть ошибки, отображаем их через Notification
            if (errorMessages.length > 0) {
                errorMessages.forEach(message => {
                    toast.value.error(message);
                });
                return false;
            }

            return true;
        };

        // Методы для управления пользователем
        const saveUser = async () => {
            // Валидируем форму
            if (!validateForm()) {
                return;
            }

            // Подготавливаем данные для отправки
            const userToSave = {
                firstName: userData.firstName,
                lastName: userData.lastName,
                email: userData.email,
                phone: userData.phone,
                address: userData.address,
                role: userData.role
            };

            // Добавляем пароль, если он указан
            if (userData.password) {
                userToSave.password = userData.password;
            }

            await executeSaveUser(async () => {
                if (isEditMode.value) {
                    return await userService.updateUser(props.id, userToSave);
                } else {
                    return await userService.createUser(userToSave);
                }
            }, {
                onSuccess: () => {
                    if (isEditMode.value) {
                        toast.value.success(t('entityEditor.user.userSaved'));
                    } else {
                        toast.value.success(t('entityEditor.user.userCreated'));
                    }
                    router.push('/dashboard/users');
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            watchLoading();
        };

        const confirmDelete = () => {
            showDeleteModal.value = true;
        };

        const cancelDelete = () => {
            showDeleteModal.value = false;
        };

        const deleteUser = async () => {
            await executeDeleteUser(async () => {
                return await userService.deleteUser(props.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('entityEditor.user.userDeleted'));
                    router.push('/dashboard/users');
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            watchLoading();
        };

        const handleBack = () => {
            router.push('/dashboard/users');
        };

        // Инициализация
        onMounted(async () => {
            // Если это режим редактирования, загружаем данные пользователя
            if (isEditMode.value) {
                await loadUser();
            }
        });

        return {
            userData,
            roles,
            isEditMode,
            isLoading,
            isSaving,
            showDeleteModal,
            validationState,
            errors,
            toast,
            firstNameInput,
            lastNameInput,
            emailInput,
            phoneInput,
            addressInput,
            passwordInput,
            confirmPasswordInput,
            validateField,
            validateForm,
            updateValidationState,
            saveUser,
            confirmDelete,
            cancelDelete,
            deleteUser,
            handleBack
        };
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
</style>
