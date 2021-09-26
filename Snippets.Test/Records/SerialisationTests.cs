using Newtonsoft.Json;
using NUnit.Framework;

namespace Snippets.Test.Records
{
    public record MyRecord(string Name);

    class Simple_record
    {
        [Test]
        public void Has_nice_default_ToString()
        {
            var record = new MyRecord("my name");

            Assert.AreEqual("MyRecord { Name = my name }", record.ToString());
        }

        [Test]
        public void Serialises_to_json_as_expected()
        {
            var record = new MyRecord("my name");

            Assert.AreEqual("{\"Name\":\"my name\"}", JsonConvert.SerializeObject(record));
        }

        [Test]
        public void Deserialises_to_equal_record()
        {
            var record = new MyRecord("my name");
            var serialised = JsonConvert.SerializeObject(record);
            var deserialised = JsonConvert.DeserializeObject<MyRecord>(serialised);

            Assert.AreNotSame(record, deserialised);
            Assert.AreEqual(record, deserialised);
        }
    }

    /// <summary>
    /// A read-only record that provides "correct by construction" guarantee.
    /// Drawback: cannot be deserialised!
    /// </summary>
    public record MyEncapsulatedRecord
    {
        public string Name { get; }

        private MyEncapsulatedRecord(string name)
        {
            Name = name;
        }

        public static MyEncapsulatedRecord Create(string name)
        {
            return new MyEncapsulatedRecord(name);
        }
    }

    class Encapsulated_record
    {
        [Test]
        public void Has_nice_default_ToString()
        {
            var record = MyEncapsulatedRecord.Create("my name");

            Assert.AreEqual("MyEncapsulatedRecord { Name = my name }", record.ToString());
        }

        [Test]
        public void Serialises_to_json_as_expected()
        {
            var record = MyEncapsulatedRecord.Create("my name");

            Assert.AreEqual("{\"Name\":\"my name\"}", JsonConvert.SerializeObject(record));
        }

        [Test]
        public void Cannot_deserialise_if_no_public_constructor()
        {
            var record = MyEncapsulatedRecord.Create("my name");
            var serialised = JsonConvert.SerializeObject(record);

            Assert.Throws<JsonSerializationException>(() =>
                JsonConvert.DeserializeObject<MyEncapsulatedRecord>(serialised));
        }
    }
}
