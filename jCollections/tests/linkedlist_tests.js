/* global console, test, ok, Comparer, LinkedList, Set */
(function (undefined) {
    'use strict';

    test('Empty LinkedList initialization', function (assert) {
        var col = new LinkedList();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
    });

    test('LinkedList initialization with single value', function (assert) {
        var col = new LinkedList("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
    });

    test('LinkedList initialization with multiple values', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('LinkedList - addFirst method - v1', function (assert) {
        var col = new LinkedList();
        col.addFirst("quick");
        col.addFirst("brown");
        col.addFirst("fox");
        col.addFirst("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.get(0), "dog");
        assert.strictEqual(col.get(1), "fox");
        assert.strictEqual(col.get(2), "brown");
        assert.strictEqual(col.get(3), "quick");
    });

    test('LinkedList - addFirst method - v2', function (assert) {
        var col = new LinkedList(["quick", "brown"]);
        col.addFirst("fox");
        col.addFirst("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.get(0), "dog");
        assert.strictEqual(col.get(1), "fox");
        assert.strictEqual(col.get(2), "quick");
        assert.strictEqual(col.get(3), "brown");
    });

    test('LinkedList - addLast method - v1', function (assert) {
        var col = new LinkedList();
        col.addLast("quick");
        col.addLast("brown");
        col.addLast("fox");
        col.addLast("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.get(0), "quick");
        assert.strictEqual(col.get(1), "brown");
        assert.strictEqual(col.get(2), "fox");
        assert.strictEqual(col.get(3), "dog");
    });

    test('LinkedList - addLast method - v2', function (assert) {
        var col = new LinkedList(["quick", "brown"]);
        col.addLast("fox");
        col.addLast("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.get(0), "quick");
        assert.strictEqual(col.get(1), "brown");
        assert.strictEqual(col.get(2), "fox");
        assert.strictEqual(col.get(3), "dog");
    });

    test('LinkedList - all method - v1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('LinkedList - all method - v1.1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });

    test('LinkedList - any method - v1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('LinkedList - any method - v1.1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('LinkedList - clear method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('LinkedList - contains method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(!col.contains("QUICK"));
        assert.ok(!col.contains("Brown"));
        assert.ok(!col.contains("zzz"));
    });

    test('LinkedList - find method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(col.contains(result));
    });

    test('LinkedList - findAll method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (item) {
            assert.ok(col.contains(item));
        });
    });

    test('LinkedList - findIndex method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.strictEqual(col.findIndex("quick"), 0);
        assert.strictEqual(col.findIndex("brown"), 1);
        assert.strictEqual(col.findIndex("fox"), 2);
        assert.strictEqual(col.findIndex("dog"), 3);
        assert.strictEqual(col.findIndex("zzz"), -1);
        assert.strictEqual(col.findIndex("QUICK"), -1);
        assert.strictEqual(col.findIndex("Brown"), -1);
    });

    test('LinkedList - findLastIndex method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.findIndex("quick"), 0);
        assert.strictEqual(col.findLastIndex("quick"), 4);
        assert.strictEqual(col.findLastIndex("brown"), 1);
        assert.strictEqual(col.findLastIndex("fox"), 2);
        assert.strictEqual(col.findLastIndex("dog"), 3);
        assert.strictEqual(col.findLastIndex("zzz"), -1);
        assert.strictEqual(col.findLastIndex("QUICK"), -1);
        assert.strictEqual(col.findLastIndex("Brown"), -1);
    });

    test('LinkedList - foreach method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        col.forEach(function (i) {
            assert.strictEqual(i, col.findIndex(this));
        });
    });

    test('LinkedList - get method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"), i;
        assert.strictEqual(col.length, 4);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.get(i));
            assert.ok(col.get(i).length > 0);
        }
    });

    test('LinkedList - groupBy method - v1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
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

    test('LinkedList - groupBy method - v2', function (assert) {
        var col = new LinkedList({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('LinkedList - groupBy method - v2.1', function (assert) {
        var col = new LinkedList({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('LinkedList - groupBy method - v2.2', function (assert) {
        var col = new LinkedList({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('LinkedList - groupBy method - v2.3', function (assert) {
        var col = new LinkedList({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('LinkedList - isEmpty method - v1', function (assert) {
        var col = new LinkedList();
        assert.ok(col.isEmpty());
    });

    test('LinkedList - isEmpty method - v1.1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('LinkedList - map method - v1.1', function (assert) {
        var col = new LinkedList({ value: "quick" },
                                 { value: "brown" },
                                 { value: "fox" },
                                 { value: "dog" }),
            results = col.map(function () {
                return this.value;
            });
        results.forEach(function (item, i) {
            assert.strictEqual(col.get(i).value, item);
        });
    });

    test('LinkedList - map method - v2', function (assert) {
        var col = new LinkedList({ value: "quick" },
                                 { value: "brown" },
                                 { value: "fox" },
                                 { value: "dog" }),
            results = col.map("value");
        results.forEach(function (item, i) {
            assert.strictEqual(col.get(i).value, item);
        });
    });

    test('LinkedList - reduce method - v1', function (assert) {
        var col = new LinkedList({ value: "quick" },
                                 { value: "brown" },
                                 { value: "fox" },
                                 { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('LinkedList - reduce method - v1.1', function (assert) {
        var col = new LinkedList({ value: "quick" },
                                 { value: "brown" },
                                 { value: "fox" },
                                 { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('LinkedList - reduce method - v2', function (assert) {
        var col = new LinkedList([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('LinkedList - reduce method - v2.1', function (assert) {
        var col = new LinkedList([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });

    test('LinkedList - reduce method - v2.2', function (assert) {
        var col = new LinkedList([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('LinkedList - remove method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.length, 5);
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.remove("quick"));
        assert.strictEqual(col.length, 3);
        assert.ok(!col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(!col.remove("Quick"));
        assert.strictEqual(col.length, 3);
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.remove("dog"));
        assert.strictEqual(col.length, 2);
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));

        assert.ok(col.remove("brown"));
        assert.strictEqual(col.length, 1);
        assert.ok(col.contains("fox"));

        assert.ok(col.remove("fox"));
        assert.strictEqual(col.length, 0);
    });

    test('LinkedList - removeFirst method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.strictEqual(col.length, 4);
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.removeFirst());
        assert.strictEqual(col.length, 3);
        assert.ok(!col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.removeFirst());
        assert.strictEqual(col.length, 2);
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.removeFirst());
        assert.strictEqual(col.length, 1);
        assert.ok(col.contains("dog"));

        assert.ok(col.removeFirst());
        assert.strictEqual(col.length, 0);
    });

    test('LinkedList - removeLast method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.strictEqual(col.length, 4);
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));

        assert.ok(col.removeLast());
        assert.strictEqual(col.length, 3);
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(!col.contains("dog"));

        assert.ok(col.removeLast());
        assert.strictEqual(col.length, 2);
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));

        assert.ok(col.removeLast());
        assert.strictEqual(col.length, 1);
        assert.ok(col.contains("quick"));

        assert.ok(col.removeLast());
        assert.strictEqual(col.length, 0);
    });

    test('LinkedList - size method - v1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog");
        assert.strictEqual(col.size(), col.length);
    });

    test('LinkedList - size method - v2', function (assert) {
        var col = new LinkedList(["quick", "brown", "fox", "dog"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('LinkedList - size method - v3', function (assert) {
        var col = new LinkedList(new Set("quick", "brown", "fox", "dog"));
        assert.strictEqual(col.size(), col.length);
    });

    test('LinkedList - size method - v4', function (assert) {
        var col = new LinkedList();
        assert.strictEqual(col.size(), col.length);
    });

    test('LinkedList - toArray method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], col.get(i));
        }
    });

    test('LinkedList - toReverseArray method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            arr = col.toReverseArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], col.get(col.length - i - 1));
        }
    });

    test('LinkedList - toSortedArray method - v1', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('LinkedList - toSortedArray method - v2', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('LinkedList - toJSON method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = LinkedList.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.strictEqual(newCol.get(i), col.get(i));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('LinkedList - toString method', function (assert) {
        var col = new LinkedList("quick", "brown", "fox", "dog"),
            str = col.toString(),
            arr,
            i;
        assert.strictEqual(typeof str, "string");
        assert.ok(str.length > 0);
        arr = str.split(',');
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], col.get(i));
        }
    });
}());