<template>
    <div class="alert alert-danger text-center" role="alert">No component registered for <code>{{ meta.type }}</code></div>
</template>

<script>
export default {
    props: ["meta", "model"],
}
</script>