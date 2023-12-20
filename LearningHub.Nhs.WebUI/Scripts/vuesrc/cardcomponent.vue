<template>
    <div class="mx-2 mx-md-4">
        <div class="panel-card" :style="getBackground()" v-bind:class="extendedClasses">

            <div class="h-100 d-flex flex-column" v-on:click="expand">
                <h2 v-bind:class="textClass" class="nhsuk-heading-xs mx-4">{{ carditem.title | truncate(50, '...') }}</h2>
                <div v-bind:class="textClass" class="mx-4 small">{{ carditem.time}}  {{carditem.resourceTypeDescription}} </div>
                <div v-bind:class="textClass" class="mx-4 small">{{carditem.publishedBy}}</div>
                <div v-bind:class="textClass" class="mt-auto mx-auto align-self-center w-100">
                    <div class="p-0 m-0 small"><hr v-bind:class="ruleClass" /></div>
                    <div class="text-center"><i v-bind:class="textClass" class="fa-solid fa-chevron-down"></i></div>
                </div>
            </div>
        </div>
        <div v-show="this.expanded == true" class="arrow-up m-auto"></div>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { TrayCardModel } from './models/trayCardModel';

    export default Vue.extend({
        name: 'cardcomp',
        props: {
            carditem: { type: Object } as PropOptions<TrayCardModel>
        },
        data() {
            return {
                expanded: false
            };
        },
        computed: {
            textClass(): any {
                return {
                    backgroundImageText: this.carditem.backgroundImage
                }
            },
            ruleClass(): any {
                return {
                    backgroundImageRule: this.carditem.backgroundImage
                }
            },
            extendedClasses(): any {
                return {
                    'cardShadow': this.expanded
                }
            }
        },
        methods: {
            getBackground(): string {
                if (this.carditem.backgroundImage)
                    return "background-image: linear-gradient(0deg, #000000 0%, rgba(0,0,0,0.56) 100%), url('" + this.carditem.backgroundImage + "'); background-size: cover";
                else
                    return "";
            },
            expand(): void {
                if (this.expanded == false) {
                    this.expanded = true;
                    this.$emit('expand', this.carditem.id);

                }
                else {
                    this.$emit('collapse');
                    this.expanded = false;
                }
            },
            collapse(): void {
                this.expanded = false;
                $('#' + this.carditem.id.toString()).removeAttr('height');
                $(".panel-card").matchHeight();
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../Styles/abstracts/all" as *;

    .arrow-up {
        width: 0;
        height: 0;
        padding-top: 5px;
        border-left: 10px solid transparent;
        border-right: 10px solid transparent;
        border-bottom: 10px solid $nhsuk-grey-light;
    }

    .heading-xs {
        font-size: 1.8rem;
    }

    .bottom {
        position: absolute;
        bottom: 5px;
    }

    .cardShadow {
        box-shadow: 0px 6px 6px #dedfe1;
    }

    .backgroundImageText {
        color: $nhsuk-white !important;
    }

    .backgroundImageRule {
        border-color: $nhsuk-white !important;
    }
</style>