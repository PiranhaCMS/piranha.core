/*global
    piranha
*/

piranha.resources = new function() {
    "use strict";

    var self = this;

    this.texts = {};

    this.init = function (texts)
    {
        self.texts = texts;
    };
};