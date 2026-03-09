/*global
    piranha
*/

piranha.archivepicker = new Vue({
    el: "#archivepicker",
    data: {
        search: '',
        sites: [],
        items: [],
        currentSiteId: null,
        currentSiteTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            var self = this;

            return this.items.filter(function (item) {
                if (self.search.length > 0) {
                    return item.title.toLowerCase().indexOf(self.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId) {
            var url = piranha.baseUrl + "manager/api/page/archives" + (siteId ? "/" + siteId : "");
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentSiteId = result.siteId;
                    self.currentSiteTitle = result.siteTitle;
                    self.sites = result.sites;
                    self.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(piranha.archivepicker.currentSiteId);
        },
        open: function (callback, siteId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId);

            $("#archivepicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#archivepicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#archivepicker").on("shown.bs.modal", function() {
        $("#archivepickerSearch").trigger("focus");
    });
});