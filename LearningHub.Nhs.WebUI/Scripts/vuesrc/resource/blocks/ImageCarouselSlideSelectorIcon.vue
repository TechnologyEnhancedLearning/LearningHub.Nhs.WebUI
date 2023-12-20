<template>
    <div class="carousel-slide-selector-item">
        <button v-on:click="$emit('slideChanged'); justSelected = true"
                v-on:blur="justSelected = false"
                v-bind:class="{ 'carousel-slide-selector-item-just-selected': justSelected }">
            <picture v-if="imageResourceLink">
                <img v-bind:src="imageResourceLink" v-bind:alt="imageBlock.mediaBlock.image.altText" />
            </picture>
            <i v-else class="fas fa-ban disabled-icon"/>
            <span>{{ imageBlock.title }}</span>
        </button>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { BlockModel } from "../../models/contribute-resource/blocks/blockModel";
    
    export default Vue.extend({
        props: {
            imageBlock: { type: Object } as PropOptions<BlockModel>
        },
        data() {
            return {
                justSelected: false,
            }
        },
        computed: {
            imageResourceLink(): string {
                return this.imageBlock.mediaBlock.image.getFileModel()?.getDownloadResourceLink();
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    .carousel-slide-selector-item {
        list-style-type: none;
        margin: 0;
        padding: 10px;
        width: 220px;
        flex: none;

        button {
            width: 100%;
            height: 100%;
            min-height: 200px;
            background-color: transparent;
            border: 3px solid transparent;
            border-radius: 15px;
            display: flex;
            flex-direction: column;
            align-items: center;
            font-size: 16px;
            font-family: $font-stack-bold;
            color: $nhsuk-blue;
            text-decoration: underline;
            padding: 15px 5px 5px 5px;
            justify-content: center;

            picture {
                flex-shrink: 0;
                height: 150px;
                width: 150px;
                border: 1px solid $nhsuk-grey-placeholder;
                border-radius: 30%;
                background-color: $nhsuk-grey-lighter;
                overflow: hidden;
                display: flex;
                justify-content: center;
                align-items: center;

                img {
                    max-width: 150px;
                    max-height: 150px;
                }
            }

            span {
                margin-top: 10px;
                word-break: break-word;
            }

            &:focus {
                border-color: $nhsuk-white;
                background-color: $govuk-focus-highlight-yellow;
                border-bottom: 3px solid black;
                color: $nhsuk-black;
            }
        }

        &-selected {
            button {
                border-color: $nhsuk-green;

                &:focus {
                    border: 3px solid $nhsuk-green;
                    box-shadow: none;
                }
            }
        }

        .carousel-slide-selector-item-just-selected:focus {
            background-color: transparent;
            color: $nhsuk-blue;
        }
    }
    
</style>