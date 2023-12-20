<template>
    <div class="page-separator">
        <div class="d-flex justify-content-between my-2">
            <Button :disabled="currentPage <= 0" class="justify-content-start" v-on:click="$emit('goToPage', currentPage - 1)">Previous Page</Button>    


            <span class="justify-content-center align-self-center">
                Page {{ currentPage + 1 }} of {{ getPageCount() }}
            </span>
            
            <Button :disabled="currentPage >= getPageCount() - 1 || !canContinue" class="justify-content-end" v-on:click="$emit('goToPage', currentPage + 1)">Next Page</Button> 
        
        </div>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Button from "../globalcomponents/Button.vue";
    import { ResourceInjection } from "./interfaces/injections";
    import { ResourceType } from "../constants";


    export default (Vue as Vue.VueConstructor<Vue & ResourceInjection>).extend({
        inject: ["resourceType"],
        components: {
            Button,
        },
        props: {
            currentPage: Number,
            pageCount: Number,
            canContinue: Boolean,
        },
        methods: {
            getPageCount() {
                return this.resourceType === ResourceType.ASSESSMENT ? this.pageCount + 1 : this.pageCount;
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .page-separator {
        background-color: $nhsuk-white;
        border-bottom: 1px solid $nhsuk-grey-light;
        border-top: 1px solid $nhsuk-grey-light;
        padding: 22px 0;
 
        button:disabled {
            background-color: $nhsuk-grey-light;
            color: $nhsuk-white;
        }
    }
</style>