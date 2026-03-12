using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OhRudi
{
    class Spinner : IDisposable
    {
        private readonly char[] sequence = new[] { '|', '/', '-', '\\' };
        private int counter = 0;
        private bool active;
        private Thread thread;

        public void Start()
        {
            active = true;
            thread = new Thread(Spin);
            thread.Start();
        }

        private void Spin()
        {
            while (active)
            {
                Console.Write(sequence[counter % sequence.Length]);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                counter++;
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            active = false;
            thread.Join();
            Console.Write(" "); // clear spinner
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

        public void Dispose() => Stop();
    }
}
