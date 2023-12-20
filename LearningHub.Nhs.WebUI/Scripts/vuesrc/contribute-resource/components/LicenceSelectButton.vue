<template>
    <div>
        <button type="button"
                class="btn btn-outline-custom"
                v-bind:class="{ 'visually-hidden': isCurrentlySelectedLicence() }"
                v-on:click="$emit('choose-licence', licence.id)"
                v-bind:disabled="isCurrentlySelectedLicence()" v-bind:aria-label="buttonAriaLabel()">
            Select
        </button>
        <i class="fa-solid fa-circle-check" v-if="isCurrentlySelectedLicence()"></i>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import { LicenceDetails } from "../../models/contribute-resource/licenceDetails";

    export default Vue.extend({
        props: {
            licence: { type: Object } as PropOptions<LicenceDetails>,
            selectedLicenceId: Number,
        },
        methods: {
            isCurrentlySelectedLicence(): boolean {
                return this.selectedLicenceId === this.licence.id;
            },
            buttonAriaLabel(): string {
                return (this.isCurrentlySelectedLicence() ? "Licence currently selected: " : "Select licence: ") + this.licence.name;
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    i.fa-check-circle {
        font-size: 4rem;
        vertical-align: middle;
        color: $nhsuk-green;
        margin: 5px;
    }
</style>