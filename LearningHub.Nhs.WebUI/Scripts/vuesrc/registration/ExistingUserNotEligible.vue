<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <div class="user-entry mx-0">
                <h1 class="nhsuk-heading-l">You already have an account</h1>
                <p>
                    Based on your email address we can see you already have a Health Education England elearning for Healthcare (HEE e-LfH) account,
                    but you're not able to access the Learning Hub yet.
                    You can continue accessing elearning resources on HEE’s e-LfH Hub.
                </p>

                <div class="mt-5 mb-3">
                    <div class="accordion" id="accordion">
                        <div class="pt-0 pb-5">
                            <div class="heading" id="headingOne">
                                <div class="mb-0">
                                    <a href="#" class="collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                        <div class="accordion-arrow">Why doesn’t my account have access to the Learning Hub?</div>
                                    </a>
                                </div>
                            </div>
                            <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordion">
                                <div class="content">
                                    <p>
                                        At the current stage of development the Learning Hub isn't available to everyone,
                                        however if you'd like to keep up to date with its progress please visit the <a href="https://telblog.hee.nhs.uk/">TEL blog</a>
                                        and follow <a href="https://twitter.com/HEE_TEL">@HEE_TEL</a> on Twitter.

                                        If you feel the information we have about your role is incorrect,
                                        please contact our <a href="">support desk</a> and we will be happy to help.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="d-flex flex-wrap-reverse">
                    <router-link to="/create-an-account" tag="button" class="btn btn-outline-custom mb-5 mr-5">
                        Previous
                    </router-link>

                    <button class="btn btn-custom mb-5" @click="launchElfhHub">
                        Sign in to the e-LfH Hub
                    </button>
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
            elfhHubUrl: String
        },
        methods: {
            launchElfhHub() {
                window.location.href = this.$props.elfhHubUrl;
            }
        }
    })

</script>
