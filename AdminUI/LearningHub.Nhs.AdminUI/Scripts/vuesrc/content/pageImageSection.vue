<template>
    <div class="py-5">
        <a :href="backUrl()">
            <i class="fa-solid fa-chevron-left">&nbsp;</i>
            Back
        </a>

        <div class="pt-5 pb-4">
            <h1>Image and text</h1>
        </div>

        <div class="pb-5" id="rowTitleDiv">
            <label class="control-label">Section Title (optional)</label><br />
            <input class="form-control w-75" type="text" name="rowTitle" v-model="sectionTitle" maxlength="128" />
            <small>You can enter a maximum of 128 characters.</small><br />
        </div>

        <div class="nhsuk-form-group">
            <fieldset class="nhsuk-fieldset">
                <legend class="nhsuk-fieldset__legend nhsuk-fieldset__legend--l">
                    <h1 class="nhsuk-fieldset__heading">
                        Section Title style
                    </h1>
                </legend>
                <div class="nhsuk-radios nhsuk-radios--inline">

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h1" name="sectionTitleElement" type="radio" value="h1" v-model="sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h1">
                            H1
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h2" name="sectionTitleElement" type="radio" value="h2" v-model="sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h2">
                            H2
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h3" name="sectionTitleElement" type="radio" value="h3" v-model="sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="radio-h3">
                            H3
                        </label>
                    </div>

                    <div class="nhsuk-radios__item">
                        <input class="nhsuk-radios__input" id="radio-h4" name="sectionTitleElement" type="radio" value="h4" v-model="sectionTitleElement">
                        <label class="nhsuk-label nhsuk-radios__label" for="h4">
                            H4
                        </label>
                    </div>
                </div>
            </fieldset>
        </div>

        <div class="pb-5 pt-3">
            <div class="d-flex justify-content-start">
                <input type="checkbox" v-model="topMargin" name="chkTopMargin" id="chkTopMargin" class="checkbox-large" />
                <label for="chkTopMargin" class="ml-2 mt-2">Top margin</label>
            </div>
            <div class="d-flex justify-content-start">
                <input type="checkbox" v-model="bottomMargin" name="chkBottomMargin" id="chkBottomMargin" class="checkbox-large" />
                <label for="chkBottomMargin" class="ml-2 mt-2">Bottom margin</label>
            </div>
            <div class="d-flex justify-content-start">
                <input type="checkbox" v-model="hasBorder" name="chkHasBorder" id="chkHasBorder" class="checkbox-large" />
                <label for="chkHasBorder" class="ml-2 mt-2">Has Border</label>
            </div>
        </div>

        <div class="py-5">
            <label class="control-label">Image Alignment</label><br />
            <div class="d-flex">
                <div class="pr-5 mr-3">
                    <div v-if="sectionLayoutType == SectionLayoutType.Left" class="position-relative">
                        <img class="img-fluid" src="/images/image-text-on-left-selected.svg" alt="Image text on the left" />
                        <img src="/images/check-circle.svg" class="img-fluid selected-check-circle" alt="Check circle" />
                    </div>

                    <a v-else @click="setSectionLayoutType(SectionLayoutType.Left)">
                        <img class="img-fluid" src="/images/image-text-on-left.svg" alt="Image text on the left" />
                    </a>
                </div>
                <div>
                    <div v-if="sectionLayoutType == SectionLayoutType.Right" class="position-relative">
                        <img class="img-fluid" src="/images/image-text-on-right-selected.svg" alt="Image text on the right" />
                        <img src="/images/check-circle.svg" class="img-fluid selected-check-circle" alt="Check circle" />
                    </div>

                    <a v-else @click="setSectionLayoutType(SectionLayoutType.Right)">
                        <img class="img-fluid" src="/images/image-text-on-right.svg" alt="Image text on the right" />
                    </a>
                </div>
            </div>
        </div>

        <div class="pb-5 pt-3" id="featureImageDiv">
            <label class="control-label">Feature image</label><br />
            <span class="text-secondary">Image must be in either .jpg, .png or .gif format.</span>

            <div class="w-75 my-2" style="background-color:#F0F4F5">
                <div :class="showImage ? '' : 'd-none'">

                    <div class="d-flex justify-content-center">
                        <div class="py-5">
                            <div class="d-block">
                                <img id="imgPreview" alt="preview image" class="img-fluid border" v-bind:src="imageUrl" />
                            </div>
                            <div class="d-block mt-4">
                                <a @click="$refs.fileInput.click()">Change image</a>
                            </div>
                        </div>
                    </div>

                    <div class="p-4" style="background-color:#D8DDE0" id="imageAltDiv">
                        <label class="control-label">Alt Tag</label><br />
                        <small>Provide a short description of the image using 100 charcters or less. This will not be shown on screen but it will help those using screen readers.</small>
                        <input class="form-control" type="text" name="imageAlt" v-model="imageAlt" maxlength="100" @blur="validateImageAlt" />
                        <span v-if="invalidImageAlt" class="text-danger field-validation-error">Enter a image description</span>
                    </div>
                </div>

                <div v-if="!showImage" class="p-5">
                    <button class="btn btn-nhs-common btn-outline-secondary bg-white mr-4" type="button" @click="$refs.fileInput.click()">Browse</button>
                    No file selected
                </div>
            </div>

            <input class="d-none" type="file" name="featureImage" ref="fileInput"
                   accept="image/gif, image/jpeg, image/png" @change="onFeatureImageChange" />
            <span v-if="imageErrorMessage != ''" class="text-danger field-validation-error d-block" v-text="imageErrorMessage"></span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Background colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-white ${backgroundColour == white ? 'selected' : ''}`]" @click="setBgColor(white)" />
                <i :class="[`section-bg section-bg-grey ${backgroundColour == grey3 ? 'selected' : ''}`]" @click="setBgColor(grey3)" />
                <i :class="[`section-bg section-bg-yellow ${backgroundColour == yellow ? 'selected' : ''}`]" @click="setBgColor(yellow)" />
                <i :class="[`section-bg section-bg-blue ${backgroundColour == blue ? 'selected' : ''}`]" @click="setBgColor(blue)" />
                <i :class="[`section-bg section-bg-grey-1 ${backgroundColour == grey1 ? 'selected' : ''}`]" @click="setBgColor(grey1)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" v-model="customBgColor" type="text" @change="setBgColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidBgColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Text colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-black ${textColour == black ? 'selected' : ''}`]" @click="setColor(black)" />
                <i :class="[`section-bg section-bg-white ${textColour == white ? 'selected' : ''}`]" @click="setColor(white)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" type="text" v-model="customTextColor" @change="setColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Text background colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-transparent ${textBackgroundColour == transparent ? 'selected' : ''}`]" @click="setTxtBgColor(transparent)" />
                <i :class="[`section-bg section-bg-white ${textBackgroundColour == white ? 'selected' : ''}`]" @click="setTxtBgColor(white)" />
                <i :class="[`section-bg section-bg-grey ${textBackgroundColour == grey3 ? 'selected' : ''}`]" @click="setTxtBgColor(grey3)" />
                <i :class="[`section-bg section-bg-yellow ${textBackgroundColour == yellow ? 'selected' : ''}`]" @click="setTxtBgColor(yellow)" />
                <i :class="[`section-bg section-bg-blue ${textBackgroundColour == blue ? 'selected' : ''}`]" @click="setTxtBgColor(blue)" />
                <i :class="[`section-bg section-bg-grey-1 ${textBackgroundColour == grey1 ? 'selected' : ''}`]" @click="setTxtBgColor(grey1)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" v-model="customTxtBgColor" type="text" @change="setTxtBgColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidTxtBgColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3">
            <label class="control-label">Hyperlink colour</label><br />
            <small>Pick a colour from the colour palette below or add your own hex value.</small>

            <div class="d-flex align-items-center mt-4">
                <i :class="[`section-bg section-bg-blue ${hyperLinkColour == blue ? 'selected' : ''}`]" @click="setHyperlinkColor(blue)" />
                <i :class="[`section-bg section-bg-white ${hyperLinkColour == white ? 'selected' : ''}`]" @click="setHyperlinkColor(white)" />
                <div class="input-color">
                    <i class="fa-solid fa-hashtag input-color-hash"></i>
                    <input class="form-control input-color-field" type="text" v-model="customHyperlinkColor" @change="setHyperlinkColor(`#${$event.target.value}`, true)" />
                </div>
            </div>
            <span v-if="invalidHyperlinkColor" class="text-danger field-validation-error">Enter a valid hex value</span>
        </div>

        <div class="pb-5 pt-3 w-75" id="descriptionDiv">
            <label class="control-label">Description</label>

            <ckeditor v-model="description" :config="editorConfig" @ready="onEditorReady" @blur="validateDescription"></ckeditor>

            <span v-if="invalidDescription" class="text-danger field-validation-error">Enter a description</span>
        </div>

        <div class="d-flex my-5 w-75">
            <button :class="[`btn btn-nhs-common ${valid ? 'btn-success' : 'btn-secondary'}`]" @click="save" type="button">Save</button>
            <button class="btn btn-nhs-common btn-outline-primary ml-5" type="button" @click="cancelConfirmation">Cancel</button>
        </div>

        <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content" style="max-width:450px">
                    <div class="text-center">
                        <i class="fas fa-exclamation-triangle text-warning">&nbsp;</i>
                        <h4 class="d-inline" id="exampleModalLongTitle">Discard changes?</h4>
                        <p class="p-4 mt-5 mb-5 small side-menu">All changes made will be lost. This action cannot be undone.</p>
                    </div>
                    <div class="d-flex flex-row justify-content-between">
                        <button type="button" class="btn btn-common btn-outline-primary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-common btn-success" @click="cancel">Continue</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { contentData } from '../data/content';
    import { SectionLayoutType } from '../models/content/pageSectionDetailModel';
    import CKEditorToolbar from '../models/ckeditorToolbar';
    import CKEditor from 'ckeditor4-vue/dist/legacy.js';

    export default Vue.extend({
        components: {
            ckeditor: CKEditor.component
        },
        props: {
        },
        data() {
            return {
                pageSectionDetailId: 0,
                transparent: '',
                white: '#FFFFFF',
                yellow: '#FFED00',
                blue: '#005EB8',
                black: '#212B32',
                grey1: '#425563',
                grey3: '#F0F4F5',
                SectionLayoutType: SectionLayoutType,
                editorConfig: {
                    toolbar: CKEditorToolbar.landingPages,
                    versionCheck: false,
                    stylesSet: 'landing-pages-image-text'
                },

                invalidBgColor: false,
                invalidColor: false,
                invalidHyperlinkColor: false,
                invalidTxtBgColor: false,
                invalidImageAlt: false,
                invalidDescription: false,

                showImage: false,
                imageErrorMessage: '',
                customBgColor: '',
                customTextColor: '',
                customHyperlinkColor: '',
                customTxtBgColor: '',

                sectionTitle: '',
                topMargin: false,
                bottomMargin: false,
                hasBorder: false,

                imageUrl: '',
                imageAlt: '',
                sectionLayoutType: SectionLayoutType.Left,
                backgroundColour: '#FFFFFF',
                textColour: '#212B32',
                hyperLinkColour: '#005EB8',
                textBackgroundColour: '',
                description: '',
                imageFile: null as File,
                sectionTitleElement: 'h2'
            }
        },
        async created() {
            const sectionId = this.$route.params.sectionId;
            if (!isNaN(parseInt(sectionId))) {
                const detail = await contentData.getPageSectionDetailForEdit(parseInt(sectionId));

                if (detail) {
                    this.pageSectionDetailId = detail.id;
                    if (detail.imageAsset) {
                        this.imageAlt = detail.imageAsset.altTag;
                        const imageFile = detail.imageAsset.imageFile;
                        this.imageUrl = `/file/download/${imageFile.filePath}/${imageFile.fileName}`;
                        this.showImage = true;
                    }

                    this.sectionTitle = detail.sectionTitle ?? '';
                    this.sectionTitleElement = detail.sectionTitleElement ?? 'h2';
                    this.topMargin = detail.topMargin;
                    this.bottomMargin = detail.bottomMargin;
                    this.hasBorder = detail.hasBorder;

                    this.sectionLayoutType = detail.sectionLayoutType;
                    this.backgroundColour = detail.backgroundColour || this.white;
                    this.textColour = detail.textColour || this.black;
                    this.hyperLinkColour = detail.hyperLinkColour || this.blue;
                    this.textBackgroundColour = detail.textBackgroundColour || this.transparent;
                    this.description = detail.description;

                    if (this.backgroundColour != this.white
                        && this.backgroundColour != this.grey3
                        && this.backgroundColour != this.yellow
                        && this.backgroundColour != this.blue
                        && this.backgroundColour != this.grey1) {
                        this.customBgColor = this.backgroundColour.substring(1);
                    }

                    if (this.textColour != this.black
                        && this.textColour != this.white) {
                        this.customTextColor = this.textColour.substring(1);
                    }

                    if (this.hyperLinkColour != this.blue
                        && this.hyperLinkColour != this.white) {
                        this.customHyperlinkColor = this.hyperLinkColour.substring(1);
                    }

                    if (this.textBackgroundColour != this.transparent
                        && this.textBackgroundColour != this.white
                        && this.textBackgroundColour != this.grey3
                        && this.textBackgroundColour != this.yellow
                        && this.textBackgroundColour != this.blue
                        && this.textBackgroundColour != this.grey1) {
                        this.customTxtBgColor = !this.textBackgroundColour ? '' : this.textBackgroundColour.substring(1);
                    }
                }
            }
        },
        computed: {
            valid: function () {
                const vm = this as any;
                return !vm.invalidBgColor &&
                    !vm.invalidColor &&
                    !vm.invalidHyperlinkColor &&
                    !vm.invalidTxtBgColor &&
                    !vm.isEmptyOrSpaces(vm.imageAlt) &&
                    !vm.invalidDescription;
            }
        },
        methods: {
            setSectionLayoutType(sectionLayoutType: SectionLayoutType) {
                this.sectionLayoutType = sectionLayoutType;
            },
            setBgColor(color: string, isCustom: boolean = false) {
                this.invalidBgColor = this.validateColorHex(color);
                this.backgroundColour = color;
                if (!isCustom) this.customBgColor = '';
            },
            setColor(color: string, isCustom: boolean = false) {
                this.invalidColor = this.validateColorHex(color);
                this.textColour = color;
                if (!isCustom) this.customTextColor = '';
            },
            setHyperlinkColor(color: string, isCustom: boolean = false) {
                this.invalidHyperlinkColor = this.validateColorHex(color);
                this.hyperLinkColour = color;
                if (!isCustom) this.customHyperlinkColor = '';
            },
            setTxtBgColor(color: string, isCustom: boolean = false) {
                this.invalidTxtBgColor = color != '' && this.validateColorHex(color);
                this.textBackgroundColour = color;
                if (!isCustom) this.customTxtBgColor = '';
            },
            validateColorHex(color: string) {
                return !(/^#([0-9A-F]{3}){1,2}$/i.test(color));
            },
            validateImageAlt() {
                this.invalidImageAlt = this.isEmptyOrSpaces(this.imageAlt);
            },
            validateDescription() {
                this.invalidDescription = this.isEmptyOrSpaces(CKEDITOR.instances.editor1.document.getBody().getText());
            },            
            onEditorReady(editor: any) {
                var current = this;
                editor.on('change', function () {
                    current.validateDescription();
                });
            },
            validateImage() {
                if (!this.showImage) {
                    this.imageErrorMessage = 'Upload an image.';
                } else {
                    this.validateImageAlt();
                }
            },
            isEmptyOrSpaces(str: string) {
                str = str.trim();
                return str === null || str === undefined || str.match(/^ *$/) !== null;
            },
            onFeatureImageChange(event: any) {
                const file = (event.target as HTMLInputElement).files[0];
                if (file) {
                    const vm = this;

                    if (!this.isImage(file.name)) {
                        vm.imageErrorMessage = 'The image must be in .jpg, .png or .gif format';
                        return;
                    } else {
                        vm.imageErrorMessage = ''
                    };

                    const reader = new FileReader();
                    reader.readAsDataURL(file);

                    reader.onload = function (e) {
                        const src = e.target.result as string;

                        vm.ensureImageIsValid(src).then(valid => {
                            if (valid) {
                                $('#imgPreview').attr('src', src);
                                vm.imageErrorMessage = '';
                                vm.imageFile = file;
                                vm.showImage = true;
                            } else {
                                vm.imageErrorMessage = 'Invalid image';
                            }
                        });
                    }
                }
            },
            ensureImageIsValid(imgSrc: string) {
                return new Promise(function (resolve, reject) {
                    const image = new Image();
                    image.onload = function () {
                        const img = this as HTMLImageElement;
                        const invalid = img.width + img.height == 0;
                        resolve(!invalid);
                    };
                    image.onerror = function () {
                        resolve(false);
                    };
                    image.src = imgSrc;
                });
            },
            isImage(filename: string) {
                const ext = this.getExtension(filename);
                switch (ext.toLowerCase()) {
                    case 'jpg':
                    case 'pjp':
                    case 'pjpeg':
                    case 'jpeg':
                    case 'gif':
                    case 'jfif':
                    case 'png':
                        return true;
                }
                return false;
            },
            getExtension(filename: string) {
                const parts = filename.split('.');
                return parts[parts.length - 1];
            },
            scrollTo(elem: string) {
                document.getElementById(elem).scrollIntoView();
            },
            backUrl() {
                return `/cms/page/${this.$route.params.pageId}`;
            },
            async save() {
                this.validateDescription();
                this.validateImage();

                if (!this.showImage) this.scrollTo("featureImageDiv");
                else if (this.invalidImageAlt) this.scrollTo("imageAltDiv");
                else if (this.invalidDescription) this.scrollTo("descriptionDiv");

                if (this.valid) {
                    const data = {
                        position: this.$route.query.position || 1,
                        imageAlt: this.imageAlt,
                        sectionLayoutType: this.sectionLayoutType,
                        backgroundColour: this.backgroundColour,
                        textColour: this.textColour,
                        hyperLinkColour: this.hyperLinkColour,
                        textBackgroundColour: this.textBackgroundColour,
                        description: this.description,
                        sectionTitle: this.sectionTitle,
                        sectionTitleElement: this.sectionTitleElement,
                        topMargin: this.topMargin,
                        bottomMargin: this.bottomMargin,
                        hasBorder: this.hasBorder,
                        imageFile: this.imageFile,
                        pageSectionDetailId: this.pageSectionDetailId 
                    } as any;

                    const form_data = new FormData();
                    for (let key in data) {
                        form_data.append(key, data[key]);
                    }

                    const success = await contentData.updatePageImageSectionDetail(parseInt(this.$route.params.pageId), form_data);

                    if (success) {
                        window.location.replace(`/cms/page/${this.$route.params.pageId}`);
                    } else {
                        console.log("Error saving data..");
                    }
                }
            },
            cancelConfirmation() {
                $('#deleteModal').modal();
            },
            cancel() {
                window.location.replace(`/cms/page/${this.$route.params.pageId}`);
            }
        },
    });
</script>