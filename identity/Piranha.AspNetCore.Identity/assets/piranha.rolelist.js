/*global
    piranha rolelist
*/

piranha.rolelist = new Vue({
    el: "#rolelist",
    data: {
        loading: true,
        roles: []
    },
    methods: {
        bind(result) {
            this.roles = result.roles;
        },
        load() {
            var self = this;
            piranha.permissions.load(function () {
                fetch(piranha.baseUrl + "manager/roles/list")
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        self.bind(result);

                        self.loading = false;
                    })
                    .catch(function (error) {
                        piranha.notifications.push({
                            body: error,
                            type: "danger",
                            hide: true
                        });
                    });
            });
        },
        remove(role) {
            const self = this;
            let roleInfo = "";

            if (role) {
                if (role.name && role.name.length > 0) {
                    roleInfo += ' <br/>"' + role.name + '"';
                }
            }
            else {
                return;
            }

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deleteRoleConfirm + roleInfo,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm() {
                    fetch(piranha.baseUrl + "manager/role/delete/" + role.id, {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders()
                    })
                        .then(function (response) { return response.json(); })
                        .then(function (result) {
                            piranha.notifications.push(result.status);

                            self.load();
                        })
                        .catch(function (error) {
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
