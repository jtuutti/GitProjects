/* global console, test, ok, EqualityComparer, Bag, Collection, Dictionary, List, LinkedList, Queue, Set, Stack, Tree */
(function (undefined) {
    'use strict';

    test('Equality Comparer is defined', function (assert) {
        assert.ok(EqualityComparer !== undefined && EqualityComparer !== null);
    });

    test('Base Collection is defined', function (assert) {
        assert.ok(Collection !== undefined && Collection !== null);
    });

    test('Bag is defined', function (assert) {
        assert.ok(Bag !== undefined && Bag !== null);

        var col = new Bag();
        assert.ok(col instanceof Collection && col instanceof Bag);
    });

    test('Dictionary is defined', function (assert) {
        assert.ok(Dictionary !== undefined && Dictionary !== null);

        var col = new Dictionary();
        assert.ok(col instanceof Collection && col instanceof Dictionary);
    });

    test('List is defined', function (assert) {
        assert.ok(List !== undefined && List !== null);

        var col = new List();
        assert.ok(col instanceof Collection && col instanceof List);
    });

    test('LinkedList is defined', function (assert) {
        assert.ok(LinkedList !== undefined && LinkedList !== null);

        var col = new LinkedList();
        assert.ok(col instanceof Collection && col instanceof LinkedList);
    });

    test('Queue is defined', function (assert) {
        assert.ok(Queue !== undefined && Queue !== null);

        var col = new Queue();
        assert.ok(col instanceof Collection && col instanceof Queue);
    });

    test('Set is defined', function (assert) {
        assert.ok(Set !== undefined && Set !== null);

        var col = new Set();
        assert.ok(col instanceof Collection && col instanceof Set);
    });

    test('Stack Collection is defined', function (assert) {
        assert.ok(Stack !== undefined && Stack !== null);

        var col = new Stack();
        assert.ok(col instanceof Collection && col instanceof Stack);
    });

    test('Tree Collection is defined', function (assert) {
        assert.ok(Tree !== undefined && Tree !== null);

        var col = new Tree();
        assert.ok(col instanceof Collection && col instanceof Tree);
    });
}());
