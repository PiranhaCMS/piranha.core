<template>
    <div class="block-body" :class="{ empty: isEmpty }">
        <markdown-field :uid="uid" :model="model.body" />
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    methods: {
        onBlur: function (e) {
            // Tell parent that title has been updated
            var title = this.model.body.value.replace(/(<([^>]+)>)/ig, "");
            if (title.length > 40) {
                title = title.substring(0, 40) + "...";
            }

            this.$emit('update-title', {
                uid: this.uid,
                title: title
            });
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyText(this.model.body.value);
        }
    }
}
</script>