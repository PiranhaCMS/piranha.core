<template>
    <li class="dd-item" :class="{ expanded: item.isExpanded || item.items.length === 0 }">
        <div class="sitemap-item expanded">
            <div class="link" :class="{ readonly: item.isCopy }">
                <a v-if="!item.isCopy" :href="piranha.baseUrl + 'manager/page/copyrelative/' + item.id + '/' + piranha.pagelist.addPageId + '/' + piranha.pagelist.addAfter">
                    {{ item.title }}
                </a>
                <a href="#" v-else>
                    {{ item.title }}
                    <span v-if="item.isCopy" class="badge badge-warning">{{ piranha.resources.texts.copy }}</span>
                </a>
                <div class="content-blocker"></div>
            </div>
            <div class="type d-none d-md-block">
                {{ item.typeName }}
            </div>
        </div>
        <ol class="dd-list" v-if="item.items.length > 0">
            <pagecopy-item v-for="child in item.items" v-bind:key="child.id" v-bind:item="child"></pagecopy-item>
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