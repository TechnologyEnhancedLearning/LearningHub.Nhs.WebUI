<template>
    <div>
        <contributeHeaderComponent></contributeHeaderComponent>
        <div class="row">
            <div class="form-group col-6">
                <h3>Legal Placeholder</h3>

            </div>
        </div>


    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import contributeHeaderComponent from './HeaderComponent.vue';

    export default Vue.extend({
        components: {
            'contributeHeaderComponent': contributeHeaderComponent
        },
        data() {
            return {

            };
        },
    })

</script>


