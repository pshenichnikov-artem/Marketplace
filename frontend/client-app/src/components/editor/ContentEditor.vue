<template>
    <div>
        <div :class="{ 'hidden': editorMode !== 'wysiwyg' }" class="border rounded-lg overflow-hidden h-300">
            <!-- Панель форматирования -->
            <div id="toolbar" class="border-b">
                <!-- Основные форматы текста -->
                <span class="ql-formats">
                    <select class="ql-font">
                        <option value="arial" selected>Arial</option>
                        <option value="comic-sans">Comic Sans</option>
                        <option value="courier-new">Courier New</option>
                        <option value="georgia">Georgia</option>
                        <option value="helvetica">Helvetica</option>
                        <option value="lucida">Lucida</option>
                        <option value="times-new-roman">Times New Roman</option>
                    </select>
                    <select class="ql-size">
                        <option value="small">Маленький</option>
                        <option selected>Обычный</option>
                        <option value="large">Большой</option>
                        <option value="huge">Огромный</option>
                    </select>
                </span>

                <!-- Стили текста -->
                <span class="ql-formats">
                    <button class="ql-bold" title="Полужирный"></button>
                    <button class="ql-italic" title="Курсив"></button>
                    <button class="ql-underline" title="Подчеркнутый"></button>
                    <button class="ql-strike" title="Зачеркнутый"></button>
                </span>

                <!-- Дополнительные стили -->
                <span class="ql-formats">
                    <button class="ql-script" value="sub" title="Нижний индекс"></button>
                    <button class="ql-script" value="super" title="Верхний индекс"></button>
                </span>

                <!-- Цвета -->
                <span class="ql-formats">
                    <select class="ql-color" title="Цвет текста"></select>
                    <select class="ql-background" title="Цвет фона"></select>
                </span>

                <!-- Выравнивание -->
                <span class="ql-formats">
                    <button class="ql-align" value="" title="Выравнивание по левому краю"></button>
                    <button class="ql-align" value="center" title="Выравнивание по центру"></button>
                    <button class="ql-align" value="right" title="Выравнивание по правому краю"></button>
                    <button class="ql-align" value="justify" title="Выравнивание по ширине"></button>
                </span>

                <!-- Списки -->
                <span class="ql-formats">
                    <button class="ql-list" value="ordered" title="Нумерованный список"></button>
                    <button class="ql-list" value="bullet" title="Маркированный список"></button>
                    <button class="ql-indent" value="-1" title="Уменьшить отступ"></button>
                    <button class="ql-indent" value="+1" title="Увеличить отступ"></button>
                </span>

                <!-- Заголовки -->
                <span class="ql-formats">
                    <select class="ql-header">
                        <option value="1">Заголовок 1</option>
                        <option value="2">Заголовок 2</option>
                        <option value="3">Заголовок 3</option>
                        <option value="4">Заголовок 4</option>
                        <option value="5">Заголовок 5</option>
                        <option value="6">Заголовок 6</option>
                        <option selected>Обычный текст</option>
                    </select>
                </span>

                <!-- Цитаты и код -->
                <span class="ql-formats">
                    <button class="ql-blockquote" title="Цитата"></button>
                    <button class="ql-code-block" title="Блок кода"></button>
                </span>

                <!-- Ссылки и изображения -->
                <span class="ql-formats">
                    <button class="ql-link" title="Ссылка"></button>
                    <button class="ql-image" title="Изображение"></button>
                    <button class="ql-video" title="Видео"></button>
                </span>

                <!-- Форматирование -->
                <span class="ql-formats">
                    <button class="ql-clean" title="Очистить форматирование"></button>
                </span>

                <!-- Направление текста -->
                <span class="ql-formats">
                    <button class="ql-direction" value="rtl" title="Направление текста (справа налево)"></button>
                </span>
            </div>
            <div id="editor" ref="quillEditor" class="quill-editor"></div>
        </div>

        <!-- HTML editor -->
        <div :class="{ 'hidden': editorMode !== 'html' }" class="border rounded-lg overflow-hidden">
            <textarea ref="htmlEditor" v-model="htmlContentModel" @input="onHtmlContentInput"
                class="w-full h-[600px] p-4 font-mono text-sm focus:outline-none focus:ring-1 focus:ring-indigo-500">
            </textarea>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, watch, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import Quill from 'quill';
import 'quill/dist/quill.snow.css';

const List = Quill.import('formats/list');
const Bold = Quill.import('formats/bold');
const Italic = Quill.import('formats/italic');
const Header = Quill.import('formats/header');
const Link = Quill.import('formats/link');

const FontAttributor = Quill.import('attributors/class/font');
FontAttributor.whitelist = [
    'arial',
    'comic-sans',
    'courier-new',
    'georgia',
    'helvetica',
    'lucida',
    'times-new-roman'
];
Quill.register(FontAttributor, true);

Quill.register(List, true);
Quill.register(Bold, true);
Quill.register(Italic, true);
Quill.register(Header, true);
Quill.register(Link, true);

export default {
    name: 'ContentEditor',
    props: {
        value: {
            type: String,
            default: ''
        },
        mode: {
            type: String,
            default: 'wysiwyg',
            validator: (value) => ['wysiwyg', 'html'].includes(value)
        },
        toast: {
            type: Object,
            default: null
        }
    },
    emits: ['update:value', 'update:mode'],
    setup(props, { emit }) {
        const { t } = useI18n();
        const quillEditor = ref(null);
        const htmlEditor = ref(null);
        const editor = ref(null);

        const htmlContent = ref('');
        const visualContent = ref('');

        const editorMode = computed({
            get: () => props.mode,
            set: (newMode) => emit('update:mode', newMode)
        });

        const htmlContentModel = computed({
            get: () => htmlContent.value,
            set: (newValue) => {
                htmlContent.value = newValue;
                if (editorMode.value === 'html') {
                    emit('update:value', newValue);
                }
            }
        });

        const onHtmlContentInput = (event) => {
            const newContent = event.target.value;
            htmlContent.value = newContent;

            if (editorMode.value === 'html') {
                emit('update:value', newContent);
            }
            console.log('HTML editor content updated:', newContent);
        };

        watch(() => props.mode, (newMode, oldMode) => {
            console.log(`Editor mode changed from ${oldMode} to ${newMode}`);

            if (newMode === 'html') {
                emit('update:value', htmlContent.value);

                setTimeout(() => {
                    if (htmlEditor.value) {
                        htmlEditor.value.focus();
                    }
                }, 100);
            } else if (newMode === 'wysiwyg') {
                try {
                    if (editor.value) {

                        emit('update:value', visualContent.value);

                        setTimeout(() => {
                            editor.value.focus();
                        }, 100);
                    }
                } catch (error) {
                    console.error('Error switching to WYSIWYG mode:', error);
                    if (props.toast) {
                        props.toast.error(t('dashboard.pages.wysiwyg.error'));
                    }
                    emit('update:mode', 'html');
                }
            }
        });

        const initQuill = () => {
            try {
                editor.value = new Quill(quillEditor.value, {
                    modules: {
                        toolbar: '#toolbar',
                        history: {
                            delay: 2000,
                            maxStack: 500,
                            userOnly: true
                        }
                    },
                    theme: 'snow',
                    placeholder: 'Начните вводить содержимое страницы...',
                });

                editor.value.keyboard.addBinding({ key: 'B', shortKey: true }, function (range, context) {
                    this.quill.format('bold', !context.format.bold);
                });

                editor.value.keyboard.addBinding({ key: 'I', shortKey: true }, function (range, context) {
                    this.quill.format('italic', !context.format.italic);
                });

                editor.value.keyboard.addBinding({ key: 'U', shortKey: true }, function (range, context) {
                    this.quill.format('underline', !context.format.underline);
                });

                if (props.value) {
                    try {
                        visualContent.value = props.value;
                        htmlContent.value = props.value;

                        if (editorMode.value === 'wysiwyg' && editor.value) {
                            editor.value.root.innerHTML = props.value;
                        }
                    } catch (error) {
                        console.error("Error setting editor content:", error);
                        editorMode.value = 'html';
                    }
                }

                editor.value.on('text-change', function () {
                    const content = editor.value.root.innerHTML;
                    visualContent.value = content;

                    if (editorMode.value === 'wysiwyg') {
                        emit('update:value', content);
                    }
                    console.log('WYSIWYG editor content updated:', content);
                });

                editor.value.root.addEventListener('click', function () {
                    editor.value.focus();
                });
            } catch (error) {
                console.error("Error initializing Quill editor:", error);
                if (props.toast) {
                    props.toast.error("Ошибка инициализации редактора. Пожалуйста, обновите страницу.");
                }
            }
        };

        watch(() => props.value, (newValue) => {
            if (editorMode.value === 'wysiwyg') {
                visualContent.value = newValue;
                if (editor.value) {
                    try {
                        const textChangeListeners = editor.value.emitter?.listeners('text-change') || [];
                        editor.value.emitter.removeAllListeners('text-change');

                        if (editor.value.root.innerHTML !== (newValue || '')) {
                            editor.value.root.innerHTML = newValue || '';
                        }

                        if (Array.isArray(textChangeListeners)) {
                            textChangeListeners.forEach(listener => {
                                editor.value.emitter.on('text-change', listener);
                            });
                        }
                    } catch (error) {
                        console.error('Error updating WYSIWYG content:', error);
                    }
                }
            } else {
                htmlContent.value = newValue;
            }
        });

        onMounted(() => {
            initQuill();

            htmlContent.value = props.value || '';
            visualContent.value = props.value || '';

            if (editor.value && props.value) {
                try {
                    editor.value.root.innerHTML = props.value || '';
                } catch (error) {
                    console.error("Error setting initial Quill content:", error);
                }
            }


        });

        return {
            quillEditor,
            htmlEditor,
            editor,
            editorMode,
            htmlContentModel,
            onHtmlContentInput
        };
    }
};
</script>

<style scoped>
.quill-editor {
    height: 600px !important;
    max-height: 600px !important;
    overflow-y: auto;
    position: relative;
    z-index: 1;
}

:deep(.ql-container) {
    height: 500px !important;
    max-height: 500px !important;
    overflow-y: auto;
    position: relative;
    z-index: 1;
}

:deep(.ql-toolbar) {
    border-bottom: 1px solid #e2e8f0;
    padding: 12px !important;
    position: relative;
    z-index: 2;
    background-color: #f9fafb;
}

:deep(.ql-toolbar .ql-formats) {
    margin-right: 15px !important;
    position: relative;
    margin-bottom: 8px !important;
    display: inline-flex !important;
    align-items: center !important;
}

:deep(.ql-toolbar button) {
    height: 28px !important;
    width: 28px !important;
    padding: 4px !important;
    margin: 2px !important;
    border-radius: 4px !important;
}

:deep(.ql-toolbar button:hover) {
    color: #4f46e5;
    background-color: #eef2ff !important;
}

:deep(.ql-toolbar .ql-picker) {
    height: 28px !important;
    margin-right: 5px !important;
    position: relative;
}

:deep(.ql-toolbar .ql-picker-label) {
    padding: 4px 8px !important;
    font-size: 14px !important;
    display: flex !important;
    align-items: center !important;
}

:deep(.ql-picker-options) {
    display: none !important;
    z-index: 10 !important;
    position: absolute !important;
    top: 100% !important;
    background-color: white !important;
    border: 1px solid #ccc !important;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1) !important;
    min-width: 120px !important;
}

:deep(.ql-picker.ql-expanded .ql-picker-options) {
    display: block !important;
    margin-top: 5px !important;
    z-index: 20 !important;
}

:deep(.ql-picker-item) {
    padding: 5px 8px !important;
    cursor: pointer !important;
}

:deep(.ql-picker-item:hover) {
    background-color: #f3f4f6 !important;
}

textarea.w-full {
    font-family: monospace;
    color: #333;
    line-height: 1.5;
    tab-size: 2;
}

textarea:focus {
    box-shadow: 0 0 0 2px rgba(79, 70, 229, 0.2);
}
</style>

<style>
.ql-font-arial {
    font-family: Arial, sans-serif;
}

.ql-font-comic-sans {
    font-family: "Comic Sans MS", cursive, sans-serif;
}

.ql-font-courier-new {
    font-family: "Courier New", Courier, monospace;
}

.ql-font-georgia {
    font-family: Georgia, serif;
}

.ql-font-helvetica {
    font-family: Helvetica, Arial, sans-serif;
}

.ql-font-lucida {
    font-family: "Lucida Sans Unicode", "Lucida Grande", sans-serif;
}

.ql-font-times-new-roman {
    font-family: "Times New Roman", Times, serif;
}

.ql-size-small {
    font-size: 0.75em;
}

.ql-size-large {
    font-size: 1.5em;
}

.ql-size-huge {
    font-size: 2.5em;
}

.ql-align-center {
    text-align: center;
}

.ql-align-right {
    text-align: right;
}

.ql-align-justify {
    text-align: justify;
}

.ql-indent-1 {
    padding-left: 3em;
}

.ql-indent-2 {
    padding-left: 6em;
}

.ql-indent-3 {
    padding-left: 9em;
}

.ql-syntax {
    background-color: #f0f0f0;
    border-radius: 3px;
    padding: 10px;
    font-family: monospace;
    white-space: pre-wrap;
}

.ql-editor blockquote {
    border-left: 4px solid #ccc;
    padding-left: 16px;
    margin-left: 0;
    margin-right: 0;
}

[dir=rtl] {
    text-align: right;
}

.ql-picker-label {
    position: relative !important;
    z-index: 5 !important;
    display: flex !important;
    align-items: center !important;
    cursor: pointer !important;
    font-size: 14px !important;
}

.ql-picker-options {
    display: none;
    position: absolute !important;
    top: 100% !important;
    left: 0 !important;
    background-color: white !important;
    border: 1px solid #ccc !important;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15) !important;
    z-index: 20 !important;
}

.ql-picker.ql-expanded .ql-picker-label {
    border-color: #ccc !important;
    color: #4f46e5 !important;
}

.ql-picker.ql-expanded .ql-picker-options {
    display: block !important;
}

.ql-picker-item {
    display: block !important;
    padding: 5px 12px !important;
    cursor: pointer !important;
    width: 100% !important;
}

.ql-picker-item:hover {
    background-color: rgba(79, 70, 229, 0.1) !important;
    color: #4f46e5 !important;
}

.ql-picker {
    display: inline-block !important;
    position: relative !important;
    vertical-align: middle !important;
}

.ql-formats {
    display: inline-block !important;
    margin-right: 15px !important;
    vertical-align: middle !important;
}
</style>
