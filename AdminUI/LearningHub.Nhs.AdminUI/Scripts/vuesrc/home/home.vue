<template>
    <div id="home">
        <div class="text-center mt-10">
            <h1 class="display-4">Learning Hub Admin</h1>
        </div>
        <div class="text-center mt-2" v-if="firstName">
            <h1 class="nhsuk-heading-xl">Welcome {{ firstName }}</h1>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    export default Vue.extend({
        components: {

        },
        props: {            
            firstName: String
        },
        data() {
            return {               
            }
        }
    });
</script>