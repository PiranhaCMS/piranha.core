/*global
    piranha
*/

piranha.permissions = {
    aliases: {
        edit: false,
        delete: false
    },
    media: {
        add: false,
        addFolder: false,
        delete: false,
        deleteFolder: false,
        edit: false
    },
    pages: {
        add: false,
        delete: false,
        edit: false,
        preview: false,
        publish: false,
        save: false
    },
    posts: {
        add: false,
        delete: false,
        edit: false,
        preview: false,
        publish: false,
        save: false
    },
    sites: {
        add: false,
        delete: false,
        edit: false,
        save: false
    },

    load: function (cb) {
        var self = this;

        fetch(piranha.baseUrl + "manager/api/permissions")
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.aliases = result.aliases;
                self.media = result.media;
                self.pages = result.pages;
                self.posts = result.posts;
                self.sites = result.sites;

                if (cb)
                    cb();
            })
            .catch(function (error) {
                console.log("error:", error );

                if (cb)
                    cb();
            });
    }
};