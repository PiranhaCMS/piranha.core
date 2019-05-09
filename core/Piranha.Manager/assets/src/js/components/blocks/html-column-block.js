/*global
    piranha
*/

Vue.component("html-column-block", {
    props: ["uid", "model"],
    data: function () {
        return {
            column1: this.model.column1.value,
            column2: this.model.column2.value,
        };
    },
    methods: {
        onBlurCol1: function (e) {
            this.model.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.model.column2.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty1: function () {
            return piranha.utils.isEmptyHtml(this.model.column1.value);
        },
        isEmpty2: function () {
            return piranha.utils.isEmptyHtml(this.model.column2.value);
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid + 1);
        piranha.editor.addInline(this.uid + 2);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid + 1);
        piranha.editor.remove(this.uid + 2);
    },
    template:
        "<div class='block-body' class='row'>" +
        "  <div class='col-md-6'>" +
        "    <div :class='{ empty: isEmpty1 }'>" +
        "      <div :id='uid + 1' contenteditable='true' spellcheck='false' v-html='column1' v-on:blur='onBlurCol1'></div>" +
        "    </div>" +
        "  </div>" +
        "  <div class='col-md-6'>" +
        "    <div :class='{ empty: isEmpty2 }'>" +
        "      <div :id='uid + 2' contenteditable='true' spellcheck='false' v-html='column2' v-on:blur='onBlurCol2'></div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
