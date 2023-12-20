<template>
    <div class="contribute-header fullwidth">
        <div v-if="showWarning" class="row warning-banner px-xl-0 mx-xl-0">
            <div class="warning-banner-left">
                <div class="warningTriangle">
                    <i class="fa-solid fa-triangle-exclamation"></i>
                </div>
                <div class="info">
                    The alert triangle is shown where information is required. You need to enter this information in order to publish a resource.
                </div>
            </div>
            <div class="dismiss">
                <button class="btn btn-link nhsuk-u-font-size-32" @click="showWarning=false" aria-label="Dismiss warning"><i class="fa-solid fa-xmark"></i></button>
            </div>
        </div>
        <div class="limit-width px-xl-0 mx-xl-0">
            <div class="row mt-4">
                <div class="col-12">
                    <h1 class="nhsuk-heading-xl">{{mainTitle}}</h1>
                    <p class="draftText nhsuk-u-font-size-16" v-bind:class="{ 'error-message': saveError, 'd-none': (!saveStatus)}">
                        {{saveStatus}}
                    </p>
                </div>
            </div>
            <hr class="nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0" />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        name: 'contributeHeaderComponent',
        //props: {
        //    previousVersionExists: Boolean,
        //    resourceLoading: Boolean
        //},
        data() {
            return {
                showWarning: true
            }
        },
        async created() {
        },
        computed: {
            saveStatus(): string {
                return this.$store.state.saveStatus;
            },
            saveError(): string {
                return this.$store.state.saveError;
            },
            mainTitle(): string {
                return this.$store.state.mainTitle;
            }
        },
        methods: {

        }
    })
</script>
