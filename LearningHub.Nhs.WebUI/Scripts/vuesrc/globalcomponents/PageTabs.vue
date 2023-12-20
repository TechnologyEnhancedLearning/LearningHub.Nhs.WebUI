<template>
    <div class="all-content">
        <div :class="['page-tabs', { 'lh-padding-fluid': navbarPadded }]">
            <div class="lh-container-xl">
                <div class="d-flex flex-wrap justify-content-start my-n2"
                     role="tablist">
                    <template v-for="tabIndex in tabIndexes">
                        <div role="tab"
                             v-bind:key="tabIndex"
                             v-bind:id="`tab_${tabIndex}`"
                             v-bind:aria-controls="`page_${tabIndex}`"
                             v-bind:aria-selected="(tabIndex === selectedTabIndex).toString()">
                            <button :key="`tab_${tabIndex}`"
                                    class="page-tabs-tab mr-16 my-2 py-15"
                                    v-bind:class="[(tabIndex === selectedTabIndex ? 'page-tabs-tab-is-selected' : 'page-tabs-tab-is-not-selected'),
                                                (spacerAfterTabs && spacerAfterTabs.includes(tabIndex)) ? 'page-tabs-spacer-after' : '']"
                                    v-on:click="selectedTabIndex = tabIndex">
                                <slot :name="`tab_${tabIndex}`"></slot>
                            </button>
                        </div>
                    </template>
                    <div role="tab" class="right-content" v-if="$slots['tab_right']">
                        <slot name="tab_right"></slot>
                    </div>
                </div>
            </div>
        </div>
        <template v-for="tabIndex in tabIndexes">
            <div :key="`page_${tabIndex}`"
                 v-show="tabIndex === selectedTabIndex"
                 class="tabs-component-page"
                 role="tabpanel"
                 v-bind:aria-labelledby="`tab_${tabIndex}`"
                 v-bind:id="`page_${tabIndex}`">
                <slot :name="`page_${tabIndex}`"></slot>
            </div>
        </template>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            spacerAfterTabs: Array,
            navbarPadded: { type: Boolean, default: false },
        },
        data() {
            return {
                selectedTabIndex: 0,
            };
        },
        computed: {
            tabIndexes(): number[] {
                const tabIndexes: number[] = [];
                let i = 0;
                while (this.$slots[`tab_${i}`] && this.$slots[`page_${i}`]) {
                    tabIndexes.push(i);
                    i++;
                }
                return tabIndexes;
            }
        }
    });
</script>
<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    
    .all-content {
        width: 100%;
    }
    
    .tabs-component-page {
        word-break: break-word;
    }

    .page-tabs {
        background-color: $nhsuk-white;
        border-bottom: 1px solid $nhsuk-grey-light;
    }

    .page-tabs button {
        background: none;
        color: inherit;
        border: none;
        font: inherit;
        cursor: pointer;
        outline: inherit;
    }
    .page-tabs button:focus {
        border-color: $govuk-focus-highlight-yellow;
        background-color: $govuk-focus-highlight-yellow;
        box-shadow: inset 0 -5px 0 $nhsuk-black;
    }
    .page-tabs button.page-tabs-tab-is-selected:focus::before {
        display: none;
    }

    .page-tabs .page-tabs-tab {
        position: relative;
    }
    .page-tabs .page-tabs-tab-is-selected::before,
    .page-tabs .page-tabs-tab:hover::before {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 5px;
        background-color: $nhsuk-blue;
    }

    .page-tabs .page-tabs-spacer-after {
        margin-right: 40px !important;
    }
    .page-tabs .page-tabs-spacer-after::after {
        content: '|';
        position: absolute;
        right: -30px;
        color: $nhsuk-grey-placeholder;
    }

    .page-tabs .right-content {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex: 1;
        text-align: right;
    }
</style>
