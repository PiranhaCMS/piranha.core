<template>
    <input class="form-control" type="text" :placeholder="meta.placeholder" v-model="model.value">
</template>

<script>
export default {
    props: ["uid", "model", "meta"]
}
</script>