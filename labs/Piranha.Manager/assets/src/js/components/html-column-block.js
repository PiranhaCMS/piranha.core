Vue.component("html-column-block", {
    props: ["block"],
    methods: {
        onBlurCol1: function (e) {
            this.block.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.block.column2.value = e.target.innerHTML;
        }
    },
    template:
        "<div class='row'>" +
        "  <div class='col-md-6'>" +
        "    <div contenteditable='true' v-html='block.column1.value' v-on:blur='onBlurCol1'></div>" +
        "  </div>" +
        "  <div class='col-md-6'>" +
        "    <div contenteditable='true' v-html='block.column2.value' v-on:blur='onBlurCol2'></div>" +
        "  </div>" +
        "</div>"
});
