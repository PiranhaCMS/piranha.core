piranha.module = new Vue({
    el: "#module",
    data: {
        items: []
    },
    methods: {
        load: function () {
            fetch(piranha.baseUrl + "manager/api/module/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.module.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        }
    },
    created: function () {
        this.load();
    }
});
