Vue.component("region", {
    props: ["model"],
    template:
        "<div class='region' v-if='!model.meta.isCollection'>" +
        "  <div class='form-group' v-for='field in model.items[0].fields'>" +
        "    <label>{{ field.meta.name }}</label>" +
        "    <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "  </div>" +
        "</div>" +
        "<div class='list-group region-list' v-else>" +
        "  <div class='list-group-item region-list-item' v-for='item in model.items'>" +
        "    <div class='region-list-item-title'>" +
        "      <a data-toggle='collapse'>List Item Title</a>" +
        "    </div>" +
        "    <div class='form-group' v-for='field in item.fields'>" +
        "      <label>{{ field.meta.name }}</label>" +
        "      <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

Vue.component("block-group", {
    props: ["uid", "model"],
    methods: {
        selectItem: function (item) {
            for (var n = 0; n < this.model.items.length; n++) {
                if (this.model.items[n] == item) {
                    this.model.items[n].isActive = true;
                } else {
                    this.model.items[n].isActive = false;
                }
            }
        },
        removeItem: function (item) {
            var itemActive = item.isActive;
            var itemIndex = this.model.items.indexOf(item);

            this.model.items.splice(itemIndex, 1);

            if (itemActive) {
                this.selectItem(this.model.items[Math.min(itemIndex, this.model.items.length - 1)]);
            }
        },
        addGroupBlock: function (type, pos) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.model.items.push(result);
                    self.selectItem(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        updateTitle: function (e) {
            for (var n = 0; n < this.model.items.length; n++) {
                if (this.model.items[n].meta.uid === e.uid) {
                    this.model.items[n].meta.title = e.title;
                    break;
                }
            }
        },
        moveItem: function (from, to) {
            this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0])
        }
    },
    mounted: function () {
        var self = this;

        sortable("#" + this.uid + " .list-group", {
            items: ":not(.unsortable)"
        })[0].addEventListener("sortupdate", function (e) {
            self.moveItem(e.detail.origin.index, e.detail.destination.index);
        });
    },
    beforeDestroy: function () {
    },
    template:
        "<div :id='uid' class='block-group'>" +
        "  <div class='block-group-header'>" +
        "    <div class='form-group' v-for='field in model.fields'>" +
        "      <label>{{ field.meta.name }}</label>" +
        "      <component v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:model='field.model'></component>" +
        "    </div>" +
        "  </div>" +
        "  <div class='row'>" +
        "    <div class='col-md-4'>" +
        "      <div class='list-group list-group-flush'>" +
        "        <div class='list-group-item' :class='{ active: child.isActive }' v-for='child in model.items' v-bind:key='child.meta.uid'>" +
        "          <a href='#' v-on:click.prevent='selectItem(child)'>" +
        "            <div class='handle'>" +
        "              <i class='fas fa-ellipsis-v'></i>" +
        "            </div>" +
        "            {{ child.meta.title }}" +
        "          </a>" +
        "          <span class='actions'>" +
        "            <a v-on:click.prevent='removeItem(child)' href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "          </span>" +
        "        </div>" +
        "      </div>" +
        "      <button v-on:click.prevent='piranha.blockpicker.open(addGroupBlock, 0, model.type)' class='btn btn-sm btn-primary btn-labeled mt-3'><i class='fas fa-plus'></i>Add item</button>" +
        "    </div>" +
        "    <div class='col-md-8'>" +
        "      <div v-for='child in model.items' v-if='child.isActive' :class='\"block \" + child.meta.component'>" +
        "        <component v-bind:is='child.meta.component' v-bind:uid='child.meta.uid' v-bind:model='child.model' v-on:update-title='updateTitle($event)'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("html-block", {
    props: ["uid", "model"],
    data: function () {
        return {
            body: this.model.body.value
        };
    },
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyHtml(this.model.body.value);
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
        "  <div contenteditable='true' :id='uid' spellcheck='false' v-html='body' v-on:blur='onBlur'></div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("html-column-block", {
    props: ["uid", "model"],
    data: function () {
        return {
            column1: this.model.column1.value,
            column2: this.model.column2.value,
        };
    },
    methods: {
        onBlurCol1: function (e) {
            this.model.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.model.column2.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty1: function () {
            return piranha.utils.isEmptyHtml(this.model.column1.value);
        },
        isEmpty2: function () {
            return piranha.utils.isEmptyHtml(this.model.column2.value);
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
        "      <div :id='uid + 1' contenteditable='true' spellcheck='false' v-html='column1' v-on:blur='onBlurCol1'></div>" +
        "    </div>" +
        "  </div>" +
        "  <div class='col-md-6'>" +
        "    <div :class='{ empty: isEmpty2 }'>" +
        "      <div :id='uid + 2' contenteditable='true' spellcheck='false' v-html='column2' v-on:blur='onBlurCol2'></div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("image-block", {
    props: ["uid", "model"],
    methods: {
        clear: function () {
            // clear media from block
        },
        select: function () {
            piranha.mediapicker.open(this.update, "Image");
        },
        remove: function () {
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
                return piranha.utils.formatUrl("~/assets/img/empty-image.png");
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
        "        {{ model.body.media.filename }}" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("quote-block", {
    props: ["model"],
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerText;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.model.body.value);
        }
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <i class='fas fa-quote-right quote'></i>" +
        "  <p class='lead' contenteditable='true' spellcheck='false' v-model='model.body.value' v-on:blur='onBlur'></pre>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("text-block", {
    props: ["model"],
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.model.body.value);
        }
    },
    template:
        "<div :class='{ empty: isEmpty }'>" +
        "  <pre contenteditable='true' spellcheck='false' v-html='model.body.value' v-on:blur='onBlur'></pre>" +
        "</div>"
});

Vue.component("missing-block", {
    props: ["model"],
    template:
        "<div class='alert alert-danger text-center' role='alert'>No component registered for <code>{{ model.type }}</code></div>"
});

Vue.component("string-field", {
    props: ["uid", "model"],
    template:
        "<input class='form-control' type='text' v-model='model.value'>"
});

Vue.component("html-field", {
    props: ["uid", "model"],
    data: function () {
        return {
            body: this.model.value
        };
    },
    methods: {
        onBlur: function (e) {
            this.model.value = e.target.innerHTML;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyHtml(this.model.value);
        }
    },
    mounted: function () {
        console.log("html-field: mounted");
        piranha.editor.addInline(this.uid);
    },
    beforeDestroy: function () {
        console.log("html-field: beforeDestroy");
        piranha.editor.remove(this.uid);
    },
    template:
        "<div class='html-field' :class='{ empty: isEmpty }'>" +
        "  <div contenteditable='true' :id='uid' spellcheck='false' v-html='body' v-on:blur='onBlur'></div>" +
        "</div>"
});

/*global
    piranha
*/

Vue.component("image-field", {
    props: ["uid", "model"],
    methods: {
        clear: function () {
            // clear media from block
        },
        select: function () {
            piranha.mediapicker.open(this.update, "Image");
        },
        remove: function () {
            this.model.media = null;
        },
        update: function (media) {
            if (media.type === "Image") {
                this.model.id = media.id;
                this.model.media = media;

                // Tell parent that title has been updated
                this.$emit('update-title', {
                    uid: this.uid,
                    title: this.model.media.filename
                });
            } else {
                console.log("No image was selected");
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.media == null;
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
    },
    template:
        "<div class='image-field' :class='{ empty: isEmpty }'>" +
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
        "        <a href='#' v-on:click.prevent='piranha.preview.open(model.id)'>{{ model.media.filename }}</a>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});

Vue.component("missing-field", {
    props: ["meta", "model"],
    template:
        "<div class='alert alert-danger text-center' role='alert'>No component registered for <code>{{ meta.type }}</code></div>"
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
        sortOrder: 0,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaKeywords: null,
        metaDescription: null,
        published: null,
        blocks: [],
        regions: [],
        selectedRegion: "uid-blocks",
        selectedSetting: "uid-settings"
    },
    computed: {
        contentRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display != "setting";
            });
        },
        settingRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display === "setting";
            });
        },
    },
    methods: {
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.id = result.id;
                    piranha.pageedit.siteId = result.siteId;
                    piranha.pageedit.parentId = result.parentId;
                    piranha.pageedit.sortOrder = result.sortOrder;
                    piranha.pageedit.typeId = result.typeId;
                    piranha.pageedit.title = result.title;
                    piranha.pageedit.navigationTitle = result.navigationTitle;
                    piranha.pageedit.slug = result.slug;
                    piranha.pageedit.metaKeywords = result.metaKeywords;
                    piranha.pageedit.metaDescription = result.metaDescription;
                    piranha.pageedit.published = result.published;
                    piranha.pageedit.blocks = result.blocks;
                    piranha.pageedit.regions = result.regions;
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
                    piranha.pageedit.blocks.splice(pos, 0, result);
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
        },
        selectRegion: function (uid) {
            this.selectedRegion = uid;
        },
        selectSetting: function (uid) {
            this.selectedSetting = uid;
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.loading)
        {
            sortable(".blocks", {
                handle: ".handle",
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
            });
        }
        else {
            sortable(".blocks", "disable");
            sortable(".blocks", "enable");
        }

        this.loading = false;
    }
});
