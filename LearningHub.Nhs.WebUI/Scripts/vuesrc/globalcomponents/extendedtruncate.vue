 <template>
     <span v-html="htmltext"></span>   
</template>

<script lang="ts">  
    import Vue, { PropOptions } from 'vue';

    export default Vue.extend({
        name: 'extendedtruncate',        
        props: {
            text: { Type: String, required: true } as PropOptions<string>,
            length: { Type: Number, required: true } as PropOptions<number>,
            href: { Type: String, required: false } as PropOptions<string>,
            suffix: { Type: String, required: false } as PropOptions<string>,
        },
        computed: {
            htmltext(): string {
                if (this.text.length <= length) { return this.text; }
                var subString = this.text.substring(0, this.length - 1);
                return subString.substring(0, subString.lastIndexOf(' ')) +  ' <a href="' + this.href + '">' + this.suffix + '</a>';                
            }
        }
    })
</script>



