/*global
    piranha
*/

 piranha.alias = new Vue({
    el: "#alias",
    data: {
        loading: true,
        siteId: null,
        items: [],
        model: {
            id: null,
            aliasUrl: null,
            redirectUrl: null,
            isPermanent: true
        }
    },
    methods: {
        load: function () {
            fetch(piranha.baseUrl + "manager/api/alias/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.alias.siteId = result.siteId;
                    piranha.alias.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        save: function () {
            fetch(piranha.baseUrl + "manager/api/alias/save", {
                    method: "post",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        id: piranha.alias.model.id,
                        siteId: piranha.alias.siteId,
                        aliasUrl: piranha.alias.model.aliasUrl,
                        redirectUrl: piranha.alias.model.redirectUrl,
                        isPermanent: piranha.alias.model.isPermanent != null ? piranha.alias.model.isPermanent : false
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status.type === "success")
                    {
                        // Close modal
                        $("#aliasModal").modal("hide");

                        // Clear modal
                        piranha.alias.model.id = null;
                        piranha.alias.model.aliasUrl = null;
                        piranha.alias.model.redirectUrl = null;
                        piranha.alias.model.isPermanent = true;

                        piranha.alias.items = result.items;
                    }

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
        },
        remove: function (id) {
            fetch(piranha.baseUrl + "manager/api/alias/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.alias.items = result.items;

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error ); });
        }
    },
    created: function () {
        this.load();
    },
    updated: function () {
        this.loading = false;
    }
});

$(document).on("shown.bs.modal","#aliasModal", function (event) {
    $(this).find("#aliasUrl").focus();
});