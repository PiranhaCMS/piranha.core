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

Vue.component("image-block", {
    props: ["block"],
    template:
        "<div>" +
        "  <img :src='block.body.media.publicUrl.substring(1)'>" +
        "  <div class='media-picker'>" +
        "    <div class='btn-group float-right'>" +
        "      <button class='btn btn-primary text-center'>" +
        "        <i class='fas fa-plus'></i>" +
        "      </button>" +
        "      <button class='btn btn-danger text-center'>" +
        "        <i class='fas fa-times'></i>" +
        "      </button>" +
        "    </div>" +
        "    <div class='card text-left'>" +
        "      <div class='card-body'>" +
        "        <a href='#' v-on:click.prevent='piranha.preview.open(block.body.id)'>{{ block.body.media.filename }}</a>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

Vue.component("missing-block", {
    props: ["block"],
    template:
        "<div class='alert alert-danger' role='alert'>Missing Component</div>"
});

piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        id: null,
        siteId: null,
        parentId: null,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaKeywords: null,
        metaDescription: null,
        blocks: []
    },
    methods: {
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.id = result.id;
                    piranha.pageedit.siteId = result.siteId;
                    piranha.pageedit.parentId = result.parentId;
                    piranha.pageedit.typeId = result.typeId;
                    piranha.pageedit.title = result.title;
                    piranha.pageedit.navigationTitle = result.navigationTitle;
                    piranha.pageedit.slug = result.slug;
                    piranha.pageedit.metaKeywords = result.metaKeywords;
                    piranha.pageedit.metaDescription = result.metaDescription;
                    piranha.pageedit.blocks = result.blocks;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (id) {
            console.log("Remove page: ", id);
        }
    },
    created: function () {
    },
    updated: function () {
    }
});
