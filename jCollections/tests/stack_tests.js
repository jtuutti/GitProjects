/* global console, test, ok, Comparer, EqualityComparer, Collection, Stack, List */
(function (undefined) {
    'use strict';

    test('Empty Stack initialization', function (assert) {
        var col = new Stack();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
    });

    test('Stack initialization with single value', function (assert) {
        var col = new Stack("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
    });

    test('Stack initialization with multiple values', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Stack initialization from array', function (assert) {
        var col = new Stack(["quick", "brown", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Stack initialization from collection', function (assert) {
        var col = new Stack(new List("quick", "brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Stack - all method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('Stack - all method - v1.1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });

    test('Stack - any method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('Stack - any method - v1.1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('Stack - clear method', function (assert) {
        var col = new Stack();
        col.push("quick");
        col.push("brown");
        col.push("fox");
        col.push("dog");
        col.push("Brown");
        assert.strictEqual(col.length, 5);
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Stack - find method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(result && result.length >= 3);
    });

    test('Stack - findAll method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (result) {
            assert.ok(result && result.length >= 3);
        });
    });

    test('Stack - foreach method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog");
        col.forEach(function () {
            assert.ok(this && this.length >= 3);
        });
    });

    test('Stack - groupBy method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog", "fox"),
            groupedCol = col.groupBy(function() {
                return this.length;
            });
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Stack - groupBy method - v2', function (assert) {
        var col = new Stack({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy(function() {
                return this.size;
            });
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Stack - groupBy method - v2.1', function (assert) {
        var col = new Stack({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Stack - groupBy method - v2.2', function (assert) {
        var col = new Stack({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('Stack - groupBy method - v2.3', function (assert) {
        var col = new Stack({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", ["word", "size"]);
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
        assert.ok(groupedCol.get(3).size !== undefined);
        assert.ok(groupedCol.get(5).size !== undefined);
        assert.strictEqual(groupedCol.get(3).size.length, 1);
        assert.strictEqual(groupedCol.get(5).size.length, 1);
    });

    test('Stack - isEmpty method - v1', function (assert) {
        var col = new Stack();
        assert.ok(col.isEmpty());
    });

    test('Stack - isEmpty method - v1.1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('Stack - map method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            results = col.map(function () {
                return this.substr(0, 2);
            });
        results.forEach(function (item, i) {
            assert.ok(col.find(function () {
                return this.indexOf(item) === 0;
            }) !== undefined);
        });
    });

    test('Stack - map method - v1.1', function (assert) {
        var col = new Stack({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            results = col.map(function () {
                return this.value;
            });
        results.forEach(function (item) {
            assert.ok(col.find(function () {
                return this.value === item;
            }) !== undefined);
        });
    });

    test('Stack - map method - v2', function (assert) {
        var col = new Stack({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            results = col.map("value");
        results.forEach(function (item, i) {
            assert.ok(col.find(function () {
                return this.value === item;
            }) !== undefined);
        });
    });

    test('Stack - push method - v1', function (assert) {
        var col = new Stack();
        col.push("quick");
        col.push("brown");
        col.push("fox");
        col.push("dog");
        col.push("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["Brown", "dog", "fox", "brown", "quick"]));
    });

    test('Stack - push method - v2', function (assert) {
        var col = new Stack("brown", "quick");
        col.push("fox");
        col.push("dog");
        col.push("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["Brown", "dog", "fox", "brown", "quick"]));
    });

    test('Stack - peek method', function (assert) {
        var col = new Stack();
        col.push("quick");
        col.push("brown");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
        assert.strictEqual(col.peek(), "brown");
        assert.strictEqual(col.length, 2);
    });

    test('Stack - pop method', function (assert) {
        var col = new Stack();
        col.push("quick");
        col.push("brown");
        col.push("fox");
        col.push("dog");
        col.push("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);

        assert.strictEqual(col.pop(), "Brown");
        assert.strictEqual(col.length, 4);

        assert.strictEqual(col.pop(), "dog");
        assert.strictEqual(col.length, 3);

        assert.strictEqual(col.pop(), "fox");
        assert.strictEqual(col.length, 2);

        assert.strictEqual(col.pop(), "brown");
        assert.strictEqual(col.length, 1);

        assert.strictEqual(col.pop(), "quick");
        assert.strictEqual(col.length, 0);

        assert.strictEqual(col.pop(), undefined);
        assert.strictEqual(col.length, 0);
    });

    test('Stack - reduce method - v1', function (assert) {
        var col = new Stack({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('Stack - reduce method - v1.1', function (assert) {
        var col = new Stack({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('Stack - reduce method - v2', function (assert) {
        var col = new Stack([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('Stack - reduce method - v2.1', function (assert) {
        var col = new Stack([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });

    test('Stack - reduce method - v2.2', function (assert) {
        var col = new Stack([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('Stack - size method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.size(), col.length);
    });

    test('Stack - size method - v2', function (assert) {
        var col = new Stack(["quick", "brown", "fox", "dog", "quick"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('Stack - size method - v3', function (assert) {
        var col = new Stack(new List("quick", "brown", "fox", "dog", "quick"));
        assert.strictEqual(col.size(), col.length);
    });

    test('Stack - size method - v4', function (assert) {
        var col = new Stack();
        assert.strictEqual(col.size(), col.length);
    });

    test('Stack - toArray method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(arr[i].length >= 3);
        }
    });

    test('Stack - toSortedArray method - v1', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Stack - toSortedArray method - v2', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Stack - toJSON method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Stack.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(data[i].length >= 3);
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Stack - toString method', function (assert) {
        var col = new Stack("quick", "brown", "fox", "dog"),
            str = col.toString(),
            arr,
            i;
        assert.strictEqual(typeof str, "string");
        assert.ok(str.length > 0);
        arr = str.split(',');
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(arr[i].length >= 3);
        }
    });
}());
