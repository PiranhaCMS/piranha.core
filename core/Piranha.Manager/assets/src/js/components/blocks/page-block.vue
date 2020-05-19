<template>
    <div class="block-body has-media-picker rounded clearfix" :class="{ empty: isEmpty }">
        <div>
            <div class="page-image" :style="'background-image:url(' + pageImage + ')'">
                <img :src="piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')">
            </div>
            <h3 :class="{ 'text-light': !hasPageTitle }">{{ pageTitle }}</h3>
            <p :class="{ 'text-light': !hasPageExcerpt }" v-html="pageExcerpt"></p>
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
                    <a :href="piranha.baseUrl + 'manager/page/edit/' + model.body.page.id" target="_blank">{{ model.body.page.title }}</a>
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
            piranha.pagepicker.open(this.update);
        },
        remove: function () {
            this.model.body.id = null;
            this.model.body.page = null;
        },
        update: function (page) {
            if (page !== null) {
                var self = this;

                fetch(piranha.baseUrl + "manager/api/page/info/" + page.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        self.model.body.id = result.id;
                        self.model.body.page = result;

                        // Tell parent that title has been updated
                        self.$emit('update-title', {
                            uid: self.uid,
                            title: self.model.body.page.title
                        });
                    })
                    .catch(function (error) { console.log("error:", error ); });
            } else {
                console.log("No page was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.body.page == null;
        },
        pageImage: function () {
            if (this.hasPageImage) {
                return piranha.baseUrl + "manager/api/media/url/" + this.model.body.page.primaryImage.id + "/446/220";
                //return piranha.utils.formatUrl(this.model.body.page.primaryImage.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        hasPageImage: function () {
            return this.model.body.page !== null && this.model.body.page.primaryImage.media !== null;
        },
        pageTitle: function () {
            if (this.hasPageTitle) {
                return this.model.body.page.title;
            }
            return "Lorem Ipsum";
        },
        hasPageTitle: function () {
            return this.model.body.page !== null;
        },
        pageExcerpt: function () {
            if (this.hasPageExcerpt) {
                return this.model.body.page.excerpt;
            }
            return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        },
        hasPageExcerpt: function () {
            return this.model.body.page !== null && this.model.body.page.excerpt !== null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.body.page !== null) {
                return this.model.body.page.title;
            } else {
                return "No page selected";
            }
        };
    }
}
</script>