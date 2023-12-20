<template>
        <div class="contribute-header fullwidth">
            <div class="row">
                <div class="col-12">
                    <nav class="subnavbarwhite navbar-expand-md navbar-toggleable-md">
                        <!-- Currently shows desktop navbar in all screen sizes as there is only one tab/screen. 
                             If a mobile verison of the navbar is required in the future, I suggest copying in the <div class="bar"> element from MyContributions.cshtml.
                             There is no white and blue styling for a mobile version of this navbar either. -->
                        <div class="navbar-collapse collapse" id="collapsingSubNavbar">
                            <ul class="navbar-nav">
                                <router-link :to="'/activity'" v-slot="{ href, route, navigate, isActive, isExactActive }">
                                    <li class="subnavwhite-item" v-bind:class="[isActive && 'active', isExactActive && 'router-link-exact-active']">
                                        <a :href="href" v-on:click="navigate" class="subnavwhite-link" data-toggle="collapse" data-target=".navbar-collapse.show">Itemised activity</a>
                                    </li>
                                </router-link>
                            </ul>
                        </div>
                    </nav>
                </div>
            </div>
        </div>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        name: 'myLearningHeaderComponent',
        data: function () {
            return {
                menuName: '',
            }
        }
    })
</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

</style>
