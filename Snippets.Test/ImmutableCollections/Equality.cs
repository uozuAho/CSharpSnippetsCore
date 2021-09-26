using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace Snippets.Test.ImmutableCollections
{
    class Equality
    {
        [Test]
        public void Adding_to_list_creates_new_list()
        {
            var list = ImmutableList.Create(1, 2, 3);
            var updatedList = list.Add(4);

            Assert.AreNotSame(list, updatedList);
        }

        [Test]
        public void Copied_list_is_not_same()
        {
            var list = ImmutableList.Create(1, 2, 3);
            var copy = ImmutableList.Create(list.ToArray());

            Assert.AreNotSame(list, copy);
        }

        [Test]
        public void Copied_list_is_equal()
        {
            var list = ImmutableList.Create(1, 2, 3);
            var copy = ImmutableList.Create(list.ToArray());

            Assert.AreEqual(list, copy);
        }

        [Test]
        public void Copied_list_reference_type_items_are_not_cloned()
        {
            var myRef = new ExampleReferenceType();
            var list = ImmutableList.Create(myRef);
            var copy = ImmutableList.Create(list.ToArray());

            Assert.AreEqual(list, copy);
            Assert.AreSame(list[0], copy[0]);
        }

        [Test]
        public void Copied_list_value_type_items_are_cloned()
        {
            var myRef = new ExampleValueType();
            var list = ImmutableList.Create(myRef);
            var copy = ImmutableList.Create(list.ToArray());

            Assert.AreEqual(list, copy);
            Assert.AreNotSame(list[0], copy[0]);
        }

        [Test]
        public void Copied_list_record_type_items_are_not_cloned()
        {
            var myRef = new ExampleRecordType();
            var list = ImmutableList.Create(myRef);
            var copy = ImmutableList.Create(list.ToArray());

            Assert.AreEqual(list, copy);
            Assert.AreSame(list[0], copy[0]);
        }
    }

    struct ExampleValueType
    {
    }

    class ExampleReferenceType
    {
    }

    record ExampleRecordType
    {
    }
}
