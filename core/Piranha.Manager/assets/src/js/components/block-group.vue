<template>
    <div :id="uid" class="block-group">
        <div v-if="model.fields.length > 0" class="actions block-group-actions">
            <button v-on:click.prevent="toggleHeader()" class="btn btn-sm" :class="{ selected: model.meta.showHeader }">
                <i class="fas fa-list"></i>
            </button>
        </div>
        <div class="block-group-header">
            <div v-if="model.meta.showHeader" class="row">
                <div class="form-group" :class="{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }" v-bind:key="field.meta.id" v-for="field in model.fields">
                    <label>{{ field.meta.name }}</label>
                    <div v-if="field.meta.description != null" v-html="field.meta.description" class="field-description small text-muted"></div>
                    <component v-bind:is="field.meta.component" v-bind:uid="field.meta.uid" v-bind:meta="field.meta" v-bind:toolbar="toolbar" v-bind:model="field.model"></component>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-4">
                <div class="list-group list-group-flush">
                    <div class="list-group-item" :class="{ active: child.isActive }" v-for="child in model.items" v-bind:key="child.meta.uid">
                        <a href="#" v-on:click.prevent="selectItem(child)">
                            <div class="handle">
                                <i class="fas fa-ellipsis-v"></i>
                            </div>
                            {{ child.meta.title }}
                        </a>
                        <span class='actions'>
                            <a v-on:click.prevent="removeItem(child)" href="#" class="danger"><i class="fas fa-trash"></i></a>
                        </span>
                    </div>
                </div>
                <button v-on:click.prevent="piranha.blockpicker.open(addGroupBlock, 0, model.type)" class="btn btn-sm btn-primary btn-labeled mt-3">
                    <i class="fas fa-plus"></i>{{ piranha.resources.texts.add }}
                </button>
            </div>
            <div class='col-md-8'>
                <div v-if="model.items.length === 0" class="empty-info unsortable">
                    <p>{{ piranha.resources.texts.emptyAddLeft }}</p>
                </div>
                <template v-for="child in model.items">
                    <div class="block" :class="child.meta.component" v-if="child.isActive" v-bind:key="'details-' + child.meta.uid">
                        <component v-bind:is="child.meta.component" v-bind:uid="child.meta.uid" v-bind:toolbar="toolbar" v-bind:model="child.model" v-on:update-title="updateTitle($event)"></component>
                    </div>
                </template>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["uid", "toolbar", "model"],
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
                    self.model.items.push(result.body);
                    self.selectItem(result.body);
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
        toggleHeader: function () {
            this.model.meta.showHeader = !this.model.meta.showHeader;
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
    }
}
</script>