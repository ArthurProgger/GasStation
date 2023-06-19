using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GasStation
{
    public partial class App
    {
        // анимация открытия вкладки (надо допилить)
        private static Grid _lastTabGrid;
        public static async Task OpenTab(Grid currentTabGrid, TabControl container)
        {
            double h = 15;
            container.IsEnabled = false;

            if (_lastTabGrid != null)
            {
                _lastTabGrid.VerticalAlignment = VerticalAlignment.Top;

                while (_lastTabGrid.Height > h)
                {
                    await Task.Run(() => Thread.Sleep(DelayAnimation));
                    _lastTabGrid.Height -= h;
                }

                _lastTabGrid.Visibility = Visibility.Hidden;
            }

            currentTabGrid.Visibility = Visibility.Visible;
            currentTabGrid.Height = 0;

            while (currentTabGrid.ActualHeight < container.ActualHeight - h)
            {
                await Task.Run(() => Thread.Sleep(DelayAnimation));
                currentTabGrid.Height += h;
            }

            _lastTabGrid = currentTabGrid;
            container.IsEnabled = true;
        }

        // анимация выбора пункта меню
        private static FrameworkElement _lastFunctionContent;
        public static FrameworkElement LastFunctionContent => _lastFunctionContent;
        public static Grid Content { get; set; }
        public static async Task OpenFunction<T>(T currentFunction, ListView menuList, double toWidth, Delegate method = null, params object[] args) where T : FrameworkElement
        {
            double w = 15;

            menuList.IsEnabled = false;
            Content.IsEnabled = false;

            if (_lastFunctionContent != null)
            {
                w = _lastFunctionContent.Name == "tablePage" || _lastFunctionContent.Name == "stuffers" || _lastFunctionContent.Name == "tanksInfo" ? 50 : 15;

                _lastFunctionContent.HorizontalAlignment = HorizontalAlignment.Left;
                _lastFunctionContent.Width = toWidth - w;
                
                while (_lastFunctionContent.Width > w)
                {
                    await Task.Run(() => Thread.Sleep(DelayAnimation));
                    _lastFunctionContent.Width -= w;
                }

                _lastFunctionContent.Visibility = Visibility.Hidden;
            }

            if (method != null)
                method.DynamicInvoke(args);

            currentFunction.Visibility = Visibility.Visible;

            while (currentFunction.ActualWidth < toWidth - w)
            {
                await Task.Run(() => Thread.Sleep(DelayAnimation));
                currentFunction.Width += w;
            }

            currentFunction.HorizontalAlignment = HorizontalAlignment.Stretch;
            currentFunction.Width = double.NaN;
            _lastFunctionContent = currentFunction;
            _pagesHistory.Add(_lastFunctionContent);

            menuList.IsEnabled = true;
            Content.IsEnabled = true;
        }

        // открытие страницы (закрытие остальных)
        // Объект Grid является страницой, если свойство Name содержит строку "Page"
        public static void OpenPage(Grid context, Grid page) => context.Children.Cast<UIElement>().Where(el => el is Grid).ToList().ForEach((UIElement el) => el.Visibility = el.Equals(page) ? Visibility.Visible : Visibility.Hidden);

        //выделение пункта меню при выборе
        public static void ItemSelectEffect(ListView context)
        {
            byte r = ((SolidColorBrush)context.Background).Color.R,
                 g = ((SolidColorBrush)context.Background).Color.G,
                 b = ((SolidColorBrush)context.Background).Color.B;

            ((ListViewItem)context.SelectedItem).Background = new SolidColorBrush(Color.FromRgb(r -= 30, g -= 30, b -= 30));
            InverseForeground((ListViewItem)context.SelectedItem);
        }

        // инвертирование цвета текста
        public static void InverseForeground(Control element)
        {
            byte r = (byte)~((SolidColorBrush)element.Background).Color.R,
                 g = (byte)~((SolidColorBrush)element.Background).Color.G,
                 b = (byte)~((SolidColorBrush)element.Background).Color.B;

            element.Foreground = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        // сворачивание элементов для CheckBox
        public static void CBFolding(Grid content, double h, double hp, Visibility v)
        {
            if (content != null)
            {
                ((Grid)content.Parent).Height = hp;
                content.Visibility = v;
                content.Height = h;
            }
        }

        //Поиск компаний в базе данных ФНС

        private readonly static List<FTSCompanyData> _companies = new List<FTSCompanyData>();
        public static List<FTSCompanyData> Companies => _companies;
        public static void SearchCompanies(ComboBox cb, string q, string key)
        {
            cb.Items.Clear();
            _companies.Clear();

            string httpQuery = $"https://api-fns.ru/api/search?q={q}&filter=active&key={key}";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(httpQuery);

                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //ответ от сервера
                    string result = streamReader.ReadToEnd();
                    JContainer data = (JContainer)JsonConvert.DeserializeObject(result);

                    data["items"].Cast<JObject>().ToList().ForEach((JObject j) =>
                    {
                        if (j.ContainsKey("ЮЛ"))
                            _companies.Add(j["ЮЛ"].ToObject<FTSCompanyData>());
                        else if (j.ContainsKey("ИП"))
                            _companies.Add(j["ИП"].ToObject<FTSCompanyData>());
                        else
                            _companies.Add(j["НР"].ToObject<FTSCompanyData>());
                    });
                }

                _companies.ForEach((FTSCompanyData comp) => cb.Items.Add(comp.НаимПолнЮЛ));
            }
            catch
            {
                cb.Items.Add("Ошибка доступа к базе ФНС");
            }
        }
    }
}