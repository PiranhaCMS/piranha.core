/*global
    Aero
*/

Aero.comment = new Vue({
    el: "#comments",
    data: {
        loading: true,
        contentId: null,
        items: [],
        state: "all"
    },
    computed: {
        filteredItems: function () {
            var self = this;

            return this.items.filter(function (item) {
                if (self.state === "all") {
                    return true;
                } else if (self.state === "pending") {
                    return !item.isApproved;
                } else {
                    return item.isApproved;
                }
            });
        }
    },
    methods: {
        load: function (id) {
            var self = this;

            if (!id) {
                id = "";
            }

            fetch(Aero.baseUrl + "manager/api/comment/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.contentId = result.contentId;
                    self.items = result.comments;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        approve: function (id) {
            var self = this;

            fetch(Aero.baseUrl + "manager/api/comment/approve", {
                method: "post",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify({
                    id: id,
                    parentId: self.contentId 
                })
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status) {
                    // Push status to notification hub
                    Aero.notifications.push(result.status);
                }
                self.contentId = result.contentId;
                self.items = result.comments;
            })
            .catch(function (error) { 
                console.log("error:", error ); 
            });
        },
        unapprove: function (id) {
            var self = this;

            fetch(Aero.baseUrl + "manager/api/comment/unapprove", {
                method: "post",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify({
                    id: id,
                    parentId: self.contentId 
                })
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status) {
                    // Push status to notification hub
                    Aero.notifications.push(result.status);
                }
                self.contentId = result.contentId;
                self.items = result.comments;
            })
            .catch(function (error) { 
                console.log("error:", error ); 
            });
        },
        toggleApproved: function (item) {
            item.isApproved = !item.isApproved;

            if (item.isApproved) {
                this.approve(item.id);
            } else {
                this.unapprove(item.id);
            }
        },
        remove: function (id) {
            var self = this;

            fetch(Aero.baseUrl + "manager/api/comment/delete", {
                method: "delete",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify(id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                // Push status to notification hub
                Aero.notifications.push(result);

                // Refresh the list
                self.load(self.contentId);
            })
            .catch(function (error) { 
                console.log("error:", error ); 
            });
        },
        setStatus: function (status) {
            this.state = status;
        }
    },
    updated: function () {
        this.loading = false;
    }
});
