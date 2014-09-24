/* global console, test, ok, Comparer, EqualityComparer, Collection, Tree, List */
(function (undefined) {
    'use strict';

    test('Empty Tree initialization', function (assert) {
        var col = new Tree();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.getComparer(), Comparer.standard);
    });

    test('Tree initialization with single value', function (assert) {
        var col = new Tree("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
        assert.strictEqual(col.getComparer(), Comparer.standard);
    });

    test('Tree initialization with multiple values', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getComparer(), Comparer.standard);
    });

    test('Tree initialization with multiple values (reverse comparer)', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getComparer(), Comparer.reverse);
    });

    test('Tree initialization from array', function (assert) {
        var col = new Tree(["quick", "brown", "fox", "dog", "brown", "fox"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree initialization from array (reverse comparer)', function (assert) {
        var col = new Tree(Comparer.reverse,
                           ["quick", "brown", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getComparer(), Comparer.reverse);
    });

    test('Tree initialization from collection', function (assert) {
        var col = new Tree(new Tree("quick", "brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getComparer(), Comparer.standard);
    });

    test('Tree initialization from collection (reverse comparer)', function (assert) {
        var col = new Tree(Comparer.reverse,
                  new Tree("quick", "brown", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getComparer(), Comparer.reverse);
    });

    test('Tree - add method - v1', function (assert) {
        var col = new Tree();
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["brown", "Brown", "dog", "fox", "quick"]));
    });

    test('Tree - add method - v1.1', function (assert) {
        var col = new Tree(["quick", "brown"]);
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["brown", "Brown", "dog", "fox", "quick"]));
    });

    test('Tree - add method - v2', function (assert) {
        var col = new Tree(Comparer.reverse);
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["quick", "fox", "dog", "Brown", "brown"]));
    });

    test('Tree - add method - v2.1', function (assert) {
        var col = new Tree(Comparer.reverse, ["quick", "brown"]);
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
        assert.ok(Collection.equivalent(col, ["quick", "fox", "dog", "Brown", "brown"]));
    });

    test('Tree - addRange method - v1', function (assert) {
        var col = new Tree("quick");
        col.addRange("brown");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
    });

    test('Tree - addRange method - v1.1', function (assert) {
        var col = new Tree(Comparer.reverse, "quick");
        col.addRange("brown");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
    });

    test('Tree - addRange method - v2', function (assert) {
        var col = new Tree("quick", "brown");
        col.addRange("fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - addRange method - v2.1', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown");
        col.addRange("fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - addRange method - v3', function (assert) {
        var col = new Tree("quick", "brown");
        col.addRange(["fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - addRange method - v3.1', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown");
        col.addRange(["fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - addRange method - v4', function (assert) {
        var col = new Tree("quick", "brown");
        col.addRange(new List("fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - addRange method - v4.1', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown");
        col.addRange(new List("fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Tree - all method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('Tree - all method - v1.1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });

    test('Tree - any method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('Tree - any method - v1.1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('Tree - clear method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog");
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Tree - contains method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(!col.contains("zzz"));
    });

    test('Tree - contains method - v2', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(!col.contains("zzz"));
    });

    test('Tree - find method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(col.contains(result));
    });

    test('Tree - findAll method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (result) {
            assert.ok(col.contains(result));
        });
    });

    test('Tree - foreach method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog");
        col.forEach(function () {
            assert.ok(col.contains(this));
        });
    });

    test('Tree - groupBy method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog", "fox"),
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

    test('Tree - groupBy method - v2', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.word, obj2.word); },
                           { word: "quick", size: 5 },
                           { word: "brown", size: 5 },
                           { word: "fox", size: 3 },
                           { word: "dog", size: 3}),
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

    test('Tree - groupBy method - v2.1', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.word, obj2.word); },
                           { word: "quick", size: 5 },
                           { word: "brown", size: 5 },
                           { word: "fox", size: 3 },
                           { word: "dog", size: 3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Tree - groupBy method - v2.2', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.reverse(obj1.word, obj2.word); },
                           { word: "quick", size: 5 },
                           { word: "brown", size: 5 },
                           { word: "fox", size: 3 },
                           { word: "dog", size: 3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('Tree - groupBy method - v2.3', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.word, obj2.word); },
                           { word: "quick", size: 5 },
                           { word: "brown", size: 5 },
                           { word: "fox", size: 3 },
                           { word: "dog", size: 3}),
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

    test('Tree - isEmpty method - v1', function (assert) {
        var col = new Tree();
        assert.ok(col.isEmpty());
    });

    test('Tree - isEmpty method - v1.1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('Tree - map method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            results = col.map(function () {
                return this.substr(0, 2);
            });
        results.forEach(function (item, i) {
            assert.ok(col.find(function () {
                return this.indexOf(item) === 0;
            }) !== undefined);
        });
    });

    test('Tree - map method - v1.1', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.value, obj2.value) },
                           { value: "quick" },
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

    test('Tree - map method - v2', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.reverse(obj1.value, obj2.value) },
                           { value: "quick" },
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

    test('Tree - reduce method - v1', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.value, obj2.value) },
                           { value: "quick" },
                           { value: "brown" },
                           { value: "fox" },
                           { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('Tree - reduce method - v1.1', function (assert) {
        var col = new Tree(function (obj1, obj2) { return Comparer.standard(obj1.value, obj2.value) },
                           { value: "quick" },
                           { value: "brown" },
                           { value: "fox" },
                           { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('Tree - reduce method - v2', function (assert) {
        var col = new Tree([5, 6], [1,  2], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,3,4,5,6].toString());
    });

    test('Tree - reduce method - v2.1', function (assert) {
        var col = new Tree([5,  6], [1, 2], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [5,6,3,4,1,2].toString());
    });

    test('Tree - reduce method - v2.2', function (assert) {
        var col = new Tree([5, 6], [1,  2], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,5,6,3,4,1,2].toString());
    });

    test('Tree - remove method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.length, 4);
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

        assert.ok(col.remove("brown"));
        assert.strictEqual(col.length, 1);

        assert.ok(col.remove("fox"));
        assert.strictEqual(col.length, 0);
    });

    test('Tree - remove method - v1.1', function (assert) {
        var col = new Tree(Comparer.reverse, "quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.length, 4);
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

        assert.ok(col.remove("brown"));
        assert.strictEqual(col.length, 1);

        assert.ok(col.remove("fox"));
        assert.strictEqual(col.length, 0);
    });

    test('Tree - size method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.size(), col.length);
    });

    test('Tree - size method - v2', function (assert) {
        var col = new Tree(["quick", "brown", "fox", "dog", "quick"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('Tree - size method - v3', function (assert) {
        var col = new Tree(new Tree("quick", "brown", "fox", "dog", "quick"));
        assert.strictEqual(col.size(), col.length);
    });

    test('Tree - size method - v4', function (assert) {
        var col = new Tree();
        assert.strictEqual(col.size(), col.length);
    });

    test('Tree - toArray method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.contains(arr[i]));
        }
    });

    test('Tree - toSortedArray method - v1', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Tree - toSortedArray method - v2', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Tree - toJSON method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Tree.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(newCol.contains(data[i]));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Tree - toString method', function (assert) {
        var col = new Tree("quick", "brown", "fox", "dog"),
            str = col.toString(),
            arr,
            i;
        assert.strictEqual(typeof str, "string");
        assert.ok(str.length > 0);
        arr = str.split(',');
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.contains(arr[i]));
        }
    });
})();
