// TypeScript wrapper for the showMedia function located in wwwroot/js/mediaPlayerInit.js

interface ShowMediaInterface {
    (mediaProxyUri: string, detailsresourcePath: any): void;
}

interface ResizeAzureMediaWindowSizeInterface {
    (): void;
}

declare var showMedia: ShowMediaInterface;
declare var resizeAzureMediaWindowSize: ResizeAzureMediaWindowSizeInterface;
