using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpFuzzer.Gui.Helpers
{
    //Log selected responses to files
    public class ResponseLogger : INotifyPropertyChanged, IDisposable
    {
        private const int _filesCount = 5;
        private readonly bool[] _logFlags = new bool[_filesCount];
        private StreamWriter[] _writers = new StreamWriter[_filesCount];

        #region LogFlags
        public bool Log1XX
        {
            get { return _logFlags[0]; }
            set
            {
                _logFlags[0] = value;
                OnPropertyChanged();
            }
        }

        public bool Log2XX
        {
            get { return _logFlags[1]; }
            set
            {
                _logFlags[1] = value;
                OnPropertyChanged();
            }
        }

        public bool Log3XX
        {
            get { return _logFlags[2]; }
            set
            {
                _logFlags[2] = value;
                OnPropertyChanged();
            }
        }

        public bool Log4XX
        {
            get { return _logFlags[3]; }
            set
            {
                _logFlags[3] = value;
                OnPropertyChanged();
            }
        }

        public bool Log5XX
        {
            get { return _logFlags[4]; }
            set
            {
                _logFlags[4] = value;
                OnPropertyChanged();
            }
        }

        #endregion LogFlags

        public ResponseLogger(string logDirectory = "log")
        {
            var fullDirectory = Environment.CurrentDirectory + "\\" + logDirectory;
            Directory.CreateDirectory(fullDirectory);
            for (var i = 0; i < _filesCount; i++)
            {
                var filePath = fullDirectory + "\\" + (i + 1) + "xx.log";
                _writers[i] = new StreamWriter(filePath, true);
            }
            Log1XX = true;
            Log2XX = true;
            Log3XX = true;
            Log4XX = true;
            Log5XX = true;
        }

        //Log message to target file
        public async Task Log(HttpResponseMessage message)
        {
            if ((int)message.StatusCode >= 100 && (int)message.StatusCode < 200 && Log1XX)
            {
                await _writers[0].WriteAsync(FormatMessage(message));
            }
            else if ((int)message.StatusCode >= 200 && (int)message.StatusCode < 300 && Log2XX)
            {
                await _writers[1].WriteAsync(FormatMessage(message));
            }
            else if ((int)message.StatusCode >= 300 && (int)message.StatusCode < 400 && Log3XX)
            {
                await _writers[2].WriteAsync(FormatMessage(message));
            }
            else if ((int)message.StatusCode >= 400 && (int)message.StatusCode < 500 && Log4XX)
            {
                await _writers[3].WriteAsync(FormatMessage(message));
            }
            else if ((int)message.StatusCode >= 500 && (int)message.StatusCode < 600 && Log5XX)
            {
                await _writers[4].WriteAsync(FormatMessage(message));
            }
        }

        public async Task Flush()
        {
            foreach (var streamWriter in _writers)
            {
                await streamWriter.FlushAsync();
            }
        }

        //Create custom format message
        private string FormatMessage(HttpResponseMessage message)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Status code: " + (int) message.StatusCode);
            builder.AppendLine("Method: " + message.RequestMessage.Method);
            builder.AppendLine("URL: " + message.RequestMessage.RequestUri);
            builder.AppendLine();
            return builder.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            foreach (var streamWriter in _writers)
            {
                streamWriter.Close();
            }
        }
    }
}
