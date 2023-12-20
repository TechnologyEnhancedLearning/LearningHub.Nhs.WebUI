import 'babel-polyfill'; // For IE11 support.
import Vue from 'vue'
import Vuetify from 'vuetify'
 
Vue.use(Vuetify)
new Vue({
    el: '#header',
    data: {
        searchtext: '',
        focus: false
    },
    methods: {
        inputFocus: function () {
            this.focus = true; 
        },
        inputBlur: function () {
            this.focus = false;
        },
        enterSearch: function (keycode: number) {
            if (keycode === 13) {
                let searchText = (<HTMLInputElement>document.getElementById('input-search-md')).value;
                searchText = this.encodeProblemCharacters(searchText);
                if (searchText) {
                    window.location.href = '/search/index/' + encodeURIComponent(searchText);
                } else {
                    (<HTMLInputElement>document.getElementById('input-search-md')).focus();
                }
            }
        },
        encodeProblemCharacters: function (searchText: string) {
            return searchText.split("%").join('&percnt;')
                .split('"').join('&quot;')
                .split('+').join('&plus;')
                .split('/').join('&sol;')
                .split('\\').join('&bsol;');
        },
        showBookmarkPanel(){
            $('#manageBookmarksPanel').modal().show();
            $('#hdnBookmarkManager').click();
        }
    }
})
