using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Ponder.Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            PonderEngine engine = null;
            try
            {
                engine = new PonderEngine();
                while (true)
                {
                    string cmd;
                    if ((cmd = Console.ReadLine()) != null)
                    {
                        if (EngineStatus.Quit == engine.ExecuteCommand(cmd))
                            break;
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (System.Exception ex)
            {
                engine.Output.WriteLine("info Exception: " + ex);
                System.Console.WriteLine(ex);
            }
           



        }
       
    }
}

