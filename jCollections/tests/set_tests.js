/* global console, test, ok, Comparer, EqualityComparer, Collection, Set, List */
(function (undefined) {
    'use strict';

    test('Empty Set initialization', function (assert) {
        var col = new Set();
        assert.ok(col);
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Set initialization with single value', function (assert) {
        var col = new Set("quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Set initialization with multiple values', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog", "brown", "fox");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Set initialization with multiple values (case insensitive comparer)', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog", "BROWN", "FoX");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Set initialization from array', function (assert) {
        var col = new Set(["quick", "brown", "fox", "dog", "brown", "fox"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Set initialization from array (case insensitive comparer)', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive,
                           ["quick", "brown", "fox", "dog", "BROWN", "FoX"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Set initialization from collection', function (assert) {
        var col = new Set(new Set("quick", "brown", "fox", "dog", "brown", "fox"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.standard);
    });

    test('Set initialization from collection (case insensitive comparer)', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive,
                           new Set("quick", "brown", "fox", "dog", "BROWN", "FoX"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
        assert.strictEqual(col.getEqualityComparer(), EqualityComparer.caseInsensitive);
    });

    test('Set - add method - v1', function (assert) {
        var col = new Set();
        col.add("quick");
        col.add("brown");
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
    });

    test('Set - add method - v2', function (assert) {
        var col = new Set(["quick", "brown"]);
        col.add("fox");
        col.add("dog");
        col.add("Brown");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
    });

    test('Set - addRange method - v1', function (assert) {
        var col = new Set("quick");
        col.addRange("Quick");
        assert.ok(col);
        assert.strictEqual(col.length, 2);
    });

    test('Set - addRange method - v1.1', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick");
        col.addRange("Quick");
        assert.ok(col);
        assert.strictEqual(col.length, 1);
    });

    test('Set - addRange method - v2', function (assert) {
        var col = new Set("quick", "brown");
        col.addRange("Quick", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 5);
    });

    test('Set - addRange method - v2.1', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick", "brown");
        col.addRange("Quick", "fox", "dog");
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Set - addRange method - v3', function (assert) {
        var col = new Set("quick", "brown");
        col.addRange(["Quick", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 5);
    });

    test('Set - addRange method - v3.1', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick", "brown");
        col.addRange(["Quick", "fox", "dog"]);
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Set - addRange method - v4', function (assert) {
        var col = new Set("quick", "brown");
        col.addRange(new List("Quick", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 5);
    });

    test('Set - addRange method - v4.1', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick", "brown");
        col.addRange(new List("Quick", "fox", "dog"));
        assert.ok(col);
        assert.strictEqual(col.length, 4);
    });

    test('Set - all method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this.length >= 3;
            });
        assert.strictEqual(result, true);
    });

    test('Set - all method - v1.1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            result = col.all(function () {
                return this === "dog";
            });
        assert.strictEqual(result, false);
    });

    test('Set - any method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this === "dog";
            });
        assert.strictEqual(result, true);
    });

    test('Set - any method - v1.1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            result = col.any(function () {
                return this.length < 3;
            });
        assert.strictEqual(result, false);
    });

    test('Set - clear method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        col.clear();
        assert.strictEqual(col.length, 0);
        assert.strictEqual(col.size(), 0);
        assert.ok(col.isEmpty());
    });

    test('Set - contains method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(!col.contains("QUICK"));
        assert.ok(!col.contains("Brown"));
        assert.ok(!col.contains("zzz"));
    });

    test('Set - contains method - v2', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog");
        assert.ok(col.contains("quick"));
        assert.ok(col.contains("brown"));
        assert.ok(col.contains("fox"));
        assert.ok(col.contains("dog"));
        assert.ok(col.contains("QUICK"));
        assert.ok(col.contains("Brown"));
        assert.ok(!col.contains("zzz"));
    });

    test('Set - difference method - v1', function (assert) {
        var set1 = new Set("quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.difference(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 3);
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
        assert.ok(targetSet.contains("dog"));
    });

    test('Set - difference method - v1.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.difference(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
    });

    test('Set - difference method - v1.2', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new Set("quick", "DOG", "Sled"),
            targetSet = set1.difference(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
    });

    test('Set - difference method - v1.3', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new List("quick", "DOG", "Sled"),
            targetSet = set1.difference(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
    });

    test('Set - difference method - v2', function (assert) {
        var obj1 = {},
            obj2 = {},
            obj3 = {},
            set1 = new Set(obj1, obj2),
            set2 = new Set([obj2, obj3]),
            targetSet = set1.difference(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 1);
        assert.ok(targetSet.contains(obj1));
    });

    test('Set - equals method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        assert.ok(col.equals("quick", "brown", "fox", "dog"));
        assert.ok(col.equals(["quick", "brown", "fox", "dog"]));
        assert.ok(!col.equals("quick", "brown", "dog", "fox"));
        assert.ok(!col.equals(new List("quick", "brown", "dog", "fox")));
    });

    test('Set - equals method - v2', function (assert) {
        var col = new Set(EqualityComparer.caseInsensitive, "Quick", "Brown", "FoX", "Dog");
        assert.ok(col.equals("quick", "brown", "fox", "dog"));
        assert.ok(col.equals(["quick", "brown", "fox", "dog"]));
        assert.ok(!col.equals("quick", "brown", "dog", "fox"));
        assert.ok(!col.equals(new List("quick", "brown", "dog", "fox")));
    });

    test('Set - find method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            result = col.find(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(result);
        assert.ok(col.contains(result));
    });

    test('Set - findAll method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            results = col.findAll(function () {
                return typeof this === "string" && this.indexOf("o") >= 0;
            });
        assert.ok(Array.isArray(results));
        assert.strictEqual(results.length, 3);
        results.forEach(function (result) {
            assert.ok(col.contains(result));
        });
    });

    test('Set - foreach method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        col.forEach(function () {
            assert.ok(col.contains(this));
        });
    });

    test('Set - isSubsetOf method - v1', function (assert) {
        var set1 = new Set(1, 2, 3, 4),
            set2 = new Set(2, 3, 1);
        assert.ok(set1.isSubsetOf(set1));
        assert.ok(set2.isSubsetOf(set1));
        assert.ok(!set1.isSubsetOf(set2));
    });

    test('Set - isSubsetOf method - v1.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, 'a', 'B', 'c', 'D'),
            set2 = new Set(EqualityComparer.caseInsensitive,'b', 'c', 'a');
        assert.ok(set1.isSubsetOf(set1));
        assert.ok(set2.isSubsetOf(set1));
        assert.ok(!set1.isSubsetOf(set2));
    });

    test('Set - isProperSubsetOf method - v2', function (assert) {
        var set1 = new Set(1, 2, 3, 4),
            set2 = new Set(2, 3, 1);
        assert.ok(!set1.isProperSubsetOf(set1));
        assert.ok(set2.isProperSubsetOf(set1));
        assert.ok(!set1.isProperSubsetOf(set2));
    });

    test('Set - isProperSubsetOf method - v2.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, 'a', 'B', 'c', 'D'),
            set2 = new Set(EqualityComparer.caseInsensitive,'b', 'c', 'a');
        assert.ok(!set1.isProperSubsetOf(set1));
        assert.ok(set2.isProperSubsetOf(set1));
        assert.ok(!set1.isProperSubsetOf(set2));
    });

    test('Set - isSupersetOf method - v1', function (assert) {
        var set1 = new Set(1, 2, 3, 4),
            set2 = new Set(2, 3, 1);
        assert.ok(set1.isSupersetOf(set1));
        assert.ok(set1.isSupersetOf(set2));
        assert.ok(!set2.isSupersetOf(set1));
    });

    test('Set - isSupersetOf method - v1.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, 'a', 'B', 'c', 'D'),
            set2 = new Set(EqualityComparer.caseInsensitive,'b', 'c', 'a');
        assert.ok(set1.isSupersetOf(set1));
        assert.ok(set1.isSupersetOf(set2));
        assert.ok(!set2.isSupersetOf(set1));
    });

    test('Set - isProperSupersetOf method - v2', function (assert) {
        var set1 = new Set(1, 2, 3, 4),
            set2 = new Set(2, 3, 1);
        assert.ok(!set1.isProperSupersetOf(set1));
        assert.ok(set1.isProperSupersetOf(set2));
        assert.ok(!set2.isProperSupersetOf(set1));
    });

    test('Set - isProperSupersetOf method - v2.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, 'a', 'B', 'c', 'D'),
            set2 = new Set(EqualityComparer.caseInsensitive,'b', 'c', 'a');
        assert.ok(!set1.isProperSupersetOf(set1));
        assert.ok(set1.isProperSupersetOf(set2));
        assert.ok(!set2.isProperSupersetOf(set1));
    });

    test('Set - groupBy method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog", "fox"),
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

    test('Set - groupBy method - v2', function (assert) {
        var col = new Set({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('Set - groupBy method - v2.1', function (assert) {
        var col = new Set({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(Array.isArray(groupedCol.get(3)));
        assert.ok(Array.isArray(groupedCol.get(5)));
        assert.strictEqual(groupedCol.get(3).length, 2);
        assert.strictEqual(groupedCol.get(5).length, 2);
    });

    test('Set - groupBy method - v2.2', function (assert) {
        var col = new Set({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
            groupedCol = col.groupBy("size", "word");
        assert.strictEqual(groupedCol.length, 2);
        assert.ok(groupedCol.containsKey(3));
        assert.ok(groupedCol.containsKey(5));
        assert.ok(groupedCol.get(3).word !== undefined);
        assert.ok(groupedCol.get(5).word !== undefined);
        assert.strictEqual(groupedCol.get(3).word.length, 2);
        assert.strictEqual(groupedCol.get(5).word.length, 2);
    });

    test('Set - groupBy method - v2.3', function (assert) {
        var col = new Set({word:"quick",size:5},{word:"brown",size:5},{word:"fox",size:3},{word:"dog",size:3}),
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

    test('Set - intersection method - v1', function (assert) {
        var set1 = new Set("quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.intersection(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 1);
        assert.ok(targetSet.contains("quick"));
    });

    test('Set - intersection method - v1.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.intersection(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("dog"));
    });

    test('Set - intersection method - v1.2', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new Set("quick", "DOG", "Sled"),
            targetSet = set1.intersection(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("dog"));
    });

    test('Set - intersection method - v1.3', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new List("quick", "DOG", "Sled"),
            targetSet = set1.intersection(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 2);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("dog"));
    });

    test('Set - intersection method - v2', function (assert) {
        var obj1 = {},
            obj2 = {},
            obj3 = {},
            set1 = new Set(obj1, obj2),
            set2 = new Set([obj2, obj3]),
            targetSet = set1.intersection(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 1);
        assert.ok(targetSet.contains(obj2));
    });

    test('Set - isEmpty method - v1', function (assert) {
        var col = new Set();
        assert.ok(col.isEmpty());
    });

    test('Set - isEmpty method - v1.1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        assert.ok(!col.isEmpty());
    });

    test('Set - isSubset method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog");
        assert.ok(col.equals("quick", "brown", "fox", "dog"));
        assert.ok(col.equals(["quick", "brown", "fox", "dog"]));
        assert.ok(!col.equals("quick", "brown", "dog", "fox"));
        assert.ok(!col.equals(new List("quick", "brown", "dog", "fox")));
    });

    test('Set - map method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            results = col.map(function () {
                return this.substr(0, 2);
            });
        results.forEach(function (item, i) {
            assert.ok(col.find(function () {
                return this.indexOf(item) === 0;
            }) !== undefined);
        });
    });

    test('Set - map method - v1.1', function (assert) {
        var col = new Set({ value: "quick" },
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

    test('Set - map method - v2', function (assert) {
        var col = new Set({ value: "quick" },
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

    test('Set - reduce method - v1', function (assert) {
        var col = new Set({ value: "quick" },
                          { value: "brown" },
                          { value: "fox" },
                          { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            });
        assert.strictEqual(result, 16);
    });

    test('Set - reduce method - v1.1', function (assert) {
        var col = new Set({ value: "quick" },
                          { value: "brown" },
                          { value: "fox" },
                          { value: "dog" }),
            result = col.reduce(function (memo) {
                return (memo || 0) + this.value.length;
            }, 10);
        assert.strictEqual(result, 26);
    });

    test('Set - reduce method - v2', function (assert) {
        var col = new Set([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            });
        assert.strictEqual(result.toString(), [1,2,5,6,3,4].toString());
    });

    test('Set - reduce method - v2.1', function (assert) {
        var col = new Set([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, true);
        assert.strictEqual(result.toString(), [3,4,5,6,1,2].toString());
    });

    test('Set - reduce method - v2.2', function (assert) {
        var col = new Set([1, 2], [5,  6], [3, 4]),
            result = col.reduce(function (memo) {
                if (!Array.isArray(memo)) {
                    return this;
                }
                return memo.concat(this);
            }, [0, 0], true);
        assert.strictEqual(result.toString(), [0,0,3,4,5,6,1,2].toString());
    });

    test('Set - remove method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog", "quick");
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

    test('Set - size method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog", "quick");
        assert.strictEqual(col.size(), col.length);
    });

    test('Set - size method - v2', function (assert) {
        var col = new Set(["quick", "brown", "fox", "dog", "quick"]);
        assert.strictEqual(col.size(), col.length);
    });

    test('Set - size method - v3', function (assert) {
        var col = new Set(new Set("quick", "brown", "fox", "dog", "quick"));
        assert.strictEqual(col.size(), col.length);
    });

    test('Set - size method - v4', function (assert) {
        var col = new Set();
        assert.strictEqual(col.size(), col.length);
    });

    test('Set - union method - v1', function (assert) {
        var set1 = new Set("quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.union(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 6);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
        assert.ok(targetSet.contains("dog"));
        assert.ok(targetSet.contains("DOG"));
        assert.ok(targetSet.contains("Sled"));
    });

    test('Set - union method - v1.1', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = ["quick", "DOG", "Sled"],
            targetSet = set1.union(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 5);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
        assert.ok(targetSet.contains("dog"));
        assert.ok(targetSet.contains("sled"));
    });

    test('Set - toArray method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            arr = col.toArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.ok(col.contains(arr[i]));
        }
    });

    test('Set - toSortedArray method - v1', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            sortedArr = ["brown", "dog", "fox", "quick"],
            arr = col.toSortedArray(),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Set - toSortedArray method - v2', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            sortedArr = ["quick", "fox", "dog", "brown"],
            arr = col.toSortedArray(Comparer.reverse),
            i;
        assert.ok(Array.isArray(arr));
        assert.strictEqual(arr.length, col.length);
        for (i = 0; i < col.length; i++) {
            assert.strictEqual(arr[i], sortedArr[i]);
        }
    });

    test('Set - toJSON method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
            newCol,
            str = col.toJSON(),
            isJSON = true,
            i;
        try {
            var data = JSON.parse(str);
            assert.ok(Array.isArray(data));
            newCol = Set.parse(data);
            assert.ok(newCol.length, col.length);
            for (i = 0; i < col.length; i++) {
                assert.ok(newCol.contains(data[i]));
            }
        } catch (e) {
            isJSON = false;
        }
        assert.strictEqual(isJSON, true);
    });

    test('Set - toString method', function (assert) {
        var col = new Set("quick", "brown", "fox", "dog"),
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

    test('Set - union method - v1.2', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new Set("quick", "DOG", "Sled"),
            targetSet = set1.union(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 5);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
        assert.ok(targetSet.contains("dog"));
        assert.ok(targetSet.contains("sled"));
    });

    test('Set - union method - v1.3', function (assert) {
        var set1 = new Set(EqualityComparer.caseInsensitive, "quick", "brown", "fox", "dog"),
            set2 = new List("quick", "DOG", "Sled"),
            targetSet = set1.union(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 5);
        assert.ok(targetSet.contains("quick"));
        assert.ok(targetSet.contains("brown"));
        assert.ok(targetSet.contains("fox"));
        assert.ok(targetSet.contains("dog"));
        assert.ok(targetSet.contains("sled"));
    });

    test('Set - union method - v2', function (assert) {
        var obj1 = {},
            obj2 = {},
            obj3 = {},
            set1 = new Set(obj1, obj2),
            set2 = new Set([obj2, obj3]),
            targetSet = set1.union(set2);
        assert.ok(targetSet);
        assert.strictEqual(targetSet.length, 3);
        assert.ok(targetSet.contains(obj1));
        assert.ok(targetSet.contains(obj2));
        assert.ok(targetSet.contains(obj3));
    });
}());
