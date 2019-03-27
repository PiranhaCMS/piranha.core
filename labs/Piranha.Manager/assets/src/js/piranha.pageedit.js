piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        loading: true,
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
                    if (pos) {
                        piranha.pageedit.blocks.splice(pos, 0, result);
                    } else {
                        piranha.pageedit.blocks.push(result);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        moveBlock: function (from, to) {
            this.blocks.splice(to, 0, this.blocks.splice(from, 1)[0])
        },
        removeBlock: function (block) {
            var index = this.blocks.indexOf(block);

            if (index !== -1) {
                this.blocks.splice(index, 1);
            }
        }
    },
    created: function () {
    },
    updated: function () {
        sortable(".blocks", {
            handle: ".handle",
            items: ":not(.unsortable)"
        })[0].addEventListener("sortupdate", function (e) {
            piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
        });

        this.loading = false;
    }
});
