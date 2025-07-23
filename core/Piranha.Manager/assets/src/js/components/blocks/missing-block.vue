<template>
    <div class="alert alert-danger text-center" role="alert">No component registered for <code>{{ model.type }}</code></div>
</template>

<script>
export default {
    props: ["model"]
}
</script>