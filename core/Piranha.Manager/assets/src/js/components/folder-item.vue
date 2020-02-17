<template>
    <li class="dd-item expanded" :class="{ active: item.id === selected, expanded: item.isExpanded || item.items.length === 0 }" :data-id="item.id">
        <a v-if="!item.edit" class="droppable" v-on:click.prevent="piranha.media.load(item.id)" href="#" draggable="true" v-on:dragstart="piranha.media.drag($event, item)" v-on:dragover="piranha.media.dragover" v-on:dragleave="piranha.media.dragleave" v-on:drop="piranha.media.drop($event, item.id)">
            <i class="fas fa-folder"></i>{{ item.name }}
            <span class="badge badge-light float-right">{{ item.mediaCount }}</span>
        </a>
        <form v-else v-on:submit.prevent="piranha.media.updateFolder()" class="d-inline-block">
            <i class="fas fa-folder"></i>
            <input :id="'folder-' + item.id" type="text" v-on:keyup.esc="piranha.media.cancelEditFolder()" v-model="piranha.media.currentFolderName" class="form-control form-control-sm d-inline-block w-auto">
        </form>
        <ol v-if="selected === item.id && piranha.media.isAdding" class="dd-list">
            <form v-on:submit.prevent="piranha.media.addFolder()" class="d-inline-block">
                <i class="fas fa-folder"></i><input id="add-folder" type="text" v-on:keyup.esc="piranha.media.isAdding = false" v-model="piranha.media.folder.name" class="form-control form-control-sm d-inline-block w-auto">
            </form>
        </ol>
        <ol v-if="item.items.length > 0" class="dd-list">
            <folder-item v-for="child in item.items" v-bind:key="child.id" v-bind:selected="selected" v-bind:item="child"></folder-item>
        </ol>
    </li>
</template>

<script>
export default {
    props: ["item", "selected"]
}
</script>