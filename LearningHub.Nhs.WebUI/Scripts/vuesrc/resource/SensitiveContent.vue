<template>

    <div class="sensitive-content align-self-center">
        <div>
            <img src="/images/sensitive-content/sensitive-content.png" alt="sensitive content" />
        </div>
        <div class="content-title mt-3">
            Sensitive content
        </div>
        <div class="content-text mt-3">
            This resource contains sensitive content which some people may find offensive or disturbing.
        </div>
        <div class="mt-5">
            <button class="nhsuk-button" @click="acceptContent">Continue</button>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        components: {
        },
        methods: {
            acceptContent() {
                this.$emit('acceptcontent');
            }
        }
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .sensitive-content {
        background-image: url(/images/sensitive-content/sensitive-content-bg.png);
        max-width: 560px;
        height: 300px;
        color: $nhsuk-white;
        display: flex;
        flex-direction: column;
        align-items: center;
        padding-top: 3rem;
        margin-top: 3rem;

        .content-title {
            font-weight: bold;
        }

        .content-text {
            max-width: 420px;
            text-align: center;
            
            @media(max-width: 768px){
                max-width: 350px;
            }
        }
           
        .btn.btn-custom
        {
            background-color: $nhsuk-white !important;
            color: $nhsuk-blue !important;
            border: 1px solid $nhsuk-white !important;
        }
    }

</style>
