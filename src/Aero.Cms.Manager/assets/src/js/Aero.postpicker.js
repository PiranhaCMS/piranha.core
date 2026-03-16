/*global
    Aero
*/

Aero.postpicker = new Vue({
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
                if (Aero.postpicker.search.length > 0) {
                    return post.title.toLowerCase().indexOf(Aero.postpicker.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId, archiveId) {
            var url = Aero.baseUrl + "manager/api/post/modal";

            if (siteId) {
                url += "?siteId=" + siteId;
                if (archiveId) {
                    url += "&archiveId=" + archiveId;
                }
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    Aero.postpicker.sites = result.sites;
                    Aero.postpicker.archives = result.archives;
                    Aero.postpicker.posts = result.posts;

                    Aero.postpicker.currentSiteId = result.siteId;
                    Aero.postpicker.currentArchiveId = result.archiveId;

                    Aero.postpicker.currentSiteTitle = result.siteTitle;
                    Aero.postpicker.currentArchiveTitle = result.archiveTitle;
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