<template>
    <div class="select_resource_type_component d-flex flex-row py-4">
        <div class="select_resource_type_component_icon">
            <img v-bind:src="optionIcon" />
        </div>
        <div class="mx-5">
            <h2 class="nhsuk-heading-l">{{ optionTitle }}</h2>
            <div>{{ optionText }}</div>
        </div>
        <div class="align-self-center">
            <Button v-on:click="$emit('selected', assessmentType)">Select</Button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from '../globalcomponents/Button.vue';
    import { AssessmentTypeEnum } from '../models/contribute-resource/blocks/assessments/assessmentTypeEnum';

    export default Vue.extend({
        components: {
            Button,
        },
        props: {
            assessmentType: {type: Number} as PropOptions<AssessmentTypeEnum>,
            optionTitle: String,
            optionText: String,
            optionIcon: String,
        },
    });
</script>

<style lang="scss">
    @use '../../../Styles/abstracts/all' as *;

    .select_resource_type_component {
        max-width: 756px;
        border-bottom: solid 1px $nhsuk-grey-light;
    }

    .select_resource_type_component_icon {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        min-width: 108px;
        height: 108px;
        border-radius: 50%;
        border: solid 5px transparent;
        color: $nhsuk-blue;
    }

    .select_resource_type_component_icon.resrouce_type_selected {
        border: solid 5px $nhsuk-green;
        color: $nhsuk-green;
    }

    .select_resource_type_component_tick_wrapper {
        padding: 0px 16.5px;
    }
</style>