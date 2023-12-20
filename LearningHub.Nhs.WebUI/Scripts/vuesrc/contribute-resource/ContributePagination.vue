<template>
    <div class="page-block lh-padding-fluid">
        <div class="d-flex text-center item-center align-items-center py-2 lh-container-xl">
            <div class="padding"/>
            <div class="small-padding d-flex justify-content-center">
                <IconButton :disabled="pageNumber <= 0"
                        v-on:click="previousPage"
                        iconClasses="fa-solid fa-chevron-left"
                        ></IconButton>
            </div>
            <div class="page-text">
                <Tick v-bind:complete="pageIsReady" class="mr-2"></Tick>
                Page {{ pageNumber + 1 }} of {{ pageCount }}
                </div>
            <span class="small-padding d-flex justify-content-center">
                <IconButton :disabled="pageNumber >= pageCount - 1"
                        v-on:click="nextPage"
                        iconClasses="fa-solid fa-chevron-right"
                        ></IconButton>
            </span>
            <span class="ml-5 page-text navigation-disabled-text">
                <span class="navigation-disabled-text" v-if="pageCount === 1">There is only one page so far so page navigation is disabled.</span>
            </span>
            <div class="page-text p-0 button-add-page">
                <Button v-on:click="newPage">+ Add a page</Button>    
            </div>
        </div>

    </div>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import IconButton from "../globalcomponents/IconButton.vue";
    import Button from "../globalcomponents/Button.vue";
    import { BlockModel } from  "../models/contribute-resource/blocks/blockModel";
    import Tick from "../globalcomponents/Tick.vue";

    export default Vue.extend({
        props: {
            pageNumber: Number,
            pageCount: Number,
            pageIsReady: Boolean,
        },

        components: {
            IconButton,
            Button,
            Tick,
        },
        
        methods: {
            newPage() {
                this.$emit("newPage");
            },

            nextPage() {
                this.$emit("nextPage");
            },

            previousPage() {
                this.$emit("previousPage");
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    .page-block {
        background-color: $nhsuk-white;
        border-bottom: 1px solid $nhsuk-grey-light;
    }

    .navigation-disabled-text {
        text-align: right;
        font-size: 12px;
        color: $nhsuk-grey;
    }

    .padding {
        flex: 30%
    }

    .small-padding {
        flex: 5%
    }
    
    .page-text {
        flex: 20%
    }

    .button-add-page {
        text-align: end;
        text-align: -webkit-right;
    }

</style>