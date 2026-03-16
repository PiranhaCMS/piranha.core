<template>
    <div class="form-group form-check">
        <input type="checkbox" class="form-check-input" :id="meta.uid" v-model="model.value">
        <label class="form-check-label" :for="meta.uid">{{ meta.placeholder}}</label>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"]
}
</script>