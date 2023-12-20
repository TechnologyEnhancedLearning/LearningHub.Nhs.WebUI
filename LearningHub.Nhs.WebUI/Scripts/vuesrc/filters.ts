import Vue from 'vue'
import moment from 'moment'

Vue.filter('formatDate', function (value: string , format: string) {
    if (value) {
        return moment(String(value)).format(format)
    }
    return value;
});

Vue.filter('currency', function (value: string) {
    return '£' + parseFloat(value).toFixed(2);
});

Vue.filter('truncate', function (text: string, length: number, suffix: string) {
    if (text.length <= length) { return text; }
    var subString = text.substring(0, length - 1);
    return subString.substring(0, subString.lastIndexOf(' ')) + suffix;
});