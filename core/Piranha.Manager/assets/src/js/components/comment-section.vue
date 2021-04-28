<template>
    <table class="table table-borderless table-comments">
        <thead>
            <tr>
                <th class="w-20">Author</th>
                <th class="w-50">Comment</th>
                <th class="text-nowrap">Response to</th>
                <th class="text-center">Approved</th>
                <th>Created</th>
                <th class="actions one"></th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="comment in items" v-bind:key="comment.id">
                <td class="text-nowrap align-middle">
                    <img class="rounded mr-2" :src="comment.authorImage" :alt="comment.author">
                    <a class="mr-3" :href="'mailto:' + comment.email">{{ comment.author }}</a>
                </td>
                <td>{{ comment.body }}</td>
                <td class="align-middle">
                    <a v-if="comment.articleTitle" class="author" :href="piranha.baseUrl + comment.articleUrl" target="_blank">{{ comment.articleTitle }}</a>
                </td>
                <td class="text-center text-success align-middle">
                    <button v-on:click.prevent="toggleApproved(comment)" class="switch" :aria-pressed="comment.isApproved"><span></span></button>
                </td>
                <td class="align-middle">{{ comment.created }}</td>
                <td class="actions one align-middle">
                    <a v-if="piranha.permissions.comments.delete" v-on:click.prevent="remove(comment.id)" href="#" title="Delete" class="danger"><i class="fas fa-trash"></i></a>
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
export default {
    props: ["id", "outline"],
    data: function () {
        return {
            loading: false,
            items: []
        };
    },
    methods: {
        load: function () {
            self = this;
            self.loading = true;

            fetch(piranha.baseUrl + "manager/api/comment/" + this.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.group = result.group;
                    self.items = result.comments;

                    self.loading = false;
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
    },
    mounted: function () {
        this.load();
    }
}
</script>