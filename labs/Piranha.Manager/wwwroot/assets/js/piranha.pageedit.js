Vue.component("block-group", {
    props: ["uid", "block"],
    methods: {
        selectItem: function (item) {
            for (var n = 0; n < this.block.items.length; n++) {
                if (this.block.items[n] == item) {
                    this.block.items[n].isActive = true;
                } else {
                    this.block.items[n].isActive = false;
                }
            }
        }
    },
    mounted: function () {
    },
    beforeDestroy: function () {
    },
    template:
        "<div class='block-group'>" +
        "  <div class='block-group-header'>" +
        "    TODO: Global group fields" +
        "  </div>" +
        "  <div class='row'>" +
        "    <div class='col-md-4'>" +
        "      <div class='list-group list-group-flush'>" +
        "        <a href='#' v-on:click.prevent='selectItem(child)' class='list-group-item' :class='{ active: child.isActive }' v-for='child in block.items'>" +
        "          <span class='handle sortable-handle'>" +
        "            <i class='fas fa-ellipsis-v'></i>" +
        "          </span>" +
        "          {{ child.item.body.media.filename }}" +
        "        </a>" +
        "      </div>" +
        "    </div>" +
        "    <div class='col-md-8'>" +
        "      <div v-for='child in block.items' v-if='child.isActive' :class='\"block \" + child.component'>" +
        "        <component v-bind:is='child.component' v-bind:uid='child.uid' v-bind:block='child.item'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("html-block", {
    props: ["uid", "block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyHtml(this.block.body.value);
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid);
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <div contenteditable='true' :id='uid' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("html-column-block", {
    props: ["uid", "block"],
    methods: {
        onBlurCol1: function (e) {
            this.block.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.block.column2.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty1: function () {
            return piranha.utils.isEmptyHtml(this.block.column1.value);
        },
        isEmpty2: function () {
            return piranha.utils.isEmptyHtml(this.block.column2.value);
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid + 1);
        piranha.editor.addInline(this.uid + 2);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid + 1);
        piranha.editor.remove(this.uid + 2);
    },
    template:
        "<div class='row'>" +
        "  <div class='col-md-6'>" +
        "    <div :class='{ empty: isEmpty1 }'>" +
        "      <div :id='uid + 1' contenteditable='true' spellcheck='false' v-html='block.column1.value' v-on:blur='onBlurCol1'></div>" +
        "    </div>" +
        "  </div>" +
        "  <div class='col-md-6'>" +
        "    <div :class='{ empty: isEmpty2 }'>" +
        "      <div :id='uid + 2' contenteditable='true' spellcheck='false' v-html='block.column2.value' v-on:blur='onBlurCol2'></div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

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
                console.log(this)
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
                return this.block.body.media.publicUrl.replace("~/", piranha.baseUrl);
            } else {
                return piranha.baseUrl + "assets/img/empty-image.png";
            }
        }
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

/*global
    piranha
*/

Vue.component("text-block", {
    props: ["block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.block.body.value);
        }
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <pre contenteditable='true' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></pre>" +
        "</div>"
});

Vue.component("missing-block", {
    props: ["block"],
    template:
        "<div class='alert alert-danger' role='alert'>Missing Component</div>"
});

/*global
    piranha
*/

piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        loading: true,
        id: null,
        siteId: null,
        parentId: null,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaKeywords: null,
        metaDescription: null,
        blocks: []
    },
    methods: {
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.id = result.id;
                    piranha.pageedit.siteId = result.siteId;
                    piranha.pageedit.parentId = result.parentId;
                    piranha.pageedit.typeId = result.typeId;
                    piranha.pageedit.title = result.title;
                    piranha.pageedit.navigationTitle = result.navigationTitle;
                    piranha.pageedit.slug = result.slug;
                    piranha.pageedit.metaKeywords = result.metaKeywords;
                    piranha.pageedit.metaDescription = result.metaDescription;
                    piranha.pageedit.blocks = result.blocks;
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        remove: function (id) {
            console.log("Remove page: ", id);
        },
        addBlock: function (type, pos) {
            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (pos) {
                        piranha.pageedit.blocks.splice(pos, 0, result);
                    } else {
                        piranha.pageedit.blocks.push(result);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        moveBlock: function (from, to) {
            this.blocks.splice(to, 0, this.blocks.splice(from, 1)[0])
        },
        removeBlock: function (block) {
            var index = this.blocks.indexOf(block);

            if (index !== -1) {
                this.blocks.splice(index, 1);
            }
        }
    },
    created: function () {
    },
    updated: function () {
        sortable(".blocks", {
            handle: ".handle",
            items: ":not(.unsortable)"
        })[0].addEventListener("sortupdate", function (e) {
            piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
        });

        this.loading = false;
    }
});
