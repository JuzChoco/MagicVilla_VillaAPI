namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, String type)
        {
            if (type == "error")
            {
                Console.WriteLine("ERROR - " + message);
            }
            else {
                Console.WriteLine(message);
            }
        }
    }
}
