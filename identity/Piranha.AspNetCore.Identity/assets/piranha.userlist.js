/*global
    piranha userlist
*/

piranha.userlist = new Vue({
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
            piranha.permissions.load(function () {
                fetch(piranha.baseUrl + "manager/users/list")
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

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deleteUserConfirm + userInfo,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/user/delete/" + user.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result.status);

                        self.load();
                    })
                    .catch(function (error) {
                        console.log("error:", error);

                        piranha.notifications.push({
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
