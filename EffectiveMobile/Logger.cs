
namespace EffectiveMobile
{
    public class Logger : ILogger
    {
        private string _filePath;
        public Logger(string filePath) 
        {
            _filePath = filePath;
        }
        
        public void Log(string message)
        {
            using (StreamWriter streamWriter = File.AppendText(_filePath))
            {
                streamWriter.WriteLine(message);
            }
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }
}
