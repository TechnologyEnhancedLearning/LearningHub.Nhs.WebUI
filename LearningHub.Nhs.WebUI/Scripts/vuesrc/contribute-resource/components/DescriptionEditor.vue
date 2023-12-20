<template>
    <div class="resource-decription-editor">

        <h3 class="nhsuk-heading-m">
            About this resource
        </h3>
        <div class="my-10">
            Write a description that explains the resource and its benefits to learners.
        </div>
        <div class="my-3">
            <ckeditorwithhint @change="(desc) => descriptionForEditing = desc" :maxLength="1000" :initialValue="initialValue"></ckeditorwithhint>
        </div>
        <div class="footer-text nhsuk-u-font-size-16">
            You can enter a maximum of 1000 characters
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from "lodash";
    import { ContributeResourceDetailModel } from "../../models/contribute/contributeResourceModel";
    import CKEditorWithHint from '../../ckeditorwithhint.vue';
    
    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        components: {
            ckeditorwithhint: CKEditorWithHint,
        },
        data() {
            return {
                initialValue: this.resourceDetails.description,
                descriptionForEditing: this.resourceDetails.description,
            }
        },
        created() {
            this.updateModelWithNewContent = _.debounce(this.updateModelWithNewContentDebounced, 1 * 1000, { 'maxWait': 10 * 1000 });
        },
        watch: {
            descriptionForEditing: {
                handler(newVal: string) {
                    this.updateModelWithNewContent(newVal);
                }
            }
        },
        methods: {
            updateModelWithNewContent(newContent: string) {
                // Replaced by debounced function in created()
            },
            updateModelWithNewContentDebounced(newContent: string) {
                this.resourceDetails.description = newContent;
            }
        },
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .resource-decription-editor .footer-text {
        color: $nhsuk-grey;
        font-size: 1.6rem;
        margin-top: .5rem;
    }
</style>