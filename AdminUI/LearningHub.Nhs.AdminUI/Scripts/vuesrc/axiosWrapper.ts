import axios, { AxiosInstance } from "axios";

export class AxiosWrapper {

    public axios: AxiosInstance = axios.create();

    constructor() {
        const sessionManager = (window as any).LHGlobal.sessionManager;
        this.axios.interceptors.request.use(function (config) {
            sessionManager.reset();
            config.headers.common['x-requested-with'] = 'XMLHttpRequest';
            return config;
        });

        this.axios.interceptors.response.use((response) => {
            sessionManager.reset();
            return response;
        }, (error) => {
            sessionManager.reset();
            if (error.response && error.response.status === 401) {
                sessionManager.showMessage();
            }

            if (error.response && error.response.data) {
                return Promise.reject(error.response.data);
            }

            return Promise.reject(error.message);
        });
    }
}

const AxiosService = new AxiosWrapper();

export default AxiosService;