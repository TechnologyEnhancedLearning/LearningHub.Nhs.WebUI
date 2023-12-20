export const getNumberOfCharacters = (text:string) => text.replace(/\s\n\n/g, ' ').replace(/\n\n/g, ' ').replace(/\s\n/g, '').replace(/\n/g, '').length;

export const getRemainingCharacters = (maxLength: number, text: string) => maxLength - getNumberOfCharacters(text);

export const getNumberOfCharactersFromHtml = (data: string) => {
    const text = $("<p></p>").html(data).text();
    return getNumberOfCharacters(text);
}

export const getRemainingCharactersFromHtml = (maxLength: number, data: string) => maxLength - getNumberOfCharactersFromHtml(data);