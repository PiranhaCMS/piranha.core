<template>
    <div class="field html-field" :class="{ empty: isEmpty }">
        <div contenteditable="true" :id="uid" v-html="body" v-on:blur="onBlur"></div>
    </div>
</template>

<script>
export default {
    props: ["uid", "toolbar", "model", "meta"],
    data: function () {
        return {
            body: this.model.value
        };
    },
    methods: {
        onBlur: function (e) {
            this.model.value = tinyMCE.activeEditor.getContent();

            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                var title = this.model.value.replace(/(<([^>]+)>)/ig, "");
                if (title.length > 40) {
                    title = title.substring(0, 40) + "...";
                }

                this.$emit('update-title', {
                    uid: this.uid,
                    title: title
                });
            }
        },
        onChange: function (data) {
            this.model.value = data;

            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                var title = this.model.value.replace(/(<([^>]+)>)/ig, "");
                if (title.length > 40) {
                    title = title.substring(0, 40) + "...";
                }

                if (this.meta.notifyChange) {
                    this.$emit('update-title', {
                        uid: this.uid,
                        title: title
                    });
                }
            }
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyHtml(this.model.value);
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid, this.toolbar, this.onChange);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid);
    }
}
</script>