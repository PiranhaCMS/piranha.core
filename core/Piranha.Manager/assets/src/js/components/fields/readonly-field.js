Vue.component("readonly-field", {
    props: ["uid", "model", "meta"],
    template:
        "<div class='alert alert-secondary mb-0'>" +
        "  <pre class='mb-0'>{{ model.value }}</pre>" +
        "</div>"
});
