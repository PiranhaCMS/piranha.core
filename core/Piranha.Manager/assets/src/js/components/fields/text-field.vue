<template>
    <textarea class="form-control" rows="4" :placeholder="meta.placeholder" v-model="model.value"></textarea>
</template>

<script>
export default {
    props: ["uid", "model", "meta"]
}
</script>