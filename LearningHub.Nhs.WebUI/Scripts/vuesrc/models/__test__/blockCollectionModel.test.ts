import { BlockCollectionModel } from "../contribute-resource/blocks/blockCollectionModel";
import { BlockModel } from "../contribute-resource/blocks/blockModel";
import { BlockTypeEnum } from "../contribute-resource/blocks/blockTypeEnum";
jest.mock('../../axiosWrapper.ts', () => jest.fn());

test('Block Collection is immutable on copy', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.blocks[0].textBlock.content = 'This is the copy';

    // act
    const blockCollectionCopy = new BlockCollectionModel(blockCollection);
    blockCollection.blocks[0].textBlock.content = 'This is the original';

    // assert
    expect(blockCollectionCopy.blocks[0].textBlock.content).toBe('This is the copy');
});

test('blockCollection ensures question blocks remain below other blocks', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Question);
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Text);

    const secondBlock = blockCollection.blocks[1];
    const thirdBlock = blockCollection.blocks[2];
    const questionBlock = blockCollection.blocks[3];

    // act
    blockCollection.moveBlockDown(secondBlock);
    blockCollection.moveBlockDown(secondBlock);
    blockCollection.moveBlockUp(thirdBlock);

    // assert
    expect(blockCollection.blocks.indexOf(secondBlock)).toBe(2);
    expect(blockCollection.blocks.indexOf(thirdBlock)).toBe(0);
    expect(blockCollection.blocks.indexOf(questionBlock)).toBe(3);
});


test('Can get pages from blockCollection', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text, 0);
    blockCollection.addBlock(BlockTypeEnum.Text, 0);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 0);
    blockCollection.addBlock(BlockTypeEnum.Question, 1);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 2);

    // act
    const pages = blockCollection.getPages();
    
    // assert
    expect(pages[0].length).toBe(2);
    expect(pages[1].length).toBe(1);
    expect(pages[2].length).toBe(1);
    expect(pages[0][0].blockType).toBe(BlockTypeEnum.Text);
    expect(pages[0][1].blockType).toBe(BlockTypeEnum.Text);
    expect(pages[1][0].blockType).toBe(BlockTypeEnum.Question);
    expect(pages[2][0].blockType).toBe(BlockTypeEnum.Text);
});


test('Questions are sorted below other block types', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    
    // act
    blockCollection.addBlock(BlockTypeEnum.Media, 0);
    blockCollection.addBlock(BlockTypeEnum.Question, 0);
    blockCollection.addBlock(BlockTypeEnum.Text, 0);
    blockCollection.addBlock(BlockTypeEnum.Question, 0);
    blockCollection.addBlock(BlockTypeEnum.Text, 0);

    // assert
    expect(blockCollection.blocks[0].blockType).toBe(BlockTypeEnum.Media);
    expect(blockCollection.blocks[1].blockType).toBe(BlockTypeEnum.Text);
    expect(blockCollection.blocks[2].blockType).toBe(BlockTypeEnum.Text);
    expect(blockCollection.blocks[3].blockType).toBe(BlockTypeEnum.Question);
    expect(blockCollection.blocks[4].blockType).toBe(BlockTypeEnum.Question);
});

test('Block collection correctly checks whether or not a block is a part of it', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    const blockInsideCollection = blockCollection.blocks[0];
    const blockOutsideCollection = new BlockModel(blockCollection.blocks[0]);

    // act
    const resultInside = blockCollection.blockBelongsToThisCollection(blockInsideCollection);
    const resultOutside = blockCollection.blockBelongsToThisCollection(blockOutsideCollection);
    
    // assert
    expect(resultInside).toBe(true);
    expect(resultOutside).toBe(false);
});

test('Block collection correctly gets end index of page', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Question);
    blockCollection.addBlock(BlockTypeEnum.PageBreak);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 2);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);
    
    // act
    const endOfPage1Index = blockCollection.getEndIndexOfPage(0);
    const endOfPage2Index = blockCollection.getEndIndexOfPage(1);
    const endOfPage3Index = blockCollection.getEndIndexOfPage(2);
    
    // assert
    expect(endOfPage1Index).toBe(2);
    expect(endOfPage2Index).toBe(4);
    expect(endOfPage3Index).toBe(8);
});

test('Block collection can correctly delete middle pages', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Question);
    blockCollection.addBlock(BlockTypeEnum.PageBreak);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);

    // act
    blockCollection.deletePage(1);

    // assert
    expect(blockCollection.blocks.map(b => b.blockType)).toEqual([
        BlockTypeEnum.Text,
        BlockTypeEnum.Question,
        BlockTypeEnum.PageBreak,
        BlockTypeEnum.Question,
        BlockTypeEnum.Question,
    ]);
});

test('Block collection can correctly delete start pages', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Question);
    blockCollection.addBlock(BlockTypeEnum.PageBreak);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);

    // act
    blockCollection.deletePage(0);

    // assert
    expect(blockCollection.blocks.map(b => b.blockType)).toEqual([
        BlockTypeEnum.Text,
        BlockTypeEnum.Text,
        BlockTypeEnum.PageBreak,
        BlockTypeEnum.Question,
        BlockTypeEnum.Question,
    ]);
});

test('Block collection can correctly delete end pages', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.Question);
    blockCollection.addBlock(BlockTypeEnum.PageBreak);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 1);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);
    blockCollection.addBlock(BlockTypeEnum.Question, 2);

    // act
    blockCollection.deletePage(2);

    // assert
    expect(blockCollection.blocks.map(b => b.blockType)).toEqual([
        BlockTypeEnum.Text,
        BlockTypeEnum.Question,
        BlockTypeEnum.PageBreak,
        BlockTypeEnum.Text,
        BlockTypeEnum.Text,
    ]);
});

test('Block collection can correctly delete empty pages', () => {
    // arrange
    const blockCollection = new BlockCollectionModel();
    blockCollection.addBlock(BlockTypeEnum.Text);
    blockCollection.addBlock(BlockTypeEnum.PageBreak);
    blockCollection.addBlock(BlockTypeEnum.PageBreak, 1);
    blockCollection.addBlock(BlockTypeEnum.Text, 2);

    // act
    blockCollection.deletePage(1);

    // assert
    expect(blockCollection.blocks.map(b => b.blockType)).toEqual([
        BlockTypeEnum.Text,
        BlockTypeEnum.PageBreak,
        BlockTypeEnum.Text,
    ]);
});
