<template>
    <div class="row my-2">
        <div class="accordion col-12" id="cat-accordion">
            <div class="pt-0 pb-4">

                <div class="heading" id="accessibilityTypeInfo">
                    <div class="mb-0">
                        <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseAccessibilityTypeInfo" aria-expanded="false" aria-controls="collapseAccessibilityTypeInfo">
                            <div class="accordion-arrow"></div>
                            <span class="pl-3">
                                More information on catalogues
                            </span>
                        </a>
                    </div>
                </div>
                <div id="collapseAccessibilityTypeInfo" class="collapse" aria-labelledby="accessibilityTypeInfo" data-parent="#cat-accordion">
                    <div class="content col-12">
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
                </div>
            </div>
        </div>
    </div>
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