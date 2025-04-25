<template>
    <div class="common-content">
        <div v-if="showProviders">
            <div class="row">
                <div class="form-group col-12">
                    <h2 class="nhsuk-heading-l">Content developed with<i v-if="resourceProviderId === null" class="warningTriangle fas fa-exclamation-triangle"></i></h2>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-12">
                    When applicable please select the provider of this content. This will enable users to search for content provided by specific organisations.
                </div>
                <div class="col-12 my-3">
                    <div v-for="provider in userProviders">
                        <label class="my-0">
                            <input class="radio-button" name="resourceProviderId" type="radio" :value="provider.id" v-model="resourceProviderId" @click="setResourceProvider($event.target.value)" />
                            {{provider.name}}
                        </label>
                        <br />
                    </div>
                </div>
            </div>
            <div class="row my-2">
                <div class="accordion col-12" id="provided-by-info-accordion">
                    <div class="pt-0 pb-4">
                        <div class="heading" id="headingProvidedBy">
                            <div class="mb-0">
                                <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseProvidedByInfo" aria-expanded="false" aria-controls="collapseProvidedByInfo">
                                    <div class="accordion-arrow">Why should I flag a resource as 'Developed with'</div>
                                </a>
                            </div>
                        </div>
                        <div id="collapseProvidedByInfo" class="collapse" aria-labelledby="headingProvidedBy" data-parent="#provided-by-info-accordion">
                            <div class="content col-12">
                                <p>
                                    <b>
                                        When publishing a resource it is important to mark a resource as 'Developed with' as it helps;
                                    </b>
                                </p>
                                <ul>
                                    <li>Users search and filter content by specific providers.</li>
                                    <li>Separate learning resources from community contributions.</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group col-12">
                <h2 class="nhsuk-heading-l">Description<i v-if="$v.resourceDescription.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                Write a description that explains the resource and its benefits to learners.
            </div>
            <div class="col-12 my-3">
                <ckeditorwithhint :initialValue="this.resourceDescription" :maxLength="1800" @blur="saveDescription" @change="changeDescription" />
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-12">
                <div class="bg-grey-white">
                    Do you want learners to be notified that this resource contains sensitive content, which they may find offensive or disturbing, before they access it?
                    <div class="mt-3">
                        <label class="checkContainer mb-0" for="sensitivecontent">
                            Yes
                            <input type="checkbox" id="sensitivecontent" v-model="sensitiveContent" @click="setSensitiveContent($event.target.checked)">
                            <span class="checkmark"></span>
                        </label>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h2 id="keyword-label" class="nhsuk-heading-l"><label for="newKeyword">Keywords</label> <i v-if="keywords.length==0" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
            </div>
        </div>
        <div class="row">
            <div class="form-group" v-bind:class="{ 'input-validation-error': keywordError }">
                <div class="col-12 mb-0 error-text" v-if="keywordError">
                    <span class="text-danger">The keyword(s) have already been added : {{formattedkeywordErrorMessage}}</span>
                </div>
                <div class="col-12 mb-0 error-text" v-if="keywordLengthExceeded">
                    <span class="text-danger" id="keyword-label"> 
                        Each keyword must be no longer than 50 characters.
                    </span>
                </div>

                <div class="col-12" id="keyworddesc">
                    To help learners find this resource, type one or more relevant keywords separated by commas and click 'Add'.
                </div>
                <div class="col-12 mt-4 input-with-button">
                    <input id="newKeyword" aria-labelledby="keyword-label" aria-describedby="keyworddesc" type="text" class="form-control nhsuk-input" maxlength="260" v-model="newKeyword" v-bind:class="{ 'input-validation-error': keywordError }" @input="keywordError=false" @change="keywordChange" />
                    <button type="button" class="nhsuk-button nhsuk-button--secondary ml-3 nhsuk-u-margin-bottom-0" @click="addKeyword">&nbsp;Add</button>
                </div>
                <div class="col-12 footer-text" id="keyword-label">
                    You can enter a maximum of 50 characters per keyword
                </div>
            </div>
        </div>
        <div class="keyword-container my-4">
            <div class="keyword-tag" v-for="keyword in keywords">
                {{keyword.keyword}}
                <button class="btn btn-link" aria-label="Delete keyword" @click="deleteKeyword(keyword.id)"><i class="fas fa-times"></i></button>
            </div>
        </div>

        <div v-if="selectedResourceType != ResourceType.WEBLINK">
            <!-- Hide licence section for weblinks -->
            <div class="row mt-4">
                <div class="form-group col-12">
                    <h2 id="licence-label" class="nhsuk-heading-l">Licence <i v-if="resourceLicenceId==0" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
                </div>
            </div>
            <div class="row mb-4">
                <div class="col-12">
                    It is your responsibility to select the most appropriate licence type for this resource.<br />
                    <a :href="resourceLicenseUrl" target="_blank">More information on licences</a>
                </div>
            </div>
            <div class="row">
                <div class="col-12 form-group">
                    <select id="licenceSelection" aria-labelledby="licence-label" class="form-control nhsuk-input" v-model="resourceLicenceId" @change="licenceSelected">
                        <option disabled v-bind:value="0">Please choose...</option>
                        <option v-for="licence in resourceLicences" :value="licence.id">
                            {{ licence.title }}
                        </option>
                    </select>
                </div>
            </div>
        </div>

        <div class="row mt-5">
            <div class="form-group col-12">
                <h2 class="nhsuk-heading-l">Authors<i v-if="authors.length==0" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h2>
            </div>
        </div>
        <div class="row">
            <div class="col-12 mb-5">
                Add the authors that should be attributed to this resource.
                You can add an author as a name, name and organisation, or just an organisation.
                An author could be an individual or a team.
                The role of the author is optional. Enter one author and add it before entering another.
                You can add a maximum of ten authors.
            </div>
            <div v-if="!userIsAuthor && authors.length < maxAllowedAuthors" class="col-12 mb-3">
                <div>
                    <label class="checkContainer" for="currentUserAuthor">
                        I am the author or co-author
                        <input type="checkbox" id="currentUserAuthor" v-model="currentUserAuthor" @change="currentUserAuthorChange">
                        <span class="checkmark"></span>
                    </label>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-12 resource-area-container">
                <div class="resource-area-body" v-if="authors.length < maxAllowedAuthors">
                    <div class="form-group" v-bind:class="{ 'input-validation-error': authorError }">
                        <div class="col-12 mb-0 error-text" v-if="authorError">
                            <span class="text-danger" data-valmsg-for="authorName">Enter the author name or organisation.</span>
                        </div>
                        <div class="col-12">
                            <label class="mb-0" for="authorName">Author name</label>
                        </div>
                        <div class="col-12">
                            <input type="text" id="authorName" name="authorName" class="form-control nhsuk-input" aria-describedby="authorNamehint" v-bind:class="{ 'input-validation-error': authorError }" maxlength="100" v-model="authorName" v-bind:disabled="currentUserAuthor" @input="authorError=false" />
                        </div>
                        <div class="col-12 footer-text" id="authorNamehint">
                            You can enter a maximum of 100 characters
                        </div>
                        <div class="col-12">
                            <label class="mb-0" for="authorOganisation">Organisation</label>
                        </div>
                        <div class="col-12">
                            <input type="text" id="authorOganisation" name="authorOganisation" aria-describedby="authorOganisationhint" class="form-control nhsuk-input" v-bind:class="{ 'input-validation-error': authorError }" maxlength="100" v-model="authorOganisation" @input="authorError=false" />
                        </div>
                        <div class="col-12 footer-text" id="authorOganisationhint">
                            You can enter a maximum of 100 characters
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="mb-0" for="authorRole">Role <span class="optional">(optional)</span></label>
                    </div>
                    <div class="col-12">
                        <input type="text" id="authorRole" name="authorRole" class="form-control nhsuk-input" maxlength="100" v-model="authorRole" aria-describedby="authorRolehint" />
                    </div>
                    <div class="col-12 footer-text" id="authorRolehint">
                        You can enter a maximum of 100 characters
                    </div>
                    <div class="col-12 mt-4 input-with-button">
                        <button type="button" class="nhsuk-button nhsuk-button--secondary nhsuk-u-margin-bottom-0 ml-1" @click="addAuthor">+&nbsp;Add</button>
                    </div>
                </div>
                <div v-for="author in authors" class="author-details mt-3">
                    <div>
                        <i class="fa fa-user" aria-hidden="true"></i> {{author.description}}
                    </div>
                    <div>
                        <button class="btn btn-link" @click="deleteAuthor(author.id)">Delete</button>
                    </div>
                </div>
            </div>
        </div>


        <div class="row mt-5">
            <div class="form-group col-12">
                <h2 id="licence-heading" class="nhsuk-heading-l">Certificate <i v-if="certificateEnabled === null" class="warningTriangle fas fa-exclamation-triangle"></i> </h2>
                <p class="nhsuk-u-font-weight-bold">Should a learner receive a certificate after completing the learning?</p>
            </div>
        </div>
        <div class="row">
            <div class="col-12 resource-area-container">

                <div class="mb-10">
                    <label class="checkContainer mr-0">
                        <span>Yes</span>
                        <input type="radio" name="resourceCertificateRadio" value="true" v-model="certificateEnabled" @click="certificateEnabledChange($event.target.value)" />
                        <span class="radioButton"></span>
                    </label>
                    <label class="checkContainer mr-0">
                        <span>No</span>
                        <input type="radio" name="resourceCertificateRadio" value="false" v-model="certificateEnabled" @click="certificateEnabledChange($event.target.value)" />
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="mb-10">
                    <a v-bind:href="resourceCertificateUrl" class="mb-10 accessible-link" target="_blank">More information on certificates</a>
                </div>
            </div>
        </div>

        <catalogue-select v-if="resourceCatalogueCount>1 && showCatalogueSelect" v-model.number="resourceCatalogueId" @input="catalogueSelected"></catalogue-select>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";
    import * as _ from "lodash";
    import { resourceData } from '../data/resource';
    import { ResourceType } from '../constants';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import { LicenceModel } from '../models/contribute/licenceModel';
    import { AuthorModel } from '../models/contribute/authorModel';
    import { KeywordModel } from '../models/contribute/keywordModel';
    import CKEditorToolbar from '../models/ckeditorToolbar';
    import CKEditor from 'ckeditor4-vue/dist/legacy.js';
    import FilePanel from './FilePanel.vue';
    import CatalogueSelect from './CatalogueSelect.vue';
    import ContentScormLicence from './ContentScormLicence.vue';
    import ckeditorwithhint from '../ckeditorwithhint.vue';
    import { ProviderModel } from '../models/providerModel';

    export default Vue.extend({
        components: {
            FilePanel,
            ckeditor: CKEditor.component,
            CatalogueSelect,
            ContentScormLicence,
            ckeditorwithhint
        },
        data() {
            return {
                maxAllowedAuthors: 10,
                resourceDescription: '',
                resourceDescriptionValid: false,
                sensitiveContent: false,
                resourceLicenceId: 0,
                resourceCatalogueId: 0,
                initialResourceCatalogueId: 0,
                initialNodeId: 0,
                authors: [] as AuthorModel[],
                keywords: [] as KeywordModel[],
                keywordError: false,
                authorName: '',
                authorOganisation: '',
                authorRole: '',
                authorError: false,
                currentUserAuthor: false,
                certificateEnabled: null,
                newKeyword: '',
                editorConfig: { toolbar: CKEditorToolbar.default, versionCheck: false },
                ResourceType,
                resourceProviderId: null,
                keywordLengthExceeded: false,
                keywordErrorMessage:[]
            };
        },
        computed: {
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
            resourceDetail(): ContributeResourceDetailModel {
                return this.$store.state.resourceDetail;
            },
            resourceLicences(): LicenceModel[] {
                return this.$store.state.licences;
            },
            userProviders(): ProviderModel[] {
                return this.$store.state.userProviders;
            },
            resourceCatalogueCount(): number {
                if (!this.$store.state.userCatalogues) {
                    return 0;
                } else {
                    return this.$store.state.userCatalogues.length;
                }
            },
            userIsAuthor(): boolean {
                return this.authors.filter(a => a.isContributor).length > 0;
            },
            resourceLicenseUrl(): string {
                return this.$store.state.resourceLicenseUrl;
            },
            resourceCertificateUrl(): string {
                return this.$store.state.resourceCertificateUrl;
            },
            selectedResourceType(): ResourceType {
                return this.$store.state.resourceDetail.resourceType;
            },
            showCatalogueSelect(): boolean {
                return (this.resourceDetail.resourceCatalogueId === this.resourceDetail.nodeId) || // show if user is contributing into the catalogue root
                    !Boolean(this.$route.query.initialCreate);                                 // or if the user is editing an existing draft (initialCreate=false)
            },
            newKeywordTrimmed(): string {
                return this.newKeyword?.trim().replace(/ +(?= )/g, '');
            },
            showProviders(): boolean {
                if (!this.$store.state.userProviders) {
                    return false
                } else {
                    return this.$store.state.userProviders.length > 0;
                }
            },
            formattedkeywordErrorMessage(): string {
                return this.keywordErrorMessage.join(', ');
            },
        },
        created() {
            this.setInitialValues();
            if (!this.$store.state.licences) {
                this.$store.commit('populateLicences');
            }
            if (!this.$store.state.userCatalogues) {
                this.$store.commit('populateUsersCatalogues');
            }
            if (!this.$store.state.userProviders) {
                this.$store.dispatch("populateUserProviders");
            }
        },
        methods: {
            setInitialValues() {
                if (this.resourceDetail.description) {
                    this.resourceDescription = this.resourceDetail.description;
                } else {
                    this.resourceDescription = '';
                }
                if (this.resourceDetail.resourceLicenceId) {
                    this.resourceLicenceId = this.resourceDetail.resourceLicenceId;
                } else {
                    this.resourceLicenceId = 0;
                }
                if (this.resourceDetail.resourceCatalogueId) {
                    this.resourceCatalogueId = this.resourceDetail.resourceCatalogueId;
                    this.initialResourceCatalogueId = this.resourceDetail.resourceCatalogueId;
                } else {
                    this.resourceCatalogueId = 0;
                    this.initialResourceCatalogueId = 0;
                }
                if (this.resourceDetail.nodeId) {
                    this.initialNodeId = this.resourceDetail.nodeId;
                } else {
                    this.initialNodeId = 0;
                }
                if (this.resourceDetail.certificateEnabled !== null) {
                    this.certificateEnabled = this.resourceDetail.certificateEnabled;
                }
                this.sensitiveContent = this.resourceDetail.sensitiveContent;
                this.authors = this.resourceDetail.resourceAuthors.map(obj => {
                    let auth = new AuthorModel();
                    auth.id = obj.id;
                    auth.authorName = obj.authorName;
                    auth.organisation = obj.organisation;
                    auth.role = obj.role;
                    auth.isContributor = obj.isContributor;
                    return auth;
                });
                this.keywords = this.resourceDetail.resourceKeywords.map(obj => {
                    let kw = new KeywordModel();
                    kw.id = obj.id;
                    kw.keyword = obj.keyword;
                    return kw;
                });
                if (this.resourceDetail.resourceProviderId > 0) {
                    this.resourceProviderId = this.resourceDetail.resourceProviderId;
                }
                else {
                    if (Boolean(this.$route.query.initialCreate)) {
                        this.resourceProviderId = null;
                    }
                    else{
                        this.resourceProviderId = 0;
                    }
                    }
                },
                saveDescription(description: string, valid: boolean) {
                    this.resourceDescription = description;
                    this.resourceDescriptionValid = valid;
                    let field: string = 'description';
                    let value: string = this.resourceDescription;
                    if (valid && this.$store.state.resourceDetail.description != value) {
                        this.$store.commit("saveResourceDetail", { field, value });
                    }
                },
                changeDescription(description: string, valid: boolean) {
                    this.resourceDescription = description;
                    this.resourceDescriptionValid = valid;
                    if (this.$store.state.resourceDetail.description != description) {
                        this.$store.commit("setCommonContentDirty", true);
                    }
                },
                setSensitiveContent(value: boolean) {
                    const field: string = 'sensitiveContent';
                    this.$store.commit("saveResourceDetail", { field, value });
                },
                licenceSelected() {
                    let field: string = 'resourceLicenceId';
                    let value: string = null;
                    if (this.resourceLicenceId != 0) {
                        value = this.resourceLicenceId.toString();
                    }
                    if (this.$store.state.resourceDetail.resourceLicenceId != value) {
                        this.$store.commit("saveResourceDetail", { field, value });
                    }
                },
                catalogueSelected() {
                    let field: string = 'resourceCatalogueId';
                    let value: string = null;
                    if (this.resourceCatalogueId != 0) {
                        value = this.resourceCatalogueId.toString();
                    }
                    if (this.$store.state.resourceDetail.resourceCatalogueId != this.resourceCatalogueId) {
                        this.$store.commit("saveResourceDetail", { field, value });
                        this.updateNodeId();
                    }
                },
                updateNodeId() {
                    // Update the nodeId. This determines where in the content structure that a resource will be saved to.
                    // Iteration 1:
                    // If a catalogue Id AND node Id were passed into the contribute screen, and that catalogue is being reselected,
                    // we have to set the nodeId back to the nodeId originally passed in. If a different catalogue has been selected, the
                    // nodeId has to be set to the catalogue Id, so the resource is saved into the root of that catalogue.
                    let field: string = 'nodeId';
                    let value: string = null;
                    if (this.resourceCatalogueId == this.initialResourceCatalogueId) {
                        value = this.initialNodeId.toString();
                    }
                    else {
                        value = this.resourceCatalogueId.toString();
                    }
                    this.$store.commit("saveResourceDetail", { field, value });
                },
                currentUserAuthorChange() {
                    this.authorError = false;
                    if (this.currentUserAuthor) {
                        this.authorName = this.$store.state.currentUserName;
                    } else {
                        this.authorName = '';
                    }
                },
                certificateEnabledChange(useDefault: boolean) {
                    this.certificateEnabled = useDefault;
                    let value: boolean = this.certificateEnabled;
                    let field: string = 'certificateEnabled';
                    if (this.$store.state.resourceDetail.certificateEnabled != this.certificateEnabled) {
                        this.$store.commit("saveResourceDetail", { field, value });
                    }

                },
                keywordChange() {
                    this.keywordError = false;
                    this.keywordLengthExceeded = false;
                    this.keywordErrorMessage = [];
                },
                resetSelectedLicence() {
                    this.resourceLicenceId = 0;
                },
                setResourceProvider(value: string) {
                    let field: string = 'resourceProviderId';

                    if (this.$store.state.resourceDetail.resourceProviderId != value) {
                        this.$store.commit("saveResourceDetail", { field, value });
                    }
                },
            async addAuthor() {
                    if (this.authorName.trim().length == 0 && this.authorOganisation.trim().length == 0) {
                        this.authorError = true;
                    } else {
                        let newAuthor = new AuthorModel();
                        newAuthor.authorName = this.authorName.trim();
                        newAuthor.organisation = this.authorOganisation.trim();
                        newAuthor.role = this.authorRole.trim();
                        newAuthor.isContributor = this.currentUserAuthor;
                        newAuthor.resourceVersionId = this.resourceDetail.resourceVersionId;
                        newAuthor = await resourceData.addAuthor(this.resourceDetail.resourceVersionId, newAuthor);
                        if (newAuthor.id > 0) {
                            this.$store.commit('addAuthor', newAuthor);
                            this.authors.push(newAuthor);
                            if (this.resourceDetail.resourceVersionId == 0) {
                                this.$store.commit('setResourceVersionId', newAuthor.resourceVersionId)
                            }
                        }
                        this.authorName = '';
                        this.authorOganisation = '';
                        this.authorRole = '';
                        this.currentUserAuthor = false;
                        this.authorError = false;
                    }
                },
            async deleteAuthor(authorId: number) {
                    let response = await resourceData.deleteAuthor(this.resourceDetail.resourceVersionId, authorId);
                    if (response) {
                        this.$store.commit('removeAuthor', authorId);
                        this.authors = _.filter(this.authors, function (f) {
                            return f.id != authorId;
                        });
                    }
                },
            async addKeyword() {
                    if (this.newKeyword && this.newKeywordTrimmed.length > 0) {
                        this.keywordChange();
                        let allTrimmedKeyword = this.newKeywordTrimmed.toLowerCase().split(',');
                        allTrimmedKeyword = allTrimmedKeyword.filter(e => String(e).trim());
                            for (var i = 0; i < allTrimmedKeyword.length; i++) {
                                let item = allTrimmedKeyword[i].trim();
                                if (item.length > 0 && item.length <= 50) {
                                    let newkeywordObj = new KeywordModel();
                                    newkeywordObj.keyword = item;
                                    newkeywordObj.resourceVersionId = this.resourceDetail.resourceVersionId;
                                    newkeywordObj = await resourceData.addKeyword(this.resourceDetail.resourceVersionId, newkeywordObj);
                                    if (newkeywordObj.id > 0) {
                                        this.$store.commit('addKeyword', newkeywordObj);
                                        this.keywords.push(newkeywordObj);
                                        if (this.resourceDetail.resourceVersionId == 0) {
                                            this.$store.commit('setResourceVersionId', newkeywordObj.resourceVersionId)
                                        }
                                        this.newKeyword = '';
                                    } else if (newkeywordObj.id == 0) {
                                        this.newKeyword = '';
                                        this.keywordError = true;
                                        this.keywordErrorMessage.push(item);
                                    }
                                    else {
                                        this.keywordError = true;
                                        break;
                                }
                                }
                                else {
                                    this.keywordLengthExceeded = true;
                                    break;
                                }
                            }
                    }
                    else {
                        this.newKeyword = '';
                    }
                },
            async deleteKeyword(keywordId: number) {
                    let response = await resourceData.deleteKeyword(this.resourceDetail.resourceVersionId, keywordId);
                    if (response) {
                        this.$store.commit('removeKeyword', keywordId);
                        this.keywords = _.filter(this.keywords, function (f) {
                            return f.id != keywordId;
                        });
                    }
                },
            },
            watch: {
                resourceVersionId(value) {
                    this.setInitialValues();
                },
            },
            validations: {
                resourceDescription: {
                    required
                }
            }
        })

</script>
