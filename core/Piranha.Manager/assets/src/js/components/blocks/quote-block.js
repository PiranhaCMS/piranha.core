/*global
    piranha
*/

Vue.component("quote-block", {
    props: ["model"],
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerText;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.model.body.value);
        }
    },
    template:
        "<div class='block-body' :class='{ empty: isEmpty }'>" +
        "  <i class='fas fa-quote-right quote'></i>" +
        "  <p class='lead' contenteditable='true' spellcheck='false' v-html='model.body.value' v-on:blur='onBlur'></pre>" +
        "</div>"
});
