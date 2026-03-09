/*global
    piranha
*/

piranha.postpicker = new Vue({
    el: "#postpicker",
    data: {
        search: '',
        sites: [],
        archives: [],
        posts: [],
        currentSiteId: null,
        currentArchiveId: null,
        currentSiteTitle: null,
        currentArchiveTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredPosts: function () {
            return this.posts.filter(function (post) {
                if (piranha.postpicker.search.length > 0) {
                    return post.title.toLowerCase().indexOf(piranha.postpicker.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId, archiveId) {
            var url = piranha.baseUrl + "manager/api/post/modal";

            if (siteId) {
                url += "?siteId=" + siteId;
                if (archiveId) {
                    url += "&archiveId=" + archiveId;
                }
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.postpicker.sites = result.sites;
                    piranha.postpicker.archives = result.archives;
                    piranha.postpicker.posts = result.posts;

                    piranha.postpicker.currentSiteId = result.siteId;
                    piranha.postpicker.currentArchiveId = result.archiveId;

                    piranha.postpicker.currentSiteTitle = result.siteTitle;
                    piranha.postpicker.currentArchiveTitle = result.archiveTitle;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(this.currentSiteId, this.currentArchiveId);
        },
        open: function (callback, siteId, archiveId, currentPostId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId, archiveId);

            $("#postpicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredPosts.length == 1) {
                this.select(this.filteredPosts[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#postpicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#postpicker").on("shown.bs.modal", function() {
        $("#postpickerSearch").trigger("focus");
    });
});