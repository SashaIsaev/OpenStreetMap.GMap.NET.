using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;

// Библиотеки для карты
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace WindowsFormsApp1
{
    public partial class Marshruts : Form
    {
        DataTable dtRouter;
        string myAPI = "";

        public Marshruts(string API)
        {
            myAPI = API;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Marshruts_Load(object sender, EventArgs e)
        {
            // Настройки для компонента GMap
            gmap.Bearing = 0;
            // Перетаскивание левой кнопки мыши
            gmap.CanDragMap = true;
            // Перетаскивание карты левой кнопкой мыши
            gmap.DragButton = MouseButtons.Left;
            gmap.GrayScaleMode = true;
            // Все маркеры будут показаны
            gmap.MarkersEnabled = true;
            // Максимальное приближение
            gmap.MaxZoom = 18;
            // Минимальное приближение
            gmap.MinZoom = 2;
            // Курсор мыши в центр карты
            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            // Отключение нигативного режима
            gmap.NegativeMode = false;
            // Разрешение полигонов
            gmap.PolygonsEnabled = true;
            // Разрешение маршрутов
            gmap.RoutesEnabled = true;
            // Скрытие внешней сетки карты
            gmap.ShowTileGridLines = false;
            // При загрузке 10-кратное увеличение
            gmap.Zoom = 11;
            // Убрать красный крестик по центру
            gmap.ShowCenter = false;

            // Чья карта используется
            gmap.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(47.2229, 38.9095);


            // Настройки таблицы

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;




            // инициализируем новую таблицу для хранения данных о маршруте
            dtRouter = new DataTable();

            // Добавляем в таблицу колонки
            dtRouter.Columns.Add("Шаг");
            dtRouter.Columns.Add("Нач. точка (latitude)");
            dtRouter.Columns.Add("Нач. точка (longitude)");
            dtRouter.Columns.Add("Кон. точка (latitude)");
            dtRouter.Columns.Add("Кон. точка (longitude)");
            dtRouter.Columns.Add("Время пути");
            dtRouter.Columns.Add("Расстояние");
            dtRouter.Columns.Add("Описание маршрута");
           
            dataGridView1.DataSource = dtRouter;

            // Задаем ширину седьмого столбца
            dataGridView1.Columns[7].Width = 250;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;

            // Добавляем способы перемещения
            comboBox1.Items.Add("Автомобильные маршруты");
            comboBox1.Items.Add("Пешеходные маршруты");
            comboBox1.Items.Add("Велосипедные маршруты");
            comboBox1.Items.Add("Маршруты общественного транспорта");

            // Выставляем по умолчанию способ перемещения автомобильные маршруты по улично-дорожной сети
            comboBox1.SelectedIndex = 0;
        }


        // Слой для маркеров и маршрута
        GMapOverlay overlaymarshrut = new GMapOverlay("overlaymarshrut");
        // Построение маршрута
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Очищаем таблицу перед загрузкой данных
            dtRouter.Rows.Clear();

            //Создаем список способов перемещения
            List<string> mode = new List<string>();

            // Автомобильные маршруты по улично-дорожной сети
            mode.Add("driving");
            // Пешеходные маршруты по прогулочным дорожкам и тротуарам
            mode.Add("walking");
            // Велосипедные маршруты по велосипедным дорожкам и предпочитаемым улицам
            mode.Add("bicycling");
            // Маршруты общественного транспорта
            mode.Add("transit");

            string zapros = "https://maps.googleapis.com/maps/api/directions/xml?origin={0},&destination={1}&sensor=false&language=ru&mode={2}&key=" + myAPI;

            string url = string.Format(zapros, Uri.EscapeDataString(textBox1.Text), Uri.EscapeDataString(textBox2.Text), Uri.EscapeDataString(mode[comboBox1.SelectedIndex]));


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);     
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();



            // Парсинг данных


            XmlDocument xmldoc = new XmlDocument();

            xmldoc.LoadXml(responsereader);

            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList nodes = xmldoc.SelectNodes("//leg//step");

                // Формируем строку для добавления в таблицу
                object[] dr;
                for (int i = 0; i < nodes.Count; i++)
                {
                    // Указываем что массив будет состоять из восьми значений
                    dr = new object[8];

                    // Номер шага
                    dr[0] = i;

                    // Получение координат начала отрезка
                    dr[1] = xmldoc.SelectNodes("//start_location").Item(i).SelectNodes("lat").Item(0).InnerText.ToString();
                    dr[2] = xmldoc.SelectNodes("//start_location").Item(i).SelectNodes("lng").Item(0).InnerText.ToString();

                    // Получение координат конца отрезка
                    dr[3] = xmldoc.SelectNodes("//end_location").Item(i).SelectNodes("lat").Item(0).InnerText.ToString();
                    dr[4] = xmldoc.SelectNodes("//end_location").Item(i).SelectNodes("lng").Item(0).InnerText.ToString();

                    // Получение времени необходимого для прохождения этого отрезка
                    dr[5] = xmldoc.SelectNodes("//duration").Item(i).SelectNodes("text").Item(0).InnerText.ToString();

                    // Получение расстояния, охватываемое этим отрезком
                    dr[6] = xmldoc.SelectNodes("//distance").Item(i).SelectNodes("text").Item(0).InnerText.ToString();

                    // Получение инструкций для этого шага, представленные в виде текстовой строки HTML
                    dr[7] = HtmlToPlainText(xmldoc.SelectNodes("//html_instructions").Item(i).InnerText.ToString());

                    // Добавление шага в таблицу
                    dtRouter.Rows.Add(dr);
                }



                // Вывод данных


                // Выводим в текстовое поле адрес начала пути
                textBox1.Text = xmldoc.SelectNodes("//leg//start_address").Item(0).InnerText.ToString();

                // Выводим в текстовое поле адрес конца пути
                textBox2.Text = xmldoc.SelectNodes("//leg//end_address").Item(0).InnerText.ToString();

                // Выводим в текстовое поле время в пути
                textBox3.Text = xmldoc.GetElementsByTagName("duration")[nodes.Count].ChildNodes[1].InnerText;

                // Выводим в текстовое поле расстояние от начальной до конечной точки
                textBox4.Text = xmldoc.GetElementsByTagName("distance")[nodes.Count].ChildNodes[1].InnerText;

                // Получение координат начала пути
                double latStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[1].InnerText);

                // Получение координат конечной точки
                double latEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[1].InnerText);

                // Выводим в текстовое поле координаты начала пути
                textBox5.Text = latStart + ";" + lngStart;

                // Выводим в текстовое поле координаты конечной точки
                textBox6.Text = latEnd + ";" + lngEnd;

                // Устанавливаем заполненную таблицу в качестве источника
                dataGridView1.DataSource = dtRouter;




                // Маркеры и их подписи


                // Устанавливаем позицию карты на начало пути
                gmap.Position = new PointLatLng(latStart, lngStart);         
                gmap.Overlays.Add(overlaymarshrut);



                GMarkerGoogle markerG = new GMarkerGoogle(new PointLatLng(latStart, lngStart),GMarkerGoogleType.green);
                markerG.ToolTip = new GMapRoundedToolTip(markerG);
                markerG.ToolTipMode = MarkerTooltipMode.Always;

                // Формируем подсказку для маркера
                string[] wordsG = textBox1.Text.Split(',');
                string dataMarkerG = string.Empty;
                foreach (string word in wordsG)
                {
                    dataMarkerG += word + ";\n";
                }             
                markerG.ToolTipText = dataMarkerG;



                GMarkerGoogle markerR = new GMarkerGoogle(new PointLatLng(latEnd, lngEnd),GMarkerGoogleType.red);
                markerR.ToolTip = new GMapRoundedToolTip(markerG);
                markerR.ToolTipMode = MarkerTooltipMode.Always;

                // Формируем подсказку для маркера
                string[] wordsR = textBox2.Text.Split(',');
                string dataMarkerR = string.Empty;
                foreach (string word in wordsR)
                {
                    dataMarkerR += word + ";\n";
                }              
                markerR.ToolTipText = dataMarkerR;



                overlaymarshrut.Markers.Add(markerG);
                overlaymarshrut.Markers.Add(markerR);
                gmap.Overlays.Clear();




                // Маршрут


                // Создаем список контрольных точек для прокладки маршрута
                List<PointLatLng> list = new List<PointLatLng>();

                // Проходимся по определенным столбцам для получения координат контрольных точек маршрута и занесением их в список координат
                for (int i = 0; i < dtRouter.Rows.Count; i++)
                {
                    double dbStartLat = double.Parse(dtRouter.Rows[i].ItemArray[1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    double dbStartLng = double.Parse(dtRouter.Rows[i].ItemArray[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    list.Add(new PointLatLng(dbStartLat, dbStartLng));

                    double dbEndLat = double.Parse(dtRouter.Rows[i].ItemArray[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    double dbEndLng = double.Parse(dtRouter.Rows[i].ItemArray[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    list.Add(new PointLatLng(dbEndLat, dbEndLng));
                }

                // Очищаем все маршруты
                overlaymarshrut.Routes.Clear();

                // Создаем маршрут на основе списка контрольных точек
                GMapRoute r = new GMapRoute(list, "Route");

                // Указываем, что данный маршрут должен отображаться
                r.IsVisible = true;

                // Устанавливаем цвет маршрута
                r.Stroke.Color = Color.DarkGreen;

                // Добавляем маршрут
                overlaymarshrut.Routes.Add(r);

                // Добавляем в компонент, список маркеров и маршрутов
                gmap.Overlays.Add(overlaymarshrut);
            }
        }




        // Удаляем HTML теги
        public string HtmlToPlainText(string html)
        {
            html = html.Replace("</b>", "");
            return html.Replace("<b>", "");
        }
    }
}