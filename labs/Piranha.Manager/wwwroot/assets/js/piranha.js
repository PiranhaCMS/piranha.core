/*
 * HTML5Sortable package
 * https://github.com/lukasoppermann/html5sortable
 *
 * Maintained by Lukas Oppermann <lukas@vea.re>
 *
 * Released under the MIT license.
 */
var sortable = (function () {
    'use strict';

    /**
     * Get or set data on element
     * @param {HTMLElement} element
     * @param {string} key
     * @param {any} value
     * @return {*}
     */
    function addData(element, key, value) {
        if (value === undefined) {
            return element && element.h5s && element.h5s.data && element.h5s.data[key];
        }
        else {
            element.h5s = element.h5s || {};
            element.h5s.data = element.h5s.data || {};
            element.h5s.data[key] = value;
        }
    }
    /**
     * Remove data from element
     * @param {HTMLElement} element
     */
    function removeData(element) {
        if (element.h5s) {
            delete element.h5s.data;
        }
    }

    function _filter (nodes, selector) {
        if (!(nodes instanceof NodeList || nodes instanceof HTMLCollection || nodes instanceof Array)) {
            throw new Error('You must provide a nodeList/HTMLCollection/Array of elements to be filtered.');
        }
        if (typeof selector !== 'string') {
            return Array.from(nodes);
        }
        return Array.from(nodes).filter(function (item) { return item.nodeType === 1 && item.matches(selector); });
    }

    /* eslint-env browser */
    var stores = new Map();
    /**
     * Stores data & configurations per Sortable
     * @param {Object} config
     */
    var Store = (function () {
        function Store() {
            this._config = new Map(); // eslint-disable-line no-undef
            this._placeholder = undefined; // eslint-disable-line no-undef
            this._data = new Map(); // eslint-disable-line no-undef
        }
        Object.defineProperty(Store.prototype, "config", {
            /**
             * get the configuration map of a class instance
             * @method config
             * @return {object}
             */
            get: function () {
                // transform Map to object
                var config = {};
                this._config.forEach(function (value, key) {
                    config[key] = value;
                });
                // return object
                return config;
            },
            /**
             * set the configuration of a class instance
             * @method config
             * @param {object} config object of configurations
             */
            set: function (config) {
                if (typeof config !== 'object') {
                    throw new Error('You must provide a valid configuration object to the config setter.');
                }
                // combine config with default
                var mergedConfig = Object.assign({}, config);
                // add config to map
                this._config = new Map(Object.entries(mergedConfig));
            },
            enumerable: true,
            configurable: true
        });
        /**
         * set individual configuration of a class instance
         * @method setConfig
         * @param  key valid configuration key
         * @param  value any value
         * @return void
         */
        Store.prototype.setConfig = function (key, value) {
            if (!this._config.has(key)) {
                throw new Error("Trying to set invalid configuration item: " + key);
            }
            // set config
            this._config.set(key, value);
        };
        /**
         * get an individual configuration of a class instance
         * @method getConfig
         * @param  key valid configuration key
         * @return any configuration value
         */
        Store.prototype.getConfig = function (key) {
            if (!this._config.has(key)) {
                throw new Error("Invalid configuration item requested: " + key);
            }
            return this._config.get(key);
        };
        Object.defineProperty(Store.prototype, "placeholder", {
            /**
             * get the placeholder for a class instance
             * @method placeholder
             * @return {HTMLElement|null}
             */
            get: function () {
                return this._placeholder;
            },
            /**
             * set the placeholder for a class instance
             * @method placeholder
             * @param {HTMLElement} placeholder
             * @return {void}
             */
            set: function (placeholder) {
                if (!(placeholder instanceof HTMLElement) && placeholder !== null) {
                    throw new Error('A placeholder must be an html element or null.');
                }
                this._placeholder = placeholder;
            },
            enumerable: true,
            configurable: true
        });
        /**
         * set an data entry
         * @method setData
         * @param {string} key
         * @param {any} value
         * @return {void}
         */
        Store.prototype.setData = function (key, value) {
            if (typeof key !== 'string') {
                throw new Error("The key must be a string.");
            }
            this._data.set(key, value);
        };
        /**
         * get an data entry
         * @method getData
         * @param {string} key an existing key
         * @return {any}
         */
        Store.prototype.getData = function (key) {
            if (typeof key !== 'string') {
                throw new Error("The key must be a string.");
            }
            return this._data.get(key);
        };
        /**
         * delete an data entry
         * @method deleteData
         * @param {string} key an existing key
         * @return {boolean}
         */
        Store.prototype.deleteData = function (key) {
            if (typeof key !== 'string') {
                throw new Error("The key must be a string.");
            }
            return this._data.delete(key);
        };
        return Store;
    }());
    function store (sortableElement) {
        // if sortableElement is wrong type
        if (!(sortableElement instanceof HTMLElement)) {
            throw new Error('Please provide a sortable to the store function.');
        }
        // create new instance if not avilable
        if (!stores.has(sortableElement)) {
            stores.set(sortableElement, new Store());
        }
        // return instance
        return stores.get(sortableElement);
    }

    /**
     * @param {Array|HTMLElement} element
     * @param {Function} callback
     * @param {string} event
     */
    function addEventListener(element, eventName, callback) {
        if (element instanceof Array) {
            for (var i = 0; i < element.length; ++i) {
                addEventListener(element[i], eventName, callback);
            }
            return;
        }
        element.addEventListener(eventName, callback);
        store(element).setData("event" + eventName, callback);
    }
    /**
     * @param {Array<HTMLElement>|HTMLElement} element
     * @param {string} eventName
     */
    function removeEventListener(element, eventName) {
        if (element instanceof Array) {
            for (var i = 0; i < element.length; ++i) {
                removeEventListener(element[i], eventName);
            }
            return;
        }
        element.removeEventListener(eventName, store(element).getData("event" + eventName));
        store(element).deleteData("event" + eventName);
    }

    /**
     * @param {Array<HTMLElement>|HTMLElement} element
     * @param {string} attribute
     * @param {string} value
     */
    function addAttribute(element, attribute, value) {
        if (element instanceof Array) {
            for (var i = 0; i < element.length; ++i) {
                addAttribute(element[i], attribute, value);
            }
            return;
        }
        element.setAttribute(attribute, value);
    }
    /**
     * @param {Array|HTMLElement} element
     * @param {string} attribute
     */
    function removeAttribute(element, attribute) {
        if (element instanceof Array) {
            for (var i = 0; i < element.length; ++i) {
                removeAttribute(element[i], attribute);
            }
            return;
        }
        element.removeAttribute(attribute);
    }

    function offset (element) {
        if (!element.parentElement || element.getClientRects().length === 0) {
            throw new Error('target element must be part of the dom');
        }
        var rect = element.getClientRects()[0];
        return {
            left: rect.left + window.pageXOffset,
            right: rect.right + window.pageXOffset,
            top: rect.top + window.pageYOffset,
            bottom: rect.bottom + window.pageYOffset
        };
    }

    function _debounce (func, wait) {
        if (wait === void 0) { wait = 0; }
        var timeout;
        return function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i - 0] = arguments[_i];
            }
            clearTimeout(timeout);
            timeout = setTimeout(function () {
                func.apply(void 0, args);
            }, wait);
        };
    }

    function index (element, elementList) {
        if (!(element instanceof HTMLElement) || !(elementList instanceof NodeList || elementList instanceof HTMLCollection || elementList instanceof Array)) {
            throw new Error('You must provide an element and a list of elements.');
        }
        return Array.from(elementList).indexOf(element);
    }

    function isInDom (element) {
        if (!(element instanceof HTMLElement)) {
            throw new Error('Element is not a node element.');
        }
        return element.parentNode !== null;
    }

    /* eslint-env browser */
    /**
     * Insert node before or after target
     * @param {HTMLElement} referenceNode - reference element
     * @param {HTMLElement} newElement - element to be inserted
     * @param {String} position - insert before or after reference element
     */
    var insertNode = function (referenceNode, newElement, position) {
        if (!(referenceNode instanceof HTMLElement) || !(referenceNode.parentElement instanceof HTMLElement)) {
            throw new Error('target and element must be a node');
        }
        referenceNode.parentElement.insertBefore(newElement, (position === 'before' ? referenceNode : referenceNode.nextElementSibling));
    };
    /**
     * Insert before target
     * @param {HTMLElement} target
     * @param {HTMLElement} element
     */
    var insertBefore = function (target, element) { return insertNode(target, element, 'before'); };
    /**
     * Insert after target
     * @param {HTMLElement} target
     * @param {HTMLElement} element
     */
    var insertAfter = function (target, element) { return insertNode(target, element, 'after'); };

    function _serialize (sortableContainer, customItemSerializer, customContainerSerializer) {
        if (customItemSerializer === void 0) { customItemSerializer = function (serializedItem, sortableContainer) { return serializedItem; }; }
        if (customContainerSerializer === void 0) { customContainerSerializer = function (serializedContainer) { return serializedContainer; }; }
        // check for valid sortableContainer
        if (!(sortableContainer instanceof HTMLElement) || !sortableContainer.isSortable === true) {
            throw new Error('You need to provide a sortableContainer to be serialized.');
        }
        // check for valid serializers
        if (typeof customItemSerializer !== 'function' || typeof customContainerSerializer !== 'function') {
            throw new Error('You need to provide a valid serializer for items and the container.');
        }
        // get options
        var options = addData(sortableContainer, 'opts');
        var item = options.items;
        // serialize container
        var items = _filter(sortableContainer.children, item);
        var serializedItems = items.map(function (item) {
            return {
                parent: sortableContainer,
                node: item,
                html: item.outerHTML,
                index: index(item, items)
            };
        });
        // serialize container
        var container = {
            node: sortableContainer,
            itemCount: serializedItems.length
        };
        return {
            container: customContainerSerializer(container),
            items: serializedItems.map(function (item) { return customItemSerializer(item, sortableContainer); })
        };
    }

    function _makePlaceholder (sortableElement, placeholder, placeholderClass) {
        if (placeholderClass === void 0) { placeholderClass = 'sortable-placeholder'; }
        if (!(sortableElement instanceof HTMLElement)) {
            throw new Error('You must provide a valid element as a sortable.');
        }
        // if placeholder is not an element
        if (!(placeholder instanceof HTMLElement) && placeholder !== undefined) {
            throw new Error('You must provide a valid element as a placeholder or set ot to undefined.');
        }
        // if no placeholder element is given
        if (placeholder === undefined) {
            if (['UL', 'OL'].includes(sortableElement.tagName)) {
                placeholder = document.createElement('li');
            }
            else if (['TABLE', 'TBODY'].includes(sortableElement.tagName)) {
                placeholder = document.createElement('tr');
                // set colspan to always all rows, otherwise the item can only be dropped in first column
                placeholder.innerHTML = '<td colspan="100"></td>';
            }
            else {
                placeholder = document.createElement('div');
            }
        }
        // add classes to placeholder
        if (typeof placeholderClass === 'string') {
            (_a = placeholder.classList).add.apply(_a, placeholderClass.split(' '));
        }
        return placeholder;
        var _a;
    }

    function _getElementHeight (element) {
        if (!(element instanceof HTMLElement)) {
            throw new Error('You must provide a valid dom element');
        }
        // get calculated style of element
        var style = window.getComputedStyle(element);
        // pick applicable properties, convert to int and reduce by adding
        return ['height', 'padding-top', 'padding-bottom']
            .map(function (key) {
            var int = parseInt(style.getPropertyValue(key), 10);
            return isNaN(int) ? 0 : int;
        })
            .reduce(function (sum, value) { return sum + value; });
    }

    /* eslint-env browser */
    /**
     * get handle or return item
     * @param {Array<HTMLElement>} items
     * @param {string} selector
     */
    function _getHandles (items, selector) {
        if (!(items instanceof Array)) {
            throw new Error('You must provide a Array of HTMLElements to be filtered.');
        }
        if (typeof selector !== 'string') {
            return items;
        }
        return items
            .filter(function (item) {
            return item.querySelector(selector) instanceof HTMLElement ||
                (item.shadowRoot && item.shadowRoot.querySelector(selector) instanceof HTMLElement);
        })
            .map(function (item) {
            return item.querySelector(selector) || (item.shadowRoot && item.shadowRoot.querySelector(selector));
        });
    }

    function getEventTarget (event) {
        return (event.composedPath && event.composedPath()[0]) || event.target;
    }

    /**
     * defaultDragImage returns the current item as dragged image
     * @param {HTMLElement} draggedElement - the item that the user drags
     * @param {object} elementOffset - an object with the offsets top, left, right & bottom
     * @param {Event} event - the original drag event object
     * @return {object} with element, posX and posY properties
     */
    var defaultDragImage = function (draggedElement, elementOffset, event) {
        return {
            element: draggedElement,
            posX: event.pageX - elementOffset.left,
            posY: event.pageY - elementOffset.top
        };
    };
    function setDragImage (event, draggedElement, customDragImage) {
        // check if event is provided
        if (!(event instanceof Event)) {
            throw new Error('setDragImage requires a DragEvent as the first argument.');
        }
        // check if draggedElement is provided
        if (!(draggedElement instanceof HTMLElement)) {
            throw new Error('setDragImage requires the dragged element as the second argument.');
        }
        // set default function of none provided
        if (!customDragImage) {
            customDragImage = defaultDragImage;
        }
        // check if setDragImage method is available
        if (event.dataTransfer && event.dataTransfer.setDragImage) {
            // get the elements offset
            var elementOffset = offset(draggedElement);
            // get the dragImage
            var dragImage = customDragImage(draggedElement, elementOffset, event);
            // check if custom function returns correct values
            if (!(dragImage.element instanceof HTMLElement) || typeof dragImage.posX !== 'number' || typeof dragImage.posY !== 'number') {
                throw new Error('The customDragImage function you provided must return and object with the properties element[string], posX[integer], posY[integer].');
            }
            // needs to be set for HTML5 drag & drop to work
            event.dataTransfer.effectAllowed = 'copyMove';
            // Firefox requires it to use the event target's id for the data
            event.dataTransfer.setData('text/plain', getEventTarget(event).id);
            // set the drag image on the event
            event.dataTransfer.setDragImage(dragImage.element, dragImage.posX, dragImage.posY);
        }
    }

    function _listsConnected (destination, origin) {
        // check if valid sortable
        if (destination.isSortable === true) {
            var acceptFrom = store(destination).getConfig('acceptFrom');
            // check if acceptFrom is valid
            if (acceptFrom !== null && acceptFrom !== false && typeof acceptFrom !== 'string') {
                throw new Error('HTML5Sortable: Wrong argument, "acceptFrom" must be "null", "false", or a valid selector string.');
            }
            if (acceptFrom !== null) {
                return acceptFrom !== false && acceptFrom.split(',').filter(function (sel) {
                    return sel.length > 0 && origin.matches(sel);
                }).length > 0;
            }
            // drop in same list
            if (destination === origin) {
                return true;
            }
            // check if lists are connected with connectWith
            if (store(destination).getConfig('connectWith') !== undefined && store(destination).getConfig('connectWith') !== null) {
                return store(destination).getConfig('connectWith') === store(origin).getConfig('connectWith');
            }
        }
        return false;
    }

    var defaultConfiguration = {
        items: null,
        // deprecated
        connectWith: null,
        // deprecated
        disableIEFix: null,
        acceptFrom: null,
        copy: false,
        placeholder: null,
        placeholderClass: 'sortable-placeholder',
        draggingClass: 'sortable-dragging',
        hoverClass: false,
        debounce: 0,
        throttleTime: 100,
        maxItems: 0,
        itemSerializer: undefined,
        containerSerializer: undefined,
        customDragImage: null
    };

    /**
     * make sure a function is only called once within the given amount of time
     * @param {Function} fn the function to throttle
     * @param {number} threshold time limit for throttling
     */
    // must use function to keep this context
    function _throttle (fn, threshold) {
        var _this = this;
        if (threshold === void 0) { threshold = 250; }
        // check function
        if (typeof fn !== 'function') {
            throw new Error('You must provide a function as the first argument for throttle.');
        }
        // check threshold
        if (typeof threshold !== 'number') {
            throw new Error('You must provide a number as the second argument for throttle.');
        }
        var lastEventTimestamp = null;
        return function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i - 0] = arguments[_i];
            }
            var now = Date.now();
            if (lastEventTimestamp === null || now - lastEventTimestamp >= threshold) {
                lastEventTimestamp = now;
                fn.apply(_this, args);
            }
        };
    }

    function enableHoverClass (sortableContainer, enable) {
        if (typeof store(sortableContainer).getConfig('hoverClass') === 'string') {
            var hoverClasses_1 = store(sortableContainer).getConfig('hoverClass').split(' ');
            // add class on hover
            if (enable === true) {
                addEventListener(sortableContainer, 'mousemove', _throttle(function (event) {
                    // check of no mouse button was pressed when mousemove started == no drag
                    if (event.buttons === 0) {
                        _filter(sortableContainer.children, store(sortableContainer).getConfig('items')).forEach(function (item) {
                            if (item !== event.target) {
                                (_a = item.classList).remove.apply(_a, hoverClasses_1);
                            }
                            else {
                                (_b = item.classList).add.apply(_b, hoverClasses_1);
                            }
                            var _a, _b;
                        });
                    }
                }, store(sortableContainer).getConfig('throttleTime')));
                // remove class on leave
                addEventListener(sortableContainer, 'mouseleave', function () {
                    _filter(sortableContainer.children, store(sortableContainer).getConfig('items')).forEach(function (item) {
                        (_a = item.classList).remove.apply(_a, hoverClasses_1);
                        var _a;
                    });
                });
            }
            else {
                removeEventListener(sortableContainer, 'mousemove');
                removeEventListener(sortableContainer, 'mouseleave');
            }
        }
    }

    /* eslint-env browser */
    /*
     * variables global to the plugin
     */
    var dragging;
    var draggingHeight;
    /*
     * Keeps track of the initialy selected list, where 'dragstart' event was triggered
     * It allows us to move the data in between individual Sortable List instances
     */
    // Origin List - data from before any item was changed
    var originContainer;
    var originIndex;
    var originElementIndex;
    var originItemsBeforeUpdate;
    // Previous Sortable Container - we dispatch as sortenter event when a
    // dragged item enters a sortableContainer for the first time
    var previousContainer;
    // Destination List - data from before any item was changed
    var destinationItemsBeforeUpdate;
    /**
     * remove event handlers from items
     * @param {Array|NodeList} items
     */
    var _removeItemEvents = function (items) {
        removeEventListener(items, 'dragstart');
        removeEventListener(items, 'dragend');
        removeEventListener(items, 'dragover');
        removeEventListener(items, 'dragenter');
        removeEventListener(items, 'drop');
        removeEventListener(items, 'mouseenter');
        removeEventListener(items, 'mouseleave');
    };
    /**
     * _getDragging returns the current element to drag or
     * a copy of the element.
     * Is Copy Active for sortable
     * @param {HTMLElement} draggedItem - the item that the user drags
     * @param {HTMLElement} sortable a single sortable
     */
    var _getDragging = function (draggedItem, sortable) {
        var ditem = draggedItem;
        if (store(sortable).getConfig('copy') === true) {
            ditem = draggedItem.cloneNode(true);
            addAttribute(ditem, 'aria-copied', 'true');
            draggedItem.parentElement.appendChild(ditem);
            ditem.style.display = 'none';
            ditem.oldDisplay = draggedItem.style.display;
        }
        return ditem;
    };
    /**
     * Remove data from sortable
     * @param {HTMLElement} sortable a single sortable
     */
    var _removeSortableData = function (sortable) {
        removeData(sortable);
        removeAttribute(sortable, 'aria-dropeffect');
    };
    /**
     * Remove data from items
     * @param {Array<HTMLElement>|HTMLElement} items
     */
    var _removeItemData = function (items) {
        removeAttribute(items, 'aria-grabbed');
        removeAttribute(items, 'aria-copied');
        removeAttribute(items, 'draggable');
        removeAttribute(items, 'role');
    };
    /**
     * find sortable from element. travels up parent element until found or null.
     * @param {HTMLElement} element a single sortable
     * @param {Event} event - the current event. We need to pass it to be able to
     * find Sortable whith shadowRoot (document fragment has no parent)
     */
    function findSortable(element, event) {
        if (event.composedPath) {
            return event.composedPath().find(function (el) { return el.isSortable; });
        }
        while (element.isSortable !== true) {
            element = element.parentElement;
        }
        return element;
    }
    /**
     * Dragging event is on the sortable element. finds the top child that
     * contains the element.
     * @param {HTMLElement} sortableElement a single sortable
     * @param {HTMLElement} element is that being dragged
     */
    function findDragElement(sortableElement, element) {
        var options = addData(sortableElement, 'opts');
        var items = _filter(sortableElement.children, options.items);
        var itemlist = items.filter(function (ele) {
            return ele.contains(element) || (ele.shadowRoot && ele.shadowRoot.contains(element));
        });
        return itemlist.length > 0 ? itemlist[0] : element;
    }
    /**
     * Destroy the sortable
     * @param {HTMLElement} sortableElement a single sortable
     */
    var _destroySortable = function (sortableElement) {
        var opts = addData(sortableElement, 'opts') || {};
        var items = _filter(sortableElement.children, opts.items);
        var handles = _getHandles(items, opts.handle);
        // remove event handlers & data from sortable
        removeEventListener(sortableElement, 'dragover');
        removeEventListener(sortableElement, 'dragenter');
        removeEventListener(sortableElement, 'drop');
        // remove event data from sortable
        _removeSortableData(sortableElement);
        // remove event handlers & data from items
        removeEventListener(handles, 'mousedown');
        _removeItemEvents(items);
        _removeItemData(items);
    };
    /**
     * Enable the sortable
     * @param {HTMLElement} sortableElement a single sortable
     */
    var _enableSortable = function (sortableElement) {
        var opts = addData(sortableElement, 'opts');
        var items = _filter(sortableElement.children, opts.items);
        var handles = _getHandles(items, opts.handle);
        addAttribute(sortableElement, 'aria-dropeffect', 'move');
        addData(sortableElement, '_disabled', 'false');
        addAttribute(handles, 'draggable', 'true');
        // @todo: remove this fix
        // IE FIX for ghost
        // can be disabled as it has the side effect that other events
        // (e.g. click) will be ignored
        if (opts.disableIEFix === false) {
            var spanEl = (document || window.document).createElement('span');
            if (typeof spanEl.dragDrop === 'function') {
                addEventListener(handles, 'mousedown', function () {
                    if (items.indexOf(this) !== -1) {
                        this.dragDrop();
                    }
                    else {
                        var parent = this.parentElement;
                        while (items.indexOf(parent) === -1) {
                            parent = parent.parentElement;
                        }
                        parent.dragDrop();
                    }
                });
            }
        }
    };
    /**
     * Disable the sortable
     * @param {HTMLElement} sortableElement a single sortable
     */
    var _disableSortable = function (sortableElement) {
        var opts = addData(sortableElement, 'opts');
        var items = _filter(sortableElement.children, opts.items);
        var handles = _getHandles(items, opts.handle);
        addAttribute(sortableElement, 'aria-dropeffect', 'none');
        addData(sortableElement, '_disabled', 'true');
        addAttribute(handles, 'draggable', 'false');
        removeEventListener(handles, 'mousedown');
    };
    /**
     * Reload the sortable
     * @param {HTMLElement} sortableElement a single sortable
     * @description events need to be removed to not be double bound
     */
    var _reloadSortable = function (sortableElement) {
        var opts = addData(sortableElement, 'opts');
        var items = _filter(sortableElement.children, opts.items);
        var handles = _getHandles(items, opts.handle);
        addData(sortableElement, '_disabled', 'false');
        // remove event handlers from items
        _removeItemEvents(items);
        removeEventListener(handles, 'mousedown');
        // remove event handlers from sortable
        removeEventListener(sortableElement, 'dragover');
        removeEventListener(sortableElement, 'dragenter');
        removeEventListener(sortableElement, 'drop');
    };
    /**
     * Public sortable object
     * @param {Array|NodeList} sortableElements
     * @param {object|string} options|method
     */
    function sortable(sortableElements, options) {
        // get method string to see if a method is called
        var method = String(options);
        options = options || {};
        // check if the user provided a selector instead of an element
        if (typeof sortableElements === 'string') {
            sortableElements = document.querySelectorAll(sortableElements);
        }
        // if the user provided an element, return it in an array to keep the return value consistant
        if (sortableElements instanceof HTMLElement) {
            sortableElements = [sortableElements];
        }
        sortableElements = Array.prototype.slice.call(sortableElements);
        if (/serialize/.test(method)) {
            return sortableElements.map(function (sortableContainer) {
                var opts = addData(sortableContainer, 'opts');
                return _serialize(sortableContainer, opts.itemSerializer, opts.containerSerializer);
            });
        }
        sortableElements.forEach(function (sortableElement) {
            if (/enable|disable|destroy/.test(method)) {
                return sortable[method](sortableElement);
            }
            // log deprecation
            ['connectWith', 'disableIEFix'].forEach(function (configKey) {
                if (options.hasOwnProperty(configKey) && options[configKey] !== null) {
                    console.warn("HTML5Sortable: You are using the deprecated configuration \"" + configKey + "\". This will be removed in an upcoming version, make sure to migrate to the new options when updating.");
                }
            });
            // merge options with default options
            options = Object.assign({}, defaultConfiguration, store(sortableElement).config, options);
            // init data store for sortable
            store(sortableElement).config = options;
            // set options on sortable
            addData(sortableElement, 'opts', options);
            // property to define as sortable
            sortableElement.isSortable = true;
            // reset sortable
            _reloadSortable(sortableElement);
            // initialize
            var listItems = _filter(sortableElement.children, options.items);
            // create element if user defined a placeholder element as a string
            var customPlaceholder;
            if (options.placeholder !== null && options.placeholder !== undefined) {
                var tempContainer = document.createElement(sortableElement.tagName);
                if (options.placeholder instanceof HTMLElement) {
                    tempContainer.appendChild(options.placeholder);
                }
                else {
                    tempContainer.innerHTML = options.placeholder;
                }
                customPlaceholder = tempContainer.children[0];
            }
            // add placeholder
            store(sortableElement).placeholder = _makePlaceholder(sortableElement, customPlaceholder, options.placeholderClass);
            addData(sortableElement, 'items', options.items);
            if (options.acceptFrom) {
                addData(sortableElement, 'acceptFrom', options.acceptFrom);
            }
            else if (options.connectWith) {
                addData(sortableElement, 'connectWith', options.connectWith);
            }
            _enableSortable(sortableElement);
            addAttribute(listItems, 'role', 'option');
            addAttribute(listItems, 'aria-grabbed', 'false');
            // enable hover class
            enableHoverClass(sortableElement, true);
            /*
             Handle drag events on draggable items
             Handle is set at the sortableElement level as it will bubble up
             from the item
             */
            addEventListener(sortableElement, 'dragstart', function (e) {
                // ignore dragstart events
                var target = getEventTarget(e);
                if (target.isSortable === true) {
                    return;
                }
                e.stopImmediatePropagation();
                if ((options.handle && !target.matches(options.handle)) || target.getAttribute('draggable') === 'false') {
                    return;
                }
                var sortableContainer = findSortable(target, e);
                var dragItem = findDragElement(sortableContainer, target);
                // grab values
                originItemsBeforeUpdate = _filter(sortableContainer.children, options.items);
                originIndex = originItemsBeforeUpdate.indexOf(dragItem);
                originElementIndex = index(dragItem, sortableContainer.children);
                originContainer = sortableContainer;
                // add transparent clone or other ghost to cursor
                setDragImage(e, dragItem, options.customDragImage);
                // cache selsection & add attr for dragging
                draggingHeight = _getElementHeight(dragItem);
                dragItem.classList.add(options.draggingClass);
                dragging = _getDragging(dragItem, sortableContainer);
                addAttribute(dragging, 'aria-grabbed', 'true');
                // dispatch sortstart event on each element in group
                sortableContainer.dispatchEvent(new CustomEvent('sortstart', {
                    detail: {
                        origin: {
                            elementIndex: originElementIndex,
                            index: originIndex,
                            container: originContainer
                        },
                        item: dragging,
                        originalTarget: target
                    }
                }));
            });
            /*
             We are capturing targetSortable before modifications with 'dragenter' event
            */
            addEventListener(sortableElement, 'dragenter', function (e) {
                var target = getEventTarget(e);
                var sortableContainer = findSortable(target, e);
                if (sortableContainer && sortableContainer !== previousContainer) {
                    destinationItemsBeforeUpdate = _filter(sortableContainer.children, addData(sortableContainer, 'items'))
                        .filter(function (item) { return item !== store(sortableElement).placeholder; });
                    sortableContainer.dispatchEvent(new CustomEvent('sortenter', {
                        detail: {
                            origin: {
                                elementIndex: originElementIndex,
                                index: originIndex,
                                container: originContainer
                            },
                            destination: {
                                container: sortableContainer,
                                itemsBeforeUpdate: destinationItemsBeforeUpdate
                            },
                            item: dragging,
                            originalTarget: target
                        }
                    }));
                }
                previousContainer = sortableContainer;
            });
            /*
             * Dragend Event - https://developer.mozilla.org/en-US/docs/Web/Events/dragend
             * Fires each time dragEvent end, or ESC pressed
             * We are using it to clean up any draggable elements and placeholders
             */
            addEventListener(sortableElement, 'dragend', function (e) {
                if (!dragging) {
                    return;
                }
                dragging.classList.remove(options.draggingClass);
                addAttribute(dragging, 'aria-grabbed', 'false');
                if (dragging.getAttribute('aria-copied') === 'true' && addData(dragging, 'dropped') !== 'true') {
                    dragging.remove();
                }
                dragging.style.display = dragging.oldDisplay;
                delete dragging.oldDisplay;
                var visiblePlaceholder = Array.from(stores.values()).map(function (data) { return data.placeholder; })
                    .filter(function (placeholder) { return placeholder instanceof HTMLElement; })
                    .filter(isInDom)[0];
                if (visiblePlaceholder) {
                    visiblePlaceholder.remove();
                }
                // dispatch sortstart event on each element in group
                sortableElement.dispatchEvent(new CustomEvent('sortstop', {
                    detail: {
                        origin: {
                            elementIndex: originElementIndex,
                            index: originIndex,
                            container: originContainer
                        },
                        item: dragging
                    }
                }));
                previousContainer = null;
                dragging = null;
                draggingHeight = null;
            });
            /*
             * Drop Event - https://developer.mozilla.org/en-US/docs/Web/Events/drop
             * Fires when valid drop target area is hit
             */
            addEventListener(sortableElement, 'drop', function (e) {
                if (!_listsConnected(sortableElement, dragging.parentElement)) {
                    return;
                }
                e.preventDefault();
                e.stopPropagation();
                addData(dragging, 'dropped', 'true');
                // get the one placeholder that is currently visible
                var visiblePlaceholder = Array.from(stores.values()).map(function (data) {
                    return data.placeholder;
                })
                    .filter(function (placeholder) { return placeholder instanceof HTMLElement; })
                    .filter(isInDom)[0];
                // attach element after placeholder
                insertAfter(visiblePlaceholder, dragging);
                // remove placeholder from dom
                visiblePlaceholder.remove();
                /*
                 * Fires Custom Event - 'sortstop'
                 */
                sortableElement.dispatchEvent(new CustomEvent('sortstop', {
                    detail: {
                        origin: {
                            elementIndex: originElementIndex,
                            index: originIndex,
                            container: originContainer
                        },
                        item: dragging
                    }
                }));
                var placeholder = store(sortableElement).placeholder;
                var originItems = _filter(originContainer.children, options.items)
                    .filter(function (item) { return item !== placeholder; });
                var destinationContainer = this.isSortable === true ? this : this.parentElement;
                var destinationItems = _filter(destinationContainer.children, addData(destinationContainer, 'items'))
                    .filter(function (item) { return item !== placeholder; });
                var destinationElementIndex = index(dragging, Array.from(dragging.parentElement.children)
                    .filter(function (item) { return item !== placeholder; }));
                var destinationIndex = index(dragging, destinationItems);
                /*
                 * When a list item changed container lists or index within a list
                 * Fires Custom Event - 'sortupdate'
                 */
                if (originElementIndex !== destinationElementIndex || originContainer !== destinationContainer) {
                    sortableElement.dispatchEvent(new CustomEvent('sortupdate', {
                        detail: {
                            origin: {
                                elementIndex: originElementIndex,
                                index: originIndex,
                                container: originContainer,
                                itemsBeforeUpdate: originItemsBeforeUpdate,
                                items: originItems
                            },
                            destination: {
                                index: destinationIndex,
                                elementIndex: destinationElementIndex,
                                container: destinationContainer,
                                itemsBeforeUpdate: destinationItemsBeforeUpdate,
                                items: destinationItems
                            },
                            item: dragging
                        }
                    }));
                }
            });
            var debouncedDragOverEnter = _debounce(function (sortableElement, element, pageY) {
                if (!dragging) {
                    return;
                }
                // set placeholder height if forcePlaceholderSize option is set
                if (options.forcePlaceholderSize) {
                    store(sortableElement).placeholder.style.height = draggingHeight + 'px';
                }
                // if element the draggedItem is dragged onto is within the array of all elements in list
                // (not only items, but also disabled, etc.)
                if (Array.from(sortableElement.children).indexOf(element) > -1) {
                    var thisHeight = _getElementHeight(element);
                    var placeholderIndex = index(store(sortableElement).placeholder, element.parentElement.children);
                    var thisIndex = index(element, element.parentElement.children);
                    // Check if `element` is bigger than the draggable. If it is, we have to define a dead zone to prevent flickering
                    if (thisHeight > draggingHeight) {
                        // Dead zone?
                        var deadZone = thisHeight - draggingHeight;
                        var offsetTop = offset(element).top;
                        if (placeholderIndex < thisIndex && pageY < offsetTop) {
                            return;
                        }
                        if (placeholderIndex > thisIndex &&
                            pageY > offsetTop + thisHeight - deadZone) {
                            return;
                        }
                    }
                    if (dragging.oldDisplay === undefined) {
                        dragging.oldDisplay = dragging.style.display;
                    }
                    if (dragging.style.display !== 'none') {
                        dragging.style.display = 'none';
                    }
                    // To avoid flicker, determine where to position the placeholder
                    // based on where the mouse pointer is relative to the elements
                    // vertical center.
                    var placeAfter = false;
                    try {
                        var elementMiddle = offset(element).top + element.offsetHeight / 2;
                        placeAfter = pageY >= elementMiddle;
                    }
                    catch (e) {
                        placeAfter = placeholderIndex < thisIndex;
                    }
                    if (placeAfter) {
                        insertAfter(element, store(sortableElement).placeholder);
                    }
                    else {
                        insertBefore(element, store(sortableElement).placeholder);
                    }
                    // get placeholders from all stores & remove all but current one
                    Array.from(stores.values())
                        .filter(function (data) { return data.placeholder !== undefined; })
                        .forEach(function (data) {
                        if (data.placeholder !== store(sortableElement).placeholder) {
                            data.placeholder.remove();
                        }
                    });
                }
                else {
                    // get all placeholders from store
                    var placeholders = Array.from(stores.values())
                        .filter(function (data) { return data.placeholder !== undefined; })
                        .map(function (data) {
                        return data.placeholder;
                    });
                    // check if element is not in placeholders
                    if (placeholders.indexOf(element) === -1 && sortableElement === element && !_filter(element.children, options.items).length) {
                        placeholders.forEach(function (element) { return element.remove(); });
                        element.appendChild(store(sortableElement).placeholder);
                    }
                }
            }, options.debounce);
            // Handle dragover and dragenter events on draggable items
            var onDragOverEnter = function (e) {
                var element = e.target;
                var sortableElement = element.isSortable === true ? element : findSortable(element, e);
                element = findDragElement(sortableElement, element);
                if (!dragging || !_listsConnected(sortableElement, dragging.parentElement) || addData(sortableElement, '_disabled') === 'true') {
                    return;
                }
                var options = addData(sortableElement, 'opts');
                if (parseInt(options.maxItems) && _filter(sortableElement.children, addData(sortableElement, 'items')).length >= parseInt(options.maxItems) && dragging.parentElement !== sortableElement) {
                    return;
                }
                e.preventDefault();
                e.stopPropagation();
                e.dataTransfer.dropEffect = store(sortableElement).getConfig('copy') === true ? 'copy' : 'move';
                debouncedDragOverEnter(sortableElement, element, e.pageY);
            };
            addEventListener(listItems.concat(sortableElement), 'dragover', onDragOverEnter);
            addEventListener(listItems.concat(sortableElement), 'dragenter', onDragOverEnter);
        });
        return sortableElements;
    }
    sortable.destroy = function (sortableElement) {
        _destroySortable(sortableElement);
    };
    sortable.enable = function (sortableElement) {
        _enableSortable(sortableElement);
    };
    sortable.disable = function (sortableElement) {
        _disableSortable(sortableElement);
    };

    return sortable;

}());

!function(t,e,s,i){function o(e,o){this.w=t(s),this.el=t(e),(o=o||a).rootClass!==i&&"dd"!==o.rootClass&&(o.listClass=o.listClass?o.listClass:o.rootClass+"-list",o.itemClass=o.itemClass?o.itemClass:o.rootClass+"-item",o.dragClass=o.dragClass?o.dragClass:o.rootClass+"-dragel",o.handleClass=o.handleClass?o.handleClass:o.rootClass+"-handle",o.collapsedClass=o.collapsedClass?o.collapsedClass:o.rootClass+"-collapsed",o.placeClass=o.placeClass?o.placeClass:o.rootClass+"-placeholder",o.noDragClass=o.noDragClass?o.noDragClass:o.rootClass+"-nodrag",o.noChildrenClass=o.noChildrenClass?o.noChildrenClass:o.rootClass+"-nochildren",o.emptyClass=o.emptyClass?o.emptyClass:o.rootClass+"-empty"),this.options=t.extend({},a,o),this.options.json!==i&&this._build(),this.init()}var n="ontouchstart"in s,l=function(){var t=s.createElement("div"),i=s.documentElement;if(!("pointerEvents"in t.style))return!1;t.style.pointerEvents="auto",t.style.pointerEvents="x",i.appendChild(t);var o=e.getComputedStyle&&"auto"===e.getComputedStyle(t,"").pointerEvents;return i.removeChild(t),!!o}(),a={contentCallback:function(t){return t.content?t.content:t.id},listNodeName:"ol",itemNodeName:"li",handleNodeName:"div",contentNodeName:"span",rootClass:"dd",listClass:"dd-list",itemClass:"dd-item",dragClass:"dd-dragel",handleClass:"dd-handle",contentClass:"dd-content",collapsedClass:"dd-collapsed",placeClass:"dd-placeholder",noDragClass:"dd-nodrag",noChildrenClass:"dd-nochildren",emptyClass:"dd-empty",expandBtnHTML:'<button class="dd-expand" data-action="expand" type="button">Expand</button>',collapseBtnHTML:'<button class="dd-collapse" data-action="collapse" type="button">Collapse</button>',group:0,maxDepth:5,threshold:20,fixedDepth:!1,fixed:!1,includeContent:!1,scroll:!1,scrollSensitivity:1,scrollSpeed:5,scrollTriggers:{top:40,left:40,right:-40,bottom:-40},effect:{animation:"none",time:"slow"},callback:function(t,e,s){},onDragStart:function(t,e,s){},beforeDragStop:function(t,e,s){},listRenderer:function(t,e){var s="<"+e.listNodeName+' class="'+e.listClass+'">';return s+=t,s+="</"+e.listNodeName+">"},itemRenderer:function(e,s,i,o,n){var l=t.map(e,function(t,e){return" "+e+'="'+t+'"'}).join(" "),a="<"+o.itemNodeName+l+">";return a+="<"+o.handleNodeName+' class="'+o.handleClass+'">',a+="<"+o.contentNodeName+' class="'+o.contentClass+'">',a+=s,a+="</"+o.contentNodeName+">",a+="</"+o.handleNodeName+">",a+=i,a+="</"+o.itemNodeName+">"}};o.prototype={init:function(){var s=this;s.reset(),s.el.data("nestable-group",this.options.group),s.placeEl=t('<div class="'+s.options.placeClass+'"/>');var i=this.el.find(s.options.itemNodeName);t.each(i,function(e,i){var o=t(i),n=o.parent();s.setParent(o),n.hasClass(s.options.collapsedClass)&&s.collapseItem(n.parent())}),i.length||this.appendEmptyElement(this.el),s.el.on("click","button",function(e){if(!s.dragEl){var i=t(e.currentTarget),o=i.data("action"),n=i.parents(s.options.itemNodeName).eq(0);"collapse"===o&&s.collapseItem(n),"expand"===o&&s.expandItem(n)}});var o=function(e){var i=t(e.target);if(!i.hasClass(s.options.handleClass)){if(i.closest("."+s.options.noDragClass).length)return;i=i.closest("."+s.options.handleClass)}i.length&&!s.dragEl&&(s.isTouch=/^touch/.test(e.type),s.isTouch&&1!==e.touches.length||(e.preventDefault(),s.dragStart(e.touches?e.touches[0]:e)))},l=function(t){s.dragEl&&(t.preventDefault(),s.dragMove(t.touches?t.touches[0]:t))},a=function(t){s.dragEl&&(t.preventDefault(),s.dragStop(t.touches?t.changedTouches[0]:t))};n&&(s.el[0].addEventListener("touchstart",o,!1),e.addEventListener("touchmove",l,!1),e.addEventListener("touchend",a,!1),e.addEventListener("touchcancel",a,!1)),s.el.on("mousedown",o),s.w.on("mousemove",l),s.w.on("mouseup",a);s.el.bind("destroy-nestable",function(){n&&(s.el[0].removeEventListener("touchstart",o,!1),e.removeEventListener("touchmove",l,!1),e.removeEventListener("touchend",a,!1),e.removeEventListener("touchcancel",a,!1)),s.el.off("mousedown",o),s.w.off("mousemove",l),s.w.off("mouseup",a),s.el.off("click"),s.el.unbind("destroy-nestable"),s.el.data("nestable",null)})},destroy:function(){this.el.trigger("destroy-nestable")},add:function(e){var s="."+this.options.listClass,o=t(this.el).children(s);e.parent_id!==i&&(o=o.find('[data-id="'+e.parent_id+'"]'),delete e.parent_id,0===o.children(s).length&&(o=o.append(this.options.listRenderer("",this.options))),o=o.find(s+":first"),this.setParent(o.parent())),o.append(this._buildItem(e,this.options))},replace:function(t){var e=this._buildItem(t,this.options);this._getItemById(t.id).replaceWith(e)},removeItem:function(e){var s=this.options,i=this.el;(e=e||this).remove();var o="."+s.listClass+" ."+s.listClass+":not(:has(*))";t(i).find(o).remove();t(i).find('[data-action="expand"], [data-action="collapse"]').each(function(){0===t(this).siblings("."+s.listClass).length&&t(this).remove()})},remove:function(t,e){var s=this.options,i=this,o=this._getItemById(t),n=s.effect.animation||"fade",l=s.effect.time||"slow";"fade"===n?o.fadeOut(l,function(){i.removeItem(o)}):this.removeItem(o),e&&e()},removeAll:function(e){function s(){l.each(function(){i.removeItem(t(this))}),n.show(),e&&e()}var i=this,o=this.options,n=i.el.find(o.listNodeName).first(),l=n.children(o.itemNodeName),a=o.effect.animation||"fade",r=o.effect.time||"slow";"fade"===a?n.fadeOut(r,s):s()},_getItemById:function(e){return t(this.el).children("."+this.options.listClass).find('[data-id="'+e+'"]')},_build:function(){var e=this.options.json;"string"==typeof e&&(e=JSON.parse(e)),t(this.el).html(this._buildList(e,this.options))},_buildList:function(e,s){if(!e)return"";var i="",o=this;return t.each(e,function(t,e){i+=o._buildItem(e,s)}),s.listRenderer(i,s)},_buildItem:function(e,s){function i(t){var e={"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#039;"};return t+"".replace(/[&<>"']/g,function(t){return e[t]})}function o(t){var e={};for(var s in t)e[t[s]]=t[s];return e}var n=function(e){delete(e=t.extend({},e)).children,delete e.classes,delete e.content;var s={};return t.each(e,function(t,e){"object"==typeof e&&(e=JSON.stringify(e)),s["data-"+t]=i(e)}),s}(e);n.class=function(e,s){var i=e.classes||{};"string"==typeof i&&(i=[i]);var n=o(i);return n[s.itemClass]=s.itemClass,t.map(n,function(t){return t}).join(" ")}(e,s);var l=s.contentCallback(e),a=this._buildList(e.children,s),r=t(s.itemRenderer(n,l,a,s,e));return this.setParent(r),r[0].outerHTML},serialize:function(){var e=this,s=function(i){var o=[];return i.children(e.options.itemNodeName).each(function(){var i=t(this),n=t.extend({},i.data()),l=i.children(e.options.listNodeName);if(e.options.includeContent){var a=i.find("."+e.options.contentClass).html();a&&(n.content=a)}l.length&&(n.children=s(l)),o.push(n)}),o};return s(e.el.find(e.options.listNodeName).first())},asNestedSet:function(){function e(i,l,a){var r,d,h=a+1;return t(i).children(o.listNodeName).children(o.itemNodeName).length>0&&(l++,t(i).children(o.listNodeName).children(o.itemNodeName).each(function(){h=e(t(this),l,h)}),l--),r=t(i).attr("data-id"),s(r)&&(r=parseInt(r)),d=t(i).parent(o.listNodeName).parent(o.itemNodeName).attr("data-id")||"",s(d)&&(r=parseInt(d)),r&&n.push({id:r,parent_id:d,depth:l,lft:a,rgt:h}),a=h+1}function s(e){return t.isNumeric(e)&&Math.floor(e)==e}var i=this,o=i.options,n=[],l=1;return i.el.find(o.listNodeName).first().children(o.itemNodeName).each(function(){l=e(this,0,l)}),n=n.sort(function(t,e){return t.lft-e.lft})},returnOptions:function(){return this.options},serialise:function(){return this.serialize()},toHierarchy:function(e){function s(e){var o=(t(e).attr(i.attribute||"id")||"").match(i.expression||/(.+)[-=_](.+)/);if(o){var n={id:o[2]};return t(e).children(i.listType).children(i.items).length>0&&(n.children=[],t(e).children(i.listType).children(i.items).each(function(){var t=s(this);n.children.push(t)})),n}}var i=t.extend({},this.options,e),o=[];return t(this.element).children(i.items).each(function(){var t=s(this);o.push(t)}),o},toArray:function(){function e(n,l,a){var r,d,h=a+1;return n.children(s.options.listNodeName).children(s.options.itemNodeName).length>0&&(l++,n.children(s.options.listNodeName).children(s.options.itemNodeName).each(function(){h=e(t(this),l,h)}),l--),r=n.data().id,d=l===i+1?s.rootID:n.parent(s.options.listNodeName).parent(s.options.itemNodeName).data().id,r&&o.push({id:r,parent_id:d,depth:l,left:a,right:h}),a=h+1}var s=t.extend({},this.options,this),i=s.startDepthCount||0,o=[],n=2,l=this;return l.el.find(l.options.listNodeName).first().children(l.options.itemNodeName).each(function(){n=e(t(this),i+1,n)}),o=o.sort(function(t,e){return t.left-e.left})},reset:function(){this.mouse={offsetX:0,offsetY:0,startX:0,startY:0,lastX:0,lastY:0,nowX:0,nowY:0,distX:0,distY:0,dirAx:0,dirX:0,dirY:0,lastDirX:0,lastDirY:0,distAxX:0,distAxY:0},this.isTouch=!1,this.moving=!1,this.dragEl=null,this.dragRootEl=null,this.dragDepth=0,this.hasNewRoot=!1,this.pointEl=null},expandItem:function(t){t.removeClass(this.options.collapsedClass)},collapseItem:function(t){t.children(this.options.listNodeName).length&&t.addClass(this.options.collapsedClass)},expandAll:function(){var e=this;e.el.find(e.options.itemNodeName).each(function(){e.expandItem(t(this))})},collapseAll:function(){var e=this;e.el.find(e.options.itemNodeName).each(function(){e.collapseItem(t(this))})},setParent:function(e){e.is(this.options.itemNodeName)&&e.children(this.options.listNodeName).length&&(e.children("[data-action]").remove(),e.prepend(t(this.options.expandBtnHTML)),e.prepend(t(this.options.collapseBtnHTML)))},unsetParent:function(t){t.removeClass(this.options.collapsedClass),t.children("[data-action]").remove(),t.children(this.options.listNodeName).remove()},dragStart:function(e){var i=this.mouse,o=t(e.target).closest(this.options.itemNodeName),n={top:e.pageY,left:e.pageX},l=this.options.onDragStart.call(this,this.el,o,n);if(void 0===l||!1!==l){this.placeEl.css("height",o.height()),i.offsetX=e.pageX-o.offset().left,i.offsetY=e.pageY-o.offset().top,i.startX=i.lastX=e.pageX,i.startY=i.lastY=e.pageY,this.dragRootEl=this.el,this.dragEl=t(s.createElement(this.options.listNodeName)).addClass(this.options.listClass+" "+this.options.dragClass),this.dragEl.css("width",o.outerWidth()),this.setIndexOfItem(o),o.after(this.placeEl),o[0].parentNode.removeChild(o[0]),o.appendTo(this.dragEl),t(s.body).append(this.dragEl),this.dragEl.css({left:e.pageX-i.offsetX,top:e.pageY-i.offsetY});var a,r,d=this.dragEl.find(this.options.itemNodeName);for(a=0;a<d.length;a++)(r=t(d[a]).parents(this.options.listNodeName).length)>this.dragDepth&&(this.dragDepth=r)}},createSubLevel:function(e,s){var i=t("<"+this.options.listNodeName+"/>").addClass(this.options.listClass);return s&&i.append(s),e.append(i),this.setParent(e),i},setIndexOfItem:function(e,s){(s=s||[]).unshift(e.index()),t(e[0].parentNode)[0]!==this.dragRootEl[0]?this.setIndexOfItem(t(e[0].parentNode),s):this.dragEl.data("indexOfItem",s)},restoreItemAtIndex:function(e,s){for(var i=this.el,o=s.length-1,n=0;n<s.length;n++){if(o===parseInt(n))return void function(e,i){0===s[o]?t(e).prepend(i.clone(!0)):t(e.children[s[o]-1]).after(i.clone(!0))}(i,e);var l=i[0]?i[0]:i,a=l.children[s[n]];i=a||this.createSubLevel(t(l))}},dragStop:function(t){var e={top:t.pageY,left:t.pageX},s=this.dragEl.data("indexOfItem"),i=this.dragEl.children(this.options.itemNodeName).first();i[0].parentNode.removeChild(i[0]),this.dragEl.remove();var o=this.options.beforeDragStop.call(this,this.el,i,this.placeEl.parent());if(void 0!==o&&!1===o){var n=this.placeEl.parent();return this.placeEl.remove(),n.children().length||this.unsetParent(n.parent()),this.restoreItemAtIndex(i,s),void this.reset()}this.placeEl.replaceWith(i),this.hasNewRoot?(!0===this.options.fixed?this.restoreItemAtIndex(i,s):this.el.trigger("lostItem"),this.dragRootEl.trigger("gainedItem")):this.dragRootEl.trigger("change"),this.options.callback.call(this,this.dragRootEl,i,e),this.reset()},dragMove:function(i){var o,n,a,r=this.options,d=this.mouse;this.dragEl.css({left:i.pageX-d.offsetX,top:i.pageY-d.offsetY}),d.lastX=d.nowX,d.lastY=d.nowY,d.nowX=i.pageX,d.nowY=i.pageY,d.distX=d.nowX-d.lastX,d.distY=d.nowY-d.lastY,d.lastDirX=d.dirX,d.lastDirY=d.dirY,d.dirX=0===d.distX?0:d.distX>0?1:-1,d.dirY=0===d.distY?0:d.distY>0?1:-1;var h=Math.abs(d.distX)>Math.abs(d.distY)?1:0;if(!d.moving)return d.dirAx=h,void(d.moving=!0);if(r.scroll)if(void 0!==e.jQuery.fn.scrollParent){var c=!1,p=this.el.scrollParent()[0];p!==s&&"HTML"!==p.tagName?(r.scrollTriggers.bottom+p.offsetHeight-i.pageY<r.scrollSensitivity?p.scrollTop=c=p.scrollTop+r.scrollSpeed:i.pageY-r.scrollTriggers.top<r.scrollSensitivity&&(p.scrollTop=c=p.scrollTop-r.scrollSpeed),r.scrollTriggers.right+p.offsetWidth-i.pageX<r.scrollSensitivity?p.scrollLeft=c=p.scrollLeft+r.scrollSpeed:i.pageX-r.scrollTriggers.left<r.scrollSensitivity&&(p.scrollLeft=c=p.scrollLeft-r.scrollSpeed)):(i.pageY-t(s).scrollTop()<r.scrollSensitivity?c=t(s).scrollTop(t(s).scrollTop()-r.scrollSpeed):t(e).height()-(i.pageY-t(s).scrollTop())<r.scrollSensitivity&&(c=t(s).scrollTop(t(s).scrollTop()+r.scrollSpeed)),i.pageX-t(s).scrollLeft()<r.scrollSensitivity?c=t(s).scrollLeft(t(s).scrollLeft()-r.scrollSpeed):t(e).width()-(i.pageX-t(s).scrollLeft())<r.scrollSensitivity&&(c=t(s).scrollLeft(t(s).scrollLeft()+r.scrollSpeed)))}else console.warn("To use scrolling you need to have scrollParent() function, check documentation for more information");this.scrollTimer&&clearTimeout(this.scrollTimer),r.scroll&&c&&(this.scrollTimer=setTimeout(function(){t(e).trigger(i)},10)),d.dirAx!==h?(d.distAxX=0,d.distAxY=0):(d.distAxX+=Math.abs(d.distX),0!==d.dirX&&d.dirX!==d.lastDirX&&(d.distAxX=0),d.distAxY+=Math.abs(d.distY),0!==d.dirY&&d.dirY!==d.lastDirY&&(d.distAxY=0)),d.dirAx=h,d.dirAx&&d.distAxX>=r.threshold&&(d.distAxX=0,a=this.placeEl.prev(r.itemNodeName),d.distX>0&&a.length&&!a.hasClass(r.collapsedClass)&&!a.hasClass(r.noChildrenClass)&&(o=a.find(r.listNodeName).last(),this.placeEl.parents(r.listNodeName).length+this.dragDepth<=r.maxDepth&&(o.length?(o=a.children(r.listNodeName).last()).append(this.placeEl):this.createSubLevel(a,this.placeEl))),d.distX<0&&(this.placeEl.next(r.itemNodeName).length||(n=this.placeEl.parent(),this.placeEl.closest(r.itemNodeName).after(this.placeEl),n.children().length||this.unsetParent(n.parent()))));var f=!1;if(l||(this.dragEl[0].style.visibility="hidden"),this.pointEl=t(s.elementFromPoint(i.pageX-s.body.scrollLeft,i.pageY-(e.pageYOffset||s.documentElement.scrollTop))),l||(this.dragEl[0].style.visibility="visible"),this.pointEl.hasClass(r.handleClass)&&(this.pointEl=this.pointEl.closest(r.itemNodeName)),this.pointEl.hasClass(r.emptyClass))f=!0;else if(!this.pointEl.length||!this.pointEl.hasClass(r.itemClass))return;var u=this.pointEl.closest("."+r.rootClass),m=this.dragRootEl.data("nestable-id")!==u.data("nestable-id");if(!d.dirAx||m||f){if(m&&r.group!==u.data("nestable-group"))return;if(this.options.fixedDepth&&this.dragDepth+1!==this.pointEl.parents(r.listNodeName).length)return;if(this.dragDepth-1+this.pointEl.parents(r.listNodeName).length>r.maxDepth)return;var g=i.pageY<this.pointEl.offset().top+this.pointEl.height()/2;n=this.placeEl.parent(),f?((o=t(s.createElement(r.listNodeName)).addClass(r.listClass)).append(this.placeEl),this.pointEl.replaceWith(o)):g?this.pointEl.before(this.placeEl):this.pointEl.after(this.placeEl),n.children().length||this.unsetParent(n.parent()),this.dragRootEl.find(r.itemNodeName).length||this.appendEmptyElement(this.dragRootEl),this.dragRootEl=u,m&&(this.hasNewRoot=this.el[0]!==this.dragRootEl[0])}},appendEmptyElement:function(t){t.append('<div class="'+this.options.emptyClass+'"/>')}},t.fn.nestable=function(s){var i=this,n=this,l=arguments;return"Nestable"in e||(e.Nestable={},Nestable.counter=0),i.each(function(){var e=t(this).data("nestable");if(e){if("string"==typeof s&&"function"==typeof e[s])if(l.length>1){for(var i=[],a=1;a<l.length;a++)i.push(l[a]);n=e[s].apply(e,i)}else n=e[s]()}else Nestable.counter++,t(this).data("nestable",new o(this,s)),t(this).data("nestable-id",Nestable.counter)}),n||i}}(window.jQuery||window.Zepto,window,document);
"use strict";

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

/*
 *
 * More info at [www.dropzonejs.com](http://www.dropzonejs.com)
 *
 * Copyright (c) 2012, Matias Meno
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 */

// The Emitter class provides the ability to call `.on()` on Dropzone to listen
// to events.
// It is strongly based on component's emitter class, and I removed the
// functionality because of the dependency hell with different frameworks.
var Emitter = function () {
  function Emitter() {
    _classCallCheck(this, Emitter);
  }

  _createClass(Emitter, [{
    key: "on",

    // Add an event listener for given event
    value: function on(event, fn) {
      this._callbacks = this._callbacks || {};
      // Create namespace for this event
      if (!this._callbacks[event]) {
        this._callbacks[event] = [];
      }
      this._callbacks[event].push(fn);
      return this;
    }
  }, {
    key: "emit",
    value: function emit(event) {
      this._callbacks = this._callbacks || {};
      var callbacks = this._callbacks[event];

      if (callbacks) {
        for (var _len = arguments.length, args = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
          args[_key - 1] = arguments[_key];
        }

        for (var _iterator = callbacks, _isArray = true, _i = 0, _iterator = _isArray ? _iterator : _iterator[Symbol.iterator]();;) {
          var _ref;

          if (_isArray) {
            if (_i >= _iterator.length) break;
            _ref = _iterator[_i++];
          } else {
            _i = _iterator.next();
            if (_i.done) break;
            _ref = _i.value;
          }

          var callback = _ref;

          callback.apply(this, args);
        }
      }

      return this;
    }

    // Remove event listener for given event. If fn is not provided, all event
    // listeners for that event will be removed. If neither is provided, all
    // event listeners will be removed.

  }, {
    key: "off",
    value: function off(event, fn) {
      if (!this._callbacks || arguments.length === 0) {
        this._callbacks = {};
        return this;
      }

      // specific event
      var callbacks = this._callbacks[event];
      if (!callbacks) {
        return this;
      }

      // remove all handlers
      if (arguments.length === 1) {
        delete this._callbacks[event];
        return this;
      }

      // remove specific handler
      for (var i = 0; i < callbacks.length; i++) {
        var callback = callbacks[i];
        if (callback === fn) {
          callbacks.splice(i, 1);
          break;
        }
      }

      return this;
    }
  }]);

  return Emitter;
}();

var Dropzone = function (_Emitter) {
  _inherits(Dropzone, _Emitter);

  _createClass(Dropzone, null, [{
    key: "initClass",
    value: function initClass() {

      // Exposing the emitter class, mainly for tests
      this.prototype.Emitter = Emitter;

      /*
       This is a list of all available events you can register on a dropzone object.
        You can register an event handler like this:
        dropzone.on("dragEnter", function() { });
        */
      this.prototype.events = ["drop", "dragstart", "dragend", "dragenter", "dragover", "dragleave", "addedfile", "addedfiles", "removedfile", "thumbnail", "error", "errormultiple", "processing", "processingmultiple", "uploadprogress", "totaluploadprogress", "sending", "sendingmultiple", "success", "successmultiple", "canceled", "canceledmultiple", "complete", "completemultiple", "reset", "maxfilesexceeded", "maxfilesreached", "queuecomplete"];

      this.prototype.defaultOptions = {
        /**
         * Has to be specified on elements other than form (or when the form
         * doesn't have an `action` attribute). You can also
         * provide a function that will be called with `files` and
         * must return the url (since `v3.12.0`)
         */
        url: null,

        /**
         * Can be changed to `"put"` if necessary. You can also provide a function
         * that will be called with `files` and must return the method (since `v3.12.0`).
         */
        method: "post",

        /**
         * Will be set on the XHRequest.
         */
        withCredentials: false,

        /**
         * The timeout for the XHR requests in milliseconds (since `v4.4.0`).
         */
        timeout: 30000,

        /**
         * How many file uploads to process in parallel (See the
         * Enqueuing file uploads* documentation section for more info)
         */
        parallelUploads: 2,

        /**
         * Whether to send multiple files in one request. If
         * this it set to true, then the fallback file input element will
         * have the `multiple` attribute as well. This option will
         * also trigger additional events (like `processingmultiple`). See the events
         * documentation section for more information.
         */
        uploadMultiple: false,

        /**
         * Whether you want files to be uploaded in chunks to your server. This can't be
         * used in combination with `uploadMultiple`.
         *
         * See [chunksUploaded](#config-chunksUploaded) for the callback to finalise an upload.
         */
        chunking: false,

        /**
         * If `chunking` is enabled, this defines whether **every** file should be chunked,
         * even if the file size is below chunkSize. This means, that the additional chunk
         * form data will be submitted and the `chunksUploaded` callback will be invoked.
         */
        forceChunking: false,

        /**
         * If `chunking` is `true`, then this defines the chunk size in bytes.
         */
        chunkSize: 2000000,

        /**
         * If `true`, the individual chunks of a file are being uploaded simultaneously.
         */
        parallelChunkUploads: false,

        /**
         * Whether a chunk should be retried if it fails.
         */
        retryChunks: false,

        /**
         * If `retryChunks` is true, how many times should it be retried.
         */
        retryChunksLimit: 3,

        /**
         * If not `null` defines how many files this Dropzone handles. If it exceeds,
         * the event `maxfilesexceeded` will be called. The dropzone element gets the
         * class `dz-max-files-reached` accordingly so you can provide visual feedback.
         */
        maxFilesize: 256,

        /**
         * The name of the file param that gets transferred.
         * **NOTE**: If you have the option  `uploadMultiple` set to `true`, then
         * Dropzone will append `[]` to the name.
         */
        paramName: "file",

        /**
         * Whether thumbnails for images should be generated
         */
        createImageThumbnails: true,

        /**
         * In MB. When the filename exceeds this limit, the thumbnail will not be generated.
         */
        maxThumbnailFilesize: 10,

        /**
         * If `null`, the ratio of the image will be used to calculate it.
         */
        thumbnailWidth: 120,

        /**
         * The same as `thumbnailWidth`. If both are null, images will not be resized.
         */
        thumbnailHeight: 120,

        /**
         * How the images should be scaled down in case both, `thumbnailWidth` and `thumbnailHeight` are provided.
         * Can be either `contain` or `crop`.
         */
        thumbnailMethod: 'crop',

        /**
         * If set, images will be resized to these dimensions before being **uploaded**.
         * If only one, `resizeWidth` **or** `resizeHeight` is provided, the original aspect
         * ratio of the file will be preserved.
         *
         * The `options.transformFile` function uses these options, so if the `transformFile` function
         * is overridden, these options don't do anything.
         */
        resizeWidth: null,

        /**
         * See `resizeWidth`.
         */
        resizeHeight: null,

        /**
         * The mime type of the resized image (before it gets uploaded to the server).
         * If `null` the original mime type will be used. To force jpeg, for example, use `image/jpeg`.
         * See `resizeWidth` for more information.
         */
        resizeMimeType: null,

        /**
         * The quality of the resized images. See `resizeWidth`.
         */
        resizeQuality: 0.8,

        /**
         * How the images should be scaled down in case both, `resizeWidth` and `resizeHeight` are provided.
         * Can be either `contain` or `crop`.
         */
        resizeMethod: 'contain',

        /**
         * The base that is used to calculate the filesize. You can change this to
         * 1024 if you would rather display kibibytes, mebibytes, etc...
         * 1024 is technically incorrect, because `1024 bytes` are `1 kibibyte` not `1 kilobyte`.
         * You can change this to `1024` if you don't care about validity.
         */
        filesizeBase: 1000,

        /**
         * Can be used to limit the maximum number of files that will be handled by this Dropzone
         */
        maxFiles: null,

        /**
         * An optional object to send additional headers to the server. Eg:
         * `{ "My-Awesome-Header": "header value" }`
         */
        headers: null,

        /**
         * If `true`, the dropzone element itself will be clickable, if `false`
         * nothing will be clickable.
         *
         * You can also pass an HTML element, a CSS selector (for multiple elements)
         * or an array of those. In that case, all of those elements will trigger an
         * upload when clicked.
         */
        clickable: true,

        /**
         * Whether hidden files in directories should be ignored.
         */
        ignoreHiddenFiles: true,

        /**
         * The default implementation of `accept` checks the file's mime type or
         * extension against this list. This is a comma separated list of mime
         * types or file extensions.
         *
         * Eg.: `image/*,application/pdf,.psd`
         *
         * If the Dropzone is `clickable` this option will also be used as
         * [`accept`](https://developer.mozilla.org/en-US/docs/HTML/Element/input#attr-accept)
         * parameter on the hidden file input as well.
         */
        acceptedFiles: null,

        /**
         * **Deprecated!**
         * Use acceptedFiles instead.
         */
        acceptedMimeTypes: null,

        /**
         * If false, files will be added to the queue but the queue will not be
         * processed automatically.
         * This can be useful if you need some additional user input before sending
         * files (or if you want want all files sent at once).
         * If you're ready to send the file simply call `myDropzone.processQueue()`.
         *
         * See the [enqueuing file uploads](#enqueuing-file-uploads) documentation
         * section for more information.
         */
        autoProcessQueue: true,

        /**
         * If false, files added to the dropzone will not be queued by default.
         * You'll have to call `enqueueFile(file)` manually.
         */
        autoQueue: true,

        /**
         * If `true`, this will add a link to every file preview to remove or cancel (if
         * already uploading) the file. The `dictCancelUpload`, `dictCancelUploadConfirmation`
         * and `dictRemoveFile` options are used for the wording.
         */
        addRemoveLinks: false,

        /**
         * Defines where to display the file previews – if `null` the
         * Dropzone element itself is used. Can be a plain `HTMLElement` or a CSS
         * selector. The element should have the `dropzone-previews` class so
         * the previews are displayed properly.
         */
        previewsContainer: null,

        /**
         * This is the element the hidden input field (which is used when clicking on the
         * dropzone to trigger file selection) will be appended to. This might
         * be important in case you use frameworks to switch the content of your page.
         *
         * Can be a selector string, or an element directly.
         */
        hiddenInputContainer: "body",

        /**
         * If null, no capture type will be specified
         * If camera, mobile devices will skip the file selection and choose camera
         * If microphone, mobile devices will skip the file selection and choose the microphone
         * If camcorder, mobile devices will skip the file selection and choose the camera in video mode
         * On apple devices multiple must be set to false.  AcceptedFiles may need to
         * be set to an appropriate mime type (e.g. "image/*", "audio/*", or "video/*").
         */
        capture: null,

        /**
         * **Deprecated**. Use `renameFile` instead.
         */
        renameFilename: null,

        /**
         * A function that is invoked before the file is uploaded to the server and renames the file.
         * This function gets the `File` as argument and can use the `file.name`. The actual name of the
         * file that gets used during the upload can be accessed through `file.upload.filename`.
         */
        renameFile: null,

        /**
         * If `true` the fallback will be forced. This is very useful to test your server
         * implementations first and make sure that everything works as
         * expected without dropzone if you experience problems, and to test
         * how your fallbacks will look.
         */
        forceFallback: false,

        /**
         * The text used before any files are dropped.
         */
        dictDefaultMessage: "Drop files here to upload",

        /**
         * The text that replaces the default message text it the browser is not supported.
         */
        dictFallbackMessage: "Your browser does not support drag'n'drop file uploads.",

        /**
         * The text that will be added before the fallback form.
         * If you provide a  fallback element yourself, or if this option is `null` this will
         * be ignored.
         */
        dictFallbackText: "Please use the fallback form below to upload your files like in the olden days.",

        /**
         * If the filesize is too big.
         * `{{filesize}}` and `{{maxFilesize}}` will be replaced with the respective configuration values.
         */
        dictFileTooBig: "File is too big ({{filesize}}MiB). Max filesize: {{maxFilesize}}MiB.",

        /**
         * If the file doesn't match the file type.
         */
        dictInvalidFileType: "You can't upload files of this type.",

        /**
         * If the server response was invalid.
         * `{{statusCode}}` will be replaced with the servers status code.
         */
        dictResponseError: "Server responded with {{statusCode}} code.",

        /**
         * If `addRemoveLinks` is true, the text to be used for the cancel upload link.
         */
        dictCancelUpload: "Cancel upload",

        /**
         * The text that is displayed if an upload was manually canceled
         */
        dictUploadCanceled: "Upload canceled.",

        /**
         * If `addRemoveLinks` is true, the text to be used for confirmation when cancelling upload.
         */
        dictCancelUploadConfirmation: "Are you sure you want to cancel this upload?",

        /**
         * If `addRemoveLinks` is true, the text to be used to remove a file.
         */
        dictRemoveFile: "Remove file",

        /**
         * If this is not null, then the user will be prompted before removing a file.
         */
        dictRemoveFileConfirmation: null,

        /**
         * Displayed if `maxFiles` is st and exceeded.
         * The string `{{maxFiles}}` will be replaced by the configuration value.
         */
        dictMaxFilesExceeded: "You can not upload any more files.",

        /**
         * Allows you to translate the different units. Starting with `tb` for terabytes and going down to
         * `b` for bytes.
         */
        dictFileSizeUnits: { tb: "TB", gb: "GB", mb: "MB", kb: "KB", b: "b" },
        /**
         * Called when dropzone initialized
         * You can add event listeners here
         */
        init: function init() {},


        /**
         * Can be an **object** of additional parameters to transfer to the server, **or** a `Function`
         * that gets invoked with the `files`, `xhr` and, if it's a chunked upload, `chunk` arguments. In case
         * of a function, this needs to return a map.
         *
         * The default implementation does nothing for normal uploads, but adds relevant information for
         * chunked uploads.
         *
         * This is the same as adding hidden input fields in the form element.
         */
        params: function params(files, xhr, chunk) {
          if (chunk) {
            return {
              dzuuid: chunk.file.upload.uuid,
              dzchunkindex: chunk.index,
              dztotalfilesize: chunk.file.size,
              dzchunksize: this.options.chunkSize,
              dztotalchunkcount: chunk.file.upload.totalChunkCount,
              dzchunkbyteoffset: chunk.index * this.options.chunkSize
            };
          }
        },


        /**
         * A function that gets a [file](https://developer.mozilla.org/en-US/docs/DOM/File)
         * and a `done` function as parameters.
         *
         * If the done function is invoked without arguments, the file is "accepted" and will
         * be processed. If you pass an error message, the file is rejected, and the error
         * message will be displayed.
         * This function will not be called if the file is too big or doesn't match the mime types.
         */
        accept: function accept(file, done) {
          return done();
        },


        /**
         * The callback that will be invoked when all chunks have been uploaded for a file.
         * It gets the file for which the chunks have been uploaded as the first parameter,
         * and the `done` function as second. `done()` needs to be invoked when everything
         * needed to finish the upload process is done.
         */
        chunksUploaded: function chunksUploaded(file, done) {
          done();
        },

        /**
         * Gets called when the browser is not supported.
         * The default implementation shows the fallback input field and adds
         * a text.
         */
        fallback: function fallback() {
          // This code should pass in IE7... :(
          var messageElement = void 0;
          this.element.className = this.element.className + " dz-browser-not-supported";

          for (var _iterator2 = this.element.getElementsByTagName("div"), _isArray2 = true, _i2 = 0, _iterator2 = _isArray2 ? _iterator2 : _iterator2[Symbol.iterator]();;) {
            var _ref2;

            if (_isArray2) {
              if (_i2 >= _iterator2.length) break;
              _ref2 = _iterator2[_i2++];
            } else {
              _i2 = _iterator2.next();
              if (_i2.done) break;
              _ref2 = _i2.value;
            }

            var child = _ref2;

            if (/(^| )dz-message($| )/.test(child.className)) {
              messageElement = child;
              child.className = "dz-message"; // Removes the 'dz-default' class
              break;
            }
          }
          if (!messageElement) {
            messageElement = Dropzone.createElement("<div class=\"dz-message\"><span></span></div>");
            this.element.appendChild(messageElement);
          }

          var span = messageElement.getElementsByTagName("span")[0];
          if (span) {
            if (span.textContent != null) {
              span.textContent = this.options.dictFallbackMessage;
            } else if (span.innerText != null) {
              span.innerText = this.options.dictFallbackMessage;
            }
          }

          return this.element.appendChild(this.getFallbackForm());
        },


        /**
         * Gets called to calculate the thumbnail dimensions.
         *
         * It gets `file`, `width` and `height` (both may be `null`) as parameters and must return an object containing:
         *
         *  - `srcWidth` & `srcHeight` (required)
         *  - `trgWidth` & `trgHeight` (required)
         *  - `srcX` & `srcY` (optional, default `0`)
         *  - `trgX` & `trgY` (optional, default `0`)
         *
         * Those values are going to be used by `ctx.drawImage()`.
         */
        resize: function resize(file, width, height, resizeMethod) {
          var info = {
            srcX: 0,
            srcY: 0,
            srcWidth: file.width,
            srcHeight: file.height
          };

          var srcRatio = file.width / file.height;

          // Automatically calculate dimensions if not specified
          if (width == null && height == null) {
            width = info.srcWidth;
            height = info.srcHeight;
          } else if (width == null) {
            width = height * srcRatio;
          } else if (height == null) {
            height = width / srcRatio;
          }

          // Make sure images aren't upscaled
          width = Math.min(width, info.srcWidth);
          height = Math.min(height, info.srcHeight);

          var trgRatio = width / height;

          if (info.srcWidth > width || info.srcHeight > height) {
            // Image is bigger and needs rescaling
            if (resizeMethod === 'crop') {
              if (srcRatio > trgRatio) {
                info.srcHeight = file.height;
                info.srcWidth = info.srcHeight * trgRatio;
              } else {
                info.srcWidth = file.width;
                info.srcHeight = info.srcWidth / trgRatio;
              }
            } else if (resizeMethod === 'contain') {
              // Method 'contain'
              if (srcRatio > trgRatio) {
                height = width / srcRatio;
              } else {
                width = height * srcRatio;
              }
            } else {
              throw new Error("Unknown resizeMethod '" + resizeMethod + "'");
            }
          }

          info.srcX = (file.width - info.srcWidth) / 2;
          info.srcY = (file.height - info.srcHeight) / 2;

          info.trgWidth = width;
          info.trgHeight = height;

          return info;
        },


        /**
         * Can be used to transform the file (for example, resize an image if necessary).
         *
         * The default implementation uses `resizeWidth` and `resizeHeight` (if provided) and resizes
         * images according to those dimensions.
         *
         * Gets the `file` as the first parameter, and a `done()` function as the second, that needs
         * to be invoked with the file when the transformation is done.
         */
        transformFile: function transformFile(file, done) {
          if ((this.options.resizeWidth || this.options.resizeHeight) && file.type.match(/image.*/)) {
            return this.resizeImage(file, this.options.resizeWidth, this.options.resizeHeight, this.options.resizeMethod, done);
          } else {
            return done(file);
          }
        },


        /**
         * A string that contains the template used for each dropped
         * file. Change it to fulfill your needs but make sure to properly
         * provide all elements.
         *
         * If you want to use an actual HTML element instead of providing a String
         * as a config option, you could create a div with the id `tpl`,
         * put the template inside it and provide the element like this:
         *
         *     document
         *       .querySelector('#tpl')
         *       .innerHTML
         *
         */
        previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-image\"><img data-dz-thumbnail /></div>\n  <div class=\"dz-details\">\n    <div class=\"dz-size\"><span data-dz-size></span></div>\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n  </div>\n  <div class=\"dz-progress\"><span class=\"dz-upload\" data-dz-uploadprogress></span></div>\n  <div class=\"dz-error-message\"><span data-dz-errormessage></span></div>\n  <div class=\"dz-success-mark\">\n    <svg width=\"54px\" height=\"54px\" viewBox=\"0 0 54 54\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:sketch=\"http://www.bohemiancoding.com/sketch/ns\">\n      <title>Check</title>\n      <defs></defs>\n      <g id=\"Page-1\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\" sketch:type=\"MSPage\">\n        <path d=\"M23.5,31.8431458 L17.5852419,25.9283877 C16.0248253,24.3679711 13.4910294,24.366835 11.9289322,25.9289322 C10.3700136,27.4878508 10.3665912,30.0234455 11.9283877,31.5852419 L20.4147581,40.0716123 C20.5133999,40.1702541 20.6159315,40.2626649 20.7218615,40.3488435 C22.2835669,41.8725651 24.794234,41.8626202 26.3461564,40.3106978 L43.3106978,23.3461564 C44.8771021,21.7797521 44.8758057,19.2483887 43.3137085,17.6862915 C41.7547899,16.1273729 39.2176035,16.1255422 37.6538436,17.6893022 L23.5,31.8431458 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=\"Oval-2\" stroke-opacity=\"0.198794158\" stroke=\"#747474\" fill-opacity=\"0.816519475\" fill=\"#FFFFFF\" sketch:type=\"MSShapeGroup\"></path>\n      </g>\n    </svg>\n  </div>\n  <div class=\"dz-error-mark\">\n    <svg width=\"54px\" height=\"54px\" viewBox=\"0 0 54 54\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:sketch=\"http://www.bohemiancoding.com/sketch/ns\">\n      <title>Error</title>\n      <defs></defs>\n      <g id=\"Page-1\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\" sketch:type=\"MSPage\">\n        <g id=\"Check-+-Oval-2\" sketch:type=\"MSLayerGroup\" stroke=\"#747474\" stroke-opacity=\"0.198794158\" fill=\"#FFFFFF\" fill-opacity=\"0.816519475\">\n          <path d=\"M32.6568542,29 L38.3106978,23.3461564 C39.8771021,21.7797521 39.8758057,19.2483887 38.3137085,17.6862915 C36.7547899,16.1273729 34.2176035,16.1255422 32.6538436,17.6893022 L27,23.3431458 L21.3461564,17.6893022 C19.7823965,16.1255422 17.2452101,16.1273729 15.6862915,17.6862915 C14.1241943,19.2483887 14.1228979,21.7797521 15.6893022,23.3461564 L21.3431458,29 L15.6893022,34.6538436 C14.1228979,36.2202479 14.1241943,38.7516113 15.6862915,40.3137085 C17.2452101,41.8726271 19.7823965,41.8744578 21.3461564,40.3106978 L27,34.6568542 L32.6538436,40.3106978 C34.2176035,41.8744578 36.7547899,41.8726271 38.3137085,40.3137085 C39.8758057,38.7516113 39.8771021,36.2202479 38.3106978,34.6538436 L32.6568542,29 Z M27,53 C41.3594035,53 53,41.3594035 53,27 C53,12.6405965 41.3594035,1 27,1 C12.6405965,1 1,12.6405965 1,27 C1,41.3594035 12.6405965,53 27,53 Z\" id=\"Oval-2\" sketch:type=\"MSShapeGroup\"></path>\n        </g>\n      </g>\n    </svg>\n  </div>\n</div>",

        // END OPTIONS
        // (Required by the dropzone documentation parser)


        /*
         Those functions register themselves to the events on init and handle all
         the user interface specific stuff. Overwriting them won't break the upload
         but can break the way it's displayed.
         You can overwrite them if you don't like the default behavior. If you just
         want to add an additional event handler, register it on the dropzone object
         and don't overwrite those options.
         */

        // Those are self explanatory and simply concern the DragnDrop.
        drop: function drop(e) {
          return this.element.classList.remove("dz-drag-hover");
        },
        dragstart: function dragstart(e) {},
        dragend: function dragend(e) {
          return this.element.classList.remove("dz-drag-hover");
        },
        dragenter: function dragenter(e) {
          return this.element.classList.add("dz-drag-hover");
        },
        dragover: function dragover(e) {
          return this.element.classList.add("dz-drag-hover");
        },
        dragleave: function dragleave(e) {
          return this.element.classList.remove("dz-drag-hover");
        },
        paste: function paste(e) {},


        // Called whenever there are no files left in the dropzone anymore, and the
        // dropzone should be displayed as if in the initial state.
        reset: function reset() {
          return this.element.classList.remove("dz-started");
        },


        // Called when a file is added to the queue
        // Receives `file`
        addedfile: function addedfile(file) {
          var _this2 = this;

          if (this.element === this.previewsContainer) {
            this.element.classList.add("dz-started");
          }

          if (this.previewsContainer) {
            file.previewElement = Dropzone.createElement(this.options.previewTemplate.trim());
            file.previewTemplate = file.previewElement; // Backwards compatibility

            this.previewsContainer.appendChild(file.previewElement);
            for (var _iterator3 = file.previewElement.querySelectorAll("[data-dz-name]"), _isArray3 = true, _i3 = 0, _iterator3 = _isArray3 ? _iterator3 : _iterator3[Symbol.iterator]();;) {
              var _ref3;

              if (_isArray3) {
                if (_i3 >= _iterator3.length) break;
                _ref3 = _iterator3[_i3++];
              } else {
                _i3 = _iterator3.next();
                if (_i3.done) break;
                _ref3 = _i3.value;
              }

              var node = _ref3;

              node.textContent = file.name;
            }
            for (var _iterator4 = file.previewElement.querySelectorAll("[data-dz-size]"), _isArray4 = true, _i4 = 0, _iterator4 = _isArray4 ? _iterator4 : _iterator4[Symbol.iterator]();;) {
              if (_isArray4) {
                if (_i4 >= _iterator4.length) break;
                node = _iterator4[_i4++];
              } else {
                _i4 = _iterator4.next();
                if (_i4.done) break;
                node = _i4.value;
              }

              node.innerHTML = this.filesize(file.size);
            }

            if (this.options.addRemoveLinks) {
              file._removeLink = Dropzone.createElement("<a class=\"dz-remove\" href=\"javascript:undefined;\" data-dz-remove>" + this.options.dictRemoveFile + "</a>");
              file.previewElement.appendChild(file._removeLink);
            }

            var removeFileEvent = function removeFileEvent(e) {
              e.preventDefault();
              e.stopPropagation();
              if (file.status === Dropzone.UPLOADING) {
                return Dropzone.confirm(_this2.options.dictCancelUploadConfirmation, function () {
                  return _this2.removeFile(file);
                });
              } else {
                if (_this2.options.dictRemoveFileConfirmation) {
                  return Dropzone.confirm(_this2.options.dictRemoveFileConfirmation, function () {
                    return _this2.removeFile(file);
                  });
                } else {
                  return _this2.removeFile(file);
                }
              }
            };

            for (var _iterator5 = file.previewElement.querySelectorAll("[data-dz-remove]"), _isArray5 = true, _i5 = 0, _iterator5 = _isArray5 ? _iterator5 : _iterator5[Symbol.iterator]();;) {
              var _ref4;

              if (_isArray5) {
                if (_i5 >= _iterator5.length) break;
                _ref4 = _iterator5[_i5++];
              } else {
                _i5 = _iterator5.next();
                if (_i5.done) break;
                _ref4 = _i5.value;
              }

              var removeLink = _ref4;

              removeLink.addEventListener("click", removeFileEvent);
            }
          }
        },


        // Called whenever a file is removed.
        removedfile: function removedfile(file) {
          if (file.previewElement != null && file.previewElement.parentNode != null) {
            file.previewElement.parentNode.removeChild(file.previewElement);
          }
          return this._updateMaxFilesReachedClass();
        },


        // Called when a thumbnail has been generated
        // Receives `file` and `dataUrl`
        thumbnail: function thumbnail(file, dataUrl) {
          if (file.previewElement) {
            file.previewElement.classList.remove("dz-file-preview");
            for (var _iterator6 = file.previewElement.querySelectorAll("[data-dz-thumbnail]"), _isArray6 = true, _i6 = 0, _iterator6 = _isArray6 ? _iterator6 : _iterator6[Symbol.iterator]();;) {
              var _ref5;

              if (_isArray6) {
                if (_i6 >= _iterator6.length) break;
                _ref5 = _iterator6[_i6++];
              } else {
                _i6 = _iterator6.next();
                if (_i6.done) break;
                _ref5 = _i6.value;
              }

              var thumbnailElement = _ref5;

              thumbnailElement.alt = file.name;
              thumbnailElement.src = dataUrl;
            }

            return setTimeout(function () {
              return file.previewElement.classList.add("dz-image-preview");
            }, 1);
          }
        },


        // Called whenever an error occurs
        // Receives `file` and `message`
        error: function error(file, message) {
          if (file.previewElement) {
            file.previewElement.classList.add("dz-error");
            if (typeof message !== "String" && message.error) {
              message = message.error;
            }
            for (var _iterator7 = file.previewElement.querySelectorAll("[data-dz-errormessage]"), _isArray7 = true, _i7 = 0, _iterator7 = _isArray7 ? _iterator7 : _iterator7[Symbol.iterator]();;) {
              var _ref6;

              if (_isArray7) {
                if (_i7 >= _iterator7.length) break;
                _ref6 = _iterator7[_i7++];
              } else {
                _i7 = _iterator7.next();
                if (_i7.done) break;
                _ref6 = _i7.value;
              }

              var node = _ref6;

              node.textContent = message;
            }
          }
        },
        errormultiple: function errormultiple() {},


        // Called when a file gets processed. Since there is a cue, not all added
        // files are processed immediately.
        // Receives `file`
        processing: function processing(file) {
          if (file.previewElement) {
            file.previewElement.classList.add("dz-processing");
            if (file._removeLink) {
              return file._removeLink.innerHTML = this.options.dictCancelUpload;
            }
          }
        },
        processingmultiple: function processingmultiple() {},


        // Called whenever the upload progress gets updated.
        // Receives `file`, `progress` (percentage 0-100) and `bytesSent`.
        // To get the total number of bytes of the file, use `file.size`
        uploadprogress: function uploadprogress(file, progress, bytesSent) {
          if (file.previewElement) {
            for (var _iterator8 = file.previewElement.querySelectorAll("[data-dz-uploadprogress]"), _isArray8 = true, _i8 = 0, _iterator8 = _isArray8 ? _iterator8 : _iterator8[Symbol.iterator]();;) {
              var _ref7;

              if (_isArray8) {
                if (_i8 >= _iterator8.length) break;
                _ref7 = _iterator8[_i8++];
              } else {
                _i8 = _iterator8.next();
                if (_i8.done) break;
                _ref7 = _i8.value;
              }

              var node = _ref7;

              node.nodeName === 'PROGRESS' ? node.value = progress : node.style.width = progress + "%";
            }
          }
        },


        // Called whenever the total upload progress gets updated.
        // Called with totalUploadProgress (0-100), totalBytes and totalBytesSent
        totaluploadprogress: function totaluploadprogress() {},


        // Called just before the file is sent. Gets the `xhr` object as second
        // parameter, so you can modify it (for example to add a CSRF token) and a
        // `formData` object to add additional information.
        sending: function sending() {},
        sendingmultiple: function sendingmultiple() {},


        // When the complete upload is finished and successful
        // Receives `file`
        success: function success(file) {
          if (file.previewElement) {
            return file.previewElement.classList.add("dz-success");
          }
        },
        successmultiple: function successmultiple() {},


        // When the upload is canceled.
        canceled: function canceled(file) {
          return this.emit("error", file, this.options.dictUploadCanceled);
        },
        canceledmultiple: function canceledmultiple() {},


        // When the upload is finished, either with success or an error.
        // Receives `file`
        complete: function complete(file) {
          if (file._removeLink) {
            file._removeLink.innerHTML = this.options.dictRemoveFile;
          }
          if (file.previewElement) {
            return file.previewElement.classList.add("dz-complete");
          }
        },
        completemultiple: function completemultiple() {},
        maxfilesexceeded: function maxfilesexceeded() {},
        maxfilesreached: function maxfilesreached() {},
        queuecomplete: function queuecomplete() {},
        addedfiles: function addedfiles() {}
      };

      this.prototype._thumbnailQueue = [];
      this.prototype._processingThumbnail = false;
    }

    // global utility

  }, {
    key: "extend",
    value: function extend(target) {
      for (var _len2 = arguments.length, objects = Array(_len2 > 1 ? _len2 - 1 : 0), _key2 = 1; _key2 < _len2; _key2++) {
        objects[_key2 - 1] = arguments[_key2];
      }

      for (var _iterator9 = objects, _isArray9 = true, _i9 = 0, _iterator9 = _isArray9 ? _iterator9 : _iterator9[Symbol.iterator]();;) {
        var _ref8;

        if (_isArray9) {
          if (_i9 >= _iterator9.length) break;
          _ref8 = _iterator9[_i9++];
        } else {
          _i9 = _iterator9.next();
          if (_i9.done) break;
          _ref8 = _i9.value;
        }

        var object = _ref8;

        for (var key in object) {
          var val = object[key];
          target[key] = val;
        }
      }
      return target;
    }
  }]);

  function Dropzone(el, options) {
    _classCallCheck(this, Dropzone);

    var _this = _possibleConstructorReturn(this, (Dropzone.__proto__ || Object.getPrototypeOf(Dropzone)).call(this));

    var fallback = void 0,
        left = void 0;
    _this.element = el;
    // For backwards compatibility since the version was in the prototype previously
    _this.version = Dropzone.version;

    _this.defaultOptions.previewTemplate = _this.defaultOptions.previewTemplate.replace(/\n*/g, "");

    _this.clickableElements = [];
    _this.listeners = [];
    _this.files = []; // All files

    if (typeof _this.element === "string") {
      _this.element = document.querySelector(_this.element);
    }

    // Not checking if instance of HTMLElement or Element since IE9 is extremely weird.
    if (!_this.element || _this.element.nodeType == null) {
      throw new Error("Invalid dropzone element.");
    }

    if (_this.element.dropzone) {
      throw new Error("Dropzone already attached.");
    }

    // Now add this dropzone to the instances.
    Dropzone.instances.push(_this);

    // Put the dropzone inside the element itself.
    _this.element.dropzone = _this;

    var elementOptions = (left = Dropzone.optionsForElement(_this.element)) != null ? left : {};

    _this.options = Dropzone.extend({}, _this.defaultOptions, elementOptions, options != null ? options : {});

    // If the browser failed, just call the fallback and leave
    if (_this.options.forceFallback || !Dropzone.isBrowserSupported()) {
      var _ret;

      return _ret = _this.options.fallback.call(_this), _possibleConstructorReturn(_this, _ret);
    }

    // @options.url = @element.getAttribute "action" unless @options.url?
    if (_this.options.url == null) {
      _this.options.url = _this.element.getAttribute("action");
    }

    if (!_this.options.url) {
      throw new Error("No URL provided.");
    }

    if (_this.options.acceptedFiles && _this.options.acceptedMimeTypes) {
      throw new Error("You can't provide both 'acceptedFiles' and 'acceptedMimeTypes'. 'acceptedMimeTypes' is deprecated.");
    }

    if (_this.options.uploadMultiple && _this.options.chunking) {
      throw new Error('You cannot set both: uploadMultiple and chunking.');
    }

    // Backwards compatibility
    if (_this.options.acceptedMimeTypes) {
      _this.options.acceptedFiles = _this.options.acceptedMimeTypes;
      delete _this.options.acceptedMimeTypes;
    }

    // Backwards compatibility
    if (_this.options.renameFilename != null) {
      _this.options.renameFile = function (file) {
        return _this.options.renameFilename.call(_this, file.name, file);
      };
    }

    _this.options.method = _this.options.method.toUpperCase();

    if ((fallback = _this.getExistingFallback()) && fallback.parentNode) {
      // Remove the fallback
      fallback.parentNode.removeChild(fallback);
    }

    // Display previews in the previewsContainer element or the Dropzone element unless explicitly set to false
    if (_this.options.previewsContainer !== false) {
      if (_this.options.previewsContainer) {
        _this.previewsContainer = Dropzone.getElement(_this.options.previewsContainer, "previewsContainer");
      } else {
        _this.previewsContainer = _this.element;
      }
    }

    if (_this.options.clickable) {
      if (_this.options.clickable === true) {
        _this.clickableElements = [_this.element];
      } else {
        _this.clickableElements = Dropzone.getElements(_this.options.clickable, "clickable");
      }
    }

    _this.init();
    return _this;
  }

  // Returns all files that have been accepted


  _createClass(Dropzone, [{
    key: "getAcceptedFiles",
    value: function getAcceptedFiles() {
      return this.files.filter(function (file) {
        return file.accepted;
      }).map(function (file) {
        return file;
      });
    }

    // Returns all files that have been rejected
    // Not sure when that's going to be useful, but added for completeness.

  }, {
    key: "getRejectedFiles",
    value: function getRejectedFiles() {
      return this.files.filter(function (file) {
        return !file.accepted;
      }).map(function (file) {
        return file;
      });
    }
  }, {
    key: "getFilesWithStatus",
    value: function getFilesWithStatus(status) {
      return this.files.filter(function (file) {
        return file.status === status;
      }).map(function (file) {
        return file;
      });
    }

    // Returns all files that are in the queue

  }, {
    key: "getQueuedFiles",
    value: function getQueuedFiles() {
      return this.getFilesWithStatus(Dropzone.QUEUED);
    }
  }, {
    key: "getUploadingFiles",
    value: function getUploadingFiles() {
      return this.getFilesWithStatus(Dropzone.UPLOADING);
    }
  }, {
    key: "getAddedFiles",
    value: function getAddedFiles() {
      return this.getFilesWithStatus(Dropzone.ADDED);
    }

    // Files that are either queued or uploading

  }, {
    key: "getActiveFiles",
    value: function getActiveFiles() {
      return this.files.filter(function (file) {
        return file.status === Dropzone.UPLOADING || file.status === Dropzone.QUEUED;
      }).map(function (file) {
        return file;
      });
    }

    // The function that gets called when Dropzone is initialized. You
    // can (and should) setup event listeners inside this function.

  }, {
    key: "init",
    value: function init() {
      var _this3 = this;

      // In case it isn't set already
      if (this.element.tagName === "form") {
        this.element.setAttribute("enctype", "multipart/form-data");
      }

      if (this.element.classList.contains("dropzone") && !this.element.querySelector(".dz-message")) {
        this.element.appendChild(Dropzone.createElement("<div class=\"dz-default dz-message\"><span>" + this.options.dictDefaultMessage + "</span></div>"));
      }

      if (this.clickableElements.length) {
        var setupHiddenFileInput = function setupHiddenFileInput() {
          if (_this3.hiddenFileInput) {
            _this3.hiddenFileInput.parentNode.removeChild(_this3.hiddenFileInput);
          }
          _this3.hiddenFileInput = document.createElement("input");
          _this3.hiddenFileInput.setAttribute("type", "file");
          if (_this3.options.maxFiles === null || _this3.options.maxFiles > 1) {
            _this3.hiddenFileInput.setAttribute("multiple", "multiple");
          }
          _this3.hiddenFileInput.className = "dz-hidden-input";

          if (_this3.options.acceptedFiles !== null) {
            _this3.hiddenFileInput.setAttribute("accept", _this3.options.acceptedFiles);
          }
          if (_this3.options.capture !== null) {
            _this3.hiddenFileInput.setAttribute("capture", _this3.options.capture);
          }

          // Not setting `display="none"` because some browsers don't accept clicks
          // on elements that aren't displayed.
          _this3.hiddenFileInput.style.visibility = "hidden";
          _this3.hiddenFileInput.style.position = "absolute";
          _this3.hiddenFileInput.style.top = "0";
          _this3.hiddenFileInput.style.left = "0";
          _this3.hiddenFileInput.style.height = "0";
          _this3.hiddenFileInput.style.width = "0";
          Dropzone.getElement(_this3.options.hiddenInputContainer, 'hiddenInputContainer').appendChild(_this3.hiddenFileInput);
          return _this3.hiddenFileInput.addEventListener("change", function () {
            var files = _this3.hiddenFileInput.files;

            if (files.length) {
              for (var _iterator10 = files, _isArray10 = true, _i10 = 0, _iterator10 = _isArray10 ? _iterator10 : _iterator10[Symbol.iterator]();;) {
                var _ref9;

                if (_isArray10) {
                  if (_i10 >= _iterator10.length) break;
                  _ref9 = _iterator10[_i10++];
                } else {
                  _i10 = _iterator10.next();
                  if (_i10.done) break;
                  _ref9 = _i10.value;
                }

                var file = _ref9;

                _this3.addFile(file);
              }
            }
            _this3.emit("addedfiles", files);
            return setupHiddenFileInput();
          });
        };
        setupHiddenFileInput();
      }

      this.URL = window.URL !== null ? window.URL : window.webkitURL;

      // Setup all event listeners on the Dropzone object itself.
      // They're not in @setupEventListeners() because they shouldn't be removed
      // again when the dropzone gets disabled.
      for (var _iterator11 = this.events, _isArray11 = true, _i11 = 0, _iterator11 = _isArray11 ? _iterator11 : _iterator11[Symbol.iterator]();;) {
        var _ref10;

        if (_isArray11) {
          if (_i11 >= _iterator11.length) break;
          _ref10 = _iterator11[_i11++];
        } else {
          _i11 = _iterator11.next();
          if (_i11.done) break;
          _ref10 = _i11.value;
        }

        var eventName = _ref10;

        this.on(eventName, this.options[eventName]);
      }

      this.on("uploadprogress", function () {
        return _this3.updateTotalUploadProgress();
      });

      this.on("removedfile", function () {
        return _this3.updateTotalUploadProgress();
      });

      this.on("canceled", function (file) {
        return _this3.emit("complete", file);
      });

      // Emit a `queuecomplete` event if all files finished uploading.
      this.on("complete", function (file) {
        if (_this3.getAddedFiles().length === 0 && _this3.getUploadingFiles().length === 0 && _this3.getQueuedFiles().length === 0) {
          // This needs to be deferred so that `queuecomplete` really triggers after `complete`
          return setTimeout(function () {
            return _this3.emit("queuecomplete");
          }, 0);
        }
      });

      var noPropagation = function noPropagation(e) {
        e.stopPropagation();
        if (e.preventDefault) {
          return e.preventDefault();
        } else {
          return e.returnValue = false;
        }
      };

      // Create the listeners
      this.listeners = [{
        element: this.element,
        events: {
          "dragstart": function dragstart(e) {
            return _this3.emit("dragstart", e);
          },
          "dragenter": function dragenter(e) {
            noPropagation(e);
            return _this3.emit("dragenter", e);
          },
          "dragover": function dragover(e) {
            // Makes it possible to drag files from chrome's download bar
            // http://stackoverflow.com/questions/19526430/drag-and-drop-file-uploads-from-chrome-downloads-bar
            // Try is required to prevent bug in Internet Explorer 11 (SCRIPT65535 exception)
            var efct = void 0;
            try {
              efct = e.dataTransfer.effectAllowed;
            } catch (error) {}
            e.dataTransfer.dropEffect = 'move' === efct || 'linkMove' === efct ? 'move' : 'copy';

            noPropagation(e);
            return _this3.emit("dragover", e);
          },
          "dragleave": function dragleave(e) {
            return _this3.emit("dragleave", e);
          },
          "drop": function drop(e) {
            noPropagation(e);
            return _this3.drop(e);
          },
          "dragend": function dragend(e) {
            return _this3.emit("dragend", e);
          }

          // This is disabled right now, because the browsers don't implement it properly.
          // "paste": (e) =>
          //   noPropagation e
          //   @paste e
        } }];

      this.clickableElements.forEach(function (clickableElement) {
        return _this3.listeners.push({
          element: clickableElement,
          events: {
            "click": function click(evt) {
              // Only the actual dropzone or the message element should trigger file selection
              if (clickableElement !== _this3.element || evt.target === _this3.element || Dropzone.elementInside(evt.target, _this3.element.querySelector(".dz-message"))) {
                _this3.hiddenFileInput.click(); // Forward the click
              }
              return true;
            }
          }
        });
      });

      this.enable();

      return this.options.init.call(this);
    }

    // Not fully tested yet

  }, {
    key: "destroy",
    value: function destroy() {
      this.disable();
      this.removeAllFiles(true);
      if (this.hiddenFileInput != null ? this.hiddenFileInput.parentNode : undefined) {
        this.hiddenFileInput.parentNode.removeChild(this.hiddenFileInput);
        this.hiddenFileInput = null;
      }
      delete this.element.dropzone;
      return Dropzone.instances.splice(Dropzone.instances.indexOf(this), 1);
    }
  }, {
    key: "updateTotalUploadProgress",
    value: function updateTotalUploadProgress() {
      var totalUploadProgress = void 0;
      var totalBytesSent = 0;
      var totalBytes = 0;

      var activeFiles = this.getActiveFiles();

      if (activeFiles.length) {
        for (var _iterator12 = this.getActiveFiles(), _isArray12 = true, _i12 = 0, _iterator12 = _isArray12 ? _iterator12 : _iterator12[Symbol.iterator]();;) {
          var _ref11;

          if (_isArray12) {
            if (_i12 >= _iterator12.length) break;
            _ref11 = _iterator12[_i12++];
          } else {
            _i12 = _iterator12.next();
            if (_i12.done) break;
            _ref11 = _i12.value;
          }

          var file = _ref11;

          totalBytesSent += file.upload.bytesSent;
          totalBytes += file.upload.total;
        }
        totalUploadProgress = 100 * totalBytesSent / totalBytes;
      } else {
        totalUploadProgress = 100;
      }

      return this.emit("totaluploadprogress", totalUploadProgress, totalBytes, totalBytesSent);
    }

    // @options.paramName can be a function taking one parameter rather than a string.
    // A parameter name for a file is obtained simply by calling this with an index number.

  }, {
    key: "_getParamName",
    value: function _getParamName(n) {
      if (typeof this.options.paramName === "function") {
        return this.options.paramName(n);
      } else {
        return "" + this.options.paramName + (this.options.uploadMultiple ? "[" + n + "]" : "");
      }
    }

    // If @options.renameFile is a function,
    // the function will be used to rename the file.name before appending it to the formData

  }, {
    key: "_renameFile",
    value: function _renameFile(file) {
      if (typeof this.options.renameFile !== "function") {
        return file.name;
      }
      return this.options.renameFile(file);
    }

    // Returns a form that can be used as fallback if the browser does not support DragnDrop
    //
    // If the dropzone is already a form, only the input field and button are returned. Otherwise a complete form element is provided.
    // This code has to pass in IE7 :(

  }, {
    key: "getFallbackForm",
    value: function getFallbackForm() {
      var existingFallback = void 0,
          form = void 0;
      if (existingFallback = this.getExistingFallback()) {
        return existingFallback;
      }

      var fieldsString = "<div class=\"dz-fallback\">";
      if (this.options.dictFallbackText) {
        fieldsString += "<p>" + this.options.dictFallbackText + "</p>";
      }
      fieldsString += "<input type=\"file\" name=\"" + this._getParamName(0) + "\" " + (this.options.uploadMultiple ? 'multiple="multiple"' : undefined) + " /><input type=\"submit\" value=\"Upload!\"></div>";

      var fields = Dropzone.createElement(fieldsString);
      if (this.element.tagName !== "FORM") {
        form = Dropzone.createElement("<form action=\"" + this.options.url + "\" enctype=\"multipart/form-data\" method=\"" + this.options.method + "\"></form>");
        form.appendChild(fields);
      } else {
        // Make sure that the enctype and method attributes are set properly
        this.element.setAttribute("enctype", "multipart/form-data");
        this.element.setAttribute("method", this.options.method);
      }
      return form != null ? form : fields;
    }

    // Returns the fallback elements if they exist already
    //
    // This code has to pass in IE7 :(

  }, {
    key: "getExistingFallback",
    value: function getExistingFallback() {
      var getFallback = function getFallback(elements) {
        for (var _iterator13 = elements, _isArray13 = true, _i13 = 0, _iterator13 = _isArray13 ? _iterator13 : _iterator13[Symbol.iterator]();;) {
          var _ref12;

          if (_isArray13) {
            if (_i13 >= _iterator13.length) break;
            _ref12 = _iterator13[_i13++];
          } else {
            _i13 = _iterator13.next();
            if (_i13.done) break;
            _ref12 = _i13.value;
          }

          var el = _ref12;

          if (/(^| )fallback($| )/.test(el.className)) {
            return el;
          }
        }
      };

      var _arr = ["div", "form"];
      for (var _i14 = 0; _i14 < _arr.length; _i14++) {
        var tagName = _arr[_i14];
        var fallback;
        if (fallback = getFallback(this.element.getElementsByTagName(tagName))) {
          return fallback;
        }
      }
    }

    // Activates all listeners stored in @listeners

  }, {
    key: "setupEventListeners",
    value: function setupEventListeners() {
      return this.listeners.map(function (elementListeners) {
        return function () {
          var result = [];
          for (var event in elementListeners.events) {
            var listener = elementListeners.events[event];
            result.push(elementListeners.element.addEventListener(event, listener, false));
          }
          return result;
        }();
      });
    }

    // Deactivates all listeners stored in @listeners

  }, {
    key: "removeEventListeners",
    value: function removeEventListeners() {
      return this.listeners.map(function (elementListeners) {
        return function () {
          var result = [];
          for (var event in elementListeners.events) {
            var listener = elementListeners.events[event];
            result.push(elementListeners.element.removeEventListener(event, listener, false));
          }
          return result;
        }();
      });
    }

    // Removes all event listeners and cancels all files in the queue or being processed.

  }, {
    key: "disable",
    value: function disable() {
      var _this4 = this;

      this.clickableElements.forEach(function (element) {
        return element.classList.remove("dz-clickable");
      });
      this.removeEventListeners();
      this.disabled = true;

      return this.files.map(function (file) {
        return _this4.cancelUpload(file);
      });
    }
  }, {
    key: "enable",
    value: function enable() {
      delete this.disabled;
      this.clickableElements.forEach(function (element) {
        return element.classList.add("dz-clickable");
      });
      return this.setupEventListeners();
    }

    // Returns a nicely formatted filesize

  }, {
    key: "filesize",
    value: function filesize(size) {
      var selectedSize = 0;
      var selectedUnit = "b";

      if (size > 0) {
        var units = ['tb', 'gb', 'mb', 'kb', 'b'];

        for (var i = 0; i < units.length; i++) {
          var unit = units[i];
          var cutoff = Math.pow(this.options.filesizeBase, 4 - i) / 10;

          if (size >= cutoff) {
            selectedSize = size / Math.pow(this.options.filesizeBase, 4 - i);
            selectedUnit = unit;
            break;
          }
        }

        selectedSize = Math.round(10 * selectedSize) / 10; // Cutting of digits
      }

      return "<strong>" + selectedSize + "</strong> " + this.options.dictFileSizeUnits[selectedUnit];
    }

    // Adds or removes the `dz-max-files-reached` class from the form.

  }, {
    key: "_updateMaxFilesReachedClass",
    value: function _updateMaxFilesReachedClass() {
      if (this.options.maxFiles != null && this.getAcceptedFiles().length >= this.options.maxFiles) {
        if (this.getAcceptedFiles().length === this.options.maxFiles) {
          this.emit('maxfilesreached', this.files);
        }
        return this.element.classList.add("dz-max-files-reached");
      } else {
        return this.element.classList.remove("dz-max-files-reached");
      }
    }
  }, {
    key: "drop",
    value: function drop(e) {
      if (!e.dataTransfer) {
        return;
      }
      this.emit("drop", e);

      // Convert the FileList to an Array
      // This is necessary for IE11
      var files = [];
      for (var i = 0; i < e.dataTransfer.files.length; i++) {
        files[i] = e.dataTransfer.files[i];
      }

      this.emit("addedfiles", files);

      // Even if it's a folder, files.length will contain the folders.
      if (files.length) {
        var items = e.dataTransfer.items;

        if (items && items.length && items[0].webkitGetAsEntry != null) {
          // The browser supports dropping of folders, so handle items instead of files
          this._addFilesFromItems(items);
        } else {
          this.handleFiles(files);
        }
      }
    }
  }, {
    key: "paste",
    value: function paste(e) {
      if (__guard__(e != null ? e.clipboardData : undefined, function (x) {
        return x.items;
      }) == null) {
        return;
      }

      this.emit("paste", e);
      var items = e.clipboardData.items;


      if (items.length) {
        return this._addFilesFromItems(items);
      }
    }
  }, {
    key: "handleFiles",
    value: function handleFiles(files) {
      for (var _iterator14 = files, _isArray14 = true, _i15 = 0, _iterator14 = _isArray14 ? _iterator14 : _iterator14[Symbol.iterator]();;) {
        var _ref13;

        if (_isArray14) {
          if (_i15 >= _iterator14.length) break;
          _ref13 = _iterator14[_i15++];
        } else {
          _i15 = _iterator14.next();
          if (_i15.done) break;
          _ref13 = _i15.value;
        }

        var file = _ref13;

        this.addFile(file);
      }
    }

    // When a folder is dropped (or files are pasted), items must be handled
    // instead of files.

  }, {
    key: "_addFilesFromItems",
    value: function _addFilesFromItems(items) {
      var _this5 = this;

      return function () {
        var result = [];
        for (var _iterator15 = items, _isArray15 = true, _i16 = 0, _iterator15 = _isArray15 ? _iterator15 : _iterator15[Symbol.iterator]();;) {
          var _ref14;

          if (_isArray15) {
            if (_i16 >= _iterator15.length) break;
            _ref14 = _iterator15[_i16++];
          } else {
            _i16 = _iterator15.next();
            if (_i16.done) break;
            _ref14 = _i16.value;
          }

          var item = _ref14;

          var entry;
          if (item.webkitGetAsEntry != null && (entry = item.webkitGetAsEntry())) {
            if (entry.isFile) {
              result.push(_this5.addFile(item.getAsFile()));
            } else if (entry.isDirectory) {
              // Append all files from that directory to files
              result.push(_this5._addFilesFromDirectory(entry, entry.name));
            } else {
              result.push(undefined);
            }
          } else if (item.getAsFile != null) {
            if (item.kind == null || item.kind === "file") {
              result.push(_this5.addFile(item.getAsFile()));
            } else {
              result.push(undefined);
            }
          } else {
            result.push(undefined);
          }
        }
        return result;
      }();
    }

    // Goes through the directory, and adds each file it finds recursively

  }, {
    key: "_addFilesFromDirectory",
    value: function _addFilesFromDirectory(directory, path) {
      var _this6 = this;

      var dirReader = directory.createReader();

      var errorHandler = function errorHandler(error) {
        return __guardMethod__(console, 'log', function (o) {
          return o.log(error);
        });
      };

      var readEntries = function readEntries() {
        return dirReader.readEntries(function (entries) {
          if (entries.length > 0) {
            for (var _iterator16 = entries, _isArray16 = true, _i17 = 0, _iterator16 = _isArray16 ? _iterator16 : _iterator16[Symbol.iterator]();;) {
              var _ref15;

              if (_isArray16) {
                if (_i17 >= _iterator16.length) break;
                _ref15 = _iterator16[_i17++];
              } else {
                _i17 = _iterator16.next();
                if (_i17.done) break;
                _ref15 = _i17.value;
              }

              var entry = _ref15;

              if (entry.isFile) {
                entry.file(function (file) {
                  if (_this6.options.ignoreHiddenFiles && file.name.substring(0, 1) === '.') {
                    return;
                  }
                  file.fullPath = path + "/" + file.name;
                  return _this6.addFile(file);
                });
              } else if (entry.isDirectory) {
                _this6._addFilesFromDirectory(entry, path + "/" + entry.name);
              }
            }

            // Recursively call readEntries() again, since browser only handle
            // the first 100 entries.
            // See: https://developer.mozilla.org/en-US/docs/Web/API/DirectoryReader#readEntries
            readEntries();
          }
          return null;
        }, errorHandler);
      };

      return readEntries();
    }

    // If `done()` is called without argument the file is accepted
    // If you call it with an error message, the file is rejected
    // (This allows for asynchronous validation)
    //
    // This function checks the filesize, and if the file.type passes the
    // `acceptedFiles` check.

  }, {
    key: "accept",
    value: function accept(file, done) {
      if (this.options.maxFilesize && file.size > this.options.maxFilesize * 1024 * 1024) {
        return done(this.options.dictFileTooBig.replace("{{filesize}}", Math.round(file.size / 1024 / 10.24) / 100).replace("{{maxFilesize}}", this.options.maxFilesize));
      } else if (!Dropzone.isValidFile(file, this.options.acceptedFiles)) {
        return done(this.options.dictInvalidFileType);
      } else if (this.options.maxFiles != null && this.getAcceptedFiles().length >= this.options.maxFiles) {
        done(this.options.dictMaxFilesExceeded.replace("{{maxFiles}}", this.options.maxFiles));
        return this.emit("maxfilesexceeded", file);
      } else {
        return this.options.accept.call(this, file, done);
      }
    }
  }, {
    key: "addFile",
    value: function addFile(file) {
      var _this7 = this;

      file.upload = {
        uuid: Dropzone.uuidv4(),
        progress: 0,
        // Setting the total upload size to file.size for the beginning
        // It's actual different than the size to be transmitted.
        total: file.size,
        bytesSent: 0,
        filename: this._renameFile(file),
        chunked: this.options.chunking && (this.options.forceChunking || file.size > this.options.chunkSize),
        totalChunkCount: Math.ceil(file.size / this.options.chunkSize)
      };
      this.files.push(file);

      file.status = Dropzone.ADDED;

      this.emit("addedfile", file);

      this._enqueueThumbnail(file);

      return this.accept(file, function (error) {
        if (error) {
          file.accepted = false;
          _this7._errorProcessing([file], error); // Will set the file.status
        } else {
          file.accepted = true;
          if (_this7.options.autoQueue) {
            _this7.enqueueFile(file);
          } // Will set .accepted = true
        }
        return _this7._updateMaxFilesReachedClass();
      });
    }

    // Wrapper for enqueueFile

  }, {
    key: "enqueueFiles",
    value: function enqueueFiles(files) {
      for (var _iterator17 = files, _isArray17 = true, _i18 = 0, _iterator17 = _isArray17 ? _iterator17 : _iterator17[Symbol.iterator]();;) {
        var _ref16;

        if (_isArray17) {
          if (_i18 >= _iterator17.length) break;
          _ref16 = _iterator17[_i18++];
        } else {
          _i18 = _iterator17.next();
          if (_i18.done) break;
          _ref16 = _i18.value;
        }

        var file = _ref16;

        this.enqueueFile(file);
      }
      return null;
    }
  }, {
    key: "enqueueFile",
    value: function enqueueFile(file) {
      var _this8 = this;

      if (file.status === Dropzone.ADDED && file.accepted === true) {
        file.status = Dropzone.QUEUED;
        if (this.options.autoProcessQueue) {
          return setTimeout(function () {
            return _this8.processQueue();
          }, 0); // Deferring the call
        }
      } else {
        throw new Error("This file can't be queued because it has already been processed or was rejected.");
      }
    }
  }, {
    key: "_enqueueThumbnail",
    value: function _enqueueThumbnail(file) {
      var _this9 = this;

      if (this.options.createImageThumbnails && file.type.match(/image.*/) && file.size <= this.options.maxThumbnailFilesize * 1024 * 1024) {
        this._thumbnailQueue.push(file);
        return setTimeout(function () {
          return _this9._processThumbnailQueue();
        }, 0); // Deferring the call
      }
    }
  }, {
    key: "_processThumbnailQueue",
    value: function _processThumbnailQueue() {
      var _this10 = this;

      if (this._processingThumbnail || this._thumbnailQueue.length === 0) {
        return;
      }

      this._processingThumbnail = true;
      var file = this._thumbnailQueue.shift();
      return this.createThumbnail(file, this.options.thumbnailWidth, this.options.thumbnailHeight, this.options.thumbnailMethod, true, function (dataUrl) {
        _this10.emit("thumbnail", file, dataUrl);
        _this10._processingThumbnail = false;
        return _this10._processThumbnailQueue();
      });
    }

    // Can be called by the user to remove a file

  }, {
    key: "removeFile",
    value: function removeFile(file) {
      if (file.status === Dropzone.UPLOADING) {
        this.cancelUpload(file);
      }
      this.files = without(this.files, file);

      this.emit("removedfile", file);
      if (this.files.length === 0) {
        return this.emit("reset");
      }
    }

    // Removes all files that aren't currently processed from the list

  }, {
    key: "removeAllFiles",
    value: function removeAllFiles(cancelIfNecessary) {
      // Create a copy of files since removeFile() changes the @files array.
      if (cancelIfNecessary == null) {
        cancelIfNecessary = false;
      }
      for (var _iterator18 = this.files.slice(), _isArray18 = true, _i19 = 0, _iterator18 = _isArray18 ? _iterator18 : _iterator18[Symbol.iterator]();;) {
        var _ref17;

        if (_isArray18) {
          if (_i19 >= _iterator18.length) break;
          _ref17 = _iterator18[_i19++];
        } else {
          _i19 = _iterator18.next();
          if (_i19.done) break;
          _ref17 = _i19.value;
        }

        var file = _ref17;

        if (file.status !== Dropzone.UPLOADING || cancelIfNecessary) {
          this.removeFile(file);
        }
      }
      return null;
    }

    // Resizes an image before it gets sent to the server. This function is the default behavior of
    // `options.transformFile` if `resizeWidth` or `resizeHeight` are set. The callback is invoked with
    // the resized blob.

  }, {
    key: "resizeImage",
    value: function resizeImage(file, width, height, resizeMethod, callback) {
      var _this11 = this;

      return this.createThumbnail(file, width, height, resizeMethod, true, function (dataUrl, canvas) {
        if (canvas == null) {
          // The image has not been resized
          return callback(file);
        } else {
          var resizeMimeType = _this11.options.resizeMimeType;

          if (resizeMimeType == null) {
            resizeMimeType = file.type;
          }
          var resizedDataURL = canvas.toDataURL(resizeMimeType, _this11.options.resizeQuality);
          if (resizeMimeType === 'image/jpeg' || resizeMimeType === 'image/jpg') {
            // Now add the original EXIF information
            resizedDataURL = ExifRestore.restore(file.dataURL, resizedDataURL);
          }
          return callback(Dropzone.dataURItoBlob(resizedDataURL));
        }
      });
    }
  }, {
    key: "createThumbnail",
    value: function createThumbnail(file, width, height, resizeMethod, fixOrientation, callback) {
      var _this12 = this;

      var fileReader = new FileReader();

      fileReader.onload = function () {

        file.dataURL = fileReader.result;

        // Don't bother creating a thumbnail for SVG images since they're vector
        if (file.type === "image/svg+xml") {
          if (callback != null) {
            callback(fileReader.result);
          }
          return;
        }

        return _this12.createThumbnailFromUrl(file, width, height, resizeMethod, fixOrientation, callback);
      };

      return fileReader.readAsDataURL(file);
    }
  }, {
    key: "createThumbnailFromUrl",
    value: function createThumbnailFromUrl(file, width, height, resizeMethod, fixOrientation, callback, crossOrigin) {
      var _this13 = this;

      // Not using `new Image` here because of a bug in latest Chrome versions.
      // See https://github.com/enyo/dropzone/pull/226
      var img = document.createElement("img");

      if (crossOrigin) {
        img.crossOrigin = crossOrigin;
      }

      img.onload = function () {
        var loadExif = function loadExif(callback) {
          return callback(1);
        };
        if (typeof EXIF !== 'undefined' && EXIF !== null && fixOrientation) {
          loadExif = function loadExif(callback) {
            return EXIF.getData(img, function () {
              return callback(EXIF.getTag(this, 'Orientation'));
            });
          };
        }

        return loadExif(function (orientation) {
          file.width = img.width;
          file.height = img.height;

          var resizeInfo = _this13.options.resize.call(_this13, file, width, height, resizeMethod);

          var canvas = document.createElement("canvas");
          var ctx = canvas.getContext("2d");

          canvas.width = resizeInfo.trgWidth;
          canvas.height = resizeInfo.trgHeight;

          if (orientation > 4) {
            canvas.width = resizeInfo.trgHeight;
            canvas.height = resizeInfo.trgWidth;
          }

          switch (orientation) {
            case 2:
              // horizontal flip
              ctx.translate(canvas.width, 0);
              ctx.scale(-1, 1);
              break;
            case 3:
              // 180° rotate left
              ctx.translate(canvas.width, canvas.height);
              ctx.rotate(Math.PI);
              break;
            case 4:
              // vertical flip
              ctx.translate(0, canvas.height);
              ctx.scale(1, -1);
              break;
            case 5:
              // vertical flip + 90 rotate right
              ctx.rotate(0.5 * Math.PI);
              ctx.scale(1, -1);
              break;
            case 6:
              // 90° rotate right
              ctx.rotate(0.5 * Math.PI);
              ctx.translate(0, -canvas.width);
              break;
            case 7:
              // horizontal flip + 90 rotate right
              ctx.rotate(0.5 * Math.PI);
              ctx.translate(canvas.height, -canvas.width);
              ctx.scale(-1, 1);
              break;
            case 8:
              // 90° rotate left
              ctx.rotate(-0.5 * Math.PI);
              ctx.translate(-canvas.height, 0);
              break;
          }

          // This is a bugfix for iOS' scaling bug.
          drawImageIOSFix(ctx, img, resizeInfo.srcX != null ? resizeInfo.srcX : 0, resizeInfo.srcY != null ? resizeInfo.srcY : 0, resizeInfo.srcWidth, resizeInfo.srcHeight, resizeInfo.trgX != null ? resizeInfo.trgX : 0, resizeInfo.trgY != null ? resizeInfo.trgY : 0, resizeInfo.trgWidth, resizeInfo.trgHeight);

          var thumbnail = canvas.toDataURL("image/png");

          if (callback != null) {
            return callback(thumbnail, canvas);
          }
        });
      };

      if (callback != null) {
        img.onerror = callback;
      }

      return img.src = file.dataURL;
    }

    // Goes through the queue and processes files if there aren't too many already.

  }, {
    key: "processQueue",
    value: function processQueue() {
      var parallelUploads = this.options.parallelUploads;

      var processingLength = this.getUploadingFiles().length;
      var i = processingLength;

      // There are already at least as many files uploading than should be
      if (processingLength >= parallelUploads) {
        return;
      }

      var queuedFiles = this.getQueuedFiles();

      if (!(queuedFiles.length > 0)) {
        return;
      }

      if (this.options.uploadMultiple) {
        // The files should be uploaded in one request
        return this.processFiles(queuedFiles.slice(0, parallelUploads - processingLength));
      } else {
        while (i < parallelUploads) {
          if (!queuedFiles.length) {
            return;
          } // Nothing left to process
          this.processFile(queuedFiles.shift());
          i++;
        }
      }
    }

    // Wrapper for `processFiles`

  }, {
    key: "processFile",
    value: function processFile(file) {
      return this.processFiles([file]);
    }

    // Loads the file, then calls finishedLoading()

  }, {
    key: "processFiles",
    value: function processFiles(files) {
      for (var _iterator19 = files, _isArray19 = true, _i20 = 0, _iterator19 = _isArray19 ? _iterator19 : _iterator19[Symbol.iterator]();;) {
        var _ref18;

        if (_isArray19) {
          if (_i20 >= _iterator19.length) break;
          _ref18 = _iterator19[_i20++];
        } else {
          _i20 = _iterator19.next();
          if (_i20.done) break;
          _ref18 = _i20.value;
        }

        var file = _ref18;

        file.processing = true; // Backwards compatibility
        file.status = Dropzone.UPLOADING;

        this.emit("processing", file);
      }

      if (this.options.uploadMultiple) {
        this.emit("processingmultiple", files);
      }

      return this.uploadFiles(files);
    }
  }, {
    key: "_getFilesWithXhr",
    value: function _getFilesWithXhr(xhr) {
      var files = void 0;
      return files = this.files.filter(function (file) {
        return file.xhr === xhr;
      }).map(function (file) {
        return file;
      });
    }

    // Cancels the file upload and sets the status to CANCELED
    // **if** the file is actually being uploaded.
    // If it's still in the queue, the file is being removed from it and the status
    // set to CANCELED.

  }, {
    key: "cancelUpload",
    value: function cancelUpload(file) {
      if (file.status === Dropzone.UPLOADING) {
        var groupedFiles = this._getFilesWithXhr(file.xhr);
        for (var _iterator20 = groupedFiles, _isArray20 = true, _i21 = 0, _iterator20 = _isArray20 ? _iterator20 : _iterator20[Symbol.iterator]();;) {
          var _ref19;

          if (_isArray20) {
            if (_i21 >= _iterator20.length) break;
            _ref19 = _iterator20[_i21++];
          } else {
            _i21 = _iterator20.next();
            if (_i21.done) break;
            _ref19 = _i21.value;
          }

          var groupedFile = _ref19;

          groupedFile.status = Dropzone.CANCELED;
        }
        if (typeof file.xhr !== 'undefined') {
          file.xhr.abort();
        }
        for (var _iterator21 = groupedFiles, _isArray21 = true, _i22 = 0, _iterator21 = _isArray21 ? _iterator21 : _iterator21[Symbol.iterator]();;) {
          var _ref20;

          if (_isArray21) {
            if (_i22 >= _iterator21.length) break;
            _ref20 = _iterator21[_i22++];
          } else {
            _i22 = _iterator21.next();
            if (_i22.done) break;
            _ref20 = _i22.value;
          }

          var _groupedFile = _ref20;

          this.emit("canceled", _groupedFile);
        }
        if (this.options.uploadMultiple) {
          this.emit("canceledmultiple", groupedFiles);
        }
      } else if (file.status === Dropzone.ADDED || file.status === Dropzone.QUEUED) {
        file.status = Dropzone.CANCELED;
        this.emit("canceled", file);
        if (this.options.uploadMultiple) {
          this.emit("canceledmultiple", [file]);
        }
      }

      if (this.options.autoProcessQueue) {
        return this.processQueue();
      }
    }
  }, {
    key: "resolveOption",
    value: function resolveOption(option) {
      if (typeof option === 'function') {
        for (var _len3 = arguments.length, args = Array(_len3 > 1 ? _len3 - 1 : 0), _key3 = 1; _key3 < _len3; _key3++) {
          args[_key3 - 1] = arguments[_key3];
        }

        return option.apply(this, args);
      }
      return option;
    }
  }, {
    key: "uploadFile",
    value: function uploadFile(file) {
      return this.uploadFiles([file]);
    }
  }, {
    key: "uploadFiles",
    value: function uploadFiles(files) {
      var _this14 = this;

      this._transformFiles(files, function (transformedFiles) {
        if (files[0].upload.chunked) {
          // This file should be sent in chunks!

          // If the chunking option is set, we **know** that there can only be **one** file, since
          // uploadMultiple is not allowed with this option.
          var file = files[0];
          var transformedFile = transformedFiles[0];
          var startedChunkCount = 0;

          file.upload.chunks = [];

          var handleNextChunk = function handleNextChunk() {
            var chunkIndex = 0;

            // Find the next item in file.upload.chunks that is not defined yet.
            while (file.upload.chunks[chunkIndex] !== undefined) {
              chunkIndex++;
            }

            // This means, that all chunks have already been started.
            if (chunkIndex >= file.upload.totalChunkCount) return;

            startedChunkCount++;

            var start = chunkIndex * _this14.options.chunkSize;
            var end = Math.min(start + _this14.options.chunkSize, file.size);

            var dataBlock = {
              name: _this14._getParamName(0),
              data: transformedFile.webkitSlice ? transformedFile.webkitSlice(start, end) : transformedFile.slice(start, end),
              filename: file.upload.filename,
              chunkIndex: chunkIndex
            };

            file.upload.chunks[chunkIndex] = {
              file: file,
              index: chunkIndex,
              dataBlock: dataBlock, // In case we want to retry.
              status: Dropzone.UPLOADING,
              progress: 0,
              retries: 0 // The number of times this block has been retried.
            };

            _this14._uploadData(files, [dataBlock]);
          };

          file.upload.finishedChunkUpload = function (chunk) {
            var allFinished = true;
            chunk.status = Dropzone.SUCCESS;

            // Clear the data from the chunk
            chunk.dataBlock = null;
            // Leaving this reference to xhr intact here will cause memory leaks in some browsers
            chunk.xhr = null;

            for (var i = 0; i < file.upload.totalChunkCount; i++) {
              if (file.upload.chunks[i] === undefined) {
                return handleNextChunk();
              }
              if (file.upload.chunks[i].status !== Dropzone.SUCCESS) {
                allFinished = false;
              }
            }

            if (allFinished) {
              _this14.options.chunksUploaded(file, function () {
                _this14._finished(files, '', null);
              });
            }
          };

          if (_this14.options.parallelChunkUploads) {
            for (var i = 0; i < file.upload.totalChunkCount; i++) {
              handleNextChunk();
            }
          } else {
            handleNextChunk();
          }
        } else {
          var dataBlocks = [];
          for (var _i23 = 0; _i23 < files.length; _i23++) {
            dataBlocks[_i23] = {
              name: _this14._getParamName(_i23),
              data: transformedFiles[_i23],
              filename: files[_i23].upload.filename
            };
          }
          _this14._uploadData(files, dataBlocks);
        }
      });
    }

    /// Returns the right chunk for given file and xhr

  }, {
    key: "_getChunk",
    value: function _getChunk(file, xhr) {
      for (var i = 0; i < file.upload.totalChunkCount; i++) {
        if (file.upload.chunks[i] !== undefined && file.upload.chunks[i].xhr === xhr) {
          return file.upload.chunks[i];
        }
      }
    }

    // This function actually uploads the file(s) to the server.
    // If dataBlocks contains the actual data to upload (meaning, that this could either be transformed
    // files, or individual chunks for chunked upload).

  }, {
    key: "_uploadData",
    value: function _uploadData(files, dataBlocks) {
      var _this15 = this;

      var xhr = new XMLHttpRequest();

      // Put the xhr object in the file objects to be able to reference it later.
      for (var _iterator22 = files, _isArray22 = true, _i24 = 0, _iterator22 = _isArray22 ? _iterator22 : _iterator22[Symbol.iterator]();;) {
        var _ref21;

        if (_isArray22) {
          if (_i24 >= _iterator22.length) break;
          _ref21 = _iterator22[_i24++];
        } else {
          _i24 = _iterator22.next();
          if (_i24.done) break;
          _ref21 = _i24.value;
        }

        var file = _ref21;

        file.xhr = xhr;
      }
      if (files[0].upload.chunked) {
        // Put the xhr object in the right chunk object, so it can be associated later, and found with _getChunk
        files[0].upload.chunks[dataBlocks[0].chunkIndex].xhr = xhr;
      }

      var method = this.resolveOption(this.options.method, files);
      var url = this.resolveOption(this.options.url, files);
      xhr.open(method, url, true);

      // Setting the timeout after open because of IE11 issue: https://gitlab.com/meno/dropzone/issues/8
      xhr.timeout = this.resolveOption(this.options.timeout, files);

      // Has to be after `.open()`. See https://github.com/enyo/dropzone/issues/179
      xhr.withCredentials = !!this.options.withCredentials;

      xhr.onload = function (e) {
        _this15._finishedUploading(files, xhr, e);
      };

      xhr.onerror = function () {
        _this15._handleUploadError(files, xhr);
      };

      // Some browsers do not have the .upload property
      var progressObj = xhr.upload != null ? xhr.upload : xhr;
      progressObj.onprogress = function (e) {
        return _this15._updateFilesUploadProgress(files, xhr, e);
      };

      var headers = {
        "Accept": "application/json",
        "Cache-Control": "no-cache",
        "X-Requested-With": "XMLHttpRequest"
      };

      if (this.options.headers) {
        Dropzone.extend(headers, this.options.headers);
      }

      for (var headerName in headers) {
        var headerValue = headers[headerName];
        if (headerValue) {
          xhr.setRequestHeader(headerName, headerValue);
        }
      }

      var formData = new FormData();

      // Adding all @options parameters
      if (this.options.params) {
        var additionalParams = this.options.params;
        if (typeof additionalParams === 'function') {
          additionalParams = additionalParams.call(this, files, xhr, files[0].upload.chunked ? this._getChunk(files[0], xhr) : null);
        }

        for (var key in additionalParams) {
          var value = additionalParams[key];
          formData.append(key, value);
        }
      }

      // Let the user add additional data if necessary
      for (var _iterator23 = files, _isArray23 = true, _i25 = 0, _iterator23 = _isArray23 ? _iterator23 : _iterator23[Symbol.iterator]();;) {
        var _ref22;

        if (_isArray23) {
          if (_i25 >= _iterator23.length) break;
          _ref22 = _iterator23[_i25++];
        } else {
          _i25 = _iterator23.next();
          if (_i25.done) break;
          _ref22 = _i25.value;
        }

        var _file = _ref22;

        this.emit("sending", _file, xhr, formData);
      }
      if (this.options.uploadMultiple) {
        this.emit("sendingmultiple", files, xhr, formData);
      }

      this._addFormElementData(formData);

      // Finally add the files
      // Has to be last because some servers (eg: S3) expect the file to be the last parameter
      for (var i = 0; i < dataBlocks.length; i++) {
        var dataBlock = dataBlocks[i];
        formData.append(dataBlock.name, dataBlock.data, dataBlock.filename);
      }

      this.submitRequest(xhr, formData, files);
    }

    // Transforms all files with this.options.transformFile and invokes done with the transformed files when done.

  }, {
    key: "_transformFiles",
    value: function _transformFiles(files, done) {
      var _this16 = this;

      var transformedFiles = [];
      // Clumsy way of handling asynchronous calls, until I get to add a proper Future library.
      var doneCounter = 0;

      var _loop = function _loop(i) {
        _this16.options.transformFile.call(_this16, files[i], function (transformedFile) {
          transformedFiles[i] = transformedFile;
          if (++doneCounter === files.length) {
            done(transformedFiles);
          }
        });
      };

      for (var i = 0; i < files.length; i++) {
        _loop(i);
      }
    }

    // Takes care of adding other input elements of the form to the AJAX request

  }, {
    key: "_addFormElementData",
    value: function _addFormElementData(formData) {
      // Take care of other input elements
      if (this.element.tagName === "FORM") {
        for (var _iterator24 = this.element.querySelectorAll("input, textarea, select, button"), _isArray24 = true, _i26 = 0, _iterator24 = _isArray24 ? _iterator24 : _iterator24[Symbol.iterator]();;) {
          var _ref23;

          if (_isArray24) {
            if (_i26 >= _iterator24.length) break;
            _ref23 = _iterator24[_i26++];
          } else {
            _i26 = _iterator24.next();
            if (_i26.done) break;
            _ref23 = _i26.value;
          }

          var input = _ref23;

          var inputName = input.getAttribute("name");
          var inputType = input.getAttribute("type");
          if (inputType) inputType = inputType.toLowerCase();

          // If the input doesn't have a name, we can't use it.
          if (typeof inputName === 'undefined' || inputName === null) continue;

          if (input.tagName === "SELECT" && input.hasAttribute("multiple")) {
            // Possibly multiple values
            for (var _iterator25 = input.options, _isArray25 = true, _i27 = 0, _iterator25 = _isArray25 ? _iterator25 : _iterator25[Symbol.iterator]();;) {
              var _ref24;

              if (_isArray25) {
                if (_i27 >= _iterator25.length) break;
                _ref24 = _iterator25[_i27++];
              } else {
                _i27 = _iterator25.next();
                if (_i27.done) break;
                _ref24 = _i27.value;
              }

              var option = _ref24;

              if (option.selected) {
                formData.append(inputName, option.value);
              }
            }
          } else if (!inputType || inputType !== "checkbox" && inputType !== "radio" || input.checked) {
            formData.append(inputName, input.value);
          }
        }
      }
    }

    // Invoked when there is new progress information about given files.
    // If e is not provided, it is assumed that the upload is finished.

  }, {
    key: "_updateFilesUploadProgress",
    value: function _updateFilesUploadProgress(files, xhr, e) {
      var progress = void 0;
      if (typeof e !== 'undefined') {
        progress = 100 * e.loaded / e.total;

        if (files[0].upload.chunked) {
          var file = files[0];
          // Since this is a chunked upload, we need to update the appropriate chunk progress.
          var chunk = this._getChunk(file, xhr);
          chunk.progress = progress;
          chunk.total = e.total;
          chunk.bytesSent = e.loaded;
          var fileProgress = 0,
              fileTotal = void 0,
              fileBytesSent = void 0;
          file.upload.progress = 0;
          file.upload.total = 0;
          file.upload.bytesSent = 0;
          for (var i = 0; i < file.upload.totalChunkCount; i++) {
            if (file.upload.chunks[i] !== undefined && file.upload.chunks[i].progress !== undefined) {
              file.upload.progress += file.upload.chunks[i].progress;
              file.upload.total += file.upload.chunks[i].total;
              file.upload.bytesSent += file.upload.chunks[i].bytesSent;
            }
          }
          file.upload.progress = file.upload.progress / file.upload.totalChunkCount;
        } else {
          for (var _iterator26 = files, _isArray26 = true, _i28 = 0, _iterator26 = _isArray26 ? _iterator26 : _iterator26[Symbol.iterator]();;) {
            var _ref25;

            if (_isArray26) {
              if (_i28 >= _iterator26.length) break;
              _ref25 = _iterator26[_i28++];
            } else {
              _i28 = _iterator26.next();
              if (_i28.done) break;
              _ref25 = _i28.value;
            }

            var _file2 = _ref25;

            _file2.upload.progress = progress;
            _file2.upload.total = e.total;
            _file2.upload.bytesSent = e.loaded;
          }
        }
        for (var _iterator27 = files, _isArray27 = true, _i29 = 0, _iterator27 = _isArray27 ? _iterator27 : _iterator27[Symbol.iterator]();;) {
          var _ref26;

          if (_isArray27) {
            if (_i29 >= _iterator27.length) break;
            _ref26 = _iterator27[_i29++];
          } else {
            _i29 = _iterator27.next();
            if (_i29.done) break;
            _ref26 = _i29.value;
          }

          var _file3 = _ref26;

          this.emit("uploadprogress", _file3, _file3.upload.progress, _file3.upload.bytesSent);
        }
      } else {
        // Called when the file finished uploading

        var allFilesFinished = true;

        progress = 100;

        for (var _iterator28 = files, _isArray28 = true, _i30 = 0, _iterator28 = _isArray28 ? _iterator28 : _iterator28[Symbol.iterator]();;) {
          var _ref27;

          if (_isArray28) {
            if (_i30 >= _iterator28.length) break;
            _ref27 = _iterator28[_i30++];
          } else {
            _i30 = _iterator28.next();
            if (_i30.done) break;
            _ref27 = _i30.value;
          }

          var _file4 = _ref27;

          if (_file4.upload.progress !== 100 || _file4.upload.bytesSent !== _file4.upload.total) {
            allFilesFinished = false;
          }
          _file4.upload.progress = progress;
          _file4.upload.bytesSent = _file4.upload.total;
        }

        // Nothing to do, all files already at 100%
        if (allFilesFinished) {
          return;
        }

        for (var _iterator29 = files, _isArray29 = true, _i31 = 0, _iterator29 = _isArray29 ? _iterator29 : _iterator29[Symbol.iterator]();;) {
          var _ref28;

          if (_isArray29) {
            if (_i31 >= _iterator29.length) break;
            _ref28 = _iterator29[_i31++];
          } else {
            _i31 = _iterator29.next();
            if (_i31.done) break;
            _ref28 = _i31.value;
          }

          var _file5 = _ref28;

          this.emit("uploadprogress", _file5, progress, _file5.upload.bytesSent);
        }
      }
    }
  }, {
    key: "_finishedUploading",
    value: function _finishedUploading(files, xhr, e) {
      var response = void 0;

      if (files[0].status === Dropzone.CANCELED) {
        return;
      }

      if (xhr.readyState !== 4) {
        return;
      }

      if (xhr.responseType !== 'arraybuffer' && xhr.responseType !== 'blob') {
        response = xhr.responseText;

        if (xhr.getResponseHeader("content-type") && ~xhr.getResponseHeader("content-type").indexOf("application/json")) {
          try {
            response = JSON.parse(response);
          } catch (error) {
            e = error;
            response = "Invalid JSON response from server.";
          }
        }
      }

      this._updateFilesUploadProgress(files);

      if (!(200 <= xhr.status && xhr.status < 300)) {
        this._handleUploadError(files, xhr, response);
      } else {
        if (files[0].upload.chunked) {
          files[0].upload.finishedChunkUpload(this._getChunk(files[0], xhr));
        } else {
          this._finished(files, response, e);
        }
      }
    }
  }, {
    key: "_handleUploadError",
    value: function _handleUploadError(files, xhr, response) {
      if (files[0].status === Dropzone.CANCELED) {
        return;
      }

      if (files[0].upload.chunked && this.options.retryChunks) {
        var chunk = this._getChunk(files[0], xhr);
        if (chunk.retries++ < this.options.retryChunksLimit) {
          this._uploadData(files, [chunk.dataBlock]);
          return;
        } else {
          console.warn('Retried this chunk too often. Giving up.');
        }
      }

      for (var _iterator30 = files, _isArray30 = true, _i32 = 0, _iterator30 = _isArray30 ? _iterator30 : _iterator30[Symbol.iterator]();;) {
        var _ref29;

        if (_isArray30) {
          if (_i32 >= _iterator30.length) break;
          _ref29 = _iterator30[_i32++];
        } else {
          _i32 = _iterator30.next();
          if (_i32.done) break;
          _ref29 = _i32.value;
        }

        var file = _ref29;

        this._errorProcessing(files, response || this.options.dictResponseError.replace("{{statusCode}}", xhr.status), xhr);
      }
    }
  }, {
    key: "submitRequest",
    value: function submitRequest(xhr, formData, files) {
      xhr.send(formData);
    }

    // Called internally when processing is finished.
    // Individual callbacks have to be called in the appropriate sections.

  }, {
    key: "_finished",
    value: function _finished(files, responseText, e) {
      for (var _iterator31 = files, _isArray31 = true, _i33 = 0, _iterator31 = _isArray31 ? _iterator31 : _iterator31[Symbol.iterator]();;) {
        var _ref30;

        if (_isArray31) {
          if (_i33 >= _iterator31.length) break;
          _ref30 = _iterator31[_i33++];
        } else {
          _i33 = _iterator31.next();
          if (_i33.done) break;
          _ref30 = _i33.value;
        }

        var file = _ref30;

        file.status = Dropzone.SUCCESS;
        this.emit("success", file, responseText, e);
        this.emit("complete", file);
      }
      if (this.options.uploadMultiple) {
        this.emit("successmultiple", files, responseText, e);
        this.emit("completemultiple", files);
      }

      if (this.options.autoProcessQueue) {
        return this.processQueue();
      }
    }

    // Called internally when processing is finished.
    // Individual callbacks have to be called in the appropriate sections.

  }, {
    key: "_errorProcessing",
    value: function _errorProcessing(files, message, xhr) {
      for (var _iterator32 = files, _isArray32 = true, _i34 = 0, _iterator32 = _isArray32 ? _iterator32 : _iterator32[Symbol.iterator]();;) {
        var _ref31;

        if (_isArray32) {
          if (_i34 >= _iterator32.length) break;
          _ref31 = _iterator32[_i34++];
        } else {
          _i34 = _iterator32.next();
          if (_i34.done) break;
          _ref31 = _i34.value;
        }

        var file = _ref31;

        file.status = Dropzone.ERROR;
        this.emit("error", file, message, xhr);
        this.emit("complete", file);
      }
      if (this.options.uploadMultiple) {
        this.emit("errormultiple", files, message, xhr);
        this.emit("completemultiple", files);
      }

      if (this.options.autoProcessQueue) {
        return this.processQueue();
      }
    }
  }], [{
    key: "uuidv4",
    value: function uuidv4() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }]);

  return Dropzone;
}(Emitter);

Dropzone.initClass();

Dropzone.version = "5.5.1";

// This is a map of options for your different dropzones. Add configurations
// to this object for your different dropzone elemens.
//
// Example:
//
//     Dropzone.options.myDropzoneElementId = { maxFilesize: 1 };
//
// To disable autoDiscover for a specific element, you can set `false` as an option:
//
//     Dropzone.options.myDisabledElementId = false;
//
// And in html:
//
//     <form action="/upload" id="my-dropzone-element-id" class="dropzone"></form>
Dropzone.options = {};

// Returns the options for an element or undefined if none available.
Dropzone.optionsForElement = function (element) {
  // Get the `Dropzone.options.elementId` for this element if it exists
  if (element.getAttribute("id")) {
    return Dropzone.options[camelize(element.getAttribute("id"))];
  } else {
    return undefined;
  }
};

// Holds a list of all dropzone instances
Dropzone.instances = [];

// Returns the dropzone for given element if any
Dropzone.forElement = function (element) {
  if (typeof element === "string") {
    element = document.querySelector(element);
  }
  if ((element != null ? element.dropzone : undefined) == null) {
    throw new Error("No Dropzone found for given element. This is probably because you're trying to access it before Dropzone had the time to initialize. Use the `init` option to setup any additional observers on your Dropzone.");
  }
  return element.dropzone;
};

// Set to false if you don't want Dropzone to automatically find and attach to .dropzone elements.
Dropzone.autoDiscover = true;

// Looks for all .dropzone elements and creates a dropzone for them
Dropzone.discover = function () {
  var dropzones = void 0;
  if (document.querySelectorAll) {
    dropzones = document.querySelectorAll(".dropzone");
  } else {
    dropzones = [];
    // IE :(
    var checkElements = function checkElements(elements) {
      return function () {
        var result = [];
        for (var _iterator33 = elements, _isArray33 = true, _i35 = 0, _iterator33 = _isArray33 ? _iterator33 : _iterator33[Symbol.iterator]();;) {
          var _ref32;

          if (_isArray33) {
            if (_i35 >= _iterator33.length) break;
            _ref32 = _iterator33[_i35++];
          } else {
            _i35 = _iterator33.next();
            if (_i35.done) break;
            _ref32 = _i35.value;
          }

          var el = _ref32;

          if (/(^| )dropzone($| )/.test(el.className)) {
            result.push(dropzones.push(el));
          } else {
            result.push(undefined);
          }
        }
        return result;
      }();
    };
    checkElements(document.getElementsByTagName("div"));
    checkElements(document.getElementsByTagName("form"));
  }

  return function () {
    var result = [];
    for (var _iterator34 = dropzones, _isArray34 = true, _i36 = 0, _iterator34 = _isArray34 ? _iterator34 : _iterator34[Symbol.iterator]();;) {
      var _ref33;

      if (_isArray34) {
        if (_i36 >= _iterator34.length) break;
        _ref33 = _iterator34[_i36++];
      } else {
        _i36 = _iterator34.next();
        if (_i36.done) break;
        _ref33 = _i36.value;
      }

      var dropzone = _ref33;

      // Create a dropzone unless auto discover has been disabled for specific element
      if (Dropzone.optionsForElement(dropzone) !== false) {
        result.push(new Dropzone(dropzone));
      } else {
        result.push(undefined);
      }
    }
    return result;
  }();
};

// Since the whole Drag'n'Drop API is pretty new, some browsers implement it,
// but not correctly.
// So I created a blacklist of userAgents. Yes, yes. Browser sniffing, I know.
// But what to do when browsers *theoretically* support an API, but crash
// when using it.
//
// This is a list of regular expressions tested against navigator.userAgent
//
// ** It should only be used on browser that *do* support the API, but
// incorrectly **
//
Dropzone.blacklistedBrowsers = [
// The mac os and windows phone version of opera 12 seems to have a problem with the File drag'n'drop API.
/opera.*(Macintosh|Windows Phone).*version\/12/i];

// Checks if the browser is supported
Dropzone.isBrowserSupported = function () {
  var capableBrowser = true;

  if (window.File && window.FileReader && window.FileList && window.Blob && window.FormData && document.querySelector) {
    if (!("classList" in document.createElement("a"))) {
      capableBrowser = false;
    } else {
      // The browser supports the API, but may be blacklisted.
      for (var _iterator35 = Dropzone.blacklistedBrowsers, _isArray35 = true, _i37 = 0, _iterator35 = _isArray35 ? _iterator35 : _iterator35[Symbol.iterator]();;) {
        var _ref34;

        if (_isArray35) {
          if (_i37 >= _iterator35.length) break;
          _ref34 = _iterator35[_i37++];
        } else {
          _i37 = _iterator35.next();
          if (_i37.done) break;
          _ref34 = _i37.value;
        }

        var regex = _ref34;

        if (regex.test(navigator.userAgent)) {
          capableBrowser = false;
          continue;
        }
      }
    }
  } else {
    capableBrowser = false;
  }

  return capableBrowser;
};

Dropzone.dataURItoBlob = function (dataURI) {
  // convert base64 to raw binary data held in a string
  // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
  var byteString = atob(dataURI.split(',')[1]);

  // separate out the mime component
  var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

  // write the bytes of the string to an ArrayBuffer
  var ab = new ArrayBuffer(byteString.length);
  var ia = new Uint8Array(ab);
  for (var i = 0, end = byteString.length, asc = 0 <= end; asc ? i <= end : i >= end; asc ? i++ : i--) {
    ia[i] = byteString.charCodeAt(i);
  }

  // write the ArrayBuffer to a blob
  return new Blob([ab], { type: mimeString });
};

// Returns an array without the rejected item
var without = function without(list, rejectedItem) {
  return list.filter(function (item) {
    return item !== rejectedItem;
  }).map(function (item) {
    return item;
  });
};

// abc-def_ghi -> abcDefGhi
var camelize = function camelize(str) {
  return str.replace(/[\-_](\w)/g, function (match) {
    return match.charAt(1).toUpperCase();
  });
};

// Creates an element from string
Dropzone.createElement = function (string) {
  var div = document.createElement("div");
  div.innerHTML = string;
  return div.childNodes[0];
};

// Tests if given element is inside (or simply is) the container
Dropzone.elementInside = function (element, container) {
  if (element === container) {
    return true;
  } // Coffeescript doesn't support do/while loops
  while (element = element.parentNode) {
    if (element === container) {
      return true;
    }
  }
  return false;
};

Dropzone.getElement = function (el, name) {
  var element = void 0;
  if (typeof el === "string") {
    element = document.querySelector(el);
  } else if (el.nodeType != null) {
    element = el;
  }
  if (element == null) {
    throw new Error("Invalid `" + name + "` option provided. Please provide a CSS selector or a plain HTML element.");
  }
  return element;
};

Dropzone.getElements = function (els, name) {
  var el = void 0,
      elements = void 0;
  if (els instanceof Array) {
    elements = [];
    try {
      for (var _iterator36 = els, _isArray36 = true, _i38 = 0, _iterator36 = _isArray36 ? _iterator36 : _iterator36[Symbol.iterator]();;) {
        if (_isArray36) {
          if (_i38 >= _iterator36.length) break;
          el = _iterator36[_i38++];
        } else {
          _i38 = _iterator36.next();
          if (_i38.done) break;
          el = _i38.value;
        }

        elements.push(this.getElement(el, name));
      }
    } catch (e) {
      elements = null;
    }
  } else if (typeof els === "string") {
    elements = [];
    for (var _iterator37 = document.querySelectorAll(els), _isArray37 = true, _i39 = 0, _iterator37 = _isArray37 ? _iterator37 : _iterator37[Symbol.iterator]();;) {
      if (_isArray37) {
        if (_i39 >= _iterator37.length) break;
        el = _iterator37[_i39++];
      } else {
        _i39 = _iterator37.next();
        if (_i39.done) break;
        el = _i39.value;
      }

      elements.push(el);
    }
  } else if (els.nodeType != null) {
    elements = [els];
  }

  if (elements == null || !elements.length) {
    throw new Error("Invalid `" + name + "` option provided. Please provide a CSS selector, a plain HTML element or a list of those.");
  }

  return elements;
};

// Asks the user the question and calls accepted or rejected accordingly
//
// The default implementation just uses `window.confirm` and then calls the
// appropriate callback.
Dropzone.confirm = function (question, accepted, rejected) {
  if (window.confirm(question)) {
    return accepted();
  } else if (rejected != null) {
    return rejected();
  }
};

// Validates the mime type like this:
//
// https://developer.mozilla.org/en-US/docs/HTML/Element/input#attr-accept
Dropzone.isValidFile = function (file, acceptedFiles) {
  if (!acceptedFiles) {
    return true;
  } // If there are no accepted mime types, it's OK
  acceptedFiles = acceptedFiles.split(",");

  var mimeType = file.type;
  var baseMimeType = mimeType.replace(/\/.*$/, "");

  for (var _iterator38 = acceptedFiles, _isArray38 = true, _i40 = 0, _iterator38 = _isArray38 ? _iterator38 : _iterator38[Symbol.iterator]();;) {
    var _ref35;

    if (_isArray38) {
      if (_i40 >= _iterator38.length) break;
      _ref35 = _iterator38[_i40++];
    } else {
      _i40 = _iterator38.next();
      if (_i40.done) break;
      _ref35 = _i40.value;
    }

    var validType = _ref35;

    validType = validType.trim();
    if (validType.charAt(0) === ".") {
      if (file.name.toLowerCase().indexOf(validType.toLowerCase(), file.name.length - validType.length) !== -1) {
        return true;
      }
    } else if (/\/\*$/.test(validType)) {
      // This is something like a image/* mime type
      if (baseMimeType === validType.replace(/\/.*$/, "")) {
        return true;
      }
    } else {
      if (mimeType === validType) {
        return true;
      }
    }
  }

  return false;
};

// Augment jQuery
if (typeof jQuery !== 'undefined' && jQuery !== null) {
  jQuery.fn.dropzone = function (options) {
    return this.each(function () {
      return new Dropzone(this, options);
    });
  };
}

if (typeof module !== 'undefined' && module !== null) {
  module.exports = Dropzone;
} else {
  window.Dropzone = Dropzone;
}

// Dropzone file status codes
Dropzone.ADDED = "added";

Dropzone.QUEUED = "queued";
// For backwards compatibility. Now, if a file is accepted, it's either queued
// or uploading.
Dropzone.ACCEPTED = Dropzone.QUEUED;

Dropzone.UPLOADING = "uploading";
Dropzone.PROCESSING = Dropzone.UPLOADING; // alias

Dropzone.CANCELED = "canceled";
Dropzone.ERROR = "error";
Dropzone.SUCCESS = "success";

/*

 Bugfix for iOS 6 and 7
 Source: http://stackoverflow.com/questions/11929099/html5-canvas-drawimage-ratio-bug-ios
 based on the work of https://github.com/stomita/ios-imagefile-megapixel

 */

// Detecting vertical squash in loaded image.
// Fixes a bug which squash image vertically while drawing into canvas for some images.
// This is a bug in iOS6 devices. This function from https://github.com/stomita/ios-imagefile-megapixel
var detectVerticalSquash = function detectVerticalSquash(img) {
  var iw = img.naturalWidth;
  var ih = img.naturalHeight;
  var canvas = document.createElement("canvas");
  canvas.width = 1;
  canvas.height = ih;
  var ctx = canvas.getContext("2d");
  ctx.drawImage(img, 0, 0);

  var _ctx$getImageData = ctx.getImageData(1, 0, 1, ih),
      data = _ctx$getImageData.data;

  // search image edge pixel position in case it is squashed vertically.


  var sy = 0;
  var ey = ih;
  var py = ih;
  while (py > sy) {
    var alpha = data[(py - 1) * 4 + 3];

    if (alpha === 0) {
      ey = py;
    } else {
      sy = py;
    }

    py = ey + sy >> 1;
  }
  var ratio = py / ih;

  if (ratio === 0) {
    return 1;
  } else {
    return ratio;
  }
};

// A replacement for context.drawImage
// (args are for source and destination).
var drawImageIOSFix = function drawImageIOSFix(ctx, img, sx, sy, sw, sh, dx, dy, dw, dh) {
  var vertSquashRatio = detectVerticalSquash(img);
  return ctx.drawImage(img, sx, sy, sw, sh, dx, dy, dw, dh / vertSquashRatio);
};

// Based on MinifyJpeg
// Source: http://www.perry.cz/files/ExifRestorer.js
// http://elicon.blog57.fc2.com/blog-entry-206.html

var ExifRestore = function () {
  function ExifRestore() {
    _classCallCheck(this, ExifRestore);
  }

  _createClass(ExifRestore, null, [{
    key: "initClass",
    value: function initClass() {
      this.KEY_STR = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
    }
  }, {
    key: "encode64",
    value: function encode64(input) {
      var output = '';
      var chr1 = undefined;
      var chr2 = undefined;
      var chr3 = '';
      var enc1 = undefined;
      var enc2 = undefined;
      var enc3 = undefined;
      var enc4 = '';
      var i = 0;
      while (true) {
        chr1 = input[i++];
        chr2 = input[i++];
        chr3 = input[i++];
        enc1 = chr1 >> 2;
        enc2 = (chr1 & 3) << 4 | chr2 >> 4;
        enc3 = (chr2 & 15) << 2 | chr3 >> 6;
        enc4 = chr3 & 63;
        if (isNaN(chr2)) {
          enc3 = enc4 = 64;
        } else if (isNaN(chr3)) {
          enc4 = 64;
        }
        output = output + this.KEY_STR.charAt(enc1) + this.KEY_STR.charAt(enc2) + this.KEY_STR.charAt(enc3) + this.KEY_STR.charAt(enc4);
        chr1 = chr2 = chr3 = '';
        enc1 = enc2 = enc3 = enc4 = '';
        if (!(i < input.length)) {
          break;
        }
      }
      return output;
    }
  }, {
    key: "restore",
    value: function restore(origFileBase64, resizedFileBase64) {
      if (!origFileBase64.match('data:image/jpeg;base64,')) {
        return resizedFileBase64;
      }
      var rawImage = this.decode64(origFileBase64.replace('data:image/jpeg;base64,', ''));
      var segments = this.slice2Segments(rawImage);
      var image = this.exifManipulation(resizedFileBase64, segments);
      return "data:image/jpeg;base64," + this.encode64(image);
    }
  }, {
    key: "exifManipulation",
    value: function exifManipulation(resizedFileBase64, segments) {
      var exifArray = this.getExifArray(segments);
      var newImageArray = this.insertExif(resizedFileBase64, exifArray);
      var aBuffer = new Uint8Array(newImageArray);
      return aBuffer;
    }
  }, {
    key: "getExifArray",
    value: function getExifArray(segments) {
      var seg = undefined;
      var x = 0;
      while (x < segments.length) {
        seg = segments[x];
        if (seg[0] === 255 & seg[1] === 225) {
          return seg;
        }
        x++;
      }
      return [];
    }
  }, {
    key: "insertExif",
    value: function insertExif(resizedFileBase64, exifArray) {
      var imageData = resizedFileBase64.replace('data:image/jpeg;base64,', '');
      var buf = this.decode64(imageData);
      var separatePoint = buf.indexOf(255, 3);
      var mae = buf.slice(0, separatePoint);
      var ato = buf.slice(separatePoint);
      var array = mae;
      array = array.concat(exifArray);
      array = array.concat(ato);
      return array;
    }
  }, {
    key: "slice2Segments",
    value: function slice2Segments(rawImageArray) {
      var head = 0;
      var segments = [];
      while (true) {
        var length;
        if (rawImageArray[head] === 255 & rawImageArray[head + 1] === 218) {
          break;
        }
        if (rawImageArray[head] === 255 & rawImageArray[head + 1] === 216) {
          head += 2;
        } else {
          length = rawImageArray[head + 2] * 256 + rawImageArray[head + 3];
          var endPoint = head + length + 2;
          var seg = rawImageArray.slice(head, endPoint);
          segments.push(seg);
          head = endPoint;
        }
        if (head > rawImageArray.length) {
          break;
        }
      }
      return segments;
    }
  }, {
    key: "decode64",
    value: function decode64(input) {
      var output = '';
      var chr1 = undefined;
      var chr2 = undefined;
      var chr3 = '';
      var enc1 = undefined;
      var enc2 = undefined;
      var enc3 = undefined;
      var enc4 = '';
      var i = 0;
      var buf = [];
      // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
      var base64test = /[^A-Za-z0-9\+\/\=]/g;
      if (base64test.exec(input)) {
        console.warn('There were invalid base64 characters in the input text.\nValid base64 characters are A-Z, a-z, 0-9, \'+\', \'/\',and \'=\'\nExpect errors in decoding.');
      }
      input = input.replace(/[^A-Za-z0-9\+\/\=]/g, '');
      while (true) {
        enc1 = this.KEY_STR.indexOf(input.charAt(i++));
        enc2 = this.KEY_STR.indexOf(input.charAt(i++));
        enc3 = this.KEY_STR.indexOf(input.charAt(i++));
        enc4 = this.KEY_STR.indexOf(input.charAt(i++));
        chr1 = enc1 << 2 | enc2 >> 4;
        chr2 = (enc2 & 15) << 4 | enc3 >> 2;
        chr3 = (enc3 & 3) << 6 | enc4;
        buf.push(chr1);
        if (enc3 !== 64) {
          buf.push(chr2);
        }
        if (enc4 !== 64) {
          buf.push(chr3);
        }
        chr1 = chr2 = chr3 = '';
        enc1 = enc2 = enc3 = enc4 = '';
        if (!(i < input.length)) {
          break;
        }
      }
      return buf;
    }
  }]);

  return ExifRestore;
}();

ExifRestore.initClass();

/*
 * contentloaded.js
 *
 * Author: Diego Perini (diego.perini at gmail.com)
 * Summary: cross-browser wrapper for DOMContentLoaded
 * Updated: 20101020
 * License: MIT
 * Version: 1.2
 *
 * URL:
 * http://javascript.nwbox.com/ContentLoaded/
 * http://javascript.nwbox.com/ContentLoaded/MIT-LICENSE
 */

// @win window reference
// @fn function reference
var contentLoaded = function contentLoaded(win, fn) {
  var done = false;
  var top = true;
  var doc = win.document;
  var root = doc.documentElement;
  var add = doc.addEventListener ? "addEventListener" : "attachEvent";
  var rem = doc.addEventListener ? "removeEventListener" : "detachEvent";
  var pre = doc.addEventListener ? "" : "on";
  var init = function init(e) {
    if (e.type === "readystatechange" && doc.readyState !== "complete") {
      return;
    }
    (e.type === "load" ? win : doc)[rem](pre + e.type, init, false);
    if (!done && (done = true)) {
      return fn.call(win, e.type || e);
    }
  };

  var poll = function poll() {
    try {
      root.doScroll("left");
    } catch (e) {
      setTimeout(poll, 50);
      return;
    }
    return init("poll");
  };

  if (doc.readyState !== "complete") {
    if (doc.createEventObject && root.doScroll) {
      try {
        top = !win.frameElement;
      } catch (error) {}
      if (top) {
        poll();
      }
    }
    doc[add](pre + "DOMContentLoaded", init, false);
    doc[add](pre + "readystatechange", init, false);
    return win[add](pre + "load", init, false);
  }
};

// As a single function to be able to write tests.
Dropzone._autoDiscoverFunction = function () {
  if (Dropzone.autoDiscover) {
    return Dropzone.discover();
  }
};
contentLoaded(window, Dropzone._autoDiscoverFunction);

function __guard__(value, transform) {
  return typeof value !== 'undefined' && value !== null ? transform(value) : undefined;
}
function __guardMethod__(obj, methodName, transform) {
  if (typeof obj !== 'undefined' && obj !== null && typeof obj[methodName] === 'function') {
    return transform(obj, methodName);
  } else {
    return undefined;
  }
}

// Prevent Dropzone from auto discoveringr all elements:
Dropzone.autoDiscover = false;

/*global
    piranha
*/

piranha.dropzone = new function () {
    var self = this;

    self.mergeBaseOptions = function (options) {
        if (!options) options = {};
        
        var config = {
            paramName: 'Uploads',
            url: piranha.baseUrl + "manager/api/media/upload",
            uploadMultiple: true,
            init: function () {
                var self = this;

                // Default addedfile callback
                if (!options.addedfile) {
                    options.addedfile = function (file) { }
                }

                // Default removedfile callback
                if (!options.removedfile) {
                    options.removedfile = function (file) { }
                }

                // Default error callback
                if (!options.error) {
                    options.error = function (file) { }
                }

                // Default complete callback
                if (!options.complete) {
                    options.complete = function (file) {
                        console.log(file)
                        if (file.status !== "success" && file.xhr.responseText !== "" ) {
                            var response = JSON.parse(file.xhr.responseText);
                            file.previewElement.querySelector("[data-dz-errormessage]").innerText = response.body;
                        }
                    }
                }

                // Default queuecomplete callback
                if (!options.queuecomplete) {
                    options.queuecomplete = function () {}
                }            
        
                self.on("addedfile", options.addedfile);
                self.on("removedfile", options.removedfile);
                self.on("complete", options.complete);
                self.on("queuecomplete", options.queuecomplete);
            }
        };

        return Object.assign(config, options);
    }

    self.initList = function (selector, options) {
        if (!options) options = {};

        var config = {
            thumbnailWidth: 70,
            thumbnailHeight: 70,
            previewsContainer: selector + " .media-list",
            previewTemplate: document.querySelector( "#media-upload-template").innerHTML
        };

        var listOptions = self.mergeBaseOptions(config);
        
        return new Dropzone(selector + " form", Object.assign(listOptions, options));
    }
    
    self.initThumbnail = function (selector, options) {
        if (!options) options = {};

        var config = {
            thumbnailWidth: 184,
            thumbnailHeight: 130,
            previewsContainer: selector + " .file-list",
            previewTemplate: document.querySelector( "#file-upload-template").innerHTML
        };    

        var thumbOptions = self.mergeBaseOptions(config);
        
        return new Dropzone(selector + " form", Object.assign(thumbOptions, options));
    }   
};
/*global
    piranha
*/

piranha.utils = {
    formatUrl: function (str) {
        return str.replace("~/", piranha.baseUrl);
    },
    isEmptyHtml: function (str) {
        return str == null || str.replace(/(<([^>]+)>)/ig,"").replace(/\s/g, "") == "" && str.indexOf("<img") === -1;
    },
    isEmptyText: function (str) {
        return str == null || str.replace(/\s/g, "") == "" || str.replace(/\s/g, "") == "<br>";
    }
};
/*global
    piranha
*/

piranha.blockpicker = new Vue({
    el: "#blockpicker",
    data: {
        categories: [],
        index: 0,
        callback: null
    },
    methods: {
        open: function (callback, index, parentType) {
            fetch(piranha.baseUrl + "manager/api/content/blocktypes" + (parentType != null ? "/" + parentType : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.typeCount > 1) {
                        // Several applicable block types, open modal
                        piranha.blockpicker.index = index;
                        piranha.blockpicker.callback = callback;
                        piranha.blockpicker.categories = result.categories;

                        $("#blockpicker").modal("show");
                    } else {
                        // There's only one valid block type, select it
                        callback(result.categories[0].items[0].type, index);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        select: function (item) {
            this.callback(item.type, this.index);

            this.index = 0;
            this.callback = null;

            $("#blockpicker").modal("hide");
        }
    },
    created: function () {
    }
});

/*global
    piranha
*/

piranha.notifications = new Vue({
    el: "#notification-hub",
    data: {
        items: [],
    },
    methods: {
        push: function (notification) {

            notification.style = {
                visible: false,
                'notification-info': notification.type === "info",
                'notification-danger': notification.type === "danger",
                'notification-success': notification.type === "success",
                'notification-warning': notification.type === "warning"
            };

            piranha.notifications.items.push(notification);

            setTimeout(function () {
                notification.style.visible = true;

                if (notification.hide)
                {
                    setTimeout(function () {
                        notification.style.visible = false;

                        setTimeout(function () {
                            piranha.notifications.items.shift();
                        }, 200);
                    }, 5000);
                }
            }, 200);
        }
    }
});
/*global
    piranha
*/

piranha.mediapicker = new Vue({
    el: "#mediapicker",
    data: {
        listView: true,
        currentFolderId: null,
        parentFolderId: null,
        folders: [],
        items: [],
        folder: {
            name: null
        },
        filter: null,
        callback: null
    },
    methods: {
        toggle: function () {
            this.listView = !this.listView;
        },
        load: function (id) {
            var url = piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : "");
            if (this.filter) {
                url += "?filter=" + this.filter;
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.mediapicker.currentFolderId = result.currentFolderId;
                    piranha.mediapicker.parentFolderId = result.parentFolderId;
                    piranha.mediapicker.folders = result.folders;
                    piranha.mediapicker.items = result.media;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        open: function (callback, filter) {
            this.callback = callback;
            this.filter = filter;

            this.load();

            $("#mediapicker").modal("show");
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;

            $("#mediapicker").modal("hide");
        }
    }
});

/*global
    piranha
*/

piranha.preview = new Vue({
    el: "#previewModal",
    data: {
        empty: {
            filename:     null,
            type:         null,
            contentType:  null,
            publicUrl:    null,
            size:         null,
            width:        null,
            height:       null,
            lastModified: null
        },
        media: null
    },
    methods: {
        open: function (mediaId) {
            piranha.preview.load(mediaId);
            piranha.preview.show();
        },
        load: function (mediaId) {
            fetch(piranha.baseUrl + "manager/api/media/" + mediaId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.preview.media = result;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        show: function () {
            $("#previewModal").modal("show");
        },
        close: function () {
            console.log("click");
            $("#previewModal").modal("hide");
            piranha.preview.clear();
        },
        clear: function () {
            this.media = this.empty;
        }
    },
    created: function () {
        this.clear();
    }
});

/*global
    piranha, tinymce
*/

piranha.editor = {
    addInline: function (id) {
        tinymce.init({
            selector: "#" + id,
            //fixed_toolbar_container: "#" + id,
            menubar: false,
            branding: false,
            statusbar: false,
            inline: true,
            convert_urls: false,
            plugins: [
                "autoresize autolink code hr paste lists" // TODO: piranhaimage piranhalink"
            ],
            width: "100%",
            autoresize_min_height: 0,
            toolbar: "bold italic | bullist numlist hr | alignleft aligncenter alignright | formatselect", // TODO: | piranhalink piranhaimage",
            block_formats: 'Paragraph=p;Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Code=pre;Quote=blockquote'
        });
    },
    remove: function (id) {
        tinymce.remove(tinymce.get(id));
    }
};

$(document).on('focusin', function (e) {
    if ($(e.target).closest(".tox-tinymce-inline").length) {
        e.stopImmediatePropagation();
    }
});