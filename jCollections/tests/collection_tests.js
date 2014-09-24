/* global console, test, ok, Collection, List, Set */
(function (undefined) {
    'use strict';

    test('Collection - static avg method', function (assert) {
        var result = Collection.avg([10, 1.5, 11]);
        assert.strictEqual(result, 7.5);
    });

    test('Collection - static avg method - NaN', function (assert) {
        var result = Collection.avg(['a', 'b', 'c']);
        assert.ok(isNaN(result));
    });

    test('Collection - static concat method', function (assert) {
        var result = Collection.concat([10, 1.5, 11], new List('a', 'b', 'c'), new Set(true, false, false));
        assert.ok(Collection.equivalent(result, [10, 1.5, 11, 'a', 'b', 'c', true, false]));
    });

    test('Collection - static distinct method - v1', function (assert) {
        var result = Collection.distinct(['a', 'c', 'c', 'd', 'a', 'A']);
        assert.ok(Collection.equivalent(result, ['a', 'c', 'd', 'A']));
    });

    test('Collection - static distinct method - v2', function (assert) {
        var col = [{name:'a'}, {name:'c'}, {name:'c'}, {name:'d'}, {name:'A'}],
            equalityComparer = function (a, b) {
                return a.name.toUpperCase() === b.name.toUpperCase();
            },
            result = Collection.distinct(col, equalityComparer);
        assert.ok(Collection.equivalent(result, [{name:'a'}, {name:'c'}, {name:'d'}], equalityComparer));
    });

    test('Collection - static equivalent method - v1', function (assert) {
        var col = new List('a', 'c', 'd', 'A');
        assert.ok(Collection.equivalent(col, ['a', 'c', 'd', 'A']));
    });

    test('Collection - static equivalent method - v2', function (assert) {
        var col = new List(1, 2, 3),
            equalityComparer = function (a, b) {
                return a.toString() === b.toString();
            };
        assert.ok(Collection.equivalent(col, ["1", "2", "3"], equalityComparer));
        assert.ok(!Collection.equivalent(col, ["1", "2", "3"]));
    });

    test('Collection - static first method', function (assert) {
        var col = [2, 3, 4];
        assert.strictEqual(Collection.first(col), 2);
    });

    test('Collection - static first method - empty', function (assert) {
        var col = [];
        assert.strictEqual(Collection.first(col), undefined);
    });

    test('Collection - static flatten method - deep', function (assert) {
        var col = new List(2, [3, new Set([4, 5])]);
        assert.ok(Collection.equivalent(Collection.flatten(col, true), [2, 3, 4, 5]));
    });

    test('Collection - static flatten method - shallow', function (assert) {
        var innerCol = new Set([4, 5]),
            col = new List(2, [3, innerCol]);
        assert.ok(Collection.equivalent(Collection.flatten(col), [2, 3, innerCol]));
    });

    test('Collection - static get method - v1', function (assert) {
        var col = [2, 3, 4];
        assert.strictEqual(Collection.get(col, 0), 2);
        assert.strictEqual(Collection.get(col, 1), 3);
        assert.strictEqual(Collection.get(col, 2), 4);
        assert.strictEqual(Collection.get(col, 3), undefined);
    });

    test('Collection - static get method - v2', function (assert) {
        var col = new Tree(4, 2, 3);
        assert.strictEqual(Collection.get(col, 0), 2);
        assert.strictEqual(Collection.get(col, 1), 3);
        assert.strictEqual(Collection.get(col, 2), 4);
        assert.strictEqual(Collection.get(col, 3), undefined);
    });

    test('Collection - static last method', function (assert) {
        var col = [2, 3, 4];
        assert.strictEqual(Collection.last(col), 4);
    });

    test('Collection - static last method - empty', function (assert) {
        var col = [];
        assert.strictEqual(Collection.last(col), undefined);
    });

    test('Collection - static min method', function (assert) {
        var col = [4, 1, 1e20, -2];
        assert.strictEqual(Collection.min(col), -2);
    });

    test('Collection - static min method - NaN', function (assert) {
        var col = [4, 1, 1e20, -2, "abc"];
        assert.ok(isNaN(Collection.min(col)));
    });

    test('Collection - static max method', function (assert) {
        var col = [4, 1, 1e20, -2];
        assert.strictEqual(Collection.max(col), 1e20);
    });

    test('Collection - static max method - NaN', function (assert) {
        var col = [4, 1, 1e20, -2, "abc"];
        assert.ok(isNaN(Collection.max(col)));
    });

    test('Collection - static range method - v1', function (assert) {
        var col = [1, 2, 3, 4],
            range = Collection.range(col, 1, 3);
        assert.ok(Collection.equivalent(range, [2, 3]));
    });

    test('Collection - static range method - v2', function (assert) {
        var col = [1, 2, 3, 4],
            range = Collection.range(col, 0);
        assert.ok(Collection.equivalent(range, [1, 2, 3, 4]));
    });

    test('Collection - static range method - v3', function (assert) {
        var col = [1, 2, 3, 4],
            range = Collection.range(col, -1);
        assert.ok(Collection.equivalent(range, [4]));
    });

    test('Collection - static size method', function (assert) {
        assert.strictEqual(Collection.size([]), 0);
        assert.strictEqual(Collection.size([1]), 1);
        assert.strictEqual(Collection.size([2, 3, 4, 5, 6]), 5);
    });

    test('Collection - static sum method', function (assert) {
        var result = Collection.sum([10, 1.5, 11]);
        assert.strictEqual(result, 22.5);
    });

    test('Collection - static sum method - NaN', function (assert) {
        var result = Collection.sum(['a', 'b', 'c']);
        assert.ok(isNaN(result));
    });

    test('Collection - static toArray method - v1', function (assert) {
        var col = ['a', 'b', 'c'],
            result = Collection.toArray(col);
        assert.ok(Collection.equivalent(result, col));
    });

    test('Collection - static toArray method - v2', function (assert) {
        var col = new Set('a', 'b', 'c'),
            result = Collection.toArray(col);
        assert.ok(Collection.equivalent(result, col.toArray()));
    });

    test('Collection - static toArray method - v3', function (assert) {
        var col = true,
            result = Collection.toArray(col);
        assert.ok(Collection.equivalent(result, [col]));
    });

    test('Collection - static toSortedArray method - v1', function (assert) {
        var col = ['c', 'b', 'x', 'a', 'z', 'y'],
            sortedCol = ['a', 'b', 'c', 'x', 'y', 'z'],
            result = Collection.toSortedArray(col);
        assert.ok(Collection.equivalent(result, sortedCol));
    });

    test('Collection - static toSortedArray method - v2', function (assert) {
        var col = ['c', 'b', 'x', 'a', 'z', 'y'],
            sortedCol = ['z', 'y', 'x', 'c', 'b', 'a'],
            result = Collection.toSortedArray(col, Comparer.reverse);
        assert.ok(Collection.equivalent(result, sortedCol));
    });

    test('Collection - static zip method - v1', function (assert) {
        var col1 = ['Joe', 'Mike', 'Sally', 'Rick'],
            col2 = new List(30, 22.5, 44),
            result = Collection.zip(col1, col2);
        assert.strictEqual(result.length, 4);
        assert.strictEqual(result[0].length, 2);
        assert.strictEqual(result[0][0], 'Joe');
        assert.strictEqual(result[0][1], 30);
        assert.strictEqual(result[1].length, 2);
        assert.strictEqual(result[1][0], 'Mike');
        assert.strictEqual(result[1][1], 22.5);
        assert.strictEqual(result[2].length, 2);
        assert.strictEqual(result[2][0], 'Sally');
        assert.strictEqual(result[2][1], 44);
        assert.strictEqual(result[3].length, 2);
        assert.strictEqual(result[3][0], 'Rick');
        assert.strictEqual(result[3][1], null);
    });

    test('Collection - static zip method - v2', function (assert) {
        var col1 = ['Joe', 'Mike'],
            col2 = new List(30, 22.5, 44),
            selector = function (obj1, obj2) {
                return obj1 + ": " + obj2;
            },
            result = Collection.zip(col1, col2, selector);
        assert.strictEqual(result.length, 2);
        assert.strictEqual(result[0], selector('Joe', 30));
        assert.strictEqual(result[1], selector('Mike', 22.5));
    });
}());
