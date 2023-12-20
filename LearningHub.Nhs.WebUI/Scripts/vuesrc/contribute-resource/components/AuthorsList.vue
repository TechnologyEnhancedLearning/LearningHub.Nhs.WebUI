<template>
    <div class="authors-editor-container">
        <div class="author-block" v-for="author in resourceAuthors">
            <span class="author-description">
                {{ getAuthorDescription(author) }}
            </span>
            <button class="btn btn-link" aria-label="Delete author" v-on:click="deleteAuthor(author.id)">
                <i class="fas fa-times"></i>
            </button>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue, { PropOptions } from 'vue';
    import * as _ from "lodash";
    import { ContributeResourceDetailModel } from "../../models/contribute/contributeResourceModel";
    import { resourceData } from "../../data/resource";
    import { AuthorModel } from "../../models/contribute/authorModel";

    export default Vue.extend({
        props: {
            resourceDetails: { type: Object } as PropOptions<ContributeResourceDetailModel>
        },
        computed: {
            resourceAuthors(): AuthorModel[] {
                return this.resourceDetails.resourceAuthors;
            }
        },
        methods: {
            getAuthorDescription(author: AuthorModel): string {
                // Format: "Name, Organisation, Role". Anything falsey we can ignore.
                let descriptionParts = [author.authorName, author.organisation, author.role]
                return _.filter(descriptionParts, s => !!s).join(', ');
            },
            
            async deleteAuthor(authorId: number) {
                let response = await resourceData.deleteAuthor(this.resourceDetails.resourceVersionId, authorId);
                if (response){
                    this.resourceDetails.resourceAuthors = _.filter(this.resourceDetails.resourceAuthors, a => a.id !== authorId);
                }
            },
        }
    })
</script>

<style lang="scss" scoped>
    @use '../../../../Styles/abstracts/all' as *;

    .authors-editor-container {
        .author-block {
            display: flex;
            justify-content: space-between;
            align-items: center;
            box-sizing: content-box;
            margin: 12px 0;
            padding: 7px 8px 7px 17px;
            border: 1px solid $nhsuk-grey-light;
            border-radius: 6px;
            height: 40px;
            background-color: $nhsuk-white;
            white-space: nowrap;
            
            .author-description {
                text-overflow: ellipsis;
                overflow: hidden;
                font-size: 16px;
            }
            
            i {
                font-size: 1.8rem;
                color: $nhsuk-red;
                vertical-align: middle;
            }
        }
    }
</style>