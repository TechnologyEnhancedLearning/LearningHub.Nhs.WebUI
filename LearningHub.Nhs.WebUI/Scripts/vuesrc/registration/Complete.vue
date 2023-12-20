<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <div class="whiteBGinner">
                    <h1 class="nhsuk-heading-l">You have created an account</h1>

                    <p>
                        You will receive an email asking you to create a password for your account. You can then sign in to the Learning Hub.
                    </p>
                    <p>
                        If you do not receive an email, please contact our <a :href="supportFormUrl" target="_blank">support team</a>.
                    </p>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        components: {
        },
        props: {
            supportFormUrl: String
        },
    })

</script>
