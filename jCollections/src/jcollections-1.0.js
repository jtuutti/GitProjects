/*!
 * jCollections JavaScript Library v1.0
 * Copyright 2014, Dmitry Starosta
 * Released under the MIT license
 */
var __collectionsInitialized = (function (undefined) {
    'use strict';

    var dataStructures = {},
        errorMessages = {
            ABSTRACT_COLLECTION_INIT: '"Collection" is an abstract class that cannot be initialized directly',
            DUPLICATE_KEY: 'Value with the same key already exists',
            INVALID_COLLECTION: 'Invalid collection provided',
            INVALID_COUNT_VALUE: 'Count must be a positive integer',
            INVALID_INDEX: 'Index must be a non-negative integer',
            INVALID_KEY_SELECTOR: 'Invalid key selector function provided',
            INVALID_KEY_VALUE_COLLECTION: 'The initial item collection can only contain key/value objects',
            INVALID_RESULT_SELECTOR: 'Invalid result selector function provided',
            INVALID_VALUE_ARRAY: 'Invalid array of values provided',
            UNCOMPARABLE_OBJECT: 'Object(s) cannot be compared using a built-in comparer',
            RESERVED_PROPERTY_NAME: 'Reserved property name "%s"',
            UNDEFINED_KEY: 'Key must be defined',
            UNDEFINED_VALUE: 'Value must be defined',
            UNINITIALIZED_CLASS: 'The keyword "this" is not defined. Did you forget the "new" operator?'
        },
        isNumber = function (value) {
            return !Array.isArray(value) && (value - parseFloat(value) + 1) >= 0;
        },
        isFunction = function (obj) {
            return !!obj && Object.prototype.toString.call(obj) === '[object Function]';
        },
        initializeItems = function (items) {
            if (!items || items.length === 0) {
                return [];
            }
            if (items.length === 1) {
                var arg = items[0];
                if (Array.isArray(arg)) {
                    return arg.slice(0);
                } else if (arg instanceof dataStructures.Collection) {
                    return arg.toArray();
                } else {
                    return [arg];
                }
            } else {
                return Array.prototype.slice.call(items, 0);
            }
        },
        clearItems = function (collection) {
            if (!collection) {
                return;
            }
            delete collection.__inner.items;
            collection.__inner.items = [];
            collection.length = 0;
        },
        convertToArray = function (col) {
            if (col !== undefined && col !== null) {
                if (Array.isArray(col)) {
                    return col.slice(0);
                } else if (col instanceof dataStructures.Collection) {
                    return col.toArray();
                }
            }
            throw new TypeError(errorMessages.INVALID_COLLECTION);
        },
        findIndex = function (item, items, equalityComparer) {
            var index = -1, length = items.length, i;
            for (i = 0; i < length; i++) {
                if (equalityComparer(item, items[i])) {
                    index = i;
                    break;
                }
            }
            return index;
        },
        stringFormat = function (format) {
            if (!format) {
                return format;
            }
            var i;
            for (i = 1; i < arguments.length; i++) {
                format = format.replace(/%s/, arguments[i]);
            }
            return format;
        };

    // Comparer implementations
    dataStructures.Comparer = {
        standard: function (first, second) {
            if (first === second) {
                return 0;
            }
            if (first === undefined || first === null || (isNumber(first) && isNumber(second) && first < second)) {
                return -1;
            }
            if (second === undefined || second === null || (isNumber(first) && isNumber(second) && second < first)) {
                return 1;
            }
            var objectStr = "[object Object]",
                firstStr = first.toString(),
                secondStr = second.toString();
            if (firstStr === objectStr || secondStr === objectStr) {
                throw new TypeError(errorMessages.UNCOMPARABLE_OBJECT);
            }
            return firstStr.localeCompare(secondStr);
        },
        reverse: function (first, second) {
            if (first === second) {
                return 0;
            }
            if (first === undefined || first === null || (isNumber(first) && isNumber(second) && first < second)) {
                return 1;
            }
            if (second === undefined || second === null || (isNumber(first) && isNumber(second) && second < first)) {
                return -1;
            }
            var objectStr = "[object Object]",
                firstStr = first.toString(),
                secondStr = second.toString();
            if (firstStr === objectStr || secondStr === objectStr) {
                throw new TypeError(errorMessages.UNCOMPARABLE_OBJECT);
            }
            return secondStr.localeCompare(firstStr);
        }
    };


    // Equality comparer implementations
    dataStructures.EqualityComparer = {
        standard: function (first, second) {
            return first === second;
        },
        caseInsensitive: function (first, second) {
            if (typeof first !== 'string' || typeof second !== 'string') {
                return first === second;
            } else {
                return first.toLocaleUpperCase() === second.toLocaleUpperCase();
            }
        }
    };


    // Key/value pair implementation
    dataStructures.KeyValue = function (key, value) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        this.key = key;
        this.value = value;
    };

    dataStructures.KeyValue.prototype.toString = function () {
        return this.key.toString() + ':' + (this.value !== null ? this.value.toString() : '');
    };


    // Base collection implementation
    dataStructures.Collection = function Collection() {
        if (this === undefined) {
            throw new TypeError(errorMessages.UNINITIALIZED_CLASS);
        }
        if (__collectionsInitialized && this.constructor.name === 'Collection') {
            throw new TypeError(errorMessages.ABSTRACT_COLLECTION_INIT);
        }
        this.__inner = {};
        this.length = 0;
    };

    dataStructures.Collection.prototype.size = function () {
        return this.__inner.items.length;
    };

    dataStructures.Collection.prototype.isEmpty = function () {
        return this.size() === 0;
    };

    dataStructures.Collection.prototype.forEach = function (callback) {
        if (this.isEmpty() || !isFunction(callback)) {
            return;
        }
        var items = this.toArray(), i;
        for (i = 0; i < items.length; i++) {
            callback.call(items[i], i);
        }
    };

    dataStructures.Collection.prototype.find = function (predicate) {
        if (this.isEmpty() || !isFunction(predicate)) {
            return undefined;
        }
        var items = this.toArray(), i, item;
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (predicate.call(item)) {
                return item;
            }
        }
        return undefined;
    };

    dataStructures.Collection.prototype.findAll = function (predicate) {
        if (this.isEmpty() || !isFunction(predicate)) {
            return [];
        }
        var items = this.toArray(), matchingItems = [], i, item;
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (predicate.call(item)) {
                matchingItems.push(item);
            }
        }
        return matchingItems;
    };

    dataStructures.Collection.prototype.groupBy = function (keySelector, propertyNameArray) {
        var groupedCol = new dataStructures.Dictionary(),
            selector,
            key,
            values,
            propertyName,
            value,
            i;
        if (typeof keySelector === "string" && keySelector.length > 0) {
            selector = function () {
                return this[keySelector];
            };
        } else if (isFunction(keySelector)) {
            selector = keySelector;
        } else {
            throw new Error(errorMessages.INVALID_KEY_SELECTOR);
        }
        if (propertyNameArray && typeof propertyNameArray === "string") {
            propertyNameArray = [propertyNameArray];
        }
        this.forEach(function() {
            key = selector.call(this);
            if (key === undefined) {
                key = null;
            }
            values = groupedCol.get(key);
            if (!Array.isArray(propertyNameArray) || propertyNameArray.length === 0) {
                if (!values) {
                    values = [];
                }
                if (values.indexOf(this) < 0) {
                    values.push(this);
                }
            } else {
                if (!values) {
                    values = {};
                }
                for (i = 0; i < propertyNameArray.length; i++) {
                    propertyName = propertyNameArray[i];
                    if (!propertyName || typeof propertyName !== "string") {
                        continue;
                    }
                    if (!values.hasOwnProperty(propertyName)) {
                        values[propertyName] = [];
                    }
                    if (this.hasOwnProperty(propertyName)) {
                        value = this[propertyName];
                        if (value !== undefined && values[propertyName].indexOf(value) < 0) {
                            values[propertyName].push(value);
                        }
                    }
                }
            }
            groupedCol.put(key, values);
        });
        return groupedCol;
    };

    dataStructures.Collection.prototype.all = function (predicate) {
        return this.findAll(predicate).length === this.size();
    };

    dataStructures.Collection.prototype.any = function (predicate) {
        return this.find(predicate) !== undefined;
    };

    dataStructures.Collection.prototype.map = function (transform) {
        if (this.isEmpty()) {
            return [];
        }        
        var transformDelegate = isFunction(transform),
            transformProperty = transform && (typeof transform === "string"),
            items = this.toArray(),
            transformedItems = [],
            value,
            i;
        if (!transformDelegate && !transformProperty) {
            return[];
        }
        for (i = 0; i < items.length; i++) {
            value = transformDelegate ? transform.call(items[i]) : items[i][transform];
            transformedItems.push(value);
        }
        return transformedItems;
    };
    
    dataStructures.Collection.prototype.reduce = function (transform, memo, reduceRight) {
        if (this.isEmpty() || !isFunction(transform)) {
            return undefined;
        }
        var items = this.toArray(), i;
        if (reduceRight === true || (memo === true && reduceRight === undefined)) {
          for (i = items.length - 1; i >= 0; i--) {
              memo = transform.call(items[i], memo);
          }
        } else {
          for (i = 0; i < items.length; i++) {
              memo = transform.call(items[i], memo);
          }
        }
        return memo;
    };

    dataStructures.Collection.prototype.toArray = function () {
        return this.__inner.items.slice(0);
    };

    dataStructures.Collection.prototype.toSortedArray = function (comparer) {
        var items = this.toArray();
        items.sort(comparer);
        return items;
    };

    dataStructures.Collection.prototype.toJSON = function () {
        return JSON.stringify(this.toArray());
    };

    dataStructures.Collection.prototype.toString = function () {
        return this.toArray().join(',');
    };

    dataStructures.Collection.avg = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return dataStructures.Collection.sum(arr) / arr.length;
    };

    dataStructures.Collection.concat = function () {
        if (arguments.length === 0) {
            return undefined;
        }
        var tempArr = [],
            arrays = [],
            i;
        for (i = 0; i < arguments.length; i++) {
            arrays.push(convertToArray(arguments[i]));
        }
        return tempArr.concat.apply(tempArr, arrays);
    };

    dataStructures.Collection.distinct = function (col, equalityComparer) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return [];
        }
        if (isFunction(equalityComparer)) {
            return new dataStructures.Set(equalityComparer, arr).toArray();
        }
        return new dataStructures.Set(arr).toArray();
    };

    dataStructures.Collection.equivalent = function (col1, col2, equalityComparer) {
        var arr1 = convertToArray(col1),
            arr2 = convertToArray(col2),
            i;
        if (arr1.length !== arr2.length) {
            return false;
        }
        if (!isFunction(equalityComparer)) {
            equalityComparer = dataStructures.EqualityComparer.standard;
        }
        for (i = 0; i < arr1.length; i++) {
            if (!equalityComparer(arr1[i], arr2[i])) {
                return false;
            }
        }
        return true;
    };

    dataStructures.Collection.first = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return arr[0];
    };

    dataStructures.Collection.flatten = function (col, recursive) {
        var arr = convertToArray(col),
            tempArr = [],
            isFlattened = true,
            flattenedArr,
            item,
            i;
        if (arr.length === 0) {
            return [];
        }
        for (i = 0; i < arr.length; i++) {
            item = arr[i];
            if (item instanceof dataStructures.Collection) {
                arr[i] = item.toArray();
            }
        }
        flattenedArr = tempArr.concat.apply(tempArr, arr);
        if (recursive) {
            flattenedArr.forEach(function (value) {
                if (Array.isArray(value) || value instanceof dataStructures.Collection) {
                    isFlattened = false;
                }
            });
            if (!isFlattened) {
                flattenedArr = dataStructures.Collection.flatten(flattenedArr, true);
            }
        }
        return flattenedArr;
    };

    dataStructures.Collection.get = function (col, index) {
        if (index !== parseInt(index, 10) || index < 0) {
            throw new Error(errorMessages.INVALID_INDEX);
        }
        return convertToArray(col)[index];
    };

    dataStructures.Collection.last = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return arr[arr.length - 1];
    };

    dataStructures.Collection.min = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return Math.min.apply(Math, arr);
    };

    dataStructures.Collection.max = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return Math.max.apply(Math, arr);
    };

    dataStructures.Collection.range = function (col, start, end) {
        return convertToArray(col).slice(start, end);
    };

    dataStructures.Collection.size = function (col) {
        if (col !== undefined && col !== null) {
            if (Array.isArray(col)) {
                return col.length;
            } else if (col instanceof dataStructures.Collection) {
                return col.size();
            }
        }
        throw new TypeError(errorMessages.INVALID_COLLECTION);
    };

    dataStructures.Collection.sum = function (col) {
        var arr = convertToArray(col);
        if (arr.length === 0) {
            return undefined;
        }
        return arr.reduce(function (a, b) {
            return a + b;
        }) / 1;
    };

    dataStructures.Collection.toArray = function (col) {
        if (col === undefined) {
            return undefined;
        }
        if (Array.isArray(col)) {
            return col.slice(0);
        }
        if (col instanceof dataStructures.Collection) {
            return col.toArray();
        }
        return [col];
    };

    dataStructures.Collection.toSortedArray = function (col, comparer) {
        if (col === undefined) {
            return undefined;
        }
        var arr = dataStructures.Collection.toArray(col);
        arr.sort(comparer);
        return arr;
    };

    dataStructures.Collection.zip = function (col1, col2, resultSelector) {
        if (!isFunction(resultSelector)) {
            resultSelector = function (first, second) {
                return [first, second];
            };
        }
        var arr1 = convertToArray(col1),
            arr2 = convertToArray(col2),
            zippedCol = [],
            i;
        for (i = 0; i < arr1.length; i++) {
            zippedCol.push(resultSelector(arr1[i], arr2.length > i ? arr2[i] : null));
        }
        return zippedCol;
    };


    // List collection implementation
    dataStructures.List = function List() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        this.__inner.items = initializeItems(arguments);
        this.length = this.__inner.items.length;
    };
    dataStructures.List.prototype = new dataStructures.Collection();
    dataStructures.List.prototype.constructor = dataStructures.List;

    dataStructures.List.prototype.findIndex = function (item) {
        return this.__inner.items.indexOf(item);
    };

    dataStructures.List.prototype.findLastIndex = function (item) {
        return this.__inner.items.lastIndexOf(item);
    };

    dataStructures.List.prototype.get = function (index) {
        if (index !== parseInt(index, 10) || index < 0) {
            throw new Error(errorMessages.INVALID_INDEX);
        }
        var size = this.size();
        if (size === 0 || index > (size - 1)) {
            return undefined;
        }
        return this.__inner.items[index];
    };

    dataStructures.List.prototype.contains = function (item) {
        return this.__inner.items.indexOf(item) >= 0;
    };

    dataStructures.List.prototype.add = function (item) {
        this.__inner.items.push(item);
        this.length = this.__inner.items.length;
    };

    dataStructures.List.prototype.addRange = function () {
        var items = initializeItems(arguments), i;
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };

    dataStructures.List.prototype.insert = function (index, item) {
        this.__inner.items.splice(index, 0, item);
        this.length = this.__inner.items.length;
    };

    dataStructures.List.prototype.moveTo = function (index, item) {
        if (!this.remove(item)) {
            return false;
        }
        this.__inner.items.splice(index, 0, item);
        this.length++;
        return true;
    };

    dataStructures.List.prototype.remove = function (item) {
        var index = this.findIndex(item), i;
        if (index < 0) {
            return false;
        }
        for (i = this.__inner.items.length - 1; i >= 0; i--) {
            if (this.__inner.items[i] === item) {
                this.__inner.items.splice(i, 1);
            }
        }
        this.length = this.__inner.items.length;
        return true;
    };

    dataStructures.List.prototype.removeAt = function (index) {
        var originalLength = this.__inner.items.length;
        this.__inner.items.splice(index, 1);
        this.length = this.__inner.items.length;
        return originalLength > this.length;
    };

    dataStructures.List.prototype.removeWhere = function (predicate) {
        if (this.isEmpty() || !isFunction(predicate)) {
            return false;
        }
        var originalLength = this.__inner.items.length, i;
        for (i = originalLength - 1; i >= 0; i--) {
            if (predicate.call(this.__inner.items[i], i)) {
                this.__inner.items.splice(i, 1);
            }
        }
        this.length = this.__inner.items.length;
        return originalLength > this.length;
    };
    
    dataStructures.List.prototype.reverse = function () {
        this.__inner.items.reverse();
        return this;
    };

    dataStructures.List.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        clearItems(this);
    };

    dataStructures.List.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.List(arr);
    };


    // Linked List collection implementation
    dataStructures.LinkedList = function LinkedList() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        this.head = null;
        this.tail = null;
        this.length = 0;
        var items = initializeItems(arguments), i;
        for (i = 0; i < items.length; i++) {
            this.addLast(items[i]);
        }
    };
    dataStructures.LinkedList.prototype = new dataStructures.Collection();
    dataStructures.LinkedList.prototype.constructor = dataStructures.LinkedList;

    dataStructures.LinkedList.Node = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        this.value = value;
        this.prev = null;
        this.next = null;
    };

    dataStructures.LinkedList.prototype.size = function () {
        var i = 0,
            node = this.head;
        while (node) {
            i++;
            node = node.next;
        }
        return i;
    };

    dataStructures.LinkedList.prototype.contains = function (value) {
        var node = this.head;
        while (node) {
            if (node.value === value) {
                return true;
            }
            node = node.next;
        }
        return false;
    };

    dataStructures.LinkedList.prototype.findIndex = function (value) {
        var i = 0,
            node = this.head;
        while (node) {
            if (node.value === value) {
                return i;
            }
            i++;
            node = node.next;
        }
        return -1;
    };

    dataStructures.LinkedList.prototype.findLastIndex = function (value) {
        var i = this.length - 1,
            node = (i <= 1) ? this.head : this.tail;
        while (node && i >= 0) {
            if (node.value === value) {
                return i;
            }
            i--;
            node = node.prev;
        }
        return -1;
    };

    dataStructures.LinkedList.prototype.get = function (index) {
        if (index !== parseInt(index, 10) || index < 0) {
            throw new Error(errorMessages.INVALID_INDEX);
        }
        var i = 0,
            node = this.head;
        while (node && i <= index) {
            if (i === index) {
                return node.value;
            }
            i++;
            node = node.next;
        }
        return undefined;
    };

    dataStructures.LinkedList.prototype.addFirst = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var node = new dataStructures.LinkedList.Node(value),
            firstNode;
        if (!this.head) {
            this.head = node;
        } else {
            firstNode = this.head;
            this.head = node;
            this.head.next = firstNode;
            firstNode.prev = this.head;
            if (!this.tail) {
                this.tail = firstNode;
                this.tail.next = null;
            }
        }
        this.length++;
    };

    dataStructures.LinkedList.prototype.addLast = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var node = new dataStructures.LinkedList.Node(value),
            lastNode;
        if (!this.head) {
            this.head = node;
        } else if (!this.tail) {
            this.tail = node;
            this.tail.prev = this.head;
            this.head.next = this.tail;
        } else {
            lastNode = this.tail;
            this.tail = node;
            this.tail.prev = lastNode;
            lastNode.next = this.tail;
        }
        this.length++;
    };

    dataStructures.LinkedList.prototype.removeFirst = function () {
        if (!this.head) {
            return false;
        }
        this.head = this.head.next;
        if (this.tail === this.head) {
            this.tail = null;
        }
        this.length--;
        return true;
    };

    dataStructures.LinkedList.prototype.removeLast = function () {
        if (!this.head && !this.tail) {
            return false;
        }
        if (!this.tail) {
            this.head = null;
        } else if (this.tail.prev && this.tail.prev !== this.head) {
            this.tail.prev.next = null;
            this.tail = this.tail.prev;
        } else {
            this.head.next = null;
            this.tail = null;
        }
        this.length--;
        return true;
    };

    dataStructures.LinkedList.prototype.remove = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        if (!this.head) {
            return false;
        }
        var originalLength = this.length,
            node = this.head;
        while (node) {
            if (node.value === value) {
                if (!node.prev) {
                    this.head = node.next;
                    if (this.head) {
                        this.head.prev = null;
                        if (this.head === this.tail) {
                            this.tail = null;
                        }
                    }
                } else if (!node.next) {
                    this.tail = node.prev;
                    if (this.tail) {
                        this.tail.next = null;
                        if (this.head === this.tail) {
                            this.tail = null;
                        }
                    }
                } else {
                    node.next.prev = node.prev;
                    node.prev.next = node.next;
                }
                this.length--;
            }
            node = node.next;
        }
        return this.length < originalLength;
    };

    dataStructures.LinkedList.prototype.clear = function () {
        this.head = null;
        this.tail = null;
        this.length = 0;
    };

    dataStructures.LinkedList.prototype.toArray = function () {
        var values = [],
            node = this.head;
        while (node) {
            values.push(node.value);
            node = node.next;
        }
        return values;
    };

    dataStructures.LinkedList.prototype.toReverseArray = function () {
        var values = [],
            node = this.tail || this.head;
        while (node) {
            values.push(node.value);
            node = node.prev;
        }
        return values;
    };

    dataStructures.LinkedList.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.LinkedList(arr);
    };


    // Set collection implementation
    dataStructures.Set = function Set() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments), i;
        if (arguments.length > 0 && isFunction(items[0])) {
            this.__inner.equalityComparer = items[0];
            items.splice(0, 1);
            items = initializeItems(items);
        } else {
            this.__inner.equalityComparer = dataStructures.EqualityComparer.standard;
        }
        this.__inner.items = [];
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };
    dataStructures.Set.prototype = new dataStructures.Collection();
    dataStructures.Set.prototype.constructor = dataStructures.Set;

    dataStructures.Set.prototype.getEqualityComparer = function () {
        return this.__inner.equalityComparer;
    };

    dataStructures.Set.prototype.contains = function (item) {
        return findIndex(item, this.__inner.items, this.__inner.equalityComparer) >= 0;
    };

    dataStructures.Set.prototype.add = function (item) {
        if (this.contains(item)) {
            return false;
        }
        this.__inner.items.push(item);
        this.length = this.__inner.items.length;
        return true;
    };

    dataStructures.Set.prototype.addRange = function () {
        var items = initializeItems(arguments), i;
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };

    dataStructures.Set.prototype.remove = function (item) {
        var index = findIndex(item, this.__inner.items, this.__inner.equalityComparer);
        if (index < 0) {
            return false;
        }
        this.__inner.items.splice(index, 1);
        this.length = this.__inner.items.length;
        return true;
    };

    dataStructures.Set.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        clearItems(this);
    };

    dataStructures.Set.prototype.union = function (set) {
        var targetSet = new dataStructures.Set(this.__inner.equalityComparer, this.__inner.items);
        targetSet.addRange(set);
        return targetSet;
    };

    dataStructures.Set.prototype.intersection = function (set) {
        var targetSet = new dataStructures.Set(this.__inner.equalityComparer),
            items = initializeItems(arguments),
            item,
            i;
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (this.contains(item)) {
                targetSet.add(item);
            }
        }
        return targetSet;
    };

    dataStructures.Set.prototype.difference = function (set) {
        var targetSet = new dataStructures.Set(this.__inner.equalityComparer),
            items = initializeItems(arguments),
            item1,
            item2,
            found,
            i,
            j;
        for (i = 0; i < this.__inner.items.length; i++) {
            item1 = this.__inner.items[i];
            found = false;
            for (j = 0; j < items.length; j++) {
                item2 = items[j];
                if (this.__inner.equalityComparer(item1, item2)) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                targetSet.add(item1);
            }
        }
        return targetSet;
    };
    
    dataStructures.Set.prototype.equals = function (set) {
        var items = initializeItems(arguments), i;
        if (items.length !== this.__inner.items.length) {
            return false;
        }
        for (i = 0; i < this.__inner.items.length; i++) {
            if (!this.__inner.equalityComparer(items[i], this.__inner.items[i])) {
                return false;
            }
        }
        return true;
    };

    dataStructures.Set.prototype.isProperSubsetOf = function (set) {
        var items = new dataStructures.Set(this.__inner.equalityComparer, initializeItems(arguments));
        return this.difference(items).length === 0 && !this.equals(items);
    };

    dataStructures.Set.prototype.isProperSupersetOf = function (set) {
        var items = new dataStructures.Set(this.__inner.equalityComparer, initializeItems(arguments));
        return items.difference(this).length === 0 && !this.equals(items);
    };

    dataStructures.Set.prototype.isSubsetOf = function (set) {
        var items = new dataStructures.Set(this.__inner.equalityComparer, initializeItems(arguments));
        return this.difference(items).length === 0;
    };

    dataStructures.Set.prototype.isSupersetOf = function (set) {
        var items = new dataStructures.Set(this.__inner.equalityComparer, initializeItems(arguments));
        return items.difference(this).length === 0;
    };

    dataStructures.Set.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Set(arr);
    };


    // Bag collection implementation
    dataStructures.Bag = function Bag () {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments), i;
        if (arguments.length > 0 && isFunction(items[0])) {
            this.__inner.equalityComparer = items[0];
            items.splice(0, 1);
            items = initializeItems(items);
        } else {
            this.__inner.equalityComparer = dataStructures.EqualityComparer.standard;
        }
        this.__inner.values = [];
        this.__inner.counts = [];
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };
    dataStructures.Bag.prototype = new dataStructures.Collection();
    dataStructures.Bag.prototype.constructor = dataStructures.Bag;

    dataStructures.Bag.prototype.getEqualityComparer = function () {
        return this.__inner.equalityComparer;
    };

    dataStructures.Bag.prototype.size = function () {
        var counts = 0, length = this.__inner.counts.length, i;
        for (i = 0; i < length; i++) {
            counts += this.__inner.counts[i];
        }
        return counts;
    };

    dataStructures.Bag.prototype.distinctSize = function () {
        return this.__inner.values.length;
    };

    dataStructures.Bag.prototype.contains = function (value) {
        return findIndex(value, this.__inner.values, this.__inner.equalityComparer) >= 0;
    };

    dataStructures.Bag.prototype.count = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var index = findIndex(value, this.__inner.values, this.__inner.equalityComparer);
        if (index < 0) {
            return 0;
        }
        return this.__inner.counts[index];
    };

    dataStructures.Bag.prototype.add = function (value, count) {
        var index = findIndex(value, this.__inner.values, this.__inner.equalityComparer);
        if (count === undefined) {
            count = 1;
        }
        if (count !== parseInt(count, 10) || count < 1) {
            throw new Error(errorMessages.INVALID_COUNT_VALUE);
        }
        if (index >= 0) {
            this.__inner.counts[index] += count;
        } else {
            this.__inner.values.push(value);
            this.__inner.counts.push(count);
        }
        this.length = this.size();
    };

    dataStructures.Bag.prototype.setCount = function (value, count) {
        var index = findIndex(value, this.__inner.values, this.__inner.equalityComparer);
        if (count === undefined || count === null || count !== parseInt(count, 10) || count < 1) {
            throw new Error(errorMessages.INVALID_COUNT_VALUE);
        }
        if (index >= 0) {
            this.__inner.counts[index] = count;
        } else {
            this.__inner.values.push(value);
            this.__inner.counts.push(count);
        }
        this.length = this.size();
    };

    dataStructures.Bag.prototype.remove = function (value) {
        var index = findIndex(value, this.__inner.values, this.__inner.equalityComparer);
        if (index < 0) {
            return false;
        }
        this.__inner.values.splice(index, 1);
        this.__inner.counts.splice(index, 1);
        this.length = this.size();
        return true;
    };

    dataStructures.Bag.prototype.toArray = function () {
        var values = [], value, count, i, j;
        for (i = 0; i < this.__inner.values.length; i++) {
            value = this.__inner.values[i];
            count = this.__inner.counts[i];
            for (j = 0; j < count; j++) {
                values.push(value);
            }
        }
        return values;
    };

    dataStructures.Bag.prototype.toDistinctArray = function () {
        return this.__inner.values.slice(0);
    };

    dataStructures.Bag.prototype.toString = function () {
        var str = '', value, count, i, j;
        for (i = 0; i < this.__inner.values.length; i++) {
            value = this.__inner.values[i];
            count = this.__inner.counts[i];
            for (j = 0; j < count; j++) {
                if (str.length > 0) {
                    str += ',';
                }
                str += value;
            }
        }
        return str;
    };

    dataStructures.Bag.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        delete this.__inner.values;
        delete this.__inner.counts;
        this.__inner.values = [];
        this.__inner.counts = [];
        this.length = 0;
    };

    dataStructures.Bag.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Bag(arr);
    };


    // Dictionary collection implementation
    dataStructures.Dictionary = function Dictionary() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments),
            ctor = this,
            item,
            i;
        if (items.length > 0 && isFunction(items[0])) {
            this.__inner.equalityComparer = items[0];
            items.splice(0, 1);
            items = initializeItems(items);
        } else {
            this.__inner.equalityComparer = dataStructures.EqualityComparer.standard;
        }
        this.__inner.keys = [];
        this.__inner.values = [];
        this.__inner.findIndex = function (key) {
            return findIndex(key, ctor.__inner.keys, ctor.__inner.equalityComparer);
        };
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (!item || !item.hasOwnProperty('key') || !item.hasOwnProperty('value')) {
                throw new Error(errorMessages.INVALID_KEY_VALUE_COLLECTION);
            }
            this.add(item.key, item.value);
        }
    };
    dataStructures.Dictionary.prototype = new dataStructures.Collection();
    dataStructures.Dictionary.prototype.constructor = dataStructures.Dictionary;

    dataStructures.Dictionary.prototype.getEqualityComparer = function () {
        return this.__inner.equalityComparer;
    };

    dataStructures.Dictionary.prototype.size = function () {
        return this.__inner.keys.length;
    };

    dataStructures.Dictionary.prototype.containsKey = function (key) {
        return this.__inner.findIndex(key) >= 0;
    };

    dataStructures.Dictionary.prototype.get = function (key) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        var index = this.__inner.findIndex(key);
        if (index < 0) {
            return undefined;
        }
        return this.__inner.values[index];
    };

    dataStructures.Dictionary.prototype.add = function (key, value) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        if (this.__inner.findIndex(key) >= 0) {
            throw new Error(errorMessages.DUPLICATE_KEY);
        }
        this.__inner.keys.push(key);
        this.__inner.values.push(value);
        this.length = this.__inner.keys.length;
    };

    dataStructures.Dictionary.prototype.put = function (key, value) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var index = this.__inner.findIndex(key);
        if (index >= 0) {
            this.__inner.values[index] = value;
            return;
        }
        this.__inner.keys.push(key);
        this.__inner.values.push(value);
        this.length = this.__inner.keys.length;
    };

    dataStructures.Dictionary.prototype.remove = function (key) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        var index = this.__inner.findIndex(key);
        if (index < 0) {
            return false;
        }
        this.__inner.keys.splice(index, 1);
        this.__inner.values.splice(index, 1);
        this.length = this.__inner.keys.length;
        return true;
    };

    dataStructures.Dictionary.prototype.keys = function () {
        return this.__inner.keys.slice(0);
    };

    dataStructures.Dictionary.prototype.values = function () {
        var values = [], value, i;
        for (i = 0; i < this.__inner.values.length; i++) {
            value = this.__inner.values[i];
            if (values.indexOf(value) < 0) {
                values.push(value);
            }
        }
        return values;
    };

    dataStructures.Dictionary.prototype.toArray = function () {
        var items = [], i;
        for (i = 0; i < this.__inner.keys.length; i++) {
            items.push(new dataStructures.KeyValue(this.__inner.keys[i], this.__inner.values[i]));
        }
        return items;
    };

    dataStructures.Dictionary.prototype.toSortedArray = function (comparer) {
        var items = [],
            keys = this.keys(),
            key,
            i;
        keys.sort(comparer);
        for (i = 0; i < keys.length; i++) {
            key = keys[i];
            items.push(new dataStructures.KeyValue(key, this.__inner.values[this.__inner.keys.indexOf(key)]));
        }
        return items;
    };

    dataStructures.Dictionary.prototype.toObject = function () {
        var obj = {}, key, i;
        for (i = 0; i < this.__inner.keys.length; i++) {
            key = this.__inner.keys[i].toString();
            if (key.toLocaleLowerCase() !== 'constructor' && key.toLocaleLowerCase() !== 'prototype') {
                obj[key] = this.__inner.values[i];
            } else {
                throw new Error(stringFormat(errorMessages.RESERVED_PROPERTY_NAME, key));
            }
        }
        return obj;
    };

    dataStructures.Dictionary.prototype.toString = function () {
        var str = '', key, value, i;
        for (i = 0; i < this.__inner.keys.length; i++) {
            if (str.length > 0) {
                str += ',';
            }
            key = this.__inner.keys[i];
            value = this.__inner.values[i];
            str += key.toString() + ':' + (value !== null ? value.toString() : '');
        }
        return str;
    };

    dataStructures.Dictionary.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        delete this.__inner.keys;
        delete this.__inner.values;
        this.__inner.keys = [];
        this.__inner.values = [];
        this.length = 0;
    };

    dataStructures.Dictionary.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Dictionary(arr);
    };


    // Hash table collection implementation
    dataStructures.Hashtable = function Hashtable() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments),
            ctor = this,
            item,
            i;
        if (items.length > 0 && isFunction(items[0])) {
            this.__inner.hashCodeGenerator = items[0];
            items.splice(0, 1);
            items = initializeItems(items);
        } else {
            this.__inner.hashCodeGenerator = function (obj) {
                if (obj === undefined) {
                    return "$undefined";
                }
                if (obj === null) {
                    return "$null";
                }
                if (typeof obj === "string") {
                    return obj;
                }
                if (isNumber(obj)) {
                    return "$" + obj.toString();
                }
                return JSON.stringify(obj);
            };
        }
        this.__inner.hash = {};
        for (i = 0; i < items.length; i++) {
            item = items[i];
            if (!item || !item.hasOwnProperty('key') || !item.hasOwnProperty('value')) {
                throw new Error(errorMessages.INVALID_KEY_VALUE_COLLECTION);
            }
            this.add(item.key, item.value);
        }
    };
    dataStructures.Hashtable.prototype = new dataStructures.Collection();
    dataStructures.Hashtable.prototype.constructor = dataStructures.Hashtable;

    dataStructures.Hashtable.prototype.size = function () {
        return Object.keys(this.__inner.hash).length;
    };

    dataStructures.Hashtable.prototype.containsKey = function (key) {
        var hashCode = this.__inner.hashCodeGenerator(key),
            items = this.__inner.hash[hashCode],
            item,
            i;
        if (Array.isArray(items)) {
            for (i = 0; i < items.length; i++) {
                item = items[i];
                if (item.key === key) {
                    return true;
                }
            }
        }
        return false;
    };

    dataStructures.Hashtable.prototype.get = function (key) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        var hashCode = this.__inner.hashCodeGenerator(key),
            items = this.__inner.hash[hashCode],
            item,
            i;
        if (Array.isArray(items)) {
            for (i = 0; i < items.length; i++) {
                item = items[i];
                if (item.key === key) {
                    return item.value;
                }
            }
        }
        return undefined;
    };

    dataStructures.Hashtable.prototype.getHashCode = function (key) {
        return this.__inner.hashCodeGenerator(key);
    };

    dataStructures.Hashtable.prototype.add = function (key, value) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var hashCode = this.__inner.hashCodeGenerator(key),
            items = this.__inner.hash[hashCode],
            i;
        if (Array.isArray(items)) {
            for (i = 0; i < items.length; i++) {
                if (items[i].key === key) {
                    throw new Error(errorMessages.DUPLICATE_KEY);
                }
            }
        } else {
            items = [];
        }
        items.push(new dataStructures.KeyValue(key, value));
        this.__inner.hash[hashCode] = items;
        this.length++;
    };

    dataStructures.Hashtable.prototype.put = function (key, value) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var hashCode = this.__inner.hashCodeGenerator(key),
            items = this.__inner.hash[hashCode],
            i;
        if (Array.isArray(items)) {
            for (i = 0; i < items.length; i++) {
                if (items[i].key === key) {
                    return;
                }
            }
        } else {
            items = [];
        }
        items.push(new dataStructures.KeyValue(key, value));
        this.__inner.hash[hashCode] = items;
        this.length++;
    };

    dataStructures.Hashtable.prototype.remove = function (key) {
        if (key === undefined || key === null) {
            throw new Error(errorMessages.UNDEFINED_KEY);
        }
        var hashCode = this.__inner.hashCodeGenerator(key),
            index = -1,
            items = this.__inner.hash[hashCode],
            i;
        if (!Array.isArray(items)) {
            return false;
        }
        for (i = 0; i < items.length; i++) {
            if (items[i].key === key) {
                index = i;
                break;
            }
        }
        if (index >= 0) {
            items.splice(index, 1);
            if (items.length === 0) {
                delete this.__inner.hash[hashCode];
            }
        }
        this.length--;
        return true;
    };

    dataStructures.Hashtable.prototype.clear = function () {
        this.__inner.hash = {};
        this.length = 0;
    };

    dataStructures.Hashtable.prototype.keys = function () {
        var keys = [],
            arr = this.toArray(),
            i;
        for (i = 0; i < arr.length; i++) {
            keys.push(arr[i].key);
        }
        return keys;
    };

    dataStructures.Hashtable.prototype.values = function () {
        var values = [],
            arr = this.toArray(),
            value,
            i;
        for (i = 0; i < arr.length; i++) {
            value = arr[i].value;
            if (values.indexOf(value) < 0) {
                values.push(value);
            }
        }
        return values;
    };

    dataStructures.Hashtable.prototype.toArray = function () {
        var items = [],
            propertyItems,
            propertyItem,
            hashCode,
            i;
        for (hashCode in this.__inner.hash) {
            if (this.__inner.hash.hasOwnProperty(hashCode)) {
                propertyItems = this.__inner.hash[hashCode];
                for (i = 0; i < propertyItems.length; i++) {
                    propertyItem = propertyItems[i];
                    items.push(new dataStructures.KeyValue(propertyItem.key, propertyItem.value));
                }
            }
        }
        return items;
    };

    dataStructures.Hashtable.prototype.toSortedArray = function (comparer) {
        var items = [],
            keys = this.keys(),
            key,
            i;
        keys.sort(comparer);
        for (i = 0; i < keys.length; i++) {
            key = keys[i];
            items.push(new dataStructures.KeyValue(key, this.get(key)));
        }
        return items;
    };

    dataStructures.Hashtable.prototype.toObject = function () {
        var obj = {},
            arr = this.toArray(),
            item,
            i;
        for (i = 0; i < arr.length; i++) {
            item = arr[i];
            if (item.key.toLocaleLowerCase() !== 'constructor' && item.key.toLocaleLowerCase() !== 'prototype') {
                obj[item.key] = item.value;
            } else {
                throw new Error(stringFormat(errorMessages.RESERVED_PROPERTY_NAME, item.key));
            }
        }
        return obj;
    };

    dataStructures.Hashtable.prototype.toString = function () {
        var str = '',
            arr = this.toArray(),
            item,
            i;
        for (i = 0; i < arr.length; i++) {
            item = arr[i];
            if (str.length > 0) {
                str += ',';
            }
            str += item.key.toString() + ':' + (item.value !== null ? item.value.toString() : '');
        }
        return str;
    };

    dataStructures.Hashtable.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Hashtable(arr);
    };


    // Stack collection implementation
    dataStructures.Stack = function Stack() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments);
        items.reverse();
        this.__inner.items = items;
        this.length = this.__inner.items.length;
    };
    dataStructures.Stack.prototype = new dataStructures.Collection();
    dataStructures.Stack.prototype.constructor = dataStructures.Stack;

    dataStructures.Stack.prototype.push = function (item) {
        this.__inner.items.push(item);
        this.length = this.__inner.items.length;
    };

    dataStructures.Stack.prototype.pop = function () {
        if (this.isEmpty()) {
            return undefined;
        }
        var item = this.__inner.items.pop();
        this.length = this.__inner.items.length;
        return item;
    };

    dataStructures.Stack.prototype.peek = function () {
        var size = this.size();
        if (size === 0) {
            return undefined;
        }
        return this.__inner.items[size - 1];
    };

    dataStructures.Stack.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        clearItems(this);
    };

    // @Overridden
    dataStructures.Stack.prototype.toArray = function () {
        var items = this.__inner.items.slice(0);
        items.reverse();
        return items;
    };

    dataStructures.Stack.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Stack(arr);
    };


    // Queue collection implementation
    dataStructures.Queue = function Queue() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        this.__inner.items = initializeItems(arguments);
        this.length = this.__inner.items.length;
    };
    dataStructures.Queue.prototype = new dataStructures.Collection();
    dataStructures.Queue.prototype.constructor = dataStructures.Queue;

    dataStructures.Queue.prototype.enqueue = function (item) {
        this.__inner.items.push(item);
        this.length = this.__inner.items.length;
    };

    dataStructures.Queue.prototype.dequeue = function () {
        if (this.isEmpty()) {
            return undefined;
        }
        var item = this.__inner.items.shift();
        this.length = this.__inner.items.length;
        return item;
    };

    dataStructures.Queue.prototype.peek = function () {
        if (this.isEmpty()) {
            return undefined;
        }
        return this.__inner.items[0];
    };

    dataStructures.Queue.prototype.clear = function () {
        if (this.isEmpty()) {
            return;
        }
        clearItems(this);
    };

    dataStructures.Queue.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Queue(arr);
    };


    // Tree (binary search tree) collection implementation
    dataStructures.Tree = function Tree() {
        dataStructures.Collection.prototype.constructor.apply(this, arguments);
        var items = initializeItems(arguments), i;
        if (arguments.length > 0 && isFunction(items[0])) {
            this.__inner.comparer = items[0];
            items.splice(0, 1);
            items = initializeItems(items);
        } else {
            this.__inner.comparer = dataStructures.Comparer.standard;
        }
        this.root = null;
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };
    dataStructures.Tree.prototype = new dataStructures.Collection();
    dataStructures.Tree.prototype.constructor = dataStructures.Tree;

    dataStructures.Tree.prototype.getComparer = function () {
        return this.__inner.comparer;
    };

    dataStructures.Tree.Node = function (value) {
        this.value = value;
        this.left = null;
        this.right = null;
        this.parent = null;
    };

    dataStructures.Tree.prototype.isEmpty = function () {
        return !this.root;
    };

    dataStructures.Tree.prototype.size = function () {
        var size = 0;
        this.forEach(function () {
            size++;
        });
        return size;
    };

    dataStructures.Tree.prototype.min = function () {
        var node = this.root;
        if (!node) {
            return undefined;
        }
        while (node.left) {
            node = node.left;
        }
        return node.value;
    };

    dataStructures.Tree.prototype.max = function () {
        var node = this.root;
        if (!node) {
            return undefined;
        }
        while (node.right) {
            node = node.right;
        }
        return node.value;
    };

    dataStructures.Tree.prototype.forEach = function (callback) {
        if (this.isEmpty() || !isFunction(callback)) {
            return;
        }
        var traverse = function (node, callback) {
            if (node === null) {
                return;
            }
            traverse(node.left, callback);
            callback.call(node.value);
            traverse(node.right, callback);
        };
        traverse(this.root, callback);
    };

    dataStructures.Tree.prototype.contains = function (value) {
        var self = this,
            found = false;
        this.forEach(function () {
            if (!found && self.__inner.comparer(this, value) === 0) {
                found = true;
            }
        });
        return found;
    };

    dataStructures.Tree.prototype.add = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var self = this,
            addNode = function (node) {
                var parent = null,
                    position = self.root,
                    compareValue = null;
                while (position !== null) {
                    compareValue = self.__inner.comparer(node.value, position.value);
                    if (compareValue === 0) {
                        return null;
                    } else if (compareValue < 0) {
                        parent = position;
                        position = position.left;
                    } else {
                        parent = position;
                        position = position.right;
                    }
                }
                node.parent = parent;
                if (parent === null) {
                    self.root = node;
                } else if (self.__inner.comparer(node.value, parent.value) < 0) {
                    parent.left = node;
                } else {
                    parent.right = node;
                }
                return node;
            };
        if (addNode(new dataStructures.Tree.Node(value)) === null) {
            return false;
        }
        this.length++;
        return true;
    };

    dataStructures.Tree.prototype.addRange = function () {
        var items = initializeItems(arguments), i;
        for (i = 0; i < items.length; i++) {
            this.add(items[i]);
        }
    };

    dataStructures.Tree.prototype.remove = function (value) {
        if (value === undefined) {
            throw new Error(errorMessages.UNDEFINED_VALUE);
        }
        var self = this,
            searchNode = function (node, value) {
                var compareValue;
                while (node !== null && compareValue !== 0) {
                    compareValue = self.__inner.comparer(value, node.value);
                    if (compareValue < 0) {
                        node = node.left;
                    } else if (compareValue > 0) {
                        node = node.right;
                    }
                }
                return node;
            },
            node = searchNode(this.root, value),
            removeNode = function (node) {
                var swap = function (node1, node2) {
                        if (node1.parent === null) {
                            self.root = node2;
                        } else if (node1 === node1.parent.left) {
                            node1.parent.left = node2;
                        } else {
                            node1.parent.right = node2;
                        }
                        if (node2 !== null) {
                            node2.parent = node1.parent;
                        }
                    },
                    minNode;
                if (node.left === null) {
                    swap(node, node.right);
                } else if (node.right === null) {
                    swap(node, node.left);
                } else {
                    minNode = node.right;
                    while (minNode.left !== null) {
                        minNode = minNode.left;
                    }
                    if (minNode.parent !== node) {
                        swap(minNode, minNode.right);
                        minNode.right = node.right;
                        minNode.right.parent = minNode;
                    }
                    swap(node, minNode);
                    minNode.left = node.left;
                    minNode.left.parent = minNode;
                }
                return node;
            };
        if (node === null) {
            return false;
        }
        removeNode(node);
        this.length--;
        return true;
    };

    dataStructures.Tree.prototype.clear = function () {
        this.root = null;
        this.length = 0;
    };

    dataStructures.Tree.prototype.toArray = function () {
        var values = [];
        this.forEach(function () {
            values.push(this);
        });
        return values;
    };

    dataStructures.Tree.prototype.toSortedArray = function (comparer) {
        var items = this.toArray();
        if (isFunction(comparer)) {
            items.sort(comparer);
        }
        return items;
    };

    dataStructures.Tree.parse = function (arr) {
        if (!Array.isArray(arr)) {
            throw new Error(errorMessages.INVALID_VALUE_ARRAY);
        }
        return new dataStructures.Tree(arr);
    };


    // Collection initialization
    var init = function () {
        if (__collectionsInitialized) {
            return false;
        }

        window.Comparer = dataStructures.Comparer;
        window.EqualityComparer = dataStructures.EqualityComparer;
        window.Bag = dataStructures.Bag;
        window.Collection = dataStructures.Collection;
        window.Dictionary = dataStructures.Dictionary;
        window.Hashtable = dataStructures.Hashtable;
        window.LinkedList = dataStructures.LinkedList;
        window.List = dataStructures.List;
        window.Queue = dataStructures.Queue;
        window.Set = dataStructures.Set;
        window.Stack = dataStructures.Stack;
        window.Tree = dataStructures.Tree;

        return true;
    };

    return init();
})();
