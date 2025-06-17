<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
          
            <div class="contribute-authors-tab">
                <h2 class="nhsuk-heading-l pt-15">Authors</h2>
                <div>
                    Add up to ten authors for this resource.
                    Add an author name, organisation or both.
                    Enter one author and add it before entering another.
                </div>
                
                <AuthorsList class="my-20" v-bind:resource-details="resourceDetails" />
                
                <div v-if="resourceAuthors.length < maxAllowedAuthors" class="mb-40">
                    <LinkTextAndIcon v-if="!isAdding"
                                     v-on:click="isAdding = true"Please choose...
                                     class="contribute-authors-tab-add-button">
                        <i class="fa-solid fa-plus"></i> Add author
                    </LinkTextAndIcon>

                    <div v-if="isAdding">
                        <div class="mt-5" v-if="!isCurrentUserSavedAsAuthor">
                            <label class="checkContainer" for="currentUserAuthor">
                                I am the author or co-author
                                <input type="checkbox" id="currentUserAuthor" class="nhsuk-checkboxes__input" v-model="authorIsContributor" v-on:change="currentUserAuthorChange">
                                <span class="checkmark nhsuk-input"></span>
                            </label>
                        </div>
                        <CharacterCount v-model="authorName"
                                        v-bind:inputId="txtauthorName"
                                        v-bind:characterLimit="100"
                                        v-bind:disabled="authorIsContributor"
                                        :showTitle="false"
                                        class="mt-25">
                            <template v-slot:description>
                                <label :for="'authorName'" class="mb-0">Author name</label>
                            </template>
                        </CharacterCount>
                        <CharacterCount v-model="authorOrganisation"
                                        v-bind:inputId="txtauthorOrganisation"
                                        v-bind:characterLimit="100"
                                        :showTitle="false"
                                        class="mt-15">
                            <template v-slot:description>
                                <label :for="'authorOrganisation'" class="mb-0">Organisation</label>
                            </template>
                        </CharacterCount>
                        <CharacterCount v-model="authorRole"
                                        v-bind:inputId="txtauthorRole"
                                        v-bind:characterLimit="100"
                                        :showTitle="false"
                                        class="mt-15">
                            <template v-slot:description>
                                <label :for="'authorRole'" class="mb-0">Role</label>
                            </template>
                        </CharacterCount>
                        <Button v-on:click="addAuthor"
                                v-bind:disabled="!authorIsValid"
                                ref="addAuthorButton"
                                class="mt-20">
                            + Add
                        </Button>
                    </div>
                </div>
            </div>
          
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { AuthorModel } from '../models/contribute/authorModel';
    import { resourceData } from '../data/resource';
    import { ContributeConfiguration } from '../models/contribute-resource/contributeConfiguration';
    
    import AuthorsList from './components/AuthorsList.vue';
    import Button from '../globalcomponents/Button.vue';
    import CharacterCount from '../globalcomponents/CharacterCount.vue';
    import LinkTextAndIcon from '../globalcomponents/LinkTextAndIcon.vue';
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            configuration: { type: Object } as PropOptions<ContributeConfiguration>,
            txtauthorName: { type: String, default: 'authorName' },
            txtauthorOrganisation: { type: String, default: 'authorOrganisation' },
            txtauthorRole: { type: String, default: 'authorRole' },
        },
        components: {
            AuthorsList,
            Button,
            CharacterCount,
            LinkTextAndIcon,

        },
        data() {
            return {
                maxAllowedAuthors: 10,
                authorName: '',
                authorOrganisation: '',
                authorRole: '',
                authorIsContributor: false,
                isAdding: false,
            }
        },
        computed: {
            authorIsValid(): boolean {
                return (this.authorName.trim().length > 0 || this.authorOrganisation.trim().length > 0);
            },
            resourceAuthors(): AuthorModel[] {
                return this.resourceDetails.resourceAuthors;
            },
            isCurrentUserSavedAsAuthor(): boolean {
                return this.resourceAuthors.filter(a => a.isContributor).length > 0;
            },
        },
        watch: {
          resourceAuthors: function() {
              this.addInitialAuthor();
          }  
        },
        mounted() {
            this.addInitialAuthor();
        },
        methods: {
            currentUserAuthorChange() {
                if (this.authorIsContributor) {
                    this.authorName = this.configuration.currentUserName;
                } else {
                    this.authorName = '';
                }
            },
            async addAuthor() {
                let newAuthor = this.generateAuthorModel();

                newAuthor = await resourceData.addAuthor(this.resourceDetails.resourceVersionId, newAuthor);
                if (newAuthor.id > 0) {
                    this.resourceDetails.resourceAuthors.push(newAuthor);
                }

                this.resetAuthorData();
                this.isAdding = false;
                
                const addAuthorButtonElement = this.$refs.addAuthorButton as any;
                addAuthorButtonElement.$el.blur(); // To get rid of the yellow focus state, which looks weird when the button is disabled
            },
            generateAuthorModel(): AuthorModel {
                return new AuthorModel({
                    authorName: this.authorName.trim(),
                    organisation: this.authorOrganisation.trim(),
                    role: this.authorRole.trim(),
                    isContributor: this.authorIsContributor,
                    resourceVersionId: this.resourceDetails.resourceVersionId,
                })
            },
            resetAuthorData() {
                this.authorName = '';
                this.authorOrganisation = '';
                this.authorRole = '';
                this.authorIsContributor = false;
            },
            addInitialAuthor() {
                if (this.resourceAuthors.length === 0){
                    this.isAdding = true;
                }
            }
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-authors-tab {
        max-width: 825px;
    }
    
    .checkContainer {
        font-family: $font-stack;
        padding-left: 40px;
    }
    
    .contribute-authors-tab-add-button {
        font-size: 16px;
    }
</style>