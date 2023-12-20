<!-- Use within ExpansionPanel -->
<template>
     <v-expansion-panel-content expand-icon="" v-model="isOpen">
            <template v-slot:header>
                <div class="d-flex align-items-center">
                    <div class="ml-32 d-flex icon-button">
                        <IconButton v-on:click="isOpen = !isOpen"
                                    :iconClasses="isOpen ? `fas fa-minus-circle blue` : `fas fa-plus-circle blue`"
                                    :ariaLabel="isOpen ? `hide content` : `reveal content`"></IconButton>
                    </div>
                    <span class="text">{{ header }}<Tick v-if="!hideCompletionStatus" :complete="isReady" class="pl-10"></Tick></span>
                </div>
            </template>

            <v-card class="border-top p-25">
                <slot></slot>
            </v-card>
        </v-expansion-panel-content>
</template>


<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuetify from "vuetify";
    import Tick from './Tick.vue';
    import IconButton from './IconButton.vue';
    Vue.use(Vuetify);

    export default Vue.extend({
        props: {
            isReady: Boolean,
            header: String,
            hideCompletionStatus: Boolean
        },
        components: {
            Tick,
            IconButton
        },
        data() {
            return {
                isOpen: false,
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .icon-button {
        flex-basis: 45px;
    }
    
    .text {
        flex: 90%;
    }
</style>