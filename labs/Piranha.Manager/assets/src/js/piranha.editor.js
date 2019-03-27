/*global
    piranha
*/

piranha.editor = {
    addInline: function (id) {
        tinymce.init({
            selector: "#" + id,
            menubar: false,
            branding: false,
            statusbar: false,
            inline: true,
            convert_urls: false,
            plugins: [
                "autoresize autolink code hr paste lists" // TODO: piranhaimage piranhalink"
            ],
            width: "100%",
            autoresize_min_height: 0,
            toolbar: "bold italic | bullist numlist hr | alignleft aligncenter alignright | formatselect", // TODO: | piranhalink piranhaimage",
            block_formats: 'Paragraph=p;Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Code=pre;Quote=blockquote'
        });
    },
    remove: function (id) {
        tinymce.remove(tinymce.get(id));
    }
};