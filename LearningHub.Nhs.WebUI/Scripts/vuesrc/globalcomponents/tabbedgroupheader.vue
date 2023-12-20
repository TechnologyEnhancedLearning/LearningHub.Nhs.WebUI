<template>
    <div class="tabbed-group-header">
        <ul>
            <li v-for="tab in tabs" :class="{active: isActive(tab)}">
                <router-link :to="tab.url" v-if="!isActive(tab)">
                    {{tab.name}}
                </router-link>
                <p v-if="isActive(tab)">{{tab.name}}</p>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    export default Vue.extend({
        name: 'TabbedGroupHeader',
        props: {
            tabs: Array,
            currentUrl: String
        },
        data() {
            return {
            }
        },
        computed: {
        },
        async created() {
        },
        async mounted() {
        },
        methods: {
            isActive: function (tab: any): Boolean {
                return this.currentUrl === tab.url
            }
        }
    })
</script>
<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    a{
        color: $nhsuk-black;
        text-decoration: none;
    }

    li.active{
        font-family: $font-stack-bold !important;
        border-bottom: 3px solid $nhsuk-blue;
    }

    ul{
        border-bottom: 1px solid $nhsuk-grey-white;
        display: flex;
        flex-direction: row;
        padding-left: 0;
    }
    li{
        list-style: none;
        margin-right: 24px;
        p{
             margin-bottom: 0;
        }
    }
</style>