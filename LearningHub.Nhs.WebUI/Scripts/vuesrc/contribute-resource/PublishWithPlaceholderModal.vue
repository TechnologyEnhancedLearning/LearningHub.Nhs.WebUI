<template>
    <Modal v-on:cancel="$emit('closePlaceholderModal')">
        <template v-slot:title>
            <WarningTriangle color="yellow"></WarningTriangle> Publish with placeholders
        </template>
        <template v-slot:body>
            There are placeholders in this case. Placeholder messages are visible to those viewing the case.
        </template>
        <template v-slot:buttons>
            <Button v-on:click="$emit('closePlaceholderModal')"
                    class="mx-12 my-2">
                Cancel
            </Button>
            <Button color="green"
                    v-on:click="$emit('proceedWithPublishing');"
                    class="mx-12 my-2">
                Proceed with publishing
            </Button>
        </template>
    </Modal>
</template>


<script lang="ts">
    import Vue from 'vue';
    import Button from "../globalcomponents/Button.vue";
    import Modal from '../globalcomponents/Modal.vue';
    import WarningTriangle from '../globalcomponents/WarningTriangle.vue';


    export default Vue.extend({
        components: {
            Button,
            Modal,
            WarningTriangle,
        }
    });
</script>