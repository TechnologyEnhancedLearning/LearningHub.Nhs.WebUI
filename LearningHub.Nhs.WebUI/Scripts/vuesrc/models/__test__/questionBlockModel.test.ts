import {QuestionTypeEnum} from '../contribute-resource/blocks/questions/questionTypeEnum';
import {AnswerTypeEnum} from '../contribute-resource/blocks/questions/answerTypeEnum';
import {newQuestionBlock} from "./builders/questionBlockBuilder";

jest.mock('../../axiosWrapper.ts', () => jest.fn());

test('Single Choice Question is not ready with a single answer', () => {
    // arrange
    const question = newQuestionBlock().withAnswersOfStatus([AnswerTypeEnum.Best]).build();
    
    // act and assert
    expect(question.isSingleChoiceWithOneBestAnswer()).toBe(true);
    expect(question.twoAnswers()).toBe(false);
    expect(question.answerIsReadyToPublish()).toBe(false);
});

test('Single Choice Question is not ready with more than 5 answers', () => {
    // arrange
    const question = newQuestionBlock()
        .withAnswersOfStatus([
            AnswerTypeEnum.Best,
            AnswerTypeEnum.Reasonable,
            AnswerTypeEnum.Reasonable,
            AnswerTypeEnum.Reasonable,
            AnswerTypeEnum.Reasonable,
            AnswerTypeEnum.Reasonable
        ])
        .build();

    // act and assert
    expect(question.isSingleChoiceWithOneBestAnswer()).toBe(true);
    expect(question.twoAnswers()).toBe(true);
    expect(question.isFull()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(false);
});

test('Single Choice Question cannot have two best answers', () => {
    // arrange
    const question = newQuestionBlock()
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Best])
        .build();

    // act and assert
    expect(question.isSingleChoiceWithOneBestAnswer()).toBe(false);
    expect(question.twoAnswers()).toBe(true);
    expect(question.isFull()).toBe(false);
    expect(question.answerIsReadyToPublish()).toBe(false);
});

test('Single Choice Question can be published with all requirements correct', () => {
    // arrange
    const question = newQuestionBlock()
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();

    // act and assert
    expect(question.isSingleChoiceWithOneBestAnswer()).toBe(true);
    expect(question.twoAnswers()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(true);
    expect(question.isReadyToPublish()).toBe(true);
});

test('Multiple Choice Question can be published with all requirements correct', () => {
    // arrange
    const question = newQuestionBlock()
        .withQuestionType(QuestionTypeEnum.MultipleChoice)
        .withAnswersOfStatus([AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();

    // act and assert
    expect(question.isMultipleChoiceWithNoBestAnswers()).toBe(true);
    expect(question.twoAnswers()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(true);
    expect(question.isReadyToPublish()).toBe(true);
});

test('Question is not ready if content is not ready', () => {
    // arrange
    const question = newQuestionBlock()
        .withQuestionContent("")
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();

    // act and assert
    expect(question.twoAnswers()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(true);
    expect(question.isReadyToPublish()).toBe(false);
});

test('Question is not ready if feedback is not ready', () => {
    // arrange
    const question = newQuestionBlock()
        .withFeedback("")
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();

    // act and assert
    expect(question.twoAnswers()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(true);
    expect(question.isReadyToPublish()).toBe(false);
});

test('Question is not ready if allowReveal is not ready', () => {
    // arrange
    const question = newQuestionBlock()
        .withAllowReveal(undefined)
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();

    // act and assert
    expect(question.twoAnswers()).toBe(true);
    expect(question.answerIsReadyToPublish()).toBe(true);
    expect(question.isReadyToPublish()).toBe(false);
});

test('Multiple Choice Question is not ready if there is a best answer', () => {
    // arrange
    const question = newQuestionBlock()
        .withQuestionType(QuestionTypeEnum.MultipleChoice)
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable])
        .build();

    // act and assert
    expect(question.isMultipleChoiceWithNoBestAnswers()).toBe(false);
    expect(question.twoAnswers()).toBe(true);
    expect(question.isFull()).toBe(false);
    expect(question.answerIsReadyToPublish()).toBe(false);
});

test('Answers can be deleted', () => {
    // arrange
    const question = newQuestionBlock()
        .withAnswersOfStatus([AnswerTypeEnum.Best, AnswerTypeEnum.Reasonable, AnswerTypeEnum.Incorrect])
        .build();
    
    // act
    question.deleteAnswer(question.answers[1]);

    // assert
    expect(question.answers.length).toBe(2);
    expect(question.answers[0].status).toBe(AnswerTypeEnum.Best);
});
