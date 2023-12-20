import { ResourceType } from "../../constants";

export const getDisplayNameForResourceType = (resourceType: ResourceType) => {
    switch (resourceType) {
        case ResourceType.CASE:
            return "case";
        case ResourceType.ASSESSMENT:
            return "assessment";
        default:
            return "resource";
    }
}