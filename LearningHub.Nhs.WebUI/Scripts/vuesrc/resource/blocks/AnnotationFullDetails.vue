<template>
<div class="annotation-details" v-if="annotation != null">
    <div class="annotation-details-title">
        <div class="annotation-details-title-left">
            <div class="annotation-details-pin-icon">
                 <span class="fa-stack">
                    <span class="fas fa-map-marker fa-stack-2x"></span>
                    <strong class="fa-stack-1x annotation-details-pin-number">
                      {{annotation.order + 1}}
                    </strong>
                </span>
            </div>
            <div class="annotation-details-title-label">
                <strong>   {{annotation.label}} </strong>
            </div>
        </div>
        <div class="annotation-details-title-right">
            <div class="annotation-details-title-edit" v-if="isEditable">
                <LinkTextAndIcon v-on:click="editDetails">Edit</LinkTextAndIcon>  <i class="fa-solid fa-pencil" v-on:click="editDetails"></i>
            </div>
        </div>
    </div>
    <div class = annotation-details-description>
        {{ annotation.description }} 
    </div>
</div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import LinkTextAndIcon from '../../globalcomponents/LinkTextAndIcon.vue';
    
    export default Vue.extend({
        name: "AnnotationFullDetails",
        components: {
            LinkTextAndIcon
        },
        props:{
            annotation: {type: Object},
            annotationIndex: Number,
            isEditable: Boolean,
        },
        methods:{
            editDetails(){
                this.$emit('editAnnotationDetails', this.annotationIndex);
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .annotation-details {
        width: 400px;
        margin-right: 10px;
        background-color: $nhsuk-white;
        border-radius: 5px;
        border: 1px solid $nhsuk-grey;
        display: flex;
        flex-direction: column;
        font-size: 16px;
        padding: 15px;
        flex-grow: 1.5;
    
        .annotation-details-title {
            display: flex;
            flex-direction: row;
            width: 100%;
            justify-content: space-between;
    
            .annotation-details-title-left {
                justify-content: flex-start;
                display: flex;
                flex-direction: row;
    
                .annotation-details-pin-icon {
                    width: 60px;
    
                    .fa-map-marker {
                        color: $nhsuk-blue;
                    }
    
                    .annotation-details-pin-number {
                        font-size: small;
                        margin-top: -3px;
                        color: $nhsuk-white;
                    }
                }
    
    
                .annotation-details-title-label {
                    margin-left: 5px;
                }
            ;
            }
    
            .annotation-details-title-right {
                justify-content: flex-end;
                display: flex;
                flex-direction: row;
                min-width: 50px;
    
                .annotation-ui-header-icons {
                    display: inline-flex;
                    text-align: center;
                    margin-left: 5px;
    
                }
            }
        }
    }
</style>