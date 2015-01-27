using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpFuzzer.Gui.Helpers;
using HttpFuzzer.Gui.TestData;

namespace HttpFuzzer.Gui
{
    //Class for Fuzzing testing
    public class TestEngine : INotifyPropertyChanged, IDisposable
    {
        private long _code1 = 0;
        private long _code2 = 0;
        private long _code3 = 0;
        private long _code4 = 0;
        private long _code5 = 0;
        private long _doneRequests = 0;
        private long _successRequests = 0;
        private long _faliedRequests = 0;
        private long _countRequests = 0;
        private bool _isRunning = false;
        private ResponseLogger _logger  = new ResponseLogger();

        //Counters for every type of response
        #region Counters

        public long Code1
        {
            get { return _code1; }
            private set
            {
                _code1 = value;
                OnPropertyChanged();
            }
        }

        public long Code2
        {
            get { return _code2; }
            private set
            {
                _code2 = value;
                OnPropertyChanged();
            }
        }

        public long Code3
        {
            get { return _code3; }
            private set
            {
                _code3 = value;
                OnPropertyChanged();
            }
        }

        public long Code4
        {
            get { return _code4; }
            private set
            {
                _code4 = value;
                OnPropertyChanged();
            }
        }

        public long Code5
        {
            get { return _code5; }
            private set
            {
                _code5 = value;
                OnPropertyChanged();
            }
        }

        public long DoneCount
        {
            get { return _doneRequests; }
            private set
            {
                _doneRequests = value;
                OnPropertyChanged();
            }
        }

        public long SuccessCount
        {
            get { return _successRequests; }
            private set
            {
                _successRequests = value;
                OnPropertyChanged();
            }
        }

        public long FaliedCount
        {
            get { return _faliedRequests; }
            private set
            {
                _faliedRequests = value;
                OnPropertyChanged();
            }
        }

        public long RequestsCount
        {
            get { return _countRequests; }
            private set
            {
                _countRequests = value;
                OnPropertyChanged();
            }
        }

        #endregion Counters

        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public static string UserAgent
        {
            get { return HttpExecutor.UserAgent; }
            set { HttpExecutor.UserAgent = value; }
        }

        public ResponseLogger Logger { get { return _logger; } }

        public event PropertyChangedEventHandler PropertyChanged;


        private void ResetCounters()
        {
            Code1 = 0;
            Code2 = 0;
            Code3 = 0;
            Code4 = 0;
            Code5 = 0;
            DoneCount = 0;
            SuccessCount = 0;
            FaliedCount = 0;
            RequestsCount = 0;
        }

        //Update GUI counters
        private void UpdateProperties()
        {
            OnPropertyChanged("Code1");
            OnPropertyChanged("Code2");
            OnPropertyChanged("Code3");
            OnPropertyChanged("Code4");
            OnPropertyChanged("Code5");
            OnPropertyChanged("DoneCount");
            OnPropertyChanged("SuccessCount");
            OnPropertyChanged("FaliedCount");
        }

        public async Task DoTest(BaseUrl url, RequestType type, IEnumerable<BaseParameter> parameters, long count, CancellationToken token)
        {
            ResetCounters();
            IsRunning = true;
            RequestsCount = count;
            var responseTasks = new ConfiguredTaskAwaitable<HttpResponseMessage>[count];
            //Send all requests
            for (var i = 0; i < RequestsCount; i++)
            {
                if (token.IsCancellationRequested)
                {
                    IsRunning = false;
                    UpdateProperties();
                    await Logger.Flush();
                    return;
                }
                      
                responseTasks[i] = Task.Run(() => HttpExecutor.Execute(url.Value, type, parameters, token), token).ConfigureAwait(false);                
            }

            //Get all responses
            for (var i = 0; i < RequestsCount; i++)
            {
                if (token.IsCancellationRequested)
                {
                    IsRunning = false;
                    UpdateProperties();
                    await Logger.Flush();
                    return;
                }
                     
                HttpResponseMessage response;
                try
                {
                    DoneCount++;
                    response = await responseTasks[i];
                }
                catch (Exception)
                {                    
                    FaliedCount++;
                    continue;
                }

                if ((int)response.StatusCode >= 100 && (int)response.StatusCode < 200)
                {
                    Code1++;
                }
                else if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    Code2++;
                }
                else if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                {
                    Code3++;
                }
                else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    Code4++;
                }
                else if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
                {
                    Code5++;
                }
                SuccessCount++;
                await Logger.Log(response);
            }
            await Logger.Flush();
            IsRunning = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Logger.Dispose();
        }
    }
}
