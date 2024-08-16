<template>
    <div>
        <!--Node-->
        <div :id="'treenode' + item.nodeId" v-if="isNode && isVisible" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1">
                    <div style="cursor:pointer" @click.prevent="toggle">
                        <i v-if="isOpen" class="fa-regular fa-folder-open fa-lg content-structure-folder" aria-hidden="true"></i>
                        <i v-if="!isOpen" class="fa-regular fa-folder fa-lg content-structure-folder" aria-hidden="true"></i>
                        <span class="node-name-text"><a href="#">{{ item.name }}</a></span>
                    </div>
                    <div v-if="editMode === EditModeEnum.MoveResource" class="ml-auto">
                        <a v-if="editingTreeNode.nodeId != item.nodeId" href="#" @click.prevent="onMoveResource">Move here</a>
                    </div>
                </div>
            </div>
        </div>

        <div v-show="isOpen" v-if="isNode">
            <div v-if="!item.childrenLoaded && showSpinner" class="tree-item-container">
                <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                    <div class="treeview-node-inner d-flex">
                        <div class="treeview-node-inner-padding d-flex flex-row">
                            <div class="ml-2"><i class="fa fa-spinner fa-spin" /></div>
                            <div class="ml-3">Contents are loading, please wait...</div>
                        </div>
                    </div>
                </div>
            </div>

            <div v-if="!readOnly && canContributeResource && this.item.childrenLoaded" class="tree-item-container">
                <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                    <div class="treeview-node-inner d-flex">
                        <div class="treeview-node-inner-padding">
                            <span style="cursor:pointer">
                                <i class="fa fa-plus-circle create-folder-circle" aria-hidden="true" @click="contributeResource"></i>
                                <a @click.prevent="contributeResource" style="padding-left:5px" href="#">Contribute a resource here</a>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="canMoveResourceToRoot" class="tree-item-container">
                <div class="treeview-node">
                    <div class="treeview-node-inner d-flex">
                        <div class="treeview-node-inner-padding">
                            <span>
                                <a @click.prevent="onMoveResource" href="#">Move here</a>
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            <tree-item class="tree-item-container"
                       v-for="(child, index) in orderedChildren"
                       :key="child.hierarchyEditDetailId"
                       :item="child"
                       :expandNodes="expandNodes"
                       :readOnly="readOnly"
                       @promptEditResource="promptEditResource"></tree-item>
        </div>

        <!--Resource-->
        <div v-if="!isNode" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1" v-bind:class="{ 'moving-highlight' : isMovingResource }">
                    <div>
                        <div>
                            <a v-if="canNavigateToResourceInfo" :href="getResourceUrl(item.resourceReferenceId)">{{item.name}}</a>
                            <span v-else>{{item.name}}</span>
                        </div>
                        <div class="resource-detail d-flex flex-row">
                            <div class="no-wrap"><i :class="commonlib.getResourceTypeIconClass(item.resourceTypeId, 0)"></i>&nbsp;{{ commonlib.getPrettifiedResourceTypeName(item.resourceTypeId, item.durationInMilliseconds) }}</div>
                            <div :class="resourceStatusCssClass()">&nbsp;- {{ resourceStatusText() }}</div>
                            <div v-if="item.resourceInEdit">&nbsp;- <span class="yellow">Editing...</span></div>
                            <div v-if="showResourceOptions && !readOnly" class="d-flex">
                                &nbsp;-&nbsp;
                                <div class="dropdown options-dropdown">
                                    <a class="dropdown-toggle" href="#" role="button" id="dropdownResourceItems" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" @click="recomputeResourceOptions">
                                        Options
                                    </a>
                                    <div class="dropdown-menu" aria-labelledby="dropdownNodeItems">
                                        <a class="dropdown-item" v-if="canEditResource" @click="onEditResource">Edit</a>
                                        <a class="dropdown-item" v-if="canMoveResourceUp" @click="onMoveResourceUp">Move up</a>
                                        <a class="dropdown-item" v-if="canMoveResourceDown" @click="onMoveResourceDown">Move down</a>
                                        <a class="dropdown-item" v-if="canMoveResource" @click="onInitiateMoveResource">Move</a>
                                        <a class="dropdown-item" v-if="canDuplicateResource" @click="onDuplicateResource">Duplicate</a>
                                        <a class="dropdown-item" v-if="canDeleteResource" @click="confirmDeleteResource" data-toggle="modal" data-target="#showDeleteConfirm">Delete</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div v-if="editMode === EditModeEnum.MoveResource && movingResource.resourceVersionId == item.resourceVersionId" class="my-auto ml-auto pr-2">
                        <span>Move this resource or <a id="cancelMoveResource" class="red" href="#" @click.prevent="onCancelMoveResource">Cancel move</a></span>
                    </div>
                </div>
            </div>

            <!-- Resource deletion modal -->
            <div v-if="showDeleteConfirm" id="deleteResourceVersion" ref="deleteResourceVersionModal" class="modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header text-center">
                            <h2 v-if="this.item.draftResourceVersionId == this.item.resourceVersionId">Delete this draft</h2>
                            <h2 v-else>Delete changed draft</h2>
                        </div>

                        <div class="modal-body">
                            <p>This will no longer be available.</p>
                            <p v-if="this.item.draftResourceVersionId != this.item.resourceVersionId">If a previously published copy of this resource exists, this will continue to be available.</p>
                        </div>

                        <div class="modal-footer">
                            <button class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal">Cancel</button>
                            <button class="nhsuk-button nhsuk-button--red" @click="onDeleteResourceVersion">Continue</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!--Error loading folder-->
        <div v-if="isNode && isOpen && isError" class="tree-item-container">
            <div class="treeview-node" :style="{marginLeft: (indentNegativeMargin - 32) + 'px', paddingLeft: (indentPostivePadding + 32) + 'px'}">
                <div class="treeview-node-inner d-flex">
                    <div class="treeview-node-inner-padding d-flex flex-row">
                        <i class="fa-solid fa-triangle-exclamation text-danger mr-3 mt-1" aria-hidden="true"></i>
                        <div>The contents of this folder failed to load, <span class="hyperlink" @click="toggle();toggle()">try again</span></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">

    import Vue, { PropOptions } from 'vue';
    import { EditModeEnum } from '../models/content-structure/editModeEnum';
    import { NodeContentEditorModel } from '../models/content-structure/nodeContentEditorModel';
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
            item: { Type: NodeContentEditorModel } as PropOptions<NodeContentEditorModel>,
            expandNodes: { Type: String, required: false, default: '' } as PropOptions<string>,
            readOnly: { Type: Boolean, required: false, default: true } as PropOptions<boolean>
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
                childNodeList: [] as NodeContentEditorModel[],
                isMoving: false,
                isVisible: false,
                isError: false,
                commonlib: commonlib,
                canMoveResourceDown: false,
                canMoveResourceUp: false,
                canEditResource: false,
                canMoveResource: false,
                showDeleteConfirm: false,
            };
        },
        computed: {
            isNode: function (): boolean {
                return this.item.nodeTypeId > 0;
            },
            orderedChildren: function (): NodeContentEditorModel[] {
                return _.orderBy(this.childNodeList, ['nodeTypeId', 'displayOrder'], ['desc', 'asc'])
            },
            childrenLoaded: function (): boolean {
                return this.item.childrenLoaded;
            },
            editMode(): EditModeEnum {
                return this.$store.state.contentStructureState.editMode;
            },
            editingTreeNode(): NodeContentEditorModel {
                return this.$store.state.contentStructureState.editingTreeNode;
            },
            updatedNode(): NodeContentEditorModel {
                return this.$store.state.contentStructureState.updatedNode;
            },
            canNavigateToResourceInfo: function (): boolean {
                return this.item.versionStatusId == VersionStatus.PUBLISHED && this.editMode === EditModeEnum.None;
            },
            showResourceOptions(): boolean {
                return !this.item.inEdit &&
                    this.item.versionStatusId != VersionStatus.SUBMITTED &&
                    this.item.versionStatusId != VersionStatus.PUBLISHING &&
                    this.editMode == EditModeEnum.None;
            },
            canContributeResource(): boolean {
                return !this.item.inEdit && this.editMode !== EditModeEnum.MoveResource && !this.isError;
            },
            canMoveResourceToRoot(): boolean {
                return this.editMode === EditModeEnum.MoveResource && this.movingResource.parent.depth > 0 && this.item.depth === 0;
            },
            movingResource(): NodeContentEditorModel {
                return this.$store.state.contentStructureState.movingResource;
            },
            isMovingResource: function (): boolean {
                return (this.editMode === EditModeEnum.MoveResource) && this.movingResource && this.movingResource.resourceVersionId === this.item.resourceVersionId;
            },
            canDuplicateResource(): boolean {
                return this.item.resourceTypeId == 10 || this.item.resourceTypeId == 11
            },
            canDeleteResource(): boolean {
                return this.item.versionStatusId == VersionStatus.FAILED ||
                    this.item.versionStatusId == VersionStatus.DRAFT ||
                    (this.item.versionStatusId == VersionStatus.PUBLISHED && this.item.resourceInEdit);
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
            recomputeResourceOptions: function () {
                this.canMoveResourceUp = this.item.displayOrder > 1;
                this.canMoveResourceDown = this.item.parent && this.item.displayOrder < this.item.parent.children.filter(c => c.nodeTypeId === 0).length;
                this.canEditResource = this.item.versionStatusId != VersionStatus.FAILED && this.item.versionStatusId != VersionStatus.SUBMITTED && this.item.versionStatusId != VersionStatus.PUBLISHING;
                this.canMoveResource = this.item.versionStatusId != VersionStatus.UNPUBLISHED && this.item.versionStatusId != VersionStatus.PUBLISHING && this.item.versionStatusId != VersionStatus.PUBLISHING;
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
            onEditResource: function () {
                if (this.item.versionStatusId == VersionStatus.DRAFT || this.item.resourceInEdit) {
                    if (this.item.resourceTypeId == ResourceType.CASE || this.item.resourceTypeId == ResourceType.ASSESSMENT) {
                        window.location.pathname = './contribute-resource/edit/' + this.item.draftResourceVersionId.toString();
                    }
                    else {
                        window.location.pathname = './Contribute/contribute-a-resource/' + this.item.draftResourceVersionId.toString();
                    }
                }
                else {
                    this.$emit('promptEditResource', this.item.resourceId);
                }
            },
            promptEditResource: function (resourceId: number) {
                this.$emit('promptEditResource', resourceId);
            },
            confirmDeleteResource() {
                // Using v-if to add popup to DOM on request, otherwise it would be repeated for every catalogue item. 
                // nextTick - Need to wait for popup to be added to DOM before showing it.
                this.showDeleteConfirm = true;
                Vue.nextTick(() => {
                    $("#deleteResourceVersion").modal("show");
                  });
            },
            async onDeleteResourceVersion() {
                $("#deleteResourceVersion").modal("hide");
                this.showDeleteConfirm = false;

                resourceData.deleteResourceVersion(this.item.draftResourceVersionId).then(async response => {
                    this.$store.dispatch('contentStructureState/refreshNodeContents', { node: this.item.parent });
                    this.$store.dispatch("myContributionsState/refreshCardData");
                });
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
            async onDuplicateResource() {
                // Copying the way the Pathology team did this.
                var isResourceDuplicationSuccess = false
                const childResourceVersionId = await resourceData.duplicateResource(this.item.resourceVersionId, this.item.parent.nodeId)
                    .then((createdId: Number) => {
                        isResourceDuplicationSuccess = true;
                        return createdId;
                    })
                    .catch(e => {
                        this.$store.commit('contentStructureState/setError', { errorMessage: 'Error duplicating resource.' });
                    });

                if (childResourceVersionId > 0 && isResourceDuplicationSuccess) {
                    window.location.pathname = './contribute-resource/edit/' + childResourceVersionId;
                }
            },
            onMoveResource: function () {
                this.$store.dispatch('contentStructureState/moveResource', { destinationNode: this.item });
            },
            onCancelMoveResource: function () {
                this.$store.commit('contentStructureState/cancelMoveResource');
            },
            async loadNodeContents() {
                this.isError = false;
                await contentStructureData.getNodeContentsForCatalogueEditor(this.item.nodePathId).then(response => {

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
            contributeResource: function () {
                window.location.href = '/contribute-resource/select-resource?catalogueId=' + this.$store.state.contentStructureState.catalogue.nodeId + '&nodeId=' + this.item.nodeId.toString() + '&primaryCatalogueNodeId=' + this.$store.state.contentStructureState.catalogue.primaryCatalogueNodeId;
            },
            getResourceUrl: function (resourceReferenceId: number) {
                if (resourceReferenceId) {
                    return `/Resource/${resourceReferenceId.toString()}/Item`;
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
    @use "../../../Styles/abstracts/all" as *;

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
        font-size: 16px !important;
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
        padding-bottom: 10px !important;
        padding-top: 10px !important;
        padding-left: 3px !important;
    }

    .tree-item-container:last-child {
        border-left: none;
    }

        .tree-item-container:last-child > .treeview-node > .treeview-node-inner:before {
            border-left: 2px solid $nhsuk-grey-light;
        }

    .resource-detail {
        font-size: 14px !important;
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
        font-size: 20px;
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
        font-size: 16px;
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

    @media (max-width: 768px) {

        .treeview-node-inner {
            width: calc(100vw - 30px);
        }

        .dropdown-toggle {
            display: block !important;
        }
    }
</style>