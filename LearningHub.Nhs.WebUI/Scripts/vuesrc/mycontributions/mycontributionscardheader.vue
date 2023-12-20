<template>
    <div class="panel-card-container">

        <div tabindex="0" style="background-size: cover;" class="h-100 my-contribute-card contribute-resource-panel-card">
            
            <a class="h-100 d-flex flex-column text-decoration-none" v-bind:href="contributeResourceUrl">
                <div class="mt-5 mb-3"><h2 class="nhsuk-heading-xs mx-4 header text-center">Contribute a resource</h2></div>
                <div class="mx-4 small"> </div>
                <div class="circle d-flex align-self-center align-items-center text-center"><div class="plus mb-2 w-100">+</div></div>
                <div class="mt-auto mx-auto align-self-center w-100">
                    <div class="pb-5 text-center"></div>
                </div>
            </a>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue from 'vue';
    import { CatalogueBasicModel } from '../models/catalogueModel';

    export default Vue.extend({
        name: 'mycontributionscardheader',
        data() {
            return {
                expanded: false,
            };
        },
        computed: {
            contributeResourceUrl(): string {
                if (this.selectedCatalogue && this.selectedCatalogue.nodeId > 1) {
                    return '/contribute-resource/select-resource?catalogueId=' + this.selectedCatalogue.nodeId.toString();
                } else {
                    return '/contribute-resource/select-resource';
                }
            },
            selectedCatalogue(): CatalogueBasicModel {
                return this.$store.state.myContributionsState.selectedCatalogue;
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .header {
        color: $nhsuk-black;
        font-family: FrutigerLT55Roman;
    }

    .button-card {
        background-color: $nhsuk-blue; /* nhs blue */
        color: $nhsuk-white;
        font-size: 19px;
        text-align: center;
        border-radius: 6px;
        width: 250px;
        height: 48px;
        border: 0;
        font-family: FrutigerLT55Roman;
    }

    .heading-xs {
        font-size: 1.8rem;
    }

    .bottom {
        position: absolute;
        bottom: 5px;
    }

    .circle {
        background-color: $nhsuk-white;
        border: solid white;
        border-radius: 50%;
        height: 80px;
        width: 80px;
    }

    .plus {
        font-size: 50px;
        color: $nhsuk-blue;
        font-weight: bold;
    }
</style>