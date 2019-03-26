Vue.component("html-column-block", {
    props: ["gid", "block"],
    methods: {
        onBlurCol1: function (e) {
            this.block.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.block.column2.value = e.target.innerHTML;
        }
    },
    mounted: function () {
        piranha.editor.inline("#" + this.gid + 1);
        piranha.editor.inline("#" + this.gid + 2);
    },
    template:
        "<div class='row'>" +
        "  <div :id='gid + 1' class='col-md-6'>" +
        "    <div contenteditable='true' spellcheck='false' v-html='block.column1.value' v-on:blur='onBlurCol1'></div>" +
        "  </div>" +
        "  <div :id='gid + 2' class='col-md-6'>" +
        "    <div contenteditable='true' spellcheck='false' v-html='block.column2.value' v-on:blur='onBlurCol2'></div>" +
        "  </div>" +
        "</div>"
});
