<template>
    <div class="mx-2 mx-md-4">
        <div style="background-image: linear-gradient(0deg, #000000 0%, rgba(0,0,0,0.56) 100%), url('/images/stat-and-mand.png'); background-size: cover" class="panel-card">

            <div class="h-100 d-flex flex-column">
                <h2 class="nhsuk-heading-xs my-auto mx-4 header text-center">View all your Learning Hub activity</h2>
                <div class="mx-4 small"> </div>
                <div class="mx-4 small"></div>
                <div class="mt-auto mx-auto align-self-center w-100">
                    <div class="pb-5 text-center"><button class="button-card">My Records</button></div>
                </div>
            </div>
        </div>
    </div>

</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        name: 'cardheadercomp',
        data() {
            return {
                expanded: false,
            };
        },
        methods: {
            navigate() {
              // placeholder for main button logic
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../Styles/abstracts/all" as *;

    .header {
        color: $nhsuk-white;
        font-family: FrutigerLT55Roman;
    }
    .button-card {
        background-color: $nhsuk-blue; /* nhs blue */
        color: $nhsuk-white;
        font-size: 19px;
        text-align: center;
        border-radius: 6px;
        width: 250px;
        height: 48px;
        border: 0;
        font-family: FrutigerLT55Roman;
    }
    .arrow-up {
        width: 0;
        height: 0;
        padding-top: 5px;
        border-left: 10px solid transparent;
        border-right: 10px solid transparent;
        border-bottom: 10px solid white;
        box-shadow: 0px 6px 6px #dedfe1;
    }

    .heading-xs {
        font-size: 1.8rem;
    }

    .panel-card.collapsed {
        max-height: 0px;
    }

    .bottom {
        position: absolute;
        bottom: 5px;
    }
</style>