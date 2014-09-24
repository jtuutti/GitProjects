/* global console, test, ok, Comparer, EqualityComparer, Collection, Dictionary */
(function (undefined) {
    'use strict';

    test('Empty Dictionary initialization', function (assert) {
        var col = new Dictionary();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Dictionary initialization with single value', function (assert) {
        var col = new Dictionary({ key: "id", value: 25 });
        assert.ok(col);
        assert.strictEqual(col.length, 1);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Dictionary initialization with multiple values', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 });
        assert.ok(col);
        assert.strictEqual(col.length, 3);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Dictionary initialization with multiple values (case insensitive comparer)', function (assert) {
        var col = new Dictionary(EqualityComparer.caseInsensitive,
                                 { key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 });
        assert.ok(col);
        assert.strictEqual(col.length, 3);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Dictionary - add method - v1', function (assert) {
        var col = new Dictionary();
        assert.strictEqual(col.length, 0);
        col.add("id1", 25);
        assert.strictEqual(col.length, 1);
        col.add("id3", 36);
        assert.strictEqual(col.length, 2);
        col.add("id2", 47);
        assert.strictEqual(col.length, 3);
    });

    test('Dictionary - add method - v1.1', function (assert) {
        var col = new Dictionary();
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

    test("Dictionary - all method", function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        assert.ok(col.all(function () {
            return this.key.indexOf("id") === 0 && !isNaN(this.key.substr(2));
        }));
        assert.ok(!col.all(function () {
            return parseInt(this.key.substr(2), 10) % 2 === 1;
        }));
    });

    test("Dictionary - any method", function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        assert.ok(col.any(function () {
            return parseInt(this.key.substr(2), 10) % 2 === 1;
        }));
        assert.ok(!col.any(function () {
            return this.value > 50;
        }));
    });

    test('Dictionary - clear method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Dictionary - containsKey method - v1', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 });
        assert.ok(col.containsKey("id1"));
        assert.ok(col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));
        assert.ok(!col.containsKey("id4"));
    });

    test('Dictionary - containsKey method - v1.1', function (assert) {
        var col = new Dictionary(EqualityComparer.caseInsensitive,
                                { key: "id1", value: 25 },
                                { key: "id3", value: 36 },
                                { key: "id2", value: 47 });
        assert.ok(col.containsKey("ID1"));
        assert.ok(col.containsKey("Id2"));
        assert.ok(col.containsKey("iD3"));
        assert.ok(col.containsKey("id1"));
        assert.ok(col.containsKey("id2"));
        assert.ok(col.containsKey("id3"));
        assert.ok(!col.containsKey("id4"));
    });

    test('Dictionary - find method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 }),
            result = col.find(function () {
                return this.key.indexOf("id") === 0 && this.value > 25;
            });
        assert.ok(result);
        assert.ok(result.key);
        assert.ok(col.containsKey(result.key));
    });

    test('Dictionary - findAll method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - foreach method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        col.forEach(function () {
            assert.ok(col.containsKey(this.key));
        });
    });

    test('Dictionary - get method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 });
        assert.strictEqual(col.get("id1"), 25);
        assert.strictEqual(col.get("id2"), 47);
        assert.strictEqual(col.get("id3"), 36);
        assert.strictEqual(col.get("Id1"), undefined);
    });

    test('Dictionary - get method (case insensitive comparer)', function (assert) {
        var col = new Dictionary(EqualityComparer.caseInsensitive,
                                 { key: "id1", value: 25 },
                                 { key: "id3", value: 36 },
                                 { key: "id2", value: 47 });
        assert.strictEqual(col.get("id1"), 25);
        assert.strictEqual(col.get("id2"), 47);
        assert.strictEqual(col.get("id3"), 36);
        assert.strictEqual(col.get("Id1"), 25);
        assert.strictEqual(col.get("ID2"), 47);
        assert.strictEqual(col.get("iD3"), 36);
        assert.strictEqual(col.get("iD4"), undefined);
    });

    test('Dictionary - groupBy method - v1', function (assert) {
        var col = new Dictionary({ key: "quick", value: "5" },
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

    test('Dictionary - groupBy method - v1.1', function (assert) {
        var col = new Dictionary({ key: "quick", value: "5" },
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

    test('Dictionary - groupBy method - v1.1', function (assert) {
            var col = new Dictionary({ key: "quick", value: "5" },
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

    test('Dictionary - isEmpty method - v1', function (assert) {
        var col = new Dictionary();
        assert.ok(col.isEmpty());
    });

    test('Dictionary - isEmpty method - v1.1', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        assert.ok(!col.isEmpty());
    });

    test('Dictionary - keys method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                { key: "id3", value: 36 },
                { key: "id2", value: 47 }),
            keys = col.keys();
        assert.strictEqual(keys.length, col.length);
        keys.forEach(function (key) {
            assert.ok(col.containsKey(key));
        });
    });

    test('Dictionary - map method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - put method - v1', function (assert) {
        var col = new Dictionary();
        assert.strictEqual(col.length, 0);
        col.put("id1", 25);
        assert.strictEqual(col.length, 1);
        col.put("id3", 36);
        assert.strictEqual(col.length, 2);
        col.put("id2", 47);
        assert.strictEqual(col.length, 3);
    });

    test('Dictionary - put method - v1.1', function (assert) {
        var col = new Dictionary();
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

    test('Dictionary - reduce method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                { key: "id3", value: 36 },
                { key: "id2", value: 47 }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value;
            }, 10);
        assert.strictEqual(result, 118);
    });

    test('Dictionary - remove method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - size method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
            { key: "id3", value: 36 },
            { key: "id2", value: 47 });
        assert.strictEqual(col.size(), col.length);
    });

    test('Dictionary - size method - v2', function (assert) {
        var col = new Dictionary();
        assert.strictEqual(col.size(), col.length);
    });

    test('Dictionary - toArray method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - toSortedArray method - v1', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - toSortedArray method - v2', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - toObject method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - toJSON method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
                { key: "id3", value: 36 },
                { key: "id2", value: 47 }),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Dictionary.parse(data);
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
        var col = new Dictionary({ key: "id1", value: 25 },
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

    test('Dictionary - values method', function (assert) {
        var col = new Dictionary({ key: "id1", value: 25 },
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
