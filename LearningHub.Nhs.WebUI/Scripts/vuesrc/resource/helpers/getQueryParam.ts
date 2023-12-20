export const getQueryParam = (param: string) => {
    var results = new RegExp(`[\?&]${param}=([^&#]*)`)
        .exec(window.location.search);

    return results?.[1];
};
