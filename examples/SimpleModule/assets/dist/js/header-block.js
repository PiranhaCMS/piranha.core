/*global
    piranha
*/

Vue.component("header-block", {
    props: ["uid", "model"],
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerHTML;

            // Tell parent that title has been updated
            var title = this.model.body.value.replace(/(<([^>]+)>)/ig, "");
            if (title.length > 40) {
                title = title.substring(0, 40) + "...";
            }

            this.$emit('update-title', {
                uid: this.uid,
                title: title
            });
        }
    },
    template:
        "<div class='block-body'>" +
        "  <h1 contenteditable='true' v-model='model.body.value' v-on:blur='onBlur'>Header</h1>" +
        "</div>"
});
