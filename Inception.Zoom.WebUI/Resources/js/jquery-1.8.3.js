/*!
 * jQuery JavaScript Library v1.8.3
 * http://jquery.com/
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 *
 * Copyright 2012 jQuery Foundation and other contributors
 * Released under the MIT license
 * http://jquery.org/license
 *
 * Date: Tue Nov 13 2012 08:20:33 GMT-0500 (Eastern Standard Time)
 */
(function (window, undefined) {
    var
        // A central reference to the root jQuery(document)
        rootjQuery,

        // The deferred used on DOM ready
        readyList,

        // Use the correct document accordingly with window argument (sandbox)
        document = window.document,
        location = window.location,
        navigator = window.navigator,

        // Map over jQuery in case of overwrite
        _jQuery = window.jQuery,

        // Map over the $ in case of overwrite
        _$ = window.$,

        // Save a reference to some core methods
        core_push = Array.prototype.push,
        core_slice = Array.prototype.slice,
        core_indexOf = Array.prototype.indexOf,
        core_toString = Object.prototype.toString,
        core_hasOwn = Object.prototype.hasOwnProperty,
        core_trim = String.prototype.trim,

        // Define a local copy of jQuery
        jQuery = function (selector, context) {
            // The jQuery object is actually just the init constructor 'enhanced'
            return new jQuery.fn.init(selector, context, rootjQuery);
        },

        // Used for matching numbers
        core_pnum = /[\-+]?(?:\d*\.|)\d+(?:[eE][\-+]?\d+|)/.source,

        // Used for detecting and trimming whitespace
        core_rnotwhite = /\S/,
        core_rspace = /\s+/,

        // Make sure we trim BOM and NBSP (here's looking at you, Safari 5.0 and IE)
        rtrim = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g,

        // A simple way to check for HTML strings
        // Prioritize #id over <tag> to avoid XSS via location.hash (#9521)
        rquickExpr = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/,

        // Match a standalone tag
        rsingleTag = /^<(\w+)\s*\/?>(?:<\/\1>|)$/,

        // JSON RegExp
        rvalidchars = /^[\],:{}\s]*$/,
        rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g,
        rvalidescape = /\\(?:["\\\/bfnrt]|u[\da-fA-F]{4})/g,
        rvalidtokens = /"[^"\\\r\n]*"|true|false|null|-?(?:\d\d*\.|)\d+(?:[eE][\-+]?\d+|)/g,

        // Matches dashed string for camelizing
        rmsPrefix = /^-ms-/,
        rdashAlpha = /-([\da-z])/gi,

        // Used by jQuery.camelCase as callback to replace()
        fcamelCase = function (all, letter) {
            return (letter + "").toUpperCase();
        },

        // The ready event handler and self cleanup method
        DOMContentLoaded = function () {
            if (document.addEventListener) {
                document.removeEventListener("DOMContentLoaded", DOMContentLoaded, false);
                jQuery.ready();
            } else if (document.readyState === "complete") {
                // we're here because readyState === "complete" in oldIE
                // which is good enough for us to call the dom ready!
                document.detachEvent("onreadystatechange", DOMContentLoaded);
                jQuery.ready();
            }
        },

        // [[Class]] -> type pairs
        class2type = {};

    jQuery.fn = jQuery.prototype = {
        constructor: jQuery,
        init: function (selector, context, rootjQuery) {
            var match, elem, ret, doc;

            // Handle $(""), $(null), $(undefined), $(false)
            if (!selector) {
                return this;
            }

            // Handle $(DOMElement)
            if (selector.nodeType) {
                this.context = this[0] = selector;
                this.length = 1;
                return this;
            }

            // Handle HTML strings
            if (typeof selector === "string") {
                if (selector.charAt(0) === "<" && selector.charAt(selector.length - 1) === ">" && selector.length >= 3) {
                    // Assume that strings that start and end with <> are HTML and skip the regex check
                    match = [null, selector, null];

                } else {
                    match = rquickExpr.exec(selector);
                }

                // Match html or make sure no context is specified for #id
                if (match && (match[1] || !context)) {

                    // HANDLE: $(html) -> $(array)
                    if (match[1]) {
                        context = context instanceof jQuery ? context[0] : context;
                        doc = (context && context.nodeType ? context.ownerDocument || context : document);

                        // scripts is true for back-compat
                        selector = jQuery.parseHTML(match[1], doc, true);
                        if (rsingleTag.test(match[1]) && jQuery.isPlainObject(context)) {
                            this.attr.call(selector, context, true);
                        }

                        return jQuery.merge(this, selector);

                        // HANDLE: $(#id)
                    } else {
                        elem = document.getElementById(match[2]);

                        // Check parentNode to catch when Blackberry 4.6 returns
                        // nodes that are no longer in the document #6963
                        if (elem && elem.parentNode) {
                            // Handle the case where IE and Opera return items
                            // by name instead of ID
                            if (elem.id !== match[2]) {
                                return rootjQuery.find(selector);
                            }

                            // Otherwise, we inject the element directly into the jQuery object
                            this.length = 1;
                            this[0] = elem;
                        }

                        this.context = document;
                        this.selector = selector;
                        return this;
                    }

                    // HANDLE: $(expr, $(...))
                } else if (!context || context.jquery) {
                    return (context || rootjQuery).find(selector);

                    // HANDLE: $(expr, context)
                    // (which is just equivalent to: $(context).find(expr)
                } else {
                    return this.constructor(context).find(selector);
                }

                // HANDLE: $(function)
                // Shortcut for document ready
            } else if (jQuery.isFunction(selector)) {
                return rootjQuery.ready(selector);
            }

            if (selector.selector !== undefined) {
                this.selector = selector.selector;
                this.context = selector.context;
            }

            return jQuery.makeArray(selector, this);
        },

        // Start with an empty selector
        selector: "",

        // The current version of jQuery being used
        jquery: "1.8.3",

        // The default length of a jQuery object is 0
        length: 0,

        // The number of elements contained in the matched element set
        size: function () {
            return this.length;
        },

        toArray: function () {
            return core_slice.call(this);
        },

        // Get the Nth element in the matched element set OR
        // Get the whole matched element set as a clean array
        get: function (num) {
            return num == null ?

                // Return a 'clean' array
                this.toArray() :

                // Return just the object
                (num < 0 ? this[this.length + num] : this[num]);
        },

        // Take an array of elements and push it onto the stack
        // (returning the new matched element set)
        pushStack: function (elems, name, selector) {

            // Build a new jQuery matched element set
            var ret = jQuery.merge(this.constructor(), elems);

            // Add the old object onto the stack (as a reference)
            ret.prevObject = this;

            ret.context = this.context;

            if (name === "find") {
                ret.selector = this.selector + (this.selector ? " " : "") + selector;
            } else if (name) {
                ret.selector = this.selector + "." + name + "(" + selector + ")";
            }

            // Return the newly-formed element set
            return ret;
        },

        // Execute a callback for every element in the matched set.
        // (You can seed the arguments with an array of args, but this is
        // only used internally.)
        each: function (callback, args) {
            return jQuery.each(this, callback, args);
        },

        ready: function (fn) {
            // Add the callback
            jQuery.ready.promise().done(fn);

            return this;
        },

        eq: function (i) {
            i = +i;
            return i === -1 ?
                this.slice(i) :
                this.slice(i, i + 1);
        },

        first: function () {
            return this.eq(0);
        },

        last: function () {
            return this.eq(-1);
        },

        slice: function () {
            return this.pushStack(core_slice.apply(this, arguments),
                "slice", core_slice.call(arguments).join(","));
        },

        map: function (callback) {
            return this.pushStack(jQuery.map(this, function (elem, i) {
                return callback.call(elem, i, elem);
            }));
        },

        end: function () {
            return this.prevObject || this.constructor(null);
        },

        // For internal use only.
        // Behaves like an Array's method, not like a jQuery method.
        push: core_push,
        sort: [].sort,
        splice: [].splice
    };

    // Give the init function the jQuery prototype for later instantiation
    jQuery.fn.init.prototype = jQuery.fn;

    jQuery.extend = jQuery.fn.extend = function () {
        var options, name, src, copy, copyIsArray, clone,
            target = arguments[0] || {},
            i = 1,
            length = arguments.length,
            deep = false;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !jQuery.isFunction(target)) {
            target = {};
        }

        // extend jQuery itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging plain objects or arrays
                    if (deep && copy && (jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)))) {
                        if (copyIsArray) {
                            copyIsArray = false;
                            clone = src && jQuery.isArray(src) ? src : [];

                        } else {
                            clone = src && jQuery.isPlainObject(src) ? src : {};
                        }

                        // Never move original objects, clone them
                        target[name] = jQuery.extend(deep, clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }

        // Return the modified object
        return target;
    };

    jQuery.extend({
        noConflict: function (deep) {
            if (window.$ === jQuery) {
                window.$ = _$;
            }

            if (deep && window.jQuery === jQuery) {
                window.jQuery = _jQuery;
            }

            return jQuery;
        },

        // Is the DOM ready to be used? Set to true once it occurs.
        isReady: false,

        // A counter to track how many items to wait for before
        // the ready event fires. See #6781
        readyWait: 1,

        // Hold (or release) the ready event
        holdReady: function (hold) {
            if (hold) {
                jQuery.readyWait++;
            } else {
                jQuery.ready(true);
            }
        },

        // Handle when the DOM is ready
        ready: function (wait) {

            // Abort if there are pending holds or we're already ready
            if (wait === true ? --jQuery.readyWait : jQuery.isReady) {
                return;
            }

            // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
            if (!document.body) {
                return setTimeout(jQuery.ready, 1);
            }

            // Remember that the DOM is ready
            jQuery.isReady = true;

            // If a normal DOM Ready event fired, decrement, and wait if need be
            if (wait !== true && --jQuery.readyWait > 0) {
                return;
            }

            // If there are functions bound, to execute
            readyList.resolveWith(document, [jQuery]);

            // Trigger any bound ready events
            if (jQuery.fn.trigger) {
                jQuery(document).trigger("ready").off("ready");
            }
        },

        // See test/unit/core.js for details concerning isFunction.
        // Since version 1.3, DOM methods and functions like alert
        // aren't supported. They return false on IE (#2968).
        isFunction: function (obj) {
            return jQuery.type(obj) === "function";
        },

        isArray: Array.isArray || function (obj) {
            return jQuery.type(obj) === "array";
        },

        isWindow: function (obj) {
            return obj != null && obj == obj.window;
        },

        isNumeric: function (obj) {
            return !isNaN(parseFloat(obj)) && isFinite(obj);
        },

        type: function (obj) {
            return obj == null ?
                String(obj) :
                class2type[core_toString.call(obj)] || "object";
        },

        isPlainObject: function (obj) {
            // Must be an Object.
            // Because of IE, we also have to check the presence of the constructor property.
            // Make sure that DOM nodes and window objects don't pass through, as well
            if (!obj || jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow(obj)) {
                return false;
            }

            try {
                // Not own constructor property must be Object
                if (obj.constructor &&
                    !core_hasOwn.call(obj, "constructor") &&
                    !core_hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                    return false;
                }
            } catch (e) {
                // IE8,9 Will throw exceptions on certain host objects #9897
                return false;
            }

            // Own properties are enumerated firstly, so to speed up,
            // if last one is own, then all properties are own.

            var key;
            for (key in obj) { }

            return key === undefined || core_hasOwn.call(obj, key);
        },

        isEmptyObject: function (obj) {
            var name;
            for (name in obj) {
                return false;
            }
            return true;
        },

        error: function (msg) {
            throw new Error(msg);
        },

        // data: string of html
        // context (optional): If specified, the fragment will be created in this context, defaults to document
        // scripts (optional): If true, will include scripts passed in the html string
        parseHTML: function (data, context, scripts) {
            var parsed;
            if (!data || typeof data !== "string") {
                return null;
            }
            if (typeof context === "boolean") {
                scripts = context;
                context = 0;
            }
            context = context || document;

            // Single tag
            if ((parsed = rsingleTag.exec(data))) {
                return [context.createElement(parsed[1])];
            }

            parsed = jQuery.buildFragment([data], context, scripts ? null : []);
            return jQuery.merge([],
                (parsed.cacheable ? jQuery.clone(parsed.fragment) : parsed.fragment).childNodes);
        },

        parseJSON: function (data) {
            if (!data || typeof data !== "string") {
                return null;
            }

            // Make sure leading/trailing whitespace is removed (IE can't handle it)
            data = jQuery.trim(data);

            // Attempt to parse using the native JSON parser first
            if (window.JSON && window.JSON.parse) {
                return window.JSON.parse(data);
            }

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            if (rvalidchars.test(data.replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {

                return (new Function("return " + data))();

            }
            jQuery.error("Invalid JSON: " + data);
        },

        // Cross-browser xml parsing
        parseXML: function (data) {
            var xml, tmp;
            if (!data || typeof data !== "string") {
                return null;
            }
            try {
                if (window.DOMParser) { // Standard
                    tmp = new DOMParser();
                    xml = tmp.parseFromString(data, "text/xml");
                } else { // IE
                    xml = new ActiveXObject("Microsoft.XMLDOM");
                    xml.async = "false";
                    xml.loadXML(data);
                }
            } catch (e) {
                xml = undefined;
            }
            if (!xml || !xml.documentElement || xml.getElementsByTagName("parsererror").length) {
                jQuery.error("Invalid XML: " + data);
            }
            return xml;
        },

        noop: function () { },

        // Evaluates a script in a global context
        // Workarounds based on findings by Jim Driscoll
        // http://weblogs.java.net/blog/driscoll/archive/2009/09/08/eval-javascript-global-context
        globalEval: function (data) {
            if (data && core_rnotwhite.test(data)) {
                // We use execScript on Internet Explorer
                // We use an anonymous function so that context is window
                // rather than jQuery in Firefox
                (window.execScript || function (data) {
                    window["eval"].call(window, data);
                })(data);
            }
        },

        // Convert dashed to camelCase; used by the css and data modules
        // Microsoft forgot to hump their vendor prefix (#9572)
        camelCase: function (string) {
            return string.replace(rmsPrefix, "ms-").replace(rdashAlpha, fcamelCase);
        },

        nodeName: function (elem, name) {
            return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
        },

        // args is for internal usage only
        each: function (obj, callback, args) {
            var name,
                i = 0,
                length = obj.length,
                isObj = length === undefined || jQuery.isFunction(obj);

            if (args) {
                if (isObj) {
                    for (name in obj) {
                        if (callback.apply(obj[name], args) === false) {
                            break;
                        }
                    }
                } else {
                    for (; i < length;) {
                        if (callback.apply(obj[i++], args) === false) {
                            break;
                        }
                    }
                }

                // A special, fast, case for the most common use of each
            } else {
                if (isObj) {
                    for (name in obj) {
                        if (callback.call(obj[name], name, obj[name]) === false) {
                            break;
                        }
                    }
                } else {
                    for (; i < length;) {
                        if (callback.call(obj[i], i, obj[i++]) === false) {
                            break;
                        }
                    }
                }
            }

            return obj;
        },

        // Use native String.trim function wherever possible
        trim: core_trim && !core_trim.call("\uFEFF\xA0") ?
            function (text) {
                return text == null ?
                    "" :
                    core_trim.call(text);
            } :

            // Otherwise use our own trimming functionality
            function (text) {
                return text == null ?
                    "" :
                    (text + "").replace(rtrim, "");
            },

        // results is for internal usage only
        makeArray: function (arr, results) {
            var type,
                ret = results || [];

            if (arr != null) {
                // The window, strings (and functions) also have 'length'
                // Tweaked logic slightly to handle Blackberry 4.7 RegExp issues #6930
                type = jQuery.type(arr);

                if (arr.length == null || type === "string" || type === "function" || type === "regexp" || jQuery.isWindow(arr)) {
                    core_push.call(ret, arr);
                } else {
                    jQuery.merge(ret, arr);
                }
            }

            return ret;
        },

        inArray: function (elem, arr, i) {
            var len;

            if (arr) {
                if (core_indexOf) {
                    return core_indexOf.call(arr, elem, i);
                }

                len = arr.length;
                i = i ? i < 0 ? Math.max(0, len + i) : i : 0;

                for (; i < len; i++) {
                    // Skip accessing in sparse arrays
                    if (i in arr && arr[i] === elem) {
                        return i;
                    }
                }
            }

            return -1;
        },

        merge: function (first, second) {
            var l = second.length,
                i = first.length,
                j = 0;

            if (typeof l === "number") {
                for (; j < l; j++) {
                    first[i++] = second[j];
                }

            } else {
                while (second[j] !== undefined) {
                    first[i++] = second[j++];
                }
            }

            first.length = i;

            return first;
        },

        grep: function (elems, callback, inv) {
            var retVal,
                ret = [],
                i = 0,
                length = elems.length;
            inv = !!inv;

            // Go through the array, only saving the items
            // that pass the validator function
            for (; i < length; i++) {
                retVal = !!callback(elems[i], i);
                if (inv !== retVal) {
                    ret.push(elems[i]);
                }
            }

            return ret;
        },

        // arg is for internal usage only
        map: function (elems, callback, arg) {
            var value, key,
                ret = [],
                i = 0,
                length = elems.length,
                // jquery objects are treated as arrays
                isArray = elems instanceof jQuery || length !== undefined && typeof length === "number" && ((length > 0 && elems[0] && elems[length - 1]) || length === 0 || jQuery.isArray(elems));

            // Go through the array, translating each of the items to their
            if (isArray) {
                for (; i < length; i++) {
                    value = callback(elems[i], i, arg);

                    if (value != null) {
                        ret[ret.length] = value;
                    }
                }

                // Go through every key on the object,
            } else {
                for (key in elems) {
                    value = callback(elems[key], key, arg);

                    if (value != null) {
                        ret[ret.length] = value;
                    }
                }
            }

            // Flatten any nested arrays
            return ret.concat.apply([], ret);
        },

        // A global GUID counter for objects
        guid: 1,

        // Bind a function to a context, optionally partially applying any
        // arguments.
        proxy: function (fn, context) {
            var tmp, args, proxy;

            if (typeof context === "string") {
                tmp = fn[context];
                context = fn;
                fn = tmp;
            }

            // Quick check to determine if target is callable, in the spec
            // this throws a TypeError, but we will just return undefined.
            if (!jQuery.isFunction(fn)) {
                return undefined;
            }

            // Simulated bind
            args = core_slice.call(arguments, 2);
            proxy = function () {
                return fn.apply(context, args.concat(core_slice.call(arguments)));
            };

            // Set the guid of unique handler to the same of original handler, so it can be removed
            proxy.guid = fn.guid = fn.guid || jQuery.guid++;

            return proxy;
        },

        // Multifunctional method to get and set values of a collection
        // The value/s can optionally be executed if it's a function
        access: function (elems, fn, key, value, chainable, emptyGet, pass) {
            var exec,
                bulk = key == null,
                i = 0,
                length = elems.length;

            // Sets many values
            if (key && typeof key === "object") {
                for (i in key) {
                    jQuery.access(elems, fn, i, key[i], 1, emptyGet, value);
                }
                chainable = 1;

                // Sets one value
            } else if (value !== undefined) {
                // Optionally, function values get executed if exec is true
                exec = pass === undefined && jQuery.isFunction(value);

                if (bulk) {
                    // Bulk operations only iterate when executing function values
                    if (exec) {
                        exec = fn;
                        fn = function (elem, key, value) {
                            return exec.call(jQuery(elem), value);
                        };

                        // Otherwise they run against the entire set
                    } else {
                        fn.call(elems, value);
                        fn = null;
                    }
                }

                if (fn) {
                    for (; i < length; i++) {
                        fn(elems[i], key, exec ? value.call(elems[i], i, fn(elems[i], key)) : value, pass);
                    }
                }

                chainable = 1;
            }

            return chainable ?
                elems :

                // Gets
                bulk ?
                    fn.call(elems) :
                    length ? fn(elems[0], key) : emptyGet;
        },

        now: function () {
            return (new Date()).getTime();
        }
    });

    jQuery.ready.promise = function (obj) {
        if (!readyList) {

            readyList = jQuery.Deferred();

            // Catch cases where $(document).ready() is called after the browser event has already occurred.
            // we once tried to use readyState "interactive" here, but it caused issues like the one
            // discovered by ChrisS here: http://bugs.jquery.com/ticket/12282#comment:15
            if (document.readyState === "complete") {
                // Handle it asynchronously to allow scripts the opportunity to delay ready
                setTimeout(jQuery.ready, 1);

                // Standards-based browsers support DOMContentLoaded
            } else if (document.addEventListener) {
                // Use the handy event callback
                document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);

                // A fallback to window.onload, that will always work
                window.addEventListener("load", jQuery.ready, false);

                // If IE event model is used
            } else {
                // Ensure firing before onload, maybe late but safe also for iframes
                document.attachEvent("onreadystatechange", DOMContentLoaded);

                // A fallback to window.onload, that will always work
                window.attachEvent("onload", jQuery.ready);

                // If IE and not a frame
                // continually check to see if the document is ready
                var top = false;

                try {
                    top = window.frameElement == null && document.documentElement;
                } catch (e) { }

                if (top && top.doScroll) {
                    (function doScrollCheck() {
                        if (!jQuery.isReady) {

                            try {
                                // Use the trick by Diego Perini
                                // http://javascript.nwbox.com/IEContentLoaded/
                                top.doScroll("left");
                            } catch (e) {
                                return setTimeout(doScrollCheck, 50);
                            }

                            // and execute any waiting functions
                            jQuery.ready();
                        }
                    })();
                }
            }
        }
        return readyList.promise(obj);
    };

    // Populate the class2type map
    jQuery.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (i, name) {
        class2type["[object " + name + "]"] = name.toLowerCase();
    });

    // All jQuery objects should point back to these
    rootjQuery = jQuery(document);
    // String to Object options format cache
    var optionsCache = {};

    // Convert String-formatted options into Object-formatted ones and store in cache
    function createOptions(options) {
        var object = optionsCache[options] = {};
        jQuery.each(options.split(core_rspace), function (_, flag) {
            object[flag] = true;
        });
        return object;
    }

    /*
     * Create a callback list using the following parameters:
     *
     *	options: an optional list of space-separated options that will change how
     *			the callback list behaves or a more traditional option object
     *
     * By default a callback list will act like an event callback list and can be
     * "fired" multiple times.
     *
     * Possible options:
     *
     *	once:			will ensure the callback list can only be fired once (like a Deferred)
     *
     *	memory:			will keep track of previous values and will call any callback added
     *					after the list has been fired right away with the latest "memorized"
     *					values (like a Deferred)
     *
     *	unique:			will ensure a callback can only be added once (no duplicate in the list)
     *
     *	stopOnFalse:	interrupt callings when a callback returns false
     *
     */
    jQuery.Callbacks = function (options) {

        // Convert options from String-formatted to Object-formatted if needed
        // (we check in cache first)
        options = typeof options === "string" ?
            (optionsCache[options] || createOptions(options)) :
            jQuery.extend({}, options);

        var // Last fire value (for non-forgettable lists)
            memory,
            // Flag to know if list was already fired
            fired,
            // Flag to know if list is currently firing
            firing,
            // First callback to fire (used internally by add and fireWith)
            firingStart,
            // End of the loop when firing
            firingLength,
            // Index of currently firing callback (modified by remove if needed)
            firingIndex,
            // Actual callback list
            list = [],
            // Stack of fire calls for repeatable lists
            stack = !options.once && [],
            // Fire callbacks
            fire = function (data) {
                memory = options.memory && data;
                fired = true;
                firingIndex = firingStart || 0;
                firingStart = 0;
                firingLength = list.length;
                firing = true;
                for (; list && firingIndex < firingLength; firingIndex++) {
                    if (list[firingIndex].apply(data[0], data[1]) === false && options.stopOnFalse) {
                        memory = false; // To prevent further calls using add
                        break;
                    }
                }
                firing = false;
                if (list) {
                    if (stack) {
                        if (stack.length) {
                            fire(stack.shift());
                        }
                    } else if (memory) {
                        list = [];
                    } else {
                        self.disable();
                    }
                }
            },
            // Actual Callbacks object
            self = {
                // Add a callback or a collection of callbacks to the list
                add: function () {
                    if (list) {
                        // First, we save the current length
                        var start = list.length;
                        (function add(args) {
                            jQuery.each(args, function (_, arg) {
                                var type = jQuery.type(arg);
                                if (type === "function") {
                                    if (!options.unique || !self.has(arg)) {
                                        list.push(arg);
                                    }
                                } else if (arg && arg.length && type !== "string") {
                                    // Inspect recursively
                                    add(arg);
                                }
                            });
                        })(arguments);
                        // Do we need to add the callbacks to the
                        // current firing batch?
                        if (firing) {
                            firingLength = list.length;
                            // With memory, if we're not firing then
                            // we should call right away
                        } else if (memory) {
                            firingStart = start;
                            fire(memory);
                        }
                    }
                    return this;
                },
                // Remove a callback from the list
                remove: function () {
                    if (list) {
                        jQuery.each(arguments, function (_, arg) {
                            var index;
                            while ((index = jQuery.inArray(arg, list, index)) > -1) {
                                list.splice(index, 1);
                                // Handle firing indexes
                                if (firing) {
                                    if (index <= firingLength) {
                                        firingLength--;
                                    }
                                    if (index <= firingIndex) {
                                        firingIndex--;
                                    }
                                }
                            }
                        });
                    }
                    return this;
                },
                // Control if a given callback is in the list
                has: function (fn) {
                    return jQuery.inArray(fn, list) > -1;
                },
                // Remove all callbacks from the list
                empty: function () {
                    list = [];
                    return this;
                },
                // Have the list do nothing anymore
                disable: function () {
                    list = stack = memory = undefined;
                    return this;
                },
                // Is it disabled?
                disabled: function () {
                    return !list;
                },
                // Lock the list in its current state
                lock: function () {
                    stack = undefined;
                    if (!memory) {
                        self.disable();
                    }
                    return this;
                },
                // Is it locked?
                locked: function () {
                    return !stack;
                },
                // Call all callbacks with the given context and arguments
                fireWith: function (context, args) {
                    args = args || [];
                    args = [context, args.slice ? args.slice() : args];
                    if (list && (!fired || stack)) {
                        if (firing) {
                            stack.push(args);
                        } else {
                            fire(args);
                        }
                    }
                    return this;
                },
                // Call all the callbacks with the given arguments
                fire: function () {
                    self.fireWith(this, arguments);
                    return this;
                },
                // To know if the callbacks have already been called at least once
                fired: function () {
                    return !!fired;
                }
            };

        return self;
    };
    jQuery.extend({

        Deferred: function (func) {
            var tuples = [
                    // action, add listener, listener list, final state
                    ["resolve", "done", jQuery.Callbacks("once memory"), "resolved"],
                    ["reject", "fail", jQuery.Callbacks("once memory"), "rejected"],
                    ["notify", "progress", jQuery.Callbacks("memory")]
            ],
                state = "pending",
                promise = {
                    state: function () {
                        return state;
                    },
                    always: function () {
                        deferred.done(arguments).fail(arguments);
                        return this;
                    },
                    then: function ( /* fnDone, fnFail, fnProgress */) {
                        var fns = arguments;
                        return jQuery.Deferred(function (newDefer) {
                            jQuery.each(tuples, function (i, tuple) {
                                var action = tuple[0],
                                    fn = fns[i];
                                // deferred[ done | fail | progress ] for forwarding actions to newDefer
                                deferred[tuple[1]](jQuery.isFunction(fn) ?
                                    function () {
                                        var returned = fn.apply(this, arguments);
                                        if (returned && jQuery.isFunction(returned.promise)) {
                                            returned.promise()
                                                .done(newDefer.resolve)
                                                .fail(newDefer.reject)
                                                .progress(newDefer.notify);
                                        } else {
                                            newDefer[action + "With"](this === deferred ? newDefer : this, [returned]);
                                        }
                                    } :
                                    newDefer[action]
                                );
                            });
                            fns = null;
                        }).promise();
                    },
                    // Get a promise for this deferred
                    // If obj is provided, the promise aspect is added to the object
                    promise: function (obj) {
                        return obj != null ? jQuery.extend(obj, promise) : promise;
                    }
                },
                deferred = {};

            // Keep pipe for back-compat
            promise.pipe = promise.then;

            // Add list-specific methods
            jQuery.each(tuples, function (i, tuple) {
                var list = tuple[2],
                    stateString = tuple[3];

                // promise[ done | fail | progress ] = list.add
                promise[tuple[1]] = list.add;

                // Handle state
                if (stateString) {
                    list.add(function () {
                        // state = [ resolved | rejected ]
                        state = stateString;

                        // [ reject_list | resolve_list ].disable; progress_list.lock
                    }, tuples[i ^ 1][2].disable, tuples[2][2].lock);
                }

                // deferred[ resolve | reject | notify ] = list.fire
                deferred[tuple[0]] = list.fire;
                deferred[tuple[0] + "With"] = list.fireWith;
            });

            // Make the deferred a promise
            promise.promise(deferred);

            // Call given func if any
            if (func) {
                func.call(deferred, deferred);
            }

            // All done!
            return deferred;
        },

        // Deferred helper
        when: function (subordinate /* , ..., subordinateN */) {
            var i = 0,
                resolveValues = core_slice.call(arguments),
                length = resolveValues.length,

                // the count of uncompleted subordinates
                remaining = length !== 1 || (subordinate && jQuery.isFunction(subordinate.promise)) ? length : 0,

                // the master Deferred. If resolveValues consist of only a single Deferred, just use that.
                deferred = remaining === 1 ? subordinate : jQuery.Deferred(),

                // Update function for both resolve and progress values
                updateFunc = function (i, contexts, values) {
                    return function (value) {
                        contexts[i] = this;
                        values[i] = arguments.length > 1 ? core_slice.call(arguments) : value;
                        if (values === progressValues) {
                            deferred.notifyWith(contexts, values);
                        } else if (!(--remaining)) {
                            deferred.resolveWith(contexts, values);
                        }
                    };
                },

                progressValues, progressContexts, resolveContexts;

            // add listeners to Deferred subordinates; treat others as resolved
            if (length > 1) {
                progressValues = new Array(length);
                progressContexts = new Array(length);
                resolveContexts = new Array(length);
                for (; i < length; i++) {
                    if (resolveValues[i] && jQuery.isFunction(resolveValues[i].promise)) {
                        resolveValues[i].promise()
                            .done(updateFunc(i, resolveContexts, resolveValues))
                            .fail(deferred.reject)
                            .progress(updateFunc(i, progressContexts, progressValues));
                    } else {
                        --remaining;
                    }
                }
            }

            // if we're not waiting on anything, resolve the master
            if (!remaining) {
                deferred.resolveWith(resolveContexts, resolveValues);
            }

            return deferred.promise();
        }
    });
    jQuery.support = (function () {

        var support,
            all,
            a,
            select,
            opt,
            input,
            fragment,
            eventName,
            i,
            isSupported,
            clickFn,
            div = document.createElement("div");

        // Setup
        div.setAttribute("className", "t");
        div.innerHTML = "  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>";

        // Support tests won't run in some limited or non-browser environments
        all = div.getElementsByTagName("*");
        a = div.getElementsByTagName("a")[0];
        if (!all || !a || !all.length) {
            return {};
        }

        // First batch of tests
        select = document.createElement("select");
        opt = select.appendChild(document.createElement("option"));
        input = div.getElementsByTagName("input")[0];

        a.style.cssText = "top:1px;float:left;opacity:.5";
        support = {
            // IE strips leading whitespace when .innerHTML is used
            leadingWhitespace: (div.firstChild.nodeType === 3),

            // Make sure that tbody elements aren't automatically inserted
            // IE will insert them into empty tables
            tbody: !div.getElementsByTagName("tbody").length,

            // Make sure that link elements get serialized correctly by innerHTML
            // This requires a wrapper element in IE
            htmlSerialize: !!div.getElementsByTagName("link").length,

            // Get the style information from getAttribute
            // (IE uses .cssText instead)
            style: /top/.test(a.getAttribute("style")),

            // Make sure that URLs aren't manipulated
            // (IE normalizes it by default)
            hrefNormalized: (a.getAttribute("href") === "/a"),

            // Make sure that element opacity exists
            // (IE uses filter instead)
            // Use a regex to work around a WebKit issue. See #5145
            opacity: /^0.5/.test(a.style.opacity),

            // Verify style float existence
            // (IE uses styleFloat instead of cssFloat)
            cssFloat: !!a.style.cssFloat,

            // Make sure that if no value is specified for a checkbox
            // that it defaults to "on".
            // (WebKit defaults to "" instead)
            checkOn: (input.value === "on"),

            // Make sure that a selected-by-default option has a working selected property.
            // (WebKit defaults to false instead of true, IE too, if it's in an optgroup)
            optSelected: opt.selected,

            // Test setAttribute on camelCase class. If it works, we need attrFixes when doing get/setAttribute (ie6/7)
            getSetAttribute: div.className !== "t",

            // Tests for enctype support on a form (#6743)
            enctype: !!document.createElement("form").enctype,

            // Makes sure cloning an html5 element does not cause problems
            // Where outerHTML is undefined, this still works
            html5Clone: document.createElement("nav").cloneNode(true).outerHTML !== "<:nav></:nav>",

            // jQuery.support.boxModel DEPRECATED in 1.8 since we don't support Quirks Mode
            boxModel: (document.compatMode === "CSS1Compat"),

            // Will be defined later
            submitBubbles: true,
            changeBubbles: true,
            focusinBubbles: false,
            deleteExpando: true,
            noCloneEvent: true,
            inlineBlockNeedsLayout: false,
            shrinkWrapBlocks: false,
            reliableMarginRight: true,
            boxSizingReliable: true,
            pixelPosition: false
        };

        // Make sure checked status is properly cloned
        input.checked = true;
        support.noCloneChecked = input.cloneNode(true).checked;

        // Make sure that the options inside disabled selects aren't marked as disabled
        // (WebKit marks them as disabled)
        select.disabled = true;
        support.optDisabled = !opt.disabled;

        // Test to see if it's possible to delete an expando from an element
        // Fails in Internet Explorer
        try {
            delete div.test;
        } catch (e) {
            support.deleteExpando = false;
        }

        if (!div.addEventListener && div.attachEvent && div.fireEvent) {
            div.attachEvent("onclick", clickFn = function () {
                // Cloning a node shouldn't copy over any
                // bound event handlers (IE does this)
                support.noCloneEvent = false;
            });
            div.cloneNode(true).fireEvent("onclick");
            div.detachEvent("onclick", clickFn);
        }

        // Check if a radio maintains its value
        // after being appended to the DOM
        input = document.createElement("input");
        input.value = "t";
        input.setAttribute("type", "radio");
        support.radioValue = input.value === "t";

        input.setAttribute("checked", "checked");

        // #11217 - WebKit loses check when the name is after the checked attribute
        input.setAttribute("name", "t");

        div.appendChild(input);
        fragment = document.createDocumentFragment();
        fragment.appendChild(div.lastChild);

        // WebKit doesn't clone checked state correctly in fragments
        support.checkClone = fragment.cloneNode(true).cloneNode(true).lastChild.checked;

        // Check if a disconnected checkbox will retain its checked
        // value of true after appended to the DOM (IE6/7)
        support.appendChecked = input.checked;

        fragment.removeChild(input);
        fragment.appendChild(div);

        // Technique from Juriy Zaytsev
        // http://perfectionkills.com/detecting-event-support-without-browser-sniffing/
        // We only care about the case where non-standard event systems
        // are used, namely in IE. Short-circuiting here helps us to
        // avoid an eval call (in setAttribute) which can cause CSP
        // to go haywire. See: https://developer.mozilla.org/en/Security/CSP
        if (div.attachEvent) {
            for (i in {
                submit: true,
                change: true,
                focusin: true
            }) {
                eventName = "on" + i;
                isSupported = (eventName in div);
                if (!isSupported) {
                    div.setAttribute(eventName, "return;");
                    isSupported = (typeof div[eventName] === "function");
                }
                support[i + "Bubbles"] = isSupported;
            }
        }

        // Run tests that need a body at doc ready
        jQuery(function () {
            var container, div, tds, marginDiv,
                divReset = "padding:0;margin:0;border:0;display:block;overflow:hidden;",
                body = document.getElementsByTagName("body")[0];

            if (!body) {
                // Return for frameset docs that don't have a body
                return;
            }

            container = document.createElement("div");
            container.style.cssText = "visibility:hidden;border:0;width:0;height:0;position:static;top:0;margin-top:1px";
            body.insertBefore(container, body.firstChild);

            // Construct the test element
            div = document.createElement("div");
            container.appendChild(div);

            // Check if table cells still have offsetWidth/Height when they are set
            // to display:none and there are still other visible table cells in a
            // table row; if so, offsetWidth/Height are not reliable for use when
            // determining if an element has been hidden directly using
            // display:none (it is still safe to use offsets if a parent element is
            // hidden; don safety goggles and see bug #4512 for more information).
            // (only IE 8 fails this test)
            div.innerHTML = "<table><tr><td></td><td>t</td></tr></table>";
            tds = div.getElementsByTagName("td");
            tds[0].style.cssText = "padding:0;margin:0;border:0;display:none";
            isSupported = (tds[0].offsetHeight === 0);

            tds[0].style.display = "";
            tds[1].style.display = "none";

            // Check if empty table cells still have offsetWidth/Height
            // (IE <= 8 fail this test)
            support.reliableHiddenOffsets = isSupported && (tds[0].offsetHeight === 0);

            // Check box-sizing and margin behavior
            div.innerHTML = "";
            div.style.cssText = "box-sizing:border-box;-moz-box-sizing:border-box;-webkit-box-sizing:border-box;padding:1px;border:1px;display:block;width:4px;margin-top:1%;position:absolute;top:1%;";
            support.boxSizing = (div.offsetWidth === 4);
            support.doesNotIncludeMarginInBodyOffset = (body.offsetTop !== 1);

            // NOTE: To any future maintainer, we've window.getComputedStyle
            // because jsdom on node.js will break without it.
            if (window.getComputedStyle) {
                support.pixelPosition = (window.getComputedStyle(div, null) || {}).top !== "1%";
                support.boxSizingReliable = (window.getComputedStyle(div, null) || { width: "4px" }).width === "4px";

                // Check if div with explicit width and no margin-right incorrectly
                // gets computed margin-right based on width of container. For more
                // info see bug #3333
                // Fails in WebKit before Feb 2011 nightlies
                // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
                marginDiv = document.createElement("div");
                marginDiv.style.cssText = div.style.cssText = divReset;
                marginDiv.style.marginRight = marginDiv.style.width = "0";
                div.style.width = "1px";
                div.appendChild(marginDiv);
                support.reliableMarginRight =
                    !parseFloat((window.getComputedStyle(marginDiv, null) || {}).marginRight);
            }

            if (typeof div.style.zoom !== "undefined") {
                // Check if natively block-level elements act like inline-block
                // elements when setting their display to 'inline' and giving
                // them layout
                // (IE < 8 does this)
                div.innerHTML = "";
                div.style.cssText = divReset + "width:1px;padding:1px;display:inline;zoom:1";
                support.inlineBlockNeedsLayout = (div.offsetWidth === 3);

                // Check if elements with layout shrink-wrap their children
                // (IE 6 does this)
                div.style.display = "block";
                div.style.overflow = "visible";
                div.innerHTML = "<div></div>";
                div.firstChild.style.width = "5px";
                support.shrinkWrapBlocks = (div.offsetWidth !== 3);

                container.style.zoom = 1;
            }

            // Null elements to avoid leaks in IE
            body.removeChild(container);
            container = div = tds = marginDiv = null;
        });

        // Null elements to avoid leaks in IE
        fragment.removeChild(div);
        all = a = select = opt = input = fragment = div = null;

        return support;
    })();
    var rbrace = /(?:\{[\s\S]*\}|\[[\s\S]*\])$/,
        rmultiDash = /([A-Z])/g;

    jQuery.extend({
        cache: {},

        deletedIds: [],

        // Remove at next major release (1.9/2.0)
        uuid: 0,

        // Unique for each copy of jQuery on the page
        // Non-digits removed to match rinlinejQuery
        expando: "jQuery" + (jQuery.fn.jquery + Math.random()).replace(/\D/g, ""),

        // The following elements throw uncatchable exceptions if you
        // attempt to add expando properties to them.
        noData: {
            "embed": true,
            // Ban all objects except for Flash (which handle expandos)
            "object": "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
            "applet": true
        },

        hasData: function (elem) {
            elem = elem.nodeType ? jQuery.cache[elem[jQuery.expando]] : elem[jQuery.expando];
            return !!elem && !isEmptyDataObject(elem);
        },

        data: function (elem, name, data, pvt /* Internal Use Only */) {
            if (!jQuery.acceptData(elem)) {
                return;
            }

            var thisCache, ret,
                internalKey = jQuery.expando,
                getByName = typeof name === "string",

                // We have to handle DOM nodes and JS objects differently because IE6-7
                // can't GC object references properly across the DOM-JS boundary
                isNode = elem.nodeType,

                // Only DOM nodes need the global jQuery cache; JS object data is
                // attached directly to the object so GC can occur automatically
                cache = isNode ? jQuery.cache : elem,

                // Only defining an ID for JS objects if its cache already exists allows
                // the code to shortcut on the same path as a DOM node with no cache
                id = isNode ? elem[internalKey] : elem[internalKey] && internalKey;

            // Avoid doing any more work than we need to when trying to get data on an
            // object that has no data at all
            if ((!id || !cache[id] || (!pvt && !cache[id].data)) && getByName && data === undefined) {
                return;
            }

            if (!id) {
                // Only DOM nodes need a new unique ID for each element since their data
                // ends up in the global cache
                if (isNode) {
                    elem[internalKey] = id = jQuery.deletedIds.pop() || jQuery.guid++;
                } else {
                    id = internalKey;
                }
            }

            if (!cache[id]) {
                cache[id] = {};

                // Avoids exposing jQuery metadata on plain JS objects when the object
                // is serialized using JSON.stringify
                if (!isNode) {
                    cache[id].toJSON = jQuery.noop;
                }
            }

            // An object can be passed to jQuery.data instead of a key/value pair; this gets
            // shallow copied over onto the existing cache
            if (typeof name === "object" || typeof name === "function") {
                if (pvt) {
                    cache[id] = jQuery.extend(cache[id], name);
                } else {
                    cache[id].data = jQuery.extend(cache[id].data, name);
                }
            }

            thisCache = cache[id];

            // jQuery data() is stored in a separate object inside the object's internal data
            // cache in order to avoid key collisions between internal data and user-defined
            // data.
            if (!pvt) {
                if (!thisCache.data) {
                    thisCache.data = {};
                }

                thisCache = thisCache.data;
            }

            if (data !== undefined) {
                thisCache[jQuery.camelCase(name)] = data;
            }

            // Check for both converted-to-camel and non-converted data property names
            // If a data property was specified
            if (getByName) {

                // First Try to find as-is property data
                ret = thisCache[name];

                // Test for null|undefined property data
                if (ret == null) {

                    // Try to find the camelCased property
                    ret = thisCache[jQuery.camelCase(name)];
                }
            } else {
                ret = thisCache;
            }

            return ret;
        },

        removeData: function (elem, name, pvt /* Internal Use Only */) {
            if (!jQuery.acceptData(elem)) {
                return;
            }

            var thisCache, i, l,

                isNode = elem.nodeType,

                // See jQuery.data for more information
                cache = isNode ? jQuery.cache : elem,
                id = isNode ? elem[jQuery.expando] : jQuery.expando;

            // If there is already no cache entry for this object, there is no
            // purpose in continuing
            if (!cache[id]) {
                return;
            }

            if (name) {

                thisCache = pvt ? cache[id] : cache[id].data;

                if (thisCache) {

                    // Support array or space separated string names for data keys
                    if (!jQuery.isArray(name)) {

                        // try the string as a key before any manipulation
                        if (name in thisCache) {
                            name = [name];
                        } else {

                            // split the camel cased version by spaces unless a key with the spaces exists
                            name = jQuery.camelCase(name);
                            if (name in thisCache) {
                                name = [name];
                            } else {
                                name = name.split(" ");
                            }
                        }
                    }

                    for (i = 0, l = name.length; i < l; i++) {
                        delete thisCache[name[i]];
                    }

                    // If there is no data left in the cache, we want to continue
                    // and let the cache object itself get destroyed
                    if (!(pvt ? isEmptyDataObject : jQuery.isEmptyObject)(thisCache)) {
                        return;
                    }
                }
            }

            // See jQuery.data for more information
            if (!pvt) {
                delete cache[id].data;

                // Don't destroy the parent cache unless the internal data object
                // had been the only thing left in it
                if (!isEmptyDataObject(cache[id])) {
                    return;
                }
            }

            // Destroy the cache
            if (isNode) {
                jQuery.cleanData([elem], true);

                // Use delete when supported for expandos or `cache` is not a window per isWindow (#10080)
            } else if (jQuery.support.deleteExpando || cache != cache.window) {
                delete cache[id];

                // When all else fails, null
            } else {
                cache[id] = null;
            }
        },

        // For internal use only.
        _data: function (elem, name, data) {
            return jQuery.data(elem, name, data, true);
        },

        // A method for determining if a DOM node can handle the data expando
        acceptData: function (elem) {
            var noData = elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()];

            // nodes accept data unless otherwise specified; rejection can be conditional
            return !noData || noData !== true && elem.getAttribute("classid") === noData;
        }
    });

    jQuery.fn.extend({
        data: function (key, value) {
            var parts, part, attr, name, l,
                elem = this[0],
                i = 0,
                data = null;

            // Gets all values
            if (key === undefined) {
                if (this.length) {
                    data = jQuery.data(elem);

                    if (elem.nodeType === 1 && !jQuery._data(elem, "parsedAttrs")) {
                        attr = elem.attributes;
                        for (l = attr.length; i < l; i++) {
                            name = attr[i].name;

                            if (!name.indexOf("data-")) {
                                name = jQuery.camelCase(name.substring(5));

                                dataAttr(elem, name, data[name]);
                            }
                        }
                        jQuery._data(elem, "parsedAttrs", true);
                    }
                }

                return data;
            }

            // Sets multiple values
            if (typeof key === "object") {
                return this.each(function () {
                    jQuery.data(this, key);
                });
            }

            parts = key.split(".", 2);
            parts[1] = parts[1] ? "." + parts[1] : "";
            part = parts[1] + "!";

            return jQuery.access(this, function (value) {

                if (value === undefined) {
                    data = this.triggerHandler("getData" + part, [parts[0]]);

                    // Try to fetch any internally stored data first
                    if (data === undefined && elem) {
                        data = jQuery.data(elem, key);
                        data = dataAttr(elem, key, data);
                    }

                    return data === undefined && parts[1] ?
                        this.data(parts[0]) :
                        data;
                }

                parts[1] = value;
                this.each(function () {
                    var self = jQuery(this);

                    self.triggerHandler("setData" + part, parts);
                    jQuery.data(this, key, value);
                    self.triggerHandler("changeData" + part, parts);
                });
            }, null, value, arguments.length > 1, null, false);
        },

        removeData: function (key) {
            return this.each(function () {
                jQuery.removeData(this, key);
            });
        }
    });

    function dataAttr(elem, key, data) {
        // If nothing was found internally, try to fetch any
        // data from the HTML5 data-* attribute
        if (data === undefined && elem.nodeType === 1) {

            var name = "data-" + key.replace(rmultiDash, "-$1").toLowerCase();

            data = elem.getAttribute(name);

            if (typeof data === "string") {
                try {
                    data = data === "true" ? true :
                    data === "false" ? false :
                    data === "null" ? null :
                    // Only convert to a number if it doesn't change the string
                    +data + "" === data ? +data :
                    rbrace.test(data) ? jQuery.parseJSON(data) :
                        data;
                } catch (e) { }

                // Make sure we set the data so it isn't changed later
                jQuery.data(elem, key, data);

            } else {
                data = undefined;
            }
        }

        return data;
    }

    // checks a cache object for emptiness
    function isEmptyDataObject(obj) {
        var name;
        for (name in obj) {

            // if the public data object is empty, the private is still empty
            if (name === "data" && jQuery.isEmptyObject(obj[name])) {
                continue;
            }
            if (name !== "toJSON") {
                return false;
            }
        }

        return true;
    }
    jQuery.extend({
        queue: function (elem, type, data) {
            var queue;

            if (elem) {
                type = (type || "fx") + "queue";
                queue = jQuery._data(elem, type);

                // Speed up dequeue by getting out quickly if this is just a lookup
                if (data) {
                    if (!queue || jQuery.isArray(data)) {
                        queue = jQuery._data(elem, type, jQuery.makeArray(data));
                    } else {
                        queue.push(data);
                    }
                }
                return queue || [];
            }
        },

        dequeue: function (elem, type) {
            type = type || "fx";

            var queue = jQuery.queue(elem, type),
                startLength = queue.length,
                fn = queue.shift(),
                hooks = jQuery._queueHooks(elem, type),
                next = function () {
                    jQuery.dequeue(elem, type);
                };

            // If the fx queue is dequeued, always remove the progress sentinel
            if (fn === "inprogress") {
                fn = queue.shift();
                startLength--;
            }

            if (fn) {

                // Add a progress sentinel to prevent the fx queue from being
                // automatically dequeued
                if (type === "fx") {
                    queue.unshift("inprogress");
                }

                // clear up the last queue stop function
                delete hooks.stop;
                fn.call(elem, next, hooks);
            }

            if (!startLength && hooks) {
                hooks.empty.fire();
            }
        },

        // not intended for public consumption - generates a queueHooks object, or returns the current one
        _queueHooks: function (elem, type) {
            var key = type + "queueHooks";
            return jQuery._data(elem, key) || jQuery._data(elem, key, {
                empty: jQuery.Callbacks("once memory").add(function () {
                    jQuery.removeData(elem, type + "queue", true);
                    jQuery.removeData(elem, key, true);
                })
            });
        }
    });

    jQuery.fn.extend({
        queue: function (type, data) {
            var setter = 2;

            if (typeof type !== "string") {
                data = type;
                type = "fx";
                setter--;
            }

            if (arguments.length < setter) {
                return jQuery.queue(this[0], type);
            }

            return data === undefined ?
                this :
                this.each(function () {
                    var queue = jQuery.queue(this, type, data);

                    // ensure a hooks for this queue
                    jQuery._queueHooks(this, type);

                    if (type === "fx" && queue[0] !== "inprogress") {
                        jQuery.dequeue(this, type);
                    }
                });
        },
        dequeue: function (type) {
            return this.each(function () {
                jQuery.dequeue(this, type);
            });
        },
        // Based off of the plugin by Clint Helfers, with permission.
        // http://blindsignals.com/index.php/2009/07/jquery-delay/
        delay: function (time, type) {
            time = jQuery.fx ? jQuery.fx.speeds[time] || time : time;
            type = type || "fx";

            return this.queue(type, function (next, hooks) {
                var timeout = setTimeout(next, time);
                hooks.stop = function () {
                    clearTimeout(timeout);
                };
            });
        },
        clearQueue: function (type) {
            return this.queue(type || "fx", []);
        },
        // Get a promise resolved when queues of a certain type
        // are emptied (fx is the type by default)
        promise: function (type, obj) {
            var tmp,
                count = 1,
                defer = jQuery.Deferred(),
                elements = this,
                i = this.length,
                resolve = function () {
                    if (!(--count)) {
                        defer.resolveWith(elements, [elements]);
                    }
                };

            if (typeof type !== "string") {
                obj = type;
                type = undefined;
            }
            type = type || "fx";

            while (i--) {
                tmp = jQuery._data(elements[i], type + "queueHooks");
                if (tmp && tmp.empty) {
                    count++;
                    tmp.empty.add(resolve);
                }
            }
            resolve();
            return defer.promise(obj);
        }
    });
    var nodeHook, boolHook, fixSpecified,
        rclass = /[\t\r\n]/g,
        rreturn = /\r/g,
        rtype = /^(?:button|input)$/i,
        rfocusable = /^(?:button|input|object|select|textarea)$/i,
        rclickable = /^a(?:rea|)$/i,
        rboolean = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i,
        getSetAttribute = jQuery.support.getSetAttribute;

    jQuery.fn.extend({
        attr: function (name, value) {
            return jQuery.access(this, jQuery.attr, name, value, arguments.length > 1);
        },

        removeAttr: function (name) {
            return this.each(function () {
                jQuery.removeAttr(this, name);
            });
        },

        prop: function (name, value) {
            return jQuery.access(this, jQuery.prop, name, value, arguments.length > 1);
        },

        removeProp: function (name) {
            name = jQuery.propFix[name] || name;
            return this.each(function () {
                // try/catch handles cases where IE balks (such as removing a property on window)
                try {
                    this[name] = undefined;
                    delete this[name];
                } catch (e) { }
            });
        },

        addClass: function (value) {
            var classNames, i, l, elem,
                setClass, c, cl;

            if (jQuery.isFunction(value)) {
                return this.each(function (j) {
                    jQuery(this).addClass(value.call(this, j, this.className));
                });
            }

            if (value && typeof value === "string") {
                classNames = value.split(core_rspace);

                for (i = 0, l = this.length; i < l; i++) {
                    elem = this[i];

                    if (elem.nodeType === 1) {
                        if (!elem.className && classNames.length === 1) {
                            elem.className = value;

                        } else {
                            setClass = " " + elem.className + " ";

                            for (c = 0, cl = classNames.length; c < cl; c++) {
                                if (setClass.indexOf(" " + classNames[c] + " ") < 0) {
                                    setClass += classNames[c] + " ";
                                }
                            }
                            elem.className = jQuery.trim(setClass);
                        }
                    }
                }
            }

            return this;
        },

        removeClass: function (value) {
            var removes, className, elem, c, cl, i, l;

            if (jQuery.isFunction(value)) {
                return this.each(function (j) {
                    jQuery(this).removeClass(value.call(this, j, this.className));
                });
            }
            if ((value && typeof value === "string") || value === undefined) {
                removes = (value || "").split(core_rspace);

                for (i = 0, l = this.length; i < l; i++) {
                    elem = this[i];
                    if (elem.nodeType === 1 && elem.className) {

                        className = (" " + elem.className + " ").replace(rclass, " ");

                        // loop over each item in the removal list
                        for (c = 0, cl = removes.length; c < cl; c++) {
                            // Remove until there is nothing to remove,
                            while (className.indexOf(" " + removes[c] + " ") >= 0) {
                                className = className.replace(" " + removes[c] + " ", " ");
                            }
                        }
                        elem.className = value ? jQuery.trim(className) : "";
                    }
                }
            }

            return this;
        },

        toggleClass: function (value, stateVal) {
            var type = typeof value,
                isBool = typeof stateVal === "boolean";

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    jQuery(this).toggleClass(value.call(this, i, this.className, stateVal), stateVal);
                });
            }

            return this.each(function () {
                if (type === "string") {
                    // toggle individual class names
                    var className,
                        i = 0,
                        self = jQuery(this),
                        state = stateVal,
                        classNames = value.split(core_rspace);

                    while ((className = classNames[i++])) {
                        // check each className given, space separated list
                        state = isBool ? state : !self.hasClass(className);
                        self[state ? "addClass" : "removeClass"](className);
                    }

                } else if (type === "undefined" || type === "boolean") {
                    if (this.className) {
                        // store className if set
                        jQuery._data(this, "__className__", this.className);
                    }

                    // toggle whole className
                    this.className = this.className || value === false ? "" : jQuery._data(this, "__className__") || "";
                }
            });
        },

        hasClass: function (selector) {
            var className = " " + selector + " ",
                i = 0,
                l = this.length;
            for (; i < l; i++) {
                if (this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf(className) >= 0) {
                    return true;
                }
            }

            return false;
        },

        val: function (value) {
            var hooks, ret, isFunction,
                elem = this[0];

            if (!arguments.length) {
                if (elem) {
                    hooks = jQuery.valHooks[elem.type] || jQuery.valHooks[elem.nodeName.toLowerCase()];

                    if (hooks && "get" in hooks && (ret = hooks.get(elem, "value")) !== undefined) {
                        return ret;
                    }

                    ret = elem.value;

                    return typeof ret === "string" ?
                        // handle most common string cases
                        ret.replace(rreturn, "") :
                        // handle cases where value is null/undef or number
                        ret == null ? "" : ret;
                }

                return;
            }

            isFunction = jQuery.isFunction(value);

            return this.each(function (i) {
                var val,
                    self = jQuery(this);

                if (this.nodeType !== 1) {
                    return;
                }

                if (isFunction) {
                    val = value.call(this, i, self.val());
                } else {
                    val = value;
                }

                // Treat null/undefined as ""; convert numbers to string
                if (val == null) {
                    val = "";
                } else if (typeof val === "number") {
                    val += "";
                } else if (jQuery.isArray(val)) {
                    val = jQuery.map(val, function (value) {
                        return value == null ? "" : value + "";
                    });
                }

                hooks = jQuery.valHooks[this.type] || jQuery.valHooks[this.nodeName.toLowerCase()];

                // If set returns undefined, fall back to normal setting
                if (!hooks || !("set" in hooks) || hooks.set(this, val, "value") === undefined) {
                    this.value = val;
                }
            });
        }
    });

    jQuery.extend({
        valHooks: {
            option: {
                get: function (elem) {
                    // attributes.value is undefined in Blackberry 4.7 but
                    // uses .value. See #6932
                    var val = elem.attributes.value;
                    return !val || val.specified ? elem.value : elem.text;
                }
            },
            select: {
                get: function (elem) {
                    var value, option,
                        options = elem.options,
                        index = elem.selectedIndex,
                        one = elem.type === "select-one" || index < 0,
                        values = one ? null : [],
                        max = one ? index + 1 : options.length,
                        i = index < 0 ?
                        max :
                            one ? index : 0;

                    // Loop through all the selected options
                    for (; i < max; i++) {
                        option = options[i];

                        // oldIE doesn't update selected after form reset (#2551)
                        if ((option.selected || i === index) &&
                            // Don't return options that are disabled or in a disabled optgroup
                                (jQuery.support.optDisabled ? !option.disabled : option.getAttribute("disabled") === null) &&
                                (!option.parentNode.disabled || !jQuery.nodeName(option.parentNode, "optgroup"))) {

                            // Get the specific value for the option
                            value = jQuery(option).val();

                            // We don't need an array for one selects
                            if (one) {
                                return value;
                            }

                            // Multi-Selects return an array
                            values.push(value);
                        }
                    }

                    return values;
                },

                set: function (elem, value) {
                    var values = jQuery.makeArray(value);

                    jQuery(elem).find("option").each(function () {
                        this.selected = jQuery.inArray(jQuery(this).val(), values) >= 0;
                    });

                    if (!values.length) {
                        elem.selectedIndex = -1;
                    }
                    return values;
                }
            }
        },

        // Unused in 1.8, left in so attrFn-stabbers won't die; remove in 1.9
        attrFn: {},

        attr: function (elem, name, value, pass) {
            var ret, hooks, notxml,
                nType = elem.nodeType;

            // don't get/set attributes on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            if (pass && jQuery.isFunction(jQuery.fn[name])) {
                return jQuery(elem)[name](value);
            }

            // Fallback to prop when attributes are not supported
            if (typeof elem.getAttribute === "undefined") {
                return jQuery.prop(elem, name, value);
            }

            notxml = nType !== 1 || !jQuery.isXMLDoc(elem);

            // All attributes are lowercase
            // Grab necessary hook if one is defined
            if (notxml) {
                name = name.toLowerCase();
                hooks = jQuery.attrHooks[name] || (rboolean.test(name) ? boolHook : nodeHook);
            }

            if (value !== undefined) {

                if (value === null) {
                    jQuery.removeAttr(elem, name);
                    return;

                } else if (hooks && "set" in hooks && notxml && (ret = hooks.set(elem, value, name)) !== undefined) {
                    return ret;

                } else {
                    elem.setAttribute(name, value + "");
                    return value;
                }

            } else if (hooks && "get" in hooks && notxml && (ret = hooks.get(elem, name)) !== null) {
                return ret;

            } else {

                ret = elem.getAttribute(name);

                // Non-existent attributes return null, we normalize to undefined
                return ret === null ?
                    undefined :
                    ret;
            }
        },

        removeAttr: function (elem, value) {
            var propName, attrNames, name, isBool,
                i = 0;

            if (value && elem.nodeType === 1) {

                attrNames = value.split(core_rspace);

                for (; i < attrNames.length; i++) {
                    name = attrNames[i];

                    if (name) {
                        propName = jQuery.propFix[name] || name;
                        isBool = rboolean.test(name);

                        // See #9699 for explanation of this approach (setting first, then removal)
                        // Do not do this for boolean attributes (see #10870)
                        if (!isBool) {
                            jQuery.attr(elem, name, "");
                        }
                        elem.removeAttribute(getSetAttribute ? name : propName);

                        // Set corresponding property to false for boolean attributes
                        if (isBool && propName in elem) {
                            elem[propName] = false;
                        }
                    }
                }
            }
        },

        attrHooks: {
            type: {
                set: function (elem, value) {
                    // We can't allow the type property to be changed (since it causes problems in IE)
                    if (rtype.test(elem.nodeName) && elem.parentNode) {
                        jQuery.error("type property can't be changed");
                    } else if (!jQuery.support.radioValue && value === "radio" && jQuery.nodeName(elem, "input")) {
                        // Setting the type on a radio button after the value resets the value in IE6-9
                        // Reset value to it's default in case type is set after value
                        // This is for element creation
                        var val = elem.value;
                        elem.setAttribute("type", value);
                        if (val) {
                            elem.value = val;
                        }
                        return value;
                    }
                }
            },
            // Use the value property for back compat
            // Use the nodeHook for button elements in IE6/7 (#1954)
            value: {
                get: function (elem, name) {
                    if (nodeHook && jQuery.nodeName(elem, "button")) {
                        return nodeHook.get(elem, name);
                    }
                    return name in elem ?
                        elem.value :
                        null;
                },
                set: function (elem, value, name) {
                    if (nodeHook && jQuery.nodeName(elem, "button")) {
                        return nodeHook.set(elem, value, name);
                    }
                    // Does not return so that setAttribute is also used
                    elem.value = value;
                }
            }
        },

        propFix: {
            tabindex: "tabIndex",
            readonly: "readOnly",
            "for": "htmlFor",
            "class": "className",
            maxlength: "maxLength",
            cellspacing: "cellSpacing",
            cellpadding: "cellPadding",
            rowspan: "rowSpan",
            colspan: "colSpan",
            usemap: "useMap",
            frameborder: "frameBorder",
            contenteditable: "contentEditable"
        },

        prop: function (elem, name, value) {
            var ret, hooks, notxml,
                nType = elem.nodeType;

            // don't get/set properties on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            notxml = nType !== 1 || !jQuery.isXMLDoc(elem);

            if (notxml) {
                // Fix name and attach hooks
                name = jQuery.propFix[name] || name;
                hooks = jQuery.propHooks[name];
            }

            if (value !== undefined) {
                if (hooks && "set" in hooks && (ret = hooks.set(elem, value, name)) !== undefined) {
                    return ret;

                } else {
                    return (elem[name] = value);
                }

            } else {
                if (hooks && "get" in hooks && (ret = hooks.get(elem, name)) !== null) {
                    return ret;

                } else {
                    return elem[name];
                }
            }
        },

        propHooks: {
            tabIndex: {
                get: function (elem) {
                    // elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
                    // http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
                    var attributeNode = elem.getAttributeNode("tabindex");

                    return attributeNode && attributeNode.specified ?
                        parseInt(attributeNode.value, 10) :
                        rfocusable.test(elem.nodeName) || rclickable.test(elem.nodeName) && elem.href ?
                            0 :
                            undefined;
                }
            }
        }
    });

    // Hook for boolean attributes
    boolHook = {
        get: function (elem, name) {
            // Align boolean attributes with corresponding properties
            // Fall back to attribute presence where some booleans are not supported
            var attrNode,
                property = jQuery.prop(elem, name);
            return property === true || typeof property !== "boolean" && (attrNode = elem.getAttributeNode(name)) && attrNode.nodeValue !== false ?
                name.toLowerCase() :
                undefined;
        },
        set: function (elem, value, name) {
            var propName;
            if (value === false) {
                // Remove boolean attributes when set to false
                jQuery.removeAttr(elem, name);
            } else {
                // value is true since we know at this point it's type boolean and not false
                // Set boolean attributes to the same name and set the DOM property
                propName = jQuery.propFix[name] || name;
                if (propName in elem) {
                    // Only set the IDL specifically if it already exists on the element
                    elem[propName] = true;
                }

                elem.setAttribute(name, name.toLowerCase());
            }
            return name;
        }
    };

    // IE6/7 do not support getting/setting some attributes with get/setAttribute
    if (!getSetAttribute) {

        fixSpecified = {
            name: true,
            id: true,
            coords: true
        };

        // Use this for any attribute in IE6/7
        // This fixes almost every IE6/7 issue
        nodeHook = jQuery.valHooks.button = {
            get: function (elem, name) {
                var ret;
                ret = elem.getAttributeNode(name);
                return ret && (fixSpecified[name] ? ret.value !== "" : ret.specified) ?
                    ret.value :
                    undefined;
            },
            set: function (elem, value, name) {
                // Set the existing or create a new attribute node
                var ret = elem.getAttributeNode(name);
                if (!ret) {
                    ret = document.createAttribute(name);
                    elem.setAttributeNode(ret);
                }
                return (ret.value = value + "");
            }
        };

        // Set width and height to auto instead of 0 on empty string( Bug #8150 )
        // This is for removals
        jQuery.each(["width", "height"], function (i, name) {
            jQuery.attrHooks[name] = jQuery.extend(jQuery.attrHooks[name], {
                set: function (elem, value) {
                    if (value === "") {
                        elem.setAttribute(name, "auto");
                        return value;
                    }
                }
            });
        });

        // Set contenteditable to false on removals(#10429)
        // Setting to empty string throws an error as an invalid value
        jQuery.attrHooks.contenteditable = {
            get: nodeHook.get,
            set: function (elem, value, name) {
                if (value === "") {
                    value = "false";
                }
                nodeHook.set(elem, value, name);
            }
        };
    }


    // Some attributes require a special call on IE
    if (!jQuery.support.hrefNormalized) {
        jQuery.each(["href", "src", "width", "height"], function (i, name) {
            jQuery.attrHooks[name] = jQuery.extend(jQuery.attrHooks[name], {
                get: function (elem) {
                    var ret = elem.getAttribute(name, 2);
                    return ret === null ? undefined : ret;
                }
            });
        });
    }

    if (!jQuery.support.style) {
        jQuery.attrHooks.style = {
            get: function (elem) {
                // Return undefined in the case of empty string
                // Normalize to lowercase since IE uppercases css property names
                return elem.style.cssText.toLowerCase() || undefined;
            },
            set: function (elem, value) {
                return (elem.style.cssText = value + "");
            }
        };
    }

    // Safari mis-reports the default selected property of an option
    // Accessing the parent's selectedIndex property fixes it
    if (!jQuery.support.optSelected) {
        jQuery.propHooks.selected = jQuery.extend(jQuery.propHooks.selected, {
            get: function (elem) {
                var parent = elem.parentNode;

                if (parent) {
                    parent.selectedIndex;

                    // Make sure that it also works with optgroups, see #5701
                    if (parent.parentNode) {
                        parent.parentNode.selectedIndex;
                    }
                }
                return null;
            }
        });
    }

    // IE6/7 call enctype encoding
    if (!jQuery.support.enctype) {
        jQuery.propFix.enctype = "encoding";
    }

    // Radios and checkboxes getter/setter
    if (!jQuery.support.checkOn) {
        jQuery.each(["radio", "checkbox"], function () {
            jQuery.valHooks[this] = {
                get: function (elem) {
                    // Handle the case where in Webkit "" is returned instead of "on" if a value isn't specified
                    return elem.getAttribute("value") === null ? "on" : elem.value;
                }
            };
        });
    }
    jQuery.each(["radio", "checkbox"], function () {
        jQuery.valHooks[this] = jQuery.extend(jQuery.valHooks[this], {
            set: function (elem, value) {
                if (jQuery.isArray(value)) {
                    return (elem.checked = jQuery.inArray(jQuery(elem).val(), value) >= 0);
                }
            }
        });
    });
    var rformElems = /^(?:textarea|input|select)$/i,
        rtypenamespace = /^([^\.]*|)(?:\.(.+)|)$/,
        rhoverHack = /(?:^|\s)hover(\.\S+|)\b/,
        rkeyEvent = /^key/,
        rmouseEvent = /^(?:mouse|contextmenu)|click/,
        rfocusMorph = /^(?:focusinfocus|focusoutblur)$/,
        hoverHack = function (events) {
            return jQuery.event.special.hover ? events : events.replace(rhoverHack, "mouseenter$1 mouseleave$1");
        };

    /*
     * Helper functions for managing events -- not part of the public interface.
     * Props to Dean Edwards' addEvent library for many of the ideas.
     */
    jQuery.event = {

        add: function (elem, types, handler, data, selector) {

            var elemData, eventHandle, events,
                t, tns, type, namespaces, handleObj,
                handleObjIn, handlers, special;

            // Don't attach events to noData or text/comment nodes (allow plain objects tho)
            if (elem.nodeType === 3 || elem.nodeType === 8 || !types || !handler || !(elemData = jQuery._data(elem))) {
                return;
            }

            // Caller can pass in an object of custom data in lieu of the handler
            if (handler.handler) {
                handleObjIn = handler;
                handler = handleObjIn.handler;
                selector = handleObjIn.selector;
            }

            // Make sure that the handler has a unique ID, used to find/remove it later
            if (!handler.guid) {
                handler.guid = jQuery.guid++;
            }

            // Init the element's event structure and main handler, if this is the first
            events = elemData.events;
            if (!events) {
                elemData.events = events = {};
            }
            eventHandle = elemData.handle;
            if (!eventHandle) {
                elemData.handle = eventHandle = function (e) {
                    // Discard the second event of a jQuery.event.trigger() and
                    // when an event is called after a page has unloaded
                    return typeof jQuery !== "undefined" && (!e || jQuery.event.triggered !== e.type) ?
                        jQuery.event.dispatch.apply(eventHandle.elem, arguments) :
                        undefined;
                };
                // Add elem as a property of the handle fn to prevent a memory leak with IE non-native events
                eventHandle.elem = elem;
            }

            // Handle multiple events separated by a space
            // jQuery(...).bind("mouseover mouseout", fn);
            types = jQuery.trim(hoverHack(types)).split(" ");
            for (t = 0; t < types.length; t++) {

                tns = rtypenamespace.exec(types[t]) || [];
                type = tns[1];
                namespaces = (tns[2] || "").split(".").sort();

                // If event changes its type, use the special event handlers for the changed type
                special = jQuery.event.special[type] || {};

                // If selector defined, determine special event api type, otherwise given type
                type = (selector ? special.delegateType : special.bindType) || type;

                // Update special based on newly reset type
                special = jQuery.event.special[type] || {};

                // handleObj is passed to all event handlers
                handleObj = jQuery.extend({
                    type: type,
                    origType: tns[1],
                    data: data,
                    handler: handler,
                    guid: handler.guid,
                    selector: selector,
                    needsContext: selector && jQuery.expr.match.needsContext.test(selector),
                    namespace: namespaces.join(".")
                }, handleObjIn);

                // Init the event handler queue if we're the first
                handlers = events[type];
                if (!handlers) {
                    handlers = events[type] = [];
                    handlers.delegateCount = 0;

                    // Only use addEventListener/attachEvent if the special events handler returns false
                    if (!special.setup || special.setup.call(elem, data, namespaces, eventHandle) === false) {
                        // Bind the global event handler to the element
                        if (elem.addEventListener) {
                            elem.addEventListener(type, eventHandle, false);

                        } else if (elem.attachEvent) {
                            elem.attachEvent("on" + type, eventHandle);
                        }
                    }
                }

                if (special.add) {
                    special.add.call(elem, handleObj);

                    if (!handleObj.handler.guid) {
                        handleObj.handler.guid = handler.guid;
                    }
                }

                // Add to the element's handler list, delegates in front
                if (selector) {
                    handlers.splice(handlers.delegateCount++, 0, handleObj);
                } else {
                    handlers.push(handleObj);
                }

                // Keep track of which events have ever been used, for event optimization
                jQuery.event.global[type] = true;
            }

            // Nullify elem to prevent memory leaks in IE
            elem = null;
        },

        global: {},

        // Detach an event or set of events from an element
        remove: function (elem, types, handler, selector, mappedTypes) {

            var t, tns, type, origType, namespaces, origCount,
                j, events, special, eventType, handleObj,
                elemData = jQuery.hasData(elem) && jQuery._data(elem);

            if (!elemData || !(events = elemData.events)) {
                return;
            }

            // Once for each type.namespace in types; type may be omitted
            types = jQuery.trim(hoverHack(types || "")).split(" ");
            for (t = 0; t < types.length; t++) {
                tns = rtypenamespace.exec(types[t]) || [];
                type = origType = tns[1];
                namespaces = tns[2];

                // Unbind all events (on this namespace, if provided) for the element
                if (!type) {
                    for (type in events) {
                        jQuery.event.remove(elem, type + types[t], handler, selector, true);
                    }
                    continue;
                }

                special = jQuery.event.special[type] || {};
                type = (selector ? special.delegateType : special.bindType) || type;
                eventType = events[type] || [];
                origCount = eventType.length;
                namespaces = namespaces ? new RegExp("(^|\\.)" + namespaces.split(".").sort().join("\\.(?:.*\\.|)") + "(\\.|$)") : null;

                // Remove matching events
                for (j = 0; j < eventType.length; j++) {
                    handleObj = eventType[j];

                    if ((mappedTypes || origType === handleObj.origType) &&
                         (!handler || handler.guid === handleObj.guid) &&
                         (!namespaces || namespaces.test(handleObj.namespace)) &&
                         (!selector || selector === handleObj.selector || selector === "**" && handleObj.selector)) {
                        eventType.splice(j--, 1);

                        if (handleObj.selector) {
                            eventType.delegateCount--;
                        }
                        if (special.remove) {
                            special.remove.call(elem, handleObj);
                        }
                    }
                }

                // Remove generic event handler if we removed something and no more handlers exist
                // (avoids potential for endless recursion during removal of special event handlers)
                if (eventType.length === 0 && origCount !== eventType.length) {
                    if (!special.teardown || special.teardown.call(elem, namespaces, elemData.handle) === false) {
                        jQuery.removeEvent(elem, type, elemData.handle);
                    }

                    delete events[type];
                }
            }

            // Remove the expando if it's no longer used
            if (jQuery.isEmptyObject(events)) {
                delete elemData.handle;

                // removeData also checks for emptiness and clears the expando if empty
                // so use it instead of delete
                jQuery.removeData(elem, "events", true);
            }
        },

        // Events that are safe to short-circuit if no handlers are attached.
        // Native DOM events should not be added, they may have inline handlers.
        customEvent: {
            "getData": true,
            "setData": true,
            "changeData": true
        },

        trigger: function (event, data, elem, onlyHandlers) {
            // Don't do events on text and comment nodes
            if (elem && (elem.nodeType === 3 || elem.nodeType === 8)) {
                return;
            }

            // Event object or event type
            var cache, exclusive, i, cur, old, ontype, special, handle, eventPath, bubbleType,
                type = event.type || event,
                namespaces = [];

            // focus/blur morphs to focusin/out; ensure we're not firing them right now
            if (rfocusMorph.test(type + jQuery.event.triggered)) {
                return;
            }

            if (type.indexOf("!") >= 0) {
                // Exclusive events trigger only for the exact event (no namespaces)
                type = type.slice(0, -1);
                exclusive = true;
            }

            if (type.indexOf(".") >= 0) {
                // Namespaced trigger; create a regexp to match event type in handle()
                namespaces = type.split(".");
                type = namespaces.shift();
                namespaces.sort();
            }

            if ((!elem || jQuery.event.customEvent[type]) && !jQuery.event.global[type]) {
                // No jQuery handlers for this event type, and it can't have inline handlers
                return;
            }

            // Caller can pass in an Event, Object, or just an event type string
            event = typeof event === "object" ?
                // jQuery.Event object
                event[jQuery.expando] ? event :
                // Object literal
                new jQuery.Event(type, event) :
                // Just the event type (string)
                new jQuery.Event(type);

            event.type = type;
            event.isTrigger = true;
            event.exclusive = exclusive;
            event.namespace = namespaces.join(".");
            event.namespace_re = event.namespace ? new RegExp("(^|\\.)" + namespaces.join("\\.(?:.*\\.|)") + "(\\.|$)") : null;
            ontype = type.indexOf(":") < 0 ? "on" + type : "";

            // Handle a global trigger
            if (!elem) {

                // TODO: Stop taunting the data cache; remove global events and always attach to document
                cache = jQuery.cache;
                for (i in cache) {
                    if (cache[i].events && cache[i].events[type]) {
                        jQuery.event.trigger(event, data, cache[i].handle.elem, true);
                    }
                }
                return;
            }

            // Clean up the event in case it is being reused
            event.result = undefined;
            if (!event.target) {
                event.target = elem;
            }

            // Clone any incoming data and prepend the event, creating the handler arg list
            data = data != null ? jQuery.makeArray(data) : [];
            data.unshift(event);

            // Allow special events to draw outside the lines
            special = jQuery.event.special[type] || {};
            if (special.trigger && special.trigger.apply(elem, data) === false) {
                return;
            }

            // Determine event propagation path in advance, per W3C events spec (#9951)
            // Bubble up to document, then to window; watch for a global ownerDocument var (#9724)
            eventPath = [[elem, special.bindType || type]];
            if (!onlyHandlers && !special.noBubble && !jQuery.isWindow(elem)) {

                bubbleType = special.delegateType || type;
                cur = rfocusMorph.test(bubbleType + type) ? elem : elem.parentNode;
                for (old = elem; cur; cur = cur.parentNode) {
                    eventPath.push([cur, bubbleType]);
                    old = cur;
                }

                // Only add window if we got to document (e.g., not plain obj or detached DOM)
                if (old === (elem.ownerDocument || document)) {
                    eventPath.push([old.defaultView || old.parentWindow || window, bubbleType]);
                }
            }

            // Fire handlers on the event path
            for (i = 0; i < eventPath.length && !event.isPropagationStopped() ; i++) {

                cur = eventPath[i][0];
                event.type = eventPath[i][1];

                handle = (jQuery._data(cur, "events") || {})[event.type] && jQuery._data(cur, "handle");
                if (handle) {
                    handle.apply(cur, data);
                }
                // Note that this is a bare JS function and not a jQuery handler
                handle = ontype && cur[ontype];
                if (handle && jQuery.acceptData(cur) && handle.apply && handle.apply(cur, data) === false) {
                    event.preventDefault();
                }
            }
            event.type = type;

            // If nobody prevented the default action, do it now
            if (!onlyHandlers && !event.isDefaultPrevented()) {

                if ((!special._default || special._default.apply(elem.ownerDocument, data) === false) &&
                    !(type === "click" && jQuery.nodeName(elem, "a")) && jQuery.acceptData(elem)) {

                    // Call a native DOM method on the target with the same name name as the event.
                    // Can't use an .isFunction() check here because IE6/7 fails that test.
                    // Don't do default actions on window, that's where global variables be (#6170)
                    // IE<9 dies on focus/blur to hidden element (#1486)
                    if (ontype && elem[type] && ((type !== "focus" && type !== "blur") || event.target.offsetWidth !== 0) && !jQuery.isWindow(elem)) {

                        // Don't re-trigger an onFOO event when we call its FOO() method
                        old = elem[ontype];

                        if (old) {
                            elem[ontype] = null;
                        }

                        // Prevent re-triggering of the same event, since we already bubbled it above
                        jQuery.event.triggered = type;
                        elem[type]();
                        jQuery.event.triggered = undefined;

                        if (old) {
                            elem[ontype] = old;
                        }
                    }
                }
            }

            return event.result;
        },

        dispatch: function (event) {

            // Make a writable jQuery.Event from the native event object
            event = jQuery.event.fix(event || window.event);

            var i, j, cur, ret, selMatch, matched, matches, handleObj, sel, related,
                handlers = ((jQuery._data(this, "events") || {})[event.type] || []),
                delegateCount = handlers.delegateCount,
                args = core_slice.call(arguments),
                run_all = !event.exclusive && !event.namespace,
                special = jQuery.event.special[event.type] || {},
                handlerQueue = [];

            // Use the fix-ed jQuery.Event rather than the (read-only) native event
            args[0] = event;
            event.delegateTarget = this;

            // Call the preDispatch hook for the mapped type, and let it bail if desired
            if (special.preDispatch && special.preDispatch.call(this, event) === false) {
                return;
            }

            // Determine handlers that should run if there are delegated events
            // Avoid non-left-click bubbling in Firefox (#3861)
            if (delegateCount && !(event.button && event.type === "click")) {

                for (cur = event.target; cur != this; cur = cur.parentNode || this) {

                    // Don't process clicks (ONLY) on disabled elements (#6911, #8165, #11382, #11764)
                    if (cur.disabled !== true || event.type !== "click") {
                        selMatch = {};
                        matches = [];
                        for (i = 0; i < delegateCount; i++) {
                            handleObj = handlers[i];
                            sel = handleObj.selector;

                            if (selMatch[sel] === undefined) {
                                selMatch[sel] = handleObj.needsContext ?
                                    jQuery(sel, this).index(cur) >= 0 :
                                    jQuery.find(sel, this, null, [cur]).length;
                            }
                            if (selMatch[sel]) {
                                matches.push(handleObj);
                            }
                        }
                        if (matches.length) {
                            handlerQueue.push({ elem: cur, matches: matches });
                        }
                    }
                }
            }

            // Add the remaining (directly-bound) handlers
            if (handlers.length > delegateCount) {
                handlerQueue.push({ elem: this, matches: handlers.slice(delegateCount) });
            }

            // Run delegates first; they may want to stop propagation beneath us
            for (i = 0; i < handlerQueue.length && !event.isPropagationStopped() ; i++) {
                matched = handlerQueue[i];
                event.currentTarget = matched.elem;

                for (j = 0; j < matched.matches.length && !event.isImmediatePropagationStopped() ; j++) {
                    handleObj = matched.matches[j];

                    // Triggered event must either 1) be non-exclusive and have no namespace, or
                    // 2) have namespace(s) a subset or equal to those in the bound event (both can have no namespace).
                    if (run_all || (!event.namespace && !handleObj.namespace) || event.namespace_re && event.namespace_re.test(handleObj.namespace)) {

                        event.data = handleObj.data;
                        event.handleObj = handleObj;

                        ret = ((jQuery.event.special[handleObj.origType] || {}).handle || handleObj.handler)
                                .apply(matched.elem, args);

                        if (ret !== undefined) {
                            event.result = ret;
                            if (ret === false) {
                                event.preventDefault();
                                event.stopPropagation();
                            }
                        }
                    }
                }
            }

            // Call the postDispatch hook for the mapped type
            if (special.postDispatch) {
                special.postDispatch.call(this, event);
            }

            return event.result;
        },

        // Includes some event props shared by KeyEvent and MouseEvent
        // *** attrChange attrName relatedNode srcElement  are not normalized, non-W3C, deprecated, will be removed in 1.8 ***
        props: "attrChange attrName relatedNode srcElement altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),

        fixHooks: {},

        keyHooks: {
            props: "char charCode key keyCode".split(" "),
            filter: function (event, original) {

                // Add which for key events
                if (event.which == null) {
                    event.which = original.charCode != null ? original.charCode : original.keyCode;
                }

                return event;
            }
        },

        mouseHooks: {
            props: "button buttons clientX clientY fromElement offsetX offsetY pageX pageY screenX screenY toElement".split(" "),
            filter: function (event, original) {
                var eventDoc, doc, body,
                    button = original.button,
                    fromElement = original.fromElement;

                // Calculate pageX/Y if missing and clientX/Y available
                if (event.pageX == null && original.clientX != null) {
                    eventDoc = event.target.ownerDocument || document;
                    doc = eventDoc.documentElement;
                    body = eventDoc.body;

                    event.pageX = original.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc && doc.clientLeft || body && body.clientLeft || 0);
                    event.pageY = original.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc && doc.clientTop || body && body.clientTop || 0);
                }

                // Add relatedTarget, if necessary
                if (!event.relatedTarget && fromElement) {
                    event.relatedTarget = fromElement === event.target ? original.toElement : fromElement;
                }

                // Add which for click: 1 === left; 2 === middle; 3 === right
                // Note: button is not normalized, so don't use it
                if (!event.which && button !== undefined) {
                    event.which = (button & 1 ? 1 : (button & 2 ? 3 : (button & 4 ? 2 : 0)));
                }

                return event;
            }
        },

        fix: function (event) {
            if (event[jQuery.expando]) {
                return event;
            }

            // Create a writable copy of the event object and normalize some properties
            var i, prop,
                originalEvent = event,
                fixHook = jQuery.event.fixHooks[event.type] || {},
                copy = fixHook.props ? this.props.concat(fixHook.props) : this.props;

            event = jQuery.Event(originalEvent);

            for (i = copy.length; i;) {
                prop = copy[--i];
                event[prop] = originalEvent[prop];
            }

            // Fix target property, if necessary (#1925, IE 6/7/8 & Safari2)
            if (!event.target) {
                event.target = originalEvent.srcElement || document;
            }

            // Target should not be a text node (#504, Safari)
            if (event.target.nodeType === 3) {
                event.target = event.target.parentNode;
            }

            // For mouse/key events, metaKey==false if it's undefined (#3368, #11328; IE6/7/8)
            event.metaKey = !!event.metaKey;

            return fixHook.filter ? fixHook.filter(event, originalEvent) : event;
        },

        special: {
            load: {
                // Prevent triggered image.load events from bubbling to window.load
                noBubble: true
            },

            focus: {
                delegateType: "focusin"
            },
            blur: {
                delegateType: "focusout"
            },

            beforeunload: {
                setup: function (data, namespaces, eventHandle) {
                    // We only want to do this special case on windows
                    if (jQuery.isWindow(this)) {
                        this.onbeforeunload = eventHandle;
                    }
                },

                teardown: function (namespaces, eventHandle) {
                    if (this.onbeforeunload === eventHandle) {
                        this.onbeforeunload = null;
                    }
                }
            }
        },

        simulate: function (type, elem, event, bubble) {
            // Piggyback on a donor event to simulate a different one.
            // Fake originalEvent to avoid donor's stopPropagation, but if the
            // simulated event prevents default then we do the same on the donor.
            var e = jQuery.extend(
                new jQuery.Event(),
                event,
                {
                    type: type,
                    isSimulated: true,
                    originalEvent: {}
                }
            );
            if (bubble) {
                jQuery.event.trigger(e, null, elem);
            } else {
                jQuery.event.dispatch.call(elem, e);
            }
            if (e.isDefaultPrevented()) {
                event.preventDefault();
            }
        }
    };

    // Some plugins are using, but it's undocumented/deprecated and will be removed.
    // The 1.7 special event interface should provide all the hooks needed now.
    jQuery.event.handle = jQuery.event.dispatch;

    jQuery.removeEvent = document.removeEventListener ?
        function (elem, type, handle) {
            if (elem.removeEventListener) {
                elem.removeEventListener(type, handle, false);
            }
        } :
        function (elem, type, handle) {
            var name = "on" + type;

            if (elem.detachEvent) {

                // #8545, #7054, preventing memory leaks for custom events in IE6-8
                // detachEvent needed property on element, by name of that event, to properly expose it to GC
                if (typeof elem[name] === "undefined") {
                    elem[name] = null;
                }

                elem.detachEvent(name, handle);
            }
        };

    jQuery.Event = function (src, props) {
        // Allow instantiation without the 'new' keyword
        if (!(this instanceof jQuery.Event)) {
            return new jQuery.Event(src, props);
        }

        // Event object
        if (src && src.type) {
            this.originalEvent = src;
            this.type = src.type;

            // Events bubbling up the document may have been marked as prevented
            // by a handler lower down the tree; reflect the correct value.
            this.isDefaultPrevented = (src.defaultPrevented || src.returnValue === false ||
                src.getPreventDefault && src.getPreventDefault()) ? returnTrue : returnFalse;

            // Event type
        } else {
            this.type = src;
        }

        // Put explicitly provided properties onto the event object
        if (props) {
            jQuery.extend(this, props);
        }

        // Create a timestamp if incoming event doesn't have one
        this.timeStamp = src && src.timeStamp || jQuery.now();

        // Mark it as fixed
        this[jQuery.expando] = true;
    };

    function returnFalse() {
        return false;
    }
    function returnTrue() {
        return true;
    }

    // jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
    // http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
    jQuery.Event.prototype = {
        preventDefault: function () {
            this.isDefaultPrevented = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }

            // if preventDefault exists run it on the original event
            if (e.preventDefault) {
                e.preventDefault();

                // otherwise set the returnValue property of the original event to false (IE)
            } else {
                e.returnValue = false;
            }
        },
        stopPropagation: function () {
            this.isPropagationStopped = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }
            // if stopPropagation exists run it on the original event
            if (e.stopPropagation) {
                e.stopPropagation();
            }
            // otherwise set the cancelBubble property of the original event to true (IE)
            e.cancelBubble = true;
        },
        stopImmediatePropagation: function () {
            this.isImmediatePropagationStopped = returnTrue;
            this.stopPropagation();
        },
        isDefaultPrevented: returnFalse,
        isPropagationStopped: returnFalse,
        isImmediatePropagationStopped: returnFalse
    };

    // Create mouseenter/leave events using mouseover/out and event-time checks
    jQuery.each({
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    }, function (orig, fix) {
        jQuery.event.special[orig] = {
            delegateType: fix,
            bindType: fix,

            handle: function (event) {
                var ret,
                    target = this,
                    related = event.relatedTarget,
                    handleObj = event.handleObj,
                    selector = handleObj.selector;

                // For mousenter/leave call the handler if related is outside the target.
                // NB: No relatedTarget if the mouse left/entered the browser window
                if (!related || (related !== target && !jQuery.contains(target, related))) {
                    event.type = handleObj.origType;
                    ret = handleObj.handler.apply(this, arguments);
                    event.type = fix;
                }
                return ret;
            }
        };
    });

    // IE submit delegation
    if (!jQuery.support.submitBubbles) {

        jQuery.event.special.submit = {
            setup: function () {
                // Only need this for delegated form submit events
                if (jQuery.nodeName(this, "form")) {
                    return false;
                }

                // Lazy-add a submit handler when a descendant form may potentially be submitted
                jQuery.event.add(this, "click._submit keypress._submit", function (e) {
                    // Node name check avoids a VML-related crash in IE (#9807)
                    var elem = e.target,
                        form = jQuery.nodeName(elem, "input") || jQuery.nodeName(elem, "button") ? elem.form : undefined;
                    if (form && !jQuery._data(form, "_submit_attached")) {
                        jQuery.event.add(form, "submit._submit", function (event) {
                            event._submit_bubble = true;
                        });
                        jQuery._data(form, "_submit_attached", true);
                    }
                });
                // return undefined since we don't need an event listener
            },

            postDispatch: function (event) {
                // If form was submitted by the user, bubble the event up the tree
                if (event._submit_bubble) {
                    delete event._submit_bubble;
                    if (this.parentNode && !event.isTrigger) {
                        jQuery.event.simulate("submit", this.parentNode, event, true);
                    }
                }
            },

            teardown: function () {
                // Only need this for delegated form submit events
                if (jQuery.nodeName(this, "form")) {
                    return false;
                }

                // Remove delegated handlers; cleanData eventually reaps submit handlers attached above
                jQuery.event.remove(this, "._submit");
            }
        };
    }

    // IE change delegation and checkbox/radio fix
    if (!jQuery.support.changeBubbles) {

        jQuery.event.special.change = {

            setup: function () {

                if (rformElems.test(this.nodeName)) {
                    // IE doesn't fire change on a check/radio until blur; trigger it on click
                    // after a propertychange. Eat the blur-change in special.change.handle.
                    // This still fires onchange a second time for check/radio after blur.
                    if (this.type === "checkbox" || this.type === "radio") {
                        jQuery.event.add(this, "propertychange._change", function (event) {
                            if (event.originalEvent.propertyName === "checked") {
                                this._just_changed = true;
                            }
                        });
                        jQuery.event.add(this, "click._change", function (event) {
                            if (this._just_changed && !event.isTrigger) {
                                this._just_changed = false;
                            }
                            // Allow triggered, simulated change events (#11500)
                            jQuery.event.simulate("change", this, event, true);
                        });
                    }
                    return false;
                }
                // Delegated event; lazy-add a change handler on descendant inputs
                jQuery.event.add(this, "beforeactivate._change", function (e) {
                    var elem = e.target;

                    if (rformElems.test(elem.nodeName) && !jQuery._data(elem, "_change_attached")) {
                        jQuery.event.add(elem, "change._change", function (event) {
                            if (this.parentNode && !event.isSimulated && !event.isTrigger) {
                                jQuery.event.simulate("change", this.parentNode, event, true);
                            }
                        });
                        jQuery._data(elem, "_change_attached", true);
                    }
                });
            },

            handle: function (event) {
                var elem = event.target;

                // Swallow native change events from checkbox/radio, we already triggered them above
                if (this !== elem || event.isSimulated || event.isTrigger || (elem.type !== "radio" && elem.type !== "checkbox")) {
                    return event.handleObj.handler.apply(this, arguments);
                }
            },

            teardown: function () {
                jQuery.event.remove(this, "._change");

                return !rformElems.test(this.nodeName);
            }
        };
    }

    // Create "bubbling" focus and blur events
    if (!jQuery.support.focusinBubbles) {
        jQuery.each({ focus: "focusin", blur: "focusout" }, function (orig, fix) {

            // Attach a single capturing handler while someone wants focusin/focusout
            var attaches = 0,
                handler = function (event) {
                    jQuery.event.simulate(fix, event.target, jQuery.event.fix(event), true);
                };

            jQuery.event.special[fix] = {
                setup: function () {
                    if (attaches++ === 0) {
                        document.addEventListener(orig, handler, true);
                    }
                },
                teardown: function () {
                    if (--attaches === 0) {
                        document.removeEventListener(orig, handler, true);
                    }
                }
            };
        });
    }

    jQuery.fn.extend({

        on: function (types, selector, data, fn, /*INTERNAL*/ one) {
            var origFn, type;

            // Types can be a map of types/handlers
            if (typeof types === "object") {
                // ( types-Object, selector, data )
                if (typeof selector !== "string") { // && selector != null
                    // ( types-Object, data )
                    data = data || selector;
                    selector = undefined;
                }
                for (type in types) {
                    this.on(type, selector, data, types[type], one);
                }
                return this;
            }

            if (data == null && fn == null) {
                // ( types, fn )
                fn = selector;
                data = selector = undefined;
            } else if (fn == null) {
                if (typeof selector === "string") {
                    // ( types, selector, fn )
                    fn = data;
                    data = undefined;
                } else {
                    // ( types, data, fn )
                    fn = data;
                    data = selector;
                    selector = undefined;
                }
            }
            if (fn === false) {
                fn = returnFalse;
            } else if (!fn) {
                return this;
            }

            if (one === 1) {
                origFn = fn;
                fn = function (event) {
                    // Can use an empty set, since event contains the info
                    jQuery().off(event);
                    return origFn.apply(this, arguments);
                };
                // Use same guid so caller can remove using origFn
                fn.guid = origFn.guid || (origFn.guid = jQuery.guid++);
            }
            return this.each(function () {
                jQuery.event.add(this, types, fn, data, selector);
            });
        },
        one: function (types, selector, data, fn) {
            return this.on(types, selector, data, fn, 1);
        },
        off: function (types, selector, fn) {
            var handleObj, type;
            if (types && types.preventDefault && types.handleObj) {
                // ( event )  dispatched jQuery.Event
                handleObj = types.handleObj;
                jQuery(types.delegateTarget).off(
                    handleObj.namespace ? handleObj.origType + "." + handleObj.namespace : handleObj.origType,
                    handleObj.selector,
                    handleObj.handler
                );
                return this;
            }
            if (typeof types === "object") {
                // ( types-object [, selector] )
                for (type in types) {
                    this.off(type, selector, types[type]);
                }
                return this;
            }
            if (selector === false || typeof selector === "function") {
                // ( types [, fn] )
                fn = selector;
                selector = undefined;
            }
            if (fn === false) {
                fn = returnFalse;
            }
            return this.each(function () {
                jQuery.event.remove(this, types, fn, selector);
            });
        },

        bind: function (types, data, fn) {
            return this.on(types, null, data, fn);
        },
        unbind: function (types, fn) {
            return this.off(types, null, fn);
        },

        live: function (types, data, fn) {
            jQuery(this.context).on(types, this.selector, data, fn);
            return this;
        },
        die: function (types, fn) {
            jQuery(this.context).off(types, this.selector || "**", fn);
            return this;
        },

        delegate: function (selector, types, data, fn) {
            return this.on(types, selector, data, fn);
        },
        undelegate: function (selector, types, fn) {
            // ( namespace ) or ( selector, types [, fn] )
            return arguments.length === 1 ? this.off(selector, "**") : this.off(types, selector || "**", fn);
        },

        trigger: function (type, data) {
            return this.each(function () {
                jQuery.event.trigger(type, data, this);
            });
        },
        triggerHandler: function (type, data) {
            if (this[0]) {
                return jQuery.event.trigger(type, data, this[0], true);
            }
        },

        toggle: function (fn) {
            // Save reference to arguments for access in closure
            var args = arguments,
                guid = fn.guid || jQuery.guid++,
                i = 0,
                toggler = function (event) {
                    // Figure out which function to execute
                    var lastToggle = (jQuery._data(this, "lastToggle" + fn.guid) || 0) % i;
                    jQuery._data(this, "lastToggle" + fn.guid, lastToggle + 1);

                    // Make sure that clicks stop
                    event.preventDefault();

                    // and execute the function
                    return args[lastToggle].apply(this, arguments) || false;
                };

            // link all the functions, so any of them can unbind this click handler
            toggler.guid = guid;
            while (i < args.length) {
                args[i++].guid = guid;
            }

            return this.click(toggler);
        },

        hover: function (fnOver, fnOut) {
            return this.mouseenter(fnOver).mouseleave(fnOut || fnOver);
        }
    });

    jQuery.each(("blur focus focusin focusout load resize scroll unload click dblclick " +
        "mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " +
        "change select submit keydown keypress keyup error contextmenu").split(" "), function (i, name) {

            // Handle event binding
            jQuery.fn[name] = function (data, fn) {
                if (fn == null) {
                    fn = data;
                    data = null;
                }

                return arguments.length > 0 ?
                    this.on(name, null, data, fn) :
                    this.trigger(name);
            };

            if (rkeyEvent.test(name)) {
                jQuery.event.fixHooks[name] = jQuery.event.keyHooks;
            }

            if (rmouseEvent.test(name)) {
                jQuery.event.fixHooks[name] = jQuery.event.mouseHooks;
            }
        });
    /*!
     * Sizzle CSS Selector Engine
     * Copyright 2012 jQuery Foundation and other contributors
     * Released under the MIT license
     * http://sizzlejs.com/
     */
    (function (window, undefined) {

        var cachedruns,
            assertGetIdNotName,
            Expr,
            getText,
            isXML,
            contains,
            compile,
            sortOrder,
            hasDuplicate,
            outermostContext,

            baseHasDuplicate = true,
            strundefined = "undefined",

            expando = ("sizcache" + Math.random()).replace(".", ""),

            Token = String,
            document = window.document,
            docElem = document.documentElement,
            dirruns = 0,
            done = 0,
            pop = [].pop,
            push = [].push,
            slice = [].slice,
            // Use a stripped-down indexOf if a native one is unavailable
            indexOf = [].indexOf || function (elem) {
                var i = 0,
                    len = this.length;
                for (; i < len; i++) {
                    if (this[i] === elem) {
                        return i;
                    }
                }
                return -1;
            },

            // Augment a function for special use by Sizzle
            markFunction = function (fn, value) {
                fn[expando] = value == null || value;
                return fn;
            },

            createCache = function () {
                var cache = {},
                    keys = [];

                return markFunction(function (key, value) {
                    // Only keep the most recent entries
                    if (keys.push(key) > Expr.cacheLength) {
                        delete cache[keys.shift()];
                    }

                    // Retrieve with (key + " ") to avoid collision with native Object.prototype properties (see Issue #157)
                    return (cache[key + " "] = value);
                }, cache);
            },

            classCache = createCache(),
            tokenCache = createCache(),
            compilerCache = createCache(),

            // Regex

            // Whitespace characters http://www.w3.org/TR/css3-selectors/#whitespace
            whitespace = "[\\x20\\t\\r\\n\\f]",
            // http://www.w3.org/TR/css3-syntax/#characters
            characterEncoding = "(?:\\\\.|[-\\w]|[^\\x00-\\xa0])+",

            // Loosely modeled on CSS identifier characters
            // An unquoted value should be a CSS identifier (http://www.w3.org/TR/css3-selectors/#attribute-selectors)
            // Proper syntax: http://www.w3.org/TR/CSS21/syndata.html#value-def-identifier
            identifier = characterEncoding.replace("w", "w#"),

            // Acceptable operators http://www.w3.org/TR/selectors/#attribute-selectors
            operators = "([*^$|!~]?=)",
            attributes = "\\[" + whitespace + "*(" + characterEncoding + ")" + whitespace +
                "*(?:" + operators + whitespace + "*(?:(['\"])((?:\\\\.|[^\\\\])*?)\\3|(" + identifier + ")|)|)" + whitespace + "*\\]",

            // Prefer arguments not in parens/brackets,
            //   then attribute selectors and non-pseudos (denoted by :),
            //   then anything else
            // These preferences are here to reduce the number of selectors
            //   needing tokenize in the PSEUDO preFilter
            pseudos = ":(" + characterEncoding + ")(?:\\((?:(['\"])((?:\\\\.|[^\\\\])*?)\\2|([^()[\\]]*|(?:(?:" + attributes + ")|[^:]|\\\\.)*|.*))\\)|)",

            // For matchExpr.POS and matchExpr.needsContext
            pos = ":(even|odd|eq|gt|lt|nth|first|last)(?:\\(" + whitespace +
                "*((?:-\\d)?\\d*)" + whitespace + "*\\)|)(?=[^-]|$)",

            // Leading and non-escaped trailing whitespace, capturing some non-whitespace characters preceding the latter
            rtrim = new RegExp("^" + whitespace + "+|((?:^|[^\\\\])(?:\\\\.)*)" + whitespace + "+$", "g"),

            rcomma = new RegExp("^" + whitespace + "*," + whitespace + "*"),
            rcombinators = new RegExp("^" + whitespace + "*([\\x20\\t\\r\\n\\f>+~])" + whitespace + "*"),
            rpseudo = new RegExp(pseudos),

            // Easily-parseable/retrievable ID or TAG or CLASS selectors
            rquickExpr = /^(?:#([\w\-]+)|(\w+)|\.([\w\-]+))$/,

            rnot = /^:not/,
            rsibling = /[\x20\t\r\n\f]*[+~]/,
            rendsWithNot = /:not\($/,

            rheader = /h\d/i,
            rinputs = /input|select|textarea|button/i,

            rbackslash = /\\(?!\\)/g,

            matchExpr = {
                "ID": new RegExp("^#(" + characterEncoding + ")"),
                "CLASS": new RegExp("^\\.(" + characterEncoding + ")"),
                "NAME": new RegExp("^\\[name=['\"]?(" + characterEncoding + ")['\"]?\\]"),
                "TAG": new RegExp("^(" + characterEncoding.replace("w", "w*") + ")"),
                "ATTR": new RegExp("^" + attributes),
                "PSEUDO": new RegExp("^" + pseudos),
                "POS": new RegExp(pos, "i"),
                "CHILD": new RegExp("^:(only|nth|first|last)-child(?:\\(" + whitespace +
                    "*(even|odd|(([+-]|)(\\d*)n|)" + whitespace + "*(?:([+-]|)" + whitespace +
                    "*(\\d+)|))" + whitespace + "*\\)|)", "i"),
                // For use in libraries implementing .is()
                "needsContext": new RegExp("^" + whitespace + "*[>+~]|" + pos, "i")
            },

            // Support

            // Used for testing something on an element
            assert = function (fn) {
                var div = document.createElement("div");

                try {
                    return fn(div);
                } catch (e) {
                    return false;
                } finally {
                    // release memory in IE
                    div = null;
                }
            },

            // Check if getElementsByTagName("*") returns only elements
            assertTagNameNoComments = assert(function (div) {
                div.appendChild(document.createComment(""));
                return !div.getElementsByTagName("*").length;
            }),

            // Check if getAttribute returns normalized href attributes
            assertHrefNotNormalized = assert(function (div) {
                div.innerHTML = "<a href='#'></a>";
                return div.firstChild && typeof div.firstChild.getAttribute !== strundefined &&
                    div.firstChild.getAttribute("href") === "#";
            }),

            // Check if attributes should be retrieved by attribute nodes
            assertAttributes = assert(function (div) {
                div.innerHTML = "<select></select>";
                var type = typeof div.lastChild.getAttribute("multiple");
                // IE8 returns a string for some attributes even when not present
                return type !== "boolean" && type !== "string";
            }),

            // Check if getElementsByClassName can be trusted
            assertUsableClassName = assert(function (div) {
                // Opera can't find a second classname (in 9.6)
                div.innerHTML = "<div class='hidden e'></div><div class='hidden'></div>";
                if (!div.getElementsByClassName || !div.getElementsByClassName("e").length) {
                    return false;
                }

                // Safari 3.2 caches class attributes and doesn't catch changes
                div.lastChild.className = "e";
                return div.getElementsByClassName("e").length === 2;
            }),

            // Check if getElementById returns elements by name
            // Check if getElementsByName privileges form controls or returns elements by ID
            assertUsableName = assert(function (div) {
                // Inject content
                div.id = expando + 0;
                div.innerHTML = "<a name='" + expando + "'></a><div name='" + expando + "'></div>";
                docElem.insertBefore(div, docElem.firstChild);

                // Test
                var pass = document.getElementsByName &&
                    // buggy browsers will return fewer than the correct 2
                    document.getElementsByName(expando).length === 2 +
                    // buggy browsers will return more than the correct 0
                    document.getElementsByName(expando + 0).length;
                assertGetIdNotName = !document.getElementById(expando);

                // Cleanup
                docElem.removeChild(div);

                return pass;
            });

        // If slice is not available, provide a backup
        try {
            slice.call(docElem.childNodes, 0)[0].nodeType;
        } catch (e) {
            slice = function (i) {
                var elem,
                    results = [];
                for (; (elem = this[i]) ; i++) {
                    results.push(elem);
                }
                return results;
            };
        }

        function Sizzle(selector, context, results, seed) {
            results = results || [];
            context = context || document;
            var match, elem, xml, m,
                nodeType = context.nodeType;

            if (!selector || typeof selector !== "string") {
                return results;
            }

            if (nodeType !== 1 && nodeType !== 9) {
                return [];
            }

            xml = isXML(context);

            if (!xml && !seed) {
                if ((match = rquickExpr.exec(selector))) {
                    // Speed-up: Sizzle("#ID")
                    if ((m = match[1])) {
                        if (nodeType === 9) {
                            elem = context.getElementById(m);
                            // Check parentNode to catch when Blackberry 4.6 returns
                            // nodes that are no longer in the document #6963
                            if (elem && elem.parentNode) {
                                // Handle the case where IE, Opera, and Webkit return items
                                // by name instead of ID
                                if (elem.id === m) {
                                    results.push(elem);
                                    return results;
                                }
                            } else {
                                return results;
                            }
                        } else {
                            // Context is not a document
                            if (context.ownerDocument && (elem = context.ownerDocument.getElementById(m)) &&
                                contains(context, elem) && elem.id === m) {
                                results.push(elem);
                                return results;
                            }
                        }

                        // Speed-up: Sizzle("TAG")
                    } else if (match[2]) {
                        push.apply(results, slice.call(context.getElementsByTagName(selector), 0));
                        return results;

                        // Speed-up: Sizzle(".CLASS")
                    } else if ((m = match[3]) && assertUsableClassName && context.getElementsByClassName) {
                        push.apply(results, slice.call(context.getElementsByClassName(m), 0));
                        return results;
                    }
                }
            }

            // All others
            return select(selector.replace(rtrim, "$1"), context, results, seed, xml);
        }

        Sizzle.matches = function (expr, elements) {
            return Sizzle(expr, null, null, elements);
        };

        Sizzle.matchesSelector = function (elem, expr) {
            return Sizzle(expr, null, null, [elem]).length > 0;
        };

        // Returns a function to use in pseudos for input types
        function createInputPseudo(type) {
            return function (elem) {
                var name = elem.nodeName.toLowerCase();
                return name === "input" && elem.type === type;
            };
        }

        // Returns a function to use in pseudos for buttons
        function createButtonPseudo(type) {
            return function (elem) {
                var name = elem.nodeName.toLowerCase();
                return (name === "input" || name === "button") && elem.type === type;
            };
        }

        // Returns a function to use in pseudos for positionals
        function createPositionalPseudo(fn) {
            return markFunction(function (argument) {
                argument = +argument;
                return markFunction(function (seed, matches) {
                    var j,
                        matchIndexes = fn([], seed.length, argument),
                        i = matchIndexes.length;

                    // Match elements found at the specified indexes
                    while (i--) {
                        if (seed[(j = matchIndexes[i])]) {
                            seed[j] = !(matches[j] = seed[j]);
                        }
                    }
                });
            });
        }

        /**
         * Utility function for retrieving the text value of an array of DOM nodes
         * @param {Array|Element} elem
         */
        getText = Sizzle.getText = function (elem) {
            var node,
                ret = "",
                i = 0,
                nodeType = elem.nodeType;

            if (nodeType) {
                if (nodeType === 1 || nodeType === 9 || nodeType === 11) {
                    // Use textContent for elements
                    // innerText usage removed for consistency of new lines (see #11153)
                    if (typeof elem.textContent === "string") {
                        return elem.textContent;
                    } else {
                        // Traverse its children
                        for (elem = elem.firstChild; elem; elem = elem.nextSibling) {
                            ret += getText(elem);
                        }
                    }
                } else if (nodeType === 3 || nodeType === 4) {
                    return elem.nodeValue;
                }
                // Do not include comment or processing instruction nodes
            } else {

                // If no nodeType, this is expected to be an array
                for (; (node = elem[i]) ; i++) {
                    // Do not traverse comment nodes
                    ret += getText(node);
                }
            }
            return ret;
        };

        isXML = Sizzle.isXML = function (elem) {
            // documentElement is verified for cases where it doesn't yet exist
            // (such as loading iframes in IE - #4833)
            var documentElement = elem && (elem.ownerDocument || elem).documentElement;
            return documentElement ? documentElement.nodeName !== "HTML" : false;
        };

        // Element contains another
        contains = Sizzle.contains = docElem.contains ?
            function (a, b) {
                var adown = a.nodeType === 9 ? a.documentElement : a,
                    bup = b && b.parentNode;
                return a === bup || !!(bup && bup.nodeType === 1 && adown.contains && adown.contains(bup));
            } :
            docElem.compareDocumentPosition ?
            function (a, b) {
                return b && !!(a.compareDocumentPosition(b) & 16);
            } :
            function (a, b) {
                while ((b = b.parentNode)) {
                    if (b === a) {
                        return true;
                    }
                }
                return false;
            };

        Sizzle.attr = function (elem, name) {
            var val,
                xml = isXML(elem);

            if (!xml) {
                name = name.toLowerCase();
            }
            if ((val = Expr.attrHandle[name])) {
                return val(elem);
            }
            if (xml || assertAttributes) {
                return elem.getAttribute(name);
            }
            val = elem.getAttributeNode(name);
            return val ?
                typeof elem[name] === "boolean" ?
                    elem[name] ? name : null :
                    val.specified ? val.value : null :
                null;
        };

        Expr = Sizzle.selectors = {

            // Can be adjusted by the user
            cacheLength: 50,

            createPseudo: markFunction,

            match: matchExpr,

            // IE6/7 return a modified href
            attrHandle: assertHrefNotNormalized ?
		{} :
		{
		    "href": function (elem) {
		        return elem.getAttribute("href", 2);
		    },
		    "type": function (elem) {
		        return elem.getAttribute("type");
		    }
		},

            find: {
                "ID": assertGetIdNotName ?
                    function (id, context, xml) {
                        if (typeof context.getElementById !== strundefined && !xml) {
                            var m = context.getElementById(id);
                            // Check parentNode to catch when Blackberry 4.6 returns
                            // nodes that are no longer in the document #6963
                            return m && m.parentNode ? [m] : [];
                        }
                    } :
                    function (id, context, xml) {
                        if (typeof context.getElementById !== strundefined && !xml) {
                            var m = context.getElementById(id);

                            return m ?
                                m.id === id || typeof m.getAttributeNode !== strundefined && m.getAttributeNode("id").value === id ?
                                    [m] :
                                undefined :
                                [];
                        }
                    },

                "TAG": assertTagNameNoComments ?
                    function (tag, context) {
                        if (typeof context.getElementsByTagName !== strundefined) {
                            return context.getElementsByTagName(tag);
                        }
                    } :
                    function (tag, context) {
                        var results = context.getElementsByTagName(tag);

                        // Filter out possible comments
                        if (tag === "*") {
                            var elem,
                                tmp = [],
                                i = 0;

                            for (; (elem = results[i]) ; i++) {
                                if (elem.nodeType === 1) {
                                    tmp.push(elem);
                                }
                            }

                            return tmp;
                        }
                        return results;
                    },

                "NAME": assertUsableName && function (tag, context) {
                    if (typeof context.getElementsByName !== strundefined) {
                        return context.getElementsByName(name);
                    }
                },

                "CLASS": assertUsableClassName && function (className, context, xml) {
                    if (typeof context.getElementsByClassName !== strundefined && !xml) {
                        return context.getElementsByClassName(className);
                    }
                }
            },

            relative: {
                ">": { dir: "parentNode", first: true },
                " ": { dir: "parentNode" },
                "+": { dir: "previousSibling", first: true },
                "~": { dir: "previousSibling" }
            },

            preFilter: {
                "ATTR": function (match) {
                    match[1] = match[1].replace(rbackslash, "");

                    // Move the given value to match[3] whether quoted or unquoted
                    match[3] = (match[4] || match[5] || "").replace(rbackslash, "");

                    if (match[2] === "~=") {
                        match[3] = " " + match[3] + " ";
                    }

                    return match.slice(0, 4);
                },

                "CHILD": function (match) {
                    /* matches from matchExpr["CHILD"]
                        1 type (only|nth|...)
                        2 argument (even|odd|\d*|\d*n([+-]\d+)?|...)
                        3 xn-component of xn+y argument ([+-]?\d*n|)
                        4 sign of xn-component
                        5 x of xn-component
                        6 sign of y-component
                        7 y of y-component
                    */
                    match[1] = match[1].toLowerCase();

                    if (match[1] === "nth") {
                        // nth-child requires argument
                        if (!match[2]) {
                            Sizzle.error(match[0]);
                        }

                        // numeric x and y parameters for Expr.filter.CHILD
                        // remember that false/true cast respectively to 0/1
                        match[3] = +(match[3] ? match[4] + (match[5] || 1) : 2 * (match[2] === "even" || match[2] === "odd"));
                        match[4] = +((match[6] + match[7]) || match[2] === "odd");

                        // other types prohibit arguments
                    } else if (match[2]) {
                        Sizzle.error(match[0]);
                    }

                    return match;
                },

                "PSEUDO": function (match) {
                    var unquoted, excess;
                    if (matchExpr["CHILD"].test(match[0])) {
                        return null;
                    }

                    if (match[3]) {
                        match[2] = match[3];
                    } else if ((unquoted = match[4])) {
                        // Only check arguments that contain a pseudo
                        if (rpseudo.test(unquoted) &&
                            // Get excess from tokenize (recursively)
                            (excess = tokenize(unquoted, true)) &&
                            // advance to the next closing parenthesis
                            (excess = unquoted.indexOf(")", unquoted.length - excess) - unquoted.length)) {

                            // excess is a negative index
                            unquoted = unquoted.slice(0, excess);
                            match[0] = match[0].slice(0, excess);
                        }
                        match[2] = unquoted;
                    }

                    // Return only captures needed by the pseudo filter method (type and argument)
                    return match.slice(0, 3);
                }
            },

            filter: {
                "ID": assertGetIdNotName ?
                    function (id) {
                        id = id.replace(rbackslash, "");
                        return function (elem) {
                            return elem.getAttribute("id") === id;
                        };
                    } :
                    function (id) {
                        id = id.replace(rbackslash, "");
                        return function (elem) {
                            var node = typeof elem.getAttributeNode !== strundefined && elem.getAttributeNode("id");
                            return node && node.value === id;
                        };
                    },

                "TAG": function (nodeName) {
                    if (nodeName === "*") {
                        return function () { return true; };
                    }
                    nodeName = nodeName.replace(rbackslash, "").toLowerCase();

                    return function (elem) {
                        return elem.nodeName && elem.nodeName.toLowerCase() === nodeName;
                    };
                },

                "CLASS": function (className) {
                    var pattern = classCache[expando][className + " "];

                    return pattern ||
                        (pattern = new RegExp("(^|" + whitespace + ")" + className + "(" + whitespace + "|$)")) &&
                        classCache(className, function (elem) {
                            return pattern.test(elem.className || (typeof elem.getAttribute !== strundefined && elem.getAttribute("class")) || "");
                        });
                },

                "ATTR": function (name, operator, check) {
                    return function (elem, context) {
                        var result = Sizzle.attr(elem, name);

                        if (result == null) {
                            return operator === "!=";
                        }
                        if (!operator) {
                            return true;
                        }

                        result += "";

                        return operator === "=" ? result === check :
                            operator === "!=" ? result !== check :
                            operator === "^=" ? check && result.indexOf(check) === 0 :
                            operator === "*=" ? check && result.indexOf(check) > -1 :
                            operator === "$=" ? check && result.substr(result.length - check.length) === check :
                            operator === "~=" ? (" " + result + " ").indexOf(check) > -1 :
                            operator === "|=" ? result === check || result.substr(0, check.length + 1) === check + "-" :
                            false;
                    };
                },

                "CHILD": function (type, argument, first, last) {

                    if (type === "nth") {
                        return function (elem) {
                            var node, diff,
                                parent = elem.parentNode;

                            if (first === 1 && last === 0) {
                                return true;
                            }

                            if (parent) {
                                diff = 0;
                                for (node = parent.firstChild; node; node = node.nextSibling) {
                                    if (node.nodeType === 1) {
                                        diff++;
                                        if (elem === node) {
                                            break;
                                        }
                                    }
                                }
                            }

                            // Incorporate the offset (or cast to NaN), then check against cycle size
                            diff -= last;
                            return diff === first || (diff % first === 0 && diff / first >= 0);
                        };
                    }

                    return function (elem) {
                        var node = elem;

                        switch (type) {
                            case "only":
                            case "first":
                                while ((node = node.previousSibling)) {
                                    if (node.nodeType === 1) {
                                        return false;
                                    }
                                }

                                if (type === "first") {
                                    return true;
                                }

                                node = elem;

                                /* falls through */
                            case "last":
                                while ((node = node.nextSibling)) {
                                    if (node.nodeType === 1) {
                                        return false;
                                    }
                                }

                                return true;
                        }
                    };
                },

                "PSEUDO": function (pseudo, argument) {
                    // pseudo-class names are case-insensitive
                    // http://www.w3.org/TR/selectors/#pseudo-classes
                    // Prioritize by case sensitivity in case custom pseudos are added with uppercase letters
                    // Remember that setFilters inherits from pseudos
                    var args,
                        fn = Expr.pseudos[pseudo] || Expr.setFilters[pseudo.toLowerCase()] ||
                            Sizzle.error("unsupported pseudo: " + pseudo);

                    // The user may use createPseudo to indicate that
                    // arguments are needed to create the filter function
                    // just as Sizzle does
                    if (fn[expando]) {
                        return fn(argument);
                    }

                    // But maintain support for old signatures
                    if (fn.length > 1) {
                        args = [pseudo, pseudo, "", argument];
                        return Expr.setFilters.hasOwnProperty(pseudo.toLowerCase()) ?
                            markFunction(function (seed, matches) {
                                var idx,
                                    matched = fn(seed, argument),
                                    i = matched.length;
                                while (i--) {
                                    idx = indexOf.call(seed, matched[i]);
                                    seed[idx] = !(matches[idx] = matched[i]);
                                }
                            }) :
                            function (elem) {
                                return fn(elem, 0, args);
                            };
                    }

                    return fn;
                }
            },

            pseudos: {
                "not": markFunction(function (selector) {
                    // Trim the selector passed to compile
                    // to avoid treating leading and trailing
                    // spaces as combinators
                    var input = [],
                        results = [],
                        matcher = compile(selector.replace(rtrim, "$1"));

                    return matcher[expando] ?
                        markFunction(function (seed, matches, context, xml) {
                            var elem,
                                unmatched = matcher(seed, null, xml, []),
                                i = seed.length;

                            // Match elements unmatched by `matcher`
                            while (i--) {
                                if ((elem = unmatched[i])) {
                                    seed[i] = !(matches[i] = elem);
                                }
                            }
                        }) :
                        function (elem, context, xml) {
                            input[0] = elem;
                            matcher(input, null, xml, results);
                            return !results.pop();
                        };
                }),

                "has": markFunction(function (selector) {
                    return function (elem) {
                        return Sizzle(selector, elem).length > 0;
                    };
                }),

                "contains": markFunction(function (text) {
                    return function (elem) {
                        return (elem.textContent || elem.innerText || getText(elem)).indexOf(text) > -1;
                    };
                }),

                "enabled": function (elem) {
                    return elem.disabled === false;
                },

                "disabled": function (elem) {
                    return elem.disabled === true;
                },

                "checked": function (elem) {
                    // In CSS3, :checked should return both checked and selected elements
                    // http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
                    var nodeName = elem.nodeName.toLowerCase();
                    return (nodeName === "input" && !!elem.checked) || (nodeName === "option" && !!elem.selected);
                },

                "selected": function (elem) {
                    // Accessing this property makes selected-by-default
                    // options in Safari work properly
                    if (elem.parentNode) {
                        elem.parentNode.selectedIndex;
                    }

                    return elem.selected === true;
                },

                "parent": function (elem) {
                    return !Expr.pseudos["empty"](elem);
                },

                "empty": function (elem) {
                    // http://www.w3.org/TR/selectors/#empty-pseudo
                    // :empty is only affected by element nodes and content nodes(including text(3), cdata(4)),
                    //   not comment, processing instructions, or others
                    // Thanks to Diego Perini for the nodeName shortcut
                    //   Greater than "@" means alpha characters (specifically not starting with "#" or "?")
                    var nodeType;
                    elem = elem.firstChild;
                    while (elem) {
                        if (elem.nodeName > "@" || (nodeType = elem.nodeType) === 3 || nodeType === 4) {
                            return false;
                        }
                        elem = elem.nextSibling;
                    }
                    return true;
                },

                "header": function (elem) {
                    return rheader.test(elem.nodeName);
                },

                "text": function (elem) {
                    var type, attr;
                    // IE6 and 7 will map elem.type to 'text' for new HTML5 types (search, etc)
                    // use getAttribute instead to test this case
                    return elem.nodeName.toLowerCase() === "input" &&
                        (type = elem.type) === "text" &&
                        ((attr = elem.getAttribute("type")) == null || attr.toLowerCase() === type);
                },

                // Input types
                "radio": createInputPseudo("radio"),
                "checkbox": createInputPseudo("checkbox"),
                "file": createInputPseudo("file"),
                "password": createInputPseudo("password"),
                "image": createInputPseudo("image"),

                "submit": createButtonPseudo("submit"),
                "reset": createButtonPseudo("reset"),

                "button": function (elem) {
                    var name = elem.nodeName.toLowerCase();
                    return name === "input" && elem.type === "button" || name === "button";
                },

                "input": function (elem) {
                    return rinputs.test(elem.nodeName);
                },

                "focus": function (elem) {
                    var doc = elem.ownerDocument;
                    return elem === doc.activeElement && (!doc.hasFocus || doc.hasFocus()) && !!(elem.type || elem.href || ~elem.tabIndex);
                },

                "active": function (elem) {
                    return elem === elem.ownerDocument.activeElement;
                },

                // Positional types
                "first": createPositionalPseudo(function () {
                    return [0];
                }),

                "last": createPositionalPseudo(function (matchIndexes, length) {
                    return [length - 1];
                }),

                "eq": createPositionalPseudo(function (matchIndexes, length, argument) {
                    return [argument < 0 ? argument + length : argument];
                }),

                "even": createPositionalPseudo(function (matchIndexes, length) {
                    for (var i = 0; i < length; i += 2) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "odd": createPositionalPseudo(function (matchIndexes, length) {
                    for (var i = 1; i < length; i += 2) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "lt": createPositionalPseudo(function (matchIndexes, length, argument) {
                    for (var i = argument < 0 ? argument + length : argument; --i >= 0;) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "gt": createPositionalPseudo(function (matchIndexes, length, argument) {
                    for (var i = argument < 0 ? argument + length : argument; ++i < length;) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                })
            }
        };

        function siblingCheck(a, b, ret) {
            if (a === b) {
                return ret;
            }

            var cur = a.nextSibling;

            while (cur) {
                if (cur === b) {
                    return -1;
                }

                cur = cur.nextSibling;
            }

            return 1;
        }

        sortOrder = docElem.compareDocumentPosition ?
            function (a, b) {
                if (a === b) {
                    hasDuplicate = true;
                    return 0;
                }

                return (!a.compareDocumentPosition || !b.compareDocumentPosition ?
                    a.compareDocumentPosition :
                    a.compareDocumentPosition(b) & 4
                ) ? -1 : 1;
            } :
            function (a, b) {
                // The nodes are identical, we can exit early
                if (a === b) {
                    hasDuplicate = true;
                    return 0;

                    // Fallback to using sourceIndex (in IE) if it's available on both nodes
                } else if (a.sourceIndex && b.sourceIndex) {
                    return a.sourceIndex - b.sourceIndex;
                }

                var al, bl,
                    ap = [],
                    bp = [],
                    aup = a.parentNode,
                    bup = b.parentNode,
                    cur = aup;

                // If the nodes are siblings (or identical) we can do a quick check
                if (aup === bup) {
                    return siblingCheck(a, b);

                    // If no parents were found then the nodes are disconnected
                } else if (!aup) {
                    return -1;

                } else if (!bup) {
                    return 1;
                }

                // Otherwise they're somewhere else in the tree so we need
                // to build up a full list of the parentNodes for comparison
                while (cur) {
                    ap.unshift(cur);
                    cur = cur.parentNode;
                }

                cur = bup;

                while (cur) {
                    bp.unshift(cur);
                    cur = cur.parentNode;
                }

                al = ap.length;
                bl = bp.length;

                // Start walking down the tree looking for a discrepancy
                for (var i = 0; i < al && i < bl; i++) {
                    if (ap[i] !== bp[i]) {
                        return siblingCheck(ap[i], bp[i]);
                    }
                }

                // We ended someplace up the tree so do a sibling check
                return i === al ?
                    siblingCheck(a, bp[i], -1) :
                    siblingCheck(ap[i], b, 1);
            };

        // Always assume the presence of duplicates if sort doesn't
        // pass them to our comparison function (as in Google Chrome).
        [0, 0].sort(sortOrder);
        baseHasDuplicate = !hasDuplicate;

        // Document sorting and removing duplicates
        Sizzle.uniqueSort = function (results) {
            var elem,
                duplicates = [],
                i = 1,
                j = 0;

            hasDuplicate = baseHasDuplicate;
            results.sort(sortOrder);

            if (hasDuplicate) {
                for (; (elem = results[i]) ; i++) {
                    if (elem === results[i - 1]) {
                        j = duplicates.push(i);
                    }
                }
                while (j--) {
                    results.splice(duplicates[j], 1);
                }
            }

            return results;
        };

        Sizzle.error = function (msg) {
            throw new Error("Syntax error, unrecognized expression: " + msg);
        };

        function tokenize(selector, parseOnly) {
            var matched, match, tokens, type,
                soFar, groups, preFilters,
                cached = tokenCache[expando][selector + " "];

            if (cached) {
                return parseOnly ? 0 : cached.slice(0);
            }

            soFar = selector;
            groups = [];
            preFilters = Expr.preFilter;

            while (soFar) {

                // Comma and first run
                if (!matched || (match = rcomma.exec(soFar))) {
                    if (match) {
                        // Don't consume trailing commas as valid
                        soFar = soFar.slice(match[0].length) || soFar;
                    }
                    groups.push(tokens = []);
                }

                matched = false;

                // Combinators
                if ((match = rcombinators.exec(soFar))) {
                    tokens.push(matched = new Token(match.shift()));
                    soFar = soFar.slice(matched.length);

                    // Cast descendant combinators to space
                    matched.type = match[0].replace(rtrim, " ");
                }

                // Filters
                for (type in Expr.filter) {
                    if ((match = matchExpr[type].exec(soFar)) && (!preFilters[type] ||
                        (match = preFilters[type](match)))) {

                        tokens.push(matched = new Token(match.shift()));
                        soFar = soFar.slice(matched.length);
                        matched.type = type;
                        matched.matches = match;
                    }
                }

                if (!matched) {
                    break;
                }
            }

            // Return the length of the invalid excess
            // if we're just parsing
            // Otherwise, throw an error or return tokens
            return parseOnly ?
                soFar.length :
                soFar ?
                    Sizzle.error(selector) :
                    // Cache the tokens
                    tokenCache(selector, groups).slice(0);
        }

        function addCombinator(matcher, combinator, base) {
            var dir = combinator.dir,
                checkNonElements = base && combinator.dir === "parentNode",
                doneName = done++;

            return combinator.first ?
                // Check against closest ancestor/preceding element
                function (elem, context, xml) {
                    while ((elem = elem[dir])) {
                        if (checkNonElements || elem.nodeType === 1) {
                            return matcher(elem, context, xml);
                        }
                    }
                } :

                // Check against all ancestor/preceding elements
                function (elem, context, xml) {
                    // We can't set arbitrary data on XML nodes, so they don't benefit from dir caching
                    if (!xml) {
                        var cache,
                            dirkey = dirruns + " " + doneName + " ",
                            cachedkey = dirkey + cachedruns;
                        while ((elem = elem[dir])) {
                            if (checkNonElements || elem.nodeType === 1) {
                                if ((cache = elem[expando]) === cachedkey) {
                                    return elem.sizset;
                                } else if (typeof cache === "string" && cache.indexOf(dirkey) === 0) {
                                    if (elem.sizset) {
                                        return elem;
                                    }
                                } else {
                                    elem[expando] = cachedkey;
                                    if (matcher(elem, context, xml)) {
                                        elem.sizset = true;
                                        return elem;
                                    }
                                    elem.sizset = false;
                                }
                            }
                        }
                    } else {
                        while ((elem = elem[dir])) {
                            if (checkNonElements || elem.nodeType === 1) {
                                if (matcher(elem, context, xml)) {
                                    return elem;
                                }
                            }
                        }
                    }
                };
        }

        function elementMatcher(matchers) {
            return matchers.length > 1 ?
                function (elem, context, xml) {
                    var i = matchers.length;
                    while (i--) {
                        if (!matchers[i](elem, context, xml)) {
                            return false;
                        }
                    }
                    return true;
                } :
                matchers[0];
        }

        function condense(unmatched, map, filter, context, xml) {
            var elem,
                newUnmatched = [],
                i = 0,
                len = unmatched.length,
                mapped = map != null;

            for (; i < len; i++) {
                if ((elem = unmatched[i])) {
                    if (!filter || filter(elem, context, xml)) {
                        newUnmatched.push(elem);
                        if (mapped) {
                            map.push(i);
                        }
                    }
                }
            }

            return newUnmatched;
        }

        function setMatcher(preFilter, selector, matcher, postFilter, postFinder, postSelector) {
            if (postFilter && !postFilter[expando]) {
                postFilter = setMatcher(postFilter);
            }
            if (postFinder && !postFinder[expando]) {
                postFinder = setMatcher(postFinder, postSelector);
            }
            return markFunction(function (seed, results, context, xml) {
                var temp, i, elem,
                    preMap = [],
                    postMap = [],
                    preexisting = results.length,

                    // Get initial elements from seed or context
                    elems = seed || multipleContexts(selector || "*", context.nodeType ? [context] : context, []),

                    // Prefilter to get matcher input, preserving a map for seed-results synchronization
                    matcherIn = preFilter && (seed || !selector) ?
                        condense(elems, preMap, preFilter, context, xml) :
                        elems,

                    matcherOut = matcher ?
                        // If we have a postFinder, or filtered seed, or non-seed postFilter or preexisting results,
                        postFinder || (seed ? preFilter : preexisting || postFilter) ?

                            // ...intermediate processing is necessary
                            [] :

                            // ...otherwise use results directly
                    results :
                        matcherIn;

                // Find primary matches
                if (matcher) {
                    matcher(matcherIn, matcherOut, context, xml);
                }

                // Apply postFilter
                if (postFilter) {
                    temp = condense(matcherOut, postMap);
                    postFilter(temp, [], context, xml);

                    // Un-match failing elements by moving them back to matcherIn
                    i = temp.length;
                    while (i--) {
                        if ((elem = temp[i])) {
                            matcherOut[postMap[i]] = !(matcherIn[postMap[i]] = elem);
                        }
                    }
                }

                if (seed) {
                    if (postFinder || preFilter) {
                        if (postFinder) {
                            // Get the final matcherOut by condensing this intermediate into postFinder contexts
                            temp = [];
                            i = matcherOut.length;
                            while (i--) {
                                if ((elem = matcherOut[i])) {
                                    // Restore matcherIn since elem is not yet a final match
                                    temp.push((matcherIn[i] = elem));
                                }
                            }
                            postFinder(null, (matcherOut = []), temp, xml);
                        }

                        // Move matched elements from seed to results to keep them synchronized
                        i = matcherOut.length;
                        while (i--) {
                            if ((elem = matcherOut[i]) &&
                                (temp = postFinder ? indexOf.call(seed, elem) : preMap[i]) > -1) {

                                seed[temp] = !(results[temp] = elem);
                            }
                        }
                    }

                    // Add elements to results, through postFinder if defined
                } else {
                    matcherOut = condense(
                        matcherOut === results ?
                            matcherOut.splice(preexisting, matcherOut.length) :
                            matcherOut
                    );
                    if (postFinder) {
                        postFinder(null, results, matcherOut, xml);
                    } else {
                        push.apply(results, matcherOut);
                    }
                }
            });
        }

        function matcherFromTokens(tokens) {
            var checkContext, matcher, j,
                len = tokens.length,
                leadingRelative = Expr.relative[tokens[0].type],
                implicitRelative = leadingRelative || Expr.relative[" "],
                i = leadingRelative ? 1 : 0,

                // The foundational matcher ensures that elements are reachable from top-level context(s)
                matchContext = addCombinator(function (elem) {
                    return elem === checkContext;
                }, implicitRelative, true),
                matchAnyContext = addCombinator(function (elem) {
                    return indexOf.call(checkContext, elem) > -1;
                }, implicitRelative, true),
                matchers = [function (elem, context, xml) {
                    return (!leadingRelative && (xml || context !== outermostContext)) || (
                        (checkContext = context).nodeType ?
                            matchContext(elem, context, xml) :
                            matchAnyContext(elem, context, xml));
                }];

            for (; i < len; i++) {
                if ((matcher = Expr.relative[tokens[i].type])) {
                    matchers = [addCombinator(elementMatcher(matchers), matcher)];
                } else {
                    matcher = Expr.filter[tokens[i].type].apply(null, tokens[i].matches);

                    // Return special upon seeing a positional matcher
                    if (matcher[expando]) {
                        // Find the next relative operator (if any) for proper handling
                        j = ++i;
                        for (; j < len; j++) {
                            if (Expr.relative[tokens[j].type]) {
                                break;
                            }
                        }
                        return setMatcher(
                            i > 1 && elementMatcher(matchers),
                            i > 1 && tokens.slice(0, i - 1).join("").replace(rtrim, "$1"),
                            matcher,
                            i < j && matcherFromTokens(tokens.slice(i, j)),
                            j < len && matcherFromTokens((tokens = tokens.slice(j))),
                            j < len && tokens.join("")
                        );
                    }
                    matchers.push(matcher);
                }
            }

            return elementMatcher(matchers);
        }

        function matcherFromGroupMatchers(elementMatchers, setMatchers) {
            var bySet = setMatchers.length > 0,
                byElement = elementMatchers.length > 0,
                superMatcher = function (seed, context, xml, results, expandContext) {
                    var elem, j, matcher,
                        setMatched = [],
                        matchedCount = 0,
                        i = "0",
                        unmatched = seed && [],
                        outermost = expandContext != null,
                        contextBackup = outermostContext,
                        // We must always have either seed elements or context
                        elems = seed || byElement && Expr.find["TAG"]("*", expandContext && context.parentNode || context),
                        // Nested matchers should use non-integer dirruns
                        dirrunsUnique = (dirruns += contextBackup == null ? 1 : Math.E);

                    if (outermost) {
                        outermostContext = context !== document && context;
                        cachedruns = superMatcher.el;
                    }

                    // Add elements passing elementMatchers directly to results
                    for (; (elem = elems[i]) != null; i++) {
                        if (byElement && elem) {
                            for (j = 0; (matcher = elementMatchers[j]) ; j++) {
                                if (matcher(elem, context, xml)) {
                                    results.push(elem);
                                    break;
                                }
                            }
                            if (outermost) {
                                dirruns = dirrunsUnique;
                                cachedruns = ++superMatcher.el;
                            }
                        }

                        // Track unmatched elements for set filters
                        if (bySet) {
                            // They will have gone through all possible matchers
                            if ((elem = !matcher && elem)) {
                                matchedCount--;
                            }

                            // Lengthen the array for every element, matched or not
                            if (seed) {
                                unmatched.push(elem);
                            }
                        }
                    }

                    // Apply set filters to unmatched elements
                    matchedCount += i;
                    if (bySet && i !== matchedCount) {
                        for (j = 0; (matcher = setMatchers[j]) ; j++) {
                            matcher(unmatched, setMatched, context, xml);
                        }

                        if (seed) {
                            // Reintegrate element matches to eliminate the need for sorting
                            if (matchedCount > 0) {
                                while (i--) {
                                    if (!(unmatched[i] || setMatched[i])) {
                                        setMatched[i] = pop.call(results);
                                    }
                                }
                            }

                            // Discard index placeholder values to get only actual matches
                            setMatched = condense(setMatched);
                        }

                        // Add matches to results
                        push.apply(results, setMatched);

                        // Seedless set matches succeeding multiple successful matchers stipulate sorting
                        if (outermost && !seed && setMatched.length > 0 &&
                            (matchedCount + setMatchers.length) > 1) {

                            Sizzle.uniqueSort(results);
                        }
                    }

                    // Override manipulation of globals by nested matchers
                    if (outermost) {
                        dirruns = dirrunsUnique;
                        outermostContext = contextBackup;
                    }

                    return unmatched;
                };

            superMatcher.el = 0;
            return bySet ?
                markFunction(superMatcher) :
                superMatcher;
        }

        compile = Sizzle.compile = function (selector, group /* Internal Use Only */) {
            var i,
                setMatchers = [],
                elementMatchers = [],
                cached = compilerCache[expando][selector + " "];

            if (!cached) {
                // Generate a function of recursive functions that can be used to check each element
                if (!group) {
                    group = tokenize(selector);
                }
                i = group.length;
                while (i--) {
                    cached = matcherFromTokens(group[i]);
                    if (cached[expando]) {
                        setMatchers.push(cached);
                    } else {
                        elementMatchers.push(cached);
                    }
                }

                // Cache the compiled function
                cached = compilerCache(selector, matcherFromGroupMatchers(elementMatchers, setMatchers));
            }
            return cached;
        };

        function multipleContexts(selector, contexts, results) {
            var i = 0,
                len = contexts.length;
            for (; i < len; i++) {
                Sizzle(selector, contexts[i], results);
            }
            return results;
        }

        function select(selector, context, results, seed, xml) {
            var i, tokens, token, type, find,
                match = tokenize(selector),
                j = match.length;

            if (!seed) {
                // Try to minimize operations if there is only one group
                if (match.length === 1) {

                    // Take a shortcut and set the context if the root selector is an ID
                    tokens = match[0] = match[0].slice(0);
                    if (tokens.length > 2 && (token = tokens[0]).type === "ID" &&
                            context.nodeType === 9 && !xml &&
                            Expr.relative[tokens[1].type]) {

                        context = Expr.find["ID"](token.matches[0].replace(rbackslash, ""), context, xml)[0];
                        if (!context) {
                            return results;
                        }

                        selector = selector.slice(tokens.shift().length);
                    }

                    // Fetch a seed set for right-to-left matching
                    for (i = matchExpr["POS"].test(selector) ? -1 : tokens.length - 1; i >= 0; i--) {
                        token = tokens[i];

                        // Abort if we hit a combinator
                        if (Expr.relative[(type = token.type)]) {
                            break;
                        }
                        if ((find = Expr.find[type])) {
                            // Search, expanding context for leading sibling combinators
                            if ((seed = find(
                                token.matches[0].replace(rbackslash, ""),
                                rsibling.test(tokens[0].type) && context.parentNode || context,
                                xml
                            ))) {

                                // If seed is empty or no tokens remain, we can return early
                                tokens.splice(i, 1);
                                selector = seed.length && tokens.join("");
                                if (!selector) {
                                    push.apply(results, slice.call(seed, 0));
                                    return results;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            // Compile and execute a filtering function
            // Provide `match` to avoid retokenization if we modified the selector above
            compile(selector, match)(
                seed,
                context,
                xml,
                results,
                rsibling.test(selector)
            );
            return results;
        }

        if (document.querySelectorAll) {
            (function () {
                var disconnectedMatch,
                    oldSelect = select,
                    rescape = /'|\\/g,
                    rattributeQuotes = /\=[\x20\t\r\n\f]*([^'"\]]*)[\x20\t\r\n\f]*\]/g,

                    // qSa(:focus) reports false when true (Chrome 21), no need to also add to buggyMatches since matches checks buggyQSA
                    // A support test would require too much code (would include document ready)
                    rbuggyQSA = [":focus"],

                    // matchesSelector(:active) reports false when true (IE9/Opera 11.5)
                    // A support test would require too much code (would include document ready)
                    // just skip matchesSelector for :active
                    rbuggyMatches = [":active"],
                    matches = docElem.matchesSelector ||
                        docElem.mozMatchesSelector ||
                        docElem.webkitMatchesSelector ||
                        docElem.oMatchesSelector ||
                        docElem.msMatchesSelector;

                // Build QSA regex
                // Regex strategy adopted from Diego Perini
                assert(function (div) {
                    // Select is set to empty string on purpose
                    // This is to test IE's treatment of not explictly
                    // setting a boolean content attribute,
                    // since its presence should be enough
                    // http://bugs.jquery.com/ticket/12359
                    div.innerHTML = "<select><option selected=''></option></select>";

                    // IE8 - Some boolean attributes are not treated correctly
                    if (!div.querySelectorAll("[selected]").length) {
                        rbuggyQSA.push("\\[" + whitespace + "*(?:checked|disabled|ismap|multiple|readonly|selected|value)");
                    }

                    // Webkit/Opera - :checked should return selected option elements
                    // http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
                    // IE8 throws error here (do not put tests after this one)
                    if (!div.querySelectorAll(":checked").length) {
                        rbuggyQSA.push(":checked");
                    }
                });

                assert(function (div) {

                    // Opera 10-12/IE9 - ^= $= *= and empty values
                    // Should not select anything
                    div.innerHTML = "<p test=''></p>";
                    if (div.querySelectorAll("[test^='']").length) {
                        rbuggyQSA.push("[*^$]=" + whitespace + "*(?:\"\"|'')");
                    }

                    // FF 3.5 - :enabled/:disabled and hidden elements (hidden elements are still enabled)
                    // IE8 throws error here (do not put tests after this one)
                    div.innerHTML = "<input type='hidden'/>";
                    if (!div.querySelectorAll(":enabled").length) {
                        rbuggyQSA.push(":enabled", ":disabled");
                    }
                });

                // rbuggyQSA always contains :focus, so no need for a length check
                rbuggyQSA = /* rbuggyQSA.length && */ new RegExp(rbuggyQSA.join("|"));

                select = function (selector, context, results, seed, xml) {
                    // Only use querySelectorAll when not filtering,
                    // when this is not xml,
                    // and when no QSA bugs apply
                    if (!seed && !xml && !rbuggyQSA.test(selector)) {
                        var groups, i,
                            old = true,
                            nid = expando,
                            newContext = context,
                            newSelector = context.nodeType === 9 && selector;

                        // qSA works strangely on Element-rooted queries
                        // We can work around this by specifying an extra ID on the root
                        // and working up from there (Thanks to Andrew Dupont for the technique)
                        // IE 8 doesn't work on object elements
                        if (context.nodeType === 1 && context.nodeName.toLowerCase() !== "object") {
                            groups = tokenize(selector);

                            if ((old = context.getAttribute("id"))) {
                                nid = old.replace(rescape, "\\$&");
                            } else {
                                context.setAttribute("id", nid);
                            }
                            nid = "[id='" + nid + "'] ";

                            i = groups.length;
                            while (i--) {
                                groups[i] = nid + groups[i].join("");
                            }
                            newContext = rsibling.test(selector) && context.parentNode || context;
                            newSelector = groups.join(",");
                        }

                        if (newSelector) {
                            try {
                                push.apply(results, slice.call(newContext.querySelectorAll(
                                    newSelector
                                ), 0));
                                return results;
                            } catch (qsaError) {
                            } finally {
                                if (!old) {
                                    context.removeAttribute("id");
                                }
                            }
                        }
                    }

                    return oldSelect(selector, context, results, seed, xml);
                };

                if (matches) {
                    assert(function (div) {
                        // Check to see if it's possible to do matchesSelector
                        // on a disconnected node (IE 9)
                        disconnectedMatch = matches.call(div, "div");

                        // This should fail with an exception
                        // Gecko does not error, returns false instead
                        try {
                            matches.call(div, "[test!='']:sizzle");
                            rbuggyMatches.push("!=", pseudos);
                        } catch (e) { }
                    });

                    // rbuggyMatches always contains :active and :focus, so no need for a length check
                    rbuggyMatches = /* rbuggyMatches.length && */ new RegExp(rbuggyMatches.join("|"));

                    Sizzle.matchesSelector = function (elem, expr) {
                        // Make sure that attribute selectors are quoted
                        expr = expr.replace(rattributeQuotes, "='$1']");

                        // rbuggyMatches always contains :active, so no need for an existence check
                        if (!isXML(elem) && !rbuggyMatches.test(expr) && !rbuggyQSA.test(expr)) {
                            try {
                                var ret = matches.call(elem, expr);

                                // IE 9's matchesSelector returns false on disconnected nodes
                                if (ret || disconnectedMatch ||
                                    // As well, disconnected nodes are said to be in a document
                                    // fragment in IE 9
                                        elem.document && elem.document.nodeType !== 11) {
                                    return ret;
                                }
                            } catch (e) { }
                        }

                        return Sizzle(expr, null, null, [elem]).length > 0;
                    };
                }
            })();
        }

        // Deprecated
        Expr.pseudos["nth"] = Expr.pseudos["eq"];

        // Back-compat
        function setFilters() { }
        Expr.filters = setFilters.prototype = Expr.pseudos;
        Expr.setFilters = new setFilters();

        // Override sizzle attribute retrieval
        Sizzle.attr = jQuery.attr;
        jQuery.find = Sizzle;
        jQuery.expr = Sizzle.selectors;
        jQuery.expr[":"] = jQuery.expr.pseudos;
        jQuery.unique = Sizzle.uniqueSort;
        jQuery.text = Sizzle.getText;
        jQuery.isXMLDoc = Sizzle.isXML;
        jQuery.contains = Sizzle.contains;


    })(window);
    var runtil = /Until$/,
        rparentsprev = /^(?:parents|prev(?:Until|All))/,
        isSimple = /^.[^:#\[\.,]*$/,
        rneedsContext = jQuery.expr.match.needsContext,
        // methods guaranteed to produce a unique set when starting from a unique set
        guaranteedUnique = {
            children: true,
            contents: true,
            next: true,
            prev: true
        };

    jQuery.fn.extend({
        find: function (selector) {
            var i, l, length, n, r, ret,
                self = this;

            if (typeof selector !== "string") {
                return jQuery(selector).filter(function () {
                    for (i = 0, l = self.length; i < l; i++) {
                        if (jQuery.contains(self[i], this)) {
                            return true;
                        }
                    }
                });
            }

            ret = this.pushStack("", "find", selector);

            for (i = 0, l = this.length; i < l; i++) {
                length = ret.length;
                jQuery.find(selector, this[i], ret);

                if (i > 0) {
                    // Make sure that the results are unique
                    for (n = length; n < ret.length; n++) {
                        for (r = 0; r < length; r++) {
                            if (ret[r] === ret[n]) {
                                ret.splice(n--, 1);
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        },

        has: function (target) {
            var i,
                targets = jQuery(target, this),
                len = targets.length;

            return this.filter(function () {
                for (i = 0; i < len; i++) {
                    if (jQuery.contains(this, targets[i])) {
                        return true;
                    }
                }
            });
        },

        not: function (selector) {
            return this.pushStack(winnow(this, selector, false), "not", selector);
        },

        filter: function (selector) {
            return this.pushStack(winnow(this, selector, true), "filter", selector);
        },

        is: function (selector) {
            return !!selector && (
                typeof selector === "string" ?
                    // If this is a positional/relative selector, check membership in the returned set
                    // so $("p:first").is("p:last") won't return true for a doc with two "p".
                    rneedsContext.test(selector) ?
                        jQuery(selector, this.context).index(this[0]) >= 0 :
                        jQuery.filter(selector, this).length > 0 :
                    this.filter(selector).length > 0);
        },

        closest: function (selectors, context) {
            var cur,
                i = 0,
                l = this.length,
                ret = [],
                pos = rneedsContext.test(selectors) || typeof selectors !== "string" ?
                    jQuery(selectors, context || this.context) :
                    0;

            for (; i < l; i++) {
                cur = this[i];

                while (cur && cur.ownerDocument && cur !== context && cur.nodeType !== 11) {
                    if (pos ? pos.index(cur) > -1 : jQuery.find.matchesSelector(cur, selectors)) {
                        ret.push(cur);
                        break;
                    }
                    cur = cur.parentNode;
                }
            }

            ret = ret.length > 1 ? jQuery.unique(ret) : ret;

            return this.pushStack(ret, "closest", selectors);
        },

        // Determine the position of an element within
        // the matched set of elements
        index: function (elem) {

            // No argument, return index in parent
            if (!elem) {
                return (this[0] && this[0].parentNode) ? this.prevAll().length : -1;
            }

            // index in selector
            if (typeof elem === "string") {
                return jQuery.inArray(this[0], jQuery(elem));
            }

            // Locate the position of the desired element
            return jQuery.inArray(
                // If it receives a jQuery object, the first element is used
                elem.jquery ? elem[0] : elem, this);
        },

        add: function (selector, context) {
            var set = typeof selector === "string" ?
                    jQuery(selector, context) :
                    jQuery.makeArray(selector && selector.nodeType ? [selector] : selector),
                all = jQuery.merge(this.get(), set);

            return this.pushStack(isDisconnected(set[0]) || isDisconnected(all[0]) ?
                all :
                jQuery.unique(all));
        },

        addBack: function (selector) {
            return this.add(selector == null ?
                this.prevObject : this.prevObject.filter(selector)
            );
        }
    });

    jQuery.fn.andSelf = jQuery.fn.addBack;

    // A painfully simple check to see if an element is disconnected
    // from a document (should be improved, where feasible).
    function isDisconnected(node) {
        return !node || !node.parentNode || node.parentNode.nodeType === 11;
    }

    function sibling(cur, dir) {
        do {
            cur = cur[dir];
        } while (cur && cur.nodeType !== 1);

        return cur;
    }

    jQuery.each({
        parent: function (elem) {
            var parent = elem.parentNode;
            return parent && parent.nodeType !== 11 ? parent : null;
        },
        parents: function (elem) {
            return jQuery.dir(elem, "parentNode");
        },
        parentsUntil: function (elem, i, until) {
            return jQuery.dir(elem, "parentNode", until);
        },
        next: function (elem) {
            return sibling(elem, "nextSibling");
        },
        prev: function (elem) {
            return sibling(elem, "previousSibling");
        },
        nextAll: function (elem) {
            return jQuery.dir(elem, "nextSibling");
        },
        prevAll: function (elem) {
            return jQuery.dir(elem, "previousSibling");
        },
        nextUntil: function (elem, i, until) {
            return jQuery.dir(elem, "nextSibling", until);
        },
        prevUntil: function (elem, i, until) {
            return jQuery.dir(elem, "previousSibling", until);
        },
        siblings: function (elem) {
            return jQuery.sibling((elem.parentNode || {}).firstChild, elem);
        },
        children: function (elem) {
            return jQuery.sibling(elem.firstChild);
        },
        contents: function (elem) {
            return jQuery.nodeName(elem, "iframe") ?
                elem.contentDocument || elem.contentWindow.document :
                jQuery.merge([], elem.childNodes);
        }
    }, function (name, fn) {
        jQuery.fn[name] = function (until, selector) {
            var ret = jQuery.map(this, fn, until);

            if (!runtil.test(name)) {
                selector = until;
            }

            if (selector && typeof selector === "string") {
                ret = jQuery.filter(selector, ret);
            }

            ret = this.length > 1 && !guaranteedUnique[name] ? jQuery.unique(ret) : ret;

            if (this.length > 1 && rparentsprev.test(name)) {
                ret = ret.reverse();
            }

            return this.pushStack(ret, name, core_slice.call(arguments).join(","));
        };
    });

    jQuery.extend({
        filter: function (expr, elems, not) {
            if (not) {
                expr = ":not(" + expr + ")";
            }

            return elems.length === 1 ?
                jQuery.find.matchesSelector(elems[0], expr) ? [elems[0]] : [] :
                jQuery.find.matches(expr, elems);
        },

        dir: function (elem, dir, until) {
            var matched = [],
                cur = elem[dir];

            while (cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery(cur).is(until))) {
                if (cur.nodeType === 1) {
                    matched.push(cur);
                }
                cur = cur[dir];
            }
            return matched;
        },

        sibling: function (n, elem) {
            var r = [];

            for (; n; n = n.nextSibling) {
                if (n.nodeType === 1 && n !== elem) {
                    r.push(n);
                }
            }

            return r;
        }
    });

    // Implement the identical functionality for filter and not
    function winnow(elements, qualifier, keep) {

        // Can't pass null or undefined to indexOf in Firefox 4
        // Set to 0 to skip string check
        qualifier = qualifier || 0;

        if (jQuery.isFunction(qualifier)) {
            return jQuery.grep(elements, function (elem, i) {
                var retVal = !!qualifier.call(elem, i, elem);
                return retVal === keep;
            });

        } else if (qualifier.nodeType) {
            return jQuery.grep(elements, function (elem, i) {
                return (elem === qualifier) === keep;
            });

        } else if (typeof qualifier === "string") {
            var filtered = jQuery.grep(elements, function (elem) {
                return elem.nodeType === 1;
            });

            if (isSimple.test(qualifier)) {
                return jQuery.filter(qualifier, filtered, !keep);
            } else {
                qualifier = jQuery.filter(qualifier, filtered);
            }
        }

        return jQuery.grep(elements, function (elem, i) {
            return (jQuery.inArray(elem, qualifier) >= 0) === keep;
        });
    }
    function createSafeFragment(document) {
        var list = nodeNames.split("|"),
        safeFrag = document.createDocumentFragment();

        if (safeFrag.createElement) {
            while (list.length) {
                safeFrag.createElement(
                    list.pop()
                );
            }
        }
        return safeFrag;
    }

    var nodeNames = "abbr|article|aside|audio|bdi|canvas|data|datalist|details|figcaption|figure|footer|" +
            "header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",
        rinlinejQuery = / jQuery\d+="(?:null|\d+)"/g,
        rleadingWhitespace = /^\s+/,
        rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/gi,
        rtagName = /<([\w:]+)/,
        rtbody = /<tbody/i,
        rhtml = /<|&#?\w+;/,
        rnoInnerhtml = /<(?:script|style|link)/i,
        rnocache = /<(?:script|object|embed|option|style)/i,
        rnoshimcache = new RegExp("<(?:" + nodeNames + ")[\\s/>]", "i"),
        rcheckableType = /^(?:checkbox|radio)$/,
        // checked="checked" or checked
        rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
        rscriptType = /\/(java|ecma)script/i,
        rcleanScript = /^\s*<!(?:\[CDATA\[|\-\-)|[\]\-]{2}>\s*$/g,
        wrapMap = {
            option: [1, "<select multiple='multiple'>", "</select>"],
            legend: [1, "<fieldset>", "</fieldset>"],
            thead: [1, "<table>", "</table>"],
            tr: [2, "<table><tbody>", "</tbody></table>"],
            td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],
            col: [2, "<table><tbody></tbody><colgroup>", "</colgroup></table>"],
            area: [1, "<map>", "</map>"],
            _default: [0, "", ""]
        },
        safeFragment = createSafeFragment(document),
        fragmentDiv = safeFragment.appendChild(document.createElement("div"));

    wrapMap.optgroup = wrapMap.option;
    wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
    wrapMap.th = wrapMap.td;

    // IE6-8 can't serialize link, script, style, or any html5 (NoScope) tags,
    // unless wrapped in a div with non-breaking characters in front of it.
    if (!jQuery.support.htmlSerialize) {
        wrapMap._default = [1, "X<div>", "</div>"];
    }

    jQuery.fn.extend({
        text: function (value) {
            return jQuery.access(this, function (value) {
                return value === undefined ?
                    jQuery.text(this) :
                    this.empty().append((this[0] && this[0].ownerDocument || document).createTextNode(value));
            }, null, value, arguments.length);
        },

        wrapAll: function (html) {
            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapAll(html.call(this, i));
                });
            }

            if (this[0]) {
                // The elements to wrap the target around
                var wrap = jQuery(html, this[0].ownerDocument).eq(0).clone(true);

                if (this[0].parentNode) {
                    wrap.insertBefore(this[0]);
                }

                wrap.map(function () {
                    var elem = this;

                    while (elem.firstChild && elem.firstChild.nodeType === 1) {
                        elem = elem.firstChild;
                    }

                    return elem;
                }).append(this);
            }

            return this;
        },

        wrapInner: function (html) {
            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapInner(html.call(this, i));
                });
            }

            return this.each(function () {
                var self = jQuery(this),
                    contents = self.contents();

                if (contents.length) {
                    contents.wrapAll(html);

                } else {
                    self.append(html);
                }
            });
        },

        wrap: function (html) {
            var isFunction = jQuery.isFunction(html);

            return this.each(function (i) {
                jQuery(this).wrapAll(isFunction ? html.call(this, i) : html);
            });
        },

        unwrap: function () {
            return this.parent().each(function () {
                if (!jQuery.nodeName(this, "body")) {
                    jQuery(this).replaceWith(this.childNodes);
                }
            }).end();
        },

        append: function () {
            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1 || this.nodeType === 11) {
                    this.appendChild(elem);
                }
            });
        },

        prepend: function () {
            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1 || this.nodeType === 11) {
                    this.insertBefore(elem, this.firstChild);
                }
            });
        },

        before: function () {
            if (!isDisconnected(this[0])) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this);
                });
            }

            if (arguments.length) {
                var set = jQuery.clean(arguments);
                return this.pushStack(jQuery.merge(set, this), "before", this.selector);
            }
        },

        after: function () {
            if (!isDisconnected(this[0])) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this.nextSibling);
                });
            }

            if (arguments.length) {
                var set = jQuery.clean(arguments);
                return this.pushStack(jQuery.merge(this, set), "after", this.selector);
            }
        },

        // keepData is for internal use only--do not document
        remove: function (selector, keepData) {
            var elem,
                i = 0;

            for (; (elem = this[i]) != null; i++) {
                if (!selector || jQuery.filter(selector, [elem]).length) {
                    if (!keepData && elem.nodeType === 1) {
                        jQuery.cleanData(elem.getElementsByTagName("*"));
                        jQuery.cleanData([elem]);
                    }

                    if (elem.parentNode) {
                        elem.parentNode.removeChild(elem);
                    }
                }
            }

            return this;
        },

        empty: function () {
            var elem,
                i = 0;

            for (; (elem = this[i]) != null; i++) {
                // Remove element nodes and prevent memory leaks
                if (elem.nodeType === 1) {
                    jQuery.cleanData(elem.getElementsByTagName("*"));
                }

                // Remove any remaining nodes
                while (elem.firstChild) {
                    elem.removeChild(elem.firstChild);
                }
            }

            return this;
        },

        clone: function (dataAndEvents, deepDataAndEvents) {
            dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
            deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

            return this.map(function () {
                return jQuery.clone(this, dataAndEvents, deepDataAndEvents);
            });
        },

        html: function (value) {
            return jQuery.access(this, function (value) {
                var elem = this[0] || {},
                    i = 0,
                    l = this.length;

                if (value === undefined) {
                    return elem.nodeType === 1 ?
                        elem.innerHTML.replace(rinlinejQuery, "") :
                        undefined;
                }

                // See if we can take a shortcut and just use innerHTML
                if (typeof value === "string" && !rnoInnerhtml.test(value) &&
                    (jQuery.support.htmlSerialize || !rnoshimcache.test(value)) &&
                    (jQuery.support.leadingWhitespace || !rleadingWhitespace.test(value)) &&
                    !wrapMap[(rtagName.exec(value) || ["", ""])[1].toLowerCase()]) {

                    value = value.replace(rxhtmlTag, "<$1></$2>");

                    try {
                        for (; i < l; i++) {
                            // Remove element nodes and prevent memory leaks
                            elem = this[i] || {};
                            if (elem.nodeType === 1) {
                                jQuery.cleanData(elem.getElementsByTagName("*"));
                                elem.innerHTML = value;
                            }
                        }

                        elem = 0;

                        // If using innerHTML throws an exception, use the fallback method
                    } catch (e) { }
                }

                if (elem) {
                    this.empty().append(value);
                }
            }, null, value, arguments.length);
        },

        replaceWith: function (value) {
            if (!isDisconnected(this[0])) {
                // Make sure that the elements are removed from the DOM before they are inserted
                // this can help fix replacing a parent with child elements
                if (jQuery.isFunction(value)) {
                    return this.each(function (i) {
                        var self = jQuery(this), old = self.html();
                        self.replaceWith(value.call(this, i, old));
                    });
                }

                if (typeof value !== "string") {
                    value = jQuery(value).detach();
                }

                return this.each(function () {
                    var next = this.nextSibling,
                        parent = this.parentNode;

                    jQuery(this).remove();

                    if (next) {
                        jQuery(next).before(value);
                    } else {
                        jQuery(parent).append(value);
                    }
                });
            }

            return this.length ?
                this.pushStack(jQuery(jQuery.isFunction(value) ? value() : value), "replaceWith", value) :
                this;
        },

        detach: function (selector) {
            return this.remove(selector, true);
        },

        domManip: function (args, table, callback) {

            // Flatten any nested arrays
            args = [].concat.apply([], args);

            var results, first, fragment, iNoClone,
                i = 0,
                value = args[0],
                scripts = [],
                l = this.length;

            // We can't cloneNode fragments that contain checked, in WebKit
            if (!jQuery.support.checkClone && l > 1 && typeof value === "string" && rchecked.test(value)) {
                return this.each(function () {
                    jQuery(this).domManip(args, table, callback);
                });
            }

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    args[0] = value.call(this, i, table ? self.html() : undefined);
                    self.domManip(args, table, callback);
                });
            }

            if (this[0]) {
                results = jQuery.buildFragment(args, this, scripts);
                fragment = results.fragment;
                first = fragment.firstChild;

                if (fragment.childNodes.length === 1) {
                    fragment = first;
                }

                if (first) {
                    table = table && jQuery.nodeName(first, "tr");

                    // Use the original fragment for the last item instead of the first because it can end up
                    // being emptied incorrectly in certain situations (#8070).
                    // Fragments from the fragment cache must always be cloned and never used in place.
                    for (iNoClone = results.cacheable || l - 1; i < l; i++) {
                        callback.call(
                            table && jQuery.nodeName(this[i], "table") ?
                                findOrAppend(this[i], "tbody") :
                                this[i],
                            i === iNoClone ?
                            fragment :
                                jQuery.clone(fragment, true, true)
                        );
                    }
                }

                // Fix #11809: Avoid leaking memory
                fragment = first = null;

                if (scripts.length) {
                    jQuery.each(scripts, function (i, elem) {
                        if (elem.src) {
                            if (jQuery.ajax) {
                                jQuery.ajax({
                                    url: elem.src,
                                    type: "GET",
                                    dataType: "script",
                                    async: false,
                                    global: false,
                                    "throws": true
                                });
                            } else {
                                jQuery.error("no ajax");
                            }
                        } else {
                            jQuery.globalEval((elem.text || elem.textContent || elem.innerHTML || "").replace(rcleanScript, ""));
                        }

                        if (elem.parentNode) {
                            elem.parentNode.removeChild(elem);
                        }
                    });
                }
            }

            return this;
        }
    });

    function findOrAppend(elem, tag) {
        return elem.getElementsByTagName(tag)[0] || elem.appendChild(elem.ownerDocument.createElement(tag));
    }

    function cloneCopyEvent(src, dest) {

        if (dest.nodeType !== 1 || !jQuery.hasData(src)) {
            return;
        }

        var type, i, l,
            oldData = jQuery._data(src),
            curData = jQuery._data(dest, oldData),
            events = oldData.events;

        if (events) {
            delete curData.handle;
            curData.events = {};

            for (type in events) {
                for (i = 0, l = events[type].length; i < l; i++) {
                    jQuery.event.add(dest, type, events[type][i]);
                }
            }
        }

        // make the cloned public data object a copy from the original
        if (curData.data) {
            curData.data = jQuery.extend({}, curData.data);
        }
    }

    function cloneFixAttributes(src, dest) {
        var nodeName;

        // We do not need to do anything for non-Elements
        if (dest.nodeType !== 1) {
            return;
        }

        // clearAttributes removes the attributes, which we don't want,
        // but also removes the attachEvent events, which we *do* want
        if (dest.clearAttributes) {
            dest.clearAttributes();
        }

        // mergeAttributes, in contrast, only merges back on the
        // original attributes, not the events
        if (dest.mergeAttributes) {
            dest.mergeAttributes(src);
        }

        nodeName = dest.nodeName.toLowerCase();

        if (nodeName === "object") {
            // IE6-10 improperly clones children of object elements using classid.
            // IE10 throws NoModificationAllowedError if parent is null, #12132.
            if (dest.parentNode) {
                dest.outerHTML = src.outerHTML;
            }

            // This path appears unavoidable for IE9. When cloning an object
            // element in IE9, the outerHTML strategy above is not sufficient.
            // If the src has innerHTML and the destination does not,
            // copy the src.innerHTML into the dest.innerHTML. #10324
            if (jQuery.support.html5Clone && (src.innerHTML && !jQuery.trim(dest.innerHTML))) {
                dest.innerHTML = src.innerHTML;
            }

        } else if (nodeName === "input" && rcheckableType.test(src.type)) {
            // IE6-8 fails to persist the checked state of a cloned checkbox
            // or radio button. Worse, IE6-7 fail to give the cloned element
            // a checked appearance if the defaultChecked value isn't also set

            dest.defaultChecked = dest.checked = src.checked;

            // IE6-7 get confused and end up setting the value of a cloned
            // checkbox/radio button to an empty string instead of "on"
            if (dest.value !== src.value) {
                dest.value = src.value;
            }

            // IE6-8 fails to return the selected option to the default selected
            // state when cloning options
        } else if (nodeName === "option") {
            dest.selected = src.defaultSelected;

            // IE6-8 fails to set the defaultValue to the correct value when
            // cloning other types of input fields
        } else if (nodeName === "input" || nodeName === "textarea") {
            dest.defaultValue = src.defaultValue;

            // IE blanks contents when cloning scripts
        } else if (nodeName === "script" && dest.text !== src.text) {
            dest.text = src.text;
        }

        // Event data gets referenced instead of copied if the expando
        // gets copied too
        dest.removeAttribute(jQuery.expando);
    }

    jQuery.buildFragment = function (args, context, scripts) {
        var fragment, cacheable, cachehit,
            first = args[0];

        // Set context from what may come in as undefined or a jQuery collection or a node
        // Updated to fix #12266 where accessing context[0] could throw an exception in IE9/10 &
        // also doubles as fix for #8950 where plain objects caused createDocumentFragment exception
        context = context || document;
        context = !context.nodeType && context[0] || context;
        context = context.ownerDocument || context;

        // Only cache "small" (1/2 KB) HTML strings that are associated with the main document
        // Cloning options loses the selected state, so don't cache them
        // IE 6 doesn't like it when you put <object> or <embed> elements in a fragment
        // Also, WebKit does not clone 'checked' attributes on cloneNode, so don't cache
        // Lastly, IE6,7,8 will not correctly reuse cached fragments that were created from unknown elems #10501
        if (args.length === 1 && typeof first === "string" && first.length < 512 && context === document &&
            first.charAt(0) === "<" && !rnocache.test(first) &&
            (jQuery.support.checkClone || !rchecked.test(first)) &&
            (jQuery.support.html5Clone || !rnoshimcache.test(first))) {

            // Mark cacheable and look for a hit
            cacheable = true;
            fragment = jQuery.fragments[first];
            cachehit = fragment !== undefined;
        }

        if (!fragment) {
            fragment = context.createDocumentFragment();
            jQuery.clean(args, context, fragment, scripts);

            // Update the cache, but only store false
            // unless this is a second parsing of the same content
            if (cacheable) {
                jQuery.fragments[first] = cachehit && fragment;
            }
        }

        return { fragment: fragment, cacheable: cacheable };
    };

    jQuery.fragments = {};

    jQuery.each({
        appendTo: "append",
        prependTo: "prepend",
        insertBefore: "before",
        insertAfter: "after",
        replaceAll: "replaceWith"
    }, function (name, original) {
        jQuery.fn[name] = function (selector) {
            var elems,
                i = 0,
                ret = [],
                insert = jQuery(selector),
                l = insert.length,
                parent = this.length === 1 && this[0].parentNode;

            if ((parent == null || parent && parent.nodeType === 11 && parent.childNodes.length === 1) && l === 1) {
                insert[original](this[0]);
                return this;
            } else {
                for (; i < l; i++) {
                    elems = (i > 0 ? this.clone(true) : this).get();
                    jQuery(insert[i])[original](elems);
                    ret = ret.concat(elems);
                }

                return this.pushStack(ret, name, insert.selector);
            }
        };
    });

    function getAll(elem) {
        if (typeof elem.getElementsByTagName !== "undefined") {
            return elem.getElementsByTagName("*");

        } else if (typeof elem.querySelectorAll !== "undefined") {
            return elem.querySelectorAll("*");

        } else {
            return [];
        }
    }

    // Used in clean, fixes the defaultChecked property
    function fixDefaultChecked(elem) {
        if (rcheckableType.test(elem.type)) {
            elem.defaultChecked = elem.checked;
        }
    }

    jQuery.extend({
        clone: function (elem, dataAndEvents, deepDataAndEvents) {
            var srcElements,
                destElements,
                i,
                clone;

            if (jQuery.support.html5Clone || jQuery.isXMLDoc(elem) || !rnoshimcache.test("<" + elem.nodeName + ">")) {
                clone = elem.cloneNode(true);

                // IE<=8 does not properly clone detached, unknown element nodes
            } else {
                fragmentDiv.innerHTML = elem.outerHTML;
                fragmentDiv.removeChild(clone = fragmentDiv.firstChild);
            }

            if ((!jQuery.support.noCloneEvent || !jQuery.support.noCloneChecked) &&
                    (elem.nodeType === 1 || elem.nodeType === 11) && !jQuery.isXMLDoc(elem)) {
                // IE copies events bound via attachEvent when using cloneNode.
                // Calling detachEvent on the clone will also remove the events
                // from the original. In order to get around this, we use some
                // proprietary methods to clear the events. Thanks to MooTools
                // guys for this hotness.

                cloneFixAttributes(elem, clone);

                // Using Sizzle here is crazy slow, so we use getElementsByTagName instead
                srcElements = getAll(elem);
                destElements = getAll(clone);

                // Weird iteration because IE will replace the length property
                // with an element if you are cloning the body and one of the
                // elements on the page has a name or id of "length"
                for (i = 0; srcElements[i]; ++i) {
                    // Ensure that the destination node is not null; Fixes #9587
                    if (destElements[i]) {
                        cloneFixAttributes(srcElements[i], destElements[i]);
                    }
                }
            }

            // Copy the events from the original to the clone
            if (dataAndEvents) {
                cloneCopyEvent(elem, clone);

                if (deepDataAndEvents) {
                    srcElements = getAll(elem);
                    destElements = getAll(clone);

                    for (i = 0; srcElements[i]; ++i) {
                        cloneCopyEvent(srcElements[i], destElements[i]);
                    }
                }
            }

            srcElements = destElements = null;

            // Return the cloned set
            return clone;
        },

        clean: function (elems, context, fragment, scripts) {
            var i, j, elem, tag, wrap, depth, div, hasBody, tbody, len, handleScript, jsTags,
                safe = context === document && safeFragment,
                ret = [];

            // Ensure that context is a document
            if (!context || typeof context.createDocumentFragment === "undefined") {
                context = document;
            }

            // Use the already-created safe fragment if context permits
            for (i = 0; (elem = elems[i]) != null; i++) {
                if (typeof elem === "number") {
                    elem += "";
                }

                if (!elem) {
                    continue;
                }

                // Convert html string into DOM nodes
                if (typeof elem === "string") {
                    if (!rhtml.test(elem)) {
                        elem = context.createTextNode(elem);
                    } else {
                        // Ensure a safe container in which to render the html
                        safe = safe || createSafeFragment(context);
                        div = context.createElement("div");
                        safe.appendChild(div);

                        // Fix "XHTML"-style tags in all browsers
                        elem = elem.replace(rxhtmlTag, "<$1></$2>");

                        // Go to html and back, then peel off extra wrappers
                        tag = (rtagName.exec(elem) || ["", ""])[1].toLowerCase();
                        wrap = wrapMap[tag] || wrapMap._default;
                        depth = wrap[0];
                        div.innerHTML = wrap[1] + elem + wrap[2];

                        // Move to the right depth
                        while (depth--) {
                            div = div.lastChild;
                        }

                        // Remove IE's autoinserted <tbody> from table fragments
                        if (!jQuery.support.tbody) {

                            // String was a <table>, *may* have spurious <tbody>
                            hasBody = rtbody.test(elem);
                            tbody = tag === "table" && !hasBody ?
								div.firstChild && div.firstChild.childNodes :

								// String was a bare <thead> or <tfoot>
								wrap[1] === "<table>" && !hasBody ?
									div.childNodes :
									[];

                            for (j = tbody.length - 1; j >= 0 ; --j) {
                                if (jQuery.nodeName(tbody[j], "tbody") && !tbody[j].childNodes.length) {
                                    tbody[j].parentNode.removeChild(tbody[j]);
                                }
                            }
                        }

                        // IE completely kills leading whitespace when innerHTML is used
                        if (!jQuery.support.leadingWhitespace && rleadingWhitespace.test(elem)) {
                            div.insertBefore(context.createTextNode(rleadingWhitespace.exec(elem)[0]), div.firstChild);
                        }

                        elem = div.childNodes;

                        // Take out of fragment container (we need a fresh div each time)
                        div.parentNode.removeChild(div);
                    }
                }

                if (elem.nodeType) {
                    ret.push(elem);
                } else {
                    jQuery.merge(ret, elem);
                }
            }

            // Fix #11356: Clear elements from safeFragment
            if (div) {
                elem = div = safe = null;
            }

            // Reset defaultChecked for any radios and checkboxes
            // about to be appended to the DOM in IE 6/7 (#8060)
            if (!jQuery.support.appendChecked) {
                for (i = 0; (elem = ret[i]) != null; i++) {
                    if (jQuery.nodeName(elem, "input")) {
                        fixDefaultChecked(elem);
                    } else if (typeof elem.getElementsByTagName !== "undefined") {
                        jQuery.grep(elem.getElementsByTagName("input"), fixDefaultChecked);
                    }
                }
            }

            // Append elements to a provided document fragment
            if (fragment) {
                // Special handling of each script element
                handleScript = function (elem) {
                    // Check if we consider it executable
                    if (!elem.type || rscriptType.test(elem.type)) {
                        // Detach the script and store it in the scripts array (if provided) or the fragment
                        // Return truthy to indicate that it has been handled
                        return scripts ?
                            scripts.push(elem.parentNode ? elem.parentNode.removeChild(elem) : elem) :
                            fragment.appendChild(elem);
                    }
                };

                for (i = 0; (elem = ret[i]) != null; i++) {
                    // Check if we're done after handling an executable script
                    if (!(jQuery.nodeName(elem, "script") && handleScript(elem))) {
                        // Append to fragment and handle embedded scripts
                        fragment.appendChild(elem);
                        if (typeof elem.getElementsByTagName !== "undefined") {
                            // handleScript alters the DOM, so use jQuery.merge to ensure snapshot iteration
                            jsTags = jQuery.grep(jQuery.merge([], elem.getElementsByTagName("script")), handleScript);

                            // Splice the scripts into ret after their former ancestor and advance our index beyond them
                            ret.splice.apply(ret, [i + 1, 0].concat(jsTags));
                            i += jsTags.length;
                        }
                    }
                }
            }

            return ret;
        },

        cleanData: function (elems, /* internal */ acceptData) {
            var data, id, elem, type,
                i = 0,
                internalKey = jQuery.expando,
                cache = jQuery.cache,
                deleteExpando = jQuery.support.deleteExpando,
                special = jQuery.event.special;

            for (; (elem = elems[i]) != null; i++) {

                if (acceptData || jQuery.acceptData(elem)) {

                    id = elem[internalKey];
                    data = id && cache[id];

                    if (data) {
                        if (data.events) {
                            for (type in data.events) {
                                if (special[type]) {
                                    jQuery.event.remove(elem, type);

                                    // This is a shortcut to avoid jQuery.event.remove's overhead
                                } else {
                                    jQuery.removeEvent(elem, type, data.handle);
                                }
                            }
                        }

                        // Remove cache only if it was not already removed by jQuery.event.remove
                        if (cache[id]) {

                            delete cache[id];

                            // IE does not allow us to delete expando properties from nodes,
                            // nor does it have a removeAttribute function on Document nodes;
                            // we must handle all of these cases
                            if (deleteExpando) {
                                delete elem[internalKey];

                            } else if (elem.removeAttribute) {
                                elem.removeAttribute(internalKey);

                            } else {
                                elem[internalKey] = null;
                            }

                            jQuery.deletedIds.push(id);
                        }
                    }
                }
            }
        }
    });
    // Limit scope pollution from any deprecated API
    (function () {

        var matched, browser;

        // Use of jQuery.browser is frowned upon.
        // More details: http://api.jquery.com/jQuery.browser
        // jQuery.uaMatch maintained for back-compat
        jQuery.uaMatch = function (ua) {
            ua = ua.toLowerCase();

            var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
                /(webkit)[ \/]([\w.]+)/.exec(ua) ||
                /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
                /(msie) ([\w.]+)/.exec(ua) ||
                ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
                [];

            return {
                browser: match[1] || "",
                version: match[2] || "0"
            };
        };

        matched = jQuery.uaMatch(navigator.userAgent);
        browser = {};

        if (matched.browser) {
            browser[matched.browser] = true;
            browser.version = matched.version;
        }

        // Chrome is Webkit, but Webkit is also Safari.
        if (browser.chrome) {
            browser.webkit = true;
        } else if (browser.webkit) {
            browser.safari = true;
        }

        jQuery.browser = browser;

        jQuery.sub = function () {
            function jQuerySub(selector, context) {
                return new jQuerySub.fn.init(selector, context);
            }
            jQuery.extend(true, jQuerySub, this);
            jQuerySub.superclass = this;
            jQuerySub.fn = jQuerySub.prototype = this();
            jQuerySub.fn.constructor = jQuerySub;
            jQuerySub.sub = this.sub;
            jQuerySub.fn.init = function init(selector, context) {
                if (context && context instanceof jQuery && !(context instanceof jQuerySub)) {
                    context = jQuerySub(context);
                }

                return jQuery.fn.init.call(this, selector, context, rootjQuerySub);
            };
            jQuerySub.fn.init.prototype = jQuerySub.fn;
            var rootjQuerySub = jQuerySub(document);
            return jQuerySub;
        };

    })();
    var curCSS, iframe, iframeDoc,
        ralpha = /alpha\([^)]*\)/i,
        ropacity = /opacity=([^)]*)/,
        rposition = /^(top|right|bottom|left)$/,
        // swappable if display is none or starts with table except "table", "table-cell", or "table-caption"
        // see here for display values: https://developer.mozilla.org/en-US/docs/CSS/display
        rdisplayswap = /^(none|table(?!-c[ea]).+)/,
        rmargin = /^margin/,
        rnumsplit = new RegExp("^(" + core_pnum + ")(.*)$", "i"),
        rnumnonpx = new RegExp("^(" + core_pnum + ")(?!px)[a-z%]+$", "i"),
        rrelNum = new RegExp("^([-+])=(" + core_pnum + ")", "i"),
        elemdisplay = { BODY: "block" },

        cssShow = { position: "absolute", visibility: "hidden", display: "block" },
        cssNormalTransform = {
            letterSpacing: 0,
            fontWeight: 400
        },

        cssExpand = ["Top", "Right", "Bottom", "Left"],
        cssPrefixes = ["Webkit", "O", "Moz", "ms"],

        eventsToggle = jQuery.fn.toggle;

    // return a css property mapped to a potentially vendor prefixed property
    function vendorPropName(style, name) {

        // shortcut for names that are not vendor prefixed
        if (name in style) {
            return name;
        }

        // check for vendor prefixed names
        var capName = name.charAt(0).toUpperCase() + name.slice(1),
            origName = name,
            i = cssPrefixes.length;

        while (i--) {
            name = cssPrefixes[i] + capName;
            if (name in style) {
                return name;
            }
        }

        return origName;
    }

    function isHidden(elem, el) {
        elem = el || elem;
        return jQuery.css(elem, "display") === "none" || !jQuery.contains(elem.ownerDocument, elem);
    }

    function showHide(elements, show) {
        var elem, display,
            values = [],
            index = 0,
            length = elements.length;

        for (; index < length; index++) {
            elem = elements[index];
            if (!elem.style) {
                continue;
            }
            values[index] = jQuery._data(elem, "olddisplay");
            if (show) {
                // Reset the inline display of this element to learn if it is
                // being hidden by cascaded rules or not
                if (!values[index] && elem.style.display === "none") {
                    elem.style.display = "";
                }

                // Set elements which have been overridden with display: none
                // in a stylesheet to whatever the default browser style is
                // for such an element
                if (elem.style.display === "" && isHidden(elem)) {
                    values[index] = jQuery._data(elem, "olddisplay", css_defaultDisplay(elem.nodeName));
                }
            } else {
                display = curCSS(elem, "display");

                if (!values[index] && display !== "none") {
                    jQuery._data(elem, "olddisplay", display);
                }
            }
        }

        // Set the display of most of the elements in a second loop
        // to avoid the constant reflow
        for (index = 0; index < length; index++) {
            elem = elements[index];
            if (!elem.style) {
                continue;
            }
            if (!show || elem.style.display === "none" || elem.style.display === "") {
                elem.style.display = show ? values[index] || "" : "none";
            }
        }

        return elements;
    }

    jQuery.fn.extend({
        css: function (name, value) {
            return jQuery.access(this, function (elem, name, value) {
                return value !== undefined ?
                    jQuery.style(elem, name, value) :
                    jQuery.css(elem, name);
            }, name, value, arguments.length > 1);
        },
        show: function () {
            return showHide(this, true);
        },
        hide: function () {
            return showHide(this);
        },
        toggle: function (state, fn2) {
            var bool = typeof state === "boolean";

            if (jQuery.isFunction(state) && jQuery.isFunction(fn2)) {
                return eventsToggle.apply(this, arguments);
            }

            return this.each(function () {
                if (bool ? state : isHidden(this)) {
                    jQuery(this).show();
                } else {
                    jQuery(this).hide();
                }
            });
        }
    });

    jQuery.extend({
        // Add in style property hooks for overriding the default
        // behavior of getting and setting a style property
        cssHooks: {
            opacity: {
                get: function (elem, computed) {
                    if (computed) {
                        // We should always get a number back from opacity
                        var ret = curCSS(elem, "opacity");
                        return ret === "" ? "1" : ret;

                    }
                }
            }
        },

        // Exclude the following css properties to add px
        cssNumber: {
            "fillOpacity": true,
            "fontWeight": true,
            "lineHeight": true,
            "opacity": true,
            "orphans": true,
            "widows": true,
            "zIndex": true,
            "zoom": true
        },

        // Add in properties whose names you wish to fix before
        // setting or getting the value
        cssProps: {
            // normalize float css property
            "float": jQuery.support.cssFloat ? "cssFloat" : "styleFloat"
        },

        // Get and set the style property on a DOM Node
        style: function (elem, name, value, extra) {
            // Don't set styles on text and comment nodes
            if (!elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style) {
                return;
            }

            // Make sure that we're working with the right name
            var ret, type, hooks,
                origName = jQuery.camelCase(name),
                style = elem.style;

            name = jQuery.cssProps[origName] || (jQuery.cssProps[origName] = vendorPropName(style, origName));

            // gets hook for the prefixed version
            // followed by the unprefixed version
            hooks = jQuery.cssHooks[name] || jQuery.cssHooks[origName];

            // Check if we're setting a value
            if (value !== undefined) {
                type = typeof value;

                // convert relative number strings (+= or -=) to relative numbers. #7345
                if (type === "string" && (ret = rrelNum.exec(value))) {
                    value = (ret[1] + 1) * ret[2] + parseFloat(jQuery.css(elem, name));
                    // Fixes bug #9237
                    type = "number";
                }

                // Make sure that NaN and null values aren't set. See: #7116
                if (value == null || type === "number" && isNaN(value)) {
                    return;
                }

                // If a number was passed in, add 'px' to the (except for certain CSS properties)
                if (type === "number" && !jQuery.cssNumber[origName]) {
                    value += "px";
                }

                // If a hook was provided, use that value, otherwise just set the specified value
                if (!hooks || !("set" in hooks) || (value = hooks.set(elem, value, extra)) !== undefined) {
                    // Wrapped to prevent IE from throwing errors when 'invalid' values are provided
                    // Fixes bug #5509
                    try {
                        style[name] = value;
                    } catch (e) { }
                }

            } else {
                // If a hook was provided get the non-computed value from there
                if (hooks && "get" in hooks && (ret = hooks.get(elem, false, extra)) !== undefined) {
                    return ret;
                }

                // Otherwise just get the value from the style object
                return style[name];
            }
        },

        css: function (elem, name, numeric, extra) {
            var val, num, hooks,
                origName = jQuery.camelCase(name);

            // Make sure that we're working with the right name
            name = jQuery.cssProps[origName] || (jQuery.cssProps[origName] = vendorPropName(elem.style, origName));

            // gets hook for the prefixed version
            // followed by the unprefixed version
            hooks = jQuery.cssHooks[name] || jQuery.cssHooks[origName];

            // If a hook was provided get the computed value from there
            if (hooks && "get" in hooks) {
                val = hooks.get(elem, true, extra);
            }

            // Otherwise, if a way to get the computed value exists, use that
            if (val === undefined) {
                val = curCSS(elem, name);
            }

            //convert "normal" to computed value
            if (val === "normal" && name in cssNormalTransform) {
                val = cssNormalTransform[name];
            }

            // Return, converting to number if forced or a qualifier was provided and val looks numeric
            if (numeric || extra !== undefined) {
                num = parseFloat(val);
                return numeric || jQuery.isNumeric(num) ? num || 0 : val;
            }
            return val;
        },

        // A method for quickly swapping in/out CSS properties to get correct calculations
        swap: function (elem, options, callback) {
            var ret, name,
                old = {};

            // Remember the old values, and insert the new ones
            for (name in options) {
                old[name] = elem.style[name];
                elem.style[name] = options[name];
            }

            ret = callback.call(elem);

            // Revert the old values
            for (name in options) {
                elem.style[name] = old[name];
            }

            return ret;
        }
    });

    // NOTE: To any future maintainer, we've window.getComputedStyle
    // because jsdom on node.js will break without it.
    if (window.getComputedStyle) {
        curCSS = function (elem, name) {
            var ret, width, minWidth, maxWidth,
                computed = window.getComputedStyle(elem, null),
                style = elem.style;

            if (computed) {

                // getPropertyValue is only needed for .css('filter') in IE9, see #12537
                ret = computed.getPropertyValue(name) || computed[name];

                if (ret === "" && !jQuery.contains(elem.ownerDocument, elem)) {
                    ret = jQuery.style(elem, name);
                }

                // A tribute to the "awesome hack by Dean Edwards"
                // Chrome < 17 and Safari 5.0 uses "computed value" instead of "used value" for margin-right
                // Safari 5.1.7 (at least) returns percentage for a larger set of values, but width seems to be reliably pixels
                // this is against the CSSOM draft spec: http://dev.w3.org/csswg/cssom/#resolved-values
                if (rnumnonpx.test(ret) && rmargin.test(name)) {
                    width = style.width;
                    minWidth = style.minWidth;
                    maxWidth = style.maxWidth;

                    style.minWidth = style.maxWidth = style.width = ret;
                    ret = computed.width;

                    style.width = width;
                    style.minWidth = minWidth;
                    style.maxWidth = maxWidth;
                }
            }

            return ret;
        };
    } else if (document.documentElement.currentStyle) {
        curCSS = function (elem, name) {
            var left, rsLeft,
                ret = elem.currentStyle && elem.currentStyle[name],
                style = elem.style;

            // Avoid setting ret to empty string here
            // so we don't default to auto
            if (ret == null && style && style[name]) {
                ret = style[name];
            }

            // From the awesome hack by Dean Edwards
            // http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

            // If we're not dealing with a regular pixel number
            // but a number that has a weird ending, we need to convert it to pixels
            // but not position css attributes, as those are proportional to the parent element instead
            // and we can't measure the parent instead because it might trigger a "stacking dolls" problem
            if (rnumnonpx.test(ret) && !rposition.test(name)) {

                // Remember the original values
                left = style.left;
                rsLeft = elem.runtimeStyle && elem.runtimeStyle.left;

                // Put in the new values to get a computed value out
                if (rsLeft) {
                    elem.runtimeStyle.left = elem.currentStyle.left;
                }
                style.left = name === "fontSize" ? "1em" : ret;
                ret = style.pixelLeft + "px";

                // Revert the changed values
                style.left = left;
                if (rsLeft) {
                    elem.runtimeStyle.left = rsLeft;
                }
            }

            return ret === "" ? "auto" : ret;
        };
    }

    function setPositiveNumber(elem, value, subtract) {
        var matches = rnumsplit.exec(value);
        return matches ?
                Math.max(0, matches[1] - (subtract || 0)) + (matches[2] || "px") :
                value;
    }

    function augmentWidthOrHeight(elem, name, extra, isBorderBox) {
        var i = extra === (isBorderBox ? "border" : "content") ?
            // If we already have the right measurement, avoid augmentation
            4 :
            // Otherwise initialize for horizontal or vertical properties
            name === "width" ? 1 : 0,

            val = 0;

        for (; i < 4; i += 2) {
            // both box models exclude margin, so add it if we want it
            if (extra === "margin") {
                // we use jQuery.css instead of curCSS here
                // because of the reliableMarginRight CSS hook!
                val += jQuery.css(elem, extra + cssExpand[i], true);
            }

            // From this point on we use curCSS for maximum performance (relevant in animations)
            if (isBorderBox) {
                // border-box includes padding, so remove it if we want content
                if (extra === "content") {
                    val -= parseFloat(curCSS(elem, "padding" + cssExpand[i])) || 0;
                }

                // at this point, extra isn't border nor margin, so remove border
                if (extra !== "margin") {
                    val -= parseFloat(curCSS(elem, "border" + cssExpand[i] + "Width")) || 0;
                }
            } else {
                // at this point, extra isn't content, so add padding
                val += parseFloat(curCSS(elem, "padding" + cssExpand[i])) || 0;

                // at this point, extra isn't content nor padding, so add border
                if (extra !== "padding") {
                    val += parseFloat(curCSS(elem, "border" + cssExpand[i] + "Width")) || 0;
                }
            }
        }

        return val;
    }

    function getWidthOrHeight(elem, name, extra) {

        // Start with offset property, which is equivalent to the border-box value
        var val = name === "width" ? elem.offsetWidth : elem.offsetHeight,
            valueIsBorderBox = true,
            isBorderBox = jQuery.support.boxSizing && jQuery.css(elem, "boxSizing") === "border-box";

        // some non-html elements return undefined for offsetWidth, so check for null/undefined
        // svg - https://bugzilla.mozilla.org/show_bug.cgi?id=649285
        // MathML - https://bugzilla.mozilla.org/show_bug.cgi?id=491668
        if (val <= 0 || val == null) {
            // Fall back to computed then uncomputed css if necessary
            val = curCSS(elem, name);
            if (val < 0 || val == null) {
                val = elem.style[name];
            }

            // Computed unit is not pixels. Stop here and return.
            if (rnumnonpx.test(val)) {
                return val;
            }

            // we need the check for style in case a browser which returns unreliable values
            // for getComputedStyle silently falls back to the reliable elem.style
            valueIsBorderBox = isBorderBox && (jQuery.support.boxSizingReliable || val === elem.style[name]);

            // Normalize "", auto, and prepare for extra
            val = parseFloat(val) || 0;
        }

        // use the active box-sizing model to add/subtract irrelevant styles
        return (val +
            augmentWidthOrHeight(
                elem,
                name,
                extra || (isBorderBox ? "border" : "content"),
                valueIsBorderBox
            )
        ) + "px";
    }


    // Try to determine the default display value of an element
    function css_defaultDisplay(nodeName) {
        if (elemdisplay[nodeName]) {
            return elemdisplay[nodeName];
        }

        var elem = jQuery("<" + nodeName + ">").appendTo(document.body),
            display = elem.css("display");
        elem.remove();

        // If the simple way fails,
        // get element's real default display by attaching it to a temp iframe
        if (display === "none" || display === "") {
            // Use the already-created iframe if possible
            iframe = document.body.appendChild(
                iframe || jQuery.extend(document.createElement("iframe"), {
                    frameBorder: 0,
                    width: 0,
                    height: 0
                })
            );

            // Create a cacheable copy of the iframe document on first call.
            // IE and Opera will allow us to reuse the iframeDoc without re-writing the fake HTML
            // document to it; WebKit & Firefox won't allow reusing the iframe document.
            if (!iframeDoc || !iframe.createElement) {
                iframeDoc = (iframe.contentWindow || iframe.contentDocument).document;
                iframeDoc.write("<!doctype html><html><body>");
                iframeDoc.close();
            }

            elem = iframeDoc.body.appendChild(iframeDoc.createElement(nodeName));

            display = curCSS(elem, "display");
            document.body.removeChild(iframe);
        }

        // Store the correct default display
        elemdisplay[nodeName] = display;

        return display;
    }

    jQuery.each(["height", "width"], function (i, name) {
        jQuery.cssHooks[name] = {
            get: function (elem, computed, extra) {
                if (computed) {
                    // certain elements can have dimension info if we invisibly show them
                    // however, it must have a current display style that would benefit from this
                    if (elem.offsetWidth === 0 && rdisplayswap.test(curCSS(elem, "display"))) {
                        return jQuery.swap(elem, cssShow, function () {
                            return getWidthOrHeight(elem, name, extra);
                        });
                    } else {
                        return getWidthOrHeight(elem, name, extra);
                    }
                }
            },

            set: function (elem, value, extra) {
                return setPositiveNumber(elem, value, extra ?
                    augmentWidthOrHeight(
                        elem,
                        name,
                        extra,
                        jQuery.support.boxSizing && jQuery.css(elem, "boxSizing") === "border-box"
                    ) : 0
                );
            }
        };
    });

    if (!jQuery.support.opacity) {
        jQuery.cssHooks.opacity = {
            get: function (elem, computed) {
                // IE uses filters for opacity
                return ropacity.test((computed && elem.currentStyle ? elem.currentStyle.filter : elem.style.filter) || "") ?
                    (0.01 * parseFloat(RegExp.$1)) + "" :
                    computed ? "1" : "";
            },

            set: function (elem, value) {
                var style = elem.style,
                    currentStyle = elem.currentStyle,
                    opacity = jQuery.isNumeric(value) ? "alpha(opacity=" + value * 100 + ")" : "",
                    filter = currentStyle && currentStyle.filter || style.filter || "";

                // IE has trouble with opacity if it does not have layout
                // Force it by setting the zoom level
                style.zoom = 1;

                // if setting opacity to 1, and no other filters exist - attempt to remove filter attribute #6652
                if (value >= 1 && jQuery.trim(filter.replace(ralpha, "")) === "" &&
                    style.removeAttribute) {

                    // Setting style.filter to null, "" & " " still leave "filter:" in the cssText
                    // if "filter:" is present at all, clearType is disabled, we want to avoid this
                    // style.removeAttribute is IE Only, but so apparently is this code path...
                    style.removeAttribute("filter");

                    // if there there is no filter style applied in a css rule, we are done
                    if (currentStyle && !currentStyle.filter) {
                        return;
                    }
                }

                // otherwise, set new filter values
                style.filter = ralpha.test(filter) ?
                    filter.replace(ralpha, opacity) :
                    filter + " " + opacity;
            }
        };
    }

    // These hooks cannot be added until DOM ready because the support test
    // for it is not run until after DOM ready
    jQuery(function () {
        if (!jQuery.support.reliableMarginRight) {
            jQuery.cssHooks.marginRight = {
                get: function (elem, computed) {
                    // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
                    // Work around by temporarily setting element display to inline-block
                    return jQuery.swap(elem, { "display": "inline-block" }, function () {
                        if (computed) {
                            return curCSS(elem, "marginRight");
                        }
                    });
                }
            };
        }

        // Webkit bug: https://bugs.webkit.org/show_bug.cgi?id=29084
        // getComputedStyle returns percent when specified for top/left/bottom/right
        // rather than make the css module depend on the offset module, we just check for it here
        if (!jQuery.support.pixelPosition && jQuery.fn.position) {
            jQuery.each(["top", "left"], function (i, prop) {
                jQuery.cssHooks[prop] = {
                    get: function (elem, computed) {
                        if (computed) {
                            var ret = curCSS(elem, prop);
                            // if curCSS returns percentage, fallback to offset
                            return rnumnonpx.test(ret) ? jQuery(elem).position()[prop] + "px" : ret;
                        }
                    }
                };
            });
        }

    });

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.hidden = function (elem) {
            return (elem.offsetWidth === 0 && elem.offsetHeight === 0) || (!jQuery.support.reliableHiddenOffsets && ((elem.style && elem.style.display) || curCSS(elem, "display")) === "none");
        };

        jQuery.expr.filters.visible = function (elem) {
            return !jQuery.expr.filters.hidden(elem);
        };
    }

    // These hooks are used by animate to expand properties
    jQuery.each({
        margin: "",
        padding: "",
        border: "Width"
    }, function (prefix, suffix) {
        jQuery.cssHooks[prefix + suffix] = {
            expand: function (value) {
                var i,

                    // assumes a single number if not a string
                    parts = typeof value === "string" ? value.split(" ") : [value],
                    expanded = {};

                for (i = 0; i < 4; i++) {
                    expanded[prefix + cssExpand[i] + suffix] =
                        parts[i] || parts[i - 2] || parts[0];
                }

                return expanded;
            }
        };

        if (!rmargin.test(prefix)) {
            jQuery.cssHooks[prefix + suffix].set = setPositiveNumber;
        }
    });
    var r20 = /%20/g,
        rbracket = /\[\]$/,
        rCRLF = /\r?\n/g,
        rinput = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i,
        rselectTextarea = /^(?:select|textarea)/i;

    jQuery.fn.extend({
        serialize: function () {
            return jQuery.param(this.serializeArray());
        },
        serializeArray: function () {
            return this.map(function () {
                return this.elements ? jQuery.makeArray(this.elements) : this;
            })
            .filter(function () {
                return this.name && !this.disabled &&
                    (this.checked || rselectTextarea.test(this.nodeName) ||
                        rinput.test(this.type));
            })
            .map(function (i, elem) {
                var val = jQuery(this).val();

                return val == null ?
                    null :
                    jQuery.isArray(val) ?
                        jQuery.map(val, function (val, i) {
                            return { name: elem.name, value: val.replace(rCRLF, "\r\n") };
                        }) :
					{ name: elem.name, value: val.replace(rCRLF, "\r\n") };
            }).get();
        }
    });

    //Serialize an array of form elements or a set of
    //key/values into a query string
    jQuery.param = function (a, traditional) {
        var prefix,
            s = [],
            add = function (key, value) {
                // If value is a function, invoke it and return its value
                value = jQuery.isFunction(value) ? value() : (value == null ? "" : value);
                s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
            };

        // Set traditional to true for jQuery <= 1.3.2 behavior.
        if (traditional === undefined) {
            traditional = jQuery.ajaxSettings && jQuery.ajaxSettings.traditional;
        }

        // If an array was passed in, assume that it is an array of form elements.
        if (jQuery.isArray(a) || (a.jquery && !jQuery.isPlainObject(a))) {
            // Serialize the form elements
            jQuery.each(a, function () {
                add(this.name, this.value);
            });

        } else {
            // If traditional, encode the "old" way (the way 1.3.2 or older
            // did it), otherwise encode params recursively.
            for (prefix in a) {
                buildParams(prefix, a[prefix], traditional, add);
            }
        }

        // Return the resulting serialization
        return s.join("&").replace(r20, "+");
    };

    function buildParams(prefix, obj, traditional, add) {
        var name;

        if (jQuery.isArray(obj)) {
            // Serialize array item.
            jQuery.each(obj, function (i, v) {
                if (traditional || rbracket.test(prefix)) {
                    // Treat each array item as a scalar.
                    add(prefix, v);

                } else {
                    // If array item is non-scalar (array or object), encode its
                    // numeric index to resolve deserialization ambiguity issues.
                    // Note that rack (as of 1.0.0) can't currently deserialize
                    // nested arrays properly, and attempting to do so may cause
                    // a server error. Possible fixes are to modify rack's
                    // deserialization algorithm or to provide an option or flag
                    // to force array serialization to be shallow.
                    buildParams(prefix + "[" + (typeof v === "object" ? i : "") + "]", v, traditional, add);
                }
            });

        } else if (!traditional && jQuery.type(obj) === "object") {
            // Serialize object item.
            for (name in obj) {
                buildParams(prefix + "[" + name + "]", obj[name], traditional, add);
            }

        } else {
            // Serialize scalar item.
            add(prefix, obj);
        }
    }
    var
        // Document location
        ajaxLocParts,
        ajaxLocation,

        rhash = /#.*$/,
        rheaders = /^(.*?):[ \t]*([^\r\n]*)\r?$/mg, // IE leaves an \r character at EOL
        // #7653, #8125, #8152: local protocol detection
        rlocalProtocol = /^(?:about|app|app\-storage|.+\-extension|file|res|widget):$/,
        rnoContent = /^(?:GET|HEAD)$/,
        rprotocol = /^\/\//,
        rquery = /\?/,
        rscript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi,
        rts = /([?&])_=[^&]*/,
        rurl = /^([\w\+\.\-]+:)(?:\/\/([^\/?#:]*)(?::(\d+)|)|)/,

        // Keep a copy of the old load method
        _load = jQuery.fn.load,

        /* Prefilters
         * 1) They are useful to introduce custom dataTypes (see ajax/jsonp.js for an example)
         * 2) These are called:
         *    - BEFORE asking for a transport
         *    - AFTER param serialization (s.data is a string if s.processData is true)
         * 3) key is the dataType
         * 4) the catchall symbol "*" can be used
         * 5) execution will start with transport dataType and THEN continue down to "*" if needed
         */
        prefilters = {},

        /* Transports bindings
         * 1) key is the dataType
         * 2) the catchall symbol "*" can be used
         * 3) selection will start with transport dataType and THEN go to "*" if needed
         */
        transports = {},

        // Avoid comment-prolog char sequence (#10098); must appease lint and evade compression
        allTypes = ["*/"] + ["*"];

    // #8138, IE may throw an exception when accessing
    // a field from window.location if document.domain has been set
    try {
        ajaxLocation = location.href;
    } catch (e) {
        // Use the href attribute of an A element
        // since IE will modify it given document.location
        ajaxLocation = document.createElement("a");
        ajaxLocation.href = "";
        ajaxLocation = ajaxLocation.href;
    }

    // Segment location into parts
    ajaxLocParts = rurl.exec(ajaxLocation.toLowerCase()) || [];

    // Base "constructor" for jQuery.ajaxPrefilter and jQuery.ajaxTransport
    function addToPrefiltersOrTransports(structure) {

        // dataTypeExpression is optional and defaults to "*"
        return function (dataTypeExpression, func) {

            if (typeof dataTypeExpression !== "string") {
                func = dataTypeExpression;
                dataTypeExpression = "*";
            }

            var dataType, list, placeBefore,
                dataTypes = dataTypeExpression.toLowerCase().split(core_rspace),
                i = 0,
                length = dataTypes.length;

            if (jQuery.isFunction(func)) {
                // For each dataType in the dataTypeExpression
                for (; i < length; i++) {
                    dataType = dataTypes[i];
                    // We control if we're asked to add before
                    // any existing element
                    placeBefore = /^\+/.test(dataType);
                    if (placeBefore) {
                        dataType = dataType.substr(1) || "*";
                    }
                    list = structure[dataType] = structure[dataType] || [];
                    // then we add to the structure accordingly
                    list[placeBefore ? "unshift" : "push"](func);
                }
            }
        };
    }

    // Base inspection function for prefilters and transports
    function inspectPrefiltersOrTransports(structure, options, originalOptions, jqXHR,
            dataType /* internal */, inspected /* internal */) {

        dataType = dataType || options.dataTypes[0];
        inspected = inspected || {};

        inspected[dataType] = true;

        var selection,
            list = structure[dataType],
            i = 0,
            length = list ? list.length : 0,
            executeOnly = (structure === prefilters);

        for (; i < length && (executeOnly || !selection) ; i++) {
            selection = list[i](options, originalOptions, jqXHR);
            // If we got redirected to another dataType
            // we try there if executing only and not done already
            if (typeof selection === "string") {
                if (!executeOnly || inspected[selection]) {
                    selection = undefined;
                } else {
                    options.dataTypes.unshift(selection);
                    selection = inspectPrefiltersOrTransports(
                            structure, options, originalOptions, jqXHR, selection, inspected);
                }
            }
        }
        // If we're only executing or nothing was selected
        // we try the catchall dataType if not done already
        if ((executeOnly || !selection) && !inspected["*"]) {
            selection = inspectPrefiltersOrTransports(
                    structure, options, originalOptions, jqXHR, "*", inspected);
        }
        // unnecessary when only executing (prefilters)
        // but it'll be ignored by the caller in that case
        return selection;
    }

    // A special extend for ajax options
    // that takes "flat" options (not to be deep extended)
    // Fixes #9887
    function ajaxExtend(target, src) {
        var key, deep,
            flatOptions = jQuery.ajaxSettings.flatOptions || {};
        for (key in src) {
            if (src[key] !== undefined) {
                (flatOptions[key] ? target : (deep || (deep = {})))[key] = src[key];
            }
        }
        if (deep) {
            jQuery.extend(true, target, deep);
        }
    }

    jQuery.fn.load = function (url, params, callback) {
        if (typeof url !== "string" && _load) {
            return _load.apply(this, arguments);
        }

        // Don't do a request if no elements are being requested
        if (!this.length) {
            return this;
        }

        var selector, type, response,
            self = this,
            off = url.indexOf(" ");

        if (off >= 0) {
            selector = url.slice(off, url.length);
            url = url.slice(0, off);
        }

        // If it's a function
        if (jQuery.isFunction(params)) {

            // We assume that it's the callback
            callback = params;
            params = undefined;

            // Otherwise, build a param string
        } else if (params && typeof params === "object") {
            type = "POST";
        }

        // Request the remote document
        jQuery.ajax({
            url: url,

            // if "type" variable is undefined, then "GET" method will be used
            type: type,
            dataType: "html",
            data: params,
            complete: function (jqXHR, status) {
                if (callback) {
                    self.each(callback, response || [jqXHR.responseText, status, jqXHR]);
                }
            }
        }).done(function (responseText) {

            // Save response for use in complete callback
            response = arguments;

            // See if a selector was specified
            self.html(selector ?

                // Create a dummy div to hold the results
                jQuery("<div>")

                    // inject the contents of the document in, removing the scripts
                    // to avoid any 'Permission Denied' errors in IE
                    .append(responseText.replace(rscript, ""))

                    // Locate the specified elements
                    .find(selector) :

                // If not, just inject the full result
                responseText);

        });

        return this;
    };

    // Attach a bunch of functions for handling common AJAX events
    jQuery.each("ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess ajaxSend".split(" "), function (i, o) {
        jQuery.fn[o] = function (f) {
            return this.on(o, f);
        };
    });

    jQuery.each(["get", "post"], function (i, method) {
        jQuery[method] = function (url, data, callback, type) {
            // shift arguments if data argument was omitted
            if (jQuery.isFunction(data)) {
                type = type || callback;
                callback = data;
                data = undefined;
            }

            return jQuery.ajax({
                type: method,
                url: url,
                data: data,
                success: callback,
                dataType: type
            });
        };
    });

    jQuery.extend({

        getScript: function (url, callback) {
            return jQuery.get(url, undefined, callback, "script");
        },

        getJSON: function (url, data, callback) {
            return jQuery.get(url, data, callback, "json");
        },

        // Creates a full fledged settings object into target
        // with both ajaxSettings and settings fields.
        // If target is omitted, writes into ajaxSettings.
        ajaxSetup: function (target, settings) {
            if (settings) {
                // Building a settings object
                ajaxExtend(target, jQuery.ajaxSettings);
            } else {
                // Extending ajaxSettings
                settings = target;
                target = jQuery.ajaxSettings;
            }
            ajaxExtend(target, settings);
            return target;
        },

        ajaxSettings: {
            url: ajaxLocation,
            isLocal: rlocalProtocol.test(ajaxLocParts[1]),
            global: true,
            type: "GET",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            processData: true,
            async: true,
            /*
            timeout: 0,
            data: null,
            dataType: null,
            username: null,
            password: null,
            cache: null,
            throws: false,
            traditional: false,
            headers: {},
            */

            accepts: {
                xml: "application/xml, text/xml",
                html: "text/html",
                text: "text/plain",
                json: "application/json, text/javascript",
                "*": allTypes
            },

            contents: {
                xml: /xml/,
                html: /html/,
                json: /json/
            },

            responseFields: {
                xml: "responseXML",
                text: "responseText"
            },

            // List of data converters
            // 1) key format is "source_type destination_type" (a single space in-between)
            // 2) the catchall symbol "*" can be used for source_type
            converters: {

                // Convert anything to text
                "* text": window.String,

                // Text to html (true = no transformation)
                "text html": true,

                // Evaluate text as a json expression
                "text json": jQuery.parseJSON,

                // Parse text as xml
                "text xml": jQuery.parseXML
            },

            // For options that shouldn't be deep extended:
            // you can add your own custom options here if
            // and when you create one that shouldn't be
            // deep extended (see ajaxExtend)
            flatOptions: {
                context: true,
                url: true
            }
        },

        ajaxPrefilter: addToPrefiltersOrTransports(prefilters),
        ajaxTransport: addToPrefiltersOrTransports(transports),

        // Main method
        ajax: function (url, options) {

            // If url is an object, simulate pre-1.5 signature
            if (typeof url === "object") {
                options = url;
                url = undefined;
            }

            // Force options to be an object
            options = options || {};

            var // ifModified key
                ifModifiedKey,
                // Response headers
                responseHeadersString,
                responseHeaders,
                // transport
                transport,
                // timeout handle
                timeoutTimer,
                // Cross-domain detection vars
                parts,
                // To know if global events are to be dispatched
                fireGlobals,
                // Loop variable
                i,
                // Create the final options object
                s = jQuery.ajaxSetup({}, options),
                // Callbacks context
                callbackContext = s.context || s,
                // Context for global events
                // It's the callbackContext if one was provided in the options
                // and if it's a DOM node or a jQuery collection
                globalEventContext = callbackContext !== s &&
                    (callbackContext.nodeType || callbackContext instanceof jQuery) ?
                            jQuery(callbackContext) : jQuery.event,
                // Deferreds
                deferred = jQuery.Deferred(),
                completeDeferred = jQuery.Callbacks("once memory"),
                // Status-dependent callbacks
                statusCode = s.statusCode || {},
                // Headers (they are sent all at once)
                requestHeaders = {},
                requestHeadersNames = {},
                // The jqXHR state
                state = 0,
                // Default abort message
                strAbort = "canceled",
                // Fake xhr
                jqXHR = {

                    readyState: 0,

                    // Caches the header
                    setRequestHeader: function (name, value) {
                        if (!state) {
                            var lname = name.toLowerCase();
                            name = requestHeadersNames[lname] = requestHeadersNames[lname] || name;
                            requestHeaders[name] = value;
                        }
                        return this;
                    },

                    // Raw string
                    getAllResponseHeaders: function () {
                        return state === 2 ? responseHeadersString : null;
                    },

                    // Builds headers hashtable if needed
                    getResponseHeader: function (key) {
                        var match;
                        if (state === 2) {
                            if (!responseHeaders) {
                                responseHeaders = {};
                                while ((match = rheaders.exec(responseHeadersString))) {
                                    responseHeaders[match[1].toLowerCase()] = match[2];
                                }
                            }
                            match = responseHeaders[key.toLowerCase()];
                        }
                        return match === undefined ? null : match;
                    },

                    // Overrides response content-type header
                    overrideMimeType: function (type) {
                        if (!state) {
                            s.mimeType = type;
                        }
                        return this;
                    },

                    // Cancel the request
                    abort: function (statusText) {
                        statusText = statusText || strAbort;
                        if (transport) {
                            transport.abort(statusText);
                        }
                        done(0, statusText);
                        return this;
                    }
                };

            // Callback for when everything is done
            // It is defined here because jslint complains if it is declared
            // at the end of the function (which would be more logical and readable)
            function done(status, nativeStatusText, responses, headers) {
                var isSuccess, success, error, response, modified,
                    statusText = nativeStatusText;

                // Called once
                if (state === 2) {
                    return;
                }

                // State is "done" now
                state = 2;

                // Clear timeout if it exists
                if (timeoutTimer) {
                    clearTimeout(timeoutTimer);
                }

                // Dereference transport for early garbage collection
                // (no matter how long the jqXHR object will be used)
                transport = undefined;

                // Cache response headers
                responseHeadersString = headers || "";

                // Set readyState
                jqXHR.readyState = status > 0 ? 4 : 0;

                // Get response data
                if (responses) {
                    response = ajaxHandleResponses(s, jqXHR, responses);
                }

                // If successful, handle type chaining
                if (status >= 200 && status < 300 || status === 304) {

                    // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
                    if (s.ifModified) {

                        modified = jqXHR.getResponseHeader("Last-Modified");
                        if (modified) {
                            jQuery.lastModified[ifModifiedKey] = modified;
                        }
                        modified = jqXHR.getResponseHeader("Etag");
                        if (modified) {
                            jQuery.etag[ifModifiedKey] = modified;
                        }
                    }

                    // If not modified
                    if (status === 304) {

                        statusText = "notmodified";
                        isSuccess = true;

                        // If we have data
                    } else {

                        isSuccess = ajaxConvert(s, response);
                        statusText = isSuccess.state;
                        success = isSuccess.data;
                        error = isSuccess.error;
                        isSuccess = !error;
                    }
                } else {
                    // We extract error from statusText
                    // then normalize statusText and status for non-aborts
                    error = statusText;
                    if (!statusText || status) {
                        statusText = "error";
                        if (status < 0) {
                            status = 0;
                        }
                    }
                }

                // Set data for the fake xhr object
                jqXHR.status = status;
                jqXHR.statusText = (nativeStatusText || statusText) + "";

                // Success/Error
                if (isSuccess) {
                    deferred.resolveWith(callbackContext, [success, statusText, jqXHR]);
                } else {
                    deferred.rejectWith(callbackContext, [jqXHR, statusText, error]);
                }

                // Status-dependent callbacks
                jqXHR.statusCode(statusCode);
                statusCode = undefined;

                if (fireGlobals) {
                    globalEventContext.trigger("ajax" + (isSuccess ? "Success" : "Error"),
                            [jqXHR, s, isSuccess ? success : error]);
                }

                // Complete
                completeDeferred.fireWith(callbackContext, [jqXHR, statusText]);

                if (fireGlobals) {
                    globalEventContext.trigger("ajaxComplete", [jqXHR, s]);
                    // Handle the global AJAX counter
                    if (!(--jQuery.active)) {
                        jQuery.event.trigger("ajaxStop");
                    }
                }
            }

            // Attach deferreds
            deferred.promise(jqXHR);
            jqXHR.success = jqXHR.done;
            jqXHR.error = jqXHR.fail;
            jqXHR.complete = completeDeferred.add;

            // Status-dependent callbacks
            jqXHR.statusCode = function (map) {
                if (map) {
                    var tmp;
                    if (state < 2) {
                        for (tmp in map) {
                            statusCode[tmp] = [statusCode[tmp], map[tmp]];
                        }
                    } else {
                        tmp = map[jqXHR.status];
                        jqXHR.always(tmp);
                    }
                }
                return this;
            };

            // Remove hash character (#7531: and string promotion)
            // Add protocol if not provided (#5866: IE7 issue with protocol-less urls)
            // We also use the url parameter if available
            s.url = ((url || s.url) + "").replace(rhash, "").replace(rprotocol, ajaxLocParts[1] + "//");

            // Extract dataTypes list
            s.dataTypes = jQuery.trim(s.dataType || "*").toLowerCase().split(core_rspace);

            // A cross-domain request is in order when we have a protocol:host:port mismatch
            if (s.crossDomain == null) {
                parts = rurl.exec(s.url.toLowerCase());
                s.crossDomain = !!(parts &&
                    (parts[1] !== ajaxLocParts[1] || parts[2] !== ajaxLocParts[2] ||
                        (parts[3] || (parts[1] === "http:" ? 80 : 443)) !=
                            (ajaxLocParts[3] || (ajaxLocParts[1] === "http:" ? 80 : 443)))
                );
            }

            // Convert data if not already a string
            if (s.data && s.processData && typeof s.data !== "string") {
                s.data = jQuery.param(s.data, s.traditional);
            }

            // Apply prefilters
            inspectPrefiltersOrTransports(prefilters, s, options, jqXHR);

            // If request was aborted inside a prefilter, stop there
            if (state === 2) {
                return jqXHR;
            }

            // We can fire global events as of now if asked to
            fireGlobals = s.global;

            // Uppercase the type
            s.type = s.type.toUpperCase();

            // Determine if request has content
            s.hasContent = !rnoContent.test(s.type);

            // Watch for a new set of requests
            if (fireGlobals && jQuery.active++ === 0) {
                jQuery.event.trigger("ajaxStart");
            }

            // More options handling for requests with no content
            if (!s.hasContent) {

                // If data is available, append data to url
                if (s.data) {
                    s.url += (rquery.test(s.url) ? "&" : "?") + s.data;
                    // #9682: remove data so that it's not used in an eventual retry
                    delete s.data;
                }

                // Get ifModifiedKey before adding the anti-cache parameter
                ifModifiedKey = s.url;

                // Add anti-cache in url if needed
                if (s.cache === false) {

                    var ts = jQuery.now(),
                        // try replacing _= if it is there
                        ret = s.url.replace(rts, "$1_=" + ts);

                    // if nothing was replaced, add timestamp to the end
                    s.url = ret + ((ret === s.url) ? (rquery.test(s.url) ? "&" : "?") + "_=" + ts : "");
                }
            }

            // Set the correct header, if data is being sent
            if (s.data && s.hasContent && s.contentType !== false || options.contentType) {
                jqXHR.setRequestHeader("Content-Type", s.contentType);
            }

            // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
            if (s.ifModified) {
                ifModifiedKey = ifModifiedKey || s.url;
                if (jQuery.lastModified[ifModifiedKey]) {
                    jqXHR.setRequestHeader("If-Modified-Since", jQuery.lastModified[ifModifiedKey]);
                }
                if (jQuery.etag[ifModifiedKey]) {
                    jqXHR.setRequestHeader("If-None-Match", jQuery.etag[ifModifiedKey]);
                }
            }

            // Set the Accepts header for the server, depending on the dataType
            jqXHR.setRequestHeader(
                "Accept",
                s.dataTypes[0] && s.accepts[s.dataTypes[0]] ?
                    s.accepts[s.dataTypes[0]] + (s.dataTypes[0] !== "*" ? ", " + allTypes + "; q=0.01" : "") :
                    s.accepts["*"]
            );

            // Check for headers option
            for (i in s.headers) {
                jqXHR.setRequestHeader(i, s.headers[i]);
            }

            // Allow custom headers/mimetypes and early abort
            if (s.beforeSend && (s.beforeSend.call(callbackContext, jqXHR, s) === false || state === 2)) {
                // Abort if not done already and return
                return jqXHR.abort();

            }

            // aborting is no longer a cancellation
            strAbort = "abort";

            // Install callbacks on deferreds
            for (i in { success: 1, error: 1, complete: 1 }) {
                jqXHR[i](s[i]);
            }

            // Get transport
            transport = inspectPrefiltersOrTransports(transports, s, options, jqXHR);

            // If no transport, we auto-abort
            if (!transport) {
                done(-1, "No Transport");
            } else {
                jqXHR.readyState = 1;
                // Send global event
                if (fireGlobals) {
                    globalEventContext.trigger("ajaxSend", [jqXHR, s]);
                }
                // Timeout
                if (s.async && s.timeout > 0) {
                    timeoutTimer = setTimeout(function () {
                        jqXHR.abort("timeout");
                    }, s.timeout);
                }

                try {
                    state = 1;
                    transport.send(requestHeaders, done);
                } catch (e) {
                    // Propagate exception as error if not done
                    if (state < 2) {
                        done(-1, e);
                        // Simply rethrow otherwise
                    } else {
                        throw e;
                    }
                }
            }

            return jqXHR;
        },

        // Counter for holding the number of active queries
        active: 0,

        // Last-Modified header cache for next request
        lastModified: {},
        etag: {}

    });

    /* Handles responses to an ajax request:
     * - sets all responseXXX fields accordingly
     * - finds the right dataType (mediates between content-type and expected dataType)
     * - returns the corresponding response
     */
    function ajaxHandleResponses(s, jqXHR, responses) {

        var ct, type, finalDataType, firstDataType,
            contents = s.contents,
            dataTypes = s.dataTypes,
            responseFields = s.responseFields;

        // Fill responseXXX fields
        for (type in responseFields) {
            if (type in responses) {
                jqXHR[responseFields[type]] = responses[type];
            }
        }

        // Remove auto dataType and get content-type in the process
        while (dataTypes[0] === "*") {
            dataTypes.shift();
            if (ct === undefined) {
                ct = s.mimeType || jqXHR.getResponseHeader("content-type");
            }
        }

        // Check if we're dealing with a known content-type
        if (ct) {
            for (type in contents) {
                if (contents[type] && contents[type].test(ct)) {
                    dataTypes.unshift(type);
                    break;
                }
            }
        }

        // Check to see if we have a response for the expected dataType
        if (dataTypes[0] in responses) {
            finalDataType = dataTypes[0];
        } else {
            // Try convertible dataTypes
            for (type in responses) {
                if (!dataTypes[0] || s.converters[type + " " + dataTypes[0]]) {
                    finalDataType = type;
                    break;
                }
                if (!firstDataType) {
                    firstDataType = type;
                }
            }
            // Or just use first one
            finalDataType = finalDataType || firstDataType;
        }

        // If we found a dataType
        // We add the dataType to the list if needed
        // and return the corresponding response
        if (finalDataType) {
            if (finalDataType !== dataTypes[0]) {
                dataTypes.unshift(finalDataType);
            }
            return responses[finalDataType];
        }
    }

    // Chain conversions given the request and the original response
    function ajaxConvert(s, response) {

        var conv, conv2, current, tmp,
            // Work with a copy of dataTypes in case we need to modify it for conversion
            dataTypes = s.dataTypes.slice(),
            prev = dataTypes[0],
            converters = {},
            i = 0;

        // Apply the dataFilter if provided
        if (s.dataFilter) {
            response = s.dataFilter(response, s.dataType);
        }

        // Create converters map with lowercased keys
        if (dataTypes[1]) {
            for (conv in s.converters) {
                converters[conv.toLowerCase()] = s.converters[conv];
            }
        }

        // Convert to each sequential dataType, tolerating list modification
        for (; (current = dataTypes[++i]) ;) {

            // There's only work to do if current dataType is non-auto
            if (current !== "*") {

                // Convert response if prev dataType is non-auto and differs from current
                if (prev !== "*" && prev !== current) {

                    // Seek a direct converter
                    conv = converters[prev + " " + current] || converters["* " + current];

                    // If none found, seek a pair
                    if (!conv) {
                        for (conv2 in converters) {

                            // If conv2 outputs current
                            tmp = conv2.split(" ");
                            if (tmp[1] === current) {

                                // If prev can be converted to accepted input
                                conv = converters[prev + " " + tmp[0]] ||
                                    converters["* " + tmp[0]];
                                if (conv) {
                                    // Condense equivalence converters
                                    if (conv === true) {
                                        conv = converters[conv2];

                                        // Otherwise, insert the intermediate dataType
                                    } else if (converters[conv2] !== true) {
                                        current = tmp[0];
                                        dataTypes.splice(i--, 0, current);
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    // Apply converter (if not an equivalence)
                    if (conv !== true) {

                        // Unless errors are allowed to bubble, catch and return them
                        if (conv && s["throws"]) {
                            response = conv(response);
                        } else {
                            try {
                                response = conv(response);
                            } catch (e) {
                                return { state: "parsererror", error: conv ? e : "No conversion from " + prev + " to " + current };
                            }
                        }
                    }
                }

                // Update prev for next iteration
                prev = current;
            }
        }

        return { state: "success", data: response };
    }
    var oldCallbacks = [],
        rquestion = /\?/,
        rjsonp = /(=)\?(?=&|$)|\?\?/,
        nonce = jQuery.now();

    // Default jsonp settings
    jQuery.ajaxSetup({
        jsonp: "callback",
        jsonpCallback: function () {
            var callback = oldCallbacks.pop() || (jQuery.expando + "_" + (nonce++));
            this[callback] = true;
            return callback;
        }
    });

    // Detect, normalize options and install callbacks for jsonp requests
    jQuery.ajaxPrefilter("json jsonp", function (s, originalSettings, jqXHR) {

        var callbackName, overwritten, responseContainer,
            data = s.data,
            url = s.url,
            hasCallback = s.jsonp !== false,
            replaceInUrl = hasCallback && rjsonp.test(url),
            replaceInData = hasCallback && !replaceInUrl && typeof data === "string" &&
                !(s.contentType || "").indexOf("application/x-www-form-urlencoded") &&
                rjsonp.test(data);

        // Handle iff the expected data type is "jsonp" or we have a parameter to set
        if (s.dataTypes[0] === "jsonp" || replaceInUrl || replaceInData) {

            // Get callback name, remembering preexisting value associated with it
            callbackName = s.jsonpCallback = jQuery.isFunction(s.jsonpCallback) ?
                s.jsonpCallback() :
                s.jsonpCallback;
            overwritten = window[callbackName];

            // Insert callback into url or form data
            if (replaceInUrl) {
                s.url = url.replace(rjsonp, "$1" + callbackName);
            } else if (replaceInData) {
                s.data = data.replace(rjsonp, "$1" + callbackName);
            } else if (hasCallback) {
                s.url += (rquestion.test(url) ? "&" : "?") + s.jsonp + "=" + callbackName;
            }

            // Use data converter to retrieve json after script execution
            s.converters["script json"] = function () {
                if (!responseContainer) {
                    jQuery.error(callbackName + " was not called");
                }
                return responseContainer[0];
            };

            // force json dataType
            s.dataTypes[0] = "json";

            // Install callback
            window[callbackName] = function () {
                responseContainer = arguments;
            };

            // Clean-up function (fires after converters)
            jqXHR.always(function () {
                // Restore preexisting value
                window[callbackName] = overwritten;

                // Save back as free
                if (s[callbackName]) {
                    // make sure that re-using the options doesn't screw things around
                    s.jsonpCallback = originalSettings.jsonpCallback;

                    // save the callback name for future use
                    oldCallbacks.push(callbackName);
                }

                // Call if it was a function and we have a response
                if (responseContainer && jQuery.isFunction(overwritten)) {
                    overwritten(responseContainer[0]);
                }

                responseContainer = overwritten = undefined;
            });

            // Delegate to script
            return "script";
        }
    });
    // Install script dataType
    jQuery.ajaxSetup({
        accepts: {
            script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"
        },
        contents: {
            script: /javascript|ecmascript/
        },
        converters: {
            "text script": function (text) {
                jQuery.globalEval(text);
                return text;
            }
        }
    });

    // Handle cache's special case and global
    jQuery.ajaxPrefilter("script", function (s) {
        if (s.cache === undefined) {
            s.cache = false;
        }
        if (s.crossDomain) {
            s.type = "GET";
            s.global = false;
        }
    });

    // Bind script tag hack transport
    jQuery.ajaxTransport("script", function (s) {

        // This transport only deals with cross domain requests
        if (s.crossDomain) {

            var script,
                head = document.head || document.getElementsByTagName("head")[0] || document.documentElement;

            return {

                send: function (_, callback) {

                    script = document.createElement("script");

                    script.async = "async";

                    if (s.scriptCharset) {
                        script.charset = s.scriptCharset;
                    }

                    script.src = s.url;

                    // Attach handlers for all browsers
                    script.onload = script.onreadystatechange = function (_, isAbort) {

                        if (isAbort || !script.readyState || /loaded|complete/.test(script.readyState)) {

                            // Handle memory leak in IE
                            script.onload = script.onreadystatechange = null;

                            // Remove the script
                            if (head && script.parentNode) {
                                head.removeChild(script);
                            }

                            // Dereference the script
                            script = undefined;

                            // Callback if not abort
                            if (!isAbort) {
                                callback(200, "success");
                            }
                        }
                    };
                    // Use insertBefore instead of appendChild  to circumvent an IE6 bug.
                    // This arises when a base node is used (#2709 and #4378).
                    head.insertBefore(script, head.firstChild);
                },

                abort: function () {
                    if (script) {
                        script.onload(0, 1);
                    }
                }
            };
        }
    });
    var xhrCallbacks,
        // #5280: Internet Explorer will keep connections alive if we don't abort on unload
        xhrOnUnloadAbort = window.ActiveXObject ? function () {
            // Abort all pending requests
            for (var key in xhrCallbacks) {
                xhrCallbacks[key](0, 1);
            }
        } : false,
        xhrId = 0;

    // Functions to create xhrs
    function createStandardXHR() {
        try {
            return new window.XMLHttpRequest();
        } catch (e) { }
    }

    function createActiveXHR() {
        try {
            return new window.ActiveXObject("Microsoft.XMLHTTP");
        } catch (e) { }
    }

    // Create the request object
    // (This is still attached to ajaxSettings for backward compatibility)
    jQuery.ajaxSettings.xhr = window.ActiveXObject ?
        /* Microsoft failed to properly
         * implement the XMLHttpRequest in IE7 (can't request local files),
         * so we use the ActiveXObject when it is available
         * Additionally XMLHttpRequest can be disabled in IE7/IE8 so
         * we need a fallback.
         */
        function () {
            return !this.isLocal && createStandardXHR() || createActiveXHR();
        } :
        // For all other browsers, use the standard XMLHttpRequest object
        createStandardXHR;

    // Determine support properties
    (function (xhr) {
        jQuery.extend(jQuery.support, {
            ajax: !!xhr,
            cors: !!xhr && ("withCredentials" in xhr)
        });
    })(jQuery.ajaxSettings.xhr());

    // Create transport if the browser can provide an xhr
    if (jQuery.support.ajax) {

        jQuery.ajaxTransport(function (s) {
            // Cross domain only allowed if supported through XMLHttpRequest
            if (!s.crossDomain || jQuery.support.cors) {

                var callback;

                return {
                    send: function (headers, complete) {

                        // Get a new xhr
                        var handle, i,
                            xhr = s.xhr();

                        // Open the socket
                        // Passing null username, generates a login popup on Opera (#2865)
                        if (s.username) {
                            xhr.open(s.type, s.url, s.async, s.username, s.password);
                        } else {
                            xhr.open(s.type, s.url, s.async);
                        }

                        // Apply custom fields if provided
                        if (s.xhrFields) {
                            for (i in s.xhrFields) {
                                xhr[i] = s.xhrFields[i];
                            }
                        }

                        // Override mime type if needed
                        if (s.mimeType && xhr.overrideMimeType) {
                            xhr.overrideMimeType(s.mimeType);
                        }

                        // X-Requested-With header
                        // For cross-domain requests, seeing as conditions for a preflight are
                        // akin to a jigsaw puzzle, we simply never set it to be sure.
                        // (it can always be set on a per-request basis or even using ajaxSetup)
                        // For same-domain requests, won't change header if already provided.
                        if (!s.crossDomain && !headers["X-Requested-With"]) {
                            headers["X-Requested-With"] = "XMLHttpRequest";
                        }

                        // Need an extra try/catch for cross domain requests in Firefox 3
                        try {
                            for (i in headers) {
                                xhr.setRequestHeader(i, headers[i]);
                            }
                        } catch (_) { }

                        // Do send the request
                        // This may raise an exception which is actually
                        // handled in jQuery.ajax (so no try/catch here)
                        xhr.send((s.hasContent && s.data) || null);

                        // Listener
                        callback = function (_, isAbort) {

                            var status,
                                statusText,
                                responseHeaders,
                                responses,
                                xml;

                            // Firefox throws exceptions when accessing properties
                            // of an xhr when a network error occurred
                            // http://helpful.knobs-dials.com/index.php/Component_returned_failure_code:_0x80040111_(NS_ERROR_NOT_AVAILABLE)
                            try {

                                // Was never called and is aborted or complete
                                if (callback && (isAbort || xhr.readyState === 4)) {

                                    // Only called once
                                    callback = undefined;

                                    // Do not keep as active anymore
                                    if (handle) {
                                        xhr.onreadystatechange = jQuery.noop;
                                        if (xhrOnUnloadAbort) {
                                            delete xhrCallbacks[handle];
                                        }
                                    }

                                    // If it's an abort
                                    if (isAbort) {
                                        // Abort it manually if needed
                                        if (xhr.readyState !== 4) {
                                            xhr.abort();
                                        }
                                    } else {
                                        status = xhr.status;
                                        responseHeaders = xhr.getAllResponseHeaders();
                                        responses = {};
                                        xml = xhr.responseXML;

                                        // Construct response list
                                        if (xml && xml.documentElement /* #4958 */) {
                                            responses.xml = xml;
                                        }

                                        // When requesting binary data, IE6-9 will throw an exception
                                        // on any attempt to access responseText (#11426)
                                        try {
                                            responses.text = xhr.responseText;
                                        } catch (e) {
                                        }

                                        // Firefox throws an exception when accessing
                                        // statusText for faulty cross-domain requests
                                        try {
                                            statusText = xhr.statusText;
                                        } catch (e) {
                                            // We normalize with Webkit giving an empty statusText
                                            statusText = "";
                                        }

                                        // Filter status for non standard behaviors

                                        // If the request is local and we have data: assume a success
                                        // (success with no data won't get notified, that's the best we
                                        // can do given current implementations)
                                        if (!status && s.isLocal && !s.crossDomain) {
                                            status = responses.text ? 200 : 404;
                                            // IE - #1450: sometimes returns 1223 when it should be 204
                                        } else if (status === 1223) {
                                            status = 204;
                                        }
                                    }
                                }
                            } catch (firefoxAccessException) {
                                if (!isAbort) {
                                    complete(-1, firefoxAccessException);
                                }
                            }

                            // Call complete if needed
                            if (responses) {
                                complete(status, statusText, responses, responseHeaders);
                            }
                        };

                        if (!s.async) {
                            // if we're in sync mode we fire the callback
                            callback();
                        } else if (xhr.readyState === 4) {
                            // (IE6 & IE7) if it's in cache and has been
                            // retrieved directly we need to fire the callback
                            setTimeout(callback, 0);
                        } else {
                            handle = ++xhrId;
                            if (xhrOnUnloadAbort) {
                                // Create the active xhrs callbacks list if needed
                                // and attach the unload handler
                                if (!xhrCallbacks) {
                                    xhrCallbacks = {};
                                    jQuery(window).unload(xhrOnUnloadAbort);
                                }
                                // Add to list of active xhrs callbacks
                                xhrCallbacks[handle] = callback;
                            }
                            xhr.onreadystatechange = callback;
                        }
                    },

                    abort: function () {
                        if (callback) {
                            callback(0, 1);
                        }
                    }
                };
            }
        });
    }
    var fxNow, timerId,
        rfxtypes = /^(?:toggle|show|hide)$/,
        rfxnum = new RegExp("^(?:([-+])=|)(" + core_pnum + ")([a-z%]*)$", "i"),
        rrun = /queueHooks$/,
        animationPrefilters = [defaultPrefilter],
        tweeners = {
            "*": [function (prop, value) {
                var end, unit,
                    tween = this.createTween(prop, value),
                    parts = rfxnum.exec(value),
                    target = tween.cur(),
                    start = +target || 0,
                    scale = 1,
                    maxIterations = 20;

                if (parts) {
                    end = +parts[2];
                    unit = parts[3] || (jQuery.cssNumber[prop] ? "" : "px");

                    // We need to compute starting value
                    if (unit !== "px" && start) {
                        // Iteratively approximate from a nonzero starting point
                        // Prefer the current property, because this process will be trivial if it uses the same units
                        // Fallback to end or a simple constant
                        start = jQuery.css(tween.elem, prop, true) || end || 1;

                        do {
                            // If previous iteration zeroed out, double until we get *something*
                            // Use a string for doubling factor so we don't accidentally see scale as unchanged below
                            scale = scale || ".5";

                            // Adjust and apply
                            start = start / scale;
                            jQuery.style(tween.elem, prop, start + unit);

                            // Update scale, tolerating zero or NaN from tween.cur()
                            // And breaking the loop if scale is unchanged or perfect, or if we've just had enough
                        } while (scale !== (scale = tween.cur() / target) && scale !== 1 && --maxIterations);
                    }

                    tween.unit = unit;
                    tween.start = start;
                    // If a +=/-= token was provided, we're doing a relative animation
                    tween.end = parts[1] ? start + (parts[1] + 1) * end : end;
                }
                return tween;
            }]
        };

    // Animations created synchronously will run synchronously
    function createFxNow() {
        setTimeout(function () {
            fxNow = undefined;
        }, 0);
        return (fxNow = jQuery.now());
    }

    function createTweens(animation, props) {
        jQuery.each(props, function (prop, value) {
            var collection = (tweeners[prop] || []).concat(tweeners["*"]),
                index = 0,
                length = collection.length;
            for (; index < length; index++) {
                if (collection[index].call(animation, prop, value)) {

                    // we're done with this property
                    return;
                }
            }
        });
    }

    function Animation(elem, properties, options) {
        var result,
            index = 0,
            tweenerIndex = 0,
            length = animationPrefilters.length,
            deferred = jQuery.Deferred().always(function () {
                // don't match elem in the :animated selector
                delete tick.elem;
            }),
            tick = function () {
                var currentTime = fxNow || createFxNow(),
                    remaining = Math.max(0, animation.startTime + animation.duration - currentTime),
                    // archaic crash bug won't allow us to use 1 - ( 0.5 || 0 ) (#12497)
                    temp = remaining / animation.duration || 0,
                    percent = 1 - temp,
                    index = 0,
                    length = animation.tweens.length;

                for (; index < length ; index++) {
                    animation.tweens[index].run(percent);
                }

                deferred.notifyWith(elem, [animation, percent, remaining]);

                if (percent < 1 && length) {
                    return remaining;
                } else {
                    deferred.resolveWith(elem, [animation]);
                    return false;
                }
            },
            animation = deferred.promise({
                elem: elem,
                props: jQuery.extend({}, properties),
                opts: jQuery.extend(true, { specialEasing: {} }, options),
                originalProperties: properties,
                originalOptions: options,
                startTime: fxNow || createFxNow(),
                duration: options.duration,
                tweens: [],
                createTween: function (prop, end, easing) {
                    var tween = jQuery.Tween(elem, animation.opts, prop, end,
                            animation.opts.specialEasing[prop] || animation.opts.easing);
                    animation.tweens.push(tween);
                    return tween;
                },
                stop: function (gotoEnd) {
                    var index = 0,
                        // if we are going to the end, we want to run all the tweens
                        // otherwise we skip this part
                        length = gotoEnd ? animation.tweens.length : 0;

                    for (; index < length ; index++) {
                        animation.tweens[index].run(1);
                    }

                    // resolve when we played the last frame
                    // otherwise, reject
                    if (gotoEnd) {
                        deferred.resolveWith(elem, [animation, gotoEnd]);
                    } else {
                        deferred.rejectWith(elem, [animation, gotoEnd]);
                    }
                    return this;
                }
            }),
            props = animation.props;

        propFilter(props, animation.opts.specialEasing);

        for (; index < length ; index++) {
            result = animationPrefilters[index].call(animation, elem, props, animation.opts);
            if (result) {
                return result;
            }
        }

        createTweens(animation, props);

        if (jQuery.isFunction(animation.opts.start)) {
            animation.opts.start.call(elem, animation);
        }

        jQuery.fx.timer(
            jQuery.extend(tick, {
                anim: animation,
                queue: animation.opts.queue,
                elem: elem
            })
        );

        // attach callbacks from options
        return animation.progress(animation.opts.progress)
            .done(animation.opts.done, animation.opts.complete)
            .fail(animation.opts.fail)
            .always(animation.opts.always);
    }

    function propFilter(props, specialEasing) {
        var index, name, easing, value, hooks;

        // camelCase, specialEasing and expand cssHook pass
        for (index in props) {
            name = jQuery.camelCase(index);
            easing = specialEasing[name];
            value = props[index];
            if (jQuery.isArray(value)) {
                easing = value[1];
                value = props[index] = value[0];
            }

            if (index !== name) {
                props[name] = value;
                delete props[index];
            }

            hooks = jQuery.cssHooks[name];
            if (hooks && "expand" in hooks) {
                value = hooks.expand(value);
                delete props[name];

                // not quite $.extend, this wont overwrite keys already present.
                // also - reusing 'index' from above because we have the correct "name"
                for (index in value) {
                    if (!(index in props)) {
                        props[index] = value[index];
                        specialEasing[index] = easing;
                    }
                }
            } else {
                specialEasing[name] = easing;
            }
        }
    }

    jQuery.Animation = jQuery.extend(Animation, {

        tweener: function (props, callback) {
            if (jQuery.isFunction(props)) {
                callback = props;
                props = ["*"];
            } else {
                props = props.split(" ");
            }

            var prop,
                index = 0,
                length = props.length;

            for (; index < length ; index++) {
                prop = props[index];
                tweeners[prop] = tweeners[prop] || [];
                tweeners[prop].unshift(callback);
            }
        },

        prefilter: function (callback, prepend) {
            if (prepend) {
                animationPrefilters.unshift(callback);
            } else {
                animationPrefilters.push(callback);
            }
        }
    });

    function defaultPrefilter(elem, props, opts) {
        var index, prop, value, length, dataShow, toggle, tween, hooks, oldfire,
            anim = this,
            style = elem.style,
            orig = {},
            handled = [],
            hidden = elem.nodeType && isHidden(elem);

        // handle queue: false promises
        if (!opts.queue) {
            hooks = jQuery._queueHooks(elem, "fx");
            if (hooks.unqueued == null) {
                hooks.unqueued = 0;
                oldfire = hooks.empty.fire;
                hooks.empty.fire = function () {
                    if (!hooks.unqueued) {
                        oldfire();
                    }
                };
            }
            hooks.unqueued++;

            anim.always(function () {
                // doing this makes sure that the complete handler will be called
                // before this completes
                anim.always(function () {
                    hooks.unqueued--;
                    if (!jQuery.queue(elem, "fx").length) {
                        hooks.empty.fire();
                    }
                });
            });
        }

        // height/width overflow pass
        if (elem.nodeType === 1 && ("height" in props || "width" in props)) {
            // Make sure that nothing sneaks out
            // Record all 3 overflow attributes because IE does not
            // change the overflow attribute when overflowX and
            // overflowY are set to the same value
            opts.overflow = [style.overflow, style.overflowX, style.overflowY];

            // Set display property to inline-block for height/width
            // animations on inline elements that are having width/height animated
            if (jQuery.css(elem, "display") === "inline" &&
                    jQuery.css(elem, "float") === "none") {

                // inline-level elements accept inline-block;
                // block-level elements need to be inline with layout
                if (!jQuery.support.inlineBlockNeedsLayout || css_defaultDisplay(elem.nodeName) === "inline") {
                    style.display = "inline-block";

                } else {
                    style.zoom = 1;
                }
            }
        }

        if (opts.overflow) {
            style.overflow = "hidden";
            if (!jQuery.support.shrinkWrapBlocks) {
                anim.done(function () {
                    style.overflow = opts.overflow[0];
                    style.overflowX = opts.overflow[1];
                    style.overflowY = opts.overflow[2];
                });
            }
        }


        // show/hide pass
        for (index in props) {
            value = props[index];
            if (rfxtypes.exec(value)) {
                delete props[index];
                toggle = toggle || value === "toggle";
                if (value === (hidden ? "hide" : "show")) {
                    continue;
                }
                handled.push(index);
            }
        }

        length = handled.length;
        if (length) {
            dataShow = jQuery._data(elem, "fxshow") || jQuery._data(elem, "fxshow", {});
            if ("hidden" in dataShow) {
                hidden = dataShow.hidden;
            }

            // store state if its toggle - enables .stop().toggle() to "reverse"
            if (toggle) {
                dataShow.hidden = !hidden;
            }
            if (hidden) {
                jQuery(elem).show();
            } else {
                anim.done(function () {
                    jQuery(elem).hide();
                });
            }
            anim.done(function () {
                var prop;
                jQuery.removeData(elem, "fxshow", true);
                for (prop in orig) {
                    jQuery.style(elem, prop, orig[prop]);
                }
            });
            for (index = 0 ; index < length ; index++) {
                prop = handled[index];
                tween = anim.createTween(prop, hidden ? dataShow[prop] : 0);
                orig[prop] = dataShow[prop] || jQuery.style(elem, prop);

                if (!(prop in dataShow)) {
                    dataShow[prop] = tween.start;
                    if (hidden) {
                        tween.end = tween.start;
                        tween.start = prop === "width" || prop === "height" ? 1 : 0;
                    }
                }
            }
        }
    }

    function Tween(elem, options, prop, end, easing) {
        return new Tween.prototype.init(elem, options, prop, end, easing);
    }
    jQuery.Tween = Tween;

    Tween.prototype = {
        constructor: Tween,
        init: function (elem, options, prop, end, easing, unit) {
            this.elem = elem;
            this.prop = prop;
            this.easing = easing || "swing";
            this.options = options;
            this.start = this.now = this.cur();
            this.end = end;
            this.unit = unit || (jQuery.cssNumber[prop] ? "" : "px");
        },
        cur: function () {
            var hooks = Tween.propHooks[this.prop];

            return hooks && hooks.get ?
                hooks.get(this) :
                Tween.propHooks._default.get(this);
        },
        run: function (percent) {
            var eased,
                hooks = Tween.propHooks[this.prop];

            if (this.options.duration) {
                this.pos = eased = jQuery.easing[this.easing](
                    percent, this.options.duration * percent, 0, 1, this.options.duration
                );
            } else {
                this.pos = eased = percent;
            }
            this.now = (this.end - this.start) * eased + this.start;

            if (this.options.step) {
                this.options.step.call(this.elem, this.now, this);
            }

            if (hooks && hooks.set) {
                hooks.set(this);
            } else {
                Tween.propHooks._default.set(this);
            }
            return this;
        }
    };

    Tween.prototype.init.prototype = Tween.prototype;

    Tween.propHooks = {
        _default: {
            get: function (tween) {
                var result;

                if (tween.elem[tween.prop] != null &&
                    (!tween.elem.style || tween.elem.style[tween.prop] == null)) {
                    return tween.elem[tween.prop];
                }

                // passing any value as a 4th parameter to .css will automatically
                // attempt a parseFloat and fallback to a string if the parse fails
                // so, simple values such as "10px" are parsed to Float.
                // complex values such as "rotate(1rad)" are returned as is.
                result = jQuery.css(tween.elem, tween.prop, false, "");
                // Empty strings, null, undefined and "auto" are converted to 0.
                return !result || result === "auto" ? 0 : result;
            },
            set: function (tween) {
                // use step hook for back compat - use cssHook if its there - use .style if its
                // available and use plain properties where available
                if (jQuery.fx.step[tween.prop]) {
                    jQuery.fx.step[tween.prop](tween);
                } else if (tween.elem.style && (tween.elem.style[jQuery.cssProps[tween.prop]] != null || jQuery.cssHooks[tween.prop])) {
                    jQuery.style(tween.elem, tween.prop, tween.now + tween.unit);
                } else {
                    tween.elem[tween.prop] = tween.now;
                }
            }
        }
    };

    // Remove in 2.0 - this supports IE8's panic based approach
    // to setting things on disconnected nodes

    Tween.propHooks.scrollTop = Tween.propHooks.scrollLeft = {
        set: function (tween) {
            if (tween.elem.nodeType && tween.elem.parentNode) {
                tween.elem[tween.prop] = tween.now;
            }
        }
    };

    jQuery.each(["toggle", "show", "hide"], function (i, name) {
        var cssFn = jQuery.fn[name];
        jQuery.fn[name] = function (speed, easing, callback) {
            return speed == null || typeof speed === "boolean" ||
                // special check for .toggle( handler, handler, ... )
                (!i && jQuery.isFunction(speed) && jQuery.isFunction(easing)) ?
                cssFn.apply(this, arguments) :
                this.animate(genFx(name, true), speed, easing, callback);
        };
    });

    jQuery.fn.extend({
        fadeTo: function (speed, to, easing, callback) {

            // show any hidden elements after setting opacity to 0
            return this.filter(isHidden).css("opacity", 0).show()

                // animate to the value specified
                .end().animate({ opacity: to }, speed, easing, callback);
        },
        animate: function (prop, speed, easing, callback) {
            var empty = jQuery.isEmptyObject(prop),
                optall = jQuery.speed(speed, easing, callback),
                doAnimation = function () {
                    // Operate on a copy of prop so per-property easing won't be lost
                    var anim = Animation(this, jQuery.extend({}, prop), optall);

                    // Empty animations resolve immediately
                    if (empty) {
                        anim.stop(true);
                    }
                };

            return empty || optall.queue === false ?
                this.each(doAnimation) :
                this.queue(optall.queue, doAnimation);
        },
        stop: function (type, clearQueue, gotoEnd) {
            var stopQueue = function (hooks) {
                var stop = hooks.stop;
                delete hooks.stop;
                stop(gotoEnd);
            };

            if (typeof type !== "string") {
                gotoEnd = clearQueue;
                clearQueue = type;
                type = undefined;
            }
            if (clearQueue && type !== false) {
                this.queue(type || "fx", []);
            }

            return this.each(function () {
                var dequeue = true,
                    index = type != null && type + "queueHooks",
                    timers = jQuery.timers,
                    data = jQuery._data(this);

                if (index) {
                    if (data[index] && data[index].stop) {
                        stopQueue(data[index]);
                    }
                } else {
                    for (index in data) {
                        if (data[index] && data[index].stop && rrun.test(index)) {
                            stopQueue(data[index]);
                        }
                    }
                }

                for (index = timers.length; index--;) {
                    if (timers[index].elem === this && (type == null || timers[index].queue === type)) {
                        timers[index].anim.stop(gotoEnd);
                        dequeue = false;
                        timers.splice(index, 1);
                    }
                }

                // start the next in the queue if the last step wasn't forced
                // timers currently will call their complete callbacks, which will dequeue
                // but only if they were gotoEnd
                if (dequeue || !gotoEnd) {
                    jQuery.dequeue(this, type);
                }
            });
        }
    });

    // Generate parameters to create a standard animation
    function genFx(type, includeWidth) {
        var which,
            attrs = { height: type },
            i = 0;

        // if we include width, step value is 1 to do all cssExpand values,
        // if we don't include width, step value is 2 to skip over Left and Right
        includeWidth = includeWidth ? 1 : 0;
        for (; i < 4 ; i += 2 - includeWidth) {
            which = cssExpand[i];
            attrs["margin" + which] = attrs["padding" + which] = type;
        }

        if (includeWidth) {
            attrs.opacity = attrs.width = type;
        }

        return attrs;
    }

    // Generate shortcuts for custom animations
    jQuery.each({
        slideDown: genFx("show"),
        slideUp: genFx("hide"),
        slideToggle: genFx("toggle"),
        fadeIn: { opacity: "show" },
        fadeOut: { opacity: "hide" },
        fadeToggle: { opacity: "toggle" }
    }, function (name, props) {
        jQuery.fn[name] = function (speed, easing, callback) {
            return this.animate(props, speed, easing, callback);
        };
    });

    jQuery.speed = function (speed, easing, fn) {
        var opt = speed && typeof speed === "object" ? jQuery.extend({}, speed) : {
            complete: fn || !fn && easing ||
                jQuery.isFunction(speed) && speed,
            duration: speed,
            easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
        };

        opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
            opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[opt.duration] : jQuery.fx.speeds._default;

        // normalize opt.queue - true/undefined/null -> "fx"
        if (opt.queue == null || opt.queue === true) {
            opt.queue = "fx";
        }

        // Queueing
        opt.old = opt.complete;

        opt.complete = function () {
            if (jQuery.isFunction(opt.old)) {
                opt.old.call(this);
            }

            if (opt.queue) {
                jQuery.dequeue(this, opt.queue);
            }
        };

        return opt;
    };

    jQuery.easing = {
        linear: function (p) {
            return p;
        },
        swing: function (p) {
            return 0.5 - Math.cos(p * Math.PI) / 2;
        }
    };

    jQuery.timers = [];
    jQuery.fx = Tween.prototype.init;
    jQuery.fx.tick = function () {
        var timer,
            timers = jQuery.timers,
            i = 0;

        fxNow = jQuery.now();

        for (; i < timers.length; i++) {
            timer = timers[i];
            // Checks the timer has not already been removed
            if (!timer() && timers[i] === timer) {
                timers.splice(i--, 1);
            }
        }

        if (!timers.length) {
            jQuery.fx.stop();
        }
        fxNow = undefined;
    };

    jQuery.fx.timer = function (timer) {
        if (timer() && jQuery.timers.push(timer) && !timerId) {
            timerId = setInterval(jQuery.fx.tick, jQuery.fx.interval);
        }
    };

    jQuery.fx.interval = 13;

    jQuery.fx.stop = function () {
        clearInterval(timerId);
        timerId = null;
    };

    jQuery.fx.speeds = {
        slow: 600,
        fast: 200,
        // Default speed
        _default: 400
    };

    // Back Compat <1.8 extension point
    jQuery.fx.step = {};

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.animated = function (elem) {
            return jQuery.grep(jQuery.timers, function (fn) {
                return elem === fn.elem;
            }).length;
        };
    }
    var rroot = /^(?:body|html)$/i;

    jQuery.fn.offset = function (options) {
        if (arguments.length) {
            return options === undefined ?
                this :
                this.each(function (i) {
                    jQuery.offset.setOffset(this, options, i);
                });
        }

        var docElem, body, win, clientTop, clientLeft, scrollTop, scrollLeft,
            box = { top: 0, left: 0 },
            elem = this[0],
            doc = elem && elem.ownerDocument;

        if (!doc) {
            return;
        }

        if ((body = doc.body) === elem) {
            return jQuery.offset.bodyOffset(elem);
        }

        docElem = doc.documentElement;

        // Make sure it's not a disconnected DOM node
        if (!jQuery.contains(docElem, elem)) {
            return box;
        }

        // If we don't have gBCR, just use 0,0 rather than error
        // BlackBerry 5, iOS 3 (original iPhone)
        if (typeof elem.getBoundingClientRect !== "undefined") {
            box = elem.getBoundingClientRect();
        }
        win = getWindow(doc);
        clientTop = docElem.clientTop || body.clientTop || 0;
        clientLeft = docElem.clientLeft || body.clientLeft || 0;
        scrollTop = win.pageYOffset || docElem.scrollTop;
        scrollLeft = win.pageXOffset || docElem.scrollLeft;
        return {
            top: box.top + scrollTop - clientTop,
            left: box.left + scrollLeft - clientLeft
        };
    };

    jQuery.offset = {

        bodyOffset: function (body) {
            var top = body.offsetTop,
                left = body.offsetLeft;

            if (jQuery.support.doesNotIncludeMarginInBodyOffset) {
                top += parseFloat(jQuery.css(body, "marginTop")) || 0;
                left += parseFloat(jQuery.css(body, "marginLeft")) || 0;
            }

            return { top: top, left: left };
        },

        setOffset: function (elem, options, i) {
            var position = jQuery.css(elem, "position");

            // set position first, in-case top/left are set even on static elem
            if (position === "static") {
                elem.style.position = "relative";
            }

            var curElem = jQuery(elem),
                curOffset = curElem.offset(),
                curCSSTop = jQuery.css(elem, "top"),
                curCSSLeft = jQuery.css(elem, "left"),
                calculatePosition = (position === "absolute" || position === "fixed") && jQuery.inArray("auto", [curCSSTop, curCSSLeft]) > -1,
                props = {}, curPosition = {}, curTop, curLeft;

            // need to be able to calculate position if either top or left is auto and position is either absolute or fixed
            if (calculatePosition) {
                curPosition = curElem.position();
                curTop = curPosition.top;
                curLeft = curPosition.left;
            } else {
                curTop = parseFloat(curCSSTop) || 0;
                curLeft = parseFloat(curCSSLeft) || 0;
            }

            if (jQuery.isFunction(options)) {
                options = options.call(elem, i, curOffset);
            }

            if (options.top != null) {
                props.top = (options.top - curOffset.top) + curTop;
            }
            if (options.left != null) {
                props.left = (options.left - curOffset.left) + curLeft;
            }

            if ("using" in options) {
                options.using.call(elem, props);
            } else {
                curElem.css(props);
            }
        }
    };


    jQuery.fn.extend({

        position: function () {
            if (!this[0]) {
                return;
            }

            var elem = this[0],

            // Get *real* offsetParent
            offsetParent = this.offsetParent(),

            // Get correct offsets
            offset = this.offset(),
            parentOffset = rroot.test(offsetParent[0].nodeName) ? { top: 0, left: 0 } : offsetParent.offset();

            // Subtract element margins
            // note: when an element has margin: auto the offsetLeft and marginLeft
            // are the same in Safari causing offset.left to incorrectly be 0
            offset.top -= parseFloat(jQuery.css(elem, "marginTop")) || 0;
            offset.left -= parseFloat(jQuery.css(elem, "marginLeft")) || 0;

            // Add offsetParent borders
            parentOffset.top += parseFloat(jQuery.css(offsetParent[0], "borderTopWidth")) || 0;
            parentOffset.left += parseFloat(jQuery.css(offsetParent[0], "borderLeftWidth")) || 0;

            // Subtract the two offsets
            return {
                top: offset.top - parentOffset.top,
                left: offset.left - parentOffset.left
            };
        },

        offsetParent: function () {
            return this.map(function () {
                var offsetParent = this.offsetParent || document.body;
                while (offsetParent && (!rroot.test(offsetParent.nodeName) && jQuery.css(offsetParent, "position") === "static")) {
                    offsetParent = offsetParent.offsetParent;
                }
                return offsetParent || document.body;
            });
        }
    });


    // Create scrollLeft and scrollTop methods
    jQuery.each({ scrollLeft: "pageXOffset", scrollTop: "pageYOffset" }, function (method, prop) {
        var top = /Y/.test(prop);

        jQuery.fn[method] = function (val) {
            return jQuery.access(this, function (elem, method, val) {
                var win = getWindow(elem);

                if (val === undefined) {
                    return win ? (prop in win) ? win[prop] :
                        win.document.documentElement[method] :
                        elem[method];
                }

                if (win) {
                    win.scrollTo(
                        !top ? val : jQuery(win).scrollLeft(),
                         top ? val : jQuery(win).scrollTop()
                    );

                } else {
                    elem[method] = val;
                }
            }, method, val, arguments.length, null);
        };
    });

    function getWindow(elem) {
        return jQuery.isWindow(elem) ?
            elem :
            elem.nodeType === 9 ?
                elem.defaultView || elem.parentWindow :
                false;
    }
    // Create innerHeight, innerWidth, height, width, outerHeight and outerWidth methods
    jQuery.each({ Height: "height", Width: "width" }, function (name, type) {
        jQuery.each({ padding: "inner" + name, content: type, "": "outer" + name }, function (defaultExtra, funcName) {
            // margin is only for outerHeight, outerWidth
            jQuery.fn[funcName] = function (margin, value) {
                var chainable = arguments.length && (defaultExtra || typeof margin !== "boolean"),
                    extra = defaultExtra || (margin === true || value === true ? "margin" : "border");

                return jQuery.access(this, function (elem, type, value) {
                    var doc;

                    if (jQuery.isWindow(elem)) {
                        // As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
                        // isn't a whole lot we can do. See pull request at this URL for discussion:
                        // https://github.com/jquery/jquery/pull/764
                        return elem.document.documentElement["client" + name];
                    }

                    // Get document width or height
                    if (elem.nodeType === 9) {
                        doc = elem.documentElement;

                        // Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
                        // unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
                        return Math.max(
                            elem.body["scroll" + name], doc["scroll" + name],
                            elem.body["offset" + name], doc["offset" + name],
                            doc["client" + name]
                        );
                    }

                    return value === undefined ?
                        // Get width or height on the element, requesting but not forcing parseFloat
                        jQuery.css(elem, type, value, extra) :

                        // Set width or height on the element
                        jQuery.style(elem, type, value, extra);
                }, type, chainable ? margin : undefined, chainable, null);
            };
        });
    });
    // Expose jQuery to the global object
    window.jQuery = window.$ = jQuery;

    // Expose jQuery as an AMD module, but only for AMD loaders that
    // understand the issues with loading multiple versions of jQuery
    // in a page that all might call define(). The loader will indicate
    // they have special allowances for multiple jQuery versions by
    // specifying define.amd.jQuery = true. Register as a named module,
    // since jQuery can be concatenated with other files that may use define,
    // but not use a proper concatenation script that understands anonymous
    // AMD modules. A named AMD is safest and most robust way to register.
    // Lowercase jquery is used because AMD module names are derived from
    // file names, and jQuery is normally delivered in a lowercase file name.
    // Do this after creating the global so that if an AMD module wants to call
    // noConflict to hide this version of jQuery, it will work.
    if (typeof define === "function" && define.amd && define.amd.jQuery) {
        define("jquery", [], function () { return jQuery; });
    }

    $.fn.autoTextarea = function () {
        $(this).each(function () {

            var me = $(this);
            me.data({
                'minHeight': me.attr('_minHeight') || me.height()
            });
            $(this).bind('keydown keyup', function () {

                var height, style = this.style, minHeight = $(this).data('minHeight'), scrollTop = $(window).scrollTop();
                this.style.height = minHeight + 'px';

                if (this.scrollHeight > minHeight) {

                    height = this.scrollHeight;

                    style.overflowY = 'hidden';
                    style.height = height + 10 + 'px';
                    $(window).scrollTop(scrollTop);
                }
            });

        });

    };

})(window);


//md5=====================================
(function ($) {

    var rotateLeft = function (lValue, iShiftBits) {
        return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits));
    }

    var addUnsigned = function (lX, lY) {
        var lX4, lY4, lX8, lY8, lResult;
        lX8 = (lX & 0x80000000);
        lY8 = (lY & 0x80000000);
        lX4 = (lX & 0x40000000);
        lY4 = (lY & 0x40000000);
        lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);
        if (lX4 & lY4) return (lResult ^ 0x80000000 ^ lX8 ^ lY8);
        if (lX4 | lY4) {
            if (lResult & 0x40000000) return (lResult ^ 0xC0000000 ^ lX8 ^ lY8);
            else return (lResult ^ 0x40000000 ^ lX8 ^ lY8);
        } else {
            return (lResult ^ lX8 ^ lY8);
        }
    }

    var F = function (x, y, z) {
        return (x & y) | ((~x) & z);
    }

    var G = function (x, y, z) {
        return (x & z) | (y & (~z));
    }

    var H = function (x, y, z) {
        return (x ^ y ^ z);
    }

    var I = function (x, y, z) {
        return (y ^ (x | (~z)));
    }

    var FF = function (a, b, c, d, x, s, ac) {
        a = addUnsigned(a, addUnsigned(addUnsigned(F(b, c, d), x), ac));
        return addUnsigned(rotateLeft(a, s), b);
    };

    var GG = function (a, b, c, d, x, s, ac) {
        a = addUnsigned(a, addUnsigned(addUnsigned(G(b, c, d), x), ac));
        return addUnsigned(rotateLeft(a, s), b);
    };

    var HH = function (a, b, c, d, x, s, ac) {
        a = addUnsigned(a, addUnsigned(addUnsigned(H(b, c, d), x), ac));
        return addUnsigned(rotateLeft(a, s), b);
    };

    var II = function (a, b, c, d, x, s, ac) {
        a = addUnsigned(a, addUnsigned(addUnsigned(I(b, c, d), x), ac));
        return addUnsigned(rotateLeft(a, s), b);
    };

    var convertToWordArray = function (string) {
        var lWordCount;
        var lMessageLength = string.length;
        var lNumberOfWordsTempOne = lMessageLength + 8;
        var lNumberOfWordsTempTwo = (lNumberOfWordsTempOne - (lNumberOfWordsTempOne % 64)) / 64;
        var lNumberOfWords = (lNumberOfWordsTempTwo + 1) * 16;
        var lWordArray = Array(lNumberOfWords - 1);
        var lBytePosition = 0;
        var lByteCount = 0;
        while (lByteCount < lMessageLength) {
            lWordCount = (lByteCount - (lByteCount % 4)) / 4;
            lBytePosition = (lByteCount % 4) * 8;
            lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition));
            lByteCount++;
        }
        lWordCount = (lByteCount - (lByteCount % 4)) / 4;
        lBytePosition = (lByteCount % 4) * 8;
        lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition);
        lWordArray[lNumberOfWords - 2] = lMessageLength << 3;
        lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29;
        return lWordArray;
    };

    var wordToHex = function (lValue) {
        var WordToHexValue = "", WordToHexValueTemp = "", lByte, lCount;
        for (lCount = 0; lCount <= 3; lCount++) {
            lByte = (lValue >>> (lCount * 8)) & 255;
            WordToHexValueTemp = "0" + lByte.toString(16);
            WordToHexValue = WordToHexValue + WordToHexValueTemp.substr(WordToHexValueTemp.length - 2, 2);
        }
        return WordToHexValue;
    };

    var uTF8Encode = function (string) {
        string = string.replace(/\x0d\x0a/g, "\x0a");
        var output = "";
        for (var n = 0; n < string.length; n++) {
            var c = string.charCodeAt(n);
            if (c < 128) {
                output += String.fromCharCode(c);
            } else if ((c > 127) && (c < 2048)) {
                output += String.fromCharCode((c >> 6) | 192);
                output += String.fromCharCode((c & 63) | 128);
            } else {
                output += String.fromCharCode((c >> 12) | 224);
                output += String.fromCharCode(((c >> 6) & 63) | 128);
                output += String.fromCharCode((c & 63) | 128);
            }
        }
        return output;
    };

    $.extend({
        md5: function (string) {
            var x = Array();
            var k, AA, BB, CC, DD, a, b, c, d;
            var S11 = 7, S12 = 12, S13 = 17, S14 = 22;
            var S21 = 5, S22 = 9, S23 = 14, S24 = 20;
            var S31 = 4, S32 = 11, S33 = 16, S34 = 23;
            var S41 = 6, S42 = 10, S43 = 15, S44 = 21;
            string = uTF8Encode(string);
            x = convertToWordArray(string);
            a = 0x67452301; b = 0xEFCDAB89; c = 0x98BADCFE; d = 0x10325476;
            for (k = 0; k < x.length; k += 16) {
                AA = a; BB = b; CC = c; DD = d;
                a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
                d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
                c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
                b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
                a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
                d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
                c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
                b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
                a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
                d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
                c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
                b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
                a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
                d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
                c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
                b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
                a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
                d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
                c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
                b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
                a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
                d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
                c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
                b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
                a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
                d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
                c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
                b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
                a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
                d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
                c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
                b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
                a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
                d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
                c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
                b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
                a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
                d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
                c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
                b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
                a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
                d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
                c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
                b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
                a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
                d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
                c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
                b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
                a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
                d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
                c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
                b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
                a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
                d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
                c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
                b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
                a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
                d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
                c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
                b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
                a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
                d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
                c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
                b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
                a = addUnsigned(a, AA);
                b = addUnsigned(b, BB);
                c = addUnsigned(c, CC);
                d = addUnsigned(d, DD);
            }
            var tempValue = wordToHex(a) + wordToHex(b) + wordToHex(c) + wordToHex(d);
            return tempValue.toLowerCase();
        }
    });
})(jQuery);

//jquery-crmextend-1.0.js=====================================
(function ($, window, undefined) {

    var document = window.document;
    var ua = navigator.userAgent.toLowerCase();
    var checkua = function (r) {
        switch (typeof r) {
            case 'object':
            case 'function':
                return r.test(ua);
            case 'string':
                return ua.indexOf(r) != -1;
            default:
                return false;
        }
    };

    $.extend($, (function () {

        var _escapeRegex = /([-.*+?^${}()|[\]\/\\])/g;
        var _decodeDateRegex = /^(\d{1,4})-(\d{1,2})-(\d{1,2})(?:T(\d{1,2})\:(\d{1,2})\:(\d{1,2})(?:\:(\d{1,4}))?)?Z?$/i;
        var ext_each = function (array, fn, scope) {//Ext中的each函数，因为和jquery有冲突，所以以内部函数方式使用
            if ($.isEmpty(array, false)) {
                return;
            }
            if (!$.isIterable(array) || $.isPrimitive(array)) {
                array = [array];
            }
            for (var i = 0, len = array.length; i < len; i++) {
                if (fn.call(scope || array[i], array[i], i, array) === false) {
                    return i;
                }
            }
        };

        return {
            emptyFn: function () { },
            emptyString: '',
            returnFalse: function () { return false; },
            fileSize: function (size) {//文件尺寸运算
                if (typeof size == 'string') {
                    size = parseInt(size, 10);
                }
                if (size < 1024) {
                    return size + 'byte';
                }
                size = size / 1024;
                if (size < 1024) {
                    return Math.formatNumber(size, 2) + 'KB';
                }
                size = size / 1024;
                if (size < 1024) {
                    return Math.formatNumber(size, 2) + 'MB';
                }
                size = size / 1024;
                if (size < 1024) {
                    return Math.formatNumber(size, 2) + 'GB';
                }
            },
            getFileSize: function (file) {

                if (file) {

                    if (typeof file.files != 'undefined') {
                        if (file.files.length > 0 && typeof FileReader != 'undefined') {
                            return file.files[0].size || file.files[0].fileSize
                        }
                    } else if ($.browser.ie) {

                        if ($.browser.ie6) {

                            var image = new Image();
                            image.dynsrc = file.value;
                            return image.fileSize;

                        } else {

                            var filePath;
                            try {

                                file.select();
                                filePath = document.selection.createRange().text;

                            } catch (e) { }

                            if (!$.isEmpty(filePath, false)) {
                                try {
                                    var fileSystem = new ActiveXObject("Scripting.FileSystemObject");
                                    if (fileSystem) {

                                        var file = fileSystem.GetFile(filePath);

                                        if (file) {
                                            return file.Size;
                                        }

                                    }
                                }
                                catch (e) { }
                            }

                        }

                    }

                }

            },
            override: function () {//扩展一个对象的原型对象
                if (arguments.length <= 1) {
                    return;
                } else {
                    arguments[0] = arguments[0].prototype;
                    $.extend.apply(arguments[0] || window, arguments);
                }
            },
            apply: function (o, c, defaults, excludes) {//复制一个对象的属性到另一个对象，直接覆盖已存在的
                if (defaults) {
                    o = $.apply(o, defaults);
                }
                if (o && c && $.isObject(c)) {
                    for (var p in c) {

                        if (typeof excludes === 'undefined' || typeof excludes[p] === 'undefined') {
                            o[p] = c[p];
                        }

                    }
                }
                return o;
            },
            applyIf: function (o, c) {//复制一个对象的属性到另一个对象，排除已存在的
                if (o) {
                    for (var p in c) {
                        if (!$.isDefined(o[p])) {
                            o[p] = c[p];
                        }
                    }
                }
                return o;
            },
            copyArray: function (src, dest) {


                if (!dest || typeof dest.length == 'undefined') {
                    return src;
                }
                if (!src) {
                    src = [];
                } else if (!$.isArray(src)) {
                    return src;
                }

                if ($.isArray(dest)) {
                    //直接拷贝数组
                    Array.prototype.push.apply(src, dest);

                } else {
                    //copy 带索引的对象
                    for (var i = 0, len = dest.length; i < len; i++) {
                        src.push(dest[i]);
                    }
                }
                return src;


            },
            removeNode: (function () {

                var timer;

                return function (el) {

                    if (el == null || el == undefined)
                        return;

                    if (document.body.removeNode) {

                        if (timer) {
                            clearTimeout(timer);
                            timer = null;
                        }

                        if (typeof el === 'string')
                            el = document.getElementById(el);

                        if (el) {

                            //移除下面的所有iframe元素
                            var ifms = el.getElementsByTagName('iframe');
                            if (ifms.length > 0) {
                                for (var i = 0, len = ifms.length, isIE10 = $.browser.ie10; i < len; i++) {
                                    if (isIE10)
                                        ifms.item(0).removeNode(true);
                                    else
                                        ifms[0].removeNode(true);
                                }
                            }

                            //移除本元素
                            el.removeNode(true);

                            ifms = null;
                            el = null;

                            if (CollectGarbage) {
                                timer = setTimeout(CollectGarbage, 1000);
                            }

                        }


                    } else {

                        if (typeof el != 'string')
                            el = el.id;
                        if (!$.isEmpty(el, false))
                            $('#' + el).remove();
                    }


                }

            })(),
            ns: function () {
                var o, d;
                ext_each(arguments, function (v) {
                    d = v.split(".");
                    o = window[d[0]] = window[d[0]] || {};
                    ext_each(d.slice(1), function (v2) {
                        o = o[v2] = o[v2] || {};
                    });
                });

                d = null;

                return o;
            },
            iterate: function (obj, fn, scope) {
                if ($.isEmpty(obj)) {
                    return;
                }
                if ($.isIterable(obj)) {
                    ext_each(obj, fn, scope);
                    return;
                } else if ($.isObject(obj)) {
                    for (var prop in obj) {
                        if (obj.hasOwnProperty(prop)) {
                            if (fn.call(scope || obj, prop, obj[prop], obj) === false) {
                                return;
                            }
                        }
                    }
                }
            },
            filterArray: function (arr, filter, isAll) {

                if ($.isEmpty(arr) || !$.isArray(arr)) {
                    return [];
                }

                if ($.isFunction(filter)) {

                    var re = [], i = 0, len = arr.length, item;
                    for (; i < len; i += 1) {
                        item = arr[i];
                        if (filter(item)) {
                            re.push(item);
                        }
                    }
                    return re;

                } else if ($.isNumber(filter)) {
                    return (filter >= 0 && filter < len) ? [arr[filter]] : [];
                }

                return isAll === true ? arr : [];

            },
            foreach_array: function (fn, arr, context, args) {

                if (!$.isFunction(fn) || arr === null || arr === undefined || typeof arr.length === 'undefined') {
                    return;
                }

                if ($.isEmpty(args)) {
                    args = [];
                } else {
                    if (!$.isArray(args)) {
                        args = [args];
                    }
                }

                var len = arr.length, i = 0, argsLen = args.length, unshift = function (p1, p2) {
                    if (argsLen != args.length) {
                        args.splice(0, 2);
                    }
                    args.unshift(p2);
                    args.unshift(p1);
                    return args;
                };

                if (len === 0) {
                    return;
                } else if (len < 8) {
                    for (; i < len; i += 1) {
                        if (fn.apply(context || window, unshift(arr[i], i)) === true) {
                            break;
                        }
                    }
                } else {

                    var iterations = Math.floor(len / 8), startAt = len % 8;
                    if (startAt != 0) {
                        iterations += 1;
                    }

                    do {
                        switch (startAt) {
                            case 0: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 7: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 6: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 5: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 4: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 3: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 2: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                            case 1: if (fn.apply(context || window, unshift(arr[i], i)) === true) return; i += 1;
                        }
                        startAt = 0;
                    } while (--iterations > 0)

                    iterations = null;
                    startAt = null;

                }

                unshift = null;
                len = null;
                i = null;

            },
            escapeRe: function (s) {
                return s.replace(_escapeRegex, "\\$1");
            },
            urlAppend: function (url, s) {
                if (!$.isEmpty(s, false)) {
                    return url + (url.indexOf('?') === -1 ? '?' : '&') + s;
                }
                return url;
            },
            urlReplace: function (url, s) {

                if (!$.isEmpty(s, false)) {

                    if (url.indexOf('?') == -1 || s.indexOf('=') == -1) {
                        url = $.urlAppend(url, s);
                    }

                    if (s.indexOf('&') != -1) {

                        var plist = s.split('&');
                        for (var i = 0, len = plist.length; i < len; i++) {
                            url = $.urlReplace(url, plist[i]);
                        }

                    } else {

                        var arr = s.split('=');
                        if (url.indexOf('?' + arr[0]) == -1 && url.indexOf('&' + arr[0]) == -1) {
                            url = $.urlAppend(url, s);
                        } else {
                            url = url.replace(arr[0] + '=' + $.urlParam(arr[0], url), s);
                        }
                    }

                }

                return url;

            },
            urlParam: function (key, s) {

                if (!$.isEmpty(key, false)) {

                    s = $.isEmpty(s, false) ? window.location.search : s;

                    var r = new RegExp('(?:\\?|&)+' + key + '=([^&]+)(?:&|$)+', 'gi');
                    return r.test(s.replace(/&amp;/g, '&')) ? RegExp.$1 : '';

                }

            },
            urlEncode: function (o, pre) {

                var empty, buf = [], e = encodeURIComponent;
                $.iterate(o, function (key, item) {

                    empty = $.isEmpty(item);

                    ext_each(empty ? key : item, function (val) {
                        buf.push('&', e(key), '=', (!$.isEmpty(val) && (val != key || !empty)) ? ($.isDate(val) ? $.urlEncode(val).replace(/"/g, '') : e(val)) : '');
                    });

                });

                if (!pre) {
                    buf.shift();
                    pre = '';
                }

                var re = pre + buf.join('');

                //clear
                empty = null;
                buf = null;
                e = null;

                return re;

            },
            urlDecode: function (string, overwrite) {

                if ($.isEmpty(string, false)) {
                    return {};
                }

                if (string.indexOf('?') != -1) {
                    string = string.split('?')[1];
                }
                if (string.indexOf('#') != -1) {
                    string = string.split('#')[0];
                }

                var obj = {}, pairs = string.split('&'), d = decodeURIComponent, name, value;
                for (var i = 0, len = pairs.length, pair; i < len; i++) {
                    pair = pairs[i].split('=');
                    name = d(pair[0]);
                    value = d(pair[1]);
                    obj[name] = overwrite || !obj[name] ? value : [].concat(obj[name]).concat(value);
                }

                return obj;

            },
            notLocalUrl: (function () {

                var fn = function (s) {
                    $(s).each(function () {
                        if (this.tagName && this.tagName.toLowerCase() === 'a' && this.href.indexOf('javascript:') == -1) {

                            if (this.href.indexOf(location.host) == -1) {//非本域链接就在新窗口中打开，以免外部网页破坏本站结构
                                this.target = '_blank';
                            }
                        }
                    })
                }

                return function (selector) {
                    setTimeout(function () {
                        if (selector == undefined) {
                            fn('a');
                        }
                        if ($.isArray(selector)) {
                            this.foreach_array(fn, selector);
                        } else {
                            fn(selector);
                        }
                    }, 300);
                }

            })(),
            formSerialize: (function () {

                var en = function (o, ft) {
                    if ($.isEmpty(ft, false)) { ft = 'o'; }
                    return ft === 'o' ? o : $.urlEncode(o);
                }

                return function (formname, ft, map, excludes) {

                    if ($.isEmpty(formname)) { return re({}); }

                    var result = {}, mapIsEmpty = $.isEmpty(map), excludesIsEmpty = $.isEmpty(excludes);
                    $.foreach_array(function (item, index, inputel, modelname) {

                        if (!excludesIsEmpty && !$.isEmpty(excludes[item.name])) {//可以排除某些不需要序列化的字段
                            return false; //continue
                        }

                        if (!mapIsEmpty && !$.isEmpty(map[item.name])) {//优先使用map中配置的映射
                            result[map[item.name]] = item.value;
                        } else {
                            inputel = $('#' + item.name.replace('$', '_'));
                            if (inputel.length > 0) {//使用在input标签上声明的modelname
                                modelname = inputel.attr('modelname');
                                if (!$.isEmpty(modelname, false)) {
                                    result[modelname] = item.value;
                                    return false; //continue
                                }
                            }

                            result[item.name] = item.value;

                        }

                    }, ((typeof formname === 'string') ? $('#' + formname) : $(formname)).serializeArray());

                    var re = en(result, ft)

                    //clear
                    mapIsEmpty = null;
                    excludesIsEmpty = null;
                    excludes = null;
                    formname = null;
                    map = null;
                    ft = null;

                    //查找页面上的防伪字段
                    if (re && typeof re["__RequestVerificationToken"] == 'undefined') {
                        re["__RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]').val();
                    }

                    return re;

                }


            })(),
            finallyStyle: function (elem, attr) {
                if (elem.style[attr]) {
                    return elem.style[attr];
                } else if (elem.currentstyle) {
                    return elem.currentstyle[attr];
                } else if (document.defaultview && document.defaultview.getcomputedstyle) {
                    return document.defaultview.getcomputedstyle(elem, null).getpropertyvalue(attr.replace(/([a-z])/g, '-$1').tolowercase());
                } else {
                    return null;
                }
            },
            getHead: (function () {
                var head;
                return function () {
                    if (head == undefined) {
                        head = document.getElementsByTagName("head")[0];
                    }
                    return head;
                };
            })(),
            getBody: function () {
                return document.body || document.documentElement;
            },
            setWndResizeHandler: function (doResize, params, scope) {

                if (!$.isEmpty(params)) {
                    if (!$.isArray(params)) {
                        params = [params];
                    }
                } else {
                    params = [];
                }

                if (top.location.href == location.href && typeof (window.orientation) != 'undefined') {//智能移动设备
                    $(window).bind('orientationchange', function () {
                        if ($.os.isAndroid) {
                            timer = setTimeout(function () { doResize.apply(scope || window, params); }, 500);
                        } else {
                            doResize.apply(scope || window, params);
                        }

                    });

                } else {//pc
                    if ($.browser.ff || $.browser.safari || $.browser.opera) {
                        $(window).resize(function () {
                            doResize.apply(scope || window, params);
                        });
                    } else {

                        var timer;
                        $(window).resize(function () {

                            if (timer !== undefined) {
                                clearTimeout(timer);
                            }

                            timer = setTimeout(function () { doResize.apply(scope || window, params); }, 100);

                        });
                    }
                }
            },
            JSON: (function () {//JSON对象处理

                var
                    useHasOwn = !!{}.hasOwnProperty,
                    pad = function (n) { return n < 10 ? '0' + n : n; },
                    doEncode = function (o) {
                        if ($.isEmpty(o)) {
                            return 'null';
                        } else if ($.isArray(o)) {
                            return encodeArray(o);
                        } else if ($.isDate(o)) {
                            return '"' + $.dt.format(o, $.dt.sortableDateTime) + '"';
                        } else if ($.isString(o)) {
                            return encodeString(o);
                        } else if (typeof o === 'number') {//don't use isNumber here, since finite checks happen inside isNumber
                            return isFinite(o) ? o + '' : 'null';
                        } else if ($.isBoolean(o)) {
                            return o.toString();
                        } else {
                            var a = ['{'], b, i, v;
                            for (i in o) {
                                if (!o.getElementsByTagName) {// don't encode DOM objects 

                                    if (!useHasOwn || Object.prototype.hasOwnProperty.call(o, i)) {
                                        v = o[i];
                                        switch (typeof v) {
                                            case 'undefined':
                                            case 'function':
                                            case 'unknown':
                                                break;
                                            default:
                                                if (b) {
                                                    a.push(',');
                                                }
                                                a.push(doEncode(i), ':', v === null ? 'null' : doEncode(v));
                                                b = true;
                                        }
                                    }
                                }
                            }
                            a.push('}');
                            return a.join('');
                        }
                    }, m = {
                        "\b": '\\b',
                        "\t": '\\t',
                        "\n": '\\n',
                        "\f": '\\f',
                        "\r": '\\r',
                        '"': '\\"',
                        "\\": '\\\\'
                    },
                    encodeString = function (s) {
                        if (/["\\\x00-\x1f]/.test(s)) {
                            return '"' + s.replace(/([\x00-\x1f\\"])/g, function (a, b) {
                                var c = m[b];
                                if (c) {
                                    return c;
                                }
                                c = b.charCodeAt();
                                return '\\u00' + Math.floor(c / 16).toString(16) + (c % 16).toString(16);
                            }) + '"';
                        }
                        return '"' + s + '"';
                    },
                    encodeArray = function (o) {
                        var a = ['['], b, i, l = o.length, v;
                        for (i = 0; i < l; i++) {
                            v = o[i];
                            switch (typeof v) {
                                case 'undefined':
                                case 'function':
                                case 'unknown':
                                    break;
                                default:
                                    if (b) {
                                        a.push(',');
                                    }
                                    a.push(v === null ? 'null' : $.JSON.encode(v));
                                    b = true;
                            }
                        }
                        a.push(']');
                        return a.join('');
                    };

                return {
                    encode: function (o) {

                        if (typeof JSON !== 'undefined' && typeof JSON.stringify !== 'undefined') {
                            return JSON.stringify(o);
                        }
                        return doEncode(o);

                    },
                    decodeDate: function (s) {

                        if (s == null || s == undefined)
                            return undefined;

                        if ($.isDate(s))
                            return s;

                        return _decodeDateRegex.test(s)
                            ? new Date(
                                RegExp.$1,//年
                                parseInt(RegExp.$2, 10) - 1,//月
                                RegExp.$3,//日
                                (typeof RegExp.$4 === 'undefined' ? 0 : RegExp.$4),//时
                                (typeof RegExp.$5 === 'undefined' ? 0 : RegExp.$5),//分
                                (typeof RegExp.$6 === 'undefined' ? 0 : RegExp.$6),//秒
                                (typeof RegExp.$7 === 'undefined' ? 0 : RegExp.$7)//毫秒
                            )
                            : undefined;
                    },
                    decodeDate2: function (s) {

                        if (s == null || s == undefined)
                            return undefined;

                        if ($.isDate(s))
                            return s;

                        return eval('new ' + s.replace(/\//g, '').replace('+0800', ''));

                    },
                    decode: function (s) {

                        if (s == null || s == undefined)
                            return undefined;

                        return $.parseJSON(s);

                    }
                };

            })(),
            os: {//平台类型检查
                isWindows: checkua(/windows|win32/i),
                isLinux: checkua('linux'),
                isAir: checkua('adobeair'),
                isMac: checkua(/macintosh|mac os x/i),
                isIPad: checkua('ipad'),
                isIPhone: checkua('iphone'),
                isWinPhone: checkua('windows phone'),
                isAndroid: checkua('android'),
                isMobile: function () {
                    return $.os.isIPad || $.os.isIPhone || $.os.isAndroid || $.os.isWinPhone;
                }
            },
            dt: (function () {

                var _shortDate = 'yyyy-MM-dd';
                var _longDate = 'yyyy-MM-dd HH:mm:ss';
                var _sortableDateTime = 'yyyy-MM-ddTHH:mm:ss';
                var pad = function (n) { return n < 10 ? '0' + n : n; };

                var dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
                var dayNames_CN = ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'];
                var dayNames_CN_short = ['日', '一', '二', '三', '四', '五', '六'];
                var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
                var monthNames_CN = ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'];
                var monthNames_CN_short = ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '十一', '十二'];
                var monthNumbers = { Jan: 0, Feb: 1, Mar: 2, Apr: 3, May: 4, Jun: 5, Jul: 6, Aug: 7, Sep: 8, Oct: 9, Nov: 10, Dec: 11 };

                var f = function (w, r, max, offset) {
                    if (!$.isNumber(w) || (w < 0 && w > max) || $.isEmpty(r)) {
                        return undefined;
                    }
                    return r[w - offset];
                }
                var setd = function (d, fs, interval) {
                    var dd = new Date();
                    switch (fs) {
                        case 'y': d.setYear(dd.getFullYear() + interval); break;
                        case 'M': d.setMonth(dd.getMonth() + interval); break;
                        case 'd': d.setDate(dd.getDate() + interval); break;
                        case 'm': d.setMinutes(dd.getMinutes() + interval); break;
                        case 's': d.setSeconds(dd.getSeconds() + interval); break;
                        case 'h':
                        case 'H':
                            d.setHours(dd.getHours() + interval); break;
                    }
                    return d;
                }

                return {
                    shortDate: _shortDate,
                    longDateTime: _longDate,
                    sortableDateTime: _sortableDateTime,

                    format: function (d, fs) {//y-年,M-月,d-日,H-时（24小时制）,h-时（12小时制）,m-分,s-秒
                        if (!d) {
                            return undefined;
                        }
                        if ($.isEmpty(fs, false)) {
                            fs = _longDate;
                        }

                        var pro = Date.prototype;

                        var y = pro.getFullYear.call(d) + '';
                        if (isNaN(y)) y = 0;

                        if (fs.indexOf('yyyy') != -1) fs = fs.replace('yyyy', y);
                        else if (fs.indexOf('yy') != -1) fs = fs.replace('yy', y.substr(2));
                        else if (fs.indexOf('y') != -1) fs = fs.replace('y', y);

                        var m = pro.getMonth.call(d);
                        if (isNaN(m)) m = 0;
                        m += 1;
                        if (fs.indexOf('MM') != -1) fs = fs.replace('MM', pad(m));
                        else if (fs.indexOf('M') != -1) fs = fs.replace('M', m);

                        var D = pro.getDate.call(d);
                        if (isNaN(D)) D = 0;
                        if (fs.indexOf('dd') != -1) fs = fs.replace('dd', pad(D));
                        else if (fs.indexOf('d') != -1) fs = fs.replace('d', D);

                        var H = pro.getHours.call(d);
                        if (isNaN(H)) H = 0;
                        if (fs.indexOf('HH') != -1) fs = fs.replace('HH', pad(H));
                        else if (fs.indexOf('H') != -1) fs = fs.replace('H', H);
                        else if (fs.indexOf('hh') != -1) fs = fs.replace('hh', pad(H > 12 ? H - 12 : H));
                        else if (fs.indexOf('h') != -1) fs = fs.replace('h', H > 12 ? H - 12 : H);

                        var min = pro.getMinutes.call(d);
                        if (isNaN(min)) min = 0;
                        if (fs.indexOf('mm') != -1) fs = fs.replace('mm', pad(min));
                        else if (fs.indexOf('m') != -1) fs = fs.replace('m', min);

                        var s = pro.getSeconds.call(d);
                        if (isNaN(s)) s = 0;
                        if (fs.indexOf('ss') != -1) fs = fs.replace('ss', pad(s));
                        else if (fs.indexOf('s') != -1) fs = fs.replace('s', s);

                        return fs;

                    },
                    formatToCrmDate: function (d) {

                        if (typeof d == 'string') {
                            d = $.dt.convert(d);
                        }

                        var now = new Date();
                        var year = d.getYear();
                        var month = d.getMonth();
                        var day = d.getDate();
                        var hour = d.getHours();
                        var minute = d.getMinutes();
                        var second = d.getSeconds();
                        var sd = (hour === 0 && minute === 0 && second === 0);

                        if (now.getYear() == year) {

                            if (now.getMonth() == month && now.getDate() == day) {//今天

                                return sd
                                    ? '今日'
                                    : $.dt.format(d, '今日 HH:mm');

                            } else if (now.getMonth() == month) {//本月

                                var dd = now.getDate() - day;

                                if (sd) {

                                    if (dd == 1)
                                        return "昨日";
                                    else if (dd == 2)
                                        return "前日";
                                    else
                                        return $.dt.format(d, 'MM-dd');

                                } else {
                                    if (dd == 1)
                                        return $.dt.format(d, '昨日 HH:mm');
                                    else if (dd == 2)
                                        return $.dt.format(d, '前日 HH:mm');
                                    else
                                        return $.dt.format(d, 'MM-dd HH:mm');
                                }


                            } else {

                                return sd
                                    ? $.dt.format(d, 'MM-dd')
                                    : $.dt.format(d, 'MM-dd HH:mm');

                            }

                        } else {

                            return sd
                                ? $.dt.format(d, 'yyyy-MM-dd')
                                : $.dt.format(d, 'yyyy-MM-dd HH:mm');

                        }

                    },
                    getWeekName: function (w) { return f(w, dayNames, 6, 0); },
                    getWeekCnName: function (w) { return f(w, dayNames_CN, 6, 0); },
                    getWeekCnShortName: function (w) { return f(w, dayNames_CN_short, 6, 0); },
                    getMonthName: function (m) { return f(m, monthNames, 12, 1); },
                    getMonthCnName: function (m) { return f(m, monthNames_CN, 12, 1); },
                    getMonthCnShortName: function (m) { return f(m, monthNames_CN_short, 12, 1); },
                    getMonthNumber: function (m) { return monthNumbers[m]; },
                    add: function (d, fs, interval) {
                        if ($.isEmpty(d)) {
                            return undefined;
                        }
                        if ($.isEmpty(fs, false)) {
                            fs = 'y';
                        }
                        if ($.isEmpty(interval) || !$.isNumber(interval)) {
                            interval = 0
                        }

                        return setd(d, fs, Math.abs(interval));
                    },
                    subtract: function (d, fs, interval) {
                        if ($.isEmpty(d)) {
                            return undefined;
                        }
                        if ($.isEmpty(fs, false)) {
                            fs = 'y';
                        }
                        if ($.isEmpty(interval) || !$.isNumber(interval)) {
                            interval = 0
                        }
                        if (interval > 0) {
                            interval = -interval;
                        }

                        return setd(d, fs, interval);
                    },
                    isLeapYear: function (d) {
                        var year = d.getFullYear();
                        return !!((year & 3) == 0 && (year % 100 || (year % 400 == 0 && year)));
                    },
                    clone: function (d) {
                        return new Date(d.getTime());
                    },
                    convert: function (str/*yyyy-MM-dd HH:mm*/) {

                        if ($.isEmpty(str, false)) {
                            return new Date();
                        }
                        arr = str.split(' ');

                        var a = arr[0].split('-');
                        var b = [];
                        if ($.isEmpty(arr[1], false)) {
                            b = [0, 0];
                        } else {
                            b = arr[1].split(':');
                        }
                        return new Date(
                            parseInt(a[0], 10),
                            parseInt(a[1], 10) - 1,
                            parseInt(a[2], 10),
                            $.isEmpty(b[0], false) ? 0 : parseInt(b[0], 10),
                            $.isEmpty(b[1], false) ? 0 : parseInt(b[1], 10),
                            0,
                            0
                        );

                    }
                };

            })(),
            cookies: (function () {

                var _o = function () {

                    this.get = function (key) {

                        var cookie = document.cookie;
                        var cookieArray = cookie.split(';');
                        for (var i = 0, len = cookieArray.length, val = ''; i < len; i++) {
                            if (cookieArray[i].trim().substr(0, key.length) === key) {
                                val = cookieArray[i].trim().substr(key.length + 1);
                                break;
                            }
                        }
                        return unescape(val);
                    };

                    this.getChild = function (key, childKey) {

                        var child = this.get(key);
                        var childs = child.split('&');

                        for (var i = 0, len = childs.length, val = ''; i < len; i++) {
                            if (childs[i].trim().substr(0, childKey.length) == childKey) {
                                val = childs[i].trim().substr(childKey.length + 1);
                                break;
                            }
                        }
                        return val;
                    };
                    this.set = function (key, value, expires, domain, path) {

                        var cookie = "";

                        if (key !== undefined && value !== undefined) {

                            cookie += key + "=" + escape(value) + ";";

                            if (expires !== undefined)
                                cookie += "expires=" + expires.toGMTString() + ";";
                            if (domain !== undefined)
                                cookie += "domain=" + domain + ";";
                            if (path !== undefined)
                                cookie += "path=" + path + ";";

                        }

                        if (!$.isEmpty(cookie, false)) {
                            document.cookie = cookie;
                        }

                    };
                    this.remove = function (key) {
                        var date = new Date();
                        date.setFullYear(date.getFullYear() - 1);
                        var cookie = " " + key + "=;expires=" + date.toGMTString() + ";"
                        document.cookie = cookie;
                    }
                }

                var _ins;

                return function () {
                    if (_ins === undefined) {
                        _ins = new _o();
                    }
                    return _ins;
                }();

            })()

        };

    })());

    //浏览器特性检查
    $.extend($.browser, (function () {


        var isIE = $.browser.msie !== undefined, docMode = document.documentMode;
        isIE = isIE ? true : /(msie\s|trident.*rv:)([\w.]+)/i.test(navigator.userAgent);

        var isIE7 = isIE && (checkua(/msie 7/i) || docMode == 7);
        var isIE8 = isIE && (checkua(/msie 8/i) || docMode == 8);
        var isIE9 = isIE && (checkua(/msie 9/i) || docMode == 9);
        var isIE10 = isIE && (checkua(/msie 10/i) || docMode == 10);
        var isIE6 = isIE && !isIE7 && !isIE8 && !isIE9 && !isIE10;

        var isWebKit = checkua(/webkit/i);
        var isChrome = isWebKit && checkua('chrome');
        var isSafari = !isChrome && checkua('safari');
        var isGecko = !isWebKit && checkua('gecko');
        var chromev = function () { return (isChrome && /Chrome\/(\d+)./i.test(ua)) ? RegExp.$1 : 0; }
        var operaV = function () { return ($.browser.opera && /Version\/(\d+)./i.test(ua)) ? RegExp.$1 : 0; }
        var ffV = function () { return ($.browser.mozilla && /Firefox\/(\d+)./i.test(ua)) ? RegExp.$1 : 0; }

        // remove css image flicker    
        if (isIE6) {
            try { document.execCommand('BackgroundImageCache', false, true); } catch (e) { }
        }

        return {

            docMode: docMode,
            webkit: isWebKit,

            chrome: isChrome,
            chromeVersion: chromev(),

            opera: $.browser.opera !== undefined,
            operaVersion: operaV(),

            ff: $.browser.mozilla !== undefined,
            ffVersion: ffV(),

            safari: isSafari,
            safari2: isSafari && checkua(/applewebkit\/4/i),
            safari3: isSafari && checkua(/version\/3/i),
            safari4: isSafari && checkua(/version\/4/i),
            safari5: isSafari && checkua(/version\/5/i),

            ie: isIE,
            ie6: isIE6,
            ie7: isIE7,
            ie8: isIE8,
            ie9: isIE9,
            ie10: isIE10,
            ieVersion: docMode,

            gecko: isGecko,
            gecko2: isGecko && checkua(/rv:1\.8/i),
            gecko3: isGecko && checkua(/rv:1\.9/i),

            secure: window.location.protocol.indexOf('https') !== -1

        };

    })());

    //数据类型验证
    $.extend($, (function () {

        var toString = Object.prototype.toString;
        var valueOf = Object.prototype.valueOf;

        return {
            isEmpty: function (v, allowBlank) { return v === null || v === undefined || ($.isArray(v) && !v.length) || (!allowBlank ? v === '' : false); },
            isArray: function (v) { return toString.apply(v) === '[object Array]'; },
            isDate: function (v) { return toString.apply(v) === '[object Date]'; },
            isObject: function (v) { return !!v && toString.call(v) === '[object Object]'; },
            isPrimitive: function (v) { return $.isString(v) || $.isNumber(v) || $.isBoolean(v); }, //是否是基础数据类型
            isFunction: function (v) { return toString.apply(v) === '[object Function]'; },
            isNumber: function (v) { return typeof v === 'number' && isFinite(v); },
            isString: function (v) { return typeof v === 'string'; },
            isBoolean: function (v) { return typeof v === 'boolean'; },
            isElement: function (v) { return v ? !!v.tagName : false; },
            isDefined: function (v) { return typeof v !== 'undefined'; },
            isIterable: function (v) {
                if ($.isArray(v) || v.callee) {
                    return true;
                }
                if (/NodeList|HTMLCollection/.test(toString.call(v))) {
                    return true;
                }
                return ((typeof v.nextNode !== 'undefined' || v.item) && $.isNumber(v.length));
            }
        };

    })());

    //扩展字符串对象    
    $.extend(String, (function () {

        var
        _escape = /('|\\|")/g,
        _r1 = /<script[^>.]*>[sS]*?<\/script>/gim,
        _r2 = /<style[^>.]*>[sS]*?<\/style>/gim,
        _r3 = /<(.| )+?>/gim;


        return {
            escape: typeof String.escape !== 'undefined' ? String.escape : function (s) {

                if ($.isEmpty(s, false)) {
                    return s;
                }
                return s.replace(_escape, '\\$1');

            },
            format: typeof String.format !== 'undefined' ? String.format : function () {
                var len = arguments.length;

                if (len <= 0) {
                    return '';
                }
                if (len == 1) {
                    return arguments[0];
                }

                var s = arguments[0];
                if ($.isEmpty(s, false)) {
                    return s;
                }

                for (var i = 1; i < len; i++) {
                    s = s.replace(new RegExp('\\{' + (i - 1) + '\\}', 'g'), arguments[i]);
                }

                return s;

            },
            htmlEncode: function (s) {
                return (s + '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\"/g, '&quot;');
            },
            htmlDecode: function (s) {
                return (s + '').replace(/&amp;/g, '&').replace(/&quot;/g, '\"').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
            },
            clearHtml: function (html) {
                if ($.isEmpty(html) || !$.isString(html)) return html;
                return html.replace(_r1, '').replace(_r2, '').replace(_r3, '').replace(/&nbsp;/gi, '');
            },
            hasHtmlTag: function (html) {
                return /<\/?[^>]+>/gi.test(html);
            },
            textAreaEncode: function (s) {
                if (s)
                    return (s + '').replace(/ /gi, '&nbsp;').replace(/\n/gi, '<br/>');
                else
                    return s;
            },
            textAreaDecode: function (s) {
                if (s)
                    return (s + '').replace(/\&nbsp;/gi, ' ').replace(/<br\/?>/gi, '\n');
                else
                    return s;
            },
            replaceEnter: function (s) {
                if (s)
                    return (s + '').replace(/\r\n/gi, '<br/>').replace(/\r/gi, '<br/>').replace(/\n/gi, '<br/>');
                else
                    return s;
            },
            buffer: function (s) {
                var _buf = [];
                if (s !== undefined) {
                    _buf.push(s);
                }

                this.append = function () {
                    if (arguments.length > 1) {
                        Array.prototype.push.apply(_buf, arguments);
                    } else if (arguments.length == 1) {
                        if ($.isArray(arguments[0])) {
                            Array.prototype.push.apply(_buf, arguments[0]);
                        } else {
                            _buf.push(arguments[0]);
                        }
                    }
                }
                this.appendLine = function () {
                    this.append.apply(this, arguments);
                    _buf.push('\n');
                }
                this.appendFormat = function () {

                    if (arguments.length == 0) {
                        return;
                    }
                    if (arguments.length == 1) {
                        _buf.push(arguments[0]);
                        return;
                    }

                    _buf.push(String.format.apply(String, arguments));

                }
                this.clear = function () {
                    _buf.length = 0;
                }
                this.toString = this.valueOf = function () {
                    return _buf.join('');
                }
            }
        }

    })());
    $.extend(String.prototype, (function () {

        var _padChr = function (len, chr) {
            var tmp = new Array(len);
            if (len > 10) {// Duff's Device
                var iterations = Math.floor(len / 8), startAt = len % 8, i = 0;
                do {
                    switch (startAt) {
                        case 0: tmp[i++] = chr;
                        case 7: tmp[i++] = chr;
                        case 6: tmp[i++] = chr;
                        case 5: tmp[i++] = chr;
                        case 4: tmp[i++] = chr;
                        case 3: tmp[i++] = chr;
                        case 2: tmp[i++] = chr;
                        case 1: tmp[i++] = chr;
                    }
                    startAt = 0;
                } while ((--iterations >= 0))

            } else {
                for (var i = 0; i < len; i++) {
                    tmp[i] = chr;
                }
            }
            return tmp;
        },
        _leftPad = function (s, len, chr) {
            if (chr.length > 1) {
                chr = chr.charAt(0);
            }
            var a = _padChr(len - s.length, chr);
            a.push(s);

            return a.join('');
        },
        _rightPad = function (s, len, chr) {
            if (chr.length > 1) {
                chr = chr.charAt(0);
            }
            var a = [s];
            Array.prototype.push.apply(a, _padChr(len - s.length, chr));

            return a.join('');
        };

        return {
            trim: typeof String.prototype.trim !== 'undefined' ? String.prototype.trim : function () {
                return $.trim(this);
            },
            toggle: typeof String.prototype.toggle !== 'undefined' ? String.prototype.toggle : function (s1, s2) {
                return this == s1 ? s2 : s1;
            },
            leftPad: typeof String.prototype.leftPad !== 'undefined' ? String.prototype.leftPad : function (len, chr) {
                return _leftPad(this, len, chr);
            },
            rightPad: typeof String.prototype.rightPad !== 'undefined' ? String.prototype.rightPad : function (len, chr) {
                return _rightPad(this, len, chr);
            },
            escape: typeof String.prototype.escape !== 'undefined' ? String.prototype.escape : function () {
                return String.escape(this);
            },
            format: typeof String.prototype.format !== 'undefined' ? String.prototype.format : function () {
                var a = [this];
                Array.prototype.push.apply(a, arguments)
                return String.format.apply(String, a);
            }
        }

    })());

    //扩展Math对象
    $.extend(Math, {

        formatNumber: function (v, p) {

            if ($.isEmpty(v, false)) {//undefined,null,''
                return 'NaN';
            }
            if (!$.isNumber(v)) {//'123.55'
                v = Number(v);
            }
            if (typeof p != 'number') {
                p = 0;
            }

            if (isNaN(v)) return 'NaN';
            if (p <= 0) return Math.floor(v);

            try {
                return v.toFixed(p);
            } catch (e) {
                return 0;
            }
        }

    });

    //扩展日期对象
    $.extend(Date.prototype, {
        add: function (fs, interval) {
            $.dt.add(this, fs, interval);
        },
        subtract: function (fs, interval) {
            $.dt.subtract(this, fs, interval);
        },
        format: function (fs) {
            return $.dt.format(this, fs);
        },
        formatToCrmDate: function (fs) {
            return $.dt.formatToCrmDate(this);
        },
        isLeapYear: function () {
            return $.dt.isLeapYear(this);
        },
        clone: function () {
            return $.dt.clone(this);
        }
    });

    //扩展函数对象
    $.extend(Function.prototype, (function () {

        return {//这几个函数都是冲Ext中移植过来的，非常有用
            createCallback: function () {
                var fn = this, args = arguments;
                return function () {
                    fn.apply(window, args);
                }
            },
            createDelegate: function (obj, args, appendArgs) {

                var fn = this;

                if (!$.isEmpty(args) && !$.isArray(args)) {
                    args = [args];
                }

                return function () {

                    var callArgs = args || arguments;
                    if (appendArgs === true) {

                        callArgs = Array.prototype.slice.call(arguments, 0);
                        Array.prototype.push.apply(callArgs, args);

                    } else if ($.isNumber(appendArgs)) {

                        callArgs = Array.prototype.slice.call(arguments, 0);

                        var applyArgs = [appendArgs, 0]
                        Array.prototype.push.apply(applyArgs, args);

                        Array.prototype.splice.apply(callArgs, applyArgs);

                    }

                    return fn.apply(obj || window, callArgs);

                };
            },
            createInterceptor: function (fcn, scope) {
                var method = this;
                return !$.isFunction(fcn) ?
                    method : function () {
                        var me = this, args = arguments;
                        fcn.target = me;
                        fcn.method = method;
                        return (fcn.apply(scope || me || window, args) !== false) ? method.apply(me || window, args) : null;
                    };
            },
            createSequence: function (fcn, scope) {
                var method = this;
                return (!$.isFunction(fcn)) ?
                    method : function () {
                        var retval = method.apply(this || window, arguments);
                        fcn.apply(scope || this || window, arguments);
                        return retval;
                    };
            },
            defer: function (millis, obj, args, appendArgs) {
                var fn = this.createDelegate(obj, args, appendArgs);
                if (millis > 0) {
                    return setTimeout(fn, millis);
                }
                fn();
                return 0;
            }
        };

    })())

})(jQuery, window);

//========================下面是一些与具体业务无关，但可公用的Ui层小功能========================================================

//========================弹出对话框========================================================
function openNewWindow(url, title, width, height, left, top, resizable) {

    if (typeof width == 'string' && width.indexOf('%') != -1) {
        width = Math.floor(screen.availWidth * (parseFloat(width.replace('%', '')) / 100));
    }
    if (typeof height == 'string' && height.indexOf('%') != -1) {
        height = Math.floor(screen.availWidth * (parseFloat(height.replace('%', '')) / 100));
    }


    if (!left)
        left = (screen.availWidth - width) / 2;
    if (!top)
        top = (screen.availHeight - height) / 2;
    if (resizable !== false)
        resizable = true;

    var parameters = [
        'width=' + Math.min(width, screen.availWidth - 100),
        'height=' + Math.min(height, screen.availHeight - 100),
        'left=' + left,
        'top=' + top,
        'resizable=' + (resizable ? 'yes' : 'no'),
        'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes'
    ];

    if (title) {
        url = $.urlAppend(url, "windowtitle=" + encodeURI(title));
    }

    var re = window.open(url, $.md5(url), parameters.join(','));

    try {
        re.focus();
        return re;
    } catch (e) {

        alert('弹出窗口可能已被浏览器拦截，请确认浏览器是否启用了弹出窗口拦截程序。');
    }

}
function openEditWindow(grid, keyId, baseUrl, title, width, height, left, top, resizable) {

    if (grid) {
        var rows = grid.getSelectedRows();
        if (rows.length == 0) {
            alert('请先选择一行。');
        } else if (rows.length > 1) {
            alert('只能选择一行。');
        } else {
            openNewWindow($.urlAppend(baseUrl, "id=" + rows[0][keyId]), title, width, height, left, top, resizable);
        }
    }

}
function openConfirmWindow(grid, keyId, confirmMsg, cb, customValidate) {

    if (grid) {
        var rows = grid.getSelectedRows();
        if (!rows || rows.length == 0) {
            alert('请先选择一行。');
        } else {
            if (confirm(confirmMsg)) {
                if (cb) {

                    var ids = [];
                    var isfunc = $.isFunction(customValidate);
                    $.foreach_array(function (item) {
                        if (isfunc) {
                            if (customValidate(item))
                                ids.push(item[keyId]);
                        } else {//表示没设置自定义函数的意思
                            ids.push(item[keyId]);
                        }
                    }, rows, window);

                    cb.apply(window, [ids]);

                }
            }
        }
    }

}
function closeWindow(isCallback, cbArgs) {
    
    if ($.isFunction(window['WindowClosing'])) {

        if (window['WindowClosing']() === false) {
            return;
        }
    }

    if (isCallback === true) {
        if (callback(cbArgs) === false)
            return;
    }

    window.opener = null;
    window.open(' ', '_self', ' ');
    window.close();

}

function callback(cbArgs, cbparam) {
    
    var cb = $.urlParam(cbparam || 'callback');
    if (!$.isEmpty(cb) && window.opener) {

        var fn = window.opener[cb];
        if (typeof fn != 'undefined') {
            if (typeof (fn.defer) != 'undefined') {

                var re = true;
                try {
                    if (cbArgs)
                        return fn(cbArgs);
                    else
                        return fn();
                }
                catch (e) {
                }

            }
        }
    }
}
//========================弹出对话框========================================================

//========================文本框空文本========================================================
(function ($) {


    var focusin = function (e) {

        e = e || window.event;
        if (!e) return;
        var o = e.srcElement || e.target;
        if (!o) return;

        var oo = $(o), t = oo.data('emptyText'), val = '';
        if (o.tagName == 'DIV') {
            val = oo.html();
        } else {
            val = oo.val();
        }

        if (val == t) {
            if (o.tagName == 'DIV') {
                oo.html('');
            } else {
                oo.val('');
            }
        }
        oo.removeClass('gray');
        o = oo = e = null;

    }

    var focusout = function (e) {

        e = e || window.event;
        if (!e) return;
        var o = e.srcElement || e.target;
        if (!o) return;

        var oo = $(o), t = oo.data('emptyText'), val = '';
        if (o.tagName == 'DIV') {
            val = oo.html();
        } else {
            val = oo.val();
        }

        if ($.isEmpty(val) || val == t) {
            if (o.tagName == 'DIV') {
                oo.html(t);
            } else {
                oo.val(t);
            }
            oo.addClass('gray');
        } else {
            oo.removeClass('gray');
        }


        o = oo = e = null;

    }

    $.fn.emptyText = function () {

        this.each(function (index, el) {
            el = $(el);
            var t = el.attr('emptyText');
            if (!$.isEmpty(t)) {

                el.data('emptyText', t);

                if (el[0].tagName == 'DIV' && el.html() == '') {
                    el.html(t);
                    el.addClass('gray');
                } else if ((el[0].tagName == 'INPUT' || el[0].tagName == 'TEXTAREA') && el.val() == '') {
                    el.val(t);
                    el.addClass('gray');
                }

                //if ((el[0].tagName == 'DIV' && el.html() == '') || ((el[0].tagName == 'INPUT' || el[0].tagName == 'TEXTAREA') && el.val() == '')) {
                //    el.addClass('gray');
                //}

                //if ($.isEmpty(el.val())) {
                //    el.addClass('gray');
                //    el.val(t);
                //}

                el
                    .focusin(focusin)
                    .focusout(focusout);

            }
        });

    }

    $.fn.disableEmptyText = function () {
        this.removeClass('gray');
    },

    $.fn.valFromEmptyText = function () {

        var t = this.data('emptyText'), val = '';
        var tag = this[0].tagName.toLowerCase();
        if (tag == 'input' || tag == 'textarea') {
            val = this.val();
        } else {
            val = this.html();
        }

        if (val == t) {
            return '';
        } else {
            return val;
        }

    }
    // 输入长度提示
    var inputkeydown = function (param) {
        var val = $(this).val().trim(), tips = param[0], length = param[1];

        tips.html(val.length + "/" + length);

        if (parseInt(val.length) >= length) {
            $(this).val(val.substring(0, length));
            tips.html(length + "/" + length);
        }
    };
    $.fn.inputLengthTips = function (tipsObj, length) {
        if (!(tipsObj && tipsObj.length > 0))
            return;

        var _defLength = $(this).val().length;
        tipsObj.html(_defLength + '/' + length);

        $(this).keydown(function () {
            inputkeydown.call(this, [tipsObj, length]);
        }).keyup(function () {
            inputkeydown.call(this, [tipsObj, length]);
        });
    }


})(jQuery)
//========================文本框空文本========================================================

//========================textArea最大输入长度验证========================================================

function textAreaMaxLen(o) {
    var nMaxLen = o.getAttribute ? parseInt(o.getAttribute("maxlength")) : "",
        val = '';
    if (o.tagName == 'DIV') {
        val = o.innerHTML.replace(/&nbsp;/gi, ' ');
    } else {
        val = o.value;
    }
    if (o.getAttribute && val.length > nMaxLen) {

        val = val.substring(0, nMaxLen);
        if (o.tagName == 'DIV') {
            o.innerHTML = val.replace(/ /g, '&nbsp;');
        } else {
            o.value = val;
        }

        o.blur();
    }
    //debugger
    //var nMaxLen = o.getAttribute ? parseInt(o.getAttribute("maxlength")) : "",
    //    val = '';
    //if (o.tagName == 'DIV') {
    //    val = o.innerHTML;
    //    val = val.replace(/&nbsp;/g, ' ');
    //    val = val.replace(/<[^>]*>/g, '');
    //} else {
    //    val = o.value;
    //}
    //if (o.getAttribute && val.length > nMaxLen) {

    //    val = val.substring(0, nMaxLen);
    //    if (o.tagName == 'DIV') {
    //        val = val.replace(/ /g, '&nbsp;');
    //        o.innerHTML = val;
    //    } else {
    //        o.value = val;
    //    }

    //    //o.blur();
    //}
    //var nMaxLen = o.getAttribute ? parseInt(o.getAttribute("maxlength")) : "",
    //    val = '';
    //if(o.tagName == 'DIV'){
    //    val = o.innerHTML.replace(/&nbsp;/, ' ');
    //    val = val.replace(/<[^>]*>/g, '');
    //} else {
    //    val = o.value;
    //}
    //if (o.getAttribute && val.length > nMaxLen) {
    //    val = val.substring(0, nMaxLen);
    //    //if (o.tagName == 'DIV') {
    //    //    val = val.replace(/ /g, '&nbsp;');
    //    //    o.innerHTML = val;
    //    //} else {
    //    //    o.value = val;
    //    //}
    //    //o.blur();
    //}

    //if (o.tagName == 'DIV') {
    //    o.innerHTML = val.replace(/ /g, '&nbsp;');
    //} else {
    //    o.value = val;
    //}
    //o.focus();
}


//========================textArea最大输入长度验证========================================================

//========================只能输入数字==================================================================
function inputNumber(inputFraction) {

    var e = event || this.constructor.caller.arguments[0];
    var k = e.keyCode || e.charCode;   //48-57是大键盘的数字键，96-105是小键盘的数字键，8是退格符←
    var el, val;

    el = e.target || e.srcElement;
    val = el.tagName == 'DIV' ? el.textContent : el.value;
    if (k == 110 || k == 190) {
        if (val.indexOf('.') != -1) {
            return false;
        }
    }

    if (e.shiftKey) {
        return false;
    }
    if (k == 8 || k == 9
        || (k <= 57 && k >= 48) || (k <= 105 && k >= 96)
        || k == 13 || k == 46 || k == 39 || k == 37 || k == 16 || k == 17 || k == 36 || k == 35 || k == 107 || k == 109 || k == 110 || k == 190 || k == 86 || k == 65 || k == 67) {
        if (typeof inputFraction == "undefined") {

            // 65=A 86=V C=67 能使用Ctrl+A Ctrl+C Ctrl+V
            if (k == 65 || k == 86 || k == 67) {
                if (e.ctrlKey) {
                    return true;
                } else {
                    return false;
                }
            }

            if (k == 190 && e.shiftKey)
                return false;
            else
                return true;
        }
        else {
            if (!inputFraction && (k == 190 || k == 110))
                return false;
            else
                return true;
        }
    }
    else
        return false;
}
function inputNumber1() {
    var e = event || this.constructor.caller.arguments[0];
    var k = e.keyCode || e.charCode;

    if (k == 86 && e.ctrlKey) {
        var v = e.target.value;
        v = v.replace("¥", "");
        if (isNaN(v)) {
            e.target.value = '';
        }
    }
};

//========================只能输入数字==================================================================

//========================保留小数位数==================================================================

function changeDecimal(num, len) {//值：num 小数位：len     
    return parseFloat(parseFloat(num).toFixed(len)); //保留小数并四舍五入
};

//========================保留小数位数==================================================================

//========================不能输入中文==================================================================

function notInputChinese(me) {
    var reg = /[\u4E00-\u9FA5]|[\uFE30-\uFFA0]/g;
    var _me = $(me);
    var val = _me.val();

    if (reg.test(_me.val())) {
        val = val.replace(reg, '');
        _me.val(val);
        _me.focus();
    }
}

//========================不能输入中文==================================================================

//========================时间格式验证==================================================================

function validateDatetime(time) {
    var reg = /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))(\s(([01]\d{1})|(2[0123])):([0-5]\d):([0-5]\d))?$/;

    if (!reg.test(time)) {
        return false;
    }
    return true;
}

//========================时间格式验证==================================================================

//========================透明层========================================================

FullScreenMask = {
    show: function (level) {
        if ($("#common_mask").length > 0) {

            $("#common_mask")
                .css({ "height": $(document).height(), "width": "100%" })
                .removeClass("hide");

        } else {

            if ((typeof level == "undefined") || (level != 50 && level != 60 && level != 80)) {
                level = 30;
            }

            $("<div>")
                .attr("id", "common_mask")
                .css({ "height": $(document).height(), "width": "100%" })
                .addClass("tran" + level)
                .appendTo("body");
        }

        $(window).resize(function () {

            $("#common_mask").css('height', $(document).height());

        });
    },

    hide: function () {
        $("#common_mask").addClass("hide");
    },

    setZindex: function (index) {
        index = index || 10000;
        $("#common_mask").css("z-index", index);
    }
}
//========================透明层========================================================

//========================加载JS/CSS========================================================
function loadResources(data) {
    if (data) {
        for (var i = 0, len = data.length, item, url, type; i < len; i++) {
            item = data[i];
            url = item.url;
            type = item.type;

            if (url && url.length > 0 && type && type.length > 0) {
                type = type.toLowerCase();
                var iHead = document.getElementsByTagName('head').item(0);

                if (type == "css") {
                    var css = document.createElement("link");
                    css.type = "text/css";
                    css.charset = "utf-8";
                    css.src = url;
                    iHead.appendChild(css);
                } else if (type == "javascript") {
                    var iScript = document.createElement("script");
                    iScript.type = "text/javascript";
                    iScript.charset = "utf-8";
                    iScript.src = url;
                    iHead.appendChild(iScript);
                }
            }
        }
    }
};

//========================swfupload错误处理========================================================
var swfuploadErrorCodeToString = function (code) {
    switch (code) {
        case SWFUpload.UPLOAD_ERROR.HTTP_ERROR: return "HTTP错误";
        case SWFUpload.UPLOAD_ERROR.MISSING_UPLOAD_URL: return "上传文件URL不正确";
        case SWFUpload.UPLOAD_ERROR.IO_ERROR: return "IO错误";
        case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR: return "安全错误";
        case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED: return "上传被限制";
        case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED: return "上传失败";
        case SWFUpload.UPLOAD_ERROR.SPECIFIED_FILE_ID_NOT_FOUND: return "文件未找到";
        case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED: return "文件验证失败";
        case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED: return "上传被取消";
        case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED: return "上传被停止";
    }
    return "";
}

//========================文本框中插入文本到指定文本框(兼容DIV和input和textarea)========================================================
function cursorControl(a) {
    this.element = a;
};
cursorControl.prototype = {
    getType: function () {
        return Object.prototype.toString.call(this.element).match(/^\[object\s(.*)\]$/)[1];
    },
    getStart: function () {
        var start;
        if (this.element.selectionStart || this.element.selectionStart == '0') {
            start = this.element.selectionStart;
        }
        else if (window.getSelection) {
            var rng = window.getSelection().getRangeAt(0).cloneRange();
            rng.setStart(this.element, 0);
            start = rng.toString().length;
        };
        return start;
    },
    insertText: function (text) {
        this.element.focus();
        if (document.all) {
            var c = document.selection.createRange();
            document.selection.empty();
            c.text = text;
            c.collapse();
            c.select();
        }
        else {
            var start = this.getStart();
            if (this.getType() == 'HTMLDivElement') {
                var txt = this.element.innerHTML.replace(/&nbsp;/gi, ' ');
                txt = txt.substr(0, start) + text + txt.substr(start);
                this.element.innerHTML = txt.replace(/ /g, '&nbsp;');
            } else {
                this.element.value = this.element.value.substr(0, start) + text + this.element.value.substr(start);
            }
        }
    }
};
function insertStringToDivOrText(id, text) {
    var cc = new cursorControl(document.getElementById(id));
    cc.insertText(text);
};
//========================文本框中插入文本到指定文本框(兼容DIV和input和textarea)========================================================



//========================文本框中插入文本到指定文本框========================================================
var insertStringToTextAreea = function (tbid, str) {
    str = str || "";
    var tb = document.getElementById(tbid);

    // 第二次点击则是选中话题
    if (tb.value.indexOf("#") != -1 && str.indexOf("#") != -1) {

        var start = tb.value.indexOf("#") + 1;
        var tempStr = tb.value.substring(start, tb.value.length);
        var tempIndex = tempStr.indexOf("#");
        var end = start + tempIndex;
        if (start >= end)
            return;

        //textarea = textarea[0];
        if (typeof tb.createTextRange != 'undefined')// IE 
        {
            var range = tb.createTextRange();
            // 先把相对起点移动到0处 
            range.moveStart("character", 0)
            range.moveEnd("character", 0);
            range.collapse(true); // 移动插入光标到start处 
            range.moveEnd("character", end);
            range.moveStart("character", start);
            range.select();
            //r.text = str;
        } else if (typeof tb.setSelectionRange != 'undefined') {
            //tb.value = tb.value.substr(0, start-1) + str + tb.value.substring(end+1);
            tb.setSelectionRange(start, end);
            tb.focus();
        }
        return;
    }

    tb.focus();
    if (document.all) {
        var r = document.selection.createRange();
        document.selection.empty();
        r.text = str;
        r.collapse();
        r.select();
    }
    else {
        var newstart = tb.selectionStart + str.length;
        tb.value = tb.value.substr(0, tb.selectionStart) + str + tb.value.substring(tb.selectionEnd);
        tb.selectionStart = newstart;
        tb.selectionEnd = newstart;
    }
}
//========================文本框中插入文本到指定文本框========================================================


//========================创建HttpRequest对象========================================================
var createXmlHttpRequest = (function () {

    var lastSuccessHttpRequestObj;
    var versions = [
        'Msxml2.XMLHTTP.7.0',
        'Msxml2.XMLHTTP.6.0',
        'Msxml2.XMLHTTP.5.0',
        'Msxml2.XMLHTTP.4.0',
        'MSXML2.XMLHTTP.3.0',
        'MSXML2.XMLHTTP',
        'MSXML.XMLHTTP',
        'Microsoft.XMLHTTP'
    ];

    return function () {

        var request = false;

        if (window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else if (window.ActiveXObject) {

            if (lastSuccessHttpRequestObj !== undefined) {

                return new ActiveXObject(lastSuccessHttpRequestObj);
                if (request) {
                    return request;
                }

            } else {

                for (var i = 0, len = versions.length; i < len; i++) {
                    try {
                        request = new ActiveXObject(versions[i]);
                        if (request) {
                            lastSuccessHttpRequestObj = versions[i];
                            return request;
                        }
                    } catch (e) {
                        request = false;
                    }
                }

            }

        }

        return request;

    }

})()
//========================创建HttpRequest对象========================================================

//========================表情转换========================================================

var emotionReplace = function (text, emoUrl, ceUrl) {

    var regEmotion = /\[(\d+)\]/gi;  //预定表情
    var customEmotion = /(\[E\:(\d+)\|(\d+)\|(\d+)\]+)/gi; //自定义表情

    var s = '<img src=\"' + emoUrl.replace("{0}", "$1") + '" />';
    text = text.replace(regEmotion, s);
    text = text.replace(customEmotion, '<img name="$1" src="' + ceUrl + '" />');

    return text;
};

var asyncReplaceCustomFace = function (text, ceUrl, cb) {
    if (!text || text.length == 0) {
        return;
    }
    var m = text.match(/(\[E\:(?:\d+)\|(?:\d+)\|(?:\d+)\]+)/gi);
    if (!m || m.length == 0) {
        return;
    }
    var param = { content: m.join(','), __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };

    $.post(ceUrl, param, function (result) {
        if (result.error) {
            if (cb) cb('');
        } else {
            if (result.value) {
                if (cb) {
                    cb(result.value);
                }
            }
        }
    });
}

//========================表情转换========================================================



//========================退格键问题========================================================

function fnBanBackSpace(ev) {

    var obj = ev.target || ev.srcElement;
    var code = ev.keyCode || ev.charCode;

    if (code == 8) {

        if (obj.tagName == "TEXTAREA")
            return true;
        if (obj.tagName == "INPUT") {

            var types = ["text", "password", "color", "date", "datetime", "datetime-local", "month", "week", "time", "email", "number", "search", "tel", "url"];
            for (var i = 0, len = types.length; i < len; i++) {
                if (types[i] == obj.type)
                    return true;
            }
        }
        if (obj.tagName == 'DIV' && obj.getAttribute('contenteditable')) {
            return true;
        }

        return false;
    }

}
$(document.body)
    .keydown(fnBanBackSpace)//禁止后退键 作用于IE、Chrome  
    .keypress(fnBanBackSpace)//禁止后退键 作用于Firefox、Opera

//========================退格键问题========================================================

//========================公共树形维护========================================================

function openTree(treeType, title, width, height, cb) {

    if ($G && $G.virtualPath) {
        var url = location.protocol
            + '//'
            + location.hostname
            + (location.port != 80 ? ":" + location.port : "")
            + $G.virtualPath
            + 'Tree';
    }

    var p = { treeType: treeType, callback: cb }
    openNewWindow(url + "?" + $.param(p), title, width, height);

}

//========================公共树形维护========================================================

//========================选择产品begin========================================================

function openProduct(title, width, heigth, selectModel, cb, hideFirstBtn) {

    var url = location.protocol
        + '//'
        + location.hostname
        + (location.port != 80 ? ":" + location.port : "")
        + $G.virtualPath
        + 'archives/product/selectproduct';

    var p = { selectModel: selectModel, callback: cb };

    if (typeof hideFirstBtn != 'undefined') {
        p.hideFirstBtn = hideFirstBtn;
    }
    openNewWindow(url + "?" + $.param(p), title, width || 700, heigth || 500);

}

//========================选择产品end========================================================

//========================解决contenteditableDIV粘贴带标签的问题begin========================================================
/*
(function () {

    var removeTag = function (el, event) {

        var el = el[0],
            txt = el.innerText,
            children = el.childNodes;

        if (txt) {

            txt = txt.replace(/[\r\n?]/g, '<br/>');
            el.innerHTML = txt;
        } else {

            var buf = [];
            for (var i = 0, len = children.length, item, txt, isBr = false; i < len; i++) {

                item = children[i];

                if (item.nodeName != 'BR' || isBr == true) {
                    if (isBr && item.nodeName == 'BR') {
                        buf.push('');
                    } else {
                        isBr = false;
                        txt = $(item).text();
                        buf.push(txt);
                    }
                    
                } else {
                    isBr = true;
                }
            }

            el.innerHTML = buf.join('<br />');
        }

    }

    var timer = null;
    $('[contenteditable="true"]').live('paste', function (e) {
        
        if (timer != null) {
            clearTimeout(timer);
            timer = null; 
        }

        console.log(e);
        var _this = $(this),
            _event = e;

        timer = setTimeout(function () {
            removeTag(_this, _event);
        }, 1000);
        
    });
})();
*/
//========================解决contenteditableDIV粘贴带标签的问题end========================================================


var validateMobileNo = function (_val) {

    var reg = /1[34578]\d{9}/;
    if (!reg.test(_val)) {
        return false;
    }

    if (_val.length != 11) {
        return false;
    }

    return true;
}

var validateFixPhone = function (_val) {

    var reg = /^\d{1,4}(\-?\d{2,11})*$/;
    return reg.test(_val);

}

var validateMail = function (m) {
    return /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(m);
};