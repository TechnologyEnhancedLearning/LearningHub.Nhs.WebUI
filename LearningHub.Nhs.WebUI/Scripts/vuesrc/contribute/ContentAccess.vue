<template>
    <div>
        <div class="row">
            <div class="col-12">
                <h2 class="nhsuk-heading-l">Audience access<i v-if="this.$v.localResourceDetail.resourceAccessibilityEnum.$invalid" class="warningTriangle fas fa-exclamation-triangle"></i></h2>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-12">
                Select a publishing option
            </div>
        </div>
        <div class="row">
            <label class="checkContainer ml-3">
                <span class="my-auto">Make this resource available to everyone within the Learning Hub community.</span>
                <input type="radio" name="resourceAccessibilityEnum" v-model="localResourceDetail.resourceAccessibilityEnum" v-bind:value="2" @click="setProperty($event.target.name, $event.target.value)" />
                <span class="radioButton"></span>
            </label>
        </div>
        <div class="row">
            <label class="checkContainer ml-3">
                <span class="my-auto">Make this resource available only to the NHS and Social care workforce community.</span>
                <input type="radio" name="resourceAccessibilityEnum" v-model="localResourceDetail.resourceAccessibilityEnum" v-bind:value="3" @click="setProperty($event.target.name, $event.target.value)" checked />
                <span class="radioButton"></span>
            </label>
        </div>
        <div class="row my-2">
            <div class="accordion col-12" id="accordion">
                <div class="pt-0 pb-4">
                    <div class="heading" id="accessibilityTypeInfo">
                        <div class="mb-0">
                            <a href="#" class="collapsed text-decoration-skip" style="color:#005EB8;" data-toggle="collapse" data-target="#collapseAccessibilityTypeInfo" aria-expanded="false" aria-controls="collapseAccessibilityTypeInfo">
                                <div class="accordion-arrow"></div>
                                <span class="pl-3">
                                    Who do you want this resource to be accessed by?
                                </span>
                            </a>
                        </div>
                    </div>
                    <div id="collapseAccessibilityTypeInfo" class="collapse" aria-labelledby="accessibilityTypeInfo" data-parent="#accordion">
                        <div class="col-12">
                            <div class="my-2">
                                <div class="my-2">
                                    <b>
                                        When publishing a resource it is important as a contributor to understand who you are giving access to.
                                    </b>
                                </div>
                                <div class="my-2">
                                    The Learning Hub has two user types
                                </div>
                                <div class="my-2 pl-2">
                                    <ul class="nhsuk-list nhsuk-list--bullet">
                                        <li>General user - can access learning resources made available to everyone.</li>
                                        <li>Full user - the NHS and Social care workforce can access learning resources, plus additional content marked as appropriate.</li>
                                    </ul>
                                </div>
                                <div>
                                    General and Full users can request access to restricted resources.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required, between } from "vuelidate/lib/validators";
    import * as _ from "lodash";

    import { ResourceAccessibility } from '../constants';
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        components: {

        },
        data() {
            return {
                localResourceDetail: null as ContributeResourceDetailModel,
            }
        },
        created() {
            this.localResourceDetail = _.cloneDeep(this.resourceDetail);
            this.setValidStatus();
        },
        computed: {
            resourceDetail(): ContributeResourceDetailModel {
                return this.$store.state.resourceDetail;
            },
            resourceVersionId(): number {
                return this.$store.state.resourceDetail.resourceVersionId;
            },
        },
        methods: {
            setProperty(field: string, value: string) {
                let storedValue: string = '';
                if (this.resourceDetail[field as keyof ContributeResourceDetailModel] != null) {
                    storedValue = this.resourceDetail[field as keyof ContributeResourceDetailModel].toString();
                    this.setValidStatus();
                }
                if (storedValue != value) {
                    this.$store.commit("saveResourceDetail", { field, value });
                }
            },
            setValidStatus() {
                this.$root.$emit('isvalid', !this.$v.localResourceDetail.resourceAccessibilityEnum.$invalid);
            }
        },
        watch: {
            resourceVersionId(value) {
                this.localResourceDetail = _.cloneDeep(this.resourceDetail);
            },
        },
        validations: {
            localResourceDetail: {
                resourceAccessibilityEnum: {
                    required, between: between(2, 3)
                }
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    ul li{
     font-size: 16px;
    }
    .checkContainer {
        padding-left: 28px;
        span{
                font-size:16px;
            }
    }

    .radioButton {
        height: 24px;
        width: 24px;
    }

        .radioButton:after {
            top: 4px;
            left: 4px;
            width: 12px;
            height: 12px;
        }    
</style>