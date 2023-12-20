<template>
    <div class="new-annotation">
        <div>
            <strong>Place pin for location of the annotation</strong> 
        </div>
        <div class="new-annotation-buttons">
            <div class="new-annotation-discard">
                <Button v-on:click="discard"
                        size="small" class="nhsuk-u-font-size-16">
                    Discard
                </Button>
            </div>
            <div class="new-annotation-done">
                <Button v-on:click="done"
                        size="small" class="nhsuk-u-font-size-16"
                        color="green">
                    Done
                </Button>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Button from '../../globalcomponents/Button.vue';
    
    export default Vue.extend({
        name: "SlideViewerAddNewAnnotation",
        components: { Button },
        methods: {
            done() {
                this.$emit('newAnnotationDone')
            },
            discard() {
                this.$emit('newAnnotationDiscard')
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    
    .new-annotation {
        height: auto;
        width: 260px;
        margin-right: 10px;
        background-color: $nhsuk-white;
        border-radius: 5px;
        border: 2px solid $nhsuk-green;
        flex-direction: column;
        align-items: center;
        font-size: 16px;
        padding: 15px;
    
        .new-annotation-buttons {
            display: inline-flex;
            width: 100%;
            text-align: center;
            margin-top: 10px;
    
            .new-annotation-done {
                flex: 1;
                width: 50%;
                text-align: center;
            }
    
            .new-annotation-discard {
                flex: 1;
                width: 50%;
                text-align: center;
            }
        }
    }
</style>