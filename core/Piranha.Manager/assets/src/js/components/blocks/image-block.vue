<template>
    <div class="block-body has-media-picker rounded" :class="{ empty: isEmpty }">
        <img class="rounded" :src="mediaUrl">
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
                    {{ model.body.media.filename }}
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    methods: {
        clear: function () {
            // clear media from block
        },
        select: function () {
            if (this.model.body.media != null) {
                piranha.mediapicker.open(this.update, "Image", this.model.body.media.folderId);
            } else {
                piranha.mediapicker.openCurrentFolder(this.update, "Image");
            }
        },
        remove: function () {
            this.model.body.id = null;
            this.model.body.media = null;
        },
        update: function (media) {
            if (media.type === "Image") {
                this.model.body.id = media.id;
                this.model.body.media = media;

                // Tell parent that title has been updated
                this.$emit('update-title', {
                    uid: this.uid,
                    title: this.model.body.media.filename
                });
            } else {
                console.log("No image was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.body.media == null;
        },
        mediaUrl: function () {
            if (this.model.body.media != null) {
                return piranha.utils.formatUrl(this.model.body.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.media != null) {
                return this.model.media.filename;
            } else {
                return "No image selected";
            }
        };
    }
}
</script>