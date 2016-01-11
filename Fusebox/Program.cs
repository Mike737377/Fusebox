using ReliabilityPatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fusebox
{
    public class Program
    {
        private static readonly ExternalUnreliableService _service = new ExternalUnreliableService();
        private static readonly CircuitBreaker _breaker = new CircuitBreaker(1, TimeSpan.FromSeconds(5));

        public static void Main(string[] args)
        {
            _breaker.StateChanged += _breaker_StateChanged;

            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
            Run();
        }

        static void _breaker_StateChanged(object sender, EventArgs e)
        {
            Console.WriteLine("State changed to: {0}", _breaker.State);
        }

        private static void Run()
        {
            var failed = false;

            Console.Write("Running... ");

            try
            {
                _breaker.Execute(() => _service.DoSomething());
                Console.WriteLine("Service call worked");
            }
            catch (OpenCircuitException)
            {
                Console.WriteLine("Failing fast");
                failed = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                failed = true;
            }

            Console.WriteLine("Service level: {0}", _breaker.ServiceLevel);

            if (failed)
            {
                Console.WriteLine("");
                Console.WriteLine("Waiting 3 seconds...");
                Thread.Sleep(3000);
            }

            Console.WriteLine("");
        }
    }

    public class ExternalUnreliableService
    {

        private int _called = 0;

        internal void DoSomething()
        {
            _called++;

            Console.WriteLine("Service called {0} times", _called);

            if (_called % 5 == 0 || _called == 6 || _called == 3)
            {
                throw new Exception("Random failure");
            }
        }
    }
}
