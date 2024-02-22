export const setResourceCetificateLink = function (resourceReferenceId: string) {

    document.querySelector(".certificateNotification").remove();
    var form = document.createElement("form");
    form.setAttribute("method", "get");
    form.setAttribute("action", "/MyLearning/Certificate");

    // Create an input element for ResourceReferenceId
    var rr = document.createElement("input");
    rr.setAttribute("type", "hidden");
    rr.setAttribute("name", "ResourceReferenceId");
    rr.setAttribute("value", resourceReferenceId);

    // create a submit button
    var s = document.createElement("input");
    s.setAttribute("type", "submit");
    s.setAttribute("formtarget", "_blank");
    s.setAttribute("value", "View Certificate");
    s.setAttribute("class", "nhsuk-button nhsuk-button--secondary nhsuk-u-margin-top-1");

    // Append the form element to the form
    form.appendChild(rr);
    form.appendChild(s);
    document.querySelector(".certificateDownload").appendChild(form);
};