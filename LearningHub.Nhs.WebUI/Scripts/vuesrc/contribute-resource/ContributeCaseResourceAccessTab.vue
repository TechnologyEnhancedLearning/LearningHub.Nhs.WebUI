<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl py-15">
            <div class="contribute-access-tab">
                <div class="row">
                    <div class="col-12">
                        <h2 class="nhsuk-heading-l">Audience access</h2>
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
                        <input type="radio" class="nhsuk-radios__input" name="resourceAccessibilityEnum" v-model="resourceDetails.resourceAccessibilityEnum" v-bind:value="2"/>
                        <span class="radioButton"></span>
                    </label>
                </div>
                <div class="row">
                    <label class="checkContainer ml-3">
                        <span class="my-auto">Make this resource available only to the NHS and Social care workforce community.</span>
                        <input type="radio" class="nhsuk-radios__input" name="resourceAccessibilityEnum" v-model="resourceDetails.resourceAccessibilityEnum" v-bind:value="3"/>
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
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import * as _ from "lodash";
    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        components: {

        },
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>            
        },
        data() {
            return {               
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-access-tab {
        max-width: 825px;
    }

    ul li {
        font-size: 16px;
    }

    .checkContainer {
        padding-left: 28px;

        span {
            font-size: 16px;
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