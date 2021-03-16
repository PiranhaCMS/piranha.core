/*global
    piranha
*/

piranha.antiforgery = new function () {
    "use strict";

    var self = this;

    this.init = function () {
        if (document.getElementsByName('__RequestVerificationToken').length > 0)
            self.value = document.getElementsByName('__RequestVerificationToken')[0].value;
    };
};

$(document).ready(function () {
    piranha.antiforgery.init();
});