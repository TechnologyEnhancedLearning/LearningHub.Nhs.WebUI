<template>
    <svg class="undo-button" width="30" height="30" viewBox="0 0 30 30" fill="none"
         xmlns="http://www.w3.org/2000/svg" v-on:click="undo">

        <circle cx="15" cy="15" r="14.5" fill="white" stroke="#AEB7BD"/>
        <g clip-path="url(#clip0_511_9870)">
            <path
                d="M22.7315 23.4399C22.7315 23.4399 23.7146 20.345 23.4515 17.9424C23.178 15.4463 22.0391 13.0853 18.4041 12.1125C16.2404 11.5344 13.4327 11.5275 13.4327 11.5275V8.60213C13.4327 8.60213 13.4708 8.30787 13.2493 8.12785C12.8858 7.83012 12.5326 8.1417 12.5326 8.1417L6.18348 13.1199C6.18348 13.1199 6 13.2757 6 13.4973C6 13.8227 6.27349 13.9508 6.27349 13.9508L12.5776 18.8771C12.5776 18.8771 12.8373 19.0606 13.0831 18.9429C13.3289 18.8252 13.4085 18.5413 13.4085 18.5413V15.474C13.4085 15.474 18.4283 15.2871 20.6093 17.0734C23.5 19.4379 22.7315 23.4399 22.7315 23.4399Z"
                fill="#005EB8"/>
        </g>
        <defs>
            <clipPath id="clip0_511_9870">
                <rect width="17.5" height="15.4402" fill="white" transform="translate(6 8)"/>
            </clipPath>
        </defs>
    </svg>
</template>

<script lang="ts">
    import Vue from 'vue';
    
    export default Vue.extend({
        methods: {
            undo() {
                this.$emit("undo");
            }
        }
    });
</script>
