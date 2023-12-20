<template>
    <div class="key-words-editor-component">

        <div class="my-2 input-with-button">
            <CharacterCount v-model="newKeyword"
                            inputId="newKeyword"
                            v-bind:characterLimit="50"
                            v-bind:hasOtherError="keywordError"
                            @input="keywordError=false">
                <template v-slot:title>
                    Keywords
                </template>
                <template v-slot:otherErrorMessage>
                    <span class="text-danger">This keyword has already been added.</span>
                </template>
                <template v-slot:description>
                    To help learners find this resource, type one or more relevant keywords separated by commas and click 'Add'.
                </template>
                <template v-slot:afterInput>
                    <Button class="ml-3" v-on:click="addKeyword">Add</Button>
                </template>
            </CharacterCount>
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
            }
        },
        computed: {
            resourceVersionId() {
                return this.resourceDetails.resourceVersionId;
            },
            newKeywordTrimmed(): string {
                return this.newKeyword?.trim().replace(/ +(?= )/g, '').toLowerCase();
            },
        },
        methods: {
            async addKeyword() {  
                if (this.newKeyword && this.newKeywordTrimmed.length > 0) {
                    let allTrimmedKeyword = this.newKeywordTrimmed.toLowerCase().split(',');
                    allTrimmedKeyword = allTrimmedKeyword.filter(e => String(e).trim());
                    if (!this.resourceDetails.resourceKeywords.find(_keyword => allTrimmedKeyword.includes(_keyword.keyword.toLowerCase()))) {
                        for (var i = 0; i < allTrimmedKeyword.length; i++) {
                            let item = allTrimmedKeyword[i];
                            if (item.length > 0) {
                                let newKeywordObj = new KeywordModel({
                                    keyword: item,
                                    resourceVersionId: this.resourceVersionId,
                                });
                                newKeywordObj = await resourceData.addKeyword(this.resourceVersionId, newKeywordObj);
                                if (newKeywordObj.id > 0) {
                                    this.resourceDetails.resourceKeywords.push(newKeywordObj);
                                    this.keywordError = false;
                                    this.newKeyword = '';
                                }
                                else {
                                    this.keywordError = true;
                                    break;
                                }   

                            }
                            else {
                                this.keywordError = true;
                            }
                        }

                    }

                    
                    else {
                        this.keywordError = true;
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