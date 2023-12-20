import {BlockCollectionModel} from '../contribute-resource/blocks/blockCollectionModel';
import {BlockModel} from '../contribute-resource/blocks/blockModel';
import {BlockTypeEnum} from '../contribute-resource/blocks/blockTypeEnum';
import {sortAndReNumberBlocks} from '../contribute-resource/blocks/orderBlocks';
import {TextBlockModel} from '../contribute-resource/blocks/textBlockModel';
import {QuestionBlockModel} from "../contribute-resource/blocks/questionBlockModel";

jest.mock('../../axiosWrapper.ts', () => jest.fn());

test('Text blocks are ordered correctly', () => {
    // arrange
    const textBlock1 = new BlockModel();
    textBlock1.blockType = BlockTypeEnum.Text;
    textBlock1.order = 1;
    textBlock1.textBlock = new TextBlockModel();
    const textBlock2 = new BlockModel();
    textBlock2.blockType = BlockTypeEnum.Text;
    textBlock2.order = 0;
    textBlock2.textBlock = new TextBlockModel();
    textBlock2.textBlock.content = "I have content";
    
    // act
    const blockCollection = new BlockCollectionModel();
    blockCollection.blocks=[textBlock1,textBlock2];
    sortAndReNumberBlocks(blockCollection);
    
    // assert
    expect(blockCollection.blocks[0].textBlock.content).toBe('I have content');
});

test('Text blocks come before question blocks', () => {
    // arrange
    const textBlock1 = new BlockModel();
    textBlock1.blockType = BlockTypeEnum.Text;
    textBlock1.order = 2;
    textBlock1.textBlock = new TextBlockModel();
    const textBlock2 = new BlockModel();
    textBlock2.blockType = BlockTypeEnum.Text;
    textBlock2.order = 1;
    textBlock2.textBlock = new TextBlockModel();
    
    const questionBlock = new BlockModel();
    questionBlock.blockType = BlockTypeEnum.Question;
    questionBlock.order = 0;
    questionBlock.questionBlock = new QuestionBlockModel();

    // act
    const blockCollection = new BlockCollectionModel();
    blockCollection.blocks=[questionBlock,textBlock1,textBlock2];
    sortAndReNumberBlocks(blockCollection);
    
    // assert
    expect(blockCollection.blocks[2].blockType).toBe(BlockTypeEnum.Question);
});

test('Reordering ignores pages', () => {
    // arrange
    const textBlock1 = new BlockModel();
    textBlock1.blockType = BlockTypeEnum.Text;
    textBlock1.order = 1;
    textBlock1.textBlock = new TextBlockModel();

    const textBlock2 = new BlockModel();
    textBlock2.blockType = BlockTypeEnum.Text;
    textBlock2.order = 3;
    textBlock2.textBlock = new TextBlockModel();

    const questionBlock = new BlockModel();
    questionBlock.blockType = BlockTypeEnum.Question;
    questionBlock.order = 0;
    questionBlock.questionBlock = new QuestionBlockModel();
    
    const pageBreak = new BlockModel();
    pageBreak.blockType = BlockTypeEnum.PageBreak;
    pageBreak.order = 2

    // act
    const blockCollection = new BlockCollectionModel();
    blockCollection.blocks=[questionBlock,textBlock1,pageBreak,textBlock2];
    sortAndReNumberBlocks(blockCollection);
    
    // assert
    expect(blockCollection.blocks[2].blockType).toBe(BlockTypeEnum.PageBreak);
    expect(blockCollection.blocks[1].blockType).toBe(BlockTypeEnum.Question);
});