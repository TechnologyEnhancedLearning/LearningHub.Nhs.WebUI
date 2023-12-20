function LogError(Message, URL) {
    var data = { Message: Message, URL: URL };

    console.log('Error: Message' + Message + ', URL: ' + URL);

    //$.ajax({
    //    cache: false,
    //    async: true,
    //    type: "POST",
    //    url: "/Error/LogJavaScriptError",
    //    data: data
    //});
}
