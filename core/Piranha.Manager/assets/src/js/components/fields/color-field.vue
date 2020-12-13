<template>
    <div class="input-group color-field">
        <div class="input-group-prepend">
            <div class="color-preview" :style="{ backgroundColor: model.value }"></div>
            <input class="form-control" type="color" v-model="model.value">
        </div>
        <input class="form-control" type="text" v-model="model.value" :placeholder="meta.placeholder">
    </div>    
</template>

<script>
export default {
    props: ["uid", "model", "meta"]
}
</script>