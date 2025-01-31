<template>
    <Modal v-on:cancel="$emit('closeExtraAttemptRequestModal')">
        <template v-slot:title>
            <WarningTriangle color="yellow"></WarningTriangle>
            Retake Assessment
        </template>
        <template v-slot:body>
            <h2 class="font-weight-bold mb-5">By selecting continue, you will be adding another attempt for this assessment.</h2>
            <p>Should you wish to proceed and extend the maximum number of attempts, you must provide your reason below.</p>
            <p>This additional attempt and reason will be recorded in your learning activity, and visible on any certificates and reports.</p>
        </template>
        <template v-slot:form>
            <h2 class="font-weight-bold mb-5">Reason for retaking assessment</h2>
            <div class="text-box p-0">
                <label class="nhsuk-u-visually-hidden" for="reason"></label>
                <textarea class="form-control" id="reason" placeholder="Add your reason..." rows="4" :maxlength="maxLength" v-model="reason"></textarea>
                <div class="characters-count">
                    You have {{ charactersRemaining }} characters remaining
                </div>
            </div>
        </template>
            <template v-slot:buttons>
                <div class="d-block w-100">
                    <Button v-on:click="$emit('closeExtraAttemptRequestModal')"
                            class="mx-12 my-2">
                        Cancel
                    </Button>
                    <Button color="blue"
                            :disabled="retrying || !reason.length"
                            v-on:click="$emit('requestExtraAttempt', reason);"
                            class="mx-12 my-2 float-right">
                        Continue
                    </Button>
                </div>
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
        },
        props: {
            retrying: Boolean,
            maxLength: Number
        },
        data() {
            return {
              isReady: false,
              reason: '',
              charactersRemaining: this.maxLength
            }
        },
        watch: {
            reason() {
                this.charactersRemaining = this.maxLength - this.reason?.length || 0;
            }
        }
    });
</script>
<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;
    
    .characters-count {
        color: $nhsuk-grey;
        font-size: 1.6rem;
        margin-top: .5rem;
    }

    .modal-component-buttons {
        display: block;
    }
</style>
