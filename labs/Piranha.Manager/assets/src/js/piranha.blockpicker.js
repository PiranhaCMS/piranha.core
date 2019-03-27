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
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/content/blocktypes")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.blockpicker.categories = result.categories;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        open: function (index, callback) {
            this.index = index;
            this.callback = callback;

            $("#blockpicker").modal("show");
        },
        select: function (item) {
            this.callback (item.type, this.index);

            this.index = 0;
            this.callback = null;

            $("#blockpicker").modal("hide");
        }
    },
    created: function () {
        this.load();
    }
});
