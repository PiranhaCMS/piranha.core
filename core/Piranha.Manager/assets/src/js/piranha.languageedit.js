/*global
    piranha
*/

piranha.languageedit = new Vue({
    el: "#languageedit",
    data: {
        loading: true,
        items: []
    },
    methods: {
        load: function () {
            var self = this;

            self.loading = true;
            fetch(piranha.baseUrl + "manager/api/language")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    for (var n = 0; n < result.items.length; n++)
                    {
                        result.items[n].errorTitle = false;
                    }
                    self.items = result.items;
                    self.loading = false;
                })
                .catch(function (error) {
                    console.log("error:", error );
                    self.loading = false;
                });
        },
        open: function () {
            this.load();
            $("#languageedit").modal("show");
        },
        close: function () {
            $("#languageedit").modal("hide");
        },
        setDefault: function (item) {
            if (!item.isDefault) {
                for (var n = 0; n < this.items.length; n++) {
                    if (this.items[n].id != item.id) {
                        this.items[n].isDefault = false;
                    }
                }
                item.isDefault = true;
            }
        },
        addItem: function () {
            this.items.push({
                id: "00000000-0000-0000-0000-000000000000",
                title: "",
                culture: "",
                isDefault: false
            });
        },
        removeItem: function (item) {
            this.items.splice(this.items.indexOf(item), 1);
        },
        save: function () {
            // Validate form
            if (this.validate()) {
                var self = this;

                self.loading = true;
                fetch(piranha.baseUrl + "manager/api/language", {
                    method: "post",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        items: JSON.parse(JSON.stringify(self.items))
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    //if (result.status.type === "success")
                    //{
                        self.items = result.items;
                        self.loading = false;
                    //}

                    // Push status to notification hub
                    // piranha.notifications.push(result.status);
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
            }
        },
        validate: function () {
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
        }
    }
});