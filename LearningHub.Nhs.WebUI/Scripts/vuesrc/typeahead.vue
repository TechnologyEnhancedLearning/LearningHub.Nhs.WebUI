<template>
    <div class="lh-typeahead nhsuk-u-width-one-half" v-click-outside="hideList">
        <i class="fa fa-spinner fa-spin" v-if="loading"></i>
        <i class="fa fa-times" v-show="!loading && isDirty" @click="reset"></i>
        <input type="text"
               class="nhsuk-input"
               v-bind:class="inputClass"
               :placeholder="placeholderText"
               :aria-labelledby="labelText"
               :id="labelText"
               :name="labelText"
               autocomplete="off"
               v-model="query"
               @keydown.down="down"
               @keydown.up="up"
               @keydown.enter.prevent="hit"
               @keydown.esc="reset"
               @input="update"               
               @focus="showList" />

        <ul v-show="hasItems && !listHidden">
            <li v-for="(item, $item) in items" :class="activeClass($item)" @mousedown="hit" @mousemove="setActive($item)">
                <div v-html="highlight(item[displayProperty])" class="nhsuk-body-m"></div>
            </li>
        </ul>
        <ul v-show="noMatchFound && !listHidden">
            <li>
                <div class="nhsuk-body-m">No matches found</div>
            </li>
        </ul>
    </div>
</template>
<script lang="ts">
    import Vue from 'vue';    
    import ClickOutside from 'vue-click-outside';

    export default Vue.extend({  
        directives: {
            ClickOutside
        },
        props: {
            value: String,
            data: Array,
            placeholderText: String,
            labelText: String,
            inputClass: String,
            displayProperty: {
                default: 'name',
                type: String
            },
            selectionDisplayProperty: {
                default: '',
                type: String
            },
            loading: {
                default: false,
                type: Boolean
            },
            internalFilter: Boolean,
            minQueryLength: {
                default: 2,
                type: Number
            }
        },
        data() {
            return {
                current: -1,
                query: '',
                listHidden: false,
            };
        },
        methods: {
            up: function up() {
                if (this.current > 0) {
                    this.current--;
                } else if (this.current === -1) {
                    this.current = this.items.length - 1;
                } else {
                    this.current = -1;
                }
            },
            down: function down() {
                if (this.current < this.items.length - 1) {
                    this.current++;
                } else {
                    this.current = -1;
                }
            },
            reset: function reset() {
                this.query = '';
                this.$emit('input', this.query);
            },
            showList: function hideList() {
                this.listHidden = false;
            },
            hideList: function hideList() {           
                this.listHidden = true;
            },
            update() {
                this.listHidden = false;
                this.$emit('input', this.query);
            },
            hit: function hit() {
                if (this.current !== -1) {
                    this.onHit(this.items[this.current]);
                }
            },
            activeClass: function activeClass(index: number) {
                return {
                    active: this.current === index
                };
            },
            setActive: function setActive(index: number) {
                this.current = index;
            },
            onHit: function onHit(selectedItem: any) {
                this.listHidden = true;
                if (!this.selectionDisplayProperty) {
                    this.query = selectedItem[this.displayProperty];
                } else {
                    this.query = selectedItem[this.selectionDisplayProperty];
                }
                 this.$emit('hit', selectedItem);
            },
            highlight: function highlight(textValue: string) {
                let pos = textValue.toLowerCase().indexOf(this.query.toLowerCase());
                if (pos > -1) {
                    return textValue.substring(0, pos) + '<span class="highlighted">' + textValue.substring(pos, pos + this.query.length) + '</span>' + textValue.substring(pos + this.query.length);
                } else {
                    return textValue;
                }

            }          
        },
        computed: {
            items: function items(): any[] {
                if (!this.data || this.data.length==0) {
                    return [];
                } else {
                    if (!this.query || !this.internalFilter) {
                        return this.data;
                    } else {
                        let query = this.query;
                        let displayProperty = this.displayProperty;
                        return this.data.filter(function (itm: any) {
                            return itm[displayProperty].toLowerCase().indexOf(query.toLowerCase()) > -1;
                        });
                    }
                }
            },          
            hasItems: function hasItems(): boolean {
                return this.items.length > 0;
            },
            isEmpty: function isEmpty(): boolean {
                return !this.query;
            },
            isDirty: function isDirty(): boolean {
                return !!this.query;
            },
            noMatchFound: function(): boolean {
                return !this.hasItems && this.query && (this.query.length >= this.minQueryLength) && !this.loading;
            }
        },
        created() {
            if (this.value) {
                this.query = this.value;
                this.listHidden = true;
            }
        },
        watch: {
            value: function (val) {
                if (this.query != val) {
                    this.query = val;
                    this.listHidden = true;
                }
            }
        }

    })
</script>
