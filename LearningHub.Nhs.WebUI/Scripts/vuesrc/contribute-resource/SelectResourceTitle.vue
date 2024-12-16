<template>
    <div class="title_input_component form-group py-5">
        <div class="lh-padding-fluid">
            <div class="lh-container-xl">
                <CharacterCount v-model="resourceDetails.title"
                                v-bind:characterLimit="255"
                                v-bind:hasOtherError="isTitleRequiredAndMissing"
                                v-bind:inputId ="resourceTitle">
                    <template v-slot:title class="nhsuk-heading-l">Add a title</template>
                    <template v-slot:description><label :for="resourceTitle">Give your resource a concise, useful title that will make sense to learners.</label></template>
                    <template v-slot:otherErrorMessage v-if="isTitleRequiredAndMissing">You must enter a title</template>
                </CharacterCount>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';
    import CharacterCount from '../globalcomponents/CharacterCount.vue';

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
            isTypeSelected: Boolean,
            resourceTitle: { type: String, default: 'resourceTitle' } 
        },
        components: {
            CharacterCount
        },
        computed: {
            isTitleRequiredAndMissing(): boolean {
                return this.isTypeSelected && this.resourceDetails.title.trim().length === 0;
            },          
        }
    });
</script>


<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .title_input_component {
        background-color: $nhsuk-grey-white;
    }
</style>