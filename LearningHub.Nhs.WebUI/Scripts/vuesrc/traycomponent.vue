

<template>
    <div class="tray pb-5">
        <div v-if="cardscount > 0" class="row pt-4 mb-3 pt-md-5 mb-md-5 trayheader d-flex align-items-center">
            <div class="col-xs-8 col-sm-8 col-md-6 trayheader-col"><h1 class="nhsuk-heading-l" id="traytitle">My {{ title }}</h1></div>
            <div class="col-xs-4 col-sm-4 col-md-6 small text-right d-sm-none d-md-block" v-if="cardscount > 0" >{{ getPagingText(currentcardsstartnumber, currentcardsendnumber ) }}  of {{ cardscount }} {{ title.toLowerCase()  }}</div>
        </div>
        <div v-if="cardscount > 0">
            <carousel class="carousel" ref="carousel" @pageChange="onCarouselPageChange" :navigationPrevLabel='`<i class="fa-lg fa-solid fa-chevron-left"></i>`' :navigationNextLabel='`<i class="fa-lg fa-solid fa-chevron-right"></i>`' @mounted="onCarouselMounted" :paginationEnabled="false" :scrollPerPage="true" :navigationEnabled="true" :navigationClickTargetSize="8" :perPageCustom="[[380, 1], [768, 2], [992, 3], [1280, 4], [1920, 6]]">
                <slide>
                    <cardheadercomp></cardheadercomp>
                </slide>
                <slide v-for="item in cards" ref="slides" :key="item.id">
                    <cardcomp @expand="onExpand" @collapse="onCollapse" :ref="item.id" v-bind:carditem="item"></cardcomp>
                </slide>
            </carousel>
        </div>
        <transition name="fade">
            <expandedcardcomp @collapse="onCollapse" ref="expandedCard" :showActionPanel="false"></expandedcardcomp>
        </transition>
        <div class="norecords" v-if="cardscount == 0">
            This is the placeholder text for a tray with no items
        </div>
        <video id="azureMediaPlayer" class="azuremediaplayer amp-default-skin amp-big-play-centered resource resource-video" style="display:none;">
            <p class="amp-no-js">
                To view this video please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video
            </p>
        </video>
    </div>

</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import { Carousel, Slide } from 'vue-carousel';
    import cardcomp from './cardcomponent.vue'
    import cardheadercomp from './cardheadercomponent.vue'
    import expandedcardcomp from './expandedcardcomponent.vue'
    import { TrayCardModel } from './models/trayCardModel';

    var cardsdict = {
        "380": 1,
        "768": 2,
        "992": 3,
        "1280": 4,
        "1920": 6
    };

    export default Vue.extend({
        name: 'traycomp',
        data() {
            return {
                currentindex: 0,
                slideby: 6,
                currentcards: null,
                expanded: false,
                expandedCardId: null as number,
                currentcardsstartnumber: 0,
                currentcardsendnumber: 6,
            };
        },
        props: {
            title: { Type: String, required: true } as PropOptions<string>,
            cards: { Type: Object, required: true } as PropOptions<TrayCardModel[]>
        },
        components: {
            'cardcomp': cardcomp,
            'cardheadercomp': cardheadercomp,
            'expandedcardcomp': expandedcardcomp,
            Carousel,
            Slide
        },
        watch: {
            compoundProperty: function () {
                if (this.cards) {
                    this.currentcards = this.cards.slice(this.currentindex, this.currentindex + this.slideby);
                }
            }          
        },
        computed: {
            compoundProperty(): any {
                // it's only required to reference those properties
                this.cards;
                this.currentindex;
                this.slideby;
                // and then return a different value every time
                return Date.now() // or performance.now()
            },            
            cardscount(): number {
                if (this.cards && this.cards.length)
                    return this.cards.length
                else
                    return 0;
            },            
            showprevious(): boolean {                
                return this.currentindex > 0;                
            },
            shownext(): boolean {
                return this.currentindex + this.slideby <= this.lastindex;
            },
            lastindex(): number {
                return this.cardscount - 1;
            },
            carouselcurrentcards(): number {
                if (this.$refs.carousel) {
                    const breakpointArray = (this.$refs.carousel as any).perPageCustom;
                    const width = (this.$refs.carousel as any).browserWidth;
                    const breakpoints = breakpointArray.sort(
                        (a: any, b: any) => (a[0] > b[0] ? -1 : 1)
                    );
                    // Reduce the breakpoints to entries where the width is in range
                    // The breakpoint arrays are formatted as [widthToMatch, numberOfSlides]
                    const matches = breakpoints.filter((breakpoint: any) => width >= breakpoint[0]);
                    // If there is a match, the result should return only
                    // the slide count from the first matching breakpoint
                    const match = matches[0] && matches[0][1];
                    return match || (this.$refs.carousel as any).perPage
                }
                return 0;
            },
            currentPage(): number {
                return this.$refs.carousel ? (this.$refs.carousel as any).currentPage : 0
            }
        },
        methods: {
            onCarouselMounted(): void {
                this.updateTrayCount(0);
            },
            onCarouselPageChange(currentPage: number): void {
                this.collapse();
                this.updateTrayCount(currentPage);               
            },
            onExpand: function (cardId: number): void {
                if (this.expandedCardId == cardId) {
                    this.collapse();
                }
                else {
                    this.collapse();
                    var cardToExpand = this.getCardById(cardId);
                    this.expanded = true;
                    (this.$refs.expandedCard as any).expand(cardToExpand);
                    this.expandedCardId = cardToExpand.id;                  
                }
            },
            onCollapse() {
                this.collapse();
            },
            updateTrayCount(currentPage: number) {
                var endNum = (currentPage * this.carouselcurrentcards) + this.carouselcurrentcards;
                var adjustmentOffset = 0;
                if (endNum > this.cards.length) {
                    adjustmentOffset = endNum - this.cards.length;
                    endNum = this.cards.length;
                }

               this.currentcardsstartnumber = (currentPage * this.carouselcurrentcards) + 1 - adjustmentOffset;
               this.currentcardsendnumber = endNum;
            },
            collapse() {
                if (this.expanded) {
                    ((this.$refs[this.expandedCardId] as any)[0] as any).expanded = false;
                }
                (this.$refs.expandedCard as any).expanded = false;
                this.expanded = false;
                this.expandedCardId = null;
            },
            getCardById(cardId: number) {
                return this.cards.filter(function (item) {
                    return item.id == cardId;
                })[0];
            },
            handleResize() {
                this.setSlideBy();
                this.setcurrentIndex();
                this.updateTrayCount(0);
            },
            next() {
                this.collapse();
                var newindex = this.currentindex + this.slideby;

                if (newindex > this.lastindex)
                    this.currentindex = this.lastindex - this.slideby;
                else
                    this.currentindex = newindex;

                this.setcurrentIndex();


            },
            previous() {
                this.collapse();
                var newindex = this.currentindex - this.slideby;

                // set new index to zero if less than zero
                if (newindex < 0)
                    this.currentindex = 0;
                else
                    this.currentindex = newindex;

            },
            setcurrentIndex() {

                var indexoutby = 0;

                if (this.cards) {
                    indexoutby = this.currentindex + this.slideby - this.cards.length;

                   if (indexoutby > 0)
                        this.currentindex = this.currentindex - indexoutby;               
                }
            },
            setSlideBy(): void {

                var width = window.innerWidth;

                switch (true) {
                    case width > 1440:
                        this.slideby = cardsdict["1920"];
                        break;
                    case width > 992:
                        this.slideby = cardsdict["1280"];
                        break;
                    case width > 768:
                        this.slideby = cardsdict["992"];
                        break;
                    case width > 380:
                        this.slideby = cardsdict["768"];
                        break;
                    default:
                        this.slideby = cardsdict["380"];
                        break;
                }
            },
            getPagingText(startnumber: number, endnumber: number) {
                if (startnumber == endnumber)
                    return startnumber;
                else
                    return startnumber + ' to ' + endnumber;
            }           
        },
        created() {
            window.addEventListener('resize', this.handleResize);
            this.setSlideBy();
        },
        updated: function () {
            $(".panel-card").matchHeight();
        }
    })
</script>


<style lang="scss" scoped>
    @use "../../Styles/abstracts/all" as *;

    .carousel, .trayheader {
        width: 98%;
        margin: 0 auto;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity .5s;
    }

    .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
        opacity: 0;
    } 

     @media (max-width: 768px) {

        .trayheader-col {
            padding-left: 1%;
        }
    }


</style>

