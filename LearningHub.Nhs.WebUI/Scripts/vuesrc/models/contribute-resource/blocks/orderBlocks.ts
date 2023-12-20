import { BlockCollectionModel } from "./blockCollectionModel";
import { BlockModel } from "./blockModel";
import { BlockTypeEnum } from "./blockTypeEnum";


export const sortAndReNumberBlocks = (blockCollection: BlockCollectionModel) => {
    const pages = blockCollection.getPages();
    const pageBreaks = blockCollection.blocks.filter(block => block.blockType === BlockTypeEnum.PageBreak);
    const newPages = pages.map((page, index) => {
        if (pages.length - 1 === index) {
            // Last page
            return sortPage(page);
        } else {
            // Not a last page
            return sortPage(page).concat(pageBreaks[index]);
        }
    }, []);

    // Flatten pages back into a block collection
    blockCollection.blocks = [].concat(...newPages);
    
    const questionBlocks = blockCollection.blocks.filter(block => (block.blockType === BlockTypeEnum.Question));
    questionBlocks.forEach((block, index) => {
            block.title = `Question ${index + 1}`;
    });
    blockCollection.blocks.forEach((blockEl, blockIndex) => blockEl.order = blockIndex);
}

const sortPage = (blocks: BlockModel[]) => {
    blocks.sort((blockA, blockB) => {
        if((blockA.blockType === BlockTypeEnum.Question) === (blockB.blockType === BlockTypeEnum.Question)) {
            return blockA.order - blockB.order;
        }
        else if (blockB.blockType === BlockTypeEnum.Question) {
            return -1;
        }
        else {
            return 1;
        }
    });
    return blocks;
}
