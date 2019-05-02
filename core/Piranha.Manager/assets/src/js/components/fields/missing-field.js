Vue.component("missing-field", {
    props: ["meta", "model"],
    template:
        "<div class='alert alert-danger text-center' role='alert'>No component registered for <code>{{ meta.type }}</code></div>"
});
