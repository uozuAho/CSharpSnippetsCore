using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Snippets.Test.MultipleDispatch
{
    interface IThing
    {
    }

    internal class SmallThing : IThing
    {
    }

    internal class BigThing : IThing
    {
    }

    internal class MultiDispatchComparer : IComparer<IThing>
    {
        private const int Less = -1;
        private const int Same = 0;
        private const int Greater = 1;
        private const int Unmatched = 2;

        public int Compare(IThing x, IThing y)
        {
            return CompareMulti(x, y, 0);
        }

        private static int CompareMulti(IThing a, IThing b, int _)
        {
            var result = CompareMulti((dynamic)a, (dynamic)b);
            if (result != Unmatched) return result;
            result = CompareMulti((dynamic)b, (dynamic)a);
            if (result != Unmatched) return -result;

            throw new ArgumentException("Undefined comparison");
        }

        private static int CompareMulti(IThing a, IThing b) => Unmatched;
        private static int CompareMulti<T>(T a, T b) where T : IThing => Same;
        private static int CompareMulti(BigThing a, SmallThing b) => Greater;
        
    }

    internal class ComparerTests
    {
        private MultiDispatchComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _comparer = new MultiDispatchComparer();
        }

        [Test]
        public void Same_things_are_equal()
        {
            Assert.That(new SmallThing(), Is.EqualTo(new SmallThing()).Using(_comparer));
            Assert.That(new BigThing(), Is.EqualTo(new BigThing()).Using(_comparer));
        }

        [Test]
        public void Big_is_greater_than_small()
        {
            Assert.That(new BigThing(), Is.GreaterThan(new SmallThing()).Using(_comparer));
        }

        [Test]
        public void Small_is_less_than_big()
        {
            Assert.That(new SmallThing(), Is.LessThan(new BigThing()).Using(_comparer));
        }
    }
}
