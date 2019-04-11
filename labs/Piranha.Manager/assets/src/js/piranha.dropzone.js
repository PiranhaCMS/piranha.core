// Prevent Dropzone from auto discoveringr all elements:
Dropzone.autoDiscover = false;

/*global
    piranha
*/

piranha.dropzone = new function () {
    var self = this;

    self.init = function (selector, options) {
        if (!options) options = {};
        var config = {
            paramName: 'Uploads',
            url: piranha.baseUrl + "manager/api/media/upload",
            uploadMultiple: true,
            thumbnailWidth: 184,
            thumbnailHeight: 130,
            previewsContainer: selector + " .file-container",
            previewTemplate : document.querySelector('#tpl').innerHTML,
            init: function () {
                this.on("complete", function (file) {
                    console.log("default complete", file)
                });
                this.on("queuecomplete", function (file) {
                    console.log("default queuecomplete", file)
                });
            }
        };
        
        return new Dropzone(selector, Object.assign(config, options));
    };
};