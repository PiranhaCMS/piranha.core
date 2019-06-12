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
        },
        bind: function () {
            var self = this;

            $(".sitemap-container").each(function (i, e) {
                $(e).nestable({
                    maxDepth: 100,
                    group: i,
                    callback: function (l, e) {
                        fetch(piranha.baseUrl + "manager/api/page/move", {
                            method: "post",
                            headers: {
                                "Content-Type": "application/json",
                            },
                            body: JSON.stringify({
                                id: $(e).attr("data-id"),
                                items: $(l).nestable("serialize")
                            })
                        })
                        .then(function (response) { return response.json(); })
                        .then(function (result) {
                            piranha.notifications.push(result.status);

                            if (result.status.type === "success") {
                                $('.sitemap-container').nestable('destroy');
                                self.sites = [];
                                Vue.nextTick(function () {
                                    self.sites = result.sites;
                                    Vue.nextTick(function () {
                                        self.bind();
                                    });
                                });
                            }
                        })
                        .catch(function (error) {
                            console.log("error:", error);
                        });
                    }
                })
            });
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.updateBindings)
        {
            this.bind();
            this.updateBindings = false;
        }

        this.loading = false;
    }
});
