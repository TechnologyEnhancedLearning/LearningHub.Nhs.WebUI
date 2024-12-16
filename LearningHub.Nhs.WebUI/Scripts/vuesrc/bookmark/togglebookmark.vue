<template>
    <div :id="'toggleBookmark' + uniqueId">
        <div>
            <a class="btn pl-0" role="button" v-on:click="toggleBookmark()">
                <img v-bind:src="`/images/${this.item.isBookmarked ? 'bookmark-selected' : 'bookmark'}.svg`" />
                <span v-if="showLabel" class="pl-2"> {{ this.item.isBookmarked ? 'Remove bookmark' : 'Add bookmark' }} </span>
            </a>
        </div>

        <div :id="uniqueId" class="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header justify-content-center">
                        <p v-if="!this.item.isBookmarked">
                            <span class="modal-title pl-3">Add bookmark</span>
                        </p>
                        <p v-if="this.item.isBookmarked">
                            <i class="fa-solid fa-triangle-exclamation warningTriangle mb-2"></i><span class="modal-title pl-3">Delete bookmark</span>
                        </p>
                    </div>
                    <div class="modal-body mt-3 p-0">
                        <div v-if="!this.item.isBookmarked">
                            <div class="center-text">If you don't enter a title the title shown on the resource is used for the bookmark title</div>
                            <div class="modal-body-container p-3 mt-2 mb-2">
                                <form>
                                    <div class="d-flex">
                                        <label class="nhsuk-u-visually-hidden" for="titleInput"></label>
                                        <input class="form-control input titleInput" id="titleInput" type="text" v-model="title" v-bind:maxlength="maxAllowedTitleLength" />
                                    </div>
                                </form>
                            </div>
                            <small class="pl-15">You have {{charsRemaining}} characters remaining</small>
                        </div>
                        <div v-if="this.item.isBookmarked" class="modal-body-container p-5 mt-2 mb-2">
                            <p class="center-text">You have chosen to delete this bookmark. Are you sure you want to delete this?</p>
                            <p class="center-text">This action cannot be undone</p>
                        </div>
                    </div>
                    <div class="d-flex mt-5">
                        <div class="d-flex mr-auto">
                            <button data-dismiss="modal" type="button" class="nhsuk-button nhsuk-button--secondary" @click="cancel">Cancel</button>
                        </div>
                        <div class="d-flex ml-auto">
                            <button type="button" class="nhsuk-button" @click="save" v-bind:disabled="!valid && !this.item.isBookmarked">Continue</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <button :id="uniqueId +'ToggleButton'" hidden @click="processDeleteBookmark();"></button>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import Vuelidate from "vuelidate";
    import { BookmarkActionType, BookmarkType, ToggleBookmarkModel, UserBookmarkModel } from '../models/userBookmark';
    import { bookmarkData } from '../data/bookmark';

    Vue.use(Vuelidate as any);

    export default Vue.extend({
        components: {
        },
        props: {
            showLabel: { type: Boolean } as PropOptions<boolean>,
            item: { type: Object } as PropOptions<ToggleBookmarkModel>,
        },
        data() {
            return {
                maxAllowedTitleLength: 60 as number,
                title: '' as string,
            }
        },
        computed: {
            valid(): boolean {
                return this.title.length > 0 && this.title.length <= this.maxAllowedTitleLength;
            },
            charsRemaining(): number {
                return this.maxAllowedTitleLength - this.title.length;
            },
            uniqueId(): string {
                if (this.item.id)
                    return `modal${this.item.id}`;
                else
                    return `modal${this.item.resourceReferenceId ? this.item.resourceReferenceId : this.item.nodeId}`;
            }
        },        
        methods: {
            toggleBookmark: function () {

                if (!this.item.isBookmarked) {
                    this.title = this.item.title.substring(0,60);
                }

                $(`#${this.uniqueId}`).appendTo("body").modal('show');
            },
            cancel() {                
                $(`#${this.uniqueId}`).modal('hide');
            },
            save() {
                const userBookmark: UserBookmarkModel =
                {
                    id: this.item.id ? this.item.id : 0,
                    bookmarkTypeId: this.item.bookmarkType,
                    title: this.title,
                    link: this.item.link,
                    resourceReferenceId: this.item.resourceReferenceId,
                    nodeId: this.item.nodeId
                };
                bookmarkData.toggle(userBookmark).then(response => {
                    $(`#${this.uniqueId}`).modal('hide');
                    this.item.id = response;
                    this.item.title = this.title;
                    this.item.isBookmarked = !this.item.isBookmarked;
                    this.$emit('onToggled', this.item);
                    $('#hdnBookmarkManager').click();
                });
            },
            processDeleteBookmark() {
                console.log('bookmark deleted from panel');
                console.log(this.item);
                this.item.isBookmarked = false;
                this.$emit('onToggled', this.item);
            }
        },
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    p, span, a, small, btn {
        font-family: FrutigerLTW01-55Roman, Arial, sans-serif !important;
    }

    .center-text{
        text-align:center;
    }

    .modal-content .modal-header .modal-title {
        font-style: normal;
        font-weight: bold;
        font-size: 22px;
        line-height: 32px;
    }

    .toggle-bookmark-modal {
        margin-left: auto;
        margin-right: auto;
    }

    .btn-custom {
        background: #007F3B !important;
    }

    .modal-body-container {
        background: #F0F4F5;
    }

    .titleInput {
        width: 100%;
    }

    .warningTriangle {
        color: #ffb81c;
        height: 19px;
        width: 18px;
    }

    .btn {
        font-style: normal;
        font-weight: normal;
        font-size: 16px;
        line-height: 28px;
    }

    .btn-link {
        color: #005EB8;
    }

    a {
        color: #005EB8;
    }
</style>
