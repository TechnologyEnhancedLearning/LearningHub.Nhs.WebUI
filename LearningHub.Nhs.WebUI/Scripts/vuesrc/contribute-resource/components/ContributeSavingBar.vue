<template>
    <div class="contribute-action-bar-saving"
        v-bind:class="{ 'invisible': savingState === ''}">
            <span v-if="savingState === 'saving'">
               Saving...
            </span>
            <span v-else-if="savingState === 'saved'">
                <i class="fa-solid fa-circle-check"></i>
                Saved
            </span>
    </div>
</template>

<script>
import Vue from "vue";

export default Vue.extend({
    props: {
        savingState: String,
    },
});
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;
    
    .contribute-action-bar-saving {
        color: $nhsuk-green;
        background-color: rgba($nhsuk-green, 0.1);
        font-size: 14px;
        border-radius: 5px;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 90px;
        height: 40px;
    
        @media screen and (min-width: 721px) {
            font-size: 16px;
        }
    }
</style>