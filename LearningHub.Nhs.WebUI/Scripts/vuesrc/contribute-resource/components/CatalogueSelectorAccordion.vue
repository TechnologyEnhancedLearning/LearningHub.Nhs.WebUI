<template>
    <details>
        <summary class="mb-10">
            More information on catalogues
        </summary>
        <div class="catalogue-selector-accordion-content">
            <p>
                A catalogue is a curated set of resources that has its own web page.
            </p>
            <p>
                You can contribute a resource as an editor of a catalogue or in your own name.
                To contribute a resource in your own name, select Community contributions from the drop down menu.
            </p>
            <p>
                You can manage all resources that you have contributed in the My contributions area.
            </p>
        </div>
    </details>
</template>

<script lang="ts">
    import Vue from 'vue';
    
    export default Vue.extend({})
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;

    summary {
        outline: none;
        color: $nhsuk-blue;
        text-decoration: underline;
    }

    .catalogue-selector-accordion-content {
        padding: 0 25px;
    }
</style>