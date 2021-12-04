using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Snippets.Test.MultipleDispatch
{
    interface ICommand
    {
    }

    internal class SmallCommand : ICommand
    {
    }

    internal class BigCommand : ICommand
    {
    }

    internal class MultiDispatchCommandComparer : IComparer<ICommand>
    {
        private const int Less = -1;
        private const int Same = 0;
        private const int Greater = 1;
        private const int Unmatched = 2;

        public int Compare(ICommand x, ICommand y)
        {
            return CompareMulti(x, y, 0);
        }

        private static int CompareMulti(ICommand a, ICommand b, int _ = 0)
        {
            var result = CompareMulti((dynamic)a, (dynamic)b);
            if (result != Unmatched) return result;
            result = CompareMulti((dynamic)b, (dynamic)a);
            if (result != Unmatched) return result;

            throw new ArgumentException("Undefined comparison");
        }

        private static int CompareMulti(ICommand a, ICommand b) => Unmatched;
        private static int CompareMulti<T>(T a, T b) where T : ICommand => Same;
        private static int CompareMulti(BigCommand a, SmallCommand b) => Greater;
        
    }

    internal class ComparerTests
    {
        private MultiDispatchCommandComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _comparer = new MultiDispatchCommandComparer();
        }

        [Test]
        public void Same_commands_are_equal()
        {
            Assert.That(new SmallCommand(), Is.EqualTo(new SmallCommand()).Using(_comparer));
            Assert.That(new BigCommand(), Is.EqualTo(new BigCommand()).Using(_comparer));
        }

        [Test]
        public void Asdf_is_greater_than_stuff()
        {
            Assert.That(new BigCommand(), Is.GreaterThan(new SmallCommand()).Using(_comparer));
        }

        [Test]
        public void Stuff_is_less_than_asdf()
        {
            Assert.That(new SmallCommand(), Is.GreaterThan(new BigCommand()).Using(_comparer));
        }
    }
}
