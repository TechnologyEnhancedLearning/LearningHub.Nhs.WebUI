<template>
    <Modal @cancel="$emit('cancel')">
        <template v-slot:title>
            <div>
                Copy Content
            </div>
        </template>
        <template v-slot:body>
            <b>You have chosen to copy {{ blocksToBeCopied }} content block{{ blocksToBeCopied > 1 ? 's' : '' }}. Where would you like to save {{ blocksToBeCopied > 1 ? 'them' : 'it' }}?</b>
        </template>
        <template v-slot:buttons>
                <Button v-on:click="newDraft = true"
                        :disabled="copyingContent"
                        class="mx-12 flex-grow-0"
                        color="blue">
                    New draft
                </Button>
                <DropdownMenu :enableDropdown="chooseAssessmentType" @cancel="chooseAssessmentType = false" class="assessment-type-dropdown">
                    <DropdownMenuItem @click="copyToNewFormalAssessment">
                        <i :class="getResourceTypeIconClass()"></i>&nbsp;Formal
                    </DropdownMenuItem>
                    <DropdownMenuItem @click="copyToNewInformalAssessment">
                        <i :class="getResourceTypeIconClass()"></i>&nbsp;Informal
                    </DropdownMenuItem>
                </DropdownMenu>
                <Button v-on:click="copyToExistingDraft"
                        :disabled="copyingContent"
                        class="mx-12 flex-grow-0"
                        color="blue">
                    Existing {{ resourceTypeDisplayName }} draft
                </Button>
                <DropdownMenu :enableDropdown="enableDropdown" @cancel="enableDropdown = false" class="destination-resource-dropdown">
                    <DropdownMenuItem v-for="(draft, index) in drafts" :key="index" @click="copyToDraft(draft.resourceId)">
                        <i :class="getResourceTypeIconClass()"></i>&nbsp;{{ draft.title }}
                    </DropdownMenuItem>
                </DropdownMenu>
        </template>
    </Modal>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Modal from '../globalcomponents/Modal.vue';
    import Button from '../globalcomponents/Button.vue';
    import { ResourceType, VersionStatus } from "../constants";
    import { getDisplayNameForResourceType } from "../resource/helpers/resourceHelper";
    import DropdownMenuItem from "../globalcomponents/DropdownMenuItem.vue";
    import DropdownMenu from "../globalcomponents/DropdownMenu.vue";
    import { commonlib } from '../common';
    import { AssessmentTypeEnum } from "../models/contribute-resource/blocks/assessments/assessmentTypeEnum";
    import ContributeChooseAssessmentType from "./ContributeChooseAssessmentType.vue";
    import { resourceData } from "../data/resource";
    
    export default Vue.extend({
        components: {
            Modal,
            Button,
            DropdownMenuItem,
            DropdownMenu,
            ContributeChooseAssessmentType
        },
        async created() {
            await this.loadDrafts();
        },
        data() {
            return {
                enableDropdown: false,
                newDraft: false,
                chooseAssessmentType: false,
                drafts: undefined,
                copyingContent: false,
            }
        },
        props: {
            blocksToBeCopied: Number,
            resourceType: { type: Number } as PropOptions<ResourceType>,
        },
        methods: {
            getResourceTypeIconClass(): any {
                return commonlib.getResourceTypeIconClass(this.resourceType);
            },
            copyToNewFormalAssessment() {
                this.copyToNewAssessment(AssessmentTypeEnum.Formal);
            },
            copyToNewInformalAssessment() {
                this.copyToNewAssessment(AssessmentTypeEnum.Informal);
            },
            copyToNewAssessment(type: AssessmentTypeEnum) {
                this.chooseAssessmentType = false;
                this.copyingContent = true;
                this.$emit('duplicate', type);
            },
            copyToDraft(draftId: number) {
                this.copyingContent = true;
                this.enableDropdown = false;
                this.$emit('duplicate', undefined, draftId);
            },
            async copyToExistingDraft() {
                if (!this.drafts) {
                    await this.loadDrafts();
                }
                this.enableDropdown = true;
            },
            async loadDrafts() {
                this.drafts = await resourceData.getMyContributions(this.resourceType, VersionStatus.DRAFT);
            }
        },
        watch: {
            newDraft() {
                if (this.newDraft) {
                    if (this.resourceType === ResourceType.ASSESSMENT) {
                        this.chooseAssessmentType = true;
                        this.newDraft = false;
                    } else {
                        this.copyingContent = true;
                        this.$emit('duplicate');
                    }
                }
            }
        },
        computed: {
            resourceTypeDisplayName(): string {
                return getDisplayNameForResourceType(this.resourceType);
            },
        }
    });
</script>
<style lang="scss" scoped>
    .modal-component-buttons {
        display: block;
    }
    
    .dropdown-content-box {
        max-height: 200px;
        overflow-y: scroll;
    }

    .destination-resource-dropdown {
        transform: unset !important;
        right: 64px;
        bottom: -20px;
    }

    .assessment-type-dropdown {
        transform: unset !important;
        left: 64px;
        bottom: 10px;
    }
</style>