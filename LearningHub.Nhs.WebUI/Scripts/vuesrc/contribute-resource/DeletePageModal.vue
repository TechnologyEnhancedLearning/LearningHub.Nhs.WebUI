<template>
    <Modal>
        <template v-slot:title>
            <WarningTriangle color="yellow" /> Delete this page
        </template>
        <template v-slot:body>
            Deleting this page will remove all the content on the page. This cannot be undone.
        </template>
        <template v-slot:buttons>
            <Button v-on:click="$emit('cancel')"
                    class="mx-12 my-2">
                Cancel
            </Button>
            <Button color="red"
                    v-on:click="$emit('confirm');"
                    class="mx-12 my-2">
                Delete
            </Button>
        </template>
    </Modal>
</template>

<script lang="ts">
import Vue from 'vue';
import Modal from '../globalcomponents/Modal.vue';
import Button from '../globalcomponents/Button.vue';
import WarningTriangle from '../globalcomponents/WarningTriangle.vue';

export default Vue.extend({
    components: {
        Modal,
        Button,
        WarningTriangle,
    }
});
</script>
