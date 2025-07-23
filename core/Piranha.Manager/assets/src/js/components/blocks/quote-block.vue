<template>
    <div class="block-body">
        <blockquote class="blockquote">
            <p contenteditable="true" class="blockquote-body" v-html="model.body.value" v-on:blur="onBodyBlur" :data-placeholder="placeholder.body"></p>
            <footer contenteditable="true" class="blockquote-footer" v-html="model.author.value" v-on:blur="onAuthorBlur" :data-placeholder="placeholder.author"></footer>
        </blockquote>
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    data: function () {
        return {
            placeholder: {
                body: "",
                author: ""
            }
        }
    },
    methods: {
        onAuthorBlur: function (e) {
            this.model.author.value = e.target.innerText;
        },
        onBodyBlur: function (e) {
            this.model.body.value = e.target.innerText;

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
    created: function () {
        var quotes = [
            {
                author: "Nelson Mandela",
                body: "The greatest glory in living lies not in never falling, but in rising every time we fall."
            },
            {
                author: "Walt Disney",
                body: "The way to get started is to quit talking and begin doing."
            },
            {
                author: "Eleanor Roosevelt",
                body: "The future belongs to those who believe in the beauty of their dreams."
            },
            {
                author: "John Lennon",
                body: "Life is what happens when you're busy making other plans."
            },
            {
                author: "Audrey Hepburn",
                body: "Nothing is impossible, the word itself says, 'I'm possible!'"
            },
            {
                author: "Mark Twain",
                body: "Twenty years from now you will be more disappointed by the things that you didn't do than by the ones you did do."
            },
            {
                author: "Maya Angelou",
                body: "You will face many defeats in life, but never let yourself be defeated."
            },
        ];

        this.placeholder = quotes[Math.floor(Math.random() * quotes.length)];
    }
}
</script>