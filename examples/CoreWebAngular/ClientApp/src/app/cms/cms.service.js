"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Observable_1 = require("rxjs/Observable");
var operators_1 = require("rxjs/operators");
var CmsService = /** @class */ (function () {
    function CmsService(http, controlerUrl) {
        this.http = http;
        this.controlerUrl = controlerUrl;
    }
    CmsService.prototype.getSiteMap = function (id) {
        var url = this.controlerUrl + "/sitemap?id=" + id;
        return this.http.get(url)
            .pipe(operators_1.map(function (res) { return res.json(); }), operators_1.catchError(this.handleError));
    };
    CmsService.prototype.getArchive = function (id, year, month, page, category, tag) {
        if (year === void 0) { year = null; }
        if (month === void 0) { month = null; }
        if (page === void 0) { page = null; }
        if (category === void 0) { category = null; }
        if (tag === void 0) { tag = null; }
        var url = this.controlerUrl + "/archive?id=" + id + "&year=" + year + "&month=" + month + "&page=" + page + "&category=" + category + "&tag=" + tag;
        return this.http.get(url)
            .pipe(operators_1.map(function (res) { return res.json(); }), operators_1.catchError(this.handleError));
    };
    CmsService.prototype.getPage = function (id) {
        var url = this.controlerUrl + "/page?id=" + id;
        return this.http.get(url)
            .pipe(operators_1.map(function (res) { return res.json(); }), operators_1.catchError(this.handleError));
    };
    CmsService.prototype.getPost = function (id) {
        var url = this.controlerUrl + "/post?id=" + id;
        return this.http.get(url)
            .pipe(operators_1.map(function (res) { return res.json(); }), operators_1.catchError(this.handleError));
    };
    CmsService.prototype.getTeaserPage = function (id) {
        var url = this.controlerUrl + "/teaserpage?id=" + id;
        return this.http.get(url)
            .pipe(operators_1.map(function (res) { return res.json(); }), operators_1.catchError(this.handleError));
    };
    CmsService.prototype.handleError = function (error) {
        var applicationError = error.headers.get('Application-Error');
        // either applicationError in header or model error in body
        if (applicationError) {
            return Observable_1.Observable.throw(applicationError);
        }
        var modelStateErrors = '';
        var serverError = error.json();
        if (!serverError.type) {
            for (var key in serverError) {
                if (serverError.hasOwnProperty(key)) {
                    if (serverError[key]) {
                        modelStateErrors += serverError[key] + '\n';
                    }
                }
            }
        }
        modelStateErrors = modelStateErrors === '' ? null : modelStateErrors;
        return Observable_1.Observable.throw(modelStateErrors || 'Server error');
    };
    return CmsService;
}());
exports.CmsService = CmsService;
//# sourceMappingURL=cms.service.js.map