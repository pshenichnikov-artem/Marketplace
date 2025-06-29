<template>
    <div class="max-w-5xl mx-auto">
        <EntityEditor :title="$t('entityEditor.banner.title')" :is-saving="isSaving" :back-link="'/dashboard'"
            @save="saveBanners">

            <div class="space-y-6">
                <!-- Индикатор загрузки при инициализации редактора -->
                <div v-if="isLoading" class="flex justify-center items-center py-8">
                    <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
                </div>

                <div v-else class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                    <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
                        <div class="flex items-center">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                viewBox="0 0 20 20" fill="currentColor">
                                <path fill-rule="evenodd"
                                    d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z"
                                    clip-rule="evenodd" />
                            </svg>
                            <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.banner.banners') }}</h3>
                        </div>
                        <button @click="addNewBanner"
                            class="bg-indigo-600 text-white px-3 py-1.5 rounded-lg hover:bg-indigo-700 transition-colors duration-200 flex items-center text-sm"
                            :disabled="isSubmitting">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M12 4v16m8-8H4" />
                            </svg>
                            {{ $t('entityEditor.banner.addBanner') }}
                        </button>
                    </div>

                    <div class="p-5 space-y-6">
                        <div v-if="banners.length === 0"
                            class="text-center py-10 bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 mx-auto text-gray-400 mb-3"
                                fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                            </svg>
                            <p class="text-gray-600 font-medium">{{ $t('entityEditor.banner.noBanners') }}</p>
                            <p class="text-gray-500 text-sm mt-1">{{ $t('entityEditor.banner.addBannerTip') }}</p>
                        </div>

                        <div v-for="(banner, index) in banners" :key="index"
                            class="border border-gray-200 rounded-lg overflow-hidden bg-white shadow-sm">
                            <div
                                class="px-4 py-3 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
                                <h4 class="font-medium text-gray-700">{{ $t('entityEditor.banner.bannerNum', {
                                    num:
                                        index + 1
                                }) }}</h4>
                                <button @click="removeBanner(index)"
                                    class="text-red-500 hover:text-red-700 transition-colors duration-200">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none"
                                        viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                    </svg>
                                </button>
                            </div>

                            <div class="p-4 space-y-4">
                                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <ValidationInput :id="`title-${index}`" :label="$t('entityEditor.banner.title')"
                                        v-model="banner.title" validationRules="required" :error-messages="{
                                            required: $t('entityEditor.banner.validation.titleRequired')
                                        }" :placeholder="$t('entityEditor.banner.titlePlaceholder')"
                                        @validateInput="updateValidationState(`title-${index}`, $event)" />

                                    <ValidationInput :id="`link-${index}`" :label="$t('entityEditor.banner.link')"
                                        v-model="banner.link" validationRules="required" :error-messages="{
                                            required: $t('entityEditor.banner.validation.linkRequired')
                                        }" :placeholder="$t('entityEditor.banner.linkPlaceholder')"
                                        @validateInput="updateValidationState(`link-${index}`, $event)" />
                                </div>

                                <ValidationInput :id="`description-${index}`"
                                    :label="$t('entityEditor.banner.description')" v-model="banner.description"
                                    validationRules="required" :error-messages="{
                                        required: $t('entityEditor.banner.validation.descriptionRequired')
                                    }" :placeholder="$t('entityEditor.banner.descriptionPlaceholder')"
                                    @validateInput="updateValidationState(`description-${index}`, $event)" />

                                <div>
                                    <label class="block text-sm font-medium text-gray-700 mb-1">
                                        {{ $t('entityEditor.banner.image') }} <span class="text-red-500">*</span>
                                    </label>

                                    <div class="mt-1 flex items-center">
                                        <div class="bg-gray-100 rounded-lg border border-dashed border-gray-300 p-4 cursor-pointer hover:border-indigo-500 transition-colors flex flex-col items-center justify-center w-full"
                                            :class="{ 'border-red-500': errors[`image-${index}`] }"
                                            @click="triggerFileInput(index)">
                                            <input type="file" :ref="el => fileInputs[index] = el" accept="image/*"
                                                class="hidden" @change="(e) => handleFileSelect(e, index)" />

                                            <template v-if="banner.imagePreview">
                                                <img :src="banner.imagePreview" class="max-h-40 object-contain mb-2"
                                                    :alt="banner.title || $t('entityEditor.banner.preview')" />
                                            </template>
                                            <template v-else>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10 text-gray-400"
                                                    fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                        stroke-width="2"
                                                        d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                                </svg>
                                            </template>

                                            <p class="text-sm text-gray-500 mt-2">{{
                                                $t('entityEditor.banner.clickToSelectImage') }}</p>
                                        </div>
                                    </div>

                                    <p v-if="errors[`image-${index}`]" class="mt-1 text-sm text-red-500">
                                        {{ errors[`image-${index}`] }}
                                    </p>
                                </div>

                                <div v-if="banner.imagePreview" class="mt-4 border rounded-lg overflow-hidden">
                                    <div class="relative aspect-w-16 aspect-h-5 bg-gray-100">
                                        <img :src="banner.imagePreview" class="object-cover w-full h-full"
                                            alt="Banner preview" />
                                        <div
                                            class="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent flex items-end">
                                            <div class="p-4 text-white">
                                                <h3 class="text-xl font-bold">{{ banner.title ||
                                                    $t('entityEditor.banner.titlePlaceholder') }}</h3>
                                                <p class="text-sm opacity-90">{{ banner.description ||
                                                    $t('entityEditor.banner.descriptionPlaceholder')
                                                    }}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EntityEditor>

        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ValidationInput from '@/components/ui/ValidationInput.vue';
import Notification from '@/components/ui/Notification.vue';
import bannerService from '@/services/api/bannerService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'BannerEditor',
    components: {
        EntityEditor,
        ValidationInput,
        Notification
    },
    setup() {
        const { t } = useI18n();
        const router = useRouter();

        const toast = ref(null);
        const fileInputs = ref({});

        const isLoading = ref(false);
        const isSaving = ref(false);
        const isSubmitting = ref(false);
        const existingBanners = ref([]);

        const banners = ref([]);

        const validationState = reactive({});
        const errors = reactive({});

        const { loading: loadingBanners, execute: executeLoadBanners } = useApiRequest();
        const { loading: savingBanners, execute: executeSaveBanners } = useApiRequest();

        const updateLoading = () => {
            isLoading.value = loadingBanners.value;
            isSaving.value = savingBanners.value;
        };

        const loadBanners = async () => {
            await executeLoadBanners(async () => {
                return await bannerService.getBanners();
            }, {
                onSuccess: (data) => {
                    if (data && data.length > 0) {
                        existingBanners.value = data;

                        data.forEach(banner => {
                            banners.value.push({
                                title: banner.title,
                                description: banner.description,
                                link: banner.link,
                                imageUrl: banner.imageUrl,
                                imagePreview: banner.imageUrl,
                                image: NonNullable
                            });
                        });
                    } else {
                        addNewBanner();
                    }
                },
                onError: () => {
                    toast.value.error(t('entityEditor.banner.loadError'));

                    addNewBanner();
                },
                finally: updateLoading
            });
        };

        const addNewBanner = () => {
            banners.value.push({
                title: '',
                description: '',
                link: '',
                imageUrl: '',
                imagePreview: '',
                image: null
            });

            const index = banners.value.length - 1;
            validationState[`title-${index}`] = false;
            validationState[`description-${index}`] = false;
            validationState[`link-${index}`] = false;
            errors[`image-${index}`] = '';
        };

        // Удаление баннера
        const removeBanner = (index) => {
            banners.value.splice(index, 1);

            delete validationState[`title-${index}`];
            delete validationState[`description-${index}`];
            delete validationState[`link-${index}`];
            delete errors[`image-${index}`];

            for (let i = index; i < banners.value.length; i++) {
                validationState[`title-${i}`] = validationState[`title-${i + 1}`];
                validationState[`description-${i}`] = validationState[`description-${i + 1}`];
                validationState[`link-${i}`] = validationState[`link-${i + 1}`];
                errors[`image-${i}`] = errors[`image-${i + 1}`];

                delete validationState[`title-${i + 1}`];
                delete validationState[`description-${i + 1}`];
                delete validationState[`link-${i + 1}`];
                delete errors[`image-${i + 1}`];
            }
        };

        const updateValidationState = (field, isValid) => {
            validationState[field] = isValid;
        };

        const triggerFileInput = (index) => {
            if (fileInputs.value[index]) {
                fileInputs.value[index].click();
            }
        };

        const handleFileSelect = (event, index) => {
            const file = event.target.files[0];
            if (file) {
                processFile(file, index);
            }
        };

        const processFile = (file, index) => {
            if (!file.type.startsWith('image/')) {
                errors[`image-${index}`] = t('entityEditor.banner.validation.imageType');
                return;
            }

            if (file.size > 5 * 1024 * 1024) { // 5MB
                errors[`image-${index}`] = t('entityEditor.banner.validation.imageSize');
                return;
            }

            errors[`image-${index}`] = '';

            const reader = new FileReader();
            reader.onload = (e) => {
                banners.value[index].imagePreview = e.target.result;
                banners.value[index].image = file;
            };
            reader.readAsDataURL(file);
        };

        // Валидация формы
        const validateForm = () => {
            let isValid = true;

            banners.value.forEach((banner, index) => {
                if (!validationState[`title-${index}`]) {
                    toast.value.error(t('entityEditor.banner.validation.titleRequired'));
                    isValid = false;
                }

                if (!validationState[`description-${index}`]) {
                    toast.value.error(t('entityEditor.banner.validation.descriptionRequired'));
                    isValid = false;
                }

                if (!validationState[`link-${index}`]) {
                    toast.value.error(t('entityEditor.banner.validation.linkRequired'));
                    isValid = false;
                }

                if (!banner.image && !banner.imageUrl) {
                    errors[`image-${index}`] = t('entityEditor.banner.validation.imageRequired');
                    toast.value.error(t('entityEditor.banner.validation.imageRequired'));
                    isValid = false;
                }
            });

            return isValid;
        };

        const saveBanners = async () => {

            if (!validateForm()) {
                return;
            }

            isSubmitting.value = true;

            try {
                const bannersToSave = banners.value
                    .filter(banner => banner.image)
                    .map(banner => ({
                        title: banner.title,
                        description: banner.description,
                        link: banner.link,
                        image: banner.image
                    }));

                console.log('Отправляемые баннеры:', bannersToSave);

                await executeSaveBanners(async () => {
                    return await bannerService.createBanner(bannersToSave);
                }, {
                    showErrorNotification: true,
                    notificationRef: toast,
                    onSuccess: () => {
                        toast.value.success(t('entityEditor.banner.saveSuccess'));

                        setTimeout(() => {
                            router.push('/');
                        }, 1500);
                    }
                });
            } catch (error) {
                console.error('Ошибка при сохранении баннеров:', error);
                toast.value.error(t('entityEditor.banner.saveError'));
            } finally {
                isSubmitting.value = false;
            }
        };

        onMounted(() => {
            loadBanners();
        });

        return {
            banners,
            isLoading,
            isSaving,
            isSubmitting,
            validationState,
            errors,
            toast,
            fileInputs,
            addNewBanner,
            removeBanner,
            triggerFileInput,
            handleFileSelect,
            updateValidationState,
            saveBanners
        };
    }
};
</script>

<style scoped>
.aspect-w-16 {
    position: relative;
    padding-bottom: 31.25%;
}

.aspect-w-16>* {
    position: absolute;
    height: 100%;
    width: 100%;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
}
</style>
