/*global
    piranha, tinymce
*/

piranha.editor = {
    editors:[],

    addInline: function (id, toolbarId) {
        console.log("No HTML editor registered.")
    },
    addInlineMarkdown: function (id, value, update) {
        var preview = $("#" + id).parent().find(".markdown-preview");
        var simplemde = new SimpleMDE({
            element: document.getElementById(id),
            status: false,
            spellChecker: true,
            hideIcons: ["preview", "guide"],
            initialValue: value,
            toolbar: [
                "bold", "italic", "heading", "|", "quote", "unordered-list", "ordered-list", "|", "link",
                {
                    name: "image",
                    action: function customFunction(editor) {
                        piranha.mediapicker.openCurrentFolder(function(media) {
                            var cm = editor.codemirror;
                            var active = simplemde.getState(cm).image;

                            var startPoint = cm.getCursor("start");
                            var endPoint = cm.getCursor("end");

                            if (active) {
                                text = cm.getLine(startPoint.line);
                                cm.replaceRange("![" + media.filename + "](" + media.publicUrl + ")",
                                    {
                                        line: startPoint.line,
                                        ch: 0
                                    });
                            } else {
                                cm.replaceSelection("![" + media.filename + "](" + media.publicUrl + ")");
                            }
                            cm.setSelection(startPoint, endPoint);
                            cm.focus();
                        }, "Image");
                    },
                    className: "fa fa-picture-o",
                    title: "Image"
                },
                "side-by-side", "fullscreen"
            ],
            renderingConfig: {
                singleLineBreaks: false
            }
        });
        simplemde.codemirror.on("change", function() {
            preview.html(simplemde.markdown(simplemde.value()));
            update(simplemde.value());
        });
        setTimeout(function() {
            preview.html(simplemde.markdown(simplemde.value()));
            simplemde.codemirror.refresh();
        }.bind(simplemde), 0);

        this.editors[id] = simplemde;
    },
    remove: function (id) {
        console.log("No HTML editor registered.")
    },
    removeMarkdown: function (id) {
        var simplemde = this.editors[id];

        if (simplemde != null) {
            var index = this.editors.indexOf(simplemde);

            simplemde.toTextArea();
            this.editors.splice[index, 1];
        }
    },
    refreshMarkdown: function () {
        for (var key in this.editors) {
            if (this.editors.hasOwnProperty(key)) {
                this.editors[key].codemirror.refresh();
            }
        }
    }
};

$(document).on('focusin', function (e) {
    if ($(e.target).closest(".tox-tinymce-inline").length) {
        e.stopImmediatePropagation();
    }
});