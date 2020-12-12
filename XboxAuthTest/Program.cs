using System;
using XboxAuthNet;

namespace XboxAuthTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("input xbox email : ");
            var email = Console.ReadLine();
            Console.WriteLine("input xbox password : ");
            var password = Console.ReadLine();

            var x = new XboxAuth();
            var r = x.Authenticate(email, password);
            Console.WriteLine("UserXUID {0}", r.UserXUID);
            Console.WriteLine("UserHash {0}", r.UserHash);
            Console.WriteLine("XSTSToken {0}", r.XSTSToken);
            Console.WriteLine("ExpireOn {0}", r.ExpireOn);

            Console.ReadLine();
        }
    }
}
