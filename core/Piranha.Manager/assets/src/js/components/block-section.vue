<template>
    <div class="card-body">
        <div :id="'content-blocks-' + section.id" :class="{ 'blocks-outline': outline }">
            <a href="#" class="block-add unsortable" v-on:click.prevent="piranha.blockpicker.open(add, 0)">
                <hr>
                <i class="fas fa-plus-circle"></i>
            </a>
            <div v-for="(block, index) in section.blocks" v-bind:key="block.meta.uid">
                    <div :class="'block ' + block.meta.component + (block.meta.isCollapsed ? ' collapsed' : '') + (block.meta.width === 'full' ? ' block-full' : '')">
                    <div :id="'tb-' + block.meta.uid" class="component-toolbar"></div>
                    <div class="block-header">
                        <div class="title">
                            <i :class="block.meta.icon"></i><strong>{{ block.meta.name }}</strong> <span v-if="!block.meta.isGroup && block.meta.isCollapsed">- {{ block.meta.title }}</span>
                        </div>
                        <div class="actions">
                            <span v-on:click.prevent="collapse(block)" class="btn btn-sm">
                                <i v-if="block.meta.isCollapsed" class="fas fa-chevron-down"></i>
                                <i v-else class="fas fa-chevron-up"></i>
                            </span>
                            <span class="btn btn-sm handle">
                                <i class="fas fa-ellipsis-v"></i>
                            </span>
                            <button v-on:click.prevent="remove(block)" class="btn btn-sm danger block-remove" tabindex="-1">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </div>
                    <component v-if="!block.meta.isGroup" v-bind:is="block.meta.component" v-bind:uid="block.meta.uid" v-bind:toolbar="'tb-' + block.meta.uid" v-bind:model="block.model" v-on:update-title='updateTitle($event)'></component>
                    <component v-if="block.meta.isGroup" v-bind:is="block.meta.component" v-bind:uid="block.meta.uid" v-bind:toolbar="'tb-' + block.meta.uid" v-bind:model="block"></component>
                    <div class="content-blocker"></div>
                </div>
                <a href="#" class="block-add" v-on:click.prevent="piranha.blockpicker.open(add, index + 1)">
                    <hr>
                    <i class="fas fa-plus-circle"></i>
                </a>
            </div>
            <div v-if="section.blocks.length == 0" class="empty-info">
                <p>@Localizer.Page["Welcome to your new page. Click on the button above to add your first block of content!"]</p>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["section", "outline"],
    methods: {
        add: function (type, pos) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.section.blocks.splice(pos, 0, result.body);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        collapse: function (block) {
            block.meta.isCollapsed = !block.meta.isCollapsed;
        },
        move: function (from, to) {
            this.section.blocks.splice(to, 0, this.section.blocks.splice(from, 1)[0]);
        },
        remove: function (block) {
            var index = this.section.blocks.indexOf(block);

            if (index !== -1) {
                this.section.blocks.splice(index, 1);
            }
        },
        updateTitle: function (e) {
            for (var n = 0; n < this.section.blocks.length; n++) {
                if (this.section.blocks[n].meta.uid === e.uid) {
                    this.section.blocks[n].meta.title = e.title;
                    break;
                }
            }
        }
    },
    mounted: function () {
        var self = this;

        sortable("#content-blocks-" + this.section.id, {
            handle: ".handle",
            items: ":not(.unsortable)"
        })[0].addEventListener("sortupdate", function (e) {
            self.move(self.section.id, e.detail.origin.index, e.detail.destination.index);
        });
    },
    updated: function () {
        sortable("#content-blocks-" + this.section.id, "disable");
        sortable("#content-blocks-" + this.section.id, "enable");
    }
}
</script>