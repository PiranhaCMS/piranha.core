/*global
    piranha
*/

Vue.component("text-block", {
    props: ["block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.block.body.value);
        }
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <pre contenteditable='true' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></pre>" +
        "</div>"
});
