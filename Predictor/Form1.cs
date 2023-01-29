using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Web.Script.Serialization;

namespace Predictor
{
    public partial class Form1 : Form
    {
        private const string APP_NAME = "ULTIMATE_PREDICTOR";
        private readonly string PREDICTIONS_CONFIG_PATH = $"{Environment.CurrentDirectory}\\predictionsConfig.json";
        private string[] _predictions;
        private Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private async void bPredict_Click(object sender, EventArgs e) // делаем асинхронным
        {
            bPredict.Enabled = false; // отключаем кнопку т.к. async await позволяет запустить много потоков
            await Task.Run(() => // запуск отдельного потока избавляемся от фризов await для ожидания основным потоком
            {
                for (int i = 1; i <= 100; i++)
                {
                    this.Invoke(new Action(() => // при отрисовке из параллельного потока формы нужно обратиться к ней
                    {
                        UpdateProgressBar(i);
                        this.Text = $"{i}%";
                    }));
                    Thread.Sleep(20);
                }

            });
            // predictionsConfig в свойствах выбираем Copy to Output Directory
            // выведется после завершения прогресс бара, без async await будет выводиться сразу
            int index = _random.Next(_predictions.Length);
            var prediction = _predictions[index];
            MessageBox.Show($"{prediction}!");


            bPredict.Enabled = true;
            progressBar1.Value = 0;
            this.Text = APP_NAME;

        }

        private void UpdateProgressBar(int i)
        {
            if (i == progressBar1.Maximum) // делаем полную отрисовку прогресс бара перед окончанием работы
            {
                progressBar1.Maximum = i + 1;
                progressBar1.Value = i + 1;
                progressBar1.Maximum = i;
            }
            else
            {
                progressBar1.Value = i + 1;
            }
            progressBar1.Value = i;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = APP_NAME;

            try
            {
                // для десериализации JSON можно установить NuGet Newtonsoft.Json, ПКМ в explorer - Manage NuGet - json net
                var data = File.ReadAllText(PREDICTIONS_CONFIG_PATH); // если ру символы не читаются, меняем кодировку файла на UTF-8

                _predictions = new JavaScriptSerializer().Deserialize<string[]>(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_predictions == null)
                {
                    Close();
                }
                else if (_predictions.Length == 0)
                {
                    MessageBox.Show("В файле конфигурации нет подходящих данных.");
                    Close();
                }
            }
        }
    }
}
