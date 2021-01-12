<template>
    <div class="block-body has-media-picker rounded clearfix" :class="{ empty: isEmpty }">
        <div>
            <div class="page-image" :style="'background-image:url(' + contentImage + ')'">
                <img :src="piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')">
            </div>
            <h3 :class="{ 'text-light': !hasContentTitle }">{{ contentTitle }}</h3>
            <p :class="{ 'text-light': !hasContentExcerpt }" v-html="contentExcerpt"></p>
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
                    <a :href="piranha.baseUrl + 'manager/content/edit/' + model.body.content.typeId + '/' + model.body.content.id" target="_blank">{{ model.body.content.title }}</a>
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
            piranha.contentpicker.open(null, this.update);
        },
        remove: function () {
            this.model.body.id = null;
            this.model.body.content = null;
        },
        update: function (content) {
            if (content !== null) {
                var self = this;

                fetch(piranha.baseUrl + "manager/api/content/info/" + content.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        self.model.body.id = result.id;
                        self.model.body.content = result;

                        // Tell parent that title has been updated
                        self.$emit('update-title', {
                            uid: self.uid,
                            title: self.model.body.content.title
                        });
                    })
                    .catch(function (error) { console.log("error:", error ); });
            } else {
                console.log("No content was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.body.content == null;
        },
        contentImage: function () {
            if (this.hasContentImage) {
                return piranha.baseUrl + "manager/api/media/url/" + this.model.body.content.primaryImage.id + "/446/220";
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        hasContentImage: function () {
            return this.model.body.content !== null && this.model.body.content.primaryImage.media !== null;
        },
        contentTitle: function () {
            if (this.hasContentTitle) {
                return this.model.body.content.title;
            }
            return "Lorem Ipsum";
        },
        hasContentTitle: function () {
            return this.model.body.content !== null;
        },
        contentExcerpt: function () {
            if (this.hasContentExcerpt) {
                return this.model.body.content.excerpt;
            }
            return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        },
        hasContentExcerpt: function () {
            return this.model.body.content !== null && this.model.body.content.excerpt !== null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.body.content !== null) {
                return this.model.body.content.title;
            } else {
                return "No content selected";
            }
        };
    }
}
</script>