/*global
    piranha, summernote
*/

var ImageButton = function (context) {
    var ui = $.summernote.ui;

    // create button
    var button = ui.button({
        contents: '<i class="note-icon-picture"/>',
        tooltip: 'Image',
        click: function () {
            context.invoke('saveRange');
            piranha.mediapicker.open(function (img) {
                context.invoke('restoreRange');
                context.invoke('insertImage', img.publicUrl, img.filename);
            }, 'image');
        }
    });

    return button.render();
}

//
// Create a new inline editor
//
piranha.editor.addInline = function (id, toolbarId) {
    $("#" + id).summernote({
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['media', ['piranhaimage']],
            ['view', ['fullscreen', 'codeview']],
        ],
        buttons: {
            piranhaimage: ImageButton
        },
        popover: {
            image: [],
            link: [],
            air: []
        }
    });
};

//
// Remove the editor instance with the given id.
//
piranha.editor.remove = function (id) {
    $("#" + id).summernote("destroy");
};
