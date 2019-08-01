/*global
    piranha
*/

piranha.config = new Vue({
    el: "#config",
    data: {
        loading: true,
        model: {
            hierarchicalPageSlugs: null,
            expandedSitemapLevels: null,
            archivePageSize: null,
            pagesExpires: null,
            postsExpires: null,
            mediaCDN: null,
            pageRevisions: null,
            postRevisions: null
        }
    },
    methods: {
        load: function () {
            self = this;

            fetch(piranha.baseUrl + "manager/api/config")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.model.hierarchicalPageSlugs = result.hierarchicalPageSlugs;
                    self.model.expandedSitemapLevels = result.expandedSitemapLevels;
                    self.model.archivePageSize = result.archivePageSize;
                    self.model.pagesExpires = result.pagesExpires;
                    self.model.postsExpires = result.postsExpires;
                    self.model.mediaCDN = result.mediaCDN;
                    self.model.pageRevisions = result.pageRevisions;
                    self.model.postRevisions = result.postRevisions;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        save: function () {
            self = this;

            fetch(piranha.baseUrl + "manager/api/config/save", {
                    method: "post",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        hierarchicalPageSlugs: self.model.hierarchicalPageSlugs,
                        expandedSitemapLevels: self.model.expandedSitemapLevels,
                        archivePageSize: self.model.archivePageSize,
                        pagesExpires: self.model.pagesExpires,
                        postsExpires: self.model.postsExpires,
                        mediaCDN: self.model.mediaCDN,
                        pageRevisions: self.model.pageRevisions,
                        postRevisions: self.model.postRevisions
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
        }
    },
    created: function () {
        this.load();
    },
    updated: function () {
        this.loading = false;
    }
});
