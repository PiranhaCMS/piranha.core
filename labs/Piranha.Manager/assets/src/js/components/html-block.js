Vue.component("html-block", {
    props: ["block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    template:
        "<div contenteditable='true' v-html='block.body.value' v-on:blur='onBlur'></div>"
});
