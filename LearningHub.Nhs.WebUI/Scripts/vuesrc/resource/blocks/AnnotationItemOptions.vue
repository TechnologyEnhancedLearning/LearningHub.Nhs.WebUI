<template>
    <div class="options-and-menu">
        <div class="options">
            <button class="option-button" v-on:click="optionsClicked"> Options </button>
        </div>
        <div v-if='showMenu' class='menu' ref="annotationOptionsMenu">
            <div v-on:click="editClicked" class='menu-item'>
                Edit
            </div>
            <div v-if="!firstInList" v-on:click="moveUpClicked" class='menu-item'>
                Move Up
            </div>
            <div v-if="!lastInList" v-on:click="moveDownClicked" class='menu-item'>
                Move Down
            </div>
            <div v-on:click="deleteClicked" class='menu-item'>
                Delete
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import LinkTextAndIcon from '../../globalcomponents/LinkTextAndIcon.vue';
    
    export default Vue.extend({
        name: "AnnotationItemOptions",
        components:{
            LinkTextAndIcon,
        },
        props:{
            firstInList: Boolean,
            lastInList: Boolean,
            showMenu: Boolean,
        },
        watch: {
            showMenu: function(){
                if (this.showMenu){
                    let vue = this;
                    
                    Vue.nextTick(function () {
                        let element = vue.$refs["annotationOptionsMenu"] as Element;
                        if (element != null) {
                            // Scroll relevant item into view, then scroll the screen back to start
                            element.scrollIntoView({block: 'nearest', inline: 'nearest'});
                            let startElement = document.getElementById("annotation-edit-start");
                            if (startElement != null) {
                                startElement.scrollIntoView({behavior: 'smooth', block: 'start', inline: 'start'});
                            }
                        }
                    });
                }
            }
        },
        methods: {
            optionsClicked: function(){
                this.$emit('optionsClicked')
            },
            editClicked: function(){
                this.$emit('edit')
            },
            deleteClicked: function(){
                this.$emit('delete')
            },
            moveUpClicked: function(){
                this.$emit('moveUp')
            },
            moveDownClicked: function(){
                this.$emit('moveDown')
            },
        }
    });
</script>

<style lang="scss" scoped>
    @use "../../../../Styles/abstracts/all" as *;
    
    .options-and-menu{
        position: relative;
        
        .options{
            .option-button{
                color: $nhsuk-blue;
                outline: none;
                border: none;
                background-color: $nhsuk-white;
            }

            button:hover{
                text-decoration: underline;
            } 
        }

        .menu {
            width: 120px;
            right: 0;
            background-color: $nhsuk-white;
            background-clip: padding-box;
            border: 1px solid $nhsuk-grey-light;
            border-radius: .25rem;
            color: $nhsuk-black;
            cursor: pointer;
            display: flex;
            flex-direction: column;
            font-size: small;
            list-style: none;
            margin: .125rem 0 0;
            padding: .5rem 0;
            position: absolute;
            text-align: left;
            z-index: 100;

            .menu-item {
                color: $nhsuk-black;
                padding: .25rem 1.5rem;
                transition: color .15s ease-in-out,background-color .15s ease-in-out,border-color .15s ease-in-out,box-shadow .15s ease-in-out;
            }

            .menu-item:hover {
                background-color: $nhsuk-btn-outline-hover;
                cursor: pointer;
            }
        } 
    }    
</style>