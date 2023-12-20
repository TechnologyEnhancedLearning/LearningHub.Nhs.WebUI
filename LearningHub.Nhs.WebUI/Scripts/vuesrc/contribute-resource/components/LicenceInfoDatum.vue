<template>
    <li class="licence-info-datum d-flex align-items-center">
        <div class="licence-info-datum--icon">
            <i v-bind:class="getIconClass()" v-bind:aria-label="`Users ${isTicked ? trueValue : falseValue}`"></i>
        </div>
        <div class="licence-info-datum--description">{{description}}</div>
    </li>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        props: {
            isTicked: Boolean,
            trueValue: String,
            falseValue: String,
            description: String,
        },
        methods: {
            getIconClass(): string {
                return this.isTicked ? "fas fa-check" : "fas fa-times";
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;
    
    .licence-info-datum {
        margin: 4px 0;
        
        &--description {
            margin-left: 9px;
        }
        
        &--icon {
            height: 24px;
            width: 24px;
            display: flex;
            align-content: center;
            justify-content: center;
        }
    }
    
    i {
        font-size: 2.4rem;
        vertical-align: middle;

        &.fa-check {
            color: $nhsuk-green;
        }
        &.fa-times {
            color: $nhsuk-red;
        }
    }
</style>