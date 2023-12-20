<template>
    <div class="contribute-actions-bar-component lh-padding-fluid">
        <div class="lh-container-xl py-20">

            <div class="contribute-actions-bar-component-inner mx-n12 my-n2">
                <div class="d-flex flex-wrap justify-content-end">
                    <div class="ml-12 mr-10 my-0 contribute-action-bar-complete-these-items-to-enable-publishing d-flex align-items-center">
                        <i class="fas fa-adjust"></i>
                        <p class="ml-12 mb-0">
                            Complete these items to enable preview and publishing
                        </p>
                    </div>
                    <Button v-if="isCaseOrAssessmentResourceType" class="mx-12 my-2 contribute-actions-bar-wide-only" color="blue" v-on:click="presentPreview" :disabled="!allowPublish">Preview</Button>
                    <Button class="mx-12 my-2 contribute-actions-bar-wide-only" color="green" v-on:click="publishResource" v-bind:disabled="!allowPublish">Publish</button>
                    <div class="dropdown-wrapper">
                        <Button class="mx-12 my-2 contribute-actions-bar-wide-only" v-on:click="enableDropdown = true">More Options</Button>
                        <DropdownMenu :enableDropdown="enableDropdown" @cancel="enableDropdown = false">
                            <DropdownMenuItem @click="saveForLater">Save for later</DropdownMenuItem>
                            <DropdownMenuItem @click="confirmDiscardDraft">Delete this draft</DropdownMenuItem>
                            <DropdownMenuItem
                                @click="() => (isDuplicatingResource || isResourceDuplicationSuccess) ? null : createDuplicate()"
                                v-if="isCaseOrAssessmentResourceType">
                                <template v-if="!isDuplicatingResource && !isResourceDuplicationSuccess">
                                    Duplicate this resource
                                </template>
                                <Spinner v-else-if="isDuplicatingResource" />
                                <template v-else-if="isResourceDuplicationSuccess">
                                    <Tick :complete="true" /> Duplicated
                                </template>
                            </DropdownMenuItem>
                            <DropdownMenuItem @click="copyContent">Copy content</DropdownMenuItem>
                        </DropdownMenu>
                    </div>
                </div>

                <div class="d-flex flex-grow-1">
                    <div class="mr-10 my-0 contribute-action-bar-saving-wrapper">
                        <ContributeSavingBar v-bind:saving-state="savingState"/>
                    </div>
                </div>
            </div>
            <div v-if="errorMessage" v-html="errorMessage" class="error-message" />

            <Modal v-if="discardDraftModalOpen" v-on:cancel="discardDraftModalOpen = false">
                <template v-slot:title>
                    Delete this draft
                </template>
                <template v-slot:body>
                    This will no longer be available.
                </template>
                <template v-slot:buttons>
                    <Button v-on:click="discardDraftModalOpen = false"
                            class="mx-12 my-2">
                        Cancel
                    </Button>
                    <Button color="red"
                            v-on:click="discardDraft"
                            class="mx-12 my-2">
                        Continue
                    </Button>
                </template>
            </Modal>

        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    import Button from '../globalcomponents/Button.vue';
    import IconButton from '../globalcomponents/IconButton.vue';
    import Modal from '../globalcomponents/Modal.vue';
    import WarningTriangle from '../globalcomponents/WarningTriangle.vue';
    import Spinner from '../globalcomponents/Spinner.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import ContributeSavingBar from "./components/ContributeSavingBar.vue";
    import DropdownMenu from "../globalcomponents/DropdownMenu.vue";
    import DropdownMenuItem from "../globalcomponents/DropdownMenuItem.vue";

    export default Vue.extend({
        props: {
            savingState: String,
            allowPublish: Boolean,
            isCaseOrAssessmentResourceType: Boolean,
            isDuplicatingResource: Boolean,
            isResourceDuplicationSuccess: Boolean,
            errorMessage: String,
        },
        components: {
            ContributeSavingBar,
            DropdownMenuItem,
            DropdownMenu,
            Button,
            IconButton,
            Modal,
            WarningTriangle,
            Spinner,
            Tick,
        },
        data() {
            return {
                discardDraftModalOpen: false,
                enableDropdown: false
            };
        },
        methods: {
            confirmDiscardDraft() {
                this.discardDraftModalOpen = true;
                this.enableDropdown = false;
            },
            discardDraft() {
                this.discardDraftModalOpen = false;
                this.enableDropdown = false;
                this.$emit('discardDraft');
            },
            saveForLater() {
                this.$emit('saveForLater');
            },
            publishResource() {
                this.$emit('publishResource');
            },
            createDuplicate() {
                this.$emit('createDuplicate');
            },
            presentPreview() {
                this.$emit('presentPreview');
            },
            copyContent() {
                this.$emit('update:duplicatingBlocks', true);
                this.enableDropdown = false;
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    
    .dropdown-wrapper {
        position: relative;
    }

    .contribute-actions-bar-component {
        background-color: $nhsuk-white;
        border-bottom: 1px solid $nhsuk-grey-light;

        .contribute-actions-bar-component-inner {
            display: flex;
            flex-direction: column;
            justify-content: space-between;

            @media screen and (min-width: 525px) {
                flex-direction: row-reverse;
            }
        }

        .contribute-action-bar-complete-these-items-to-enable-publishing > i {
            color: $nhsuk-grey-placeholder;
        }

        .contribute-action-bar-complete-these-items-to-enable-publishing > p {
            font-size: 14px;
            line-height: 19px;

            @media screen and (min-width: 525px) {
                max-width: 145px;
            }

            @media screen and (min-width: 721px) {
                font-size: 16px;
                max-width: 165px;
            }
        }

        .contribute-action-bar-saving-wrapper {
            flex-grow: 1;
            display: flex;
            align-items: start;
            max-width: 220px;
            justify-content: flex-end;

            @media screen and (min-width: 525px) {
                justify-content: start;
            }
        }

        .contribute-actions-bar-wide-only {            

            @media screen and (max-width: 940px) {
                font-size: 16px;
                min-width: 75px;
                margin: auto 8px !important;
            }
        }

        .contribute-actions-bar-narrow-only {
            @media screen and (min-width: 721px) {
                display: none;
            }
        }

        .error-message {
            text-align: right;
            margin-top: 20px;
            color: $nhsuk-red !important;
        }
    }
</style>

<style lang="scss">
    @use '../../../Styles/abstracts/all' as *;

    .contribute-actions-bar-publish-icon-green i {
        color: $nhsuk-green !important;
    }
</style>
