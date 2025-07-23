<template>
    <li :data-id="item.id" class="dd-item" :class="{ expanded: item.isExpanded || item.items.length === 0 }">
        <div class="sitemap-item expanded">
            <div class="link">
                <span class="actions">
                    <a v-if="item.items.length > 0 && item.isExpanded" v-on:click.prevent="toggleItem(item)" class="expand" href="#"><i class="fas fa-minus"></i></a>
                    <a v-if="item.items.length > 0 && !item.isExpanded" v-on:click.prevent="toggleItem(item)" class="expand" href="#"><i class="fas fa-plus"></i></a>
                </span>
                <a href="#" v-on:click.prevent="piranha.pagepicker.select(item)">
                    {{ item.title }}
                </a>
            </div>
            <div class="type d-none d-md-block">
                {{ item.typeName }}
            </div>
        </div>
        <ol class="dd-list" v-if="item.items.length > 0">
            <page-item v-for="child in item.items" v-bind:key="child.id" v-bind:item="child">
            </page-item>
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