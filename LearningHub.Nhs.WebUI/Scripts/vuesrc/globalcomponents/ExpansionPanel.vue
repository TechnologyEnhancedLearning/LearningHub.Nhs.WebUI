<template>
  <div class="expansion-panel-parent">
        <v-expansion-panel class="my-0" v-model="expansionPanelValue">
            <slot></slot>
        </v-expansion-panel>
    </div>
</template>
    
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuetify from "vuetify";
    Vue.use(Vuetify);

    export default Vue.extend({
        props: {
            value: Number,
        },
        data() {
            return {
                expansionPanelValue: this.value,
            }
        },
        watch: {
            expansionPanelValue() {
                this.$emit("expansionPanelValueChanged", this.expansionPanelValue);
            },
            value() {
                this.expansionPanelValue = this.value;
            }
        }
    });
</script>

<!--V-expansion-panel CSS from Vuetify (https://cdn.jsdelivr.net/npm/vuetify/dist/vuetify.min.css)-->
<style lang="scss">
    .expansion-panel-parent {

        .theme--light.v-expansion-panel .v-expansion-panel__container {
            border-top: 1px solid rgba(0,0,0,0.12);
            background-color: #fff;
            color: rgba(0,0,0,0.87);
        }

        .theme--light.v-expansion-panel .v-expansion-panel__container--disabled {
            color: rgba(0,0,0,0.38);
        }

        .theme--light.v-expansion-panel--focusable .v-expansion-panel__container:focus {
            background-color: #eee;
        }

        .v-expansion-panel {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            list-style-type: none;
            padding: 0;
            text-align: left;
            width: 100%;
        }

        .v-expansion-panel__container {
            flex: 1 0 100%;
            max-width: 100%;
            outline: none;
            transition: 0.3s cubic-bezier(0.25, 0.8, 0.5, 1);
        }

        .v-expansion-panel__container:first-child {
            border-top: none !important;
        }

        .v-expansion-panel__container--disabled .v-expansion-panel__header {
            pointer-events: none;
        }

        .v-expansion-panel__header {
            display: flex;
            cursor: pointer;
            align-items: center;
            position: relative;
            padding: 6px 24px;
            min-height: 48px;
        }

        .v-expansion-panel__header > *:not(.v-expansion-panel__header__icon) {
            flex: 1 1 auto;
        }

        .v-expansion-panel__body {
            transition: 0.3s cubic-bezier(0.25, 0.8, 0.5, 1);
        }
    }
</style>