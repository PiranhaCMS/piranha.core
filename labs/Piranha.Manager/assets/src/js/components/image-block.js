Vue.component("image-block", {
    props: ["block"],
    template:
        "<div>" +
        "  <img :src='block.body.media.publicUrl.substring(1)'>" +
        "  <div class='media-picker'>" +
        "    <div class='btn-group float-right'>" +
        "      <button class='btn btn-primary text-center'>" +
        "        <i class='fas fa-plus'></i>" +
        "      </button>" +
        "      <button class='btn btn-danger text-center'>" +
        "        <i class='fas fa-times'></i>" +
        "      </button>" +
        "    </div>" +
        "    <div class='card text-left'>" +
        "      <div class='card-body'>" +
        "        <a href='#' v-on:click.prevent='piranha.preview.open(block.body.id)'>{{ block.body.media.filename }}</a>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
