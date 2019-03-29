/*global
    piranha
*/

Vue.component("image-block", {
    props: ["block"],
    methods: {
        clear: function () {
            // clear media from block
        },
        select: function () {
            piranha.mediapicker.open(this.update);
        },
        remove: function () {
            this.block.body.media = null;
        },
        update: function (media) {
            if (media.type === "Image") {
                this.block.body.id = media.id;
                this.block.body.media = media;
            } else {
                console.log("No image was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.block.body.media == null;
        },
        mediaUrl: function () {
            if (this.block.body.media != null) {
                return piranha.utils.formatUrl(this.block.body.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/assets/img/empty-image.png");
            }
        }
    },
    mounted: function() {
        this.block.getTitle = function () {
            if (this.block.media != null) {
                return this.block.media.filename;
            } else {
                return "No image selected";
            }
        };
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <img :src='mediaUrl'>" +
        "  <div class='media-picker'>" +
        "    <div class='btn-group float-right'>" +
        "      <button v-on:click.prevent='select' class='btn btn-primary text-center'>" +
        "        <i class='fas fa-plus'></i>" +
        "      </button>" +
        "      <button v-on:click.prevent='remove' class='btn btn-danger text-center'>" +
        "        <i class='fas fa-times'></i>" +
        "      </button>" +
        "    </div>" +
        "    <div class='card text-left'>" +
        "      <div class='card-body' v-if='isEmpty'>" +
        "        &nbsp;" +
        "      </div>" +
        "      <div class='card-body' v-else>" +
        "        {{ block.body.media.filename }}" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
