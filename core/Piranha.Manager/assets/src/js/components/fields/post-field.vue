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
                    <a href="#">{{ model.post.title }}</a>
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
            piranha.postpicker.open(this.update);
        },
        remove: function () {
            this.model.id = null;
            this.model.post = null;
        },
        update: function (post) {
            this.model.id = post.id;
            this.model.post = post;

            // Tell parent that title has been updated
            this.$emit('update-title', {
                uid: this.uid,
                title: this.model.post.title
            });
        }
    },
    computed: {
        isEmpty: function () {
            return this.model.post == null;
        }
    },
    mounted: function() {
        this.model.getTitle = function () {
            if (this.model.post != null) {
                return this.model.post.title;
            } else {
                return "No post selected";
            }
        };
    }
}
</script>