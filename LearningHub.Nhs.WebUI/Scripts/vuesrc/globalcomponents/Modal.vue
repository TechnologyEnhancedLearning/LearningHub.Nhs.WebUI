<!-- INSTRUCTIONS FOR USE: -->
<!--
    Option A: STRUCTURED MODAL
    Specify:
    - Title
    - Body
    - Buttons
    e.g.
    <Modal>
        <template v-slot:title>
            Title goes here
        </template>
        <template v-slot:body>
            Body goes here
        </template>
        <template v-slot:buttons>
            Buttons go here
        </template>
    </Modal>

    Option B: FREE-FORM MODAL
    Specify:
    - The content of the whole modal
    e.g.
    <Modal>
        Modal content goes here...
    </Modal>
-->

<template>
    <div class="modal-component"
         v-on:click="$emit('cancel')"
         ref="modalComponent"
         tabindex="-1"><!-- tabIndex="-1" is used to make the item focussable (but only by javascript) -->
        <div class="d-flex flex-column"
             v-on:click.stop><!-- This div is required for styling -->

            <IconButton v-if="$listeners.cancel"
                        v-on:click="$emit('cancel')"
                        iconClasses="fa-solid fa-xmark"
                        ariaLabel="Close"
                        class="modal-component-close-button"></IconButton>
            
            <slot>
                <h4 class="modal-component-title">
                    <slot name="title"></slot>
                </h4>
                <p class="modal-component-body">
                    <slot name="body"></slot>
                </p>
                <p class="modal-component-form">
                  <slot name="form"></slot>
                </p>
                <div class="modal-component-buttons mx-n12">
                    <slot name="buttons"></slot>
                </div>
            </slot>

        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    import IconButton from './IconButton.vue';

    export default Vue.extend({
        components: {
            IconButton,
        },
        mounted() {
            const modalComponentElement = this.$refs.modalComponent as any;
            modalComponentElement.focus();
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .modal-component {
        position: fixed;
        z-index: 9998;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, 0.5);
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .modal-component > div {
        position: relative;
        max-width: 944px;
        min-width: 450px;
        max-height: 90vh;
        background-color: $nhsuk-white;
        border-radius: 0.8rem;
        padding: 4rem 4rem 3rem 4rem;
        margin: 4rem;
    }

    .modal-component .modal-component-close-button {
        position: absolute;
        top: 10px;
        right: 10px;
        font-size: 30px;
    }

    .modal-component .modal-component-title {
        font-size: 3.2rem;
        font-weight: bold;
        width: 100%;
        text-align: center;
        margin-bottom: 15px;
    }

    .modal-component .modal-component-body {
        background-color: $nhsuk-grey-white;
        border-radius: .5rem;
        margin-bottom: 2rem;
        padding: 2.5rem;
        width: 100%;
        text-align: center;
    }

    .modal-component .modal-component-buttons {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
    }

    .modal-component .modal-component-buttons button {
        flex-grow: 1;
    }
</style>