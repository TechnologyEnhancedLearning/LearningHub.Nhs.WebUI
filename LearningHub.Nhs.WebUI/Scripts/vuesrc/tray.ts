import Vue from 'vue'
import Vuetify from 'vuetify'
import AxiosWrapper from './axiosWrapper';
import traycomp from './traycomponent.vue'
import './filters'
import commoncomponents from './globalcomponents'

Vue.use(Vuetify)

commoncomponents.forEach(component => {
    Vue.component(component.name, component);
});

var trayapp = new Vue({
    el: '#activitytray',
    data: {
        title: "Resources",
        cards: null
    },
    components: {
        'traycomp': traycomp        
    },
    created() {
        AxiosWrapper.axios.get('/api/card/GetMyResourceCards')
            .then(response => {
                this.cards = response.data.cards;
            })
            .catch(e => {
                console.log(e);
            })
    }

});