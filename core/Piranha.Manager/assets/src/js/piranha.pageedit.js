/*global
    piranha
*/

piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        loading: true,
        id: null,
        siteId: null,
        parentId: null,
        sortOrder: 0,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaKeywords: null,
        metaDescription: null,
        published: null,
        state: "new",
        blocks: [],
        regions: [],
        selectedRegion: {
            uid: "uid-blocks",
            name: null,
            icon: null,
        },
        selectedSetting: "uid-settings"
    },
    computed: {
        contentRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display != "setting";
            });
        },
        settingRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display === "setting";
            });
        },
    },
    methods: {
        bind: function (model) {
            this.id = model.id;
            this.siteId = model.siteId;
            this.parentId = model.parentId;
            this.sortOrder = model.sortOrder;
            this.typeId = model.typeId;
            this.title = model.title;
            this.navigationTitle = model.navigationTitle;
            this.slug = model.slug;
            this.metaKeywords = model.metaKeywords;
            this.metaDescription = model.metaDescription;
            this.published = model.published;
            this.state = model.state;
            this.blocks = model.blocks;
            this.regions = model.regions;
        },
        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        create: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/create/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        save: function ()
        {
            this.saveInternal(piranha.baseUrl + "manager/api/page/save");
        },
        saveDraft: function ()
        {
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/draft");
        },
        unpublish: function ()
        {
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/unpublish");
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: piranha.pageedit.id,
                siteId: piranha.pageedit.siteId,
                parentId: piranha.pageedit.parentId,
                sortOrder: piranha.pageedit.sortOrder,
                typeId: piranha.pageedit.typeId,
                title: piranha.pageedit.title,
                navigationTitle: piranha.pageedit.navigationTitle,
                slug: piranha.pageedit.slug,
                metaKeywords: piranha.pageedit.metaKeywords,
                metaDescription: piranha.pageedit.metaDescription,
                published: piranha.pageedit.published,
                blocks: JSON.parse(JSON.stringify(piranha.pageedit.blocks)),
                regions: JSON.parse(JSON.stringify(piranha.pageedit.regions))
            };

            fetch(route, {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.slug = result.slug;
                self.published = result.published;
                self.state = result.state;

                piranha.notifications.push(result.status);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        revert: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/revert/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        remove: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/delete/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.notifications.push(result);

                    window.location = piranha.baseUrl + "manager/pages";
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        addBlock: function (type, pos) {
            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.blocks.splice(pos, 0, result.body);
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
        },
        selectRegion: function (region) {
            this.selectedRegion = region;
        },
        selectSetting: function (uid) {
            this.selectedSetting = uid;
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.loading)
        {
            sortable(".blocks", {
                handle: ".handle",
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
            });
        }
        else {
            sortable(".blocks", "disable");
            sortable(".blocks", "enable");
        }

        this.loading = false;
    }
});
