/*global
    piranha
*/

piranha.permissions = {
    loaded: false,
    aliases: {
        edit: false,
        delete: false
    },
    comments: {
        approve: false,
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

        if (!this.loaded) {
            fetch(piranha.baseUrl + "manager/api/permissions")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.aliases = result.aliases;
                    self.comments = result.comments;
                    self.media = result.media;
                    self.pages = result.pages;
                    self.posts = result.posts;
                    self.sites = result.sites;
                    self.loaded = true;

                    if (cb)
                        cb();
                })
                .catch(function (error) {
                    console.log("error:", error );

                    if (cb)
                        cb();
                });
        } else {
            if (cb)
                cb();
        }
    }
};