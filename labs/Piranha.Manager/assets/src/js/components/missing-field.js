Vue.component("missing-field", {
    props: ["model"],
    template:
        "<div class='alert alert-danger text-center' role='alert'>No component registered for <code>{{ model.type }}</code></div>"
});
