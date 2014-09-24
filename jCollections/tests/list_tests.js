/* global console, test, ok, Comparer, List, Set */
(function (undefined) {
    'use strict';

    test('Empty List initialization', function (assert) {
        var col = new List();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
    });

    test('List initialization with single value', function (assert) {
        var col = new List("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
    });

    test('List initialization with multiple values', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List initialization from array', function (assert) {
        var col = new List(["quick", "brown", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List initialization from collection', function (assert) {
        var col = new List(new Set("quick", "brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - add method - v1', function (assert) {
        var col = new List();
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.add("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - add method - v2', function (assert) {
        var col = new List(["quick", "brown"]);
        col.add("fox");
        col.add("dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - addRange method - v1', function (assert) {
        var col = new List("quick");
        col.addRange("brown");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
    });

    test('List - addRange method - v2', function (assert) {
        var col = new List("quick");
        col.addRange("brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - addRange method - v3', function (assert) {
        var col = new List("quick");
        col.addRange(["brown", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - addRange method - v4', function (assert) {
        var col = new List("quick");
        col.addRange(new Set("brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('List - all method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('List - all method - v1.1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });
    
    test('List - any method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('List - any method - v1.1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('List - clear method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('List - contains method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(!col.contains("QUICK"));
        assert.ok(!col.contains("Brown"));
        assert.ok(!col.contains("zzz"));
    });

    test('List - find method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(col.contains(result));
    });

    test('List - findAll method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (item) {
            assert.ok(col.contains(item));
        });
    });

    test('List - findIndex method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.strictEqual(col.findIndex("quick"), 0);
        assert.strictEqual(col.findIndex("brown"), 1);
        assert.strictEqual(col.findIndex("fox"), 2);
        assert.strictEqual(col.findIndex("dog"), 3);
        assert.strictEqual(col.findIndex("zzz"), -1);
        assert.strictEqual(col.findIndex("QUICK"), -1);
        assert.strictEqual(col.findIndex("Brown"), -1);
    });

    test('List - findLastIndex method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.findIndex("quick"), 0);
        assert.strictEqual(col.findLastIndex("quick"), 4);
        assert.strictEqual(col.findLastIndex("brown"), 1);
        assert.strictEqual(col.findLastIndex("fox"), 2);
        assert.strictEqual(col.findLastIndex("dog"), 3);
        assert.strictEqual(col.findLastIndex("zzz"), -1);
        assert.strictEqual(col.findLastIndex("QUICK"), -1);
        assert.strictEqual(col.findLastIndex("Brown"), -1);
    });

    test('List - foreach method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        col.forEach(function (i) {
            assert.strictEqual(i, col.findIndex(this));
        });
    });

    test('List - get method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"), i;
        assert.strictEqual(col.length, 4);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.get(i));
            assert.ok(col.get(i).length > 0);
        }
    });

    test('List - groupBy method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
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

    test('List - groupBy method - v2', function (assert) {
        var col = new List({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('List - groupBy method - v2.1', function (assert) {
        var col = new List({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('List - groupBy method - v2.2', function (assert) {
        var col = new List({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('List - groupBy method - v2.3', function (assert) {
        var col = new List({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('List - isEmpty method - v1', function (assert) {
        var col = new List();
        assert.ok(col.isEmpty());
    });

    test('List - isEmpty method - v1.1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('List - insert method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        col.insert(0, "abc");
        col.insert(3, "def");
        assert.strictEqual(col.length, 6);
        assert.strictEqual(col.findIndex("abc"), 0);
        assert.strictEqual(col.findIndex("quick"), 1);
        assert.strictEqual(col.findIndex("brown"), 2);
        assert.strictEqual(col.findIndex("def"), 3);
        assert.strictEqual(col.findIndex("fox"), 4);
        assert.strictEqual(col.findIndex("dog"), 5);
    });

    test('List - map method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            results = col.map(function () {
                return this.substr(0, 2);
            });
        results.forEach(function (item, i) {
            assert.strictEqual(col.get(i).indexOf(item), 0);
        });
    });

    test('List - map method - v1.1', function (assert) {
        var col = new List({ value: "quick" },
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
    
    test('List - map method - v2', function (assert) {
        var col = new List({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            results = col.map("value");
        results.forEach(function (item, i) {
            assert.strictEqual(col.get(i).value, item);
        });
    });

    test('List - moveTo method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.ok(col.moveTo(0, "dog"));
        assert.ok(col.moveTo(3, "quick"));
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.findIndex("dog"), 0);
        assert.strictEqual(col.findIndex("brown"), 1);
        assert.strictEqual(col.findIndex("fox"), 2);
        assert.strictEqual(col.findIndex("quick"), 3);
    });

    test('List - reduce method - v1', function (assert) {
        var col = new List({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });
    
    test('List - reduce method - v1.1', function (assert) {
        var col = new List({ value: "quick" },
                            { value: "brown" },
                            { value: "fox" },
                            { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });
    
    test('List - reduce method - v2', function (assert) {
        var col = new List([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                  return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('List - reduce method - v2.1', function (assert) {
        var col = new List([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                  return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });
    
    test('List - reduce method - v2.2', function (assert) {
        var col = new List([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                  return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('List - remove method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog", "quick");
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

    test('List - removeAt method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"), i;
        assert.ok(!col.removeAt(5));
        for (i = col.length - 1; i >= 0; i--) {
            assert.ok(col.removeAt(i));
        }
        assert.ok(col.isEmpty());
    });

    test('List - removeWhere method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"), i;
        col.removeWhere(function (i) {
            return this[0] === "b" || i === 3;
        });
        assert.strictEqual(col.length, 2);
        assert.ok(col.contains("quick"));
        assert.ok(!col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(!col.contains("dog"));
    });

    test('List - reverse method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            reversedCol = col.reverse();
        assert.strictEqual(reversedCol, col);
        assert.strictEqual(reversedCol.findIndex("dog"), 0);
        assert.strictEqual(reversedCol.findIndex("fox"), 1);
        assert.strictEqual(reversedCol.findIndex("brown"), 2);
        assert.strictEqual(reversedCol.findIndex("quick"), 3);
    });

    test('List - size method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog");
        assert.strictEqual(col.size(), col.length);
    });

    test('List - size method - v2', function (assert) {
        var col = new List(["quick", "brown", "fox", "dog"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('List - size method - v3', function (assert) {
        var col = new List(new Set("quick", "brown", "fox", "dog"));
        assert.strictEqual(col.size(), col.length);
    });

    test('List - size method - v4', function (assert) {
        var col = new List();
        assert.strictEqual(col.size(), col.length);
    });

    test('List - toArray method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], col.get(i));
        }
    });

    test('List - toSortedArray method - v1', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('List - toSortedArray method - v2', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('List - toJSON method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = List.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.strictEqual(newCol.get(i), col.get(i));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('List - toString method', function (assert) {
        var col = new List("quick", "brown", "fox", "dog"),
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
