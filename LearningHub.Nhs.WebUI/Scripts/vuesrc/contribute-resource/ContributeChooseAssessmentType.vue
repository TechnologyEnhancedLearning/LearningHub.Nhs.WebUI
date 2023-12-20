<template>
    <div class="lh-padding-fluid">
        <div class="lh-container-xl">
            <h2 id="title-label" class="nhsuk-heading-l pt-5">Is your Assessment formal or informal?</h2>
            <SelectAssessmentType :assessmentType="AssessmentTypeEnum.Informal" 
                                   :optionText="'These assessments are like ‘quizzes’. After the assessment is finished, readers can see their score and whether they passed or not, but they can also see which answers were correct and read any additional feedback you provide.'"
                                   :optionTitle="'Informal Assessment'"
                                   :optionIcon="'/images/resource-type-icons/case.svg'"
                                   v-on:selected="chosenAssessmentType"></SelectAssessmentType>
                                   
            <SelectAssessmentType :assessmentType="AssessmentTypeEnum.Formal" 
                                   :optionText="'These assessments are like exams. After the assessment is finished, readers will be told their score and whether they passed or not, but no more.'"
                                   :optionTitle="'Formal Assessment'"
                                   :optionIcon="'/images/resource-type-icons/genericfile.svg'"
                                   v-on:selected="chosenAssessmentType"></SelectAssessmentType>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import SelectAssessmentType from './SelectAssessmentType.vue';
    import { AssessmentTypeEnum } from '../models/contribute-resource/blocks/assessments/assessmentTypeEnum';

    export default Vue.extend({
        components: {
            SelectAssessmentType,
        },
        data() {
            return {
                AssessmentTypeEnum
            }
        },
        methods: {
            chosenAssessmentType(value: AssessmentTypeEnum) {
                this.$emit("updateType", value);
            }
        }
    });
</script>
