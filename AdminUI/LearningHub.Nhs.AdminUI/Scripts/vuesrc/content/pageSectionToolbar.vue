<template>
    <div>
        <div class="toolBarBox">
            <div class="toolBar">
                <div :style="getStatusStyle()" class="mr-4 p-3">
                    <i :class="contentLib.getPageSectionStatusIcon(this.pageSection.pageSectionDetail)"></i>
                    {{ contentLib.getPageSectionStatusText(this.pageSection.pageSectionDetail) }}
                </div>
                <div v-show="itemIndex !== 0 && !isDeletePending">
                    <button class="btn btn-light toolBarButton mr-1"
                            @click="onMoveUp()">
                        <i class="fa-solid fa-chevron-up"></i>
                    </button>
                </div>
                <div v-show="itemIndex !== (totalItems -1 ) && !isDeletePending">
                    <button class="btn btn-light toolBarButton mr-1"
                            @click="onMoveDown()">
                        <i class="fa-solid fa-chevron-down"></i>
                    </button>
                </div>
                <div v-show="!isDeletePending">
                    <button class="btn btn-light toolBarButton mr-1"
                            @click="onClone()">
                        <i class="fas fa-copy"></i>
                    </button>
                </div>
                <div v-show="!isDeletePending">
                    <button class="btn btn-light toolBarButton  mr-1" v-show="!hidden"
                            data-toggle="modal" :data-target="'#' + getHideModalId()">
                        <i class="fas fa-eye-slash"></i>
                    </button>
                    <button class="btn btn-light toolBarButton mr-1" v-show="hidden"
                            data-toggle="modal" :data-target="'#' + getUnHideModalId()">
                        <i class="fas fa-eye"></i>
                    </button>
                </div>
                <div v-show="!isDeletePending">
                    <button class="btn btn-light toolBarButton mr-1"
                            @click="onEdit()">
                        <i class="fa-solid fa-pencil"></i>
                    </button>
                </div>
                <div>
                    <button class="btn btn-light toolBarButton mr-1"
                            @click="onAddNew()">
                        <i class="fa-solid fa-circle-plus"></i>
                    </button>
                </div>
                <div class="mr-1" v-show="!isDeletePending">
                    <button class="btn btn-light toolBarButton mr-1"
                            data-toggle="modal" :data-target="'#' + getDeleteModalId()">
                        <i class="fas fa-times-circle"></i>
                    </button>
                </div>
            </div>
        </div>
        <div :id="getDeleteModalId()" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Delete row</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div>
                                <p>
                                    This row will be removed from the landing page. This action cannot be undone.
                                </p>
                                <p>
                                    Are you sure you want to delete this row?
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-primary btn-lg" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-custom-green btn-lg" data-dismiss="modal" @click="onDelete();">Continue</button>
                    </div>
                </div>
            </div>
        </div>
        <div :id="getHideModalId()" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Hide row</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div>
                                <p>
                                    This row will not be visible on the landing page.
                                </p>
                                <p>
                                    Are you sure you want to hide this row?
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-primary btn-lg" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-custom-green btn-lg" data-dismiss="modal" @click="onHide();">Continue</button>
                    </div>
                </div>
            </div>
        </div>
        <div :id="getUnHideModalId()" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Show row</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div>
                                <p>
                                    This row will be visible on the landing page.
                                </p>
                                <p>
                                    Are you sure you want to show this row?
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-primary btn-lg" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-custom-green btn-lg" data-dismiss="modal" @click="onUnHide();">Continue</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { PageSectionModel, PageSectionStatus } from '../models/content/pageSectionModel';
    import { contentLib } from './common';
    export default Vue.extend({
        components: {

        },
        props: {
            pageSection: { Type: PageSectionModel, required: true } as PropOptions<PageSectionModel>,
            itemIndex: { Type: Number, required: true } as PropOptions<Number>,
            totalItems: { Type: Number, required: true } as PropOptions<Number>,
        },
        data() {
            return {
                contentLib: contentLib,
            }
        },
        created() {
        },
        computed: {
            hidden():boolean {
                return this.pageSection.pageSectionDetail && this.pageSection.pageSectionDetail.draftHidden !== null
                    ? this.pageSection.pageSectionDetail.draftHidden
                    : this.pageSection.isHidden;
            },
            isDeletePending(): boolean {
                return this.pageSection.pageSectionDetail.deletePending == null
                    ? false
                    : this.pageSection.pageSectionDetail.deletePending;
            }
        },
        methods: {
            getStatusStyle(): string {
                return this.pageSection.pageSectionDetail.pageSectionStatus === PageSectionStatus.Live
                    ? 'background: #007F3B; height: 45px; margin: -8px;border: 1px solid #FFFFFF;'
                    : ((this.pageSection.pageSectionDetail.deletePending == null || !this.pageSection.pageSectionDetail.deletePending) ? 'background: #FFB81C; height: 45px; margin: -8px;border: 1px solid #FFFFFF;color: #425563;' : 'background: #DA291C; height: 45px; margin: -8px; border: 1px solid #FFFFFF;');
            },
            getStatusIconStyle(): string {
                return 'margin-top:20px;'
            },
            getHideModalId(): string {
                return "hideModal" + this.itemIndex;
            },
            getUnHideModalId(): string {
                return "unHideModal" + this.itemIndex;
            },
            getDeleteModalId(): string {
                return "deleteModal" + this.itemIndex;
            },
            onMoveUp() {
                this.$emit('moveUp', this.itemIndex);
            },
            onMoveDown() {
                this.$emit('moveDown', this.itemIndex);
            },
            onClone() {
                this.$emit('clone', this.itemIndex);
            },
            onHide() {
                this.$emit('hide', this.itemIndex);
            },
            onUnHide() {
                this.$emit('unHide', this.itemIndex);
            },
            onEdit() {
                this.$emit('edit', this.itemIndex);
            },
            onAddNew() {
                this.$emit('addNew', this.itemIndex);
            },
            onDelete() {
                this.$emit('delete', this.itemIndex);
            },
        },
    });
</script>

<style lang="scss" scoped>
    @use "../../../Styles/Abstracts/all" as *;

    .toolBarBox {
        position: absolute;
        right: 20px;
        top: 20px;
        z-index: 1;
    }

    .warningTriangle {
        color: $nhsuk-warm-yellow;
        font-weight: 600 !important;
    }

    .warningTriangleGrey {
        color: $nhsuk-grey;
        font-weight: 600 !important;
    }

    .liveCircle {
        font-weight: 600 !important;
    }

    .toolBar {
        display: flex;
        padding: 16px;
        height: 64px;
        float: right;
        background: $nhsuk-grey;
        border: 2px solid $nhsuk-white;
        color: $nhsuk-white;
        box-sizing: border-box;
        border-radius: 6px;
    }

    .toolBarButton {
        background: $nhsuk-grey;
        border: 0;
    }

    .btn-light {
        color: #ffffff;
        outline: 0 !important;
        border: none !important;
    }

    .btn-light:focus {
        outline: 0 !important;
        border: none !important;
    }

    .toolBarButton i {
        font-size: 18px;
    }

    .toolBarButton i .fa, .fas {
        font-weight: 100;
    }
</style>