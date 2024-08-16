<template>
    <div class="white-background">
        <div class="lh-padding-fluid white-background" v-if="selectedCatalogue">
            <div class="lh-container-xl d-flex flex-row justify-content-between align-items-center">
                <div>
                    <div class="lh-container-xl d-flex flex-row justify-content-between align-items-center">
                        <h1>{{ selectedCatalogue.name }}</h1>
                        <div v-if="selectedCatalogue.hidden" class="ml-4 px-3 hidden">Hidden</div>
                    </div>
                    <div class="subheading mb-4" v-if="selectedCatalogue && selectedCatalogue.nodeId>1">
                        Role: Editor
                    </div>
                </div>
                <div class="d-block d-md-inline-block pl-md-4 pt-3 pt-md-0" v-if="userCatalogues && userCatalogues.length>1">
                    <div class="dropdown catalogue-dropdown">
                        <a class="dropdown-toggle" href="#" role="button" id="dropdownCatalogueSelect" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            Change catalogue
                        </a>
                        <div class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownCatalogueSelect">
                            <a v-for="catalogue in userCatalogues" class="dropdown-item" @click="catalogueChange(catalogue.id, catalogue.nodeId)">{{getCatalogueName(catalogue)}}</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="lh-padding-fluid-md">
            <div class="lh-container-xl">
                <div class="row mx-0 w-100">
                    <div class="col-12 px-md-0">
                        <nav class="subnavbarwhite navbar-expand-md navbar-toggleable-md">
                            <div class="bar">
                                <div class="navbar-toggler navbar-toggler-left">{{ selectedTabName }}</div>
                                <button class="navbar-toggler navbar-toggler-right" type="button" aria-controls="collapsingSubNavbar" aria-expanded="false" aria-label="Toggle navigation" id="subnavbar-toggler" data-toggle="collapse" data-target="#collapsingSubNavbar">
                                    <i class="fa-solid fa-chevron-down"></i>
                                    <i class="fa-solid fa-xmark"></i>
                                </button>
                            </div>
                            <div class="navbar-collapse collapse" id="collapsingSubNavbar">
                                <ul class="navbar-nav navbar-nav">
                                    <li class="subnavwhite-item" v-bind:class="[MyContributeTabEnum.AllContent == selectedTab && 'active']" v-if="selectedCatalogue && selectedCatalogue.nodeId>1">
                                        <a ref="allContentLink" v-on:keyup.enter="keyboardSelection(MyContributeTabEnum.AllContent)" tabindex="0" v-on:click="selectTab(MyContributeTabEnum.AllContent)" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">All content</a>
                                    </li>
                                    <li class="subnavwhite-item" v-bind:class="[MyContributeTabEnum.Draft == selectedTab && 'active']">
                                        <a ref="draftLink" v-on:keyup.enter="keyboardSelection(MyContributeTabEnum.Draft)" tabindex="0" v-on:click="selectTab(MyContributeTabEnum.Draft)" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">Draft ({{ draftCount }})</a>
                                    </li>
                                    <li class="subnavwhite-item" v-bind:class="[MyContributeTabEnum.Published == selectedTab && 'active']">
                                        <a ref="publishedLink" v-on:keyup.enter="keyboardSelection(MyContributeTabEnum.Published)" tabindex="0" v-on:click="selectTab(MyContributeTabEnum.Published); return false;" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">Published ({{ publishedCount }})</a>
                                    </li>
                                    <li class="subnavwhite-item" v-bind:class="[MyContributeTabEnum.Unpublished == selectedTab && 'active']">
                                        <a ref="unpublishedLink" v-on:keyup.enter="keyboardSelection(MyContributeTabEnum.Unpublished)" tabindex="0" v-on:click="selectTab(MyContributeTabEnum.Unpublished)" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">Unpublished ({{ unpublishedCount }})</a>
                                    </li>
                                    <li class="subnavwhite-item" v-bind:class="[MyContributeTabEnum.ActionRequired == selectedTab && 'active']">
                                        <a ref="actionRequiredLink" v-on:keyup.enter="keyboardSelection(MyContributeTabEnum.ActionRequired)" tabindex="0" v-on:click="selectTab(MyContributeTabEnum.ActionRequired)" class="subnavwhite-link text-nowrap" data-toggle="collapse" data-target=".navbar-collapse.show">Action required ({{ actionRequiredCount }})</a>
                                    </li>
                                </ul>
                            </div>
                        </nav>
                    </div>
                </div>
            </div>
        </div>

        <!--All content tab-->
        <div v-show="MyContributeTabEnum.AllContent == selectedTab" v-if="selectedCatalogue != null && selectedCatalogue.nodeId > 1">
            <div v-show="catalogueLockedForEdit" class="lh-padding-fluid grey-banner">
                <div class="lh-container-xl">
                    <div class="d-flex flex-row">
                        <i class="fa-solid fa-triangle-exclamation mr-4 mt-2 text-warning" aria-hidden="true"></i>
                        <div class="small">A Learning Hub system administrator is currently making changes to this catalogue. You can browse the catalogue but cannot add or move resources at this time.</div>
                    </div>
                </div>
            </div>
            <div class="lh-padding-fluid">
                <div class="lh-container-xl">
                    <div class="node-contents-treeview">
                        <content-structure :expandNodes="expandNodes" :readOnly="isReadonly" />
                    </div>
                </div>
            </div>
        </div>

        <!--Other tabs-->
        <div class="lh-padding-fluid" v-show="MyContributeTabEnum.AllContent != selectedTab">
            <div class="lh-container-xl radio-options mt-5" v-if="selectedCatalogue && selectedCatalogue.nodeId>1">
                <label class="checkContainer mb-0 mr-5">
                    View my contributions ({{userTotal}})
                    <input type="radio" name="restrictToCurrentUser" v-model="restrictToCurrentUser" :value="true" @change="setRestrictToCurrentUser()" />
                    <span class="radioButton"></span>
                </label>
                <label class="checkContainer mb-0">
                    View all resources in this catalogue ({{catalogueTotal}})
                    <input type="radio" name="restrictToCurrentUser" v-model="restrictToCurrentUser" :value="false" @change="setRestrictToCurrentUser()" />
                    <span class="radioButton"></span>
                </label>
            </div>
            <div class="lh-container-xl">
                <gridcomp class="w-100 mt-5"></gridcomp>
            </div>
        </div>

    </div>
</template>
<script lang="ts">
    import Vue, { PropOptions } from 'vue'
    import Vuetify from 'vuetify'
    import contentStructure from '../content-structure-editor/contentStructure.vue'
    import { contentStructureData } from '../data/contentStructure';
    import gridcomp from './gridcomponent.vue'
    import '../filters'
    import commoncomponents from '../globalcomponents'
    import { MyContributeTabEnum } from './mycontributionsEnum'
    import { CatalogueBasicModel } from '../models/catalogueModel';
    import { MyContributionsCardModel } from '../models/contribute/mycontributionsCardModel';
    import { HierarchyEditStatusEnum } from '../models/content-structure/hierarchyEditModel';

    Vue.use(Vuetify);
    commoncomponents.forEach(component => {
        Vue.component(component.name, component);
    });
    export default Vue.extend({
        name: 'mycontributions',
        props: {
            'isReadonly': { Type: Boolean } as PropOptions<boolean>
        },
        data() {
            return {
                MyContributeTabEnum,
                selectedTabName: "",
                restrictToCurrentUser: true,
                expandNodes: "",
                HierarchyEditStatusEnum: HierarchyEditStatusEnum,
            }
        },
        components: {
            'contentStructure': contentStructure,
            'gridcomp': gridcomp
        },
        computed: {
            cardsLoaded(): boolean {
                return this.$store.state.myContributionsState.cardsLoaded;
            },
            cardTotalsLoaded(): boolean {
                return this.$store.state.myContributionsState.cardTotalsLoaded;
            },
            selectedTab(): MyContributeTabEnum {
                return this.$store.state.myContributionsState.selectedTab;
            },
            userCatalogues(): CatalogueBasicModel[] {
                return this.$store.state.myContributionsState.userCatalogues;
            },
            selectedCatalogue(): CatalogueBasicModel {
                return this.$store.state.myContributionsState.selectedCatalogue;
            },
            cards(): MyContributionsCardModel[] {
                return this.$store.state.myContributionsState.cards;
            },
            actionRequiredCount(): number {
                return this.$store.state.myContributionsState.actionRequiredCount;
            },
            draftCount(): number {
                return this.$store.state.myContributionsState.draftCount;
            },
            publishedCount(): number {
                return this.$store.state.myContributionsState.publishedCount;
            },
            unpublishedCount(): number {
                return this.$store.state.myContributionsState.unpublishedCount;
            },
            userTotal(): number {
                switch (this.$store.state.myContributionsState.selectedTab) {
                    case MyContributeTabEnum.ActionRequired:
                        return this.$store.state.myContributionsState.userActionRequiredCount;
                    case MyContributeTabEnum.Draft:
                        return this.$store.state.myContributionsState.userDraftCount;
                    case MyContributeTabEnum.Published:
                        return this.$store.state.myContributionsState.userPublishedCount;
                    case MyContributeTabEnum.Unpublished:
                        return this.$store.state.myContributionsState.userUnpublishedCount;
                    case MyContributeTabEnum.AllContent:
                        return this.$store.state.myContributionsState.allContent;
                    default:
                        return 0;
                }
            },
            catalogueTotal(): number {
                switch (this.$store.state.myContributionsState.selectedTab) {
                    case MyContributeTabEnum.ActionRequired:
                        return this.$store.state.myContributionsState.actionRequiredCount;
                    case MyContributeTabEnum.Draft:
                        return this.$store.state.myContributionsState.draftCount;
                    case MyContributeTabEnum.Published:
                        return this.$store.state.myContributionsState.publishedCount;
                    case MyContributeTabEnum.Unpublished:
                        return this.$store.state.myContributionsState.unpublishedCount;
                    case MyContributeTabEnum.AllContent:
                        return this.$store.state.myContributionsState.allContent;
                    default:
                        return 0;
                }
            },
            catalogueLockedForEdit(): boolean {
                return !(this.$store.state.contentStructureState.hierarchyEdit === null)
                    && (this.$store.state.contentStructureState.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Draft
                        || this.$store.state.contentStructureState.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.SubmittedToPublishingQueue
                        || this.$store.state.contentStructureState.hierarchyEdit.hierarchyEditStatus === HierarchyEditStatusEnum.Publishing);
            }
        },
        methods: {
            selectTab(tabIndex: number) {
                this.$store.commit("myContributionsState/setSelectedTab", tabIndex);
                this.setTabName();

                if (this.selectedCatalogue.nodeId > 1 &&
                    this.$store.state.contentStructureState.catalogue &&
                    this.$store.state.contentStructureState.catalogue.catalogueNodeVersionId != this.selectedCatalogue.id) {
                    this.$store.dispatch('contentStructureState/populateCatalogue', this.selectedCatalogue.id);
                }

                this.setRoute();
            },
            setTabName() {
                if (this.selectedTab === MyContributeTabEnum.ActionRequired) {
                    this.selectedTabName = "Action required (" + this.actionRequiredCount + ")";
                }
                if (this.selectedTab === MyContributeTabEnum.Draft) {
                    this.selectedTabName = "Draft (" + this.draftCount + ")";
                }
                if (this.selectedTab === MyContributeTabEnum.Published) {
                    this.selectedTabName = "Published (" + this.publishedCount + ")";
                }
                if (this.selectedTab === MyContributeTabEnum.Unpublished) {
                    this.selectedTabName = "Unpublished (" + this.unpublishedCount + ")";
                }
                if (this.selectedTab === MyContributeTabEnum.AllContent) {
                    this.selectedTabName = "All content";
                }
            },
            selectedTabDescription() {
                switch (this.selectedTab) {
                    case MyContributeTabEnum.ActionRequired:
                        return 'action-required';
                    case MyContributeTabEnum.Draft:
                        return 'draft';
                    case MyContributeTabEnum.Published:
                        return 'published';
                    case MyContributeTabEnum.Unpublished:
                        return 'unpublished';
                    case MyContributeTabEnum.AllContent:
                        return 'allcontent';
                    default:
                        return '';
                }
            },
            setRestrictToCurrentUser() {
                this.$store.commit("myContributionsState/setRestrictToCurrentUser", this.restrictToCurrentUser);
            },
            keyboardSelection(tabIndex: number) {
                switch (tabIndex) {
                    case MyContributeTabEnum.ActionRequired:
                        (this.$refs.actionRequiredLink as any).click();
                        break;
                    case MyContributeTabEnum.Draft:
                        (this.$refs.draftLink as any).click();
                        break;
                    case MyContributeTabEnum.Published:
                        (this.$refs.publishedLink as any).click();
                        break;
                    case MyContributeTabEnum.Unpublished:
                        (this.$refs.unpublishedLink as any).click();
                        break;
                    case MyContributeTabEnum.AllContent:
                        (this.$refs.allContentLink as any).click();
                        break;
                    default:
                }
            },
            catalogueChange(catalogueNodeVersionId: number, nodeId: number) {
                this.expandNodes = "";
                this.$store.commit("myContributionsState/setCatalogue", nodeId);

                if (nodeId === 1) {
                    this.$store.commit("myContributionsState/setSelectedTab", this.selectedTab === MyContributeTabEnum.AllContent ? MyContributeTabEnum.Draft : this.selectedTab);
                } else {
                    this.$store.commit("myContributionsState/setSelectedTab", MyContributeTabEnum.AllContent);
                }

                this.setTabName();

                if (nodeId > 1) {
                    this.$store.dispatch('contentStructureState/populateCatalogue', this.selectedCatalogue.id);
                }

                this.setRoute();

                return false;
            },
            getCatalogueName(catalogue: CatalogueBasicModel) {
                if (catalogue.hidden) {
                    return catalogue.name + ' ** Hidden **';
                } else {
                    return catalogue.name;
                }
            },
            setRoute() {
                if (this.selectedCatalogue.nodeId === 1) {
                    this.$router.push({ name: 'MyContributionsStatus', params: { status: this.selectedTabDescription() } });
                } else {
                    this.$router.push({ name: 'MyContributionsStatusCatalogue', params: { status: this.selectedTabDescription(), catalogueUrl: this.selectedCatalogue.url } });
                }
            },
        },
        async created() {
            if (this.$route.params.status) {
                var selectedTab = MyContributeTabEnum.Unspecified;
                switch (this.$route.params.status) {
                    case "draft":
                        selectedTab = MyContributeTabEnum.Draft;
                        break;
                    case "published":
                        selectedTab = MyContributeTabEnum.Published;
                        break;
                    case "unpublished":
                        selectedTab = MyContributeTabEnum.Unpublished;
                        break;
                    case "action-required":
                        selectedTab = MyContributeTabEnum.ActionRequired;
                        break;
                    case "allcontent":
                        selectedTab = MyContributeTabEnum.AllContent;
                        break;
                }
                this.$store.commit("myContributionsState/setSelectedTab", selectedTab);

                if (selectedTab == MyContributeTabEnum.AllContent) {
                    this.setTabName();

                }
            }
            if (this.$route.params.catalogueUrl) {
                this.$store.commit("myContributionsState/setCatalogueByUrl", this.$route.params.catalogueUrl);
            }
            this.$store.commit("myContributionsState/setReadOnly", this.isReadonly);
            this.$store.dispatch("myContributionsState/populateCatalogues");

            if (this.$route.params.nodeId) {
                await contentStructureData.getActiveNodePathToNode(parseInt(this.$route.params.nodeId)).then(x => {
                    this.expandNodes = x.nodePathString;
                });
            }
        },
        watch: {
            cardsLoaded(value) {
                if (value) {
                    this.setTabName();
                }
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .grey-banner {
        background-color: $nhsuk-grey-white;
        padding: 22px 0px;
        border-bottom: 1px solid $nhsuk-grey-light;
    }

    .hidden {
        background-color: $nhsuk-warm-yellow;
        color: $nhsuk-black;
    }

    .node-contents-treeview {
        padding: 24px 0 24px 0;
        overflow-x: auto;
        display: block;
        max-width: 1145px;
        margin-left: auto;
        margin-right: auto;
    }

    @media (min-width: 769px) {
        .border-y-grey {
            border-top: 1px solid $nhsuk-grey-light;
            border-bottom: 1px solid $nhsuk-grey-light;
        }
    }

    @media (max-width: 768px) {
        .dropdown-toggle {
            display: block !important;
        }
    }

</style>
