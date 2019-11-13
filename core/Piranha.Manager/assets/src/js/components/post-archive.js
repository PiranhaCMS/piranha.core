Vue.component("post-archive", {
    props: ["uid", "id"],
    data: function() {
        return {
            items: [],
            categories: [],
            postTypes: [],
            totalPosts: 0,
            totalPages: 0,
            index: 0,
            status: "all",
            category: piranha.resources.texts.allCategories
        }
    },
    methods: {
        load: function (index) {
            var self = this;

            if (!index) {
                index = 0;
            }

            fetch(piranha.baseUrl + "manager/api/post/list/" + self.id + "/" + index)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.items = result.posts;
                    self.categories = result.categories;
                    self.postTypes = result.postTypes;
                    self.totalPosts = result.totalPosts;
                    self.totalPages = result.totalPages;
                    self.index = result.index;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (postId) {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePostConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/post/delete/" + postId)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        self.load();
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
        first: function () {
            if (this.hasPrev()) {
                this.load(0);
            }
        },
        prev: function () {
            if (this.hasPrev()) {
                this.load(this.index - 1);
            }
        },
        next: function () {
            if (this.hasNext()) {
                this.load(this.index + 1);
            }
        },
        last: function () {
            if (this.hasNext()) {
                this.load(this.totalPages - 1);
            }
        },
        hasPrev: function () {
            return this.index > 0;
        },
        hasNext: function () {
            return this.index < (this.totalPages - 1);
        },
        isSelected: function (item) {
            // Check category
            if (this.category !== piranha.resources.texts.allCategories && item.category !== this.category) {
                return false;
            }

            // Check status
            if (this.status === "draft") {
                return item.status === "draft" || item.status === "unpublished";
            } else if (this.status === 'scheduled') {
                return item.isScheduled;
            }

            // Selected
            return true;
        },
        selectStatus: function (status) {
            this.status = status;
        },
        selectCategory: function (category) {
            this.category = category;
        }
    },
    mounted: function () {
        this.load();
    },
    beforeDestroy: function () {
    },
    template:
        "<div :id='uid'>" +
        "  <div class='mb-2'>" +
        "    <div class='btn-group' role='group'>" +
        "      <button v-on:click='selectStatus(\"all\")' class='btn btn-sm' :class='status === \"all\" ? \"btn-primary\" : \"btn-light\"' href='#'>{{ piranha.resources.texts.all }}</button>" +
        "      <button v-on:click='selectStatus(\"draft\")' class='btn btn-sm' :class='status === \"draft\" ? \"btn-primary\" : \"btn-light\"' href='#'>{{ piranha.resources.texts.drafts }}</button>" +
        "      <button v-on:click='selectStatus(\"scheduled\")' class='btn btn-sm' :class='status === \"scheduled\" ? \"btn-primary\" : \"btn-light\"' href='#'>{{ piranha.resources.texts.scheduled }}</button>" +
        "    </div>" +
        "    <div v-if='postTypes.length > 1' class='btn-group' role='group'>" +
        "      <button type='button' class='btn btn-sm btn-light dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>" +
        "        {{ piranha.resources.texts.all }}" +
        "      </button>" +
        "      <div class='dropdown-menu dropdown-menu-right'>" +
        "        <a v-for='type in postTypes' href='#' class='dropdown-item'>{{ type.title }}</a>" +
        "      </div>" +
        "    </div>" +
        "    <div v-if='categories.length > 1' class='btn-group' role='group'>" +
        "      <button type='button' class='btn btn-sm dropdown-toggle' :class='category === piranha.resources.texts.allCategories ? \"btn-light\" : \"btn-primary\"' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>" +
        "        {{ category }}" +
        "      </button>" +
        "      <div class='dropdown-menu dropdown-menu-right'>" +
        "        <a v-on:click.prevent='selectCategory(piranha.resources.texts.allCategories)' href='#' class='dropdown-item'>{{ piranha.resources.texts.allCategories }}</a>" +
        "        <a v-on:click.prevent='selectCategory(category.title)' v-for='category in categories' href='#' class='dropdown-item'>{{ category.title }}</a>" +
        "      </div>" +
        "    </div>" +
        "    <div v-if='postTypes.length > 1 && piranha.permissions.posts.add' class='btn-group float-right'>" +
        "      <button id='addPostGroup' class='btn btn-sm btn-primary btn-labeled dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'><i class='fas fa-plus'></i>{{ piranha.resources.texts.add }}</button>" +
        "      <div class='dropdown-menu dropdown-menu-right' aria-labelledby='addPostGroup'>" +
        "        <a class='dropdown-item' :href='piranha.baseUrl + type.addUrl + id + \"/\" + type.id' v-for='type in postTypes'>{{ type.title }}</a>" +
        "      </div>" +
        "    </div>" +
        "    <a v-if='postTypes.length === 1 && piranha.permissions.posts.add' :href='piranha.baseUrl + postTypes[0].addUrl + id + \"/\" + postTypes[0].id' class='btn btn-sm btn-primary btn-labeled float-right'><i class='fas fa-plus'></i>{{ piranha.resources.texts.add }}</a>" +
        "  </div>" +
        "  <table class='table'>" +
        "    <tbody>" +
        "      <tr v-if='isSelected(post)' v-for='post in items' :class='post.status'>" +
        "        <td>" +
        "          <a :href='piranha.baseUrl + post.editUrl + post.id'>{{ post.title }}</a> " +
        "          <small v-if='post.status === \"published\" || post.status === \"draft\"' class='text-muted'>| Published: {{ post.published }}</small>" +
        "          <small v-else-if='post.status === \"unpublished\"' class='text-muted'>| Unpublished</small>" +
        "          <span v-if='post.status === \"draft\"' class='badge badge-info float-right'>{{ piranha.resources.texts.draft }}</span>" +
        "        </td>" +
        "        <td>" +
        "          {{ post.typeName }}" +
        "        </td>" +
        "        <td>" +
        "          {{ post.category }}" +
        "        </td>" +
        "        <td class='actions one'>" +
        "          <a v-if='piranha.permissions.posts.delete' v-on:click.prevent='remove(post.id)' class='danger'><i class='fas fa-trash'></i></a>" +
        "        </td>" +
        "      </tr>" +
        "    </tbody>" +
        "  </table>" +
        "  <nav v-if='totalPages > 1'>" +
        "    <ul class='pagination justify-content-center'>" +
        "      <li class='page-item' :class='{ disabled: !hasPrev() }'><button v-on:click.prevent='first()' :disabled='!hasPrev()' class='page-link' href='#'><i class='fas fa-angle-double-left'></i></i></button></li>" +
        "      <li class='page-item' :class='{ disabled: !hasPrev() }'><button v-on:click.prevent='prev()' :disabled='!hasPrev()' class='page-link' href='#'><i class='fas fa-chevron-left'></i></button></li>" +
        "      <li class='page-item disabled'><span class='page-link'>{{ index + 1}} / {{ totalPages }}</span></li>" +
        "      <li class='page-item' :class='{ disabled: !hasNext() }'><button v-on:click.prevent='next()' :disabled='!hasNext()' class='page-link' href='#'><i class='fas fa-chevron-right'></i></button></li>" +
        "      <li class='page-item' :class='{ disabled: !hasNext() }'><button v-on:click.prevent='last()' :disabled='!hasNext()' class='page-link' href='#'><i class='fas fa-angle-double-right'></i></button></li>" +
        "    </ul>" +
        "  </nav>" +
        "</div>"
});
