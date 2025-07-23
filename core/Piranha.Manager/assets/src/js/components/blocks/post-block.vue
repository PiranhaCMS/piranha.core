<template>
    <div class="block-body has-media-picker rounded clearfix" :class="{ empty: isEmpty }">
        <div>
            <div class="page-image" :style="'background-image:url(' + postImage + ')'">
                <img :src="piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')">
            </div>
            <h3 :class="{ 'text-light': !hasPostTitle }">{{ postTitle }}</h3>
            <p :class="{ 'text-light': !hasPostExcerpt }" v-html="postExcerpt"></p>
        </div>
        <div class="media-picker">
            <div class="btn-group float-right">
                <button v-on:click.prevent="select" class="btn btn-primary text-center">
                    <i class="fas fa-plus"></i>
                </button>
                <button v-on:click.prevent="remove" class="btn btn-danger text-center">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="card text-left">
                <div class="card-body" v-if="isEmpty">
                    &nbsp;
                </div>
                <div class="card-body" v-else>
                    <a :href="piranha.baseUrl + 'manager/post/edit/' + model.body.post.id" target="_blank">{{ model.body.post.title }}</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    methods: {
        select: function () {
            piranha.postpicker.open(this.update);
        },
        remove: function () {
            this.model.body.id = null;
            this.model.body.post = null;
        },
        update: function (post) {
            if (post !== null) {
                var self = this;

                fetch(piranha.baseUrl + "manager/api/post/info/" + post.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        self.model.body.id = result.id;
                        self.model.body.post = result;

                        // Tell parent that title has been updated
                        self.$emit('update-title', {
                            uid: self.uid,
                            title: self.model.body.post.title
                        });
                    })
                    .catch(function (error) { console.log("error:", error ); });
            } else {
                console.log("No post was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.body.post == null;
        },
        postImage: function () {
            if (this.hasPostImage) {
                return piranha.baseUrl + "manager/api/media/url/" + this.model.body.post.primaryImage.id + "/446/220";
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        hasPostImage: function () {
            return this.model.body.post !== null && this.model.body.post.primaryImage.media !== null;
        },
        postTitle: function () {
            if (this.hasPostTitle) {
                return this.model.body.post.title;
            }
            return "Lorem Ipsum";
        },
        hasPostTitle: function () {
            return this.model.body.post !== null;
        },
        postExcerpt: function () {
            if (this.hasPostExcerpt) {
                return this.model.body.post.excerpt;
            }
            return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        },
        hasPostExcerpt: function () {
            return this.model.body.post !== null && this.model.body.post.excerpt !== null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.body.post !== null) {
                return this.model.body.post.title;
            } else {
                return "No post selected";
            }
        };
    }
}
</script>