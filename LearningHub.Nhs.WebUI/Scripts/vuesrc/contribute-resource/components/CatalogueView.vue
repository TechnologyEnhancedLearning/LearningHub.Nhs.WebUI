<template>
    <div class="p-24 d-flex align-items-center justify-content-between">
        <div class="d-flex align-items-center justify-content-between" v-if="resourceCatalogueId > 0">
            <picture v-if="!logoErrorOnLoad">
                <img class="logo"
                     v-bind:src="`/api/catalogue/download-image/${catalogue.badgeUrl}`"
                     v-bind:alt="`Logo for ${catalogue.name} Catalogue`"
                     @error="errorLoadingLogo">
            </picture>
            <div class="ml-15">{{catalogue.name}}</div>
        </div>
        <div v-else>
            <p>No catalogue available.</p>
        </div>
        <a href="#" class="change-text-button accessible-link" v-if="!selectionInProgress && allowChange" v-on:click.prevent="onChangeClick">Change</a>
    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { CatalogueModel } from "../../models/catalogueModel";
    export default Vue.extend({
        props: {
            catalogue: { type: Object } as PropOptions<CatalogueModel>,
            selectionInProgress: Boolean,
            allowChange: Boolean,
            resourceCatalogueId: {
                type: Number,
                required: false,
                default: null
            }
        },
        data() {
            return {
                logoErrorOnLoad: false,
            }
        },
        methods: {
            onChangeClick() {
                this.$emit('change-click');
            },
            errorLoadingLogo() {
                this.logoErrorOnLoad = true;
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    picture {
        img {
            object-fit: contain;
            max-width: 180px;
            max-height: 60px;
        }
    }
</style>
