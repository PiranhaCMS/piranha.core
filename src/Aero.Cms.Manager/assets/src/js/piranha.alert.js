/*global
    piranha
*/

piranha.alert = new Vue({
    el: "#alert",
    data: {
        title: null,
        body: null,
        confirmCss: null,
        confirmIcon: null,
        confirmText: null,
        cancelCss: null,
        cancelIcon: null,
        cancelText: null,
        onConfirm: null,
        onCancel: null,
        verifyPhrase: null,
        verifyPlaceholder: null,
        verifyText: null,
        verifyInput: null,
    },
    methods: {
        open: function (options) {
            if (options) {
                this.title = options.title;
                this.body = options.body;
                this.confirmCss = options.confirmCss ? options.confirmCss : "btn-success";
                this.confirmIcon = options.confirmIcon;
                this.confirmText = options.confirmText ? options.confirmText : piranha.resources.texts.ok;
                this.cancelCss = options.cancelCss ? options.cancelCss : "btn-secondary";
                this.cancelIcon = options.cancelIcon;
                this.cancelText = options.cancelText ? options.cancelText : piranha.resources.texts.cancel;
                this.onConfirm = options.onConfirm;
                this.onCancel = options.onCancel;
                this.verifyPhrase = options.verifyPhrase;
                this.verifyPlaceholder = options.verifyPlaceholder;
                this.verifyText = options.verifyText;

                $("#alert").modal("show");
            }
        },
        confirm: function () {
            if (this.onConfirm) {
                this.onConfirm();
                this.clear();
            }
            $("#alert").modal("hide");
        },
        cancel: function () {
            if (this.onCancel) {
                this.onCancel();
                this.clear();
            }
            $("#alert").modal("hide");
        },
        canConfirm: function () {
            return !this.verifyPhrase || this.verifyPhrase === this.verifyInput;
        },
        clear: function () {
            this.onCancel = null;
            this.onConfirm = null;
            this.verifyInput = null;
        }
    }
});