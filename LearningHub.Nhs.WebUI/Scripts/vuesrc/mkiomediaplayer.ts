import { MKPlayer, MKPlayerConfig } from '@mediakind/mkplayer';

interface ClearKeyConfig {
    LA_URL: string;
    headers: {
        Authorization: string;
    };
}

interface PlayerConfig {
    key: string;
    ui: boolean;
    theme: string;
    playback: {
        muted: boolean;
        autoplay: boolean;
        preferredTech: Array<{ player: string; streaming: string }>;
    };
    events: {
        ready: () => void;
        subtitleadded: () => void;
        // Add other event handlers as needed
    };
}

interface SourceConfig {
    enableLowLatency: boolean;
    hls: string;
    drm: {
        clearkey: ClearKeyConfig;
    };
}

function getBearerToken(): string {
    // Replace this with your actual logic to get the bearer token
    return "your_bearer_token_here";
}

function getClearKeyConfig(): ClearKeyConfig {
    return {
        LA_URL: "HLS_AES",
        headers: {
            Authorization: getBearerToken()
        }
    };
}

function getPlayerConfig(mkioKey: string, onPlayerReady: () => void, onSubtitleAdded: () => void): PlayerConfig {
    return {
        key: mkioKey,
        ui: true,
        theme: "dark",
        playback: {
            muted: false,
            autoplay: false,
            preferredTech: [{ player: "Html5", streaming: "Hls" }] // Adjust these strings if you have specific types
        },
        events: {
            ready: onPlayerReady,
            subtitleadded: onSubtitleAdded
        }
    };
}

function initializePlayer(videoContainer: HTMLElement, playerConfig: MKPlayerConfig, playBackUrl: string): any {
    const player = new MKPlayer(videoContainer, playerConfig);
    const sourceConfig: SourceConfig = {
        enableLowLatency: true,
        hls: playBackUrl,
        drm: {
            clearkey: getClearKeyConfig()
        }
    };

    player.load(sourceConfig)
        .then(() => {
            console.log("Source loaded successfully!");
        })
        .catch(() => {
            console.error("An error occurred while loading the source!");
        });

    return player;
}

export { getPlayerConfig, initializePlayer };
