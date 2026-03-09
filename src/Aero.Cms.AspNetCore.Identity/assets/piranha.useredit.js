/*global
    Aero userlist
*/

Aero.useredit= new Vue({
    el: "#useredit",
    data: {
        loading: true,
        isNew: false,
        userModel: null,
        currentUserName: null
    },
    methods: {
        bind: function (result) {
            this.userModel = result;
            this.isNew = result.user.id === "00000000-0000-0000-0000-000000000000";
        },
        load: function (id, isNew) {
            var self = this;

            var url = isNew ? Aero.baseUrl + "manager/user/add" : Aero.baseUrl + "manager/user/edit/" + id;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                    self.loading = false;
                })
                .catch(function (error) { console.log("error:", error); });
        },
        getRoleRows: function () {
            var roleRows = Array();
            for (var i = 0, j = this.userModel.roles.length; i < j; i += 3) {
                roleRows.push(this.userModel.roles.slice(i, i + 3));
            }
            return roleRows;
        },
        save: function () {
            // Validate form
            var form = document.getElementById("usereditForm");
            if (form.checkValidity() === false) {
                form.classList.add("was-validated");
                return;
            }

            var ok = false;
            var self = this;
            console.log(JSON.stringify(self.userModel));
            fetch(Aero.baseUrl + "manager/user/save", {
                method: "post",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify(self.userModel)
            })
            .then(function (response) {
                ok = response.ok;
                return response.json();
            })
            .then(function (result) {
                if (ok) {
                    self.bind(result);
                    
                    Aero.notifications.push({
                        body: Aero.resources.texts.userIsSaved,
                        type: "success",
                        hide: true
                    });
                }
                else if (result.status) {
                    Aero.notifications.push(result.status);
                }
                else {
                    Aero.notifications.push({
                        body: "<strong>" + Aero.resources.texts.errorOccurred + "</strong>",
                        type: "danger",
                        hide: true
                    });
                }

            })
            .catch(function (error) {
                Aero.notifications.push({
                    body: error,
                    type: "danger",
                    hide: true
                });

                console.log("error:", error);
            });
        },
        remove: function (userId) {
            var self = this;

            Aero.alert.open({
                title: Aero.resources.texts.delete,
                body: Aero.resources.texts.deleteUserConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: Aero.resources.texts.delete,
                onConfirm: function () {
                    var ok = false;
                    fetch(Aero.baseUrl + "manager/user/delete", {
                        method: "delete",
                        headers: Aero.utils.antiForgeryHeaders(),
                        body: JSON.stringify(userId)
                    })
                    .then(function (response) { 
                        ok = response.ok;
                        return response.json();
                    })
                    .then(function (result) {
                        Aero.notifications.push(result.status);
                        if (ok) {
                            window.location = Aero.baseUrl + "manager/users/?d=1";
                        }
                    })
                    .catch(function (error) {
                        console.log("error:", error);

                        Aero.notifications.push({
                            body: error,
                            type: "danger",
                            hide: true
                        });
                    });
                }
            });
        }
    }
});
