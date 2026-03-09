<template>
    <div class="media-field" :class="{ empty: isEmpty }">
        <div class="media-picker">
            <div class="btn-group float-right">
                <button v-on:click.prevent="select" class="btn btn-primary text-center">
                    <i class="fas fa-plus"></i>
                </button>
                <button v-on:click.prevent="remove" class="btn btn-danger text-center">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="card text-left">
                <div class="card-body" v-if="isEmpty">
                    <span v-if="meta.placeholder != null" class="text-secondary">{{ meta.placeholder }}</span>
                    <span v-if="meta.placeholder == null" class="text-secondary">&nbsp;</span>
                </div>
                <div class="card-body" v-else>
                    <a :href="piranha.baseUrl + 'manager/content/edit/' + model.content.typeId + '/' + model.content.id" target="_blank">{{ model.content.title }}</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    methods: {
        select: function () {
            piranha.contentpicker.open(this.meta.settings.Group, this.update);
        },
        remove: function () {
            this.model.id = null;
            this.model.content = null;
        },
        update: function (content) {
            this.model.id = content.id;
            this.model.content = content;

            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                this.$emit('update-title', {
                    uid: this.uid,
                    title: this.model.content.title
                });
            }
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.content == null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.content != null) {
                return this.model.content.title;
            } else {
                return "No content selected";
            }
        };
    }
}
</script>