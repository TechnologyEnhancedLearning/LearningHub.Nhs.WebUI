import { resourceData } from '../../data/resource';
import type { CatalogueBasicModel } from '../../models/catalogueModel';
import type { ContributeResourceDetailModel } from '../../models/contribute/contributeResourceModel';

const findResourceCatalogue = (resource: ContributeResourceDetailModel, userCatalogues: CatalogueBasicModel[]): CatalogueBasicModel | undefined => {
    return userCatalogues.find(cat => cat.nodeId === resource.resourceCatalogueId);
}

const resourceHasValidCatalogue = (resource: ContributeResourceDetailModel, userCatalogues: CatalogueBasicModel[]): boolean => {
    return typeof findResourceCatalogue(resource, userCatalogues) !== 'undefined';
}

export const redirectToMyContributions = (base: string, resource: ContributeResourceDetailModel, userCatalogues: CatalogueBasicModel[]): void => {
    const catalogue = findResourceCatalogue(resource, userCatalogues);
    window.location.href = `/my-contributions/${base}/${catalogue?.url ?? ''}`;
}

export const redirectToMyContributionsNode = (base: string, resource: ContributeResourceDetailModel, userCatalogues: CatalogueBasicModel[]): void => {
    const catalogue = findResourceCatalogue(resource, userCatalogues);
    window.location.href = `/my-contributions/${base}/${catalogue.url}/${resource.nodeId}`;
}

export const publishResource = async (
    resource: ContributeResourceDetailModel,
    userCatalogues: CatalogueBasicModel[],
    notes = 'Published from draft.'
) => {
    if (resourceHasValidCatalogue(resource, userCatalogues)) {
        const success = await resourceData.publishResource(resource.resourceVersionId, notes);
        if (success) {

            if (resource.resourceCatalogueId > 1) { // IT1 redirect to "all content" if not working in Community Contributions
                redirectToMyContributionsNode('allcontent', resource, userCatalogues);
            }
            else {
                redirectToMyContributions('published', resource, userCatalogues);
            }

        } else {
            throw new Error('Failed to publish');
        }
    } else {
        throw new Error('Invalid catalogue selected');
    }
}
