import Vue from 'vue'
import { setResourceCetificateLink } from './resource/helpers/resourceCertificateHelper';
import { resourceData } from './data/resource';

var certificateCheck = new Vue({
    el: '#resourcePg',
    data: {
        rr: 0,
    },
    methods: {
        async checkUserCertificateAvailability(resourceReference: number) {
            const targetElement = document.querySelector(".certificateNotification");
            if (targetElement) {
                this.rr = resourceReference;
                this.delay(3000).then(any => {
                    this.checkCert();
                });
            }
        },
        async checkCert(): Promise<void> {
            let certificateStatus = await resourceData.userHasResourceCertificate(this.rr);
            if (certificateStatus == true) {
                setResourceCetificateLink(this.rr.toString());
            }
        },
        async delay(ms: number) {
            await new Promise<void>(resolve => setTimeout(() => resolve(), ms));
        }
    }
   
});