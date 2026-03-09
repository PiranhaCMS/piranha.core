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
        currentGroupIcon: null,
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
        bind: function (result, partial) {
            this.currentGroupId = result.group.id;
            this.currentGroupTitle = result.group.title;
            this.currentGroupIcon = result.group.icon;
            this.types = result.types;
            this.items = result.items.map(function (i) {
                var type = result.types.find(function (t) {
                    return t.id === i.typeId;
                });

                i.type = type.title || i.typeId;

                return i;
            });

            if (!partial)
            {
                // Only bind groups if this is a full reload
                this.groups = result.groups;
            }
        },
        load: function (groupId, partial) {
            var url = piranha.baseUrl + "manager/api/content/" + (groupId ? groupId + "/" : "") + "list";
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result, partial);
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        loadGroups: function () {

        },
        refresh: function () {
            this.load(piranha.contentpicker.currentGroupId, true);
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
    $("#contentpicker").on("shown.bs.modal", function() {
        $("#contentpickerSearch").trigger("focus");
    });
});