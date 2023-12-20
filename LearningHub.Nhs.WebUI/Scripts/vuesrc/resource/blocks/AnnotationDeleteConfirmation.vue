<template>
    <div class="delete-confirmation">
        <div>
            <strong>Are you sure you want to delete the "{{ this.annotationTitle }}"
                    {{ imageZone ? "answer option" : "annotation" }}?</strong>
        </div>
        <div class="delete-annotation-buttons">
            <div class="delete-annotation-confirm">
                <Button @click="confirmDelete"
                        size="small"
                        color="red">
                    Delete
                </Button>
            </div>
            <div class="delete-annotation-cancel">
                <Button @click="cancelDelete"
                        size="small">
                    Cancel
                </Button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Button from '../../globalcomponents/Button.vue';

    export default Vue.extend({
        name: "AnnotationDeleteConfirmation",
        components: {
            Button
        },
        props: {
            annotationTitle: String,
            annotationIndex: Number,
            imageZone: Boolean,
        },
        methods: {
            confirmDelete() {
                this.$emit('confirmDelete', this.annotationIndex);
            },
            cancelDelete() {
                this.$emit('cancelDelete');
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .delete-confirmation {
        width: 300px;
        background-color: $nhsuk-white;
        border-radius: 5px;
        border: 1px solid $nhsuk-grey;
        flex-direction: column;
        align-items: center;
        font-size: 16px;
        padding: 15px;

        .delete-annotation-buttons {
            display: inline-flex;
            width: 80%;
            text-align: center;
            margin-top: 10px;

            .delete-annotation-confirm {
                flex: 1;
                width: 50%;
                text-align: center;
            }

            .delete-annotation-cancel {
                flex: 1;
                width: 50%;
                text-align: center;
            }
        }
    }
</style>