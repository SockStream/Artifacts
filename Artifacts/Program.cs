
using Artifacts.Bot;
using Artifacts.Status;
using Artifacts.Utilities;
using Artifacts.Utilities.ConsoleManager;

namespace Artifacts
{
    static class Program
    {

        static void  Main()
        {

            MCU mcu = new MCU();
            try
            {
                mcu.Init();
                Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    mcu.Stop = true;
                    ConsoleManager.Write("DEMANDE D'ARRET RECUE", ConsoleManager.errorConsoleColor);
                };
                mcu.Run();
            }
            catch (Exception e)
            {
                ConsoleManager.Write(e.Message, ConsoleManager.errorConsoleColor);
                ConsoleManager.Write(e.StackTrace, ConsoleManager.errorConsoleColor);
            }
            finally
            {
                mcu.endMCU();
            }
        }
    }
}
