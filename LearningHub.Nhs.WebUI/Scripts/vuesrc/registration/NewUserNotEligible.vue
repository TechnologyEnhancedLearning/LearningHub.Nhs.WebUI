<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <h1 class="nhsuk-heading-l">You cannot create an account yet</h1>
                <p>
                    Based on your work email address, you cannot create an account for the Learning Hub yet. This is a beta release of the platform and access will be widened in the future.
                </p>
                <p>
                    If you work in:
                    <ul>
                        <li>Social care</li>
                        <li>Dental practice or university dental school that provides NHS care</li>
                    </ul>
                    Contact our <a :href="supportFormUrl" target="_blank">support team</a> to create accounts for a group of staff.
                </p>

                <p class="mb-5">
                    <a :href="whoCanAccessUrl" target="_blank">Who can access the Learning Hub?</a>
                </p>

                <div class="d-flex flex-wrap-reverse">
                    <router-link to="/create-an-account" tag="button" class="btn btn-outline-custom mb-5 mr-5">
                        Previous
                    </router-link>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        //components: {
        //},
        props: {
            //elfhHubUrl: String,
            whoCanAccessUrl: String,
            supportFormUrl: String
        },
        methods: {
            //launchElfhHub() {
            //    window.location.href = this.$props.elfhHubUrl;
            //}
        }
    })

</script>
