<template>
    <div class="contribute-add-content-block-component-option">
        <div class="contribute-add-content-block-component-option--icon">
            <img v-bind:src="imgSrc" class="flexible-image" />
        </div>
        <div class="contribute-add-content-block-component-option--description">
            <h5>{{blockTypeName}}</h5>
            <p>{{blockDescription}}</p>
            <div v-if="!contributeResourceAVFlag && blockTypeName === 'Media'">
                <div v-html="audioVideoUnavailableView"></div>
            </div>
        </div>
        <Button v-on:click="$emit('choose')">Select</Button>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { resourceData } from '../../../data/resource';

    import Button from "../../../globalcomponents/Button.vue";

    import { BlockTypeEnum } from "../../../models/contribute-resource/blocks/blockTypeEnum";

    export default Vue.extend({
        components: {
            Button,
        },
        props: {
            imgSrc: String,
            blockTypeName: String,
            blockDescription: String,
        },
        data() {
            return {
                contributeResourceAVFlag: true
            }
        },
        created() {
            this.getContributeResAVResourceFlag();
        },
        computed: {
            audioVideoUnavailableView(): string {
                return this.$store.state.getAVUnavailableView;
            },
        },
        methods: {
            getContributeResAVResourceFlag() {
                resourceData.getContributeAVResourceFlag().then(response => {
                    this.contributeResourceAVFlag = response;
                });
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../../Styles/abstracts/all' as *;

    .contribute-add-content-block-component-option {
        display: flex;
        align-items: center;
        border-bottom: 1px solid $nhsuk-grey-light;

        &:last-child {
            border-bottom: none;
        }

        h5 {
            font-size: 24px;
        }

        &--icon {
            display: flex;
            flex-shrink: 0;
            align-items: center;
            justify-content: center;
            width: 100px;
            height: 100px;
            margin: 8px 20px 12px 0;
            border-radius: 50%;
        }

        &--description {
            flex-grow: 1;
            margin: 12px 12px 12px 0;
        }

        @media(max-width: 560px) {
            .contribute-add-content-block-component-option--icon {
                display: none;
            }
        }

        @media(max-width: 800px) {
            .contribute-add-content-block-component-option--icon {
                width: 15.625%;
            }
        }
    }

    .flexible-image {
        width: 100%;
    }
</style>