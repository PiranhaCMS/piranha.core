/*global
    piranha
*/

piranha.blockpicker = new Vue({
    el: "#blockpicker",
    data: {
        filter: "",
        categories: [],
        index: 0,
        callback: null
    },
    computed: {
        filteredCategories: function () {
            var self = this;
            return this.categories.filter(function (category) {
                var items = self.filterBlockTypes(category);

                if (items.length > 0) {
                    return true;
                }
                return false;
            });
        }
    },
    methods: {
        open: function (callback, index, parentType) {
            var self = this;

            var url = piranha.baseUrl + "manager/api/content/blocktypes";

            if (piranha.pageedit)
            {
                url += "/page/" + piranha.pageedit.typeId;
            }
            else if (piranha.postedit)
            {
                url += "/post/" + piranha.postedit.typeId;
            }

            fetch(url + (parentType != null ? "/" + parentType : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.typeCount > 1) {
                        // Several applicable block types, open modal
                        self.filter = "";
                        self.index = index;
                        self.callback = callback;
                        self.categories = result.categories;

                        $("#blockpicker").modal("show");
                    } else {
                        // There's only one valid block type, select it
                        callback(result.categories[0].items[0].type, index);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        select: function (item) {
            this.callback(item.type, this.index);

            this.index = 0;
            this.callback = null;

            $("#blockpicker").modal("hide");
        },
        selectSingleItem: function () {
            var categories = this.filteredCategories;

            if (categories.length === 1) {
                var items = this.filterBlockTypes(categories[0]);

                if (items.length === 1) {
                    this.select(items[0]);
                }
            }
        },
        filterBlockTypes: function (category) {
            var self = this;
            return category.items.filter(function (item) {
                return item.name.toLowerCase().indexOf(self.filter.toLowerCase()) > -1;
            });
        }
    },
    created: function () {
    }
});

$(document).ready(function() {
    $("#blockpicker").on("shown.bs.modal", function() {
        $("#blockpickerSearch").trigger("focus");
    });
});