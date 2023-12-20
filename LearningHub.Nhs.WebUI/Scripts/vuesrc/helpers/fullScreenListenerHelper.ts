export function addListenerToDocument(fullscreenChangeEventListener : (element: string) => () => void ): void {
    document.addEventListener('fullscreenchange', fullscreenChangeEventListener('fullscreenElement'));
    document.addEventListener('MSFullscreenChange', fullscreenChangeEventListener('msFullscreenElement')); /* IE11 */
    document.addEventListener('webkitfullscreenchange', fullscreenChangeEventListener('webkitFullscreenElement')); /* Safari */
}

export async function requestFullscreen(element: any): Promise<void> {
    if (element.requestFullscreen) {
        await element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) { /* Safari */
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) { /* IE11 */
        element.msRequestFullscreen();
    }
}

export async function exitFullscreen(): Promise<void> {
    if (document.exitFullscreen) {
        await document.exitFullscreen();
    } else if ((document as any).webkitExitFullscreen) { /* Safari */
        (document as any).webkitExitFullscreen();
    } else if ((document as any).msExitFullscreen) { /* IE11 */
        (document as any).msExitFullscreen();
    }
}

export const isNotFullscreenMode = function (): boolean {
    return !document.fullscreenElement && !(document as any).webkitFullscreenElement && !(document as any).msFullscreenElement;
}