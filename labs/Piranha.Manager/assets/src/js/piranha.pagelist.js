piranha.pagelist = new Vue({
    el: "#pagelist",
    data: {
        items: [],
        sites: []
    },
    methods: {
        load: function () {
            fetch(piranha.baseUrl + "manager/api/page/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pagelist.sites = result.sites;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (id) {
            console.log("Remove page: ", id);
        }
    },
    created: function () {
        this.load();
    },
    updated: function () {
        $(".sitemap-container").nestable({
            group: 1
        }).on('change', function (e) {
            console.log("changed: ", $(e.target).nestable("serialize"));
        });
    }
});
