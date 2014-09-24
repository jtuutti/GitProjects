/* global console, test, ok, Comparer, EqualityComparer, Collection, Hashtable */
(function (undefined) {
    'use strict';

    test('Empty Hashtable initialization', function (assert) {
        var col = new Hashtable();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
    });

    test('Hashtable initialization with single value', function (assert) {
        var col = new Hashtable({ key: "id", value: 25 });
        assert.ok(col);
        assert.strictEqual(col.length, 1);
        assert.strictEqual(col.getHashCode("id"), "id");
    });

    test('Hashtable initialization with multiple values', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col);
        assert.strictEqual(col.length, 3);
        assert.strictEqual(col.getHashCode("id1"), "id1");
        assert.strictEqual(col.getHashCode("id2"), "id2");
    });

    test('Hashtable initialization with multiple values (custom hash generator) - v1.0', function (assert) {
        var hashCodeGenerator = function (key) {
                return parseInt(key.substr(2), 10) * -1;
            },
            col = new Hashtable(hashCodeGenerator,
                                { key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col);
        assert.strictEqual(col.length, 3);
        assert.strictEqual(col.getHashCode("id1"), -1);
        assert.strictEqual(col.getHashCode("id2"), -2);
    });

    test('Hashtable initialization with multiple values (custom hash generator) - v1.1', function (assert) {
        var hashCodeGenerator = function (key) {
                return parseInt(key.substr(2), 10) * -1;
            },
            col = new Hashtable(hashCodeGenerator);
        col.add("id1", 25);
        col.add("id3", 36);
        col.add("id2", 47);
        assert.ok(col);
        assert.strictEqual(col.length, 3);
        assert.strictEqual(col.getHashCode("id1"), -1);
        assert.strictEqual(col.getHashCode("id2"), -2);
    });

    test('Hashtable - add method - v1', function (assert) {
        var col = new Hashtable();
        assert.strictEqual(col.length, 0);
        col.add("id1", 25);
        assert.strictEqual(col.length, 1);
        col.add("id3", 36);
        assert.strictEqual(col.length, 2);
        col.add("id2", 47);
        assert.strictEqual(col.length, 3);
    });

    test('Hashtable - add method - v1.1', function (assert) {
        var col = new Hashtable();
        assert.strictEqual(col.length, 0);
        col.add("id1", 25);
        assert.strictEqual(col.length, 1);
        col.add("id3", 36);
        assert.strictEqual(col.length, 2);
        col.add("id2", 47);
        assert.strictEqual(col.length, 3);
        assert.throws(function () {
            col.add("id2", 47);
        });
        assert.strictEqual(col.length, 3);
    });

    test("Hashtable - all method", function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col.all(function () {
            return this.key.indexOf("id") === 0 && !isNaN(this.key.substr(2));
        }));
        assert.ok(!col.all(function () {
            return parseInt(this.key.substr(2), 10) % 2 === 1;
        }));
    });

    test("Hashtable - any method", function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col.any(function () {
            return parseInt(this.key.substr(2), 10) % 2 === 1;
        }));
        assert.ok(!col.any(function () {
            return this.value > 50;
        }));
    });

    test('Hashtable - clear method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Hashtable - containsKey method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col.containsKey("id1"));
        assert.ok(col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));
        assert.ok(!col.containsKey("id4"));
    });

    test('Hashtable - find method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            result = col.find(function () {
                return this.key.indexOf("id") === 0 && this.value > 25;
            });
        assert.ok(result);
        assert.ok(result.key);
        assert.ok(col.containsKey(result.key));
    });

    test('Hashtable - findAll method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            results = col.findAll(function () {
                return this.key.indexOf("id") === 0 && this.value > 25;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 2);
        results.forEach(function (result) {
            assert.ok(col.containsKey(result.key));
        });
    });

    test('Hashtable - foreach method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        col.forEach(function () {
            assert.ok(col.containsKey(this.key));
        });
    });

    test('Hashtable - get method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.strictEqual(col.get("id1"), 25);
        assert.strictEqual(col.get("id2"), 47);
        assert.strictEqual(col.get("id3"), 36);
        assert.strictEqual(col.get("Id1"), undefined);
    });

    test('Hashtable - getHashCode method - v1', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            hashCode;
        assert.strictEqual(col.length, 3);
        col.forEach(function () {
            hashCode = col.getHashCode(this.key);
            assert.strictEqual(typeof hashCode, "string");
            assert.ok(hashCode.length > 0);
        });
    });

    test('Hashtable - getHashCode method - v2', function (assert) {
        var hashCodeGenerator = function (str) {
                var charSum = 0, i;
                for (i = 0; i < str.length; i++) {
                    charSum += str.charCodeAt(i);
                }
                return charSum;
            },
            col = new Hashtable(hashCodeGenerator,
                                { key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            hashCode;
        assert.strictEqual(col.length, 3);
        col.forEach(function () {
            hashCode = col.getHashCode(this.key);
            assert.ok(!isNaN(hashCode));
            assert.ok(hashCode > 0);
        });
    });

    test('Hashtable - isEmpty method - v1', function (assert) {
        var col = new Hashtable();
        assert.ok(col.isEmpty());
    });

    test('Hashtable - isEmpty method - v1.1', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(!col.isEmpty());
    });

    test('Hashtable - groupBy method - v1', function (assert) {
        var col = new Hashtable({ key: "quick", value: "5" },
                                { key: "brown", value: "5" },
                                { key: "fox", value: "3" },
                                { key: "dog", value: "3" }),
            groupedCol = col.groupBy(function() {
                return this.key.length;
            });
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Hashtable - groupBy method - v1.1', function (assert) {
        var col = new Hashtable({ key: "quick", value: "5" },
                                { key: "brown", value: "5" },
                                { key: "fox", value: "3" },
                                { key: "dog", value: "3" }),
            groupedCol = col.groupBy(function() {
                return this.key.length;
            }, "key");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).key !== undefined);
        assert.ok(groupedCol.get(5).key !== undefined);
        assert.strictEqual(groupedCol.get(3).key.length, 2);
        assert.strictEqual(groupedCol.get(5).key.length, 2);
    });

    test('Hashtable - groupBy method - v1.1', function (assert) {
        var col = new Hashtable({ key: "quick", value: "5" },
                                { key: "brown", value: "5" },
                                { key: "fox", value: "3" },
                                { key: "dog", value: "3" }),
            groupedCol = col.groupBy(function() {
                return this.key.length;
            }, ["key", "value"]);
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).key !== undefined);
        assert.ok(groupedCol.get(5).key !== undefined);
        assert.strictEqual(groupedCol.get(3).key.length, 2);
        assert.strictEqual(groupedCol.get(5).key.length, 2);
        assert.ok(groupedCol.get(3).value !== undefined);
        assert.ok(groupedCol.get(5).value !== undefined);
        assert.strictEqual(groupedCol.get(3).value.length, 1);
        assert.strictEqual(groupedCol.get(5).value.length, 1);
    });

    test('Hashtable - keys method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            keys = col.keys();
        assert.strictEqual(keys.length, col.length);
        keys.forEach(function (key) {
            assert.ok(col.containsKey(key));
        });
    });

    test('Hashtable - map method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            results = col.map(function () {
                return this.value;
            });
        results.forEach(function (item) {
            assert.ok(!isNaN(item));
            assert.ok(item > 0 && item < 50);
        });
    });

    test('Hashtable - put method - v1', function (assert) {
        var col = new Hashtable();
        assert.strictEqual(col.length, 0);
        col.put("id1", 25);
        assert.strictEqual(col.length, 1);
        col.put("id3", 36);
        assert.strictEqual(col.length, 2);
        col.put("id2", 47);
        assert.strictEqual(col.length, 3);
    });

    test('Hashtable - put method - v1.1', function (assert) {
        var col = new Hashtable();
        assert.strictEqual(col.length, 0);
        col.put("id1", 25);
        assert.strictEqual(col.length, 1);
        col.put("id3", 36);
        assert.strictEqual(col.length, 2);
        col.put("id2", 47);
        assert.strictEqual(col.length, 3);
        col.put("id2", 49);
        assert.strictEqual(col.length, 3);
    });

    test('Hashtable - reduce method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value;
            }, 10);
        assert.strictEqual(result, 118);
    });

    test('Hashtable - remove method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.strictEqual(col.length, 3);
        assert.ok(col.containsKey("id1"));
        assert.ok(col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));

        assert.ok(col.remove("id1"));
        assert.strictEqual(col.length, 2);
        assert.ok(!col.containsKey("id1"));
        assert.ok(col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));

        assert.ok(col.remove("id2"));
        assert.strictEqual(col.length, 1);
        assert.ok(!col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));

        assert.ok(col.remove("id3"));
        assert.strictEqual(col.length, 0);
    });

    test('Hashtable - size method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.strictEqual(col.size(), col.length);
    });

    test('Hashtable - size method - v2', function (assert) {
        var col = new Hashtable();
        assert.strictEqual(col.size(), col.length);
    });

    test('Hashtable - toArray method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.containsKey(arr[i].key));
        }
    });

    test('Hashtable - toSortedArray method - v1', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            sortedKeyArr = ["id1", "id2", "id3"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i].key, sortedKeyArr[i]);
        }
    });

    test('Hashtable - toSortedArray method - v2', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            sortedKeyArr = ["id3", "id2", "id1"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i].key, sortedKeyArr[i]);
        }
    });

    test('Hashtable - toObject method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            obj = col.toObject(),
            key;
        assert.strictEqual(typeof obj, "object");
        assert.strictEqual(Object.keys(obj).length, col.length);
        for (key in obj) {
            if (obj.hasOwnProperty(key)) {
                assert.ok(col.containsKey(key));
            } else {
                assert.ok(false); // fail on invalid property
            }
        }
    });

    test('Hashtable - toJSON method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Hashtable.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(newCol.containsKey(data[i].key));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Set - toString method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 }),
            str = col.toString(),
            arr,
            i;
        assert.strictEqual(typeof str, "string");
        assert.ok(str.length > 0);
        col.forEach(function () {
            assert.ok(str.indexOf(this.key) >= 0);
            assert.ok(str.indexOf(this.value.toString()) >= 0);
        });
    });

    test('Hashtable - values method', function (assert) {
        var col = new Hashtable({ key: "id1", value: 25 },
                                { key: "id3", value: 25 },
                                { key: "id2", value: 47 }),
            values = col.values();
        assert.ok(values.length > 0);
        assert.ok(values.length <= col.length);
        values.forEach(function (value) {
            assert.ok(col.find(function () {
                return this.value === value;
            }));
        });
    });
}());
