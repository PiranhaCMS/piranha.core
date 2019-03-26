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
                .catch(function (error) { console.log("error:", error );
            });
        },
        remove: function (id) {
            console.log("Remove page: ", id);
        },
        addBlock: function (type, pos) {
            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    console.log("result: ", result);
                    if (pos) {
                        piranha.pageedit.blocks.splice(pos, 0, result);
                    } else {
                        piranha.pageedit.blocks.push(result);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        }
    },
    created: function () {
    },
    updated: function () {
    }
});
