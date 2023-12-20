import type {Config} from '@jest/types';

export default async (): Promise<Config.InitialOptions> => {
    return {
        moduleFileExtensions: [
            "js",
            "ts",
            "json",
            "vue",
        ],
        testRegex: "(/__tests__/.*|(\\.|/)(test|spec))\\.(jsx?|tsx?)$",
        transform: {
            ".*\\.(vue)$": "vue-jest",
            "^.+\\.tsx?$": "ts-jest"
        },
        testURL: "http://localhost/",
        testEnvironment: "jsdom",
    };
};