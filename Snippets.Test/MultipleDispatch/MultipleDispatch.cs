using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Snippets.Test.MultipleDispatch
{
    interface ICommand
    {
    }

    internal class StuffCommand : ICommand
    {
    }

    internal class AsdfCommand : ICommand
    {
    }

    internal class ActualCommandComparer
    {
        private const int Less = -1;
        private const int Same = 0;
        private const int Greater = 1;
        private const int Unmatched = 2;

        public static int Compare(ICommand a, ICommand b, int _ = 0)
        {
            var result = Compare((dynamic)a, (dynamic)b);
            if (result != Unmatched) return result;
            result = Compare((dynamic)b, (dynamic)a);
            if (result != Unmatched) return result;

            throw new ArgumentException("Undefined comparison");
        }

        private static int Compare(ICommand a, ICommand b) => Unmatched;
        private static int Compare<T>(T a, T b) where T : ICommand => Same;
        private static int Compare(AsdfCommand a, StuffCommand b) => Greater;
    }

    internal class CommandComparer : IComparer<ICommand>
    {
        public int Compare(ICommand x, ICommand y)
        {
            return ActualCommandComparer.Compare(x, y);
        }
    }

    internal class ComparerTests
    {
        private CommandComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _comparer = new CommandComparer();
        }

        [Test]
        public void Same_commands_are_equal()
        {
            Assert.That(new StuffCommand(), Is.EqualTo(new StuffCommand()).Using(_comparer));
            Assert.That(new AsdfCommand(), Is.EqualTo(new AsdfCommand()).Using(_comparer));
        }

        [Test]
        public void Asdf_is_greater_than_stuff()
        {
            Assert.That(new AsdfCommand(), Is.GreaterThan(new StuffCommand()).Using(_comparer));
        }

        [Test]
        public void Stuff_is_less_than_asdf()
        {
            Assert.That(new StuffCommand(), Is.GreaterThan(new AsdfCommand()).Using(_comparer));
        }
    }
}
