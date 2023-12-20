<template>
    <div class="white-background">
        <div class="lh-container-xl">            
            <div class="userProfile" v-if="user.firstName">
                <h1>{{ user.firstName }} {{ user.lastName}}</h1>
                <span v-if="!user.lastUpdated">Profile created on {{ user.lastUpdated | formatDate('DD MMM YYYY')}}</span>
                <span v-if="user.lastUpdated">Profile last updated on {{ user.lastUpdated | formatDate('DD MMM YYYY')}}</span>
            </div>
            <div class="userProfile" v-if="!user.firstName && localUser">
                <h1>{{ localUser.firstName }} {{ localUser.lastName}}</h1>
                <span v-if="!localUser.lastUpdated">Profile created on {{ localUser.lastUpdated | formatDate('DD MMM YYYY')}}</span>
                <span v-if="localUser.lastUpdated">Profile last updated on {{ localUser.lastUpdated | formatDate('DD MMM YYYY')}}</span>
            </div>
            <div v-show="!isMobile">
                <ul class="nav nav-pills">
                    <li class="nav-link">
                        <router-link to="/personaldetails" :class="activeClass('personaldetails')">Personal details</router-link>
                    </li>
                    <li class="nav-link">
                        <router-link to="/changepassword" :class="activeClass('changepassword')">Change password</router-link>
                    </li>
                    <li class="nav-link">
                        <router-link to="/additionalsecurity" :class="activeClass('additionalsecurity')">Additional security</router-link>
                    </li>
                </ul>
            </div>
            <div class="accordion" id="accordion" v-show="isMobile">
                <div>
                    <div class="heading" id="headingTwo">
                        <div @click="onToggle" class="collapsed mobile-nav-header" data-toggle="collapse" data-target="#profileMobNav" aria-expanded="false" aria-controls="profileMobNav">
                            <span class="menuHeaderText justify-content-start">{{menuHeader}}</span>
                            <span class="close" data-toggle="collapse" data-target="#collapsingProfileNavbar">
                                <i v-show="!isExpanded" class="fa-solid fa-chevron-down"></i>
                                <i v-show="isExpanded" class="fa-solid fa-xmark"></i>
                            </span>
                        </div>
                    </div>
                    <div id="profileMobNav" class="collapse" aria-labelledby="profileMobNav" data-parent="#accordion">
                        <div class="content">
                            <div class="mobile-nav-link">
                                <router-link to="/personaldetails">Personal details</router-link>
                            </div>
                            <div class="mobile-nav-link">
                                <router-link to="/changepassword">Change password</router-link>
                            </div>
                            <div class="mobile-nav-link">
                                <router-link to="/additionalsecurity">Additional security</router-link>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="tab-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import '../filters';
    import VueRouter from 'vue-router';
    import { UserBasicModel } from '../models/userBasicModel';
    import { userData } from '../data/user';
    import PersonalDetails from './personaldetails.vue';
    import ChangePassword from './changepassword.vue';
    import AdditionalSecurity from './additionalsecurity.vue';
    import { mapActions, mapGetters } from 'vuex';

    const router = new VueRouter({
        routes: [
            { path: '/personaldetails', name: 'personaldetails', component: PersonalDetails },
            { path: '/changepassword', name: 'changepassword', component: ChangePassword },
            { path: '/additionalsecurity', name: 'additionalsecurity', component: AdditionalSecurity, }
        ]
    });

    Vue.use(VueRouter);
    export default Vue.extend({
        router,
        components: {
        },
        props: {
        },
        data() {
            return {
                localUser: null,
                isExpanded: false,
                isMobile: false,
                menuHeader: 'Personal details'
            }
        },
        computed: {
            user(): UserBasicModel {                
                return this.$store.state.user;
            }
        },
        watch: {           
        },
        beforeDestroy() {
            if (typeof window !== 'undefined') {
                window.removeEventListener('resize', this.onResize, {});
            }
        },
        mounted() {
            this.onResize()
            window.addEventListener('resize', this.onResize, { passive: true });              
        },
        async created() {
            this.loadUserDetails();
        },
        methods: {
            onResize() {
                this.isMobile = window.innerWidth < 768;
            },
            onToggle() {
                this.isExpanded = !this.isExpanded;
            },           
            activeClass: function (...names: any) {
                for (let name of names) {
                    if (name == this.$route.name)
                        return 'active';
                }

                if (this.$route.name === 'personaldetails')
                    this.menuHeader = 'Personal details'
                else if (this.$route.name === 'changepassword')
                    this.menuHeader = 'Change password'
                else if (this.$route.name === 'additionalsecurity')
                    this.menuHeader = 'Additional security'

                return 'active'
            },
            async loadUserDetails() {
                await userData.getCurrentUserBasicDetails().then(response => {
                    this.localUser = response;
                    this.$store.commit('setUser', {
                        id: this.localUser.id,
                        userName: this.localUser.userName,
                        firstName: this.localUser.firstName,
                        lastName: this.localUser.lastName,
                        lastUpdated: this.localUser.lastUpdated,
                    }); 
                    this.$router.push('personaldetails').catch(e => { });
                });
            }
        },
    })

</script>

<style lang="scss" scoped>
    @use "../../../Styles/abstracts/all" as *;

    li {
        height:46px;
        padding-left:0px !important;
        a{
            text-decoration: none;
        }
        a.router-link-exact-active {
            padding-bottom: 10px;
            border-bottom: 6px solid #005EB8;
        }
    a {
        text-decoration: none;
        color: #212B32;
        font-size: 19px;
        line-height: 28px;
    }
    }
    @media (max-width: 768px) {
        li {
            padding-left: 15px !important;
        }
    }
    .nav-link {
        margin-right: 50px !important;
        justify-content: left !important;
    }

    .tab-content {
        border-top: 1px solid #AEB7BD;
    }
    .white-background {
        background-color: #ffffff;
    }
    .grey-background {
        background-color: #F0F4F5;
    }
    .userProfile {
        padding-top: 40px;
        margin-bottom: 40px;
    }

    @media(max-width: 1200px){
        .userProfile {
            padding-left: 4rem !important;
            padding-right: 4rem !important;
        }
    }
    @media (max-width: 768px) {
        .userProfile {
            padding-left: 15px !important;
            padding-right: 5px !important;
        }
    }
    .menuHeaderText {
        border-bottom: 6px solid #005EB8;
        padding-bottom:12px;
    }
    @media(max-width: 1200px) {
        .nav-pills{
            padding-left: 4rem;
            padding-right: 4rem;
        }
    }
    .mobile-nav-header {
        padding: 10px;
        border-top: 1px solid #AEB7BD;
        border-bottom: 1px solid #AEB7BD;
        height: 50px;
        font-weight: bold;
        font-size: 16px;
        line-height: 24px;
        i {
              font-size:20px;
          }
        @media(max-width: 768px){
            padding-left: 15px;
        }
    }

    .mobile-nav-link {
        padding: 10px;
        border-bottom: 1px solid #AEB7BD;
        font-size: 14px;
        line-height: 24px;

        a.router-link-exact-active {
            text-decoration: none;
            color: #212B32;
            cursor: default;
        }

        @media(max-width: 768px) {
            padding-left: 15px;
        }
    }
</style>
