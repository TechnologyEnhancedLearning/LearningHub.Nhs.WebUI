<template>
    <div class="common-content">

        <div class="row">
            <div class="form-group" v-bind:class="{ 'input-validation-error': keywordError }">
                <div class="col-12 mb-0 error-text" v-if="keywordError">
                    <span class="text-danger">The keyword(s) have already been added : {{formattedkeywordErrorMessage}}</span>
                </div>
                <div class="col-12 mb-0 error-text" v-if="keywordLengthExceeded">
                    <span class="text-danger">
                        Each keyword must be no longer than 50 characters.
                    </span>
                </div>

                <div class="col-12">
                    To help learners find this resource, type one or more relevant keywords separated by commas and click 'Add'.
                </div>
                <div class="col-12 input-with-button">
                    <input id="newKeyword" aria-labelledby="keyword-label" type="text" class="form-control" maxlength="260" v-model="newKeyword" v-bind:class="{ 'input-validation-error': keywordError }" @input="keywordError=false" @change="keywordChange" />
                    <button type="button" class="nhsuk-button nhsuk-button--secondary ml-3 button_width nhsuk-u-margin-bottom-0" @click="addKeyword">&nbsp;Add</button>
                </div>
                <div class="col-12 footer-text">
                    You can enter a maximum of 50 characters per keyword
                </div>
            </div>
        </div>    
        <div class="keyword-container my-4 d-flex">
            <div class="keyword-tag" v-for="keyword in resourceDetails.resourceKeywords" :key="keyword.id">
                <button class="btn btn-link" aria-label="Delete keyword" @click="deleteKeyword(keyword.id)">
                    <i class="fas fa-times"></i>
                </button>
                {{ keyword.keyword }}
            </div>
        </div>

    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import * as _ from "lodash";
    
    import { KeywordModel } from "../../models/contribute/keywordModel";
    import { resourceData } from "../../data/resource";
    import { ContributeResourceDetailModel } from "../../models/contribute/contributeResourceModel";

    import Button from '../../globalcomponents/Button.vue';
    import CharacterCount from '../../globalcomponents/CharacterCount.vue';
    
    export default Vue.extend({
        components: {
            Button,
            CharacterCount,
        },
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        data() {
            return {
                newKeyword: '',
                keywordError: false,
                keywordLengthExceeded: false,
                keywordErrorMessage: []
            }
        },
        computed: {
            resourceVersionId() {
                return this.resourceDetails.resourceVersionId;
            },
            newKeywordTrimmed(): string {
                return this.newKeyword?.trim().replace(/ +(?= )/g, '').toLowerCase();
            },
            formattedkeywordErrorMessage(): string {
                return this.keywordErrorMessage.join(', ');
            },
        },
        methods: {
            keywordChange() {
                this.keywordError = false;
                this.keywordLengthExceeded = false;
                this.keywordErrorMessage = [];
            },
            async addKeyword() {  
                if (this.newKeyword && this.newKeywordTrimmed.length > 0) {
                    let allTrimmedKeyword = this.newKeywordTrimmed.toLowerCase().split(',');
                    allTrimmedKeyword = allTrimmedKeyword.filter(e => String(e).trim());
                        for (var i = 0; i < allTrimmedKeyword.length; i++) {
                            let item = allTrimmedKeyword[i];
                            if (item.length > 0 && item.length <= 50) {
                                let newKeywordObj = new KeywordModel({
                                    keyword: item,
                                    resourceVersionId: this.resourceVersionId,
                                });
                                newKeywordObj = await resourceData.addKeyword(this.resourceVersionId, newKeywordObj);
                                if (newKeywordObj.id > 0) {
                                    this.resourceDetails.resourceKeywords.push(newKeywordObj);
                                    this.newKeyword = '';
                                } else if (newKeywordObj.id == 0) {
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
                            }
                        }
                }
            },
            async deleteKeyword(keywordId: number) {
                let response = await resourceData.deleteKeyword(this.resourceVersionId, keywordId);
                if (response) {
                    this.resourceDetails.resourceKeywords = _.filter(this.resourceDetails.resourceKeywords, k => k.id != keywordId);
                }
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .input-with-button {
        display: flex;
        align-items: center;
    }
    .button_width
    {
        width:15%;
    }

    .input-with-button::v-deep #newKeyword {
        text-transform: lowercase;
    }

    .keyword-container {
        flex-wrap: wrap;
        margin: -12px;

        .keyword-tag {
            display: flex;
            align-items: center;
            margin: 12px;
            padding: 2px 12px 2px 6px;
            border: 1px solid $nhsuk-grey-light;
            border-radius: 6px;
            height: 40px;            
            background-color: $nhsuk-white;
            white-space: nowrap;

            i {
                font-size: 12px;
                color: $nhsuk-red;
                vertical-align: middle;
            }
        }
    }

    .key-words-editor-component .footer-text {
        color: $nhsuk-grey;
        font-size: 1.6rem;
        margin-top: .5rem;
    }
</style>