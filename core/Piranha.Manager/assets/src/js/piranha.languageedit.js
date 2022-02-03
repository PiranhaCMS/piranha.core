/*global
    piranha
*/

piranha.languageedit = new Vue({
    el: "#languageedit",
    data: {
        loading: true,
        items: [],
        originalDefault: null,
        currentDefault: null,
        showDefaultInfo: false,
        currentDelete: null,
        showDeleteInfo: false,
    },
    methods: {
        bind: function (result) {
            for (var n = 0; n < result.items.length; n++)
            {
                result.items[n].errorTitle = false;

                if (result.items[n].isDefault) {
                    this.originalDefault = this.currentDefault = result.items[n];
                }
            }
            this.items = result.items;
            this.showDefaultInfo = false;
            this.showDeleteInfo = false;
            this.currentDelete = null;
            this.loading = false;

        },
        load: function () {
            var self = this;

            self.loading = true;
            fetch(piranha.baseUrl + "manager/api/language")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) {
                    console.log("error:", error );
                    self.loading = false;
                });
        },
        save: function () {
            // Validate form
            if (this.validate()) {
                var self = this;

                self.loading = true;
                fetch(piranha.baseUrl + "manager/api/language", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        items: JSON.parse(JSON.stringify(self.items))
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status.type === "success") {
                        self.bind(result);
                    }
                    
                    if (result.status !== 400) {
                        // Refresh language list
                        self.refreshLanguageList();
                        // Push status to notification hub
                        piranha.notifications.push(result.status);
                    } else {
                        // Unauthorized request
                        piranha.notifications.unauthorized();
                        self.loading = false;
                    }
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
            }
        },
        remove: function (item) {
            var self = this;

            self.loading = true;
            fetch(piranha.baseUrl + "manager/api/language/" + item.id, {
                method: "delete",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(item)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success") {
                    self.bind(result);
                }

                if (result.status !== 400) {
                    // Refresh language list
                    self.refreshLanguageList();
                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                } else {
                    // Unauthorized request
                    piranha.notifications.unauthorized();
                    self.loading = false;
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        open: function () {
            this.load();
            $("#languageedit").modal("show");
        },
        close: function () {
            $("#languageedit").modal("hide");
        },
        addItem: function () {
            this.items.push({
                id: "00000000-0000-0000-0000-000000000000",
                title: "",
                culture: "",
                isDefault: false
            });
        },
        setDefault: function (item) {
            if (!item.isDefault) {
                for (var n = 0; n < this.items.length; n++) {
                    if (this.items[n].id != item.id) {
                        this.items[n].isDefault = false;
                    }
                }
                item.isDefault = true;
                this.currentDefault = item;
                if (this.originalDefault != item) {
                    this.showDefaultInfo = true;
                }
            }
        },
        setDefaultConfirm: function (item) {
            this.showDefaultInfo = false;
        },
        setDefaultCancel: function (items) {
            this.setDefault(this.originalDefault);
            this.currentDefault = this.originalDefault;
            this.showDefaultInfo = false;
        },
        removeItem: function (item) {
            this.currentDelete = item;
            this.showDeleteInfo = true;
        },
        removeConfirm: function () {
            this.remove(this.currentDelete);
        },
        removeCancel: function () {
            this.currentDelete = null;
            this.showDeleteInfo = false;
        },
        validate: function (item) {
            isValid = true;

            for (var n = 0; n < this.items.length; n++) {
                if (this.items[n].title === null || this.items[n].title === "")
                {
                    this.items[n].errorTitle = true;
                    isValid = false;
                }
                else
                {
                    this.items[n].errorTitle = false;
                }
                Vue.set(this.items, n, this.items[n]);
            }
            return isValid;
        },
        refreshLanguageList() {
            if (piranha.siteedit) {
                piranha.siteedit.refreshLanguageList();
            }
        }
    }
});