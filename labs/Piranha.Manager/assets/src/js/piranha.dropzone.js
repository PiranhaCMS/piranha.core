// Prevent Dropzone from auto discoveringr all elements:
Dropzone.autoDiscover = false;

/*global
    piranha
*/

piranha.dropzone = new function () {
    var self = this;

    self.mergeBaseOptions = function (options) {
        if (!options) options = {};
        
        var config = {
            paramName: 'Uploads',
            url: piranha.baseUrl + "manager/api/media/upload",
            uploadMultiple: true,
            init: function () {
                var self = this;

                // Default addedfile callback
                if (!options.addedfile) {
                    options.addedfile = function (file) { }
                }

                // Default removedfile callback
                if (!options.removedfile) {
                    options.removedfile = function (file) { }
                }

                // Default error callback
                if (!options.error) {
                    options.error = function (file) { }
                }

                // Default complete callback
                if (!options.complete) {
                    options.complete = function (file) {
                        console.log(file)
                        if (file.status !== "success" && file.xhr.responseText !== "" ) {
                            var response = JSON.parse(file.xhr.responseText);
                            file.previewElement.querySelector("[data-dz-errormessage]").innerText = response.body;
                        }
                    }
                }

                // Default queuecomplete callback
                if (!options.queuecomplete) {
                    options.queuecomplete = function () {}
                }            
        
                self.on("addedfile", options.addedfile);
                self.on("removedfile", options.removedfile);
                self.on("complete", options.complete);
                self.on("queuecomplete", options.queuecomplete);
            }
        };

        return Object.assign(config, options);
    }

    self.initList = function (selector, options) {
        if (!options) options = {};

        var config = {
            thumbnailWidth: 70,
            thumbnailHeight: 70,
            previewsContainer: selector + " .media-list",
            previewTemplate: document.querySelector( "#media-upload-template").innerHTML
        };

        var listOptions = self.mergeBaseOptions(config);
        
        return new Dropzone(selector + " form", Object.assign(listOptions, options));
    }
    
    self.initThumbnail = function (selector, options) {
        if (!options) options = {};

        var config = {
            thumbnailWidth: 184,
            thumbnailHeight: 130,
            previewsContainer: selector + " .file-list",
            previewTemplate: document.querySelector( "#file-upload-template").innerHTML
        };    

        var thumbOptions = self.mergeBaseOptions(config);
        
        return new Dropzone(selector + " form", Object.assign(thumbOptions, options));
    }   
};