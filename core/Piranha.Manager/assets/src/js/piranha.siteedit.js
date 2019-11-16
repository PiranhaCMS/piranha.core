/*global
    piranha
*/

piranha.siteedit = new Vue({
    el: "#siteedit",
    data: {
        loading: true,
        id: null,
        typeId: null,
        title: null,
        internalId: null,
        culture: null,
        description: null,
        hostnames: null,
        isDefault: false,
        siteTypes: [],
        regions: [],
        isNew: false,
        isConfirm: false,
        confirmTitle: null,
        selectedRegion: {
            uid: "uid-settings",
            name: null,
            icon: null,
        },
        callback: null,
    },
    methods: {
        load: function (id) {
            self = this;

            fetch(piranha.baseUrl + "manager/api/site/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.id = result.id;
                    self.typeId = result.typeId;
                    self.title = result.title;
                    self.internalId = result.internalId;
                    self.culture = result.culture;
                    self.description = result.description;
                    self.hostnames = result.hostnames;
                    self.isDefault = result.isDefault;
                    self.siteTypes = result.siteTypes;
                })
                .catch(function (error) { console.log("error:", error ); });

            fetch(piranha.baseUrl + "manager/api/site/content/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.regions = result.regions;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        save: function () {
            // Validate form
            var form = document.getElementById("siteForm");
            if (form.checkValidity() === false) {
                form.classList.add("was-validated");
                return;
            }

            var self = this;
            var model = {
                id: this.id,
                typeId: this.typeId,
                title: this.title,
                internalId: this.internalId,
                culture: this.culture,
                description: this.description,
                hostnames: this.hostnames,
                isDefault: this.isDefault
            };

            fetch(piranha.baseUrl + "manager/api/site/save", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                piranha.notifications.push(result);

                if (result.type === "success") {
                    // Check if we should save content as well
                    if (self.id != null && self.typeId != null) {
                        var content = {
                            id: self.id,
                            typeId: self.typeId,
                            title: self.title,
                            regions: JSON.parse(JSON.stringify(self.regions))
                        };

                        fetch(piranha.baseUrl + "manager/api/site/savecontent", {
                            method: "post",
                            headers: {
                                "Content-Type": "application/json",
                            },
                            body: JSON.stringify(content)
                        })
                        .catch(function (error) { console.log("error:", error ); });
                    }

                    $("#siteedit").modal("hide");

                    if (self.callback)
                    {
                        self.callback();
                        self.callback = null;
                    }
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        open: function (id, cb) {
            // Remove old validation
            var form = document.getElementById("siteForm");
            form.classList.remove("was-validated");

            // Store callback
            this.callback = cb;
            this.isNew = false;
            this.selectedRegion = {
                uid: "uid-settings",
                name: null,
                icon: null,
            };

            this.isConfirm = false;
            this.confirmTitle = null;

            // Load the site data from the server
            this.load(id);

            // Open the modal
            $("#siteedit").modal("show");
            $("#sitetitle").focus();
        },
        create: function (cb) {
            var self = this;

            // Remove old validation
            var form = document.getElementById("siteForm");
            form.classList.remove("was-validated");

            // Create a new site
            fetch(piranha.baseUrl + "manager/api/site/create")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.id = result.id;
                    self.typeId = result.typeId;
                    self.title = result.title;
                    self.internalId = result.internalId;
                    self.culture = result.culture;
                    self.description = result.description;
                    self.hostnames = result.hostnames;
                    self.isDefault = result.isDefault;
                    self.siteTypes = result.siteTypes;

                    self.isNew = true;
                    self.callback = cb;
                    self.selectedRegion = {
                        uid: "uid-settings",
                        name: null,
                        icon: null,
                    };
                })
                .catch(function (error) { console.log("error:", error ); });

            // Open the modal
            $("#siteedit").modal("show");
            $("#sitetitle").focus();
        },
        confirm: function () {
            this.isConfirm = true;
        },
        cancel: function () {
            this.isConfirm = false;
            this.confirmTitle = null;
        },
        remove: function () {
            this.isConfirm = false;
            this.confirmTitle = null;

            var self = this;

            fetch(piranha.baseUrl + "manager/api/site/delete/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.notifications.push(result);

                    if (result.type === "success") {
                        $("#siteedit").modal("hide");

                        if (self.callback)
                        {
                            self.callback();
                            self.callback = null;
                        }
                    }
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        selectRegion: function (region) {
            this.selectedRegion = region;
        },
    },
    updated: function () {
        this.loading = false;
    }
});

//
// Set focus when opening modal
//
$("#siteedit").on("shown.bs.modal", function () {
    $("#sitetitle").trigger("focus");
});
