/*global
    piranha
*/

piranha.pagelist = new Vue({
    el: "#pagelist",
    data: {
        loading: true,
        updateBindings: false,
        items: [],
        sites: [],
        pageTypes: []
    },
    methods: {
        load: function () {
            fetch(piranha.baseUrl + "manager/api/page/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pagelist.sites = result.sites;
                    piranha.pagelist.pageTypes = result.pageTypes;

                    piranha.pagelist.updateBindings = true;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.notifications.push(result);

                    self.load();
                })
                .catch(function (error) { console.log("error:", error ); });
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.updateBindings)
        {
            console.log("binding nestable");

            $('.sitemap-container').nestable('destroy');
            $(".sitemap-container").nestable({
                maxDepth: 100,
                group: 1
            }).on('change', function (e) {
                console.log("changed: ", $(e.target).nestable("serialize"));
            });

            this.updateBindings = false;
        }

        this.loading = false;
    }
});
