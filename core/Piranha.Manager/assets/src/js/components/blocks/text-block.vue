<template>
    <div class="block-body" :class="{ empty: isEmpty }">
        <pre class="invisible" v-html="model.body.value"></pre>
        <textarea v-model="model.body.value" v-on:blur="onBlur"></textarea>
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    methods: {
        onBlur: function (e) {
            // this.model.body.value = e.target.innerHTML;

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