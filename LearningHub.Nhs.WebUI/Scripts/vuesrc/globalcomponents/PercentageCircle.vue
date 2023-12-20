<!-- INSTRUCTIONS FOR USE: -->
<!--
    Use as per the examples below:

        Percentage circle: 60% fixed
        <PercentageCircle :percentage="60" :textSize="30" :text="`60%`" color="green"/>

        Percentage circle: set to percentage data
        <PercentageCircle :percentage="percentage" :textSize="30" :text="percentage + `%`" color="green"/>

        Percentage circle: set to score
        <PercentageCircle :percentage="100 * score / totalMarks" :textSize="20" :text="score + ` of ` + totalMarks"/>

    COLOURS:
        Blue
            color="blue"
        Green
            color="green"
        Red
            color="red"
        Orange
            color="orange"
        
        Default is blue
-->

<template>
    <svg :width="circleRadius * 2" :height="circleRadius * 2">
        <circle class="percentage-circle-background" :r="circleRadius" :cx="circleRadius" :cy="circleRadius"/>
        <circle class="percentage-circle-inner" :r="circleRadius - circleWidth" :cx="circleRadius" :cy="circleRadius"/>
        <circle ref="circle" fill="transparent" class="percentage-circle-outer" :transform="`rotate(-90 ${circleRadius} ${circleRadius})`"               
                v-bind:class="{
                    'blue': (!color || color === 'blue'),
                    'green': (color === 'green'),
                    'orange': (color === 'orange'),
                    'red': (color === 'red'),
                }"
                :stroke-width="circleWidth" :r="circleRadius - circleWidth / 2" :cx="circleRadius" :cy="circleRadius"/>
        <line :aria-hidden="hideLine" :class="hideLine ? `hidden` : ``" :x1="circleRadius" :y1="circleRadius" :x2="circleRadius" y2="0" stroke="white" stroke-width="2"/>
        <line :aria-hidden="hideLine" :class="`line ` + hideLine ? `hidden` : ``" :x1="circleRadius" :y1="circleRadius" :x2="circleRadius" y2="0" stroke="white" stroke-width="2" transform-origin="center" :transform="`rotate(` + clampedPercentage * 3.6 + `)`"/>
        <text :aria-hidden="!text" :class="`inner-text ` + text ? `` : `hidden`" :font-size="textSize" :fill="textColor" font-weight="bold" :x="circleRadius" :y="circleRadius" dy="0.35em" text-anchor="middle">{{ text }}</text>
    </svg>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import _ from 'lodash';

    export default Vue.extend({
        props: {
            hideLine: Boolean,
            percentage: Number,
            color: String,
            text: String,
            textSize: Number,
            circleRadius: { type: Number, default: 100 } as PropOptions<Number>,
            circleWidth: { type: Number, default: 50 } as PropOptions<Number>,
            textColor: { type: String, default: "black" } as PropOptions<String>
        },
        computed: {
            clampedPercentage(): number {
                return _.round(_.clamp(this.percentage, 0, 100));
            }
        },
        mounted() { 
            const circle = this.$refs.circle as SVGCircleElement;
            const radius = circle.r.baseVal.value;
            const circumference = radius * 2 * Math.PI;
            circle.style.strokeDasharray = `${circumference}, ${circumference}`;
            circle.style.strokeDashoffset = `${circumference}`;
            const offset = circumference - this.clampedPercentage / 100 * circumference;
            circle.style.strokeDashoffset = offset.toString();
        },
        watch: {
            percentage() {
                const circle = this.$refs.circle as SVGCircleElement;
                const radius = circle.r.baseVal.value;
                const circumference = radius * 2 * Math.PI;
                const offset = circumference - this.clampedPercentage / 100 * circumference;
                circle.style.strokeDashoffset = offset.toString();
            }
        }
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .percentage-circle-background {
        fill: $nhsuk-grey-light;
    }

    .percentage-circle-inner {
        fill: $nhsuk-white;
    }

    .percentage-circle-outer {
        transition: 0.7s stroke-dashoffset,
                    0.4s stroke;
    }

    .blue {
        stroke: $nhsuk-blue;
    }

    .green {
        stroke: $nhsuk-green;
    }

    .red {
        stroke: $nhsuk-red; 
    }

    .orange {
        stroke: $nhsuk-warm-yellow;
    }

    .inner-text {
        font: $font-stack-bold;
    }

    .line {
        transition: 0.7s transform;
    }
    
    .hidden {
        display: none;
    }

</style>