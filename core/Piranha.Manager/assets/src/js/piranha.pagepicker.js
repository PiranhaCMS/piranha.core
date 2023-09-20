/*global
    piranha
*/

piranha.pagepicker = new Vue({
    el: "#pagepicker",
    data: {
        search: '',
        sites: [],
        items: [],
        currentSiteId: null,
        currentSiteTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            let self = this;
            
            if (self.search && self.search.length < 1) {
                return this.items;   
            }
            
            let items = Object.assign([], this.items);
            let searchTerm = self.search ? self.search.toLowerCase() : "";
            
            let filterRecursive = function(arr) {
                return arr.reduce(function(acc, item){
                    let newItem = Object.assign({}, item);
                    
                    if (newItem.items) {
                        newItem.items = filterRecursive(item.items);
                        newItem.isExpanded = newItem.items.length > 0;
                    }
                    
                    if (newItem.title && (newItem.title.toLowerCase().indexOf(searchTerm) > -1 || newItem.isExpanded)) {
                        acc.push(newItem);
                    }
                    
                    return acc;
                }, []);
            };
            
            return filterRecursive(items);
        }
    },
    methods: {
        load: function (siteId) {
            var url = piranha.baseUrl + "manager/api/page/sitemap" + (siteId ? "/" + siteId : "");
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentSiteId = result.siteId;
                    self.currentSiteTitle = result.siteTitle;
                    self.sites = result.sites;
                    self.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(piranha.pagepicker.currentSiteId);
        },
        open: function (callback, siteId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId);

            $("#pagepicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#pagepicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#pagepicker").on("shown.bs.modal", function() {
        $("#pagepickerSearch").trigger("focus");
    });
});