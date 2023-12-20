<template>
    <div v-if="firstColumn.length"
         :id="matchGameId"
         class="match-game-view"
         :class="{
            'assessment-summary': !belongsToCase && showAnswers || showCorrectAnswers,
            'image-to-image': matchQuestionType === MatchGameSettings.ImageToImage,
            'image-to-text': matchQuestionType === MatchGameSettings.ImageToText}">
        <Modal v-if="fullscreen" @cancel="fullscreen = false">
            <template v-slot:body>
                <picture>
                    <img :src="href" loading="lazy">
                </picture>
            </template>
        </Modal>

        <div v-for="index in matchQuestionsState.length"
             :id="'row' + index"
             class=" d-flex flex-row flex-wrap mt-2"
             :class="{
                'mt-5': matchQuestionType === MatchGameSettings.ImageToImage && index !== 1,
                'mb-35': index === matchQuestionsState.length,
                'scrolling-up': scrollingUp,
                'sticky-row': isRowSelected(index),
                'middle-row': index !== matchQuestionsState.length
             }">
            <MatchGameOption :matchQuestionType="matchQuestionType"
                             :matchAnswer="firstColumn[index - 1].blockCollection.blocks[0]"
                             :isSelected="selectedFirstAnswerOrder === firstColumn[index - 1].order"
                             isFirstBlock
                             :isMatched="showCorrectAnswers || selectedAnswersProperty[firstColumn[index - 1].order] !== undefined"
                             @showFullScreen="showFullScreen"
                             @selectMatchAnswer="selectFirstAnswer(firstColumn[index - 1].order)"/>
            <MatchGameOption :matchQuestionType="matchQuestionType"
                             :matchAnswer="secondColumn[index - 1].blockCollection.blocks[1]"
                             :isSelected="selectedSecondAnswerOrder === secondColumn[index - 1].order"
                             :isMatched="showCorrectAnswers || selectedAnswersProperty.findIndex(p => p === secondColumn[index - 1].order) !== -1"
                             @showFullScreen="showFullScreen"
                             @selectMatchAnswer="selectSecondAnswer(secondColumn[index - 1].order)"/>

            <div v-if="displayUndoButtonForRow(index)"
                 class="col d-flex flex-column flex-wrap pl-0 undo-button-wrapper">
                <UndoButton @undo="undo(firstColumn[index - 1].order, index - 1)"/>
            </div>

            <div v-if="displayFeedbackIcon"
                 class="col d-flex flex-column flex-wrap pl-0 feedback-icon">
                <i class="fas mr-2"
                   :class="{
                      'fa-check-circle': isRowMatchedCorrectly(index),
                      'fa-times-circle': !isRowMatchedCorrectly(index)
                   }"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import { MatchGameSettings } from "../../models/contribute-resource/blocks/questions/matchGameSettings";
    import Modal from "../../globalcomponents/Modal.vue";
    import { MatchQuestionState } from "../../models/mylearning/matchQuestionState";
    import MatchGameOption from "./MatchGameOption.vue";
    import UndoButton from "../../globalcomponents/UndoButton.vue";
    
    export default Vue.extend({
        components: {
            Modal,
            MatchGameOption,
            UndoButton
        },
        props: {
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            selectedAnswersProperty: { type: Array, default: () => [] } as PropOptions<number[]>,
            matchQuestionType: { type: Number } as PropOptions<MatchGameSettings>,
            isSubmitted: { type: Boolean } as PropOptions<boolean>,
            showAnswers: { type: Boolean, default: false } as PropOptions<boolean>,
            completed: { type: Boolean, default: false } as PropOptions<boolean>,
            matchQuestionsState: { type: Array } as PropOptions<MatchQuestionState[]>,
            showCorrectAnswers: Boolean,
            belongsToCase: Boolean
        },
        data() {
            return {
                firstColumn: [] as AnswerModel[],
                secondColumn: [] as AnswerModel[],
                selectedFirstAnswerOrder: -1,
                selectedSecondAnswerOrder: -1,
                MatchGameSettings: MatchGameSettings,
                fullscreen: false,
                href: undefined,
                scrollTop: 0,
                scrollingUp: false,
                matchGameId: "match-game-" + this.matchQuestionsState[0].questionNumber
            }
        },
        async mounted() {
            this.populateColumns();
            Vue.nextTick(() => {
                const element = document.getElementById(this.matchGameId) as any;
                if (element) {
                    element.addEventListener('scroll', this.handleScroll);
                }
            });
        },
        watch: {
            selectedAnswersProperty: {
                deep: true,
                handler() {
                    this.populateColumns();
                    this.selectedSecondAnswerOrder = -1;
                    this.selectedFirstAnswerOrder = -1;
                    if (!this.completed) {
                        this.$emit("selectedAnswersUpdated");
                    }
                }
            }
        },
        computed: {
            displayFeedbackIcon(): boolean {
                return this.showAnswers && !this.showCorrectAnswers && !this.belongsToCase;
            }  
        },
        methods: {
            isRowSelected(index: number) {
                return this.selectedFirstAnswerOrder === this.firstColumn[index - 1].order ||
                    this.selectedSecondAnswerOrder === this.secondColumn[index - 1].order;
            },
            displayUndoButtonForRow(index: number) {
                return !this.showCorrectAnswers && !this.isSubmitted &&
                    this.selectedAnswersProperty[this.firstColumn[index - 1].order] !== undefined;
            },
            handleScroll(e: any) {
                if (this.scrollTop > e.target.scrollTop) {
                    this.scrollingUp = true;
                }
                this.scrollTop = e.target.scrollTop;
            },
            undo(firstAnswerOrder: number, index: number) {
                this.selectedAnswersProperty[firstAnswerOrder] = undefined;
                this.unmatchPair(index);
                this.$emit("selectedAnswersUpdated");
            },
            populateColumns() {
                const selectedSecondAnswers: AnswerModel[] = [];
                if (this.matchQuestionsState) {
                    const allAnswersPaired = this.selectedAnswersProperty.every(a => a !== undefined);
                    this.matchQuestionsState.forEach((row, i) => {
                        let firstAnswer = this.firstColumn?.[i];
                        if (firstAnswer === undefined) {
                            firstAnswer = this.answers.find(a => a.id === row.firstMatchAnswerId);
                            this.firstColumn.push(firstAnswer);
                        }
                        if (this.showCorrectAnswers) {
                            const correctAnswer = this.answers.find(a => a.order === firstAnswer.order);
                            this.secondColumn.push(correctAnswer);
                        } else if (!allAnswersPaired) {
                            this.secondColumn.push(this.answers.find(a => a.id === row.secondMatchAnswerId));
                        } else {
                            const selectedAnswer = this.answers.find(a => a.order === this.selectedAnswersProperty[firstAnswer.order]);
                            selectedSecondAnswers.push(selectedAnswer);
                        }
                    });
                    if (!this.showCorrectAnswers && allAnswersPaired) {
                        this.secondColumn = selectedSecondAnswers;
                    }
                }
            },
            selectFirstAnswer(order: number) {
                if (order === this.selectedFirstAnswerOrder) {
                    this.selectedFirstAnswerOrder = -1;
                } else if (this.selectedSecondAnswerOrder != -1) {
                    this.matchAnswers(order, this.selectedSecondAnswerOrder);
                } else {
                    this.selectedFirstAnswerOrder = order;
                }
            },
            selectSecondAnswer(order: number) {
                if (order === this.selectedSecondAnswerOrder) {
                    this.selectedSecondAnswerOrder = -1;
                } else if (this.selectedFirstAnswerOrder != -1) {
                    this.matchAnswers(this.selectedFirstAnswerOrder, order);
                } else {
                    this.selectedSecondAnswerOrder = order;
                }
            },
            isRowMatchedCorrectly(index: number) {
                const firstAnswerOrder = this.firstColumn[index - 1].order;
                return this.selectedAnswersProperty[firstAnswerOrder] === firstAnswerOrder;
            },
            unmatchPair(index: number) {
                if (!this.selectedAnswersProperty.every(a => a === undefined)) {
                    this.firstColumn.push(this.firstColumn.splice(index, 1)[0]);
                    this.secondColumn.push(this.secondColumn.splice(index, 1)[0]);
                } else {
                    this.firstColumn = [...this.firstColumn];
                }
            },
            matchAnswers(firstAnswerOrder: number, secondAnswerOrder: number) {
                this.selectedAnswersProperty[firstAnswerOrder] = secondAnswerOrder;

                const answersAnswered = this.selectedAnswersProperty.filter(a => a !== undefined).length;
                this.firstColumn = this.getColumnReordered(this.firstColumn, firstAnswerOrder, answersAnswered);
                this.secondColumn = this.getColumnReordered(this.secondColumn, secondAnswerOrder, answersAnswered);

                this.selectedFirstAnswerOrder = -1;
                this.selectedSecondAnswerOrder = -1;

                this.$emit('selectedAnswersUpdated');

                (document.activeElement as any).blur();
                this.scrollToMatchedPair(answersAnswered);
            },
            getColumnReordered(column: AnswerModel[], answerOrder: number, answersAnswered: number) {
                const columnUpdated = [...column];
                const answerIndex = column.findIndex(a => a.order === answerOrder);
                const answer = column[answerIndex];
                columnUpdated[answerIndex] = column[answersAnswered - 1];
                columnUpdated[answersAnswered - 1] = answer;
                return columnUpdated;
            },
            scrollToMatchedPair(index: number) {
                const id = this.matchGameId;
                Vue.nextTick(function () {
                    const row = document.getElementById("row" + index);
                    const container = document.getElementById(id);

                    const offset = row.offsetTop - container.offsetTop;

                    container.scroll({ top: offset, behavior: "smooth" });
                });
            },
            showFullScreen(href: string) {
                this.fullscreen = true;
                this.href = href;
            }
    
        }
    });
</script>

<style lang="scss">
    @use '../../../../Styles/abstracts/all' as *;
    
    .match-game-view {
        .undo-button-wrapper { 
            max-width: 30px;
        }

        img {
            max-height: 60vh;
            max-width: 100%;
        }
    
        .sticky-row {
            position: sticky;
            top: 0;
            z-index: 200;
            background: $nhsuk-white;
        }

        .sticky-row.scrolling-up {
            bottom: 0;
        }
        
        .margin-row {
            margin-bottom: 30px;
        }
    
        .answer-block {
            border-radius: 4px;
            min-width: 100px;
            border: 1px solid $nhsuk-grey;
            background-color: $nhsuk-grey-white;
            margin: 20px;
            text-align: center;
        }
    
        .undo-button {
            margin-top: 7px;
            margin-left: -20px;
            cursor: pointer;
        }
    
        .first-column,
        .second-column {
            max-width: 405px;
        }

        .feedback-icon {
            max-width: 27px;
            justify-content: center;
            font-size: 27px;

            .fa-check-circle {
                color: $nhsuk-green;
            }
        }
    }
    
    .image-to-image {
        .undo-button {
            margin-top: 35px;
            margin-left: -25px;
        }
    
        .first-column {
            max-width: 267px;
        }
    
        .second-column {
            max-width: 267px;
            
            .matched-block {
                margin-left: -13%;
            }
        }
    }
    
    .image-to-text {
        .undo-button {
            margin-top: 4px;
            margin-left: -30px;
        }
    
        .first-column {
            max-width: 334px;
        }
    
        .second-column {
            max-width: 346px;
        }
        
        .match-game-answer-block {
            cursor: default;
        }
        
        .middle-row {
            margin-bottom: 30px;
        }
        
        .matched-block {
            .main-border,
            text {
                pointer-events: none;
                cursor: default;
            }
        }
        
        .feedback-icon {
            height: 40px;
        }
    }
    
    .assessment-summary.image-to-image .second-column .matched-block {
        margin-left: -11%;
    }
</style>