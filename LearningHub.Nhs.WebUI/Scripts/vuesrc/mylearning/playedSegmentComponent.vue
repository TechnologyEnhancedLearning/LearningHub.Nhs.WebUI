<template>
    <div id="progressModal" class="modal fade" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false" aria-labelledby="exampleModalLiveLabel">
        <div class="modal-dialog modal-dialog-centered modal-xl" role="document" @keypress="keyHandler($event)">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="nhsuk-heading-xl">Your progress</h1>
                    <div class="close-button">
                        <button type="button" class="close" @click="closeModal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>

                </div>
                <div class="modal-body" v-if="!errorLoadingData">
                    <div class="guide-container">
                        Select each section for more information. Use the tab key on your keyboard or the Next and Previous section arrows to move between each section. You can also select a section using your mouse.

                        <div class="d-flex flex-row align-items-center small mt-3">
                            <div class="key-colour played"></div>
                            <div class="key-text mr-5">
                                Sections played
                            </div>
                            <div class="key-colour not-played"></div>
                            <div class="key-text">
                                Sections not played
                            </div>
                        </div>
                    </div>

                    <div class="progress-bar-container">
                        <div class="progress">
                            <div v-for="(segment, index) in segments" class="segment" role="progressbar" data-toggle="popover" data-trigger="manual" tabindex="0" data-placement="top" data-html="true" :data-content="getPopupText(segment)"
                                 :class="{ 'progress-bar': segment.played, 'segment-first': index == 0, 'segment-last': index == (segments.length - 1), 'played': segment.played, 'not-played': !segment.played, 'segment-highlight': selectedSegment == index }"
                                 :style="{width: segment.percentage + '%'}" :aria-valuenow="segment.percentage" aria-valuemin="0" aria-valuemax="100" @click="selectSegment(index)"></div>
                        </div>
                        <div class="progress-bar-times small d-flex flex-row justify-content-between">
                            <div>00:00</div>
                            <div>{{ getDurationHhmmss(mediaLengthInSeconds) }}</div>
                        </div>
                    </div>

                    <div class="d-flex flex-row justify-content-between">
                        <div class="segment-nav d-flex flex-row align-items-center small">
                            <div class="fa-stack fa-2x" style="vertical-align:middle;">
                                <i class="fas fa-circle fa-stack-2x segment-nav-circle" :class="{ 'segment-circle-disabled': selectedSegment == 0 }" @click="previousSegment()" />
                                <i class="fa-solid fa-chevron-left fa-stack-1x fa-inverse segment-nav-chevron left-chevron" :class="{ 'segment-nav-chevron-disabled': selectedSegment == 0 }" @click="previousSegment()" />
                            </div>
                            <div class="segment-nav-text">Previous section</div>
                        </div>
                        <div class="segment-nav d-flex flex-row align-items-center small">
                            <div class="segment-nav-text" style="text-align: right;">Next section</div>

                            <div class="fa-stack fa-2x" style="vertical-align:middle;">
                                <i class="fas fa-circle fa-stack-2x segment-nav-circle" :class="{ 'segment-circle-disabled': selectedSegment == segments.length - 1 }" />
                                <i class="fa-solid fa-chevron-right fa-stack-1x fa-inverse segment-nav-chevron right-chevron" :class="{ 'segment-nav-chevron-disabled': selectedSegment == segments.length - 1 }" @click="nextSegment()" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div v-if="errorLoadingData">
                        <div class="error-text">Error loading data: {{ errorMessage }}</div>
                    </div>
                    <div class="accordion col-md-12" id="accordion" v-if="!errorLoadingData">
                        <div class="pt-0 pb-3">
                            <div class="heading" id="activityDetailHeading">
                                <div class="mb-0">
                                    <a href="#" class="collapsed" data-toggle="collapse" data-target="#activityDetailBody" aria-expanded="false" aria-controls="activityDetailBody">
                                        <div class="accordion-arrow">Activity details</div>
                                    </a>
                                </div>
                            </div>
                            <div id="activityDetailBody" class="collapse" aria-labelledby="activityDetailHeading" data-parent="#accordion">
                                <div class="collapse-inner">
                                    <table class="segment-table">
                                        <tbody>
                                            <tr v-for="segment in segments">
                                                <td>
                                                    {{ getDurationHhmmss(segment.segmentStartTime) }} to {{ getDurationHhmmss(segment.segmentEndTime) }}<span v-if="isCurrentResourceVersion && !segment.played"> | <a :href="getResourceReference(segment.segmentStartTime)"><i class="fas fa-play-circle mr-2 play-icon"></i>Play</a></span>
                                                </td>
                                                <td class="no-wrap">
                                                    <div class="progress-indicator d-flex flex-row justify-content-end align-items-center">
                                                        <div>{{ segment.played ? 'Played' : 'Not played' }}</div>
                                                        <div>
                                                            <i v-if="segment.played" class="fa-solid fa-circle-check complete-icon" />
                                                            <span v-if="!segment.played" class="fa-stack fa-1x" style="width: 40px;">
                                                                <i class="fas fa-circle fa-stack-1x incomplete-circle" />
                                                                <i class="fas fa-ellipsis-h fa-stack-1x incomplete-ellipsis" />
                                                            </span>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { myLearningData } from '../data/myLearning';
    import { PlayedSegmentModel } from '../models/mylearning/playedSegmentModel';
    import { commonlib } from '../common';

    export default Vue.extend({
        name: 'PlayedSegmentComponent',
        data() {
            return {
                segments: new Array<PlayedSegmentModel>(),
                resourceReferenceId: 0,
                isCurrentResourceVersion: false,
                mediaLengthInSeconds: 0,
                errorLoadingData: false,
                errorMessage: '',
                showModal: true,
                selectedSegment: 0
            }
        },
        async mounted() {
            let self = this;

            window.addEventListener('keyup', function (ev) {
                self.keyHandler(ev);
            });
        },
        methods: {
            async viewProgress(resourceId: number, resourceReferenceId: number, majorVersion: number, mediaLengthInSeconds: number, isCurrentResourceVersion: boolean) {
                this.resourceReferenceId = resourceReferenceId;
                this.isCurrentResourceVersion = isCurrentResourceVersion;
                this.mediaLengthInSeconds = mediaLengthInSeconds;
                this.selectedSegment = 0;

                $('#progressModal').modal('show');

                await myLearningData.getPlayedSegments(resourceId, majorVersion, mediaLengthInSeconds)
                    .then(response => {
                        this.segments = response;
                        this.errorLoadingData = false;
                        this.errorMessage = undefined;

                        // Initialise bootstrap popovers. Have to wait to next tick so that they all exist in the DOM.
                        Vue.nextTick(function () {
                            $('[data-toggle="popover"]').popover({
                                container: 'body'
                            });
                        })

                        // Focus first segment. Need to use this modal event otherwise it doesn't work.
                        $('#progressModal').on('shown.bs.modal', function (e) {
                            if ($('.progress-bar-container').is(":visible")) {
                                // Only do this in desktop, not mobile.
                                $('.segment-first').popover('show').focus();
                            }
                        })

                        // Bootstrap Popover doesn't update its position when the element it is anchored to moves. Therefore when the Activity Details panel is
                        // collapsed or expanded we have to hide and redisplay the popover to get it back to the correct place.
                        var self = this;
                        $('#activityDetailBody').on('show.bs.collapse', function (e) {
                            $('.progress-bar-container .segment').popover('hide');
                        })
                        $('#activityDetailBody').on('shown.bs.collapse', function (e) {
                            $('.progress-bar-container .segment:eq(' + self.selectedSegment + ')').popover('show').focus();
                        })
                        $('#activityDetailBody').on('hide.bs.collapse', function (e) {
                            $('.progress-bar-container .segment').popover('hide');
                        })
                        $('#activityDetailBody').on('hidden.bs.collapse', function (e) {
                            $('.progress-bar-container .segment:eq(' + self.selectedSegment + ')').popover('show').focus();
                        })
                    })
                    .catch(e => {
                        console.log(e);
                        this.errorLoadingData = true;
                        this.errorMessage = e;
                    });
            },
            previousSegment() {
                if (this.selectedSegment > 0) {
                    this.selectSegment(this.selectedSegment - 1);
                }
            },
            nextSegment() {
                if (this.selectedSegment < this.segments.length - 1) {
                    this.selectSegment(this.selectedSegment + 1);
                }
            },
            selectSegment(index: number) {
                if ($('.progress-bar-container').is(":visible")) {
                    $('.progress-bar-container .segment').popover('hide');
                    this.selectedSegment = index;
                    $('.progress-bar-container .segment:eq(' + this.selectedSegment + ')').popover('show').focus();
                }
            },
            keyHandler(e: KeyboardEvent) {
                if (e.shiftKey && e.key === 'Tab') {
                    this.previousSegment();
                }
                else if (e.key === 'Tab') {
                    this.nextSegment();
                }
            },
            closeModal() {
                $('#progressModal').modal('hide');
                this.selectedSegment = 0;
                this.segments = new Array<PlayedSegmentModel>();
            },
            getPopupText(segment: PlayedSegmentModel) {
                let text = "";
                if (segment.played) {
                    text = 'Played: ';
                }
                else {
                    text = 'Not played: ';
                }

                text += commonlib.getDurationHhmmss(segment.segmentStartTime) + " to " + commonlib.getDurationHhmmss(segment.segmentEndTime);

                if (this.isCurrentResourceVersion && !segment.played) {
                    text += ' | <a style="font-color:white !important;" href="' + this.getResourceReference(segment.segmentStartTime) + '"><i class="fas fa-play-circle mr-2"></i>Play</a>';
                }

                return text;
            },
            getResourceReference(mediaStartTime: number) {
                return "/Resource/" + this.resourceReferenceId + "/item?mediaStartTime=" + mediaStartTime;
            },
            getDurationHhmmss(seconds: number) {
                return commonlib.getDurationHhmmss(seconds);
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    .title {
        text-align: center;
    }

    .close-button {
        float: right;
    }

    .modal-content {
        padding: 0;
    }

    .modal-header {
        padding: 35px 40px 10px 10px !important;
    }

    .modal-body {
        padding: 10px 40px 20px 40px !important;
    }

    .modal-footer {
        padding: 15px 40px 0 40px;
        background-color: $nhsuk-grey-white;
        border-bottom-right-radius: 8px;
        border-bottom-left-radius: 8px;
    }

    .guide-container {
        background-color: $nhsuk-grey-white;
        padding: 24px;
    }

    .key-text {
        margin-left: 10px;
        margin-right: 10px;
    }

    .key-colour {
        width: 18px;
        height: 18px;
    }

    .not-played {
        background-color: $nhsuk-grey-white;
        border: 1px solid $nhsuk-grey-light;
    }

    .played {
        background-color: $nhsuk-green;
    }

    .progress-bar-container {
        margin: 60px 55px 0 55px;
    }

    .progress {
        height: 20px;
        border-radius: 10px;
        border: 1px solid $nhsuk-grey-light;
    }

    .progress-bar-times {
        margin-top: 5px;
        margin-bottom: 15px;
    }

    .segment {
        outline-color: yellow;
        outline-width: 2px;
        cursor: pointer;
    }

    .segment-highlight {
        box-shadow: inset 0 0 0 2px yellow, inset 0 0 0 3px black;
        cursor: default;
    }

    .segment-first {
        border-top-left-radius: 10px;
        border-bottom-left-radius: 10px;
    }

    .segment-last {
        border-top-right-radius: 10px;
        border-bottom-right-radius: 10px;
    }

    .segment-nav-text {
        max-width: 60px;
    }

    .segment-nav-chevron {
        cursor: pointer;
    }

    .segment-nav-circle {
        color: $nhsuk-blue;
        font-size: 1.6em;
        margin-top: 6px;
    }

    .segment-nav-chevron-disabled {
        cursor: default;
    }

    .segment-circle-disabled {
        color: $nhsuk-grey-light;
    }

    .left-chevron {
        margin-left: -2px;
    }

    .right-chevron {
        margin-left: 2px;
    }

    .segment-table {
        width: 100%;
        display: block;
        max-height: 300px;
        overflow: auto;
    }

        .segment-table tbody {
            min-width: 100%;
            display: table;
        }

        .segment-table td {
            border-top: 1px solid $nhsuk-grey-light;
            border-bottom: 1px solid $nhsuk-grey-light;
            padding: 12px 0 11px 0;
        }

    .accordion-arrow {
        padding-left: 30px;
    }

    .accordion {
        padding: 0;
    }

    .collapse-inner {
        padding-top: 15px;
        padding-bottom: 15px;
    }

    a div.accordion-arrow::before {
        content: url('/images/triangle-down.svg');
        position: absolute;
        top: 0;
        left: 0;
    }

    a.collapsed div.accordion-arrow::before {
        content: url('/images/triangle-right.svg');
        position: absolute;
        top: 1px;
        left: 0;
    }

    .error-text {
        color: $nhsuk-red;
        padding-bottom: 15px;
    }

    @media (max-width: 768px) {
        .title {
            text-align: left;
        }

        .modal-header {
            padding: 25px 15px 25px 25px !important;
        }

        .modal-body {
            display: none;
        }

        .modal-footer {
            padding: 10px 15px 0 25px !important;
        }

        .segment-table {
            max-height: 285px;
            padding-right: 10px;
        }

        #activityDetailHeading {
            display: none;
        }

        #activityDetailBody {
            display: block;
        }

        .play-icon {
            display: none;
        }
    }
</style>
<!-- These styles only worked when not scoped -->
<style lang="scss">
    .popover-body {
        font-size: 16px !important;
        background-color: #425563;
        color: white;
    }

    .bs-popover-top > .arrow::after, .bs-popover-auto[x-placement^="top"] > .arrow::after {
        bottom: 1px;
        border-width: 0.5rem 0.5rem 0;
        border-top-color: #425563;
    }

    .popover-body a {
        color: white;
    }
</style>
