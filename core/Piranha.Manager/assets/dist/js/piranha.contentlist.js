/*global
    piranha
*/

piranha.contentlist = new Vue({
    el: "#contentlist",
    data: {
        loading: true,
        group: null,
        items: [],
        types: []
    },
    methods: {
        bind: function (result) {

            this.group = result.group;
            this.types = result.types;
            this.items = result.items.map(function (i) {
                var type = result.types.find(function (t) {
                    return t.id === i.typeId;
                });

                i.type = type.title || i.typeId;

                return i;
            });
        },
        load: function (group) {
            var self = this;

            piranha.permissions.load(function () {
                fetch(piranha.baseUrl + "manager/api/content/" + group + "/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    self.loading = false;
                })
                .catch(function (error) { console.log("error:", error ); });
            });
        },
        remove: function (id) {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePageConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/content/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        self.load(self.group.id);
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
    },
    created: function () {
    }
});
