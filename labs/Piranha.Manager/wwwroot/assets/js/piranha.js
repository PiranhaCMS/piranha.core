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
        callback: null
    },
    methods: {
        toggle: function () {
            this.listView = !this.listView;
        },
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.mediapicker.currentFolderId = result.currentFolderId;
                    piranha.mediapicker.parentFolderId = result.parentFolderId;
                    piranha.mediapicker.folders = result.folders;
                    piranha.mediapicker.items = result.media;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        open: function (callback) {
            this.callback = callback;

            $("#mediapicker").modal("show");
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;

            $("#mediapicker").modal("hide");
        }
    },
    created: function () {
        this.load();
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
    addForm: function (id) {
        tinymce.init({
            selector: "#" + id,
            menubar: false,
            branding: false,
            convert_urls: false,
            plugins: [
                "autoresize autolink code hr paste lists" // piranhaimage piranhalink"
            ],
            width: "100%",
            height: "300",
            autoresize_min_height: 340,
            content_css: ["//fonts.googleapis.com/css?family=Gentium+Book+Basic:700", "//fonts.googleapis.com/css?family=Open+Sans:300,400,600", piranha.baseUrl + "/assets/css/editor.min.css"],
            toolbar: "bold italic | bullist numlist hr | alignleft aligncenter alignright | formatselect", // | piranhalink piranhaimage",
            block_formats: 'Paragraph=p;Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Code=pre;Quote=blockquote',
            setup: function (editor) {
                editor.on('change', function () {
                    editor.save();
                });
            }
        });
    },
    addInline: function (id) {
        tinymce.init({
            selector: "#" + id,
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