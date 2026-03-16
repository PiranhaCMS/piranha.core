/*global
    Aero
*/

 Aero.alias = new Vue({
    el: "#alias",
    data: {
        loading: true,
        siteId: null,
        siteTitle: null,
        sites: [],
        items: [],
        model: {
            id: null,
            aliasUrl: null,
            redirectUrl: null,
            isPermanent: true
        }
    },
    methods: {
        load: function (siteId) {
            var self = this;

            if (!siteId) {
                siteId = "";
            }

            fetch(Aero.baseUrl + "manager/api/alias/list/" + siteId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.siteId = result.siteId;
                    self.siteTitle = result.siteTitle;
                    self.sites = result.sites;
                    self.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        save: function () {
            // Validate form
            var form = document.getElementById("aliasForm");
            if (form.checkValidity() === false) {
                form.classList.add("was-validated");
                return;
            }

            fetch(Aero.baseUrl + "manager/api/alias/save", {
                method: "post",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify({
                    id: Aero.alias.model.id,
                    siteId: Aero.alias.siteId,
                    aliasUrl: Aero.alias.model.aliasUrl,
                    redirectUrl: Aero.alias.model.redirectUrl,
                    isPermanent: Aero.alias.model.isPermanent != null ? Aero.alias.model.isPermanent : false
                })
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success") {
                    // Remove validation class
                    form.classList.remove("was-validated");

                    // Close modal
                    $("#aliasModal").modal("hide");

                    // Clear modal
                    Aero.alias.model.id = null;
                    Aero.alias.model.aliasUrl = null;
                    Aero.alias.model.redirectUrl = null;
                    Aero.alias.model.isPermanent = true;

                    Aero.alias.items = result.items;
                }

                if (result.status !== 400) {
                    // Push status to notification hub
                    Aero.notifications.push(result.status);
                } else {
                    // Unauthorized request
                    Aero.notifications.unauthorized();
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        remove: function (id) {
            var self = this;

            Aero.alert.open({
                title: Aero.resources.texts.delete,
                body: Aero.resources.texts.deleteAliasConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: Aero.resources.texts.delete,
                onConfirm: function () {
                    fetch(Aero.baseUrl + "manager/api/alias/delete", {
                        method: "delete",
                        headers: Aero.utils.antiForgeryHeaders(),
                        body: JSON.stringify(id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        if (result.status.type === "success") {
                            self.items = result.items;
                        }

                        if (result.status !== 400) {
                            // Push status to notification hub
                            Aero.notifications.push(result.status);
                        } else {
                            // Unauthorized request
                            Aero.notifications.unauthorized();
                        }
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        }
    },
    updated: function () {
        this.loading = false;
    }
});

$(document).on("shown.bs.modal","#aliasModal", function (event) {
    $(this).find("#aliasUrl").focus();
});