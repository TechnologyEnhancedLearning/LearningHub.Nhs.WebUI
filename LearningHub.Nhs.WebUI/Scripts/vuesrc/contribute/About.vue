<template>
    <div>
        <contributeHeaderComponent></contributeHeaderComponent>
        <form @submit.prevent="onSubmit">

            <div class="row ml-3 mt-5">
                <div class="form-group col-6">
                    <h3>About Placeholder</h3>

                </div>
            </div>

            <!--<div class="row ml-3 mt-5">
                <div class="form-group col-6">
                    <h3>Description<i v-if="$v.resourceDescription.$invalid" class="warningTriangle fa-solid fa-triangle-exclamation"></i></h3>
                    <div class="explanationText">Write a description that is easy to understand and describes the benefits of this resource to learners.</div>
                    <ckeditor :editor="editor" v-model="resourceDescription"></ckeditor>
                </div>
            </div>
            <div class="row ml-3">
                <div class="form-group col-6">
                    <button type="submit" class="btn btn-custom my-2">Next</button>
                </div>
            </div>-->

        </form>


    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { required } from "vuelidate/lib/validators";
    import contributeHeaderComponent from './HeaderComponent.vue';
    import Typeahead from  '../typeahead.vue';
    //import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    //import CKEditor from '@ckeditor/ckeditor5-vue';

    export default Vue.extend({
        components: {
            'contributeHeaderComponent': contributeHeaderComponent,
            Typeahead//,
            //ckeditor: CKEditor.component
        },
        data() {
            return {
                //editor: ClassicEditor
            };
        },
        computed: {
            resourceDescription: {
                get() {
                    //return this.$store.state.step2Data.resourceDescription;
                    return '';
                },
                set(value) {
                    
                }
            },
        },
        methods: {
            onSubmit() {
                if (this.$v.$invalid) {
                    alert('test');
                }
                else {
                    this.$router.push({ name: 'Legal' });
                }
            }
        },
        validations: {
            resourceDescription: {
                required
            }
        }
    })

</script>
