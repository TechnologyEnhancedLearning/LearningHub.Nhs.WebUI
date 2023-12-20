<template>
    <div class="contribute-title-component lh-padding-fluid">
        <div class="lh-container-xl pt-30 pb-20">

            <div class="d-flex flex-wrap justify-content-between align-items-center mx-n12 my-n2">
                <h1 class="nhsuk-heading-xl">
                    <span v-if="!editing" class="contribute-title-component-title">{{ title || 'Untitled' }}</span>
                    <input type="text" v-if="editing" v-model="title" placeholder="Untitled" ref="contributeTitleInput" />
                </h1>
                <LinkTextAndIcon v-if="!editing"
                                 v-on:click="editing = true"
                                 class="mx-12 my-2"
                                 iconClasses="fa-solid fa-pencil">
                    Edit title
                </LinkTextAndIcon>
                <Button color="blue"
                        class="mx-12 my-2"
                        v-if="editing"
                        v-on:click="save">
                    save title
                </Button>
            </div>

        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';

    import Button from '../globalcomponents/Button.vue';
    import LinkTextAndIcon from '../globalcomponents/LinkTextAndIcon.vue';

    import { ContributeResourceDetailModel } from '../models/contribute/contributeResourceModel';

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>,
        },
        components: {
            Button,
            LinkTextAndIcon,
        },
        data() {
            return {
                title: this.resourceDetails.title,
                editing: false,
            };
        },
        updated() {
            if (this.$refs.contributeTitleInput) {
                (this.$refs.contributeTitleInput as HTMLElement).focus();
            }
        },
        methods: {
            save() {
                if (this.resourceDetails.title !== this.title) {
                    this.resourceDetails.title = this.title;
                }
                this.editing = false;
            }
        },
    });
</script>

<style lang="scss" scoped>
    @use '../../../Styles/abstracts/all' as *;

    .contribute-title-component {
        background-color: $nhsuk-white;
    }

    .contribute-title-component .contribute-title-component-title {
        display: inline-block;
        margin: 2px 2px;
    }
    @media (max-width: 768px) {
        .contribute-title-component .contribute-title-component-title {
            margin: 3px 2px;
        }
    }

    .contribute-title-component input {
        height: initial !important;
        width: 100%;
    }

    .contribute-title-component .text-decoration-underline {
        text-decoration: underline;
    }
</style>