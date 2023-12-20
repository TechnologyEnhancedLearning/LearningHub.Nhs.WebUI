<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute-resource-info">

                <h2 class="nhsuk-heading-l pt-15">
                    Resource info
                </h2>

                <DescriptionEditor v-bind:resource-details="resourceDetails" class="nhsuk-u-padding-top-5" />

                <SensitiveContentTickBox v-bind:resource-details="resourceDetails" class="nhsuk-u-padding-top-5" />

                <KeyWordsEditor v-bind:resource-details="resourceDetails" class="nhsuk-u-padding-top-5" />

            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import DescriptionEditor from "./components/DescriptionEditor.vue";
    import SensitiveContentTickBox from "./components/SensitiveContentTickBox.vue";
    import KeyWordsEditor from "./components/KeyWordsEditor.vue";
    import { KeywordModel } from "../models/contribute/keywordModel";

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        components: {
            DescriptionEditor,
            SensitiveContentTickBox,
            KeyWordsEditor,
        },
        methods: {
            resourceDescription(): string {
                return this.resourceDetails.description;
            },
            resourceKeywords(): KeywordModel[] {
                return this.resourceDetails.resourceKeywords;
            }
        },
    })
</script>

<style lang="scss" scoped>
    .contribute-resource-info {
        max-width: 825px;
    }
</style>