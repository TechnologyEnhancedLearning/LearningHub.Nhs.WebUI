<template>
    <div id="contentPageHeader" class="py-4 bg-white">
        <div class="d-flex flex-wrap mx-4">
            <div class="d-flex align-items-center p-2">
                <a href="..">< Go back </a>
            </div>
            <div class="d-flex align-items-center p-2" style="width:200px;">
                <i :class="contentLib.getPageStatusIcon(this.page.pageStatus)"></i>
                <div class="pl-3">Viewing {{ contentLib.getPageHeaderStatusText(this.page.pageStatus) }} version</div>
            </div>
            <div class="d-flex align-items-center p-2">
                <button class="btn btn-outline-primary btn-lg" :disabled="!page.canDiscard" data-toggle="modal" data-target="#discardModal">Discard changes</button>
            </div>
            <div class="d-flex align-items-center p-2">
                <button class="btn btn-outline-primary btn-lg" :disabled="!page.hasHiddenSections" @click="onToggleHiddenSections();"> {{toogleHiddenSectionsButtonText}} hidden rows</button>
            </div>
            <div class="d-flex align-items-center p-2">
                <button class="btn btn-outline-primary btn-lg" @click="onToggleToolbar();">{{toogleToobarButtonText}} tools</button>
            </div>
            <div class="d-flex align-items-center p-2">
                <button class="btn btn-outline-primary btn-lg" :disabled="!page.canPreview" @click="onPreview();">Preview changes</button>
            </div>
            <div class="d-flex align-items-center p-2">
                <button class="btn btn-custom-green btn-lg" :disabled="!page.canPublish" data-toggle="modal" data-target="#publishModal">Publish changes</button>
            </div>
        </div>

        <div id="discardModal" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header mb-15">
                        <h2><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Discard changes</h2>

                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div>
                                <p>All changes made will be lost. This action cannot be undone.</p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer" v-show="!publishCompleted">
                        <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-custom-green" data-dismiss="modal" @click="onDiscard();">Continue</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="publishModal" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-md" role="document">
                <div class="modal-content">
                    <div class="modal-header mb-15">
                        <h2 v-show="!publishCompleted"><i class="warningTriangle fas fa-exclamation-triangle mr-3"></i>Publish changes</h2>
                        <h2 v-show="publishCompleted"><i class="liveCircle fas fa-check-circle mr-3"></i>Published</h2>
                    </div>
                    <div class="modal-body">
                        <div class="model-body-container mb-25">
                            <div v-show="!publishCompleted">
                                <p>All changes will be made to the landing page.</p>
                                <p>Are you sure you want to publish the changes?</p>
                            </div>
                            <div v-show="publishCompleted">
                                <p>Changes to the landing page have been published.</p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer" v-show="!publishCompleted">
                        <button type="button" class="btn btn-outline-primary" data-dismiss="modal" @click="hidePublishWarningModal()">Cancel</button>
                        <button type="button" class="btn btn-custom-green" @click="onPublish();">Continue</button>
                    </div>
                    <div class="modal-footer" v-show="publishCompleted">
                        <button type="button" class="btn btn-outline-primary" data-dismiss="modal" @click="onPublishReset()">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<style lang="scss" scoped>
    #contentPageHeader {
        .btn {
            height: 40px;
            min-width: 140px;
            font-size: 19px;
            padding: 0px 5px 0px 5px;
        }
        
        .btn:hover {
            background-color: #005EB8;
            color: #FFFFFF;
        }
        
        .btn:disabled {
            color: #AEB7BD !important;
            border-color: #AEB7BD !important;
        }
        
        .btn:disabled:hover {
            background-color: #ffffff;
        }
        
        .btn-custom-green:disabled {
            background: #AEB7BD !important;
            border-color: #AEB7BD !important;
            color: #FFFFFF !important;
        }
    }
</style>
<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { PageModel, PageStatus } from '../models/content/pageModel';
    import { contentLib } from './common';
    import { contentData } from '../data/content';
    export default Vue.extend({
        components: {

        },
        props: {
            page: { Type: PageModel, required: true } as PropOptions<PageModel>,
            publishCompleted: { Type: Boolean } as PropOptions<Boolean>,
        },
        data() {
            return {
                contentLib: contentLib,
                toggleToolBarVisibility: false,
                toogleHiddenSectionsButtonText: 'Show',
                toogleToobarButtonText: 'Hide',
                showPublishCompleteModal: false as Boolean,
            }
        },
        created() {
        },
        methods: {
            onToggleHiddenSections() {
                if (this.toogleHiddenSectionsButtonText === 'Show')
                    this.toogleHiddenSectionsButtonText = 'Hide';
                else
                    this.toogleHiddenSectionsButtonText = 'Show'

                this.$emit('toggleSection');
            },
            onToggleToolbar() {
                if (this.toogleToobarButtonText === 'Hide')
                    this.toogleToobarButtonText = 'Show';
                else
                    this.toogleToobarButtonText = 'Hide'

                this.$emit('toggleToolBar');
            },
            onDiscard() {
                contentData.discardPageChanges(this.page.id).then(response => {
                    this.$emit('discard');
                });
            },
            onPublishReset() {
                this.$emit('publishReset');
            },
            onPreview() {
                window.open(this.page.previewUrl, "_blank")
            },
            onPublish() {
                this.$emit('publish');
            }
        },
    });
</script>