using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using HttpFuzzer.Gui.TestData;

namespace HttpFuzzer.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TestEngine Engine { get; private set; }
        private BaseUrl url;
        private long requestsCount;
        private readonly List<ParameterData> parameters = new List<ParameterData>();
        private RequestType type;
        private CancellationTokenSource cts;

        public MainWindow()
        {
            DataContext = this;
            Engine = new TestEngine();
            InitializeComponent();
            ParamsGrid.ItemsSource = parameters;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<BaseParameter> httpParameters;
            try
            {
                ParseSettings();
                httpParameters = GenerateHttpParameters();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return;
            } 
            
            cts = new CancellationTokenSource();
            var token = cts.Token;
            await Engine.DoTest(url, type, httpParameters, requestsCount, token).ConfigureAwait(false);
        }

        private void ParseSettings()
        {
             requestsCount = Int64.Parse(CounTextBox.Text);

            if (RegExUrlBox.Text == String.Empty)
            {
                url = new StaticUrl(StaticUrlBox.Text);
            }
            else
            {
                url = new RegexUrl(StaticUrlBox.Text, RegExUrlBox.Text);
            }
            if (UserAgentBox.Text != String.Empty)
            {
                TestEngine.UserAgent = UserAgentBox.Text;
            }
            type = RequestTypeBox.SelectedIndex == 0 ? RequestType.Get : RequestType.Post;
        }

        private IEnumerable<BaseParameter> GenerateHttpParameters()
        {
            var result = new List<BaseParameter>();
            foreach (var parameterData in parameters)
            {
                if (parameterData.IsRegex)
                {
                    result.Add(new RegexParameter(parameterData.Name, parameterData.Value));
                }
                else
                {
                    result.Add(new StaticParameter(parameterData.Name, parameterData.Value));
                }
            }
            return result;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (cts == null) return;
            cts.Cancel();
            cts = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopButton_Click(null, null);
            HttpExecutor.Stop();
            Engine.Dispose();
        }
    }
}
