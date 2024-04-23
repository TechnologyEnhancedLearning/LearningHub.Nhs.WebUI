<template>
    <div class="select_resource_type_component d-flex flex-row py-4">
        <div class="select_resource_type_component_icon" v-bind:class="{ 'resrouce_type_selected': (isSelected) }">
            <img v-bind:src="`/images/resource-type-icons/${iconFileName}.svg`" />
        </div>
        <div class="mx-5">
            <h3 class="nhsuk-heading-l nhsuk-u-margin-bottom-2">{{title}}</h3>
            <div>{{description}}</div>
        </div>
        <div v-if="!isSelected" class="align-self-center">
            <Button v-on:click="onSelectClick">Select</Button>
        </div>
        <div v-if="isSelected" class="align-self-center select_resource_type_component_tick_wrapper">
            <Tick class="select_resource_type_component_tick" complete></Tick>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { commonlib } from '../common';
    import Button from '../globalcomponents/Button.vue';
    import Tick from '../globalcomponents/Tick.vue';
    import { ResourceType } from '../constants';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        components: {
            Button,
            Tick
        },
        props: {
            resourceType: { type: Number } as PropOptions<ResourceType>,
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
        },
        computed: {
            title(): String {
                return commonlib.getResourceTypeText(this.resourceType);
            },
            iconFileName(): string {
                return ResourceType[this.resourceType].toLowerCase();
            },
            description(): String {
                return this.getResourceTypeDescription(this.resourceType);
            },
            isSelected(): boolean {
                return this.resourceDetails.resourceType === this.resourceType;
            }
        },
        methods: {
            getResourceTypeDescription(resourceType: ResourceType): string {
                switch (resourceType) {
                    case ResourceType.ARTICLE:
                        return "If you want to publish text on a topic and attach associated files."
                    case ResourceType.AUDIO:
                        return "";
                    case ResourceType.EMBEDDED:
                        return "";
                    case ResourceType.EQUIPMENT:
                        return "";
                    case ResourceType.GENERICFILE:
                        return "If you have a file stored on your computer or storage device that you want to add, for example a Word document, image, video or audio file.";
                    case ResourceType.IMAGE:
                        return "";
                    case ResourceType.SCORM:
                        return "If you want to add an elearning package in a zip file that is in SCORM format, version 1.2.";
                    case ResourceType.VIDEO:
                        return "";
                    case ResourceType.WEBLINK:
                        return "If you have a link to a website that you want to share, for example https://www.nhs.uk";
                    case ResourceType.CASE:
                        return "If you want to create a page that has a variety of media such as text and whole slide images.";
                    case ResourceType.ASSESSMENT:
                        return "If you want to set questions to assess people informally (correct answers and feedback provided at the end), or formally (without feedback).";
                    case ResourceType.HTML:
                        return "If you want to add an HTML resource package in a zip file.";
                    default:
                        return "";
                }
            },
            onSelectClick() {
                this.resourceDetails.resourceType = this.resourceType;
            }
        }
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

    .select_resource_type_component_tick {
        font-size: 54px;
    }
</style>