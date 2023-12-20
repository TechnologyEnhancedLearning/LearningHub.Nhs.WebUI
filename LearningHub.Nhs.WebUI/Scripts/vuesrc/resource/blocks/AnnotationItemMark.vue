<template>
    <div class="annotation-mark"
         :class="markIsSelected ? 'selected-mark' : ''">
        <div class="annotation-mark-details">
            <div class="annotation-mark-details-image">
                <svg ref="icon">
                </svg>
            </div>
            <div v-if="!editingMarkLabel"
                 class="annotation-mark-details-title"
                 @click="toggleThisSelectedMark">
                {{ annotationMark.markLabel ? annotationMark.markLabel : "Freehand Mark" + (index + 1) }}
            </div>
            <div class="annotation-mark-details-title-edit"
                 v-else>
                <CharacterCountWithSaveCancelButtons v-model="annotationMark.markLabel"
                                                     :characterLimit="60"
                                                     :size="fontSize"
                                                     @close="finishRenamingMark"/>
            </div>
        </div>
        <div v-if="!editingMarkLabel && isEditable"
             class="annotation-mark-details-options">
            <AnnotationMarkOptions :show-menu="markOptionOpen"
                                   @markOptionsClicked="markOptionsClicked"
                                   @renameMark="renameMark"
                                   @deleteMark="deleteMark"/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from "vue";
    import { ImageAnnotationMarkModel } from "../../models/contribute-resource/blocks/annotations/imageAnnotationMarkModel";
    import { ImageAnnotationMarkFreehandShapeData } from "../../models/contribute-resource/blocks/annotations/imageAnnotationMarkFreehandShapeData";
    import AnnotationMarkOptions from "./AnnotationMarkOptions.vue";
    import EditSaveFieldWithCharacterCount from "../../globalcomponents/EditSaveFieldWithCharacterCount.vue";
    import CharacterCountWithSaveCancelButtons from "../../globalcomponents/CharacterCountWithSaveCancelButtons.vue";
    import { ImageAnnotationColourEnum } from "../../models/contribute-resource/blocks/annotations/imageAnnotationColourEnum";
    import { PathCoordinates } from "../../models/contribute-resource/blocks/annotations/pathCoordinates";

    const SVG_NS = 'http://www.w3.org/2000/svg';

    export default Vue.extend({
        components: { CharacterCountWithSaveCancelButtons, EditSaveFieldWithCharacterCount, AnnotationMarkOptions },
        props: {
            annotationMark: { type: Object } as PropOptions<ImageAnnotationMarkModel>,
            index: Number,
            markOptionOpen: Boolean,
            isEditable: Boolean,
            annotationColour: Number,
            markIsSelected: Boolean,
        },
        data() {
            return {
                minX: null,
                minY: null,
                maxX: null,
                maxY: null,
                scaleFactor: 20,
                circleSize: 30,
                editingMarkLabel: false,
                fontSize: "small",
            };
        },
        mounted() {
            this.createIconCircle();
            this.createPathImage();
        },
        watch: {
            annotationMark: {
                handler() {
                    this.createIconCircle();
                    this.createPathImage();
                },
                deep: true
            },
            annotationColour: function () {
                this.createIconCircle();
                this.createPathImage();
            },
            markIsSelected: {
                handler() {
                    if (!this.markIsSelected) {
                        this.editingMarkLabel = false;
                    }
                }
            }
        },
        methods: {
            createPathImage() {
                let svgIcon = this.$refs.icon as any;
                if (svgIcon) {
                    let shapeData = this.annotationMark.freehandMarkShapeData;
                    let lineColour = this.setLineColour(this.annotationColour);

                    if (shapeData?.pathCoordinates?.length != 1) {
                        let path = document.createElementNS(SVG_NS, 'path');
                        path.setAttribute('stroke', lineColour);
                        path.setAttribute('stroke-width', '2');
                        path.setAttribute('vector-effect', 'non-scaling-stroke');
                        path.setAttribute('fill', 'none');

                        let borderPath = document.createElementNS(SVG_NS, 'path');
                        borderPath.setAttribute('stroke', 'grey');
                        borderPath.setAttribute('stroke-width', '3');
                        borderPath.setAttribute('vector-effect', 'non-scaling-stroke');
                        borderPath.setAttribute('fill', 'none');

                        let d = this.createIconPathDAttribute(shapeData);

                        path.setAttribute("d", d);
                        borderPath.setAttribute("d", d);
                        svgIcon.appendChild(borderPath);
                        svgIcon.appendChild(path);
                        this.resetValues();
                    }
                }
            },
            setLineColour(colourEnum: ImageAnnotationColourEnum): string {
                switch (colourEnum) {
                    case ImageAnnotationColourEnum.Black:
                        return "black";
                    case ImageAnnotationColourEnum.White:
                        return "white";
                    case ImageAnnotationColourEnum.Red:
                        return "red";
                    case ImageAnnotationColourEnum.Green:
                        return "green";
                    default:
                        return "black";
                }
            },
            createIconPathDAttribute(shapeData: any): string {
                this.normaliseMarkShape(shapeData);

                // 0.15 ~ is approximately correct for the offset to be within the circle always
                let offset = 0.15 * this.circleSize;

                let diffX = this.maxX - this.minX;
                let diffY = this.maxY - this.minY;

                let d = '';
                for (let i = 0; i < shapeData.pathCoordinates.length; i++) {
                    let coords = shapeData.pathCoordinates[i];
                    let command = (i === 0) ? 'M' : 'L';

                    // Scale the coordinate such that a smaller version of the mark can be given within the circle icon
                    let xCoordinate = (Math.round(coords.x - this.minX) / diffX * this.scaleFactor) + offset;
                    let yCoordinate = (Math.round(coords.y - this.minY) / diffY * this.scaleFactor) + offset;

                    // Set path coordinate to the calculated coordinate, or if NaN, set the coordinate to the middle of the icon.
                    d += `${ command } ${ xCoordinate ? xCoordinate : this.circleSize / 2 } ${ yCoordinate ? yCoordinate : this.circleSize / 2 }`;
                }

                return d;
            },
            createIconCircle() {
                let svgIcon = this.$refs.icon as SVGElement;

                while (svgIcon.lastChild) {
                    svgIcon.removeChild(svgIcon.lastChild);
                }

                if (svgIcon) {
                    let circle = document.createElementNS(SVG_NS, 'circle');
                    let circleRadius = (this.circleSize / 2).toString();

                    circle.setAttribute('stroke', 'black');
                    circle.setAttribute('stroke-width', '1');
                    circle.setAttribute('vector-effect', 'non-scaling-stroke');
                    circle.setAttribute('fill', 'white');
                    circle.setAttribute('cx', circleRadius);
                    circle.setAttribute('cy', circleRadius);
                    circle.setAttribute('r', circleRadius);

                    svgIcon.appendChild(circle);
                }
            },
            normaliseMarkShape(shapeData: ImageAnnotationMarkFreehandShapeData): void {
                shapeData.pathCoordinates.forEach((coords) => {
                    this.setInitialCoordinates(coords);

                    if (coords.x < this.minX) {
                        this.minX = coords.x;
                    }
                    if (coords.x > this.maxX) {
                        this.maxX = coords.x;
                    }
                    if (coords.y < this.minY) {
                        this.minY = coords.y;
                    }
                    if (coords.y > this.maxY) {
                        this.maxY = coords.y;
                    }
                });
            },
            setInitialCoordinates(coords: PathCoordinates) {
                if (this.maxX === null) {
                    this.maxX = coords.x;
                }
                if (this.minX === null) {
                    this.minX = coords.x;
                }
                if (this.minY === null) {
                    this.minY = coords.y;
                }
                if (this.maxY === null) {
                    this.maxY = coords.y;
                }
            },
            resetValues() {
                this.maxX = null;
                this.maxY = null;
                this.minX = null;
                this.minY = null;
            },
            markOptionsClicked() {
                this.$emit('markOptionsClicked', this.index);
            },
            renameMark() {
                this.editingMarkLabel = true;
                this.$emit('renameMark', this.index);
            },
            deleteMark() {
                this.$emit('deleteMark', this.index);
            },
            finishRenamingMark() {
                this.editingMarkLabel = false;
            },
            toggleThisSelectedMark() {
                if (this.markIsSelected) {
                    this.$emit('selectAMark', -1);
                } else {
                    this.$emit('selectAMark', this.index);
                }
            }
        }
    });
</script>

<style lang="scss"
       scoped>
    @use "../../../../Styles/abstracts/all" as *;

    .annotation-mark {
        width: 100%;
        margin: 5px;
        display: flex;
        border-left: 7px solid transparent;
        justify-content: space-between;
        font-size: 14px;

        .annotation-mark-details {
            display: inline-flex;

            .annotation-mark-details-image {
                margin: 4px;
                width: 30px;
                height: 30px;
                flex: 0 0 auto;

                svg {
                    width: 100%;
                    height: 100%;
                }
            }

            .annotation-mark-details-title {
                display: flex;
                flex: 1 1 auto;
                align-items: center;
                margin: 4px;
            }

            .annotation-mark-details-title-edit {
                flex: 1 1 auto;
                padding: 5px;
            }
        }

        .annotation-mark-details-options {
            display: flex;
            align-items: center;
            margin: 4px;
        }
    }

    .selected-mark {
        background-color: $nhsuk-grey-lighter;
        border-left: 7px solid $nhsuk-blue;
    }
</style>