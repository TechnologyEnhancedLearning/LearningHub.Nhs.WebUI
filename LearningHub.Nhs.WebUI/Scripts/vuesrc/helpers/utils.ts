export function isIncludedInListIgnoringCase(list: string[], item: string): boolean {
    return list.some(i => i.toLowerCase() === item.toLowerCase());
}
