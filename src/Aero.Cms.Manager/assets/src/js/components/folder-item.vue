<template>
    <li class="dd-item expanded" :class="{ active: item.id === selected, expanded: item.isExpanded || item.items.length === 0 }" :data-id="item.id">
        <a v-if="!item.edit" class="droppable" v-on:click.prevent="Aero.media.load(item.id)" href="#" draggable="true" v-on:dragstart="Aero.media.drag($event, item)" v-on:dragover="Aero.media.dragover" v-on:dragleave="Aero.media.dragleave" v-on:drop="Aero.media.drop($event, item.id)">
            <i class="fas fa-folder"></i>{{ item.name }}
            <span class="badge badge-light float-right">{{ item.mediaCount }}</span>
        </a>
        <form v-else v-on:submit.prevent="Aero.media.updateFolder()" class="d-flex">
            <i class="fas fa-folder"></i>
            <input :id="'folder-' + item.id" type="text" v-on:keyup.esc="Aero.media.cancelEditFolder()" v-model="Aero.media.currentFolderName" class="form-control form-control-sm">
        </form>
        <ol v-if="selected === item.id && Aero.media.isAdding" class="dd-list">
            <form v-on:submit.prevent="Aero.media.addFolder()" class="d-flex">
                <i class="fas fa-folder"></i><input id="add-folder" type="text" v-on:keyup.esc="Aero.media.isAdding = false" v-model="Aero.media.folder.name" class="form-control form-control-sm">
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