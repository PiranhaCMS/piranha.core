/*global
    Aero userlist
*/

Aero.userlist = new Vue({
    el: "#userlist",
    data: {
        loading: true,
        users: [],
        roles: [],
        currentUserName: null
    },
    methods: {
        bind: function (result) {
            this.users = result.users;
            this.roles = result.roles;
        },
        load: function () {
            var self = this;
            Aero.permissions.load(function () {
                fetch(Aero.baseUrl + "manager/users/list")
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        self.bind(result);

                        self.loading = false;
                    })
                    .catch(function (error) { console.log("error:", error); });
            });
        },
        remove: function (user) {
            var self = this;
            
            var userInfo = "";
            if (user) {
                if (user.userName && user.userName.length > 0) {
                    userInfo += ' <br/>"' + user.userName + '"';
                }
            }
            else {
                return;
            }

            Aero.alert.open({
                title: Aero.resources.texts.delete,
                body: Aero.resources.texts.deleteUserConfirm + userInfo,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: Aero.resources.texts.delete,
                onConfirm: function () {
                    fetch(Aero.baseUrl + "manager/user/delete", {
                        method: "delete",
                        headers: Aero.utils.antiForgeryHeaders(),
                        body: JSON.stringify(user.id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        Aero.notifications.push(result.status);

                        self.load();
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
