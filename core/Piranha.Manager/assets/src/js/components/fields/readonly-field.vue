<template>
    <div class="alert alert-secondary mb-0">
        <pre class="mb-0">{{ model.value }}</pre>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"]
}
</script>