using System;
using FakeItEasy;

namespace CSharpSnippetsCore.Testing.FakeItEasy
{
    public interface IThingDoer
    {
        void DoThing();
        void PrintThis(string message);
    }

    class ThingDoer : IThingDoer
    {
        public void DoThing()
        {
            Console.WriteLine("do thing");
        }

        public void PrintThis(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class Wrapping
    {
        public static void WrappingExamples()
        {
            // SimpleWrapping();
            WrappingWithArgs();
        }

        public static void SimpleWrapping()
        {
            var doer = new ThingDoer();
            var fakeDoer = A.Fake<IThingDoer>(o => o.Wrapping(doer));
            Console.WriteLine("Fake wrapping instance does stuff:");
            fakeDoer.DoThing();
            
            // now override DoThing behaviour
            A.CallTo(() => fakeDoer.DoThing()).Invokes(() => 
            {
                Console.WriteLine("Call to fake method can do stuff, then call the instance method");
                doer.DoThing();
                Console.WriteLine("Then do other stuff");
            });
            fakeDoer.DoThing();
        }

        public static void WrappingWithArgs()
        {
            var doer = new ThingDoer();
            var fakeDoer = A.Fake<IThingDoer>(o => o.Wrapping(doer));
            
            // now override.PrintThis behaviour
            A.CallTo(() => fakeDoer.PrintThis(null)).WithAnyArguments().Invokes((call) => 
            {
                Console.WriteLine("blah1");
                doer.PrintThis((string)call.Arguments[0]);
                Console.WriteLine("blah2");
            });
            fakeDoer.PrintThis("poopoo");
        }
    }
}