<template>
    <div>
        <!--Node-->
        <div :id="'treenode' + item.nodePathId" v-if="isNode" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1">
                    <div class="pr-4" style="cursor:pointer" @click.prevent="toggle">
                        <i v-if="isOpen" class="fa-regular fa-folder-open fa-lg content-structure-folder" aria-hidden="true"></i>
                        <i v-if="!isOpen" class="fa-regular fa-folder fa-lg content-structure-folder" aria-hidden="true"></i>
                        <span class="node-name-text"><a href="#">{{ item.name }}</a></span>
                    </div>
                    <div class="ml-auto">
                        <a href="#" v-if="!isAncestor" @click.prevent="selectItem">Select {{nodeTypeText()}}</a>
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
                            <div class="small ml-3">Contents are loading, please wait...</div>
                        </div>
                    </div>
                </div>
            </div>

            <content-search-item class="tree-item-container"
                       v-for="(child, index) in orderedChildren"
                       :key="child.resourceId ? child.nodePathId + '_' + child.resourceId : child.nodePathId"
                       :item="child">
            </content-search-item>
            <!--:key="child.hierarchyEditDetailId"-->
        </div>

        <!--Resource-->
        <div v-if="!isNode" class="treeview-node" :style="{marginLeft: indentNegativeMargin + 'px', paddingLeft: indentPostivePadding + 'px'}">
            <div class="treeview-node-inner d-flex">
                <div class="treeview-node-inner-padding d-flex flex-row flex-grow-1">
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
                        <a href="#" @click.prevent="selectItem">Select resource</a>
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
        name: 'contentSearchItem',
        components: {
        },
        props: {
            item: { Type: NodeContentAdminModel } as PropOptions<NodeContentAdminModel>,
        },
        watch: { // triggered on create / cancel hierarchy edit
            item: function (newVal, oldVal) {
                console.log('treeItem.item changed: ', newVal, ' | was: ', oldVal);
                if (this.item.nodeTypeId === NodeType.Catalogue) {
                    this.isOpen = false;
                    this.childNodeList = [];
                    this.$forceUpdate();
                }
            },
        },
        data: function () {
            return {
                isOpen: false,
                showSpinner: false,
                childNodeList: [] as NodeContentAdminModel[],
                isError: false,
                commonlib: commonlib,

                isVisible: false,
            };
        },
        computed: {
            isNode: function (): boolean {
                return this.item.nodeTypeId > 0;
            },
            orderedChildren: function (): NodeContentAdminModel[] {
                return _.orderBy(this.childNodeList, ['isResource', 'displayOrder'], ['asc', 'asc'])
            },
            childrenLoaded: function (): boolean {
                return this.item.childrenLoaded;
            },
            canNavigateToResourceInfo: function (): boolean {
                return this.item.versionStatusId == VersionStatus.PUBLISHED;
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
            isEditingCatalogue: function (): boolean {
                return this.$store.state.contentStructureState.rootExtReferencedNode.nodePathId === this.$store.state.contentStructureState.rootNode.nodePathId;
            },
            isAncestor(): boolean {
                if (this.item.depth > 0) {
                    return this.$store.state.contentStructureState.editingTreeNode.parentNodeIds.includes(this.item.nodeId);
                }
                else return false;
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
            //else {
            //    var expandNodeList = this.expandNodes.toString().split('\\').map(Number);

            //    if (expandNodeList.includes(this.item.nodeId)) {
            //        this.toggle();
            //    }
            //}
        },
        methods: {
            nodeTypeText: function (): string {
                switch (this.item.nodeTypeId) {
                    case NodeType.Catalogue:
                        return "catalogue";
                    case NodeType.Folder:
                        return "folder";
                    default:
                        return "unknown";
                }
            },
            toggle: function () {
                if (this.isNode && this.item.nodeId) {
                    // Automatically horizontally scroll the treeview if user opens a folder whose left edge is greater than 40% from the left.
                    if (!this.isOpen) {
                        var viewportWidth = $(window).width();
                        var treenode = $('#treenode' + this.item.nodePathId);
                        if (treenode.position()) {
                            var treeNodeLeft = $('#treenode' + this.item.nodePathId).position().left;
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
            selectItem: function () {
                //alert("Item selected.");
                this.$store.dispatch('contentStructureState/referenceExternalItem', { selectedItem: this.item });
            },
            async loadNodeContents() {
                this.isError = false;
                await contentStructureData.getNodeContentsAdmin(this.item.nodePathId, !this.isEditingCatalogue).then(response => {

                    this.item.children = response;
                    this.item.children.forEach((child) => {
                        child.parent = this.item;
                        child.inEdit = this.item.inEdit;
                        child.showInTreeView = true;
                        child.depth = child.parent.depth + 1;
                        child.path = child.depth === 0 ? child.name : `${child.parent.path} > ${child.name}`;
                        child.isResource = child.nodeTypeId === NodeType.Resource;
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

    div.options-dropdown,
    div.references-dropdown {
        color: $nhsuk-black;

        .dropdown-menu {
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

            &:hover {
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

    div.references-dropdown {
        margin-right: 15px;

        ul {
            padding: 2.5px 0;
            margin: 0;

            li {
                padding: 2.5px 5px;
            }
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
    .node-path-edit-indicator {
        padding-right: 10px;
    }
</style>