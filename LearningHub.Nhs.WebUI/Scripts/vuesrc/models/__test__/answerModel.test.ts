import {createAnswerModel} from "./builders/answerModelInProgress";
jest.mock('../../axiosWrapper.ts', () => jest.fn());

test("Answer with text block of less than 120 characters is ready to publish", () => {
    // arrange
    const answer = createAnswerModel()
        .withTextContent("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam justo enim, rutrum eu dictum vel, condimentum eget libero.1")
        .build();

    // act
    const result = answer.isReadyToPublish();
    
    // assert
    expect(result).toBe(true);
});

test("Answer with text block of more than 120 characters is not ready to publish", () => {
    // arrange and act
    const answer = createAnswerModel()
        .withTextContent("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam justo enim, rutrum eu dictum vel, condimentum eget libero. 1")
        .build();

    // act
    const result = answer.isReadyToPublish();

    // assert
    expect(result).toBe(false);
});
