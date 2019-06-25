/*global
    piranha
*/

piranha.pagepicker = new Vue({
    el: "#pagepicker",
    data: {
        search: '',
        items: [],
        currentSiteId: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            return this.items.filter(function (item) {
                if (piranha.pagepicker.search.length > 0) {
                    return item.title.toLowerCase().indexOf(piranha.pagepicker.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId) {
            var url = piranha.baseUrl + "manager/api/page/sitemap" + (siteId ? "/" + siteId : "");

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pagepicker.items = result;
                    piranha.pagepicker.currentSiteId = siteId;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            piranha.pagepicker.load(piranha.pagepicker.currentSiteId);
        },
        open: function (callback, siteId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId);

            $("#pagepicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1 && this.filteredFolders.length == 0) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#pagepicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#pagepicker").on("shown.bs.modal", function() {
        $("#pagepickerSearch").trigger("focus");
    });
});