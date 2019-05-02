/*global
    piranha
*/

piranha.blockpicker = new Vue({
    el: "#blockpicker",
    data: {
        categories: [],
        index: 0,
        callback: null
    },
    methods: {
        open: function (callback, index, parentType) {
            fetch(piranha.baseUrl + "manager/api/content/blocktypes" + (parentType != null ? "/" + parentType : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.typeCount > 1) {
                        // Several applicable block types, open modal
                        piranha.blockpicker.index = index;
                        piranha.blockpicker.callback = callback;
                        piranha.blockpicker.categories = result.categories;

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
        }
    },
    created: function () {
    }
});
