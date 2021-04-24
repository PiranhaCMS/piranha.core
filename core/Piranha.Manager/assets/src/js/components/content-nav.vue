<template>
    <div class="col side-nav">
        <ul class="list-group list-group-flush list-content app" :class="{ ready: !loading }">
            <li class="list-group-item header">
                <div class="input-group">
                    <input v-model="filter" type="text" class="form-control" :placeholder="piranha.resources.texts.search">
                    <div class="input-group-append">
                        <button class="btn btn-primary btn-icon">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </div>
            </li>
            <li v-for="item in filteredItems" v-bind:key="item.id" class="list-group-item" :class="{ active: item === selectedItem}">
                <a v-on:click.prevent="select(item)" href="#">
                    <img v-if="group.listImage" class="float-left" :src="piranha.utils.formatUrl(item.imageUrl)" :alt="item.title">
                    <span>{{ item.title }}</span>
                    <span>{{ item.type }}</span>
                </a>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
    props: ["groupid", "contentid"],
    data: function () {
        return {
            id: null,
            loading: true,
            filter: null,
            group: null,
            items: [],
            selectedItem: null,
        }
    },
    computed: {
        filteredItems: function () {
            var self = this;

            if (!this.filter || this.filter === "")
                return this.items;

            var lcFilter = this.filter.toLowerCase();

            return this.items.filter(function (i) {
                return i.title.toLowerCase().indexOf(lcFilter) !== -1;
            });
        }
    },
    methods: {
        load: function () {
            self = this;
            self.loading = true;

            if (this.id === null) {
                this.id = this.contentid;
            }

            var route = "";
            if (this.id !== null) {
                route = piranha.baseUrl + "manager/api/content/" + this.id + "/listbyid";
            } else {
                route = piranha.baseUrl + "manager/api/content/" + this.groupid + "/list";
            }

            fetch(route)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.group = result.group;
                    self.items = result.items.map(function (i) {
                        var type = result.types.find(function (t) {
                            return t.id === i.typeId;
                        });

                        i.type = type.title || i.typeId;

                        if (i.id === self.id) {
                            self.selectedItem = i;
                        }
                        return i;
                    });
                    self.loading = false;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        select: function (item)
        {
            history.pushState({ id: item.id }, "", piranha.baseUrl + "manager/content/edit/" + item.id);
            piranha.content.load("content", item.id);
            this.selectedItem = item;
        },
        onSaved: function () {
            this.id = piranha.content.id;
            this.load();
        }
    },
    mounted: function () {
        this.load();
        this.eventBus.$on("onSaved", this.onSaved);
    },
    beforeDestroy: function () {
        this.eventBus.$off("onSaved");
    }
}
</script>