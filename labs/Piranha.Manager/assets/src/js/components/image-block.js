Vue.component("image-block", {
    props: ["block"],
    methods: {
        clear: function () {
            // clear media from block
        },
        select: function () {
            piranha.mediapicker.open(this.update);
        },
        update: function (media) {
            if (media.type === "Image") {
                console.log(this)
                this.block.body.id = media.id;
                this.block.body.media = media;
            } else {
                console.log("No image was selected");
            }
        }
    },
    computed: {
        mediaUrl: function () {
            if (this.block.body.media.publicUrl.startsWith("~")) {
                return this.block.body.media.publicUrl.substring(1)
            } else {
                return this.block.body.media.publicUrl;
            }
        }
    },
    template:
        "<div>" +
        "  <img :src='mediaUrl'>" +
        "  <div class='media-picker'>" +
        "    <div class='btn-group float-right'>" +
        "      <button v-on:click.prevent='select' class='btn btn-primary text-center'>" +
        "        <i class='fas fa-plus'></i>" +
        "      </button>" +
        "      <button class='btn btn-danger text-center'>" +
        "        <i class='fas fa-times'></i>" +
        "      </button>" +
        "    </div>" +
        "    <div class='card text-left'>" +
        "      <div class='card-body'>" +
        "        {{ block.body.media.filename }}" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
