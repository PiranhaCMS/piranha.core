<template>
    <li class="dd-item" :class="{ expanded: item.isExpanded || item.items.length === 0 }" :data-id="item.id">
        <div class="sitemap-item">
            <div class="handle dd-handle"><i class="fas fa-ellipsis-v"></i></div>
            <div class="link">
                <span class="actions">
                    <a v-if="item.items.length > 0 && item.isExpanded" v-on:click.prevent="toggleItem(item)" class="expand" href="#"><i class="fas fa-minus"></i></a>
                    <a v-if="item.items.length > 0 && !item.isExpanded" v-on:click.prevent="toggleItem(item)" class="expand" href="#"><i class="fas fa-plus"></i></a>
                </span>
                <a v-if="piranha.permissions.pages.edit" :href="piranha.baseUrl + item.editUrl + item.id">
                    <span v-html="item.title"></span>
                    <span v-if="item.status" class="badge badge-info">{{ item.status }}</span>
                    <span v-if="item.isCopy" class="badge badge-warning">{{ piranha.resources.texts.copy }}</span>
                </a>
                <span v-else class="title">
                    <span v-html="item.title"></span>
                    <span v-if="item.status" class="badge badge-info">{{ item.status }}</span>
                    <span v-if="item.isCopy" class="badge badge-warning">{{ piranha.resources.texts.copy }}</span>
                </span>
            </div>
            <div class="type d-none d-md-block">{{ item.typeName }}</div>
            <div class="date d-none d-lg-block">{{ item.published }}</div>
            <div class="actions">
                <a v-if="piranha.permissions.pages.add" href="#" v-on:click.prevent="piranha.pagelist.add(item.siteId, item.id, true)"><i class="fas fa-angle-down"></i></a>
                <a v-if="piranha.permissions.pages.add" href="#" v-on:click.prevent="piranha.pagelist.add(item.siteId, item.id, false)"><i class="fas fa-angle-right"></i></a>
                <a v-if="piranha.permissions.pages.delete && item.items.length === 0" v-on:click.prevent="piranha.pagelist.remove(item.id)" class="danger" href="#"><i class="fas fa-trash"></i></a>
            </div>
        </div>
        <ol v-if="item.items.length > 0" class="dd-list">
            <sitemap-item v-for="child in item.items" v-bind:key="child.id" v-bind:item="child">
            </sitemap-item>
        </ol>
    </li>
</template>

<script>
export default {
    props: ["item"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    }
}
</script>