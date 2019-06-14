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
        isDefault: false
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
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        save: function () {
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
                    $("#siteedit").modal("hide");
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        open: function (id) {
            // Load the site data from the server
            this.load(id);

            // Open the modal
            $("#siteedit").modal("show");
            $("#sitetitle").focus();
        }
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
