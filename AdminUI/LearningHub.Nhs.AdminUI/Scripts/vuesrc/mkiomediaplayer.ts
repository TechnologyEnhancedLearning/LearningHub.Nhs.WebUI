import { MKPlayer, MKPlayerConfig } from '@mediakind/mkplayer';
import { MKPlayerType, MKStreamType } from './MKPlayerConfigEnum';
interface ClearKeyConfig {
    LA_URL: string;
    headers: {
        Authorization: string;
    };
}

interface PlayerConfig {
    key: string;
    ui: boolean;
    playback: {
        muted: boolean;
        autoplay: boolean;
        preferredTech: Array<{ player: string; streaming: string }>;
    };
    theme: string;
    events: {
        ready: () => void;
    };
}

interface SourceConfig {
    enableLowLatency: boolean; 
    hls: string;
    drm: {
        clearkey: ClearKeyConfig;
    };
}

function getBearerToken(authenticationToken: string): string {
    // Replace this with your actual logic to get the bearer token
    return `Bearer ${authenticationToken}`;
}

function getPlayerConfig(
    mkioKey: string,
    onPlayerReady: () => void
): PlayerConfig {
    return {
        key: mkioKey,
        ui: true,
        playback: {
            muted: false,
            autoplay: false,
            preferredTech: [{ player: "Html5", streaming: "Hls" }] // Adjust these strings if you have specific types
        },
        theme: "dark",
        events: {
            ready: onPlayerReady,
        }
    };
}

function getSourceConfig(
    locatorUri: string,
    authenticationToken: string
): SourceConfig {
    return {
        enableLowLatency: true,
        hls: locatorUri,
        drm: {
            clearkey: {
                LA_URL: "HLS_AES",
                headers: {
                    Authorization: getBearerToken(authenticationToken)
                }
            }
        }
    };
}

function initializePlayer(videoContainer: HTMLElement, playerConfig: MKPlayerConfig, playBackUrl: string, bearerToken: string): any {
    const player = new MKPlayer(videoContainer, playerConfig);

    var clearKeyConfig = {
        //LA_URL: "https://ottapp-appgw-amp.prodc.mkio.tv3cloud.com/drm/clear-key?ownerUid=azuki",
        LA_URL: "HLS_AES",
        headers: {
            "Authorization": bearerToken
        }
    };

    const sourceConfig: SourceConfig = {
        enableLowLatency: true,
        hls: playBackUrl,
        drm: {
            clearkey: clearKeyConfig
        },
    };

    player.load(sourceConfig)
        .then(() => {
            console.log("Source loaded successfully!");
        })
        .catch(() => {
            console.error("An error occurred while loading the source!");
        });

    return player;
};

export { getPlayerConfig, getSourceConfig, initializePlayer };
