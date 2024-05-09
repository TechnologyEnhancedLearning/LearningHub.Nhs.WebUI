<template>
    <div>
        <!--Node-->
        <div :id="'treenode' + item.nodeId" v-if="isNode && isVisible" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1" v-bind:class="{ 'moving-highlight': isMovingOrReferencingNode }">
                    <div class="pr-4" style="cursor:pointer" @click.prevent="toggle">
                        <i v-if="isOpen" class="fa-regular fa-folder-open fa-lg content-structure-folder" aria-hidden="true"></i>
                        <i v-if="!isOpen" class="fa-regular fa-folder fa-lg content-structure-folder" aria-hidden="true"></i>
                        <span class="node-name-text"><a href="#">{{ item.name }}</a></span>
                    </div>
                    <div v-if="editMode === EditModeEnum.Structure" class="ml-auto">
                        <div class="d-flex">
                            <div :class="{ 'has-resources-indicator' : item.hasResourcesInBranchInd, 'no-resources-indicator' : !item.hasResourcesInBranchInd }" />
                            <div class="dropdown options-dropdown">
                                <a class="dropdown-toggle no-wrap" href="#" role="button" id="dropdownNodeItems" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" @click="recomputeNodeOptions">
                                    options
                                </a>
                                <div class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownNodeItems">
                                    <a class="dropdown-item" v-if="canEditNode" @click="onEditFolder">Edit</a>
                                    <a class="dropdown-item" v-if="canMoveNodeUp" @click="onMoveNodeUp">Move up</a>
                                    <a class="dropdown-item" v-if="canMoveNodeDown" @click="onMoveNodeDown">Move down</a>
                                    <a class="dropdown-item" v-if="canMoveNode" @click="onInitiateMoveNode">Move</a>
                                    <a class="dropdown-item" @click="onInitiateReferenceNode">Create reference</a>
                                    <a class="dropdown-item" v-if="canDeleteNode" @click="onDeleteFolder">Delete</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div v-if="editMode === EditModeEnum.MoveNode" class="ml-auto">
                        <a v-if="canMoveHere" href="#" @click.prevent="onMoveNode">Move here</a>
                        <span v-if="editingTreeNode.nodeId === item.nodeId" class="mr-3">Move this folder or <a id="cancelMoveNode" href="#" @click.prevent="onCancelMoveNode">Cancel move</a></span>
                    </div>
                    <div v-if="editMode === EditModeEnum.ReferenceNode" class="ml-auto">
                        <a v-if="canReferenceHere" href="#" @click.prevent="onReferenceNode">Create reference here</a>
                        <span v-if="editingTreeNode.nodeId === item.nodeId" class="mr-3">Create a reference to this folder or <a id="cancelReferenceNode" href="#" @click.prevent="onCancelReferenceNode">Cancel create reference</a></span>
                    </div>
                    <div v-if="editMode === EditModeEnum.MoveResource" class="ml-auto">
                        <a v-if="editingTreeNode.nodeId != item.nodeId" href="#" @click.prevent="onMoveResource">Move here</a>
                    </div>
                </div>
            </div>
        </div>

        <div v-show="isOpen" v-if="isNode">
            <div v-if="item.inEdit && !this.isError" class="tree-item-container">
                <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                    <div class="treeview-node-inner d-flex">
                        <div class="treeview-node-inner-padding" style="cursor:pointer">
                            <i class="fa fa-plus-circle create-folder-circle" aria-hidden="true" @click="createFolder"></i>
                            <a @click.prevent="createFolder()" style="padding-left:5px" href="#">Create a folder</a>
                        </div>

                        <div v-if="editMode === EditModeEnum.MoveNode && item.depth === 0 " class="ml-auto my-auto">
                            <a v-if="canMoveHere" href="#" @click.prevent="onMoveNode">Move here</a>
                        </div>

                        <div v-if="editMode === EditModeEnum.MoveResource && item.depth === 0 " class="ml-auto my-auto">
                            <a v-if="canMoveResourceToRoot" href="#" @click.prevent="onMoveResource">Move here</a>
                        </div>
                    </div>
                </div>
            </div>
            
            <div v-if="!item.childrenLoaded && showSpinner" class="tree-item-container">
                <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                    <div class="treeview-node-inner d-flex">
                        <div class="treeview-node-inner-padding d-flex flex-row">
                            <div class="ml-2"><i class="fa fa-spinner fa-spin" /></div>
                            <div class="small ml-3">Contents are loading, please wait...</div>
                        </div>
                    </div>
                </div>
            </div>

            <tree-item class="tree-item-container"
                       v-for="(child, index) in orderedChildren"
                       :key="child.hierarchyEditDetailId"
                       :item="child"
                       :expandNodes="expandNodes"
                       @delete-folder="$emit('delete-folder', $event)">
            </tree-item>
        </div>

        <!--Resource-->
        <div v-if="!isNode" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1" v-bind:class="{ 'moving-highlight' : isMovingResource }">
                    <div>
                        <div class="small">
                            <a v-if="canNavigateToResourceInfo" :href="getResourceUrl(item.resourceVersionId)">{{item.name}}</a>
                            <span v-else>{{item.name}}</span>
                        </div>
                        <div class="resource-detail d-flex flex-row">
                            <div class="no-wrap"><i :class="commonlib.getResourceTypeIconClass(item.resourceTypeId, 0)"></i>&nbsp;{{ commonlib.getPrettifiedResourceTypeName(item.resourceTypeId, item.durationInMilliseconds) }}</div>
                            <div :class="resourceStatusCssClass()">&nbsp;- {{ resourceStatusText() }}</div>
                            <div v-if="item.resourceInEdit">&nbsp;- <span class="yellow">Editing...</span></div>
                        </div>
                    </div>
                    <div class="ml-auto">
                        <div v-if="showResourceOptions" class="d-flex">
                            <div class="dropdown options-dropdown">
                                <a class="dropdown-toggle no-wrap" href="#" role="button" id="dropdownResourceItems" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" @click="recomputeResourceOptions">
                                    options
                                </a>
                                <div class="dropdown-menu" aria-labelledby="dropdownNodeItems">
                                    <a class="dropdown-item" v-if="canMoveResourceUp" @click="onMoveResourceUp">Move up</a>
                                    <a class="dropdown-item" v-if="canMoveResourceDown" @click="onMoveResourceDown">Move down</a>
                                    <a class="dropdown-item" v-if="canMoveResource" @click="onInitiateMoveResource">Move</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div v-if="editMode === EditModeEnum.MoveResource && movingResource.resourceVersionId == item.resourceVersionId" class="my-auto ml-auto pr-2">
                        <span class="small">Move this resource or <a id="cancelMoveResource" class="red" href="#" @click.prevent="onCancelMoveResource">Cancel move</a></span>
                    </div>
                </div>
            </div>
        </div>

        <!--Error loading folder-->
        <div v-if="isNode && isOpen && isError" class="tree-item-container">
            <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                <div class="treeview-node-inner d-flex">
                    <div class="treeview-node-inner-padding d-flex flex-row">
                        <i class="fas fa-exclamation-triangle text-danger mr-3 mt-1" aria-hidden="true"></i>
                        <div class="small">The contents of this folder failed to load, <span class="hyperlink" @click="toggle();toggle()">try again</span></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">

    import Vue, { PropOptions } from 'vue';
    import { NodeContentAdminModel } from '../models/content-structure/NodeContentAdminModel';
    import { EditModeEnum } from '../models/content-structure/editModeEnum';
    import { contentStructureData } from '../data/contentStructure';
    import { resourceData } from '../data/resource';
    import { NodeType, VersionStatus, ResourceType } from '../constants';
    import { commonlib } from '../common';
    import * as _ from "lodash";

    export default Vue.extend({
        name: 'treeItem',
        components: {
        },
        props: {
            item: { Type: NodeContentAdminModel } as PropOptions<NodeContentAdminModel>,
            expandNodes: { Type: String, required: false, default: '' } as PropOptions<string>,
        },
        watch: { // triggered on create / cancel hierarchy edit
            item: function (newVal, oldVal) {
                console.log('treeItem.item changed: ', newVal, ' | was: ', oldVal);
                if (this.item.nodeTypeId === NodeType.Catalogue) {
                    this.isOpen = true;
                    this.$forceUpdate();
                }
            },
            updatedNode: function (newVal, oldVal) {
                if (newVal && newVal.nodeId == this.item.nodeId) {
                    Vue.set(this, "isVisible", this.item.showInTreeView);
                    Vue.set(this, "childNodeList", this.item.children);
                    this.$forceUpdate();
                }
            },
        },
        data: function () {
            return {
                isOpen: false,
                showSpinner: false,
                EditModeEnum: EditModeEnum,
                childNodeList: [] as NodeContentAdminModel[],
                isMoving: false,
                isVisible: false,
                isError: false,
                commonlib: commonlib,
                canMoveNodeDown: false,
                canMoveNodeUp: false,
                canDeleteNode: false,
                canEditNode: false,
                canMoveNode: false,
                canMoveResourceDown: false,
                canMoveResourceUp: false,
                canMoveResource: false,
            };
        },
        computed: {
            isNode: function (): boolean {
                return this.item.nodeTypeId > 0;
            },
            orderedChildren: function (): NodeContentAdminModel[] {
                return _.orderBy(this.childNodeList, ['nodeTypeId', 'displayOrder'], ['desc', 'asc'])
            },
            childrenLoaded: function (): boolean {
                return this.item.childrenLoaded;
            },
            editMode(): EditModeEnum {
                return this.$store.state.contentStructureState.editMode;
            },
            editingTreeNode(): NodeContentAdminModel {
                return this.$store.state.contentStructureState.editingTreeNode;
            },
            updatedNode(): NodeContentAdminModel {
                return this.$store.state.contentStructureState.updatedNode;
            },
            canMoveHere(): boolean {
                var underEditingTreeNode: boolean = false;
                if (this.item.depth > this.editingTreeNode.depth) {
                    var parent = this.item.parent;
                    while (parent.depth >= this.editingTreeNode.depth) {
                        if (parent.nodeId == this.editingTreeNode.nodeId) {
                            underEditingTreeNode = true;
                            break;
                        }
                        parent = parent.parent;
                    }
                }
                return this.editingTreeNode.nodeId != this.item.nodeId && this.editingTreeNode.parent.nodeId != this.item.nodeId && !underEditingTreeNode;
            },
            canReferenceHere(): boolean {
                var underEditingTreeNode: boolean = false;
                if (this.item.depth > this.editingTreeNode.depth) {
                    var parent = this.item.parent;
                    while (parent.depth >= this.editingTreeNode.depth) {
                        if (parent.nodeId == this.editingTreeNode.nodeId) {
                            underEditingTreeNode = true;
                            break;
                        }
                        parent = parent.parent;
                    }
                }
                return this.editingTreeNode.nodeId != this.item.nodeId && this.editingTreeNode.parent.nodeId != this.item.nodeId && !underEditingTreeNode;
            },
            isMovingOrReferencingNode: function (): boolean {
                return (this.editMode === EditModeEnum.MoveNode || this.editMode === EditModeEnum.ReferenceNode) && this.editingTreeNode && this.editingTreeNode.nodeId === this.item.nodeId;
            },
            canNavigateToResourceInfo: function (): boolean {
                return this.item.versionStatusId == VersionStatus.PUBLISHED && this.editMode === EditModeEnum.None;
            },
            readOnly(): boolean {
                return this.$store.state.contentStructureState.readOnly;
            },
            showResourceOptions(): boolean {
                return this.item.inEdit &&
                    this.item.versionStatusId != VersionStatus.SUBMITTED &&
                    this.item.versionStatusId != VersionStatus.PUBLISHING &&
                    this.editMode == EditModeEnum.Structure;
            },
            canMoveResourceToRoot(): boolean {
                return this.editMode === EditModeEnum.MoveResource && this.movingResource.parent.depth > 0 && this.item.depth === 0;
            },
            movingResource(): NodeContentAdminModel {
                return this.$store.state.contentStructureState.movingResource;
            },
            isMovingResource: function (): boolean {
                return (this.editMode === EditModeEnum.MoveResource) && this.movingResource && this.movingResource.resourceVersionId === this.item.resourceVersionId;
            },
            indentNegativeMargin(): number {
                if (this.item.depth > 0) {
                    return -32 * (this.item.depth - 1)
                }
                else return 0;
            },
            indentPostivePadding(): number {
                if (this.item.depth > 0) {
                    return 32 * (this.item.depth - 1)
                }
                else return 0;
            },
        },
        mounted() {
            Vue.set(this, "isVisible", this.item.showInTreeView);
            if (this.item.childrenLoaded) {
                Vue.set(this, "childNodeList", this.item.children);
            }
            if (this.item.nodeTypeId == NodeType.Catalogue) {
                this.toggle();
            }
            else {
                var expandNodeList = this.expandNodes.toString().split('\\').map(Number);

                if (expandNodeList.includes(this.item.nodeId)) {
                    this.toggle();
                }
            }
        },
        methods: {
            recomputeNodeOptions: function () {
                this.canMoveNodeUp = this.item.displayOrder > 1;
                this.canMoveNodeDown = this.item.parent && this.item.displayOrder < this.item.parent.children.filter(c => c.nodeTypeId > 0).length;
                this.canDeleteNode = !this.item.hasResourcesInBranchInd;
                this.canEditNode = true;
                this.canMoveNode = this.item.parent != null;
            },
            recomputeResourceOptions: function () {
                this.canMoveResourceUp = this.item.displayOrder > 1;
                this.canMoveResourceDown = this.item.parent && this.item.displayOrder < this.item.parent.children.filter(c => c.nodeTypeId === 0).length;
                this.canMoveResource = this.item.versionStatusId != VersionStatus.PUBLISHING;
            },
            onDeleteFolder(event: MouseEvent) {
                this.$emit('delete-folder', this.item)
            },
            toggle: function () {
                if (this.isNode && this.item.nodeId) {
                    // Automatically horizontally scroll the treeview if user opens a folder whose left edge is greater than 40% from the left.
                    if (!this.isOpen) {
                        var viewportWidth = $(window).width();
                        var treenode = $('#treenode' + this.item.nodeId);
                        if (treenode.position()) {
                            var treeNodeLeft = $('#treenode' + this.item.nodeId).position().left;
                            var autoScrollTriggerPercentage = 40;

                            if (treeNodeLeft / viewportWidth * 100 > autoScrollTriggerPercentage) {
                                var treeElement = $('.node-contents-treeview');
                                treeElement.animate({ scrollLeft: treeNodeLeft + treeElement.scrollLeft() - 25 });
                            }
                        }
                    }

                    // Load the data.
                    if (!this.isOpen && (!this.item.childrenLoaded || this.isError)) {
                        this.isOpen = true;
                        setTimeout(() => this.showSpinner = true, this.item.depth == 0 ? 0 : 300);
                        this.loadNodeContents().then(response => {
                            this.item.childrenLoaded = true;

                            Vue.set(this, "childNodeList", this.item.children);
                        });
                    }
                    else {
                        this.isOpen = !this.isOpen;
                    }
                }
            },
            createFolder: function () {
                this.$store.commit('contentStructureState/setCreatingFolder', { parentNode: this.item });
            },
            onEditFolder: function () {
                this.$store.commit('contentStructureState/setEditingFolder', { folderNode: this.item });
            },
            onMoveNodeUp: function () {
                this.$store.dispatch('contentStructureState/moveNodeUp', { node: this.item });
            },
            onMoveNodeDown: function () {
                this.$store.dispatch('contentStructureState/moveNodeDown', { node: this.item });
            },
            onInitiateMoveNode: function () {
                this.$store.commit('contentStructureState/setMovingNode', { node: this.item });
            },
            onInitiateReferenceNode: function () {
                this.$store.commit('contentStructureState/setReferencingNode', { node: this.item });
            },
            onMoveNode: function () {
                this.$store.dispatch('contentStructureState/moveNode', { destinationNode: this.item });
            },
            onReferenceNode: function () {
                this.$store.dispatch('contentStructureState/referenceNode', { destinationNode: this.item });
            },
            onCancelMoveNode: function () {
                this.$store.commit('contentStructureState/cancelMoveNode');
            },
            onCancelReferenceNode: function () {
                this.$store.commit('contentStructureState/cancelReferenceNode');
            },
            onMoveResourceUp: function () {
                this.$store.dispatch('contentStructureState/moveResourceUp', { node: this.item });
            },
            onMoveResourceDown: function () {
                this.$store.dispatch('contentStructureState/moveResourceDown', { node: this.item });
            },
            onInitiateMoveResource: function () {
                this.$store.commit('contentStructureState/setMovingResource', { node: this.item });
            },
            onMoveResource: function () {
                this.$store.dispatch('contentStructureState/moveResource', { destinationNode: this.item });
            },
            onCancelMoveResource: function () {
                this.$store.commit('contentStructureState/cancelMoveResource');
            },
            async loadNodeContents() {
                this.isError = false;
                await contentStructureData.getNodeContentsAdmin(this.item.nodePathId, this.readOnly && !this.item.inEdit).then(response => {

                    this.item.children = response;
                    this.item.children.forEach((child) => {
                        child.parent = this.item;
                        child.inEdit = this.item.inEdit;
                        child.showInTreeView = true;
                        child.depth = child.parent.depth + 1;
                        child.path = child.depth === 0 ? child.name : `${child.parent.path} > ${child.name}`;
                    });
                }).catch(e => {
                    console.log(e);
                    this.isError = true;
                });
            },
            getResourceUrl: function (resourceVersionId: number) {
                if (resourceVersionId) {
                    return `/Resource/Details/${resourceVersionId.toString()}`;
                }
                else {
                    return "";
                }
            },
            resourceStatusText: function (): string {
                switch (this.item.versionStatusId) {
                    case VersionStatus.DRAFT:
                        return "Draft";
                    case VersionStatus.PUBLISHED:
                        return "Published";
                    case VersionStatus.UNPUBLISHED:
                        if (this.item.unpublishedByAdmin) {
                            return "Unpublished by admin";
                        }
                        else {
                            return "Unpublished";
                        }
                    case VersionStatus.PUBLISHING:
                    case VersionStatus.SUBMITTED:
                        return "Publishing...";
                    case VersionStatus.FAILED:
                        return "Failed to publish";
                    default:
                        return "Unknown status";
                }
            },
            resourceStatusCssClass: function (): string {
                switch (this.item.versionStatusId) {
                    case VersionStatus.DRAFT:
                    case VersionStatus.PUBLISHED:
                    default:
                        return "";
                    case VersionStatus.UNPUBLISHED:
                        if (this.item.unpublishedByAdmin) {
                            return "red";
                        }
                        else {
                            return "yellow";
                        }
                    case VersionStatus.PUBLISHING:
                    case VersionStatus.SUBMITTED:
                        return "yellow";
                    case VersionStatus.FAILED:
                        return "red";
                }
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;

    /* The 'root-tree-item-container' element is defined in the content-structure.vue file, not here. */
    .root-tree-item-container {
        margin-left: -29px;
        min-width: fit-content;
    }

    .root-tree-item-container > .tree-item-container {
        margin-left: 0px;
    }

    .tree-item-container {
        /* "fit-content" is needed on mobile when there's lots levels of folder nesting */
        min-width: fit-content;
        margin-left: 29px;
        border-left: 2px solid $nhsuk-grey-light;
    }

    .treeview-node {
        font-size: 1.6rem !important;
        box-sizing: unset;
        border-top: 1px solid $nhsuk-grey-divider;
    }

    .treeview-node-inner:before {
        width: 0.9em;
        height: 1.3em;
        margin-right: 0.1em;
        vertical-align: top;
        border-bottom: 2px solid $nhsuk-grey-light;
        content: "";
        display: inline-block;
    }

    .treeview-node-inner-padding {
        padding-bottom: 1rem !important;
        padding-top: 1rem !important;
        padding-left: 0.3rem !important;
    }

    .tree-item-container:last-child {
        border-left: none;
    }

        .tree-item-container:last-child > .treeview-node > .treeview-node-inner:before {
            border-left: 2px solid $nhsuk-grey-light;
        }

    .resource-detail {
        font-size: 1.4rem !important;
        margin-top: 5px;
    }

    .node-name-text {
        margin-left: 6px;
    }

    .yellow {
        color: $nhsuk-warm-yellow;
    }

    .red {
        color: $nhsuk-red;
    }

    .moving-highlight {
        border-color: #ffc107;
        border-style: solid;
        border-width: 2px;
        background-color: rgba(255, 184, 28, 0.05);
    }

    .create-folder-circle {
        color: $nhsuk-green;
        font-size: 2rem;
    }

    .has-resources-indicator {
        background: $nhsuk-green;
        border: 2px solid $nhsuk-grey-placeholder;
        box-sizing: border-box;
        border-radius: 4px;
        width: 30px;
        height: 15px;
        margin-right: 26px;
        margin-top: 8px;
    }

    .no-resources-indicator {
        border: 2px solid $nhsuk-grey-placeholder;
        box-sizing: border-box;
        border-radius: 4px;
        width: 30px;
        height: 15px;
        margin-right: 26px;
        margin-top: 8px;
    }

    div.options-dropdown {
        color: $nhsuk-black;
        .dropdown-menu

    {
        border: 2px solid $nhsuk-grey;
        box-sizing: border-box;
        border-radius: 5px;
        margin: 0;
        padding: 0;
        margin-top: 10px;
    }

    .dropdown-toggle::after {
        vertical-align: middle;
        border-top: 0.5em solid;
        border-right: 0.5em solid transparent;
        border-bottom: 0;
        border-left: 0.5em solid transparent;
    }

    a.dropdown-item {
        height: 35px !important;
        font-size: 1.6rem;
        text-decoration: none;
        display: flex;
        flex-direction: column;
        justify-content: center;
        border-top: 1px solid $nhsuk-grey-lighter;
        &:hover

    {
        cursor: pointer;
        background-color: $nhsuk-blue;
        color: $nhsuk-white;
    }

    }

    a.dropdown-item:first-child {
        border: none;
    }

    }

    div.options-dropdown {
        a .dropdown-item

    {
        width: 182px !important;
    }

    }

    #cancelMoveNode {
        color: $nhsuk-red;
    }

    @media (max-width: 768px) {

        .treeview-node-inner {
            width: calc(100vw - 30px);
        }
    }

    .content-structure-folder {
        font-size: 2rem;
    }
    i {
        color: #4C6272;
    }
</style>