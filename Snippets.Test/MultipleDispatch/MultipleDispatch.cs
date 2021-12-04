using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Snippets.Test.MultipleDispatch
{
    interface IThing { }
    internal class SmallThing : IThing { }
    internal class MediumThing : IThing { }
    internal class BigThing : IThing { }

    internal class MultiDispatchComparer : IComparer<IThing>
    {
        private const int Less = -1;
        private const int Same = 0;
        private const int Greater = 1;
        private const int Unmatched = 2;

        public int Compare(IThing a, IThing b)
        {
            return CompareMulti(a, b, 0);
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
        private static int CompareMulti(SmallThing a, IThing b) => Less;
        private static int CompareMulti(MediumThing a, BigThing b) => Less;
    }

    internal class SingleDispatchComparer : IComparer<IThing>
    {
        private const int Less = -1;
        private const int Same = 0;
        private const int Greater = 1;

        public int Compare(IThing x, IThing y)
        {
            return x switch
            {
                SmallThing => y switch
                {
                    SmallThing => Same,
                    _ => Less
                },
                MediumThing => y switch
                {
                    SmallThing => Greater,
                    BigThing => Less,
                    _ => Same
                },
                BigThing => y switch
                {
                    BigThing => Same,
                    _ => Greater,
                },
            };
        }

        // can't do this, as it 'steals' matches from more specific types
        // private static int CompareSingle<T>(T a, T b) where T : IThing => Same;
    }

    [TestFixture("single")]
    [TestFixture("multi")]
    internal class ComparerTests
    {
        private readonly IComparer<IThing> _comparer;

        public ComparerTests(string comparerType)
        {
            _comparer = comparerType switch
            {
                "single" => new SingleDispatchComparer(),
                "multi" => new MultiDispatchComparer(),
                _ => throw new ArgumentException()
            };
        }

        [Test]
        public void Same_things_are_equal()
        {
            Assert.That(new SmallThing(), Is.EqualTo(new SmallThing()).Using(_comparer));
            Assert.That(new MediumThing(), Is.EqualTo(new MediumThing()).Using(_comparer));
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

        [Test]
        public void Small_is_less_than_medium()
        {
            Assert.That(new SmallThing(), Is.LessThan(new MediumThing()).Using(_comparer));
        }
    }
}
