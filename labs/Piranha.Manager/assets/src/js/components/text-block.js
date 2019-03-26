Vue.component("text-block", {
    props: ["block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    template:
        "<pre contenteditable='true' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></pre>"
});
