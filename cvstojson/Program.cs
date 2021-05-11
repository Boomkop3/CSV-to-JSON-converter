using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Boomkop3;
using Boomkop3.Serializer_;

namespace cvstojson
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string inp = args[0];
                string outp = args[1];
                var input = new FileTextLines(inp);
                var output = File.CreateText(outp);
                output.WriteLine("[");
                Console.WriteLine("[");
                // output.WriteLine("[");
                bool first = true;
                foreach (string line in input)
                {
                    if (!first) {
                        output.Write(", ");
                        Console.Write(", ");
                    }
                    first = false;
                    string jsonLine = Serializer.Serialize.toJSON(line.Split(','));
                    output.WriteLine();
                    output.Write(jsonLine);
                    Console.WriteLine();
                    Console.Write(jsonLine);
                }
                output.Close();
                output = File.AppendText(outp);
                output.WriteLine();
                Console.WriteLine();
                output.WriteLine("]");
                output.Close();
                output = File.AppendText(outp);
                output.WriteLine("]");
                Console.WriteLine("]");
                input.keepAlive();
                GC.KeepAlive(output);
            } catch (Exception up)
            {
                Console.WriteLine(up);
                Console.ReadLine();
            }
            Console.WriteLine("done, press any key to continue");
            Console.ReadLine();
        }
    }

    class FileTextLines : IEnumerable<string>
    {
        Enumerator enumerator;
        public FileTextLines(string path)
        {
            enumerator = new Enumerator(path);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void keepAlive()
        {
            GC.KeepAlive(enumerator);
        }
        class Enumerator : IEnumerator<string>
        {
            public string _current;
            object IEnumerator.Current => _current;
            public string Current => _current;

            private StreamReader reader;
            private string path;

            public Enumerator(string path)
            {
                this.path = path;
                var stream = File.OpenRead(path);
                BufferedStream streamBuffer = new BufferedStream(stream, 1024 * 1024 * 1024);
                this.reader = new StreamReader(streamBuffer);
            }

            public void Dispose()
            {
                // this.reader.Close();
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public bool MoveNext()
            {
                if (reader.EndOfStream)
                {
                    return false;
                }
                _current = reader.ReadLine();
                return true;
            }

            public void Reset()
            {
                reader.Close();
                reader.Dispose();
                reader = File.OpenText(path);
            }

            public void keepAlive()
            {
                GC.KeepAlive(reader);
            }
        }
    }
}
