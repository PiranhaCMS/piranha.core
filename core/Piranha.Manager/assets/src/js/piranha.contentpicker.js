/*global
    piranha
*/

piranha.contentpicker = new Vue({
    el: "#contentpicker",
    data: {
        search: '',
        groups: [],
        items: [],
        currentGroupId: null,
        currentGroupTitle: null,
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
        load: function (groupId) {
            var url = piranha.baseUrl + "manager/api/content/" + (groupId ? groupId + "/" : "") + "list";
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
        loadGroups: function () {

        },
        refresh: function () {
            this.load(piranha.contentpicker.currentGroupId);
        },
        open: function (groupId, callback) {
            this.search = '';
            this.callback = callback;

            this.load(groupId);

            $("#contentpicker").modal("show");
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

            $("#contentpicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#pagepicker").on("shown.bs.modal", function() {
        $("#contentpickerSearch").trigger("focus");
    });
});