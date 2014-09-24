/* global console, test, ok, Comparer, EqualityComparer, Collection, Bag, List */
(function (undefined) {
    'use strict';

    test('Empty Bag initialization', function (assert) {
        var col = new Bag();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Bag initialization with single value', function (assert) {
        var col = new Bag("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Bag initialization with multiple values', function (assert) {
        var col = new Bag("quick", "brown", "fox", "dog", "brown", "fox");
        assert.ok(col);
        assert.strictEqual(col.size(), 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Bag initialization with multiple values (case insensitive comparer)', function (assert) {
        var col = new Bag(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog", "BROWN", "FoX");
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Bag initialization from array', function (assert) {
        var col = new Bag(["quick", "brown", "fox", "dog", "brown", "fox"]);
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Bag initialization from array (case insensitive comparer)', function (assert) {
        var col = new Bag(EqualityComparer.caseInsensitive,
            ["quick", "brown", "fox", "dog", "BROWN", "FoX"]);
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Bag initialization from collection', function (assert) {
        var col = new Bag(new Bag("quick", "brown", "fox", "dog", "brown", "fox"));
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Bag initialization from collection (case insensitive comparer)', function (assert) {
        var col = new Bag(EqualityComparer.caseInsensitive,
            new Bag("quick", "brown", "fox", "dog", "BROWN", "FoX"));
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Bag - add method - v1', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.add("dog");
        col.add("brown");
        col.add("fox");
        assert.ok(col);
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.distinctSize(), 4);
    });

    test('Bag - add method - v2', function (assert) {
        var col = new Bag();
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.distinctSize(), 3);
    });

    test('Bag - all method - v1', function (assert) {
        var col = new Bag(), result;
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        result = col.all(function () {
            return this.length >= 3;
        });
        assert.strictEqual(result, true);
    });

    test('Bag - all method - v1.1', function (assert) {
        var col = new Bag(), result;
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        result = col.all(function () {
            return this === "fox";
        });
        assert.strictEqual(result, false);
    });

    test('Bag - any method - v1', function (assert) {
        var col = new Bag(), result;
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        result = col.any(function () {
            return this === "fox";
        });
        assert.strictEqual(result, true);
    });

    test('Bag - any method - v1.1', function (assert) {
        var col = new Bag(), result;
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        result = col.any(function () {
            return this.length < 3;
        });
        assert.strictEqual(result, false);
    });

    test('Bag - clear method', function (assert) {
        var col = new Bag();
        col.add("quick", 1);
        col.add("brown", 10);
        col.add("fox", 5);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.size(), 16);
        assert.ok(!col.isEmpty());
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Bag - count method', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.count("quick"), 1);
        assert.strictEqual(col.count("brown"), 10);
        assert.strictEqual(col.count("fox"), 5);
        assert.strictEqual(col.size(), 16);
    });

    test('Bag - find method', function (assert) {
        var col = new Bag(), result;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        result = col.find(function () {
            return typeof this === "string" && this.indexOf("o") >= 0;
        });
        assert.ok(result);
        assert.ok(col.contains(result));
    });

    test('Bag - findAll method', function (assert) {
        var col = new Bag(), results;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        results = col.findAll(function () {
            return typeof this === "string" && this.indexOf("o") >= 0;
        });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 15);
        results.forEach(function (result) {
            assert.ok(col.contains(result));
        });
    });

    test('Bag - foreach method', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        col.forEach(function (i) {
            assert.ok(col.contains(this));
        });
    });

    test('Bag - groupBy method - v1', function (assert) {
        var col = new Bag(), groupedCol;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        groupedCol = col.groupBy(function() {
            return this.length;
        });
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 1);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Bag - groupBy method - v2', function (assert) {
        var col = new Bag(), groupedCol;
        col.add({ name: "quick", size: 5 });
        col.add({ name: "brown", size: 5 }, 10);
        col.add({ name: "fox", size: 3 }, 5);
        groupedCol = col.groupBy(function() {
            return this.size;
        });
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 1);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Bag - groupBy method - v2.1', function (assert) {
        var col = new Bag(), groupedCol;
        col.add({ name: "quick", size: 5 });
        col.add({ name: "brown", size: 5 }, 10);
        col.add({ name: "fox", size: 3 }, 5);
        groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 1);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Bag - groupBy method - v2.2', function (assert) {
        var col = new Bag(), groupedCol;
        col.add({ name: "quick", size: 5 });
        col.add({ name: "brown", size: 5 }, 10);
        col.add({ name: "fox", size: 3 }, 5);
        groupedCol = col.groupBy("size", ["name", "size"]);
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).name !== undefined);
        assert.ok(groupedCol.get(5).name !== undefined);
        assert.strictEqual(groupedCol.get(3).name.length, 1);
        assert.strictEqual(groupedCol.get(5).name.length, 2);
        assert.ok(groupedCol.get(3).size !== undefined);
        assert.ok(groupedCol.get(5).size !== undefined);
        assert.strictEqual(groupedCol.get(3).size.length, 1);
        assert.strictEqual(groupedCol.get(5).size.length, 1);
    });

    test('Bag - groupBy method - v3', function (assert) {
        var col = new Bag(),
            groupedCol,
            name,
            count;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        groupedCol = col.groupBy(function() {
            return col.count(this);
        });
        assert.strictEqual(groupedCol.length, 3);
        groupedCol.keys().forEach(function (key) {
            name = groupedCol.get(key)[0];
            assert.ok(name !== undefined);
            count = col.count(name);
            assert.strictEqual(count, key);
        });
    });

    test('Bag - map method - v1', function (assert) {
        var col = new Bag(), results;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        results = col.map(function () {
            return this.substr(0, 2);
        });
        results.forEach(function (item) {
            assert.ok(col.find(function () {
                return this.indexOf(item) === 0;
            }) !== undefined);
        });
    });

    test('Bag - map method - v2', function (assert) {
        var col = new Bag(), results;
        col.add({ name: "quick" });
        col.add({ name: "brown" }, 10);
        col.add({ name: "fox" }, 5);
        results = col.map(function () {
            return this.value;
        });
        results.forEach(function (item) {
            assert.ok(col.find(function () {
                return this.value === item;
            }) !== undefined);
        });
    });

    test('Bag - reduce method - v1', function (assert) {
        var col = new Bag({ value: "quick" },
                          { value: "brown" },
                          { value: "fox" },
                          { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('Bag - reduce method - v1.1', function (assert) {
        var col = new Bag({ value: "quick" },
                          { value: "brown" },
                          { value: "fox" },
                          { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('Bag - reduce method - v2', function (assert) {
        var col = new Bag([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('Bag - reduce method - v2.1', function (assert) {
        var col = new Bag([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });

    test('Bag - reduce method - v2.2', function (assert) {
        var col = new Bag([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('Bag - remove method', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.distinctSize(), 3);

        assert.ok(col.remove("quick"));
        assert.strictEqual(col.length, 15);
        assert.strictEqual(col.distinctSize(), 2);

        assert.ok(!col.remove("quick"));
        assert.strictEqual(col.length, 15);
        assert.strictEqual(col.distinctSize(), 2);

        assert.ok(col.remove("brown"));
        assert.strictEqual(col.length, 5);
        assert.strictEqual(col.distinctSize(), 1);

        assert.ok(col.remove("fox"));
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.distinctSize(), 0);
    });

    test('Bag - setCount method - v1', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("quick");
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.setCount("quick", 1);
        col.setCount("brown", 10);
        col.setCount("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.distinctSize(), 3);
    });

    test('Bag - size method - v1', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.strictEqual(col.size(), col.length);
    });

    test('Bag - size method - v1.1', function (assert) {
        var col = new Bag();
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.strictEqual(col.size(), col.count("quick") + col.count("brown") + col.count("fox"));
    });

    test('Bag - size method - v2', function (assert) {
        var col = new Bag("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.strictEqual(col.size(), col.length);
    });

    test('Bag - size method - v2.1', function (assert) {
        var col = new Bag("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.strictEqual(col.size(), col.count("quick") + col.count("brown") + col.count("fox"));
    });

    test('Bag - toArray method', function (assert) {
        var col = new Bag(), arr;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.distinctSize(), 3);
        arr = col.toArray();
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, 16);
    });

    test('Bag - toDistinctArray method', function (assert) {
        var col = new Bag(), arr;
        col.add("quick");
        col.add("brown", 10);
        col.add("fox", 5);
        assert.ok(col);
        assert.strictEqual(col.length, 16);
        assert.strictEqual(col.distinctSize(), 3);
        arr = col.toDistinctArray();
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, 3);
    });

    test('Bag - toSortedArray method - v1', function (assert) {
        var col = new Bag(), arr;
        col.add("quick");
        col.add("brown", 2);
        col.add("fox");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.distinctSize(), 3);
        arr = col.toSortedArray();
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, 4);
        assert.strictEqual(arr[0], "brown");
        assert.strictEqual(arr[1], "brown");
        assert.strictEqual(arr[2], "fox");
        assert.strictEqual(arr[3], "quick");
    });

    test('Bag - toSortedArray method - v1.1', function (assert) {
        var col = new Bag(), arr;
        col.add("quick");
        col.add("brown", 2);
        col.add("fox");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.distinctSize(), 3);
        arr = col.toSortedArray(Comparer.reverse);
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, 4);
        assert.strictEqual(arr[0], "quick");
        assert.strictEqual(arr[1], "fox");
        assert.strictEqual(arr[2], "brown");
        assert.strictEqual(arr[3], "brown");
    });

    test('Bag - toJSON method', function (assert) {
        var col = new Bag("quick"),
            newCol,
            str,
            isJSON = true,
            i;
        col.add("brown", 10);
        col.add("fox", 5);
        str = col.toJSON();
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Bag.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(newCol.contains(data[i]));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Bag - toString method', function (assert) {
        var col = new Bag("quick"),
            str,
            arr,
            i;
        col.add("brown", 10);
        col.add("fox", 5);
        str = col.toString();
        assert.strictEqual(typeof str, "string");
        assert.ok(str.length > 0);
        arr = str.split(',');
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.contains(arr[i]));
        }
    });
}());
