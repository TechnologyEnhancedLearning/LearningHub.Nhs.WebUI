import { BlockModel } from '../contribute-resource/blocks/blockModel';
import { BlockTypeEnum } from '../contribute-resource/blocks/blockTypeEnum';
import { TextBlockModel } from '../contribute-resource/blocks/textBlockModel';
jest.mock('../../axiosWrapper.ts', () => jest.fn());

test('BlockModel initialization copy is immutable', () => {
    // arrange
    let block = new BlockModel();
    block.blockType = BlockTypeEnum.Text;
    block.textBlock = new TextBlockModel();
    block.textBlock.content = "This is the copy";

    // act
    let copyBlock = new BlockModel(block);
    block.textBlock.content = "This is the original";
    const copyBlockText = copyBlock.textBlock.content;

    // assert
    expect(copyBlockText).toBe("This is the copy");
});

test("BlockModel is not ready to publish if the component is empty", () => {
    // arrange and act
    let block = new BlockModel();
    block.blockType = BlockTypeEnum.Text;
    block.textBlock = new TextBlockModel();

    // assert
    expect(block.isReadyToPublish()).toBe(false);
})