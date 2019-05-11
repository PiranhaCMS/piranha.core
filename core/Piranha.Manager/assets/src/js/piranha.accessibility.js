/*global
    piranha
*/

piranha.accessibility = new function () {
    "use strict";

    var self = this;

    self.focus = null;

    //
    // Removes the currently focused block
    //
    self.removeBlock = function (e) {
        var block = $(":focus").parents(".block");

        if (block.length === 1) {
            // Check if we have an active editor
            if (tinymce) {
                var editor = tinymce.activeEditor;

                if (editor && editor.inline) {
                    editor.destroy();
                }
            }

            // Remove focused block
            block.find(".block-remove").click();
        }
    };

    //
    // Store focus poing when modal is opened.
    //
    $(document).on("show.bs.modal", ".modal", function() {
        self.focus = $(":focus");
    });

    //
    // Restore previous focus point when modal i closed.
    //
    $(document).on("hidden.bs.modal", ".modal", function() {
        if (self.focus) {
            self.focus.focus();
            self.focus = null;
        }
    });

    //
    // Keyboard Shortcuts
    //
    $(window).on("keydown", function (e) {
        var menu = $(".main-nav");

        // Function key
        if (e.altKey) {
            // Pressed 'c', toggle content switcher
            if (e.keyCode === 67) {
                $("#contentSelector").click();
            }
            // Pressed backspace, check for focused block
            else if (e.keyCode === 8) {
                self.removeBlock(e);
            }
        }
    });
}
