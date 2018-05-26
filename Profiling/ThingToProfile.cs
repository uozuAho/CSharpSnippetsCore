using System;
using System.Data.SqlClient;
using System.Threading;

namespace CSharpSnippetsCore.Profiling
{
    class ThingToProfile
    {
        public static void Run()
        {
            Console.WriteLine("Profile this");
            KeepCpuBusyForSeconds(2);
            AllocateMb(50);
            StayIdleForSeconds(2);
            // todo: disk io
            // todo: network io
        }

        private static void KeepCpuBusyForSeconds(int seconds)
        {
            Console.WriteLine("KeepCpuBusyForSeconds: " + seconds);
            var random = new Random();
            var end = DateTime.Now + TimeSpan.FromSeconds(seconds);
            while (DateTime.Now < end);
        }

        private static void StayIdleForSeconds(int seconds)
        {
            Console.WriteLine("StayIdleForSeconds: " + seconds);
            Thread.Sleep(seconds * 1000);
        }

        private static void AllocateMb(int mb)
        {
            Console.WriteLine("AllocateMb: " + mb);
            var asdf = new int[mb * 1000000];
        }
    }
}
