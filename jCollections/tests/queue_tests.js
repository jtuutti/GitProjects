/* global console, test, ok, Comparer, EqualityComparer, Collection, Queue, List */
(function (undefined) {
    'use strict';

    test('Empty Queue initialization', function (assert) {
        var col = new Queue();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
    });

    test('Queue initialization with single value', function (assert) {
        var col = new Queue("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
    });

    test('Queue initialization with multiple values', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Queue initialization from array', function (assert) {
        var col = new Queue(["quick", "brown", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Queue initialization from collection', function (assert) {
        var col = new Queue(new List("quick", "brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Queue - all method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('Queue - all method - v1.1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });

    test('Queue - any method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('Queue - any method - v1.1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('Queue - clear method', function (assert) {
        var col = new Queue();
        col.enqueue("quick");
        col.enqueue("brown");
        col.enqueue("fox");
        col.enqueue("dog");
        col.enqueue("Brown");
        assert.strictEqual(col.length, 5);
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Queue - enqueue method - v1', function (assert) {
        var col = new Queue();
        col.enqueue("quick");
        col.enqueue("brown");
        col.enqueue("fox");
        col.enqueue("dog");
        col.enqueue("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["quick", "brown", "fox", "dog", "Brown"]));
    });

    test('Queue - enqueue method - v2', function (assert) {
        var col = new Queue("quick", "brown");
        col.enqueue("fox");
        col.enqueue("dog");
        col.enqueue("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["quick", "brown", "fox", "dog", "Brown"]));
    });

    test('Queue - dequeue method', function (assert) {
        var col = new Queue();
        col.enqueue("quick");
        col.enqueue("brown");
        col.enqueue("fox");
        col.enqueue("dog");
        col.enqueue("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);

        assert.strictEqual(col.dequeue(), "quick");
        assert.strictEqual(col.length, 4);

        assert.strictEqual(col.dequeue(), "brown");
        assert.strictEqual(col.length, 3);

        assert.strictEqual(col.dequeue(), "fox");
        assert.strictEqual(col.length, 2);

        assert.strictEqual(col.dequeue(), "dog");
        assert.strictEqual(col.length, 1);

        assert.strictEqual(col.dequeue(), "Brown");
        assert.strictEqual(col.length, 0);

        assert.strictEqual(col.dequeue(), undefined);
        assert.strictEqual(col.length, 0);
    });

    test('Queue - find method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(result && result.length >= 3);
    });

    test('Queue - findAll method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (result) {
            assert.ok(result && result.length >= 3);
        });
    });

    test('Queue - foreach method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog");
        col.forEach(function () {
            assert.ok(this && this.length >= 3);
        });
    });

    test('Queue - groupBy method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog", "fox"),
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

    test('Queue - groupBy method - v2', function (assert) {
        var col = new Queue({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('Queue - groupBy method - v2.1', function (assert) {
        var col = new Queue({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Queue - groupBy method - v2.2', function (assert) {
        var col = new Queue({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('Queue - groupBy method - v2.3', function (assert) {
        var col = new Queue({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('Queue - isEmpty method - v1', function (assert) {
        var col = new Queue();
        assert.ok(col.isEmpty());
    });

    test('Queue - isEmpty method - v1.1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('Queue - map method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            results = col.map(function () {
                return this.substr(0, 2);
            });
        results.forEach(function (item, i) {
            assert.ok(col.find(function () {
                return this.indexOf(item) === 0;
            }) !== undefined);
        });
    });

    test('Queue - map method - v1.1', function (assert) {
        var col = new Queue({ value: "quick" },
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

    test('Queue - map method - v2', function (assert) {
        var col = new Queue({ value: "quick" },
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

    test('Queue - peek method', function (assert) {
        var col = new Queue();
        col.enqueue("quick");
        col.enqueue("brown");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
        assert.strictEqual(col.peek(), "quick");
        assert.strictEqual(col.length, 2);
    });

    test('Queue - reduce method - v1', function (assert) {
        var col = new Queue({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('Queue - reduce method - v1.1', function (assert) {
        var col = new Queue({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('Queue - reduce method - v2', function (assert) {
        var col = new Queue([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('Queue - reduce method - v2.1', function (assert) {
        var col = new Queue([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });

    test('Queue - reduce method - v2.2', function (assert) {
        var col = new Queue([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('Queue - size method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.size(), col.length);
    });

    test('Queue - size method - v2', function (assert) {
        var col = new Queue(["quick", "brown", "fox", "dog", "quick"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('Queue - size method - v3', function (assert) {
        var col = new Queue(new List("quick", "brown", "fox", "dog", "quick"));
        assert.strictEqual(col.size(), col.length);
    });

    test('Queue - size method - v4', function (assert) {
        var col = new Queue();
        assert.strictEqual(col.size(), col.length);
    });

    test('Queue - toArray method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(arr[i].length >= 3);
        }
    });

    test('Queue - toSortedArray method - v1', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Queue - toSortedArray method - v2', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Queue - toJSON method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Queue.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(data[i].length >= 3);
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Queue - toString method', function (assert) {
        var col = new Queue("quick", "brown", "fox", "dog"),
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
