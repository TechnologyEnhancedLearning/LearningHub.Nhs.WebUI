<!-- INSTRUCTIONS --
    
    <div class="dropdown-wrapper"> 
        // NOTE: The 'dropdown-wrapper' class should contain a position:relative attribute.
        
        <Button v-on:click="enableDropdown = true">More Options</button>
        
        <DropdownMenu :enableDropdown="controlVariable">
            <DropdownMenuItem @customEvent="saveForLater">Save for later</DropdownMenuItem>
            <DropdownMenuItem @customEvent="discardDraft">Discard this draft</DropdownMenuItem>
            <DropdownMenuItem @customEvent="duplicateDraft">Duplicate this draft</DropdownMenuItem>
        </DropdownMenu>
        
    </div>
-->

<template>
    <div v-if="enableDropdown" class="dropdown-content-box">
        <span class="d-flex header-box">
            <b>Options</b>
            <i aria-label="Close"
               @click="$emit('cancel')"
               class="fa-solid fa-xmark ml-auto my-auto cross-icon"/>
        </span>
        <slot/>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import IconButton from "./IconButton.vue";
    
    export default Vue.extend({
        components: {IconButton},
        props: {
            enableDropdown: {type: Boolean} as PropOptions<boolean>,
        }
    });
</script>

<style scoped lang="scss">
    @use '../../../Styles/abstracts/all' as *;

    .dropdown-content-box {
        position: absolute;
        border: 1px solid $nhsuk-grey;
        border-radius: 5px;
        background: $nhsuk-white;
        z-index: 1;
        color: black;
        display: flex;
        flex-direction: column;
        text-align: left;
        min-width: 250px;
        transform: translate(-50%, 2%);
    }
    
    .header-box {
      padding: 10px;
    }
    
    
    
    .dropdown-content-top {
        border-radius: 5px 5px 0px 0px;
    }
    
    
    .cross-icon {
        color: $nhsuk-grey;
        font-size: 30px;
    }
    
    .cross-icon:hover {
        color: $nhsuk-navbar-blue;
        cursor: pointer;
    }
</style>
