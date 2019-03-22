piranha.notifications = new Vue({
    el: "#notification-hub",
    data: {
        items: [],
    },
    methods: {
        push: function (notification) {
            notification.isVisible = false;

            piranha.notifications.items.push(notification);

            setTimeout(function () {
                notification.isVisible = true;

                if (notification.hide)
                {
                    setTimeout(function () {
                        notification.isVisible = false;

                        setTimeout(function () {
                            piranha.notifications.items.shift();
                        }, 200);
                    }, 5000);
                }
            }, 200);
        }
    }
});