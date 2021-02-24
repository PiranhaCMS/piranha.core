/*global
    piranha
*/

piranha.comment = new Vue({
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

            console.log(">>> self.state=" + self.state);
            self.items.forEach(function(item, i) {
                console.log(">>> >> self.items[" + i + "].isApproved=" + item.isApproved);
                console.log(">>> >> self.items[" + i + "].isRejected=" + item.isRejected);
                console.log(">>> >> self.items[" + i + "].!a&&!r=" + (!item.isApproved && !item.isRejected));
            });
            return this.items.filter(function (item) {
                if (self.state === "all") {
                    return true;
                } else if (self.state === "approved") {
                    return item.isApproved;
                } else if (self.state === "rejected") {
                    return item.isRejected;
                } else {
                    return !item.isApproved && !item.isRejected;
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

            fetch(piranha.baseUrl + "manager/api/comment/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.contentId = result.contentId;
                    self.items = result.comments;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        approve: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/comment/approve/" + id + (self.contentId != null ? "/" + self.contentId : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status) {
                        // Push status to notification hub
                        piranha.notifications.push(result.status);
                    }
                    self.contentId = result.contentId;
                    self.items = result.comments;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        unapprove: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/comment/unapprove/" + id + (self.contentId != null ? "/" + self.contentId : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status) {
                        // Push status to notification hub
                        piranha.notifications.push(result.status);
                    }
                    self.contentId = result.contentId;
                    self.items = result.comments;
                })
                .catch(function (error) { console.log("error:", error ); });
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

            fetch(piranha.baseUrl + "manager/api/comment/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    // Push status to notification hub
                    piranha.notifications.push(result);

                    // Refresh the list
                    self.load(self.contentId);
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        setStatus: function (status) {
            this.state = status;
        }
    },
    updated: function () {
        this.loading = false;
    }
});
