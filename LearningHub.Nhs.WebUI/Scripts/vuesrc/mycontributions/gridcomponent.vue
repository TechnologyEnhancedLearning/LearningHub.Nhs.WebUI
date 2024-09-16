<template>
    <div>
        <div>
            <div v-if="uiCardCount > 1">

                <div class="row mx-n10 no-gutters">
                    <div class="col-lg-4 col-md-6 col-xs-12 px-10 mb-5 d-flex justify-content-center justify-content-md-start" v-if="!isReadonly">
                        <mycontributionscardheader></mycontributionscardheader>
                    </div>
                    <template v-for="item in this.cards">

                        <div class="col-lg-4 col-md-6 col-xs-12 px-10 mb-5 d-flex justify-content-center justify-content-md-start">
                            <gridcardcomp :ref="item.id"
                                          v-bind:key="item.id"
                                          v-bind:carditem="item"
                                          @createedit="createEdit"
                                          @createDuplicate="createDuplicate"
                                          @deleteResourceVersion="deleteResourceVersion"
                                          :isDuplicatingResource="isDuplicatingResource"
                                          :isResourceDuplicationSuccess="isResourceDuplicationSuccess"></gridcardcomp>
                        </div>

                    </template>
                </div>
                <div class="load-more text-center pb-5" v-if="!this.hideLoadMore">
                    <button class="btn btn-outline-custom" @click="loadNextRecordBatch()">{{ getLoadMoreButtonText() }}</button>
                </div>

            </div>

            <div class="norecords" v-if="uiCardCount == 1">
                <div class="row mx-n10 no-gutters">
                    <div class="col-lg-4 col-md-6 col-xs-12 px-10 mb-5 d-flex justify-content-center justify-content-md-start" v-if="!isReadonly">
                        <mycontributionscardheader></mycontributionscardheader>
                    </div>
                </div>
            </div>

            <div id="createEdit" ref="createEditModal" class="modal create-edit-resource-modal" tabindex="-1" role="dialog" data-backdrop="static" data-keyboad="false" v-if="!isReadonly">
                <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                    <div class="modal-content">
                        <div class="modal-header text-center">
                            <h2>Edit resource</h2>
                        </div>

                        <div class="modal-body">
                            You are about to make a change to this resource. Editing this page will create a new draft, nothing will change until this draft is published.
                        </div>

                        <div class="modal-footer">
                            <button class="nhsuk-button nhsuk-button--secondary" data-dismiss="modal">Cancel</button>
                            <button class="nhsuk-button" @click="createVersion">Continue</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    
    import gridcardcomp from './gridcardcomponent.vue'
    import mycontributionscardheader from './mycontributionscardheader.vue'
    import { MyContributeTabEnum } from './mycontributionsEnum'
    
    import { MyContributionsCardModel } from '../models/contribute/mycontributionsCardModel';
    import { resourceData } from '../data/resource';

    export default Vue.extend({
        name: 'gridcomp',
        data() {
            return {
                MyContributeTabEnum,
                cardsPerRow: 4,
                selectedResourceId: 0,
                isDuplicatingResource: false,
                isResourceDuplicationSuccess: undefined,
            };
        },
        props: {
        },
        components: {
            'gridcardcomp': gridcardcomp,
            'mycontributionscardheader': mycontributionscardheader
        },
        mounted() {
            $(".panel-card-container").matchHeight();
        },
        updated() {
            $(".panel-card-container").matchHeight();
        },
        computed: {
            isReadonly(): boolean {
                return this.$store.state.myContributionsState.isReadonly;
            },
            cards(): MyContributionsCardModel[] {
                return this.$store.state.myContributionsState.cards;
            },
            cardsLoaded(): boolean {
                return this.$store.state.myContributionsState.cardsLoaded;
            },
            uiCardCount(): number {
                if (this.cards && this.cards.length && this.cards.length > 0)
                    return this.cards.length + 1
                else
                    return 1;
            },
            totalCardCount(): number {
                switch (this.$store.state.myContributionsState.selectedTab) {
                    case MyContributeTabEnum.ActionRequired:
                        return this.$store.state.myContributionsState.restrictToCurrentUser ? this.$store.state.myContributionsState.userActionRequiredCount : this.$store.state.myContributionsState.actionRequiredCount;
                    case MyContributeTabEnum.Draft:
                        return this.$store.state.myContributionsState.restrictToCurrentUser ? this.$store.state.myContributionsState.userDraftCount : this.$store.state.myContributionsState.draftCount;
                    case MyContributeTabEnum.Published:
                        return this.$store.state.myContributionsState.restrictToCurrentUser ? this.$store.state.myContributionsState.userPublishedCount : this.$store.state.myContributionsState.publishedCount;
                    case MyContributeTabEnum.Unpublished:
                        return this.$store.state.myContributionsState.restrictToCurrentUser ? this.$store.state.myContributionsState.userUnpublishedCount : this.$store.state.myContributionsState.unpublishedCount;
                    default:
                        return 0;
                }
            },
            hideLoadMore(): boolean {
                return (this.cards.length) == this.totalCardCount;
            }
        },
        methods: {
            createEdit(resourceId: number) {
                this.selectedResourceId = resourceId;
                $('#createEdit').modal('show');
            },
            createVersion(resourceId: number) {
                $('#createEdit').modal('hide');
                window.location.pathname = './Contribute/create-version/' + this.selectedResourceId.toString();
            },
            async loadNextRecordBatch() {
                var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
                // Remove the focus from the Load More button
                if (document.activeElement instanceof HTMLElement) {
                    document.activeElement.blur();
                }
                document.documentElement.scrollTop = scrollTop;
                this.$store.commit("myContributionsState/getNextPage");
            },
            getLoadMoreButtonText() {
                let remaining = this.totalCardCount - this.cards.length;
                const take = this.$store.state.myContributionsState.take;
                remaining = Math.min(take, remaining);
                let loadMoreText = `Load ${remaining} more resource`;
                if (remaining > 1) loadMoreText += 's';
                return loadMoreText;
            },
            async createDuplicate(selectedCard: MyContributionsCardModel): Promise<void> {
                console.log(`Duplicating Resource ${selectedCard.resourceId} of Version ${selectedCard.resourceVersionId}`);
                
                this.isDuplicatingResource = true;
                const childResourceVersionId = await resourceData.duplicateResource(selectedCard.resourceVersionId, this.$store.state.myContributionsState.selectedCatalogue.nodeId)
                    .then((createdId) => {
                        this.isResourceDuplicationSuccess = true;
                        this.isDuplicatingResource = false;
                        return createdId;
                    })
                    .catch(() => {
                        this.isResourceDuplicationSuccess = false;
                        this.isDuplicatingResource = false;
                    });
                
                if (childResourceVersionId > 0 && this.isResourceDuplicationSuccess) {
                    window.location.pathname = './contribute-resource/edit/' + childResourceVersionId;
                }
            },
            async deleteResourceVersion(selectedCard: MyContributionsCardModel): Promise<void> {
                resourceData.deleteResourceVersion(selectedCard.resourceVersionId).then(async response => {
                    this.$store.dispatch('contentStructureState/refreshContainingNodeContents', { resourceVersionId: selectedCard.resourceVersionId });
                    this.$store.dispatch("myContributionsState/refreshCardData");
                });
            },
        }
    })
</script>

