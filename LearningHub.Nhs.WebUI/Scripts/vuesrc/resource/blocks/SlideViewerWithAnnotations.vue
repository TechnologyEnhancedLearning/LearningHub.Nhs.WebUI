<template>
    <div class="slide-viewer-with-annotations"
         :class="{ 'slide-viewer-fullscreen': isFullScreen }">
        <div ref="viewport"
             class="slide-viewer-viewport"></div>
        <div class="slide-viewer-map">
            <div :id="`slide_viewer_${slideViewerRef}_map`"
                 class="slide-viewer-map-inner">
            </div>
        </div>

        <div class="slide-viewer-annotations-wrapper">
            <div v-show="showControls">
                <SlideViewerAnnotationUI v-if="isEditable || feedbackVisible || (!imageZone && listOfAnnotations.length > 0)"
                                         class="slide-annotation-ui"
                                         :adding-annotation="newAnnotationBeingPlaced"
                                         :list-of-annotations="listOfAnnotations"
                                         :index-of-editing-annotation="editingIndexValue"
                                         :index-of-selected-annotation="selectedIndexValue"
                                         :index-of-deleting-annotation="deletingIndexValue"
                                         :index-of-pin-edited="movePinIndex"
                                         :is-editable="isEditable"
                                         :drawing-mode-enabled="drawingModeEnabled"
                                         :selected-mark-index="selectedMarkIndexValue"
                                         :answers="answers"
                                         :feedbackVisible="feedbackVisible"
                                         :questionBlock="questionBlock"
                                         :imageZone="imageZone"
                                         @addAnnotationButtonPushed="addNewAnnotation"
                                         @editAnnotationClicked="editExistingAnnotation"
                                         @deleteAnnotationClicked="deleteAnnotationConfirm"
                                         @showAnnotationDetails="displayAnnotationDetails"
                                         @movePinMode="movePinMode"
                                         @newAnnotationDone="keepNewAnnotation"
                                         @newAnnotationDiscard="discardNewAnnotation"
                                         @pinDoneMoving="stopPinMovement"
                                         @moveAnnotationUp="moveAnnotationUpOne"
                                         @moveAnnotationDown="moveAnnotationDownOne"
                                         @stopDrawing="stopDrawingMode"
                                         @startDrawing="startDrawingMode"
                                         @annotationMarkDeleted="refreshAllOverlays"
                                         @annotationColourChanged="refreshColours"
                                         @selectAMark="setSelectedMarkIndex"/>
                <AnnotationDeleteConfirmation v-if="deleteAnnotationConfirmationPanel"
                                              class="slide-annotation-confirm-delete"
                                              :annotation-index="deletingIndexValue"
                                              :annotation-title="listOfAnnotations[deletingIndexValue].label"
                                              :imageZone="imageZone"
                                              @cancelDelete="deleteAnnotationCancelled"
                                              @confirmDelete="deleteAnnotationConfirmed"/>
            </div>
            <div>
                <AnnotationFullDetails class="slide-annotation-full-details"
                                       v-if="showAnnotationDetailsPanel"
                                       :is-editable="isEditable"
                                       :annotation-index="selectedIndexValue"
                                       :annotation="listOfAnnotations[selectedIndexValue]"
                                       @editAnnotationDetails="editAnnotation"/>
                <AnnotationDetailEdit class="slide-edit-annotation"
                                      v-if="editingAnnotationPanel"
                                      :annotation-index="editingIndexValue"
                                      :annotation-to-edit="listOfAnnotations[editingIndexValue]"
                                      :answer="answer"
                                      :force-cancel="forceCancelEdit"
                                      :imageZone="imageZone"
                                      @editDone="completedEdit"
                                      @editCancel="cancelledEdit"/>
            </div>
        </div>
        
        <div class="controls">
            <a v-on:click="toggleControlsVisibility()">
                <i class="fa-solid fa-chevron-up" v-if="showControls"></i>
                <i class="fa-solid fa-chevron-down" v-if="!showControls"></i>
                {{ showControls ? 'Hide Controls' : 'Show Controls' }}
            </a>
        </div>
        <div class="slide-viewer-controls-wrapper">
            <div v-show="showControls"
                 class="slide-viewer-controls">
                <SlideViewerStackControl :layers="layersCount"
                                         v-if="layersCount > 1"
                                         @setSlideImage="(layer) => setSlideImage(layer, false)"
                                         class="slide-viewer-control" />
                <SlideViewerRotateControl :viewer="viewer"
                                          class="slide-viewer-control"/>
                <SlideViewerZoomControl :viewer="viewer"
                                        class="slide-viewer-control"/>
            </div>
            <Button size="medium"
                    v-if="!isEditable"
                    @click="toggleControlsVisibility()"
                    class="slide-viewer-show-hide-controls nhsuk-u-font-size-16">
                {{ showControls ? 'Hide Controls' : 'Show Controls' }}
            </Button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import OpenSeadragon, { Placement, Point, Viewer } from 'openseadragon';
    import Button from '../../globalcomponents/Button.vue';
    import IconButton from '../../globalcomponents/IconButton.vue';
    import SlideViewerRotateControl from './SlideViewerRotateControl.vue';
    import SlideViewerZoomControl from './SlideViewerZoomControl.vue';
    import { WholeSlideImageModel } from '../../models/contribute-resource/blocks/wholeSlideImageModel';
    import SlideViewerAnnotationUI from './SlideViewerAnnotationUI.vue';
    import SlideViewerAddNewAnnotation from './SlideViewerAddNewAnnotation.vue';
    import AnnotationDetailEdit from './AnnotationDetailEdit.vue';
    import AnnotationDeleteConfirmation from './AnnotationDeleteConfirmation.vue';
    import AnnotationFullDetails from './AnnotationFullDetails.vue';
    import { ImageAnnotation } from '../../models/contribute-resource/blocks/annotations/imageAnnotationModel';
    import { PathCoordinates } from "../../models/contribute-resource/blocks/annotations/pathCoordinates";
    import { ImageAnnotationMarkFreehandShapeData } from "../../models/contribute-resource/blocks/annotations/imageAnnotationMarkFreehandShapeData";
    import { ImageAnnotationMarkModel } from "../../models/contribute-resource/blocks/annotations/imageAnnotationMarkModel";
    import { ImageAnnotationColourEnum } from "../../models/contribute-resource/blocks/annotations/imageAnnotationColourEnum";
    import { QuestionBlockModel } from "../../models/contribute-resource/blocks/questionBlockModel";
    import { AnswerTypeEnum } from "../../models/contribute-resource/blocks/questions/answerTypeEnum";
    import { AnswerModel } from "../../models/contribute-resource/blocks/questions/answerModel";
    import SlideViewerStackControl from "./SlideViewerStackControl.vue";

    let slideViewerRefSource = 0;
    const SVG_NS = 'http://www.w3.org/2000/svg';

    export default Vue.extend({
        components: {
            AnnotationFullDetails,
            AnnotationDeleteConfirmation,
            AnnotationDetailEdit,
            SlideViewerAddNewAnnotation,
            SlideViewerAnnotationUI,
            Button,
            IconButton,
            SlideViewerRotateControl,
            SlideViewerZoomControl,
            SlideViewerStackControl
        },
        props: {
            wholeSlideImage: { type: Object } as PropOptions<WholeSlideImageModel>,
            isFullScreen: Boolean,
            controlsVisible: Boolean,
            isEditable: Boolean,
            imageZone: Boolean,
            questionBlock: { type: Object } as PropOptions<QuestionBlockModel>,
            selectedAnswersProperty: { type: Array } as PropOptions<number[]>,
            disabled: Boolean,
            answers: { type: Array } as PropOptions<AnswerModel[]>,
            feedbackVisible: { type: Boolean, default: false } as PropOptions<boolean>,
        },
        data() {
            return {
                viewer: null as Viewer,
                slideViewerRef: slideViewerRefSource++,

                current: { type: Object } as PropOptions<ImageAnnotation>,

                // TODO ticket 11297, reduce these to a single index
                newAnnotationBeingPlaced: false,
                movePinIndex: -1,
                pinIsMoveable: false,
                editingAnnotationPanel: false,
                editingIndexValue: -1,
                deleteAnnotationConfirmationPanel: false,
                forceCancelEdit: false,
                deletingIndexValue: -1,
                showAnnotationDetailsPanel: true,
                selectedIndexValue: -1,
                drawingModeEnabled: false,
                isDrawingMark: false,
                drawingIndexValue: -1,
                nextAnnotationId: 0,
                selectedMarkIndexValue: -1,

                // True only if IE 6-11  
                isIE: navigator.userAgent.indexOf('MSIE') !== -1 || navigator.appVersion.indexOf('Trident/') > -1,

                // Only used in IE to allow marks to scale properly
                innerStrokeWidth: 3,
                borderStrokeWidth: 5,
                outerStrokeWidth: 13,
                showControls : false
            };
        },
        computed: {
            listOfAnnotations(): ImageAnnotation[] {
                return this.wholeSlideImage?.annotations;
            },
            answer(): AnswerModel {
                return this.questionBlock?.answers?.[this.editingIndexValue];
            },
            layersCount(): number {
                return this.wholeSlideImage?.getFileModel().wholeSlideImageFile.layers || 1;
            }
        },
        watch: {
            viewer: function (newVal, oldVal) {
                // If IE, redraw marks everytime zoom level is changes to maintain relative sizing
                if (this.isIE) {
                    this.viewer.addHandler('zoom', () => {
                        this.refreshAllOverlays();
                    });
                }
            },
            wholeSlideImage: function (newVal, oldVal) {
                this.showAnnotationDetailsPanel = false;
                this.setSlideImage();
                this.selectedIndexValue = -1;
            },
            newAnnotationBeingPlaced: function () {
                if (this.newAnnotationBeingPlaced) {
                    this.pinIsMoveable = true;
                    this.movePinIndex = this.wholeSlideImage.annotations.length - 1;
                } else {
                    this.movePinIndex = -1;
                    this.pinIsMoveable = false;
                }
            },
            editingAnnotationPanel: function () {
                if (this.editingAnnotationPanel) {
                    this.pinIsMoveable = false;
                } else {
                    this.editingIndexValue = -1;
                    if (!this.newAnnotationBeingPlaced) {
                        this.movePinIndex = -1;
                    }
                }

                this.stopDrawingMode();
                this.refreshAllOverlays();
            },
            deleteAnnotationConfirmationPanel: function () {
                if (this.deleteAnnotationConfirmationPanel) {
                    this.pinIsMoveable = false;
                } else {
                    this.deletingIndexValue = -1;
                }

                this.refreshAllOverlays();
            },
            showAnnotationDetailsPanel: function () {
                if (this.showAnnotationDetailsPanel) {
                    this.pinIsMoveable = false;
                } else {
                    this.selectedIndexValue = -1;
                }

                this.refreshAllOverlays();
            },
            pinIsMoveable: function () {
                if (this.pinIsMoveable) {
                    this.noZoomOnClick();
                } else {
                    this.movePinIndex = -1;
                    this.normalZoomOnClick();
                }

                this.refreshAllOverlays();
            },
            selectedIndexValue: function () {
                this.selectedMarkIndexValue = -1;
                this.stopDrawingMode();
                this.refreshAllOverlays();
            },
            drawingModeEnabled: function () {
                if (this.drawingModeEnabled) {
                    this.disableSlideMovement();
                    this.addFreehandDrawing();
                } else {
                    this.drawingIndexValue = -1;
                    this.enableSlideMovement();
                    this.refreshAllOverlays();
                }
            },
            selectedMarkIndexValue: function () {
                this.refreshAllOverlays();
            }
        },
        mounted() {
            this.setSlideImage();
            this.showControls = this.controlsVisible;
        },
        methods: {
            toggleControlsVisibility()
            {
                this.showControls = !this.showControls;
            },
            setSlideImage(layer: number = 0, resetControls: boolean = true): void {
                // The image is set after this element is created, so don't cause an error if there's no image  
                if (!this.wholeSlideImage || !this.wholeSlideImage.getFileModel()) return;

                if (!this.viewer) {
                    this.initialiseViewer();
                }

                const file = this.wholeSlideImage.getFileModel();
                const wholeSlideImageFile = file.wholeSlideImageFile;
                const currentZoom = this.viewer.viewport.getZoom();
                const bounds = this.viewer.viewport.getBounds();
                
                this.viewer.open({
                    Image: {
                        xmlns: 'http://schemas.microsoft.com/deepzoom/2008',
                        Url: `/api/Resource/SlideImageTile/${file.filePath}/${layer}/`,
                        Format: 'jpg',
                        Overlap: wholeSlideImageFile.deepZoomOverlap,
                        TileSize: wholeSlideImageFile.deepZoomTileSize,
                        Size: {
                            Height: wholeSlideImageFile.height,
                            Width: wholeSlideImageFile.width,
                        },
                    }
                });

                // Ensure any pins are loaded after the image to get them in correct position
                let self = this;
                var tileDrawnHandler = function () {
                    self.viewer.removeHandler('tile-drawn', tileDrawnHandler);
                    self.refreshAllOverlays();
                    if (!resetControls) {
                        self.viewer.viewport.zoomTo(currentZoom);
                        self.viewer.viewport.fitBounds(bounds);
                    }
                };

                this.viewer.addHandler('tile-drawn', tileDrawnHandler);

                if (resetControls) {
                    this.resetRotation();
                    this.resetZoom();
                }

                if (this.disabled) {
                    this.showAnnotationDetailsPanel = true;
                    this.$emit('updateSelectedAnswersProperty', this.selectedAnswersProperty);
                }
            },
            initialiseViewer() {
                const nhsukGrey = '#425563';
                const viewportElement = this.$refs.viewport as HTMLElement;

                this.viewer = OpenSeadragon({
                    element: viewportElement,
                    initialPage: 0,

                    showNavigator: true,
                    navigatorId: `slide_viewer_${ this.slideViewerRef }_map`,
                    navigatorDisplayRegionColor: nhsukGrey,

                    sequenceMode: true,
                    visibilityRatio: 0.9,
                    showNavigationControl: false,
                    showRotationControl: false,
                    showFullPageControl: false,
                    showHomeControl: false,
                    showSequenceControl: false,
                });

                // disable flip
                this.viewer.addHandler('flip', () => {
                    if (this.viewer.viewport.getFlip()) {
                        this.viewer.viewport.setFlip(false);
                    }
                });

                // Add click event
                this.viewer.addHandler('canvas-click', (event) => {
                    if (event.quick) {
                        this.moveMarkerPin(event);
                    }
                });
            },
            resetRotation(): void {
                this.viewer.viewport.setRotation(0);
            },
            resetZoom(): void {
                this.viewer.viewport.zoomTo(this.viewer.viewport.getMinZoom());
            },
            moveMarkerPin(event: OpenSeadragon.ViewerEvent): void {
                if (this.pinIsMoveable) {
                    let pinIdName = this.getMarkerName(this.movePinIndex + 1);
                    let viewportPoint = this.viewer.viewport.pointFromPixel(event.position);
                    let location = new OpenSeadragon.Point(viewportPoint.x, viewportPoint.y);

                    this.wholeSlideImage.annotations[this.movePinIndex].pinXCoordinate = this.checkMarkerOutOfBounds(viewportPoint.x, 'x');
                    this.wholeSlideImage.annotations[this.movePinIndex].pinYCoordinate = this.checkMarkerOutOfBounds(viewportPoint.y, 'y');
                    this.viewer.updateOverlay(pinIdName, location);
                    this.refreshAllOverlays();
                }
            },
            // Returns the coordinates of the initial marker.
            addInitialMarker(index: number): ({ x: number, y: number }) {
                let bounds = this.viewer.viewport.getBounds();
                const centreOfViewXCoordinate = bounds.x + 0.5 * bounds.width;
                const centreOfViewYCoordinate = bounds.y + 0.5 * bounds.height;
                let coordinatesOfCentrePoint = this.coordinatesOfMarker(centreOfViewXCoordinate, centreOfViewYCoordinate);

                let elt = this.createMarkerElement("pin", index);

                this.viewer.addOverlay({
                    element: elt,
                    placement: OpenSeadragon.Placement.BOTTOM,
                    location: new OpenSeadragon.Point(coordinatesOfCentrePoint.x, coordinatesOfCentrePoint.y),
                    rotationMode: OpenSeadragon.OverlayRotationMode.NO_ROTATION,
                });

                return coordinatesOfCentrePoint;
            },
            coordinatesOfMarker: function (x: number, y: number) {
                return {
                    x: x,
                    y: y
                };
            },
            refreshAllOverlays(): void {
                // Clears all present overlays, adds the annotation mark overlay at the bottom, and then draws each pin on top.
                this.viewer.clearOverlays();
                this.addBaseAnnotationOverlay();

                // TODO ticket 11297, this logic will end up being obselete
                // At any given time, there should only be the current index i and -1 for the other values. This grabs i and ignores the -1 values.
                let currentIndex = Math.max(this.editingIndexValue, this.selectedIndexValue, this.movePinIndex, this.deletingIndexValue);

                for (let i = 0; i < this.wholeSlideImage.annotations.length; i++) {
                    if (i === currentIndex) {
                        this.drawSelectedPin(i);
                    } else {
                        this.drawCirclePins(i);
                    }
                }
            },
            drawCirclePins(index: number): void {
                let annotation = this.wholeSlideImage.annotations[index];

                let x = annotation.pinXCoordinate;
                let y = annotation.pinYCoordinate;

                let elt = this.createMarkerElement("circle", index);

                this.viewer.addOverlay({
                    element: elt,
                    placement: OpenSeadragon.Placement.BOTTOM,
                    location: new OpenSeadragon.Point(x, y),
                    rotationMode: OpenSeadragon.OverlayRotationMode.NO_ROTATION,
                });

                let vue = this;
                new OpenSeadragon.MouseTracker({
                    element: elt,
                    clickHandler: function (event) {
                        vue.displayAnnotationDetails(index);
                    }
                });
            },
            drawSelectedPin(index: number): void {
                let annotation = this.wholeSlideImage.annotations[index];

                let x = annotation.pinXCoordinate;
                let y = annotation.pinYCoordinate;

                let elt = this.createMarkerElement("pin", index);

                this.viewer.addOverlay({
                    element: elt,
                    placement: OpenSeadragon.Placement.BOTTOM,
                    location: new OpenSeadragon.Point(x, y),
                    rotationMode: OpenSeadragon.OverlayRotationMode.NO_ROTATION,
                });

                let self = this;
                new OpenSeadragon.MouseTracker({
                    element: elt,
                    clickHandler: function (event) {
                        if (self.movePinIndex === -1) {
                            self.displayAnnotationDetails(index);
                        }
                    }
                });

                this.makePinDraggable(elt, index);
            },
            makePinDraggable(element: Element, index: number) {
                /* Needs to be as any as overlaysContainer is defined on the viewer initially, 
                 but after initialisation the property is not defined.*/
                let viewer = this.viewer as any;
                let self = this;

                new OpenSeadragon.MouseTracker({
                    element: element,

                    dragHandler: function (e) {
                        if (self.movePinIndex === index) {
                            let overlay = viewer.getOverlayById(element.id);
                            /* Need to use (e as any) here as delta is present in the ViewerEvents on construction
                             but is not defined after initialisation*/
                            let delta = viewer.viewport.deltaPointsFromPixels((e as any).delta);
                            overlay.update(overlay.location.plus(delta), Placement.BOTTOM);
                            overlay.drawHTML(viewer.overlaysContainer, viewer.viewport);
                        }
                    },

                    dragEndHandler: function () {
                        if (self.movePinIndex === index) {
                            let overlay = viewer.getOverlayById(element.id);

                            self.wholeSlideImage.annotations[index].pinXCoordinate = self.checkMarkerOutOfBounds(overlay.location.x, 'x');
                            self.wholeSlideImage.annotations[index].pinYCoordinate = self.checkMarkerOutOfBounds(overlay.location.y, 'y');

                            self.refreshAllOverlays();
                        }
                    }
                });
            },
            checkMarkerOutOfBounds: function (coordinate: number, axis: string) {
                let tiledImage = this.viewer.world.getItemAt(0);
                let imageBounds = tiledImage.getBounds();
                const boundary = axis === 'x' ? imageBounds.width : imageBounds.height;

                if (coordinate >= boundary) {
                    coordinate = boundary;
                } else if (coordinate < 0) {
                    coordinate = 0;
                }

                return coordinate;
            },
            createMarkerElement(typeOfMarker: string, index: number): Element {
                /* Manually creating elements here is required, as vue components cannot be supplied to 
                OpenSeaDragon and included within a dynamic overlay atop the image */

                let elt = document.createElement("div");

                // typeOfMarker will be either "pin" or "circle", depending on what needs to be drawn.

                if (typeOfMarker == "pin") {
                    elt.id = this.getMarkerName(index + 1);
                    elt.className = "fa-stack";

                    let pinElt = document.createElement("span");
                    pinElt.className = "fas fa-map-marker fa-stack-2x disabled";

                    let numElt = document.createElement("strong");
                    numElt.className = "fa-stack-1x annotation-item-pin-number";
                    numElt.innerText = (index + 1).toString();

                    pinElt.appendChild(numElt);
                    elt.appendChild(pinElt);
                } else if (typeOfMarker == "circle") {
                    elt.id = "markerPin" + (index + 1);
                    elt.className = "circle-icon";

                    let numElt = document.createElement("strong");
                    numElt.className = "annotation-item-circle-number";
                    numElt.innerText = (index + 1).toString();

                    elt.appendChild(numElt);
                }

                return elt;
            },
            addNewAnnotation(): void {
                this.closeAllPanels();
                this.newAnnotationBeingPlaced = true;

                const newAnnotation = this.wholeSlideImage.addAnnotation();
                if (this.imageZone) {
                    this.questionBlock.addAnswer(AnswerTypeEnum.Incorrect, newAnnotation.order);
                }
                newAnnotation.label = "Label " + (newAnnotation.order + 1);

                // Get the current bounds of the image, get the coordinates of the center and add a marker there.     
                let coordinates = this.addInitialMarker(this.wholeSlideImage.annotations.length - 1);

                newAnnotation.pinXCoordinate = coordinates.x;
                newAnnotation.pinYCoordinate = coordinates.y;
            },
            keepNewAnnotation(): void {
                this.editingIndexValue = this.listOfAnnotations.length - 1;
                this.editingAnnotationPanel = true;
                this.newAnnotationBeingPlaced = false;
            },
            discardNewAnnotation(): void {
                this.viewer.removeOverlay("markerPin" + (this.wholeSlideImage.annotations.length));
                this.listOfAnnotations.pop();
                this.newAnnotationBeingPlaced = false;
            },
            completedEdit(): void {
                this.editingAnnotationPanel = false;
                this.newAnnotationBeingPlaced = false;
            },
            cancelledEdit(originalValue: ImageAnnotation): void {
                Vue.set(this.listOfAnnotations, this.editingIndexValue, originalValue);
                this.editingAnnotationPanel = false;
                this.forceCancelEdit = false;
            },
            editExistingAnnotation(index: number): void {
                if (this.editingAnnotationPanel != true) // Cannot open new edit while edit is already happening
                {
                    this.closeAllPanels();
                    this.editingIndexValue = index;
                    this.editingAnnotationPanel = true;
                }
            },
            deleteAnnotationConfirm(index: number): void {
                this.closeAllPanels();
                this.deletingIndexValue = index;
                this.deleteAnnotationConfirmationPanel = true;
            },
            deleteAnnotationCancelled() {
                this.deleteAnnotationConfirmationPanel = false;
            },
            deleteAnnotationConfirmed(index: number) {
                this.wholeSlideImage.deleteAnnotation(this.listOfAnnotations[index]);
                this.questionBlock?.deleteAnswer(this.questionBlock.answers[index]);
                this.refreshAllOverlays();
                this.deleteAnnotationConfirmationPanel = false;
            },
            displayAnnotationDetails(index: number) {
                if (!this.disabled) {
                    this.closeAllPanels();
                    if (this.selectedIndexValue === index) {
                        this.showAnnotationDetailsPanel = false; // Toggle it if clicked again.
                        this.$emit('updateSelectedAnswersProperty', []);
                    } else {
                        this.selectedIndexValue = index;
                        this.showAnnotationDetailsPanel = true;
                        this.$emit('updateSelectedAnswersProperty', [index]);
                    }
                }
            },
            editAnnotation(index: number) {
                this.showAnnotationDetailsPanel = false;
                this.editExistingAnnotation(index);
            },
            cancelEdit() {
                if (this.editingAnnotationPanel == true) {
                    this.forceCancelEdit = true;  /* Trigger the open editing window to cancel any changes and close editing window */
                }
            },
            closeAllPanels() {
                this.cancelEdit();
                this.deleteAnnotationConfirmationPanel = false;
                this.newAnnotationBeingPlaced = false;
                this.showAnnotationDetailsPanel = false;
                this.pinIsMoveable = false;
            },
            noZoomOnClick() {
                // zoomPerClick is present on Options in the constructor, but is not on the type definition after initialisation
                (this.viewer as any).zoomPerClick = 1;
            },
            normalZoomOnClick() {
                // zoomPerClick is present on Options in the constructor, but is not on the type definition after initialisation
                (this.viewer as any).zoomPerClick = 2;
            },
            getMarkerName(indexOfMarker: number): string {
                return "markerPin" + indexOfMarker;
            },
            movePinMode(index: number) {
                this.movePinIndex = index;
                this.pinIsMoveable = true;
            },
            stopPinMovement() {
                this.movePinIndex = -1;
                this.pinIsMoveable = false;
            },
            moveAnnotationUpOne(index: number) {
                this.closeAllPanels();
                this.$nextTick(() => {
                    let annotation = this.wholeSlideImage.annotations[index];
                    this.wholeSlideImage.moveAnnotationUp(annotation);
                    if (this.imageZone) {
                        let answer = this.questionBlock.answers[index];
                        this.questionBlock.moveAnswerUp(answer);
                    }
                    this.refreshAllOverlays();
                });
            },
            moveAnnotationDownOne(index: number) {
                this.closeAllPanels();
                this.$nextTick(() => {
                    let annotation = this.wholeSlideImage.annotations[index];
                    this.wholeSlideImage.moveAnnotationDown(annotation);
                    if (this.imageZone) {
                        let answer = this.questionBlock.answers[index];
                        this.questionBlock.moveAnswerDown(answer);
                    }
                    this.refreshAllOverlays();
                });
            },
            stopDrawingMode() {
                this.drawingModeEnabled = false;
            },
            startDrawingMode(index: number) {
                this.drawingIndexValue = index;
                this.drawingModeEnabled = true;
            },
            disableSlideMovement() {
                /* These properties are present on Options in the constructor for the viewer, 
                but is not on the type definition after initialisation */
                (this.viewer as any).zoomPerClick = 1;
                (this.viewer as any).zoomPerScroll = 1;
                (this.viewer as any).panHorizontal = false;
                (this.viewer as any).panVertical = false;
            },
            enableSlideMovement() {
                /* These properties are present on Options in the constructor for the viewer, 
                but is not on the type definition after initialisation */
                (this.viewer as any).zoomPerClick = 2;
                (this.viewer as any).zoomPerScroll = 1.2;
                (this.viewer as any).panHorizontal = true;
                (this.viewer as any).panVertical = true;
            },
            addBaseAnnotationOverlay() {
                // Shows all annotation marks with no event handlers present to interfere with the normal functionality.
                let imageSize = this.viewer.world.getItemAt(0)?.source.dimensions;
                if (imageSize) {
                    let overlay = document.createElement('div');
                    let baseSvg = document.createElementNS(SVG_NS, 'svg');
                    overlay.appendChild(baseSvg);
                    baseSvg.setAttribute('class', 'svg-mark-container');
                    baseSvg.setAttribute('viewBox', `0 0 ${ imageSize.x } ${ imageSize.y }`);

                    this.viewer.addOverlay({
                        element: overlay,
                        px: 0,
                        py: 0,
                        width: imageSize.x,
                        height: imageSize.y
                    });

                    this.redrawAnnotationMarks(baseSvg);
                }
            },
            addFreehandDrawing() {
                let annotation = this.wholeSlideImage.annotations[this.selectedIndexValue];

                let self = this;
                let imageSize = this.viewer.world.getItemAt(0).source.dimensions;

                let overlay = document.createElement('div');
                let newSvg = document.createElementNS(SVG_NS, 'svg');
                overlay.appendChild(newSvg);
                newSvg.setAttribute('class', 'svg-mark-container-no-events');
                newSvg.setAttribute('viewBox', `0 0 ${ imageSize.x } ${ imageSize.y }`);

                let currentAnnotationMarkId = annotation.imageAnnotationMarks?.length || 0;

                // Add events for actually drawing the marks
                new OpenSeadragon.MouseTracker({
                    element: overlay,

                    pressHandler: function (event) {
                        if (self.drawingModeEnabled) {
                            self.isDrawingMark = true;
                            let coords = self.getCoordinates(overlay, event, imageSize);
                            self.createPath(coords, annotation);
                            self.redrawAnnotationMarks(newSvg);
                        }
                    },

                    moveHandler: function (event) {
                        if (self.drawingModeEnabled && currentAnnotationMarkId !== -1 && self.isDrawingMark) {
                            let coords = self.getCoordinates(overlay, event, imageSize);
                            self.addToPath(coords, annotation);
                            self.redrawAnnotationMarks(newSvg);
                        }
                    },

                    releaseHandler: function () {
                        self.isDrawingMark = false;
                        currentAnnotationMarkId = undefined;
                        self.redrawAnnotationMarks(newSvg);
                    }
                });

                this.viewer.addOverlay({
                    element: overlay,
                    px: 0,
                    py: 0,
                    width: imageSize.x,
                    height: imageSize.y
                });
            },
            redrawAnnotationMarks(svg: SVGElement) {
                while (svg.lastChild) {
                    svg.removeChild(svg.lastChild);
                }

                let allAnnotations = this.wholeSlideImage.annotations;

                allAnnotations.forEach(annotation => {
                    let allMarks = annotation.imageAnnotationMarks;
                    let lineColour = this.setLineColour(annotation.colour);
                    let annotationGroup = document.createElementNS(SVG_NS, 'g');

                    if (annotation.order != this.selectedIndexValue) {
                        annotationGroup.setAttribute('class', 'not-selected-mark-path');
                        this.drawNonSelectedMarks(annotation, annotationGroup, allMarks, lineColour, svg);
                    } else {
                        annotationGroup.setAttribute('class', 'selected-mark-path');
                        this.drawSelectedMarks(annotation, annotationGroup, allMarks, lineColour, svg);
                    }
                });
            },
            drawNonSelectedMarks(
                annotation: ImageAnnotation,
                annotationGroup: SVGElement,
                allMarks: ImageAnnotationMarkModel[],
                lineColour: string,
                svg: SVGElement) {
                // Annotation not selected; show translucent marks
                let outerGroup = document.createElementNS(SVG_NS, 'g');
                let borderGroup = document.createElementNS(SVG_NS, 'g');
                let innerGroup = document.createElementNS(SVG_NS, 'g');

                outerGroup.setAttribute('class', 'foog');
                borderGroup.setAttribute('class', 'foogborder');
                innerGroup.setAttribute('class', 'fooginner');

                if (allMarks) {
                    allMarks.forEach((mark) => {
                        let path = this.createMarkPathElement(mark, 'inner', lineColour);
                        let pathBorder = this.createMarkPathElement(mark, 'border', lineColour);
                        let pathOuter = this.createMarkPathElement(mark, 'outer', lineColour);

                        outerGroup.appendChild(pathOuter);
                        borderGroup.appendChild(pathBorder);
                        innerGroup.appendChild(path);

                        annotationGroup.appendChild(outerGroup);
                        annotationGroup.appendChild(borderGroup);
                        annotationGroup.appendChild(innerGroup);

                        svg.appendChild(annotationGroup);

                        // Each mark path needs this set separately
                        this.addUnselectedMarkEvents(path, annotation.order, annotationGroup);
                        this.addUnselectedMarkEvents(pathBorder, annotation.order, annotationGroup);
                        this.addUnselectedMarkEvents(pathOuter, annotation.order, annotationGroup);
                    });
                }
            },
            drawSelectedMarks(
                annotation: ImageAnnotation,
                annotationGroup: SVGElement,
                allMarks: ImageAnnotationMarkModel[],
                lineColour: string,
                svg: SVGElement) {
                //Annotation is selected; show selected mark as filled.
                if (allMarks) {
                    allMarks.forEach((mark, currentMarkIndex) => {

                        let selectedGroup = document.createElementNS(SVG_NS, 'g');
                        let outerGroup = document.createElementNS(SVG_NS, 'g');
                        let borderGroup = document.createElementNS(SVG_NS, 'g');
                        let innerGroup = document.createElementNS(SVG_NS, 'g');

                        selectedGroup.setAttribute('class', 'barg');
                        outerGroup.setAttribute('class', 'bar');
                        borderGroup.setAttribute('class', 'barborder');
                        innerGroup.setAttribute('class', 'barinner');

                        outerGroup.setAttribute('opacity', '0');

                        let path = this.createMarkPathElement(mark, 'inner', lineColour);
                        let pathBorder = this.createMarkPathElement(mark, 'border', lineColour);
                        let pathOuter = this.createMarkPathElement(mark, 'outer', lineColour);

                        outerGroup.appendChild(pathOuter);
                        borderGroup.appendChild(pathBorder);
                        innerGroup.appendChild(path);

                        selectedGroup.appendChild(outerGroup);
                        selectedGroup.appendChild(borderGroup);
                        selectedGroup.appendChild(innerGroup);
                        annotationGroup.appendChild(selectedGroup);

                        this.setSelectedGroupAppearance(selectedGroup, outerGroup, currentMarkIndex, annotation);

                        svg.appendChild(annotationGroup);

                        // Each mark path needs this set separately
                        this.addSelectedMarkEvents(path, annotation.order, currentMarkIndex, selectedGroup, outerGroup);
                        this.addSelectedMarkEvents(pathBorder, annotation.order, currentMarkIndex, selectedGroup, outerGroup);
                        this.addSelectedMarkEvents(pathOuter, annotation.order, currentMarkIndex, selectedGroup, outerGroup);
                    });
                }
            },
            setSelectedGroupAppearance(selectedGroup: SVGElement, outerGroup: SVGElement, currentMarkIndex: number, annotation: ImageAnnotation) {
                if (this.selectedMarkIndexValue === -1) {
                    // No mark selected; show all highlighted
                    selectedGroup.setAttribute('class', 'selected-mark-path');
                } else if (currentMarkIndex === this.selectedMarkIndexValue && annotation.order === this.selectedIndexValue) {
                    // Mark selected; just show that one highlighted & with a border
                    selectedGroup.setAttribute('class', 'selected-mark-path');
                    outerGroup.setAttribute('opacity', '0.4');
                } else {
                    // Not selected mark - show transparent
                    selectedGroup.setAttribute('class', 'not-selected-mark-path');
                }
            },
            setLineColour(colourEnum: ImageAnnotationColourEnum): string {
                return ImageAnnotationColourEnum[colourEnum].toLowerCase();
            },
            addUnselectedMarkEvents(targetMarkPath: SVGElement, annotationIndex: number, markGroup: SVGElement) {
                let self = this;

                // Adds click and "hover" events to a path. If clicked, set annotation index. If hovered over, make that group of marks be highlighted (but not selected)
                if (!this.drawingModeEnabled && !this.pinIsMoveable) {
                    new OpenSeadragon.MouseTracker({
                        element: targetMarkPath,

                        clickHandler: function (event) {
                            self.selectedIndexValue = annotationIndex;
                            self.showAnnotationDetailsPanel = true;
                        },
                        enterHandler: function (event) {
                            markGroup.setAttribute('class', 'selected-mark-path');
                        },
                        exitHandler: function (event) {
                            markGroup.setAttribute('class', 'not-selected-mark-path');
                        }
                    });
                }
            },
            addSelectedMarkEvents(element: SVGElement, annotationIndex: number, markIndex: number, markGroup: SVGElement, outerGroup: SVGElement) {
                let self = this;
                if (!this.drawingModeEnabled && !this.pinIsMoveable) {
                    new OpenSeadragon.MouseTracker({
                        element: element,

                        clickHandler: function (event) {
                            if (!self.drawingModeEnabled && self.selectedIndexValue === annotationIndex) {
                                // Toggle if a mark is selected or not
                                if (self.selectedMarkIndexValue === markIndex) {
                                    self.selectedMarkIndexValue = -1;
                                } else {
                                    self.selectedMarkIndexValue = markIndex;
                                }
                            }
                        },
                        enterHandler: function (event) {
                            // If no mark is selected, then just show a border around the current mark, otherwise make the mark more visible
                            if (self.selectedMarkIndexValue != -1 && self.selectedMarkIndexValue != markIndex) {
                                markGroup.setAttribute('class', 'selected-mark-path');
                            }

                            outerGroup.setAttribute('opacity', '0.4');
                        },
                        exitHandler: function (event) {
                            if (self.selectedMarkIndexValue != -1 && self.selectedMarkIndexValue != markIndex) {
                                markGroup.setAttribute('class', 'not-selected-mark-path');
                            }

                            // Don't reset the border of a selected mark
                            if (self.selectedMarkIndexValue != markIndex) {
                                outerGroup.setAttribute('opacity', '0');
                            }
                        }
                    });
                }
            },
            createMarkPathElement(mark: ImageAnnotationMarkModel, pathStyle: string, lineColour: string): SVGElement {
                let shapeData = mark.freehandMarkShapeData;

                let path = document.createElementNS(SVG_NS, 'path');

                path.setAttribute('fill', 'none');

                if (pathStyle === 'inner') {
                    path.setAttribute('stroke', lineColour);
                }

                if (!this.isIE) {
                    path.setAttribute('vector-effect', 'non-scaling-stroke');
                } else {
                    // If IE, calculate stroke-width to mirror the 'vector-effect: non-scaling-stroke' attribute which is not supported
                    let zoom = this.viewer.viewport.getZoom();

                    // Scale factor needed to account for images of different sizes/levels of zoom.
                    let scaleFactor = this.viewer.viewport.getMaxZoom();
                    const scaledWidth = (width: number) => width * scaleFactor / zoom;

                    let strokeWidth;
                    if (pathStyle === 'inner') {
                        strokeWidth = scaledWidth(this.innerStrokeWidth);
                    } else if (pathStyle === 'border') {
                        strokeWidth = scaledWidth(this.borderStrokeWidth);
                    } else if (pathStyle === 'outer') {
                        strokeWidth = scaledWidth(this.outerStrokeWidth);
                    }

                    path.setAttribute('stroke-width', strokeWidth + 'px');
                }

                let d = '';
                for (let i = 0; i < shapeData.pathCoordinates.length; i++) {
                    let coords = shapeData.pathCoordinates[i];
                    let command = (i === 0) ? 'M' : 'L';
                    d += `${ command } ${ Math.round(coords.x) } ${ Math.round(coords.y) }`;
                }

                path.setAttribute("d", d);
                path.style.pointerEvents = 'auto';

                return path;
            },
            getnextAnnotationId() {
                let annotationId = this.nextAnnotationId;
                this.nextAnnotationId++;
                return annotationId;
            },
            createPath(coords: PathCoordinates, annotation: ImageAnnotation) {
                let initialCoordinates = new PathCoordinates(
                    {
                        x: Math.round(coords.x),
                        y: Math.round(coords.y)
                    });

                let newMarkShapeData = new ImageAnnotationMarkFreehandShapeData();
                let newMarkData = new ImageAnnotationMarkModel();

                newMarkShapeData.pathCoordinates.push(initialCoordinates);
                newMarkData.freehandMarkShapeData = newMarkShapeData;

                if (annotation.imageAnnotationMarks == undefined) {
                    annotation.imageAnnotationMarks = [];
                }

                newMarkData.markLabel = "Mark " + (annotation.imageAnnotationMarks.length + 1);
                annotation.imageAnnotationMarks.push(newMarkData);
            },
            addToPath(coords: PathCoordinates, annotation: ImageAnnotation) {
                annotation.imageAnnotationMarks[annotation.imageAnnotationMarks.length - 1]
                    .freehandMarkShapeData.pathCoordinates.push({
                    x: Math.round(coords.x),
                    y: Math.round(coords.y)
                });
            },
            getCoordinates(overlay: HTMLDivElement, event: any, imageSize: Point): PathCoordinates {
                let originalEvent = event.originalEvent;
                return {
                    x: originalEvent.offsetX / overlay.offsetWidth * imageSize.x,
                    y: originalEvent.offsetY / overlay.offsetHeight * imageSize.y
                };
            },
            refreshColours() {
                this.refreshAllOverlays();
                this.addFreehandDrawing();
            },
            setSelectedMarkIndex(annotationIndex: number, markIndex: number) {
                if (annotationIndex === this.selectedIndexValue) {
                    this.stopDrawingMode();
                    this.selectedMarkIndexValue = markIndex;
                }
            }
        }
    });
</script>


<style lang="scss"> // Note: NOT "scoped" because we want these styles to apply to the children
    @use '../../../../Styles/abstracts/all' as *;

    .slide-viewer-with-annotations {
        background-color: $nhsuk-grey;
        position: relative;
        height: 100%;
        min-height: 700px;

        .slide-viewer-viewport {
            width: 100%;
            height: 100%;
        }

        .slide-viewer-map {
            position: absolute;
            right: 3px;
            bottom: 3px;
            width: 150px;
            height: 150px;
            background-color: $nhsuk-grey;
            border: 1px solid $nhsuk-grey-light;
            padding: 2px;

            .slide-viewer-map-inner {
                width: 100%;
                height: 100%;
            }
        }

        .slide-viewer-annotations-wrapper {
            position: absolute;
            display: flex;
            align-items: stretch;
            top: 20px;
            width: 100%;

            .slide-edit-annotation {
                z-index: 75;
            }

            .slide-annotation-ui {
                position: absolute;
                right: 20px;
                display: flex;
                max-height: 650px;
                z-index: 50;
            }

            .slide-edit-annotation,
            .slide-annotation-confirm-delete,
            .slide-annotation-full-details {
                position: absolute;
                margin-left: auto;
                margin-right: auto;
                left: 0;
                right: 0;
                width: auto;
                max-width: 400px;
                min-width: 250px;
                z-index: 50;
                display: flex;
            }

            .slide-annotation-confirm-delete {
                z-index: 100;
            }
        }


        .slide-viewer-controls-wrapper {
            position: absolute;
            left: 0;
            top: 35px;
            width: 165px;
            display: flex;
            flex-direction: column;
            align-items: stretch;
            padding: 10px;
            background-color: rgba(255, 255, 255, 0.5);
            height: 100%;

            .slide-viewer-controls {
                margin-bottom: 10px;

                .slide-viewer-control {
                    height: 200px;
                    width: 135px;
                    margin-top: 10px;
                    background-color: $nhsuk-white;
                    border-radius: 5px;
                    border: 1px solid $nhsuk-grey;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    font-size: 16px;
                    padding-top: 8px;

                    &:last-child {
                        margin-right: 0;
                    }
                }
            }
        }

        &.slide-viewer-fullscreen {
            .slide-viewer-controls-wrapper {
                @media screen and (orientation: portrait) {
                    bottom: 70px;
                }
            }

            .slide-annotation-full-details {
                @media screen and (orientation: landscape) {
                    top: 70px;
                }
            }
        }
    }

    .controls {
        font-size: 16px;
        background-color: white;
        width: 165px;
        height: 34px;
        position: absolute;
        top: 1px;
        left: 0;
        padding: 5px;

        a,
        a:hover{
            margin:10px;
            color: $nhsuk-blue;
            text-decoration: underline;
            cursor: pointer;
        }
    }
    
    .svgElement {
        border: 1px solid;
        margin-top: 4px;
        margin-left: 4px;
        cursor: default;
    }
</style>

<style lang="scss">// Note: NOT "scoped" because we want these styles to apply to the children
    @use "../../../../Styles/abstracts/all" as *;
    
    .openseadragon-canvas:focus {
        border: 3px solid $govuk-focus-highlight-yellow !important;
    }
    
    .openseadragon-canvas {
        .highlight {
            background-color: green;
        }
    
        .triangle-down {
            width: 0;
            height: 0;
            border-left: 30px solid transparent;
            border-right: 30px solid transparent;
            border-top: 50px solid red;
        }
    
        .fa-map-marker {
            color: $nhsuk-blue;
            -webkit-text-stroke-width: 1px;
            -webkit-text-stroke-color: $nhsuk-white;
            cursor: pointer;
        }
    
        .disabled {
            pointer-events: none;
        }
    
        .outer-pin {
            color: $nhsuk-white;
        }
    
        .annotation-item-pin-number {
            font-size: small;
            font-family: FrutigerLTW01-55Roman, Arial, sans-serif;
            margin-top: 6px;
            color: $nhsuk-white;
            -webkit-text-stroke-width: 0;
        }
    
        .circle-icon {
            font-size: 3em;
            background-color: $nhsuk-white;
            border-radius: 50%;
            border: 1px solid $nhsuk-black;
            color: black;
            line-height: 2px;
            width: 30px;
            height: 30px;
            text-align: center;
            display: inline-block;
    
    
            .annotation-item-circle-number {
                font-size: small;
                color: $nhsuk-black;
                margin-top: -10px;
            }
        }
    
        .not-selected-mark-path {
            opacity: 0.2;
        }
    
        .selected-mark-path {
            opacity: 1;
        }
    
        .svg-mark-container {
            width: 100%;
            height: 100%;
            display: block;
            pointer-events: auto;
        }
    
        .svg-mark-container-no-events {
            width: 100%;
            height: 100%;
            display: block;
            pointer-events: none;
        }
    
        /* FOO */
        .foog {
            stroke-linecap: round;
            stroke: $nhsuk-blue;
            stroke-width: 14;
            opacity: 0.4;
        }
    
        .foogborder {
            stroke: $nhsuk-grey-light;
            stroke-width: 7;
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
            pointer-events: none;
        }
    
        .fooginner {
            stroke: white;
            stroke-width: 3;
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
            pointer-events: none;
        }
    
        /* BAR */
        .barg .bar {
            stroke: rgba(255, 0, 0, 0.1);
            stroke: transparent;
            stroke-width: 50;
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
            fill: none;
        }
    
        .bar {
            stroke-linecap: round;
            stroke: $nhsuk-blue;
            stroke-width: 14;
        }
    
        .barborder {
            stroke: $nhsuk-grey-light;
            stroke-width: 7;
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
            fill: none;
            pointer-events: none;
        }
    
        .barinner {
            stroke-width: 3;
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
            fill: none;
            pointer-events: none;
        }
    
    }
</style>
