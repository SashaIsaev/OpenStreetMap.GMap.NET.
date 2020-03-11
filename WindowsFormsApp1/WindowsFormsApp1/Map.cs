using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Device.Location;
using System.Net;

// Библиотеки для карты
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace WindowsFormsApp1
{
    public partial class Map : Form
    {
        // Класс точка - координаты
        public class CPoint
        {
            public double x { get; set; }
            public double y { get; set; }
            public string place { get; set; }
            public string building { get; set; }
            public string street { get; set; }
            public string town { get; set; }

            public string Image;

            public CPoint() { }
            public CPoint(double _x, double _y)
            {
                x = _x;
                y = _y;
            }

            public CPoint(double _x, double _y, string _place)
            {
                x = _x;
                y = _y;
                place = _place;
            }

            public CPoint(double _x, double _y, string _building, string _street, string _town)
            {
                x = _x;
                y = _y;
                building = _building;
                street = _street;
                town = _town;
            }

            public CPoint(double _x, double _y, string _building, string _street, string _town, string _image)
            {
                x = _x;
                y = _y;
                building = _building;
                street = _street;
                town = _town;
                Image = _image;
            }
        }

        // Мой ключ для АПИ
        string myAPI = "";


        public Map()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        // Список точек для учереждений
        List<CPoint> ListWithPoinsOfParks = new List<CPoint>();
        List<CPoint> ListWithPoinsOfMcDonalds = new List<CPoint>();
        List<CPoint> ListWithPoinsOfCemeterys = new List<CPoint>();
        List<CPoint> ListWithPoinsOfJails = new List<CPoint>();
        List<CPoint> ListWithPoinsOfHospitals = new List<CPoint>();
        List<CPoint> ListWithPoinsOfGyms = new List<CPoint>();
        List<CPoint> ListWithPoinsOfUser = new List<CPoint>();
        List<CPoint> ListWithPoinsFromXML = new List<CPoint>();
        List<CPoint> ListWithPoinsPlaces = new List<CPoint>();
        List<CPoint> ListWithPoinsPict = new List<CPoint>();

        // Список слоёв к каждому месту
        GMapOverlay ListOfParks = new GMapOverlay("Парки");
        GMapOverlay ListOfMcDonalds = new GMapOverlay("МакДональдсы");
        GMapOverlay ListOfCemetery = new GMapOverlay("Кладбища");
        GMapOverlay ListOfJails = new GMapOverlay("Тюрьмы");
        GMapOverlay ListOfHospitals = new GMapOverlay("Больницы");
        GMapOverlay ListOfGyms = new GMapOverlay("Жимы");
        GMapOverlay PointsOfMyHouse = new GMapOverlay("МойДомик");
        GMapOverlay MyPosition = new GMapOverlay("Позиция");
        GMapOverlay PositionsForUser = new GMapOverlay("ПозицияпоЛКМ");
        GMapOverlay PositionFromTextBoxes = new GMapOverlay("ПозицияДляТекстБокса");
        GMapOverlay PositionForCircle = new GMapOverlay("ПозицияДляКруга");
        GMapOverlay ListOfXML = new GMapOverlay("XML");
        GMapOverlay ListOfTooltip = new GMapOverlay("Надписи");
        GMapOverlay ListOfPict = new GMapOverlay("Картинки");

        // Полигон для мака и моего места
        GMapOverlay polyOverlay = new GMapOverlay("Мак");

        // Полигоны трёх районов
        GMapOverlay polygonTestSquare = new GMapOverlay("Квадрат");
        List<CPoint> PointsPolygonsSquare = new List<CPoint>();
        GMapOverlay polygonTestPMK = new GMapOverlay("ПМК");
        List<CPoint> PointsPolygonsPMK = new List<CPoint>();
        GMapOverlay polygonTestMyArea = new GMapOverlay("Мой район");
        List<CPoint> PointsPolygonsMyArea = new List<CPoint>();

        // Полигон для Института - красивого полигона
        GMapOverlay overlayforpoly = new GMapOverlay("polygon");
        List<PointLatLng> listforpoly = new List<PointLatLng>();

        // Слой для пляжа - работа с тултипом
        GMapOverlay markersOverlayMy111 = new GMapOverlay("temp");

        // Инфа о маркере
        GMapOverlay OverlayCLICK = new GMapOverlay("Click");
        List<CPoint> ListClick = new List<CPoint>();

        // Точка в районе
        GMapOverlay Areas = new GMapOverlay("Areas");

        GMapOverlay polyOverlay12 = new GMapOverlay("polygons");


        private void gMapControl1_Load(object sender, EventArgs e)
        {

            // Создание элементов меню
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("Сохранить карту");

            ToolStripMenuItem DisplayPolygon = new ToolStripMenuItem("Отобразить полигон");
            ToolStripMenuItem DisplayAreas = new ToolStripMenuItem("Отобразить районы города");
            ToolStripMenuItem DisplayPolygonToolTip = new ToolStripMenuItem("Отобразить полигон с подписью");
            ToolStripMenuItem PolygonInPolygon = new ToolStripMenuItem("Полигон в полигоне");
            ToolStripMenuItem PolygonWithHole = new ToolStripMenuItem("Полигон с дырой");

            ToolStripMenuItem YandexMenuItem = new ToolStripMenuItem("Установить Яндекс-карту");
            ToolStripMenuItem GoogleMenuItem = new ToolStripMenuItem("Установить Google-карту");
            ToolStripMenuItem OpenCycleMapMenuItem = new ToolStripMenuItem("Установить OpenCycleMap-карту");

            ToolStripMenuItem ClearMap = new ToolStripMenuItem("Очистить карту");

            contextMenuStrip1.Items.AddRange(new[] { saveMenuItem, DisplayPolygon, DisplayPolygonToolTip, DisplayAreas, PolygonInPolygon,
                PolygonWithHole, YandexMenuItem, GoogleMenuItem, OpenCycleMapMenuItem,ClearMap });

            gmap.ContextMenuStrip = contextMenuStrip1;

            saveMenuItem.Click += saveMenuItem_Click;
            YandexMenuItem.Click += YandexMenuItem_Click;
            GoogleMenuItem.Click += GoogleMenuItem_Click;
            OpenCycleMapMenuItem.Click += OpenCycleMapMenuItem_Click;
            DisplayPolygonToolTip.Click += DisplayPolygonToolTip_Click;
            PolygonInPolygon.Click += PolygonInPolygon_Click;
            PolygonWithHole.Click += PolygonWithHole_Click;


            DisplayPolygon.Click += DisplayPolygon_Click;
            DisplayAreas.Click += DisplayAreas_Click;
            ClearMap.Click += ClearMap_Click;

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

            //gmap.CacheLocation = @"C:\Users\PC\Desktop\Проекты\OSM\WindowsFormsApp1\WindowsFormsApp1\obj\Debug";
            //gmap.CacheLocation = @"C:\Users\PC\Desktop\Проекты\OSM\WindowsFormsApp1\WindowsFormsApp1\obj\Debug\TileDBv5\en\Data.gmdb";


            // Для запросов
            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            // Для клика по адресу
            gmap.MouseClick += new MouseEventHandler(map_MouseClick);


            //GMapOverlay markersOverlayMy111 = new GMapOverlay("TempPosition");
            //Bitmap flag = new Bitmap(@"Icon/Cemetery.png");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(47.199346, 38.925777), flag);
            //marker.ToolTip = new GMapRoundedToolTip(marker);
            //// Наименование подписи
            //marker.ToolTipText = "Кладбище на пляже))";
            //marker.ToolTipMode = MarkerTooltipMode.Always;

            //marker.Offset = new Point(-flag.Width / 2, -flag.Height / 2);

            //// Задний фон подписи - 0 - 255 насыщенность и цвет
            //// marker.ToolTip.Fill = new SolidBrush(Color.FromArgb(150, Color.Yellow));

            //// Шрифт и размер надписи, подпись 
            //// marker.ToolTip.Font = new Font("Courier New", 15, FontStyle.Underline);

            //////Зафиксировать подпись без наведения на неё - при наведении, всегда и никогда
            //// marker.ToolTipMode = MarkerTooltipMode.Always;

            ////// Другой вид надписи
            //// marker.ToolTip = new GMapBaloonToolTip(marker);

            ////// Убрать рамочку у подписи
            //// marker.ToolTip.Stroke.Color = Color.FromArgb(0, 255, 255, 0);

            ////// Прозрачный фон
            //// marker.ToolTip.Fill = new SolidBrush(Color.Transparent);

            ////// Отступы надписи от краёв
            ////marker.ToolTip.TextPadding = new Size(20, 20);

            //////  Цвет текста надписи
            ////marker.ToolTip.Foreground = Brushes.Red;

            //markersOverlayMy111.Markers.Add(marker);
            //gmap.Overlays.Add(markersOverlayMy111);
        }


        // Полигон с дырой
        void PolygonWithHole_Click(object sender, EventArgs e)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            points.Add(new PointLatLng(47.2032319530629, 38.9336532354355));
            points.Add(new PointLatLng(47.2034360494388, 38.9358955621719));
            points.Add(new PointLatLng(47.2026706839804, 38.9359921216965));
            points.Add(new PointLatLng(47.2025722790489, 38.9336639642715));
            points.Add(new PointLatLng(47.2032319530629, 38.9336532354355));

            points.Add(new PointLatLng(47.2031299045805, 38.9340502023697));
            points.Add(new PointLatLng(47.2027836671951, 38.9340823888779));
            points.Add(new PointLatLng(47.2028602040748, 38.9356219768524));
            points.Add(new PointLatLng(47.2032829772305, 38.9355576038361));
            points.Add(new PointLatLng(47.2031299045805, 38.9340502023697));

            var polygon = new GMapPolygon(points, "mypolygon");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red);


            gmap.Overlays.Add(polyOverlay12);
            polyOverlay12.Polygons.Add(polygon);
        }

        // Полигон в полигоне
        void PolygonInPolygon_Click(object sender, EventArgs e)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(47.202574, 38.933674));
            points.Add(new PointLatLng(47.203226, 38.933651));
            points.Add(new PointLatLng(47.203445, 38.935897));
            points.Add(new PointLatLng(47.202670, 38.935984));
            var polygon = new GMapPolygon(points, "mypolygon");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);

            List<PointLatLng> points1 = new List<PointLatLng>();
            points1.Add(new PointLatLng(47.2031299045805, 38.9340502023697));
            points1.Add(new PointLatLng(47.2027836671951, 38.9340823888779));
            points1.Add(new PointLatLng(47.2028602040748, 38.9356219768524));
            points1.Add(new PointLatLng(47.2032829772305, 38.9355576038361));
            var polygon1 = new GMapPolygon(points1, "mypolygon1");
            polygon1.Fill = new SolidBrush(Color.FromArgb(50, Color.Green));
            polygon1.Stroke = new Pen(Color.Green, 1);

            gmap.Overlays.Add(polyOverlay12);
            polyOverlay12.Polygons.Add(polygon);
            polyOverlay12.Polygons.Add(polygon1);
        }


        // Подпись полигона
        void DisplayPolygonToolTip_Click(object sender, EventArgs e)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(47.202574, 38.933674));
            points.Add(new PointLatLng(47.203226, 38.933651));
            points.Add(new PointLatLng(47.203445, 38.935897));
            points.Add(new PointLatLng(47.202670, 38.935984));
            var polygon = new GMapPolygon(points, "mypolygon");

            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polygon.IsHitTestVisible = true;

            gmap.Overlays.Add(polyOverlay12);
            polyOverlay12.Polygons.Add(polygon);

            polyOverlay12.Markers.Add(new GMarkerCross(new PointLatLng(47.2029877652957, 38.9348119497299))
            {
                ToolTipText = "Университет",
                IsVisible = false,
                ToolTipMode = MarkerTooltipMode.Always
            });
            gmap.OnPolygonEnter += (poly) => polyOverlay12.Markers.First().IsVisible = true;
            gmap.OnPolygonLeave += (poly) => polyOverlay12.Markers.First().IsVisible = false;
        }



        // Отображение районов города
        void DisplayAreas_Click(object sender, EventArgs e)
        {
            // Добавление слоя для полигона
            // Чтение координат краёв района
            // Добавление точек в список
            // Создание из списка точек полигонов - района

            // РАЙОН КВАДРАТ
            gmap.Overlays.Add(polygonTestSquare);

            string[] ArrayOfStringsWithCoorSQ = File.ReadAllLines(@"Date/Полигоны_Квадратик.txt");
            for (int i = 0; i < ArrayOfStringsWithCoorSQ.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoorSQ[i].Split(new char[] { ';' });
                PointsPolygonsSquare.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            List<PointLatLng> pointsSquare = new List<PointLatLng>();
            for (int i = 0; i < PointsPolygonsSquare.Count; i++)
            {
                CPoint pointnew = new CPoint(PointsPolygonsSquare[i].x, PointsPolygonsSquare[i].y);
                pointsSquare.Add(new PointLatLng(pointnew.x, pointnew.y));
            }

            GMapPolygon polygonSq = new GMapPolygon(pointsSquare, "Квадрат");
            polygonSq.Fill = new SolidBrush(Color.FromArgb(50, Color.Green));
            polygonSq.Stroke = new Pen(Color.Green, 1);
            polygonTestSquare.Polygons.Add(polygonSq);
            gmap.Overlays.Add(polygonTestSquare);




            // РАЙОН ПМК
            gmap.Overlays.Add(polygonTestPMK);

            string[] ArrayOfStringsWithCoorPMK = File.ReadAllLines(@"Date/Полигоны_ПМК.txt");
            for (int i = 0; i < ArrayOfStringsWithCoorPMK.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoorPMK[i].Split(new char[] { ';' });
                PointsPolygonsPMK.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            List<PointLatLng> pointsPMK = new List<PointLatLng>();
            for (int i = 0; i < PointsPolygonsPMK.Count; i++)
            {
                CPoint pointnew = new CPoint(PointsPolygonsPMK[i].x, PointsPolygonsPMK[i].y);
                pointsPMK.Add(new PointLatLng(pointnew.x, pointnew.y));
            }

            GMapPolygon polygonPMK = new GMapPolygon(pointsPMK, "ПМК");
            polygonPMK.Fill = new SolidBrush(Color.FromArgb(50, Color.Yellow));
            polygonPMK.Stroke = new Pen(Color.Yellow, 1);
            polygonTestPMK.Polygons.Add(polygonPMK);
            gmap.Overlays.Add(polygonTestPMK);



            // РАЙОН МОЙ
            gmap.Overlays.Add(polygonTestMyArea);

            string[] ArrayOfStringsWithCoorMA = File.ReadAllLines(@"Date/Полигоны_Северный.txt");
            for (int i = 0; i < ArrayOfStringsWithCoorMA.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoorMA[i].Split(new char[] { ';' });
                PointsPolygonsMyArea.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            List<PointLatLng> pointsMyArea = new List<PointLatLng>();
            for (int i = 0; i < PointsPolygonsMyArea.Count; i++)
            {
                CPoint pointnew = new CPoint(PointsPolygonsMyArea[i].x, PointsPolygonsMyArea[i].y);
                pointsMyArea.Add(new PointLatLng(pointnew.x, pointnew.y));
            }

            GMapPolygon polygonMA = new GMapPolygon(pointsMyArea, "Мой район");
            polygonMA.Fill = new SolidBrush(Color.FromArgb(50, Color.Blue));
            polygonMA.Stroke = new Pen(Color.Blue, 1);
            polygonTestMyArea.Polygons.Add(polygonMA);
            gmap.Overlays.Add(polygonTestMyArea);
        }

        // Свойства полигона
        void DisplayPolygon_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(overlayforpoly);

            listforpoly.Add(new PointLatLng(47.202574, 38.933674));
            listforpoly.Add(new PointLatLng(47.203226, 38.933651));
            listforpoly.Add(new PointLatLng(47.203445, 38.935897));
            listforpoly.Add(new PointLatLng(47.202670, 38.935984));

            GMapPolygon mapPolygon = new GMapPolygon(listforpoly, "Институт");
            mapPolygon.Fill = new SolidBrush(Color.LightBlue); // центр - цвет
            mapPolygon.Stroke = new Pen(Color.Blue, 2); // рамочка - цвет и толщина 
            overlayforpoly.Polygons.Add(mapPolygon);
            gmap.Overlays.Add(overlayforpoly);
        }

        // Очистка карты
        void ClearMap_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Clear();

            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;

            ListWithPoinsOfParks.Clear();
            ListWithPoinsOfMcDonalds.Clear();
            ListWithPoinsOfCemeterys.Clear();
            ListWithPoinsOfJails.Clear();
            ListWithPoinsOfHospitals.Clear();
            ListWithPoinsOfGyms.Clear();
            ListWithPoinsOfUser.Clear();
            ListWithPoinsFromXML.Clear();
            ListWithPoinsPlaces.Clear();
            ListWithPoinsPict.Clear();

            ListOfParks.Clear();
            ListOfMcDonalds.Clear();
            ListOfCemetery.Clear();
            ListOfJails.Clear();
            ListOfHospitals.Clear();
            ListOfGyms.Clear();
            PointsOfMyHouse.Clear();
            MyPosition.Clear();
            PositionsForUser.Clear();
            PositionFromTextBoxes.Clear();
            PositionForCircle.Clear();
            ListOfXML.Clear();
            ListOfTooltip.Clear();
            ListOfPict.Clear();

            polygonTestSquare.Clear();
            PointsPolygonsSquare.Clear();
            polygonTestPMK.Clear();
            PointsPolygonsPMK.Clear();
            polygonTestMyArea.Clear();
            PointsPolygonsMyArea.Clear();

            overlayforpoly.Clear();
            listforpoly.Clear();

            OverlayCLICK.Clear();
            ListClick.Clear();

            markersOverlayMy111.Clear();
            Areas.Clear();

            polyOverlay12.Clear();
        }

        // Сохранение изображения
        void saveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialogforsavemap = new SaveFileDialog())
                {
                    // Формат картинки
                    dialogforsavemap.Filter = "PNG (*.png)|*.png";

                    // Название картинки
                    dialogforsavemap.FileName = "Текущее положение карты";

                    Image image = gmap.ToImage();

                    if (image != null)
                    {
                        using (image)
                        {
                            if (dialogforsavemap.ShowDialog() == DialogResult.OK)
                            {
                                string fileName = dialogforsavemap.FileName;
                                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                    fileName += ".png";

                                image.Save(fileName);
                                MessageBox.Show("Карта успешно сохранена в директории: " + Environment.NewLine + dialogforsavemap.FileName, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                }
            }

            // Если ошибка
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка при сохранении карты: " + Environment.NewLine + exception.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        // Смена поставщика карт
        void YandexMenuItem_Click(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.YandexMap;
            gmap.Zoom = 11;
            gmap.Position = new PointLatLng(47.2229, 38.9095);
        }

        void GoogleMenuItem_Click(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.GoogleMap;
            gmap.Zoom = 11;
            gmap.Position = new PointLatLng(47.2229, 38.9095);
        }

        void OpenCycleMapMenuItem_Click(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.OpenCycleMap;
            gmap.Zoom = 11;
            gmap.Position = new PointLatLng(47.2229, 38.9095);
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            // СМОТРИ 8 ЧАСТЬ С ИСПРАВЛЕНИЕМ ОШИБОК!!

            // ВЕЗДЕ ТОЛЬКО ОДИН СЛОЙ ДОБАВИТЬ!!!
            gmap.Overlays.Add(ListOfParks);
            if (checkBox2.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date/Координаты_Парков.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfParks.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfParks.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfParks[i].x, ListWithPoinsOfParks[i].y), new Bitmap(@"Icon/Park.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "Парк";
                    ListOfParks.Markers.Add(marker);
                    // СЛОЙ ДОБАВЛЯТЬ ВТОРОЙ НЕ НАДО
                }
            }
            else
            {
                // ВЕЗДЕ УБИРАТЬ СЛОЙ
                gmap.Overlays.Remove(ListOfParks);
                ListOfParks.Clear();
                ListWithPoinsOfParks.Clear();
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfMcDonalds);
            if (checkBox1.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Маков.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfMcDonalds.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfMcDonalds.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfMcDonalds[i].x, ListWithPoinsOfMcDonalds[i].y), new Bitmap(@"Icon/McDonalds.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "McDonalds";
                    ListOfMcDonalds.Markers.Add(marker);
                }
            }
            else
            {
                gmap.Overlays.Remove(ListOfMcDonalds);
                ListOfMcDonalds.Clear();
                ListWithPoinsOfMcDonalds.Clear();
            }
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfCemetery);
            if (checkBox3.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Кладбищей.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfCemeterys.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfCemeterys.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfCemeterys[i].x, ListWithPoinsOfCemeterys[i].y), new Bitmap(@"Icon/Cemetery.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "Кладбище";
                    ListOfCemetery.Markers.Add(marker);

                }
                gmap.Overlays.Add(ListOfCemetery);
            }
            else
            {
                ListOfCemetery.Clear();
                ListWithPoinsOfCemeterys.Clear();
            }
        }

        private void checkBox4_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfJails);
            if (checkBox4.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Тюрем.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfJails.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfJails.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfJails[i].x, ListWithPoinsOfJails[i].y), new Bitmap(@"Icon/Jail.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "Тюрьма";
                    ListOfJails.Markers.Add(marker);

                }
                gmap.Overlays.Add(ListOfJails);
            }
            else
            {
                ListOfJails.Clear();
                ListWithPoinsOfJails.Clear();
            }
        }

        private void checkBox5_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfHospitals);
            if (checkBox5.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Больниц.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfHospitals.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfHospitals.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfHospitals[i].x, ListWithPoinsOfHospitals[i].y), new Bitmap(@"Icon/Help.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "Больница";
                    ListOfHospitals.Markers.Add(marker);

                }
                gmap.Overlays.Add(ListOfHospitals);
            }
            else
            {
                ListOfHospitals.Clear();
                ListWithPoinsOfHospitals.Clear();
            }
        }

        private void checkBox6_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfGyms);
            if (checkBox6.Checked)
            {
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Жимов.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfGyms.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfGyms.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfGyms[i].x, ListWithPoinsOfGyms[i].y), new Bitmap(@"Icon/Gyms.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "Жим";
                    ListOfGyms.Markers.Add(marker);

                }
                gmap.Overlays.Add(ListOfGyms);
            }
            else
            {
                ListOfGyms.Clear();
                ListWithPoinsOfGyms.Clear();
            }
        }

        private void checkBox7_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(PointsOfMyHouse);
            if (checkBox7.Checked)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(47.2361059, 38.9061373), new Bitmap(@"Icon/MyHouse.png"));
                marker.ToolTip = new GMapRoundedToolTip(marker);
                marker.ToolTipText = "Мой домик";
                PointsOfMyHouse.Markers.Add(marker);
                gmap.Overlays.Add(PointsOfMyHouse);
            }
            else
                PointsOfMyHouse.Clear();
        }

        // Загрузить из файла координаты самому
        private void загрузитьКоординатыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<CPoint> points = new List<CPoint>();
            // Имя выбранного файла
            string FileName = "";

            // Пользователь выбирает файл на компьютере
            try
            {
                OpenFileDialog OpenDialogorChoiceAFile = new OpenFileDialog();
                // Форматы, которые может выбрать пользователь
                OpenDialogorChoiceAFile.Filter = "Text Files (*.TXT; *.CSV;|*.TXT; *.CSV;|All files(*.*)|*.*";

                // Путь, который перед ним откроется
                OpenDialogorChoiceAFile.InitialDirectory = @"C:\Users\PC\Desktop";
                //ofd.InitialDirectory = @"C:\Users\PC\Desktop";

                OpenDialogorChoiceAFile.Title = "Выбирите файл для подгрузки данных";

                if (OpenDialogorChoiceAFile.ShowDialog() == DialogResult.OK)
                    // Сохранили имя и формат выбранного файла
                    FileName = OpenDialogorChoiceAFile.FileName.ToString();
            }
            catch
            {
                MessageBox.Show("Error");
            }

            // Отсекаем имя и формат от друг друга
            string[] FileNameAndFormat = FileName.Split(new char[] { '.' });

            // Если пользователь выбран текстовый файл
            if (FileNameAndFormat[1] == "txt")
            {
                // Читаем в массив все строки файла
                string[] ArrayOfStringsWithCoor = File.ReadAllLines(FileName, Encoding.GetEncoding(1251));

                // От 0 до количества строк в файле
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    // Парсим строку на поля структуры
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    // Добавляем эту структуру 
                    points.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }
                MessageBox.Show("Данные успешно прочитаны");
            }

            // Если пользователь выбрал ксв файл - другого выбора не может быть
            else
            {
                // Читаем все строки
                using (StreamReader ReaderCSCFile = new StreamReader(FileName, Encoding.GetEncoding(1251)))
                {
                    while (!ReaderCSCFile.EndOfStream)
                    {
                        string ArrayOfStringsWithCoor = ReaderCSCFile.ReadLine();
                        string[] OneStringWithCoor = ArrayOfStringsWithCoor.Split(new char[] { ';' });
                        points.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                    }
                }
                MessageBox.Show("Данные успешно прочитаны");
            }

            // Проверка самого себя - что всё работает
            FileStream fileStream = new FileStream(@"Date\w.txt", FileMode.Open, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(1251));

            for (int i = 0; i < points.Count; i++)
                streamWriter.WriteLine(points[i].x + ";" + points[i].y);
            streamWriter.Close();
        }

        // Загрузка формы
        private void Map_Load(object sender, EventArgs e)
        {
            // Тебе не надо это смотри 10 и 11 часть
            StreamReader sr = new StreamReader(@"C:\Users\PC\Desktop\API.txt");
            myAPI = sr.ReadLine();

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 360;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Найти меня - не работает
        private void button1_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfMcDonalds);
            gmap.Overlays.Add(MyPosition);

            // 40074км - 360градусов
            // 111км - 1 градус
            // 1км - 0.009009009 = 0.009

            string SelectTypeOfPlace = comboBox1.Text;
            if (comboBox1.Text == "")
                MessageBox.Show("Выберите где искать");

            if (SelectTypeOfPlace == "McDonald`s")
            {
                CPoint Me = new CPoint(47.218930, 38.919681);      // yes
                // CPoint Me = new CPoint(47.214828, 38.911777);   // no

                GMarkerGoogle MarkerWithMyPosition = new GMarkerGoogle(new PointLatLng(Me.x, Me.y), GMarkerGoogleType.red_big_stop);
                MarkerWithMyPosition.ToolTip = new GMapRoundedToolTip(MarkerWithMyPosition);
                MarkerWithMyPosition.ToolTipText = "Ваше местоположение";
                MyPosition.Markers.Add(MarkerWithMyPosition);

                gmap.Overlays.Add(ListOfMcDonalds);
                gmap.Overlays.Add(MyPosition);

                string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Маков.txt");
                for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
                {
                    string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                    ListWithPoinsOfMcDonalds.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                for (int i = 0; i < ListWithPoinsOfMcDonalds.Count; i++)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                        (ListWithPoinsOfMcDonalds[i].x, ListWithPoinsOfMcDonalds[i].y), new Bitmap(@"Icon/McDonalds.png"));
                    marker.ToolTip = new GMapRoundedToolTip(marker);

                    marker.ToolTipText = "McDonalds";
                    ListOfMcDonalds.Markers.Add(marker);
                }
                gmap.Overlays.Add(ListOfMcDonalds);

                int count = 0;
                for (int i = 0; i < ListWithPoinsOfMcDonalds.Count; i++)
                {
                    double x1 = ListWithPoinsOfMcDonalds[i].x - Me.x;
                    double y1 = ListWithPoinsOfMcDonalds[i].y - Me.y;
                    double res = Math.Sqrt(x1 * x1 + y1 * y1);
                    if (res < 0.009)
                        count++;
                }

                if (count != 0)
                    MessageBox.Show("Рядом с вами есть Мак");
                else
                    MessageBox.Show("Рядом с вами нет Мака");
            }

        }

        private void полигонОбластьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(MyPosition);
            // CPoint Me = new CPoint(47.218930, 38.919681); // yes
            CPoint Me = new CPoint(47.214828, 38.911777);   // no  
            GMarkerGoogle MarkerWithMyPosition = new GMarkerGoogle(new PointLatLng(Me.x, Me.y), GMarkerGoogleType.red_big_stop);
            MarkerWithMyPosition.ToolTip = new GMapRoundedToolTip(MarkerWithMyPosition);
            MarkerWithMyPosition.ToolTipText = "Ваше местоположение";
            MyPosition.Markers.Add(MarkerWithMyPosition);
            gmap.Overlays.Add(MyPosition);


            List<PointLatLng> points = new List<PointLatLng>();
            CPoint point1 = new CPoint(47.219538, 38.919806);
            CPoint point2 = new CPoint(47.219350, 38.919666);
            CPoint point3 = new CPoint(47.219283, 38.919865);
            CPoint point4 = new CPoint(47.219473, 38.920003);

            points.Add(new PointLatLng(point1.x, point1.y)); // левый вверх
            points.Add(new PointLatLng(point2.x, point2.y)); // левый низ
            points.Add(new PointLatLng(point3.x, point3.y)); // правый низ
            points.Add(new PointLatLng(point4.x, point4.y)); // правый вверх

            GMapPolygon polygon = new GMapPolygon(points, "МакДональдс");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polyOverlay.Polygons.Add(polygon);
            gmap.Overlays.Add(polyOverlay);

            double oX = (point1.x + point2.x + point3.x + point4.x) / 4;
            double oY = (point1.y + point2.y + point3.y + point4.y) / 4;

            CPoint pointofaveragebuild = new CPoint(oX, oY);

            double x1 = pointofaveragebuild.x - Me.x;
            double y1 = pointofaveragebuild.y - Me.y;
            double res = Math.Sqrt(x1 * x1 + y1 * y1);

            if (res < 0.009)
                MessageBox.Show("Рядом с вами есть Мак");
            else
                MessageBox.Show("Рядом с вами нет Мака");
        }

        private void очиститьМеткиToolStripMenuItem_Click(object sender, EventArgs e) { }

        // Добавление маркера по двойному клику ЛКМ по карте
        private void gmap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                gmap.Overlays.Add(PositionsForUser);

                // Широта - latitude - lat - с севера на юг
                double x = gmap.FromLocalToLatLng(e.X, e.Y).Lng;
                // Долгота - longitude - lng - с запада на восток
                double y = gmap.FromLocalToLatLng(e.X, e.Y).Lat;

                textBox2.Text = x.ToString();
                textBox1.Text = y.ToString();

                // Добавляем метку на слой
                GMarkerGoogle MarkerWithMyPosition = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.blue_pushpin);
                MarkerWithMyPosition.ToolTip = new GMapRoundedToolTip(MarkerWithMyPosition);
                MarkerWithMyPosition.ToolTipText = "Метка пользователя";
                PositionsForUser.Markers.Add(MarkerWithMyPosition);

                // Сохранение наших координат (текстовик, цсв, бд, текстбокс, строки, лист)
                FileStream fileStream = new FileStream(@"Date\Координаты_ВыбранныеПользователем.txt", FileMode.Append, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(1251));
                streamWriter.WriteLine(y + ";" + x);
                streamWriter.Close();
            }
        }

        // Очистить все метки с карты
        private void очиститьМеткиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PositionsForUser.Clear();
            ListWithPoinsOfUser.Clear();
        }

        // Отобразить все метки на карте, поставленные ранее
        private void отобразитьМеткиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PositionsForUser.Clear();

            gmap.Overlays.Add(PositionsForUser);

            string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_ВыбранныеПользователем.txt");
            for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                ListWithPoinsOfUser.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            for (int i = 0; i < ListWithPoinsOfUser.Count; i++)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                    (ListWithPoinsOfUser[i].x, ListWithPoinsOfUser[i].y), GMarkerGoogleType.red_pushpin);
                marker.ToolTip = new GMapRoundedToolTip(marker);

                marker.ToolTipText = "Метка пользователя";
                // Изменить цвет надписи у маркера
                marker.ToolTip.Foreground = Brushes.Red;
                PositionsForUser.Markers.Add(marker);
            }
            gmap.Overlays.Add(PositionsForUser);
        }

        // Поставить по введенным координатам метку
        private void button2_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(PositionFromTextBoxes);

            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) &&
                !string.IsNullOrEmpty(textBox4.Text) && !string.IsNullOrWhiteSpace(textBox4.Text))
            {
                double x = Convert.ToDouble(textBox4.Text);
                double y = Convert.ToDouble(textBox3.Text);

                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(y, x), new Bitmap(@"Icon/Flag.png"));
                marker.ToolTip = new GMapRoundedToolTip(marker);
                marker.ToolTipText = "Метка пользователя";
                PositionFromTextBoxes.Markers.Add(marker);
                gmap.Overlays.Add(PositionFromTextBoxes);
            }
            else
                MessageBox.Show("Введите обе координаты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        // Расстояние между двумя точками
        private void временнаяКнопкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Две координаты
            GeoCoordinate MyHouse = new GeoCoordinate(47.2361911063733, 38.9064772169801);
            GeoCoordinate MyUniversity = new GeoCoordinate(47.203103, 38.934850);

            // Расстояние между ними
            double distanse = MyHouse.GetDistanceTo(MyUniversity);
            // Округление расстояния
            distanse = Math.Ceiling(distanse);

            // Вывод расстояние в метрах
            MessageBox.Show("Расстояние от моего дома до университета: " + distanse / 1000 + " км");
        }


        GMapOverlay ааааа = new GMapOverlay("пппп");

        // Определение нашего местоположения
        private void моёМестоположениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ааааа);

            // Телефон - Точка доступа, GPS, Интернет
            // Расположение включено
            //bool abort = false;

            // Координата нашего местоположения
            GeoCoordinate coord;
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            // Инициирует получение данных расположения от текущего поставщика расположения
            if (watcher.TryStart(false, TimeSpan.FromMilliseconds(1000)))
            {
                while (watcher.Status != GeoPositionStatus.Ready)
                    watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

                //DateTime start = DateTime.Now;

                //// Получаем текущее состояние объекта watcher
                //while (watcher.Status != GeoPositionStatus.Ready && !abort)
                //{
                //    Thread.Sleep(200);
                //    if (DateTime.Now.Subtract(start).TotalSeconds > 5)
                //        abort = true;
                //}

                // Определить наши координаты
                coord = watcher.Position.Location;

                // Если координаты числа
                if (!coord.IsUnknown)
                {
                    // Сохраняем в переменные наши координаты
                    double Latitude = coord.Latitude;
                    double Longitude = coord.Longitude;

                    GMarkerGoogle marker11 = new GMarkerGoogle(new PointLatLng(Latitude, Longitude), GMarkerGoogleType.arrow);
                    marker11.ToolTip = new GMapRoundedToolTip(marker11);
                    marker11.ToolTipText = "Моf";
                    ааааа.Markers.Add(marker11);
                    gmap.Overlays.Add(ааааа);
                }
                // Если координаты не получены
                else
                    throw new Exception("Координаты - не числа");
            }
            // Если не может получить данные
            else
                throw new Exception("Данные не получены");
        }

        // Отрисовка круга около маркера
        // -----------------------------------------------------------------------------
        private void CreateCircle(double lat, double lon, double radius, int ColorIndex)
        {
            PointLatLng point = new PointLatLng(lat, lon);
            int segments = 1080;

            List<PointLatLng> gpollist = new List<PointLatLng>();

            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i * (Math.PI / 180), radius / 1000));

            GMapPolygon polygon = new GMapPolygon(gpollist, "Circle");
            switch (ColorIndex)
            {
                case 1:
                    polygon.Fill = new SolidBrush(Color.FromArgb(80, Color.Red));
                    break;
                case 2:
                    polygon.Fill = new SolidBrush(Color.FromArgb(80, Color.Orange));
                    break;
                case 3:
                    polygon.Fill = new SolidBrush(Color.FromArgb(20, Color.Aqua));
                    break;
                default:
                    MessageBox.Show("No search zone found!");
                    break;
            }
            polygon.Stroke = new Pen(Color.Red, 1);
            PositionForCircle.Polygons.Add(polygon);
            gmap.Overlays.Add(PositionForCircle);
        }
        public static PointLatLng FindPointAtDistanceFrom(PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));
            var endLonRads = startLonRad + Math.Atan2(Math.Sin(initialBearingRadians) * distRatioSine * startLatCos, distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }
        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }
        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }
        public static double DistanceTwoPoint(double startLat, double startLong, double endLat, double endLong)
        {
            var startPoint = new GeoCoordinate(startLat, startLong);
            var endPoint = new GeoCoordinate(endLat, endLong);
            return startPoint.GetDistanceTo(endPoint);
        }
        // -----------------------------------------------------------------------------

        private void кругУТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Наша точка
            CPoint PointForCircle = new CPoint(47.205007, 38.946395);
            gmap.Overlays.Add(PositionForCircle);
            GMarkerGoogle gMarkerForCircle = new GMarkerGoogle(new PointLatLng(PointForCircle.x, PointForCircle.y), GMarkerGoogleType.red);
            gMarkerForCircle.ToolTip = new GMapRoundedToolTip(gMarkerForCircle);
            gMarkerForCircle.ToolTipText = "Радиус";
            PositionForCircle.Markers.Add(gMarkerForCircle);
            gmap.Overlays.Add(PositionForCircle);


            // Вариант №1
            // 1 - красная область, 2 - оранжевая область, 3 - голубая прозрачная область
            CreateCircle(PointForCircle.x, PointForCircle.y, 1000, 1);

            // Вариант №2
            //int segments = 100;
            //double radius = 0.001;
            //List<PointLatLng> pointLats = new List<PointLatLng>();

            //double seg = Math.PI * 2 / segments;
            //int y = 0;
            //for (int i = 0; i < segments; i++)
            //{
            //    double theta = seg * i;
            //    double a = PointForCircle.x + Math.Cos(theta) * radius;
            //    double b = PointForCircle.y + Math.Sin(theta) * radius;

            //    PointLatLng gpoi = new PointLatLng(a, b);
            //    pointLats.Add(gpoi);
            //}
            //GMapPolygon gpol = new GMapPolygon(pointLats, "pol");
            //PositionForCircle.Polygons.Add(gpol);
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListOfXML.Clear();
            ListWithPoinsFromXML.Clear();
        }

        private void отобразитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfXML);

            // Создали документ
            XmlDocument xml = new XmlDocument();
            // Открыли его по пути
            xml.Load(@"Date\Coordinats.xml");
            XmlElement element = xml.DocumentElement;

            // Элементы ХМЛ-документа
            foreach (XmlElement xnode in element)
            {
                CPoint cPoint = new CPoint();
                // Если атрибут = place
                XmlNode attr = xnode.Attributes.GetNamedItem("place");

                // У каждого узла смотрим его поля
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "X")
                        cPoint.x = Convert.ToDouble(childnode.InnerText);
                    if (childnode.Name == "Y")
                        cPoint.y = Convert.ToDouble(childnode.InnerText);
                    if (childnode.Name == "description")
                        cPoint.place = childnode.InnerText.ToString();
                }
                ListWithPoinsFromXML.Add(cPoint);
            }

            for (int i = 0; i < ListWithPoinsFromXML.Count; i++)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                    (ListWithPoinsFromXML[i].x, ListWithPoinsFromXML[i].y), GMarkerGoogleType.pink_pushpin);
                marker.ToolTip = new GMapRoundedToolTip(marker);
                marker.ToolTipText = ListWithPoinsFromXML[i].place;
                ListOfXML.Markers.Add(marker);
            }
        }

        // Нажатие на красный крестик формы
        private void Map_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    e.Cancel = false;
            //else
            //    e.Cancel = true;
        }

        private void отобразитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfTooltip);

            string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Описание.txt", Encoding.GetEncoding(1251));
            for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                ListWithPoinsPlaces.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1]), OneStringWithCoor[2], OneStringWithCoor[3], OneStringWithCoor[4]));
            }

            for (int i = 0; i < ListWithPoinsPlaces.Count; i++)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ListWithPoinsPlaces[i].x, ListWithPoinsPlaces[i].y), GMarkerGoogleType.arrow);
                marker.ToolTip = new GMapRoundedToolTip(marker);
                marker.ToolTipText = ListWithPoinsPlaces[i].building + "\n" + ListWithPoinsPlaces[i].street + "\n" + ListWithPoinsPlaces[i].town;
                ListOfTooltip.Markers.Add(marker);
            }
            gmap.Overlays.Add(ListOfTooltip);
        }

        private void очиститьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListOfTooltip.Clear();
            ListWithPoinsPlaces.Clear();
        }

        private void отобразитьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(ListOfPict);

            string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Маков.txt");
            for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                ListWithPoinsPict.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
            }

            for (int i = 0; i < ListWithPoinsPict.Count; i++)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng
                    (ListWithPoinsPict[i].x, ListWithPoinsPict[i].y), new Bitmap(@"Icon/Place4.png"));
                marker.ToolTip = new GMapRoundedToolTip(marker);

                //marker.ToolTipText = "McDonalds";
                ListOfPict.Markers.Add(marker);
            }
            gmap.Overlays.Add(ListOfPict);
        }

        private void очиститьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListOfPict.Clear();
            ListWithPoinsPict.Clear();
        }

        // Последняя точка выбранная на карте в каком районе
        private void тЧКВРайонеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(Areas);

            // Текста для координат
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                double x = Convert.ToDouble(textBox1.Text);
                double y = Convert.ToDouble(textBox2.Text);

                // Флаг, наша точка и ее район
                PointLatLng pointsearch = new PointLatLng(x, y);
                bool flag = false;
                string nameofarea = "";

                // Идем по слоям
                var overlays = gmap.Overlays;
                foreach (var overlay in overlays)
                {
                    // Идем по полигонам
                    var polygons = overlay.Polygons;
                    foreach (var polygon in polygons)
                    {
                        // В каждом полигоне каждого слоя проверяем принадлежность точки полигону
                        if (polygon.IsInside(pointsearch))
                        {
                            // Флаг тру
                            flag = true;
                            // Имя района - имя полигона
                            nameofarea = polygon.Name;

                            // Ставим маркер в район и даем имя маркеру имя района
                            GMarkerGoogle pointinarea = new GMarkerGoogle(new PointLatLng(x, y), GMarkerGoogleType.green_big_go);
                            pointinarea.ToolTip = new GMapRoundedToolTip(pointinarea);
                            pointinarea.ToolTipText = nameofarea;
                            Areas.Markers.Add(pointinarea);
                        }
                    }
                }

                // Выводим на экран надпись где же метка
                if (flag)
                    MessageBox.Show("Координата принадлежит району: " + nameofarea);
                else
                    MessageBox.Show("Координата не принадлежит какому-либо району");
            }
            else
                MessageBox.Show("Выберите точку на карте двойным ЛКМ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        // Клик по маркеру чем-либо
        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            // Если нажали ЛКМ по метке - выдать подробную ифну о маркере
            if (e.Button == MouseButtons.Left)
            {
                // идем по всем координатам
                for (int i = 0; i < ListClick.Count; i++)
                {
                    // если координаты выбранной совпали с 1 из списка
                    if (item.Position.Lat == ListClick[i].x && item.Position.Lng == ListClick[i].y)
                    {
                        // выводим место улицу и город и фото
                        textBox5.Text = ListClick[i].building;
                        textBox6.Text = ListClick[i].street;
                        textBox7.Text = ListClick[i].town;

                        pictureBox1.Image = Image.FromFile(Application.StartupPath + ListClick[i].Image);
                    }
                }
            }

            // Если нажали слева на мышке вверху - удалить маркер
            if (e.Button == MouseButtons.XButton1)
            {
                // Узнаем слой удаляемого маркера
                GMapOverlay overlay = item.Overlay;
                // Удаляем в этом слое этот маркер
                overlay.Markers.Remove(item);

                // Сделать неведимым маркер
                // item.IsVisible = false;

                // Удалить весь слой при удалении одного маркера
                // gmap.Overlays.Remove(item.Overlay);
            }

            // Если нажали на колесо мыши - переимновать маркер по текстбоксу
            if (e.Button == MouseButtons.Middle)
            {
                // Текста для координат
                if (!string.IsNullOrEmpty(textBox8.Text) && !string.IsNullOrWhiteSpace(textBox8.Text))
                    item.ToolTipText = textBox8.Text;
                else
                    MessageBox.Show("Дайте новое имя метке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        // Наведение на маркер
        private void gmap_OnMarkerEnter(GMapMarker item)
        {
            // MessageBox.Show("Ты навёл на меня. Зачем?");

            // textBox3.Text = item.Position.Lat.ToString();
            // textBox4.Text = item.Position.Lng.ToString();
        }


        // Инфа о маркере - считываем инфу о маркере - фото, адрес, сайт телефон вр работы
        private void button3_Click(object sender, EventArgs e)
        {
            gmap.Overlays.Add(OverlayCLICK);

            string[] ArrayOfStringsWithCoor = File.ReadAllLines(@"Date\Координаты_Маркер_Информация.txt", Encoding.GetEncoding(1251));
            for (int i = 0; i < ArrayOfStringsWithCoor.Length; i++)
            {
                string[] OneStringWithCoor = ArrayOfStringsWithCoor[i].Split(new char[] { ';' });
                ListClick.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1]), OneStringWithCoor[2], OneStringWithCoor[3], OneStringWithCoor[4], OneStringWithCoor[5]));
            }

            for (int i = 0; i < ListClick.Count; i++)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ListClick[i].x, ListClick[i].y), GMarkerGoogleType.arrow);
                marker.ToolTip = new GMapRoundedToolTip(marker);
                marker.ToolTipMode = MarkerTooltipMode.Never;
                OverlayCLICK.Markers.Add(marker);
            }
            gmap.Overlays.Add(OverlayCLICK);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            gmap.Bearing = trackBar1.Value;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e) { }

        // Скачка карты
        private void button4_Click(object sender, EventArgs e)
        {
            RectLatLng area = gmap.SelectedArea;
            if (area.IsEmpty)
            {
                DialogResult res = MessageBox.Show("Нет области для скачивания", "Ошибка", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                    area = gmap.ViewArea;
            }

            if (!area.IsEmpty)
            {
                DialogResult res = MessageBox.Show("Ready ripp at Zoom = " + (int)gmap.Zoom + " ?", "GMap.NET", MessageBoxButtons.YesNo);

                for (int i = 1; i <= gmap.MaxZoom; i++)
                {
                    if (res == DialogResult.Yes)
                    {
                        TilePrefetcher obj = new TilePrefetcher();
                        obj.ShowCompleteMessage = false;
                        obj.Start(area, i, gmap.MapProvider, 100, 0);

                    }
                    else if (res == DialogResult.No)
                        continue;
                    else if (res == DialogResult.Cancel)
                        break;
                }
            }
            else
                MessageBox.Show("Выбери площадь на карте с зажатой ATL", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }


        // Слой для маркеров аддресов
        GMapOverlay OverlayForAddress = new GMapOverlay("Address");
        // Ввод адреса в текстбокс и отобразить на карте координаты этого адреса
        private void button5_Click(object sender, EventArgs e)
        {
            // Запрос + считанный из апи ключ
            string zapros = "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true_or_false&language=ru&key=" + myAPI;

            // Запрос к API
            string url = string.Format(zapros, Uri.EscapeDataString(textBox9.Text));

            // Выполняем запрос
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Получаем ответ от интернет-ресурса
            WebResponse response = request.GetResponse();

            // Чтение данных из интернет-ресурса
            Stream dataStream = response.GetResponseStream();

            // Чтение
            StreamReader sreader = new StreamReader(dataStream);

            // Считывает поток от текущего положения до конца         
            string responsereader = sreader.ReadToEnd();

            // Закрываем поток ответа
            response.Close();



            // Блок парсинга данных

            XmlDocument xmldoc = new XmlDocument();

            xmldoc.LoadXml(responsereader);

            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                // Получение широты и долготы
                XmlNodeList nodes = xmldoc.SelectNodes("//location");

                // Широта и долгота
                double latitude = 0.0;
                double longitude = 0.0;

                // Получаем широту и долготу
                foreach (XmlNode node in nodes)
                {
                    latitude = XmlConvert.ToDouble(node.SelectSingleNode("lat").InnerText.ToString());
                    longitude = XmlConvert.ToDouble(node.SelectSingleNode("lng").InnerText.ToString());
                }

                // Строка со всеми данными
                string formatted_address = xmldoc.SelectNodes("//formatted_address").Item(0).InnerText.ToString();

                //// Запись данных
                //FileStream fileStream = new FileStream(@"Date\Адреса.txt", FileMode.Append, FileAccess.Write);
                //StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(1251));
                //streamWriter.Write(latitude + " " + longitude + " ");


                // Парсинг №1
                string[] words = formatted_address.Split(',');
                string dataMarker = string.Empty;
                foreach (string word in words)
                {
                    //streamWriter.Write(word + " ");
                    dataMarker += word + ";" + Environment.NewLine;
                }

                //streamWriter.WriteLine();
                //streamWriter.Close();



                // Виды парсингов


                // Парсинг №2
                //Таганрог, улица 1 - ая Котельная, 71
                //string[] words = formatted_address.Split(',');
                ////Улица.
                //string Street = words[0].Trim();
                ////Дом.
                //string house = words[1].Trim();
                ////Город.
                //string City = words[2].Trim();
                ////Область.
                //string Region = words[3].Trim();
                ////Страна.
                //string Country = words[4].Trim();
                ////Почтовый индекс.
                //string PostalCode = words[5].Trim();





                // Парсинг №3
                // Таганрог, улица 1-ая Котельная, 71
                //string house = xmldoc.SelectNodes("//address_component").Item(0).SelectNodes("short_name").Item(0).InnerText.ToString();
                //string Street = xmldoc.SelectNodes("//address_component").Item(1).SelectNodes("short_name").Item(0).InnerText.ToString();
                //string Region = xmldoc.SelectNodes("//address_component").Item(2).SelectNodes("short_name").Item(0).InnerText.ToString();
                //string City = xmldoc.SelectNodes("//address_component").Item(3).SelectNodes("short_name").Item(0).InnerText.ToString();
                //string Country = xmldoc.SelectNodes("//address_component").Item(6).SelectNodes("long_name").Item(0).InnerText.ToString();
                //string PostalCode = xmldoc.SelectNodes("//address_component").Item(7).SelectNodes("short_name").Item(0).InnerText.ToString();


                // Маркер для 1 парсинга
                gmap.Overlays.Add(OverlayForAddress);
                GMarkerGoogle addressmarker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.orange);
                addressmarker.ToolTip = new GMapRoundedToolTip(addressmarker);
                addressmarker.ToolTipMode = MarkerTooltipMode.Always;
                addressmarker.ToolTipText = dataMarker;
                OverlayForAddress.Markers.Add(addressmarker);
                gmap.Position = new PointLatLng(latitude, longitude);





                // Маркер для 2 и 3 парсинга
                //addressmarker.ToolTipText =
                //   "Почтовый ин.: " + PostalCode + Environment.NewLine +
                //   "Страна: " + Country + Environment.NewLine +
                //   "Город: " + City + Environment.NewLine +
                //   "Область: " + Region + Environment.NewLine +
                //   "Улица: " + Street + Environment.NewLine +
                //   "Номер дома: " + house + Environment.NewLine;
            }
        }


        // Слой для маркеров адресов по клику на карте
        GMapOverlay AddressClick = new GMapOverlay("AddressClick");

        // Клик по карте ПКМ и установка маркера с данными о нём
        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            // Если нажата ПКМ и отключен КМ
            if (e.Button == MouseButtons.Right)
            {
                double lat = gmap.FromLocalToLatLng(e.X, e.Y).Lat;
                double lng = gmap.FromLocalToLatLng(e.X, e.Y).Lng;

                // Запрос + считанный из апи ключ
                string zapros = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=true_or_false&language=ru&key=" + myAPI;

                // Запрос к API
                string url = string.Format(zapros, lat.ToString().Replace(",", "."), lng.ToString().Replace(",", "."));

                // Выполняем запрос
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Получаем ответ от интернет-ресурса
                WebResponse response = request.GetResponse();

                // Чтение данных из интернет-ресурса
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);          
                string responsereader = sreader.ReadToEnd();
                response.Close();


                XmlDocument xmldoc = new XmlDocument();

                xmldoc.LoadXml(responsereader);

                if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
                {
                    // Получение информации о найденном объекте
                    string formatted_address = xmldoc.SelectNodes("//formatted_address").Item(0).InnerText.ToString();

                    string[] words = formatted_address.Split(',');
                    string dataMarker = string.Empty;
                    foreach (string word in words)
                    {
                        dataMarker += word + ";" + Environment.NewLine;
                    }

                    gmap.Overlays.Add(AddressClick);
                    GMarkerGoogle addressmarker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red);
                    addressmarker.ToolTip = new GMapRoundedToolTip(addressmarker);
                    addressmarker.ToolTipMode = MarkerTooltipMode.Always;
                    addressmarker.ToolTipText = dataMarker;
                    AddressClick.Markers.Add(addressmarker);
                }
            }
        }


        // Слой для маркеров и маршрута
        GMapOverlay AtoB = new GMapOverlay("AtoB");
        List<CPoint> WayAtoB = new List<CPoint>();

        // Маршрут из А в Б
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AtoB.Clear();
            WayAtoB.Clear();

            // Запрос
            string zapros = "https://maps.googleapis.com/maps/api/directions/xml?origin={0},&destination={1}&sensor=false&language=ru&mode={2}&key=" + myAPI;

            string url = string.Format(zapros, 
                Uri.EscapeDataString("Таганрог, улица 1-ая Котельная, 71"), 
                Uri.EscapeDataString("Таганрог, улица Дзержинского, 169"), 
                Uri.EscapeDataString("driving"));


            // Запрос
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();


            // Парсинг
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responsereader);
            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList nodes = xmldoc.SelectNodes("//leg//step");

                // Получение координат начала пути
                double latStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[1].InnerText);

                // Получение координат конечной точки
                double latEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[1].InnerText);


                gmap.Overlays.Add(AtoB);

                // Маркер старта и финиша
                GMarkerGoogle markerG = new GMarkerGoogle(new PointLatLng(latStart, lngStart), GMarkerGoogleType.green);
                markerG.ToolTip = new GMapRoundedToolTip(markerG);
                markerG.ToolTipMode = MarkerTooltipMode.Always;
                markerG.ToolTipText = "Таганрог, улица 1-ая Котельная, 71";

                GMarkerGoogle markerR = new GMarkerGoogle(new PointLatLng(latEnd, lngEnd), GMarkerGoogleType.red);
                markerR.ToolTip = new GMapRoundedToolTip(markerR);
                markerR.ToolTipMode = MarkerTooltipMode.Always;
                markerR.ToolTipText = "Таганрог, улица Дзержинского, 169";

                AtoB.Markers.Add(markerG);
                AtoB.Markers.Add(markerR);
                gmap.Overlays.Clear();


                string[] temp = File.ReadAllLines(@"Date/АБ.txt");
                for (int i = 0; i < temp.Length; i++)
                {
                    string[] OneStringWithCoor = temp[i].Split(new char[] { ';' });
                    WayAtoB.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                List<PointLatLng> WayAtoBPoints = new List<PointLatLng>();
                for (int i = 0; i < WayAtoB.Count; i++)
                {
                    CPoint pointnew = new CPoint(WayAtoB[i].x, WayAtoB[i].y);
                    WayAtoBPoints.Add(new PointLatLng(pointnew.x, pointnew.y));
                }


                // Очищаем все маршруты
                AtoB.Routes.Clear();

                // Создаем маршрут на основе списка контрольных точек
                GMapRoute r = new GMapRoute(WayAtoBPoints, "Route");

                // Указываем, что данный маршрут должен отображаться
                r.IsVisible = true;

                // Устанавливаем цвет маршрута
                r.Stroke.Color = Color.Yellow;

                // Добавляем маршрут
                AtoB.Routes.Add(r);

                // Добавляем в компонент, список маркеров и маршрутов
                gmap.Overlays.Add(AtoB);
            }
        }



        // Слой для маркеров и маршрута
        GMapOverlay AtoA = new GMapOverlay("AtoA");
        List<CPoint> WayAtoA = new List<CPoint>();

        // Маршрут из А в A
        private void маршрутИзАВАToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AtoA.Clear();
            WayAtoA.Clear();
            
            // Запрос
            string zapros = "https://maps.googleapis.com/maps/api/directions/xml?origin={0},&destination={1}&sensor=false&language=ru&mode={2}&key=" + myAPI;

            string url = string.Format(zapros,
                Uri.EscapeDataString("Таганрог, улица 1-ая Котельная, 71"),
                Uri.EscapeDataString("Таганрог, улица 1-ая Котельная, 71"),
                Uri.EscapeDataString("driving"));


            // Запрос
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();


            // Парсинг
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responsereader);
            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList nodes = xmldoc.SelectNodes("//leg//step");

                // Получение координат начала пути
                double latStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngStart = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[1].InnerText);

                // Получение координат конечной точки
                double latEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[0].InnerText);
                double lngEnd = XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[1].InnerText);


                gmap.Overlays.Add(AtoA);

                // Маркер старта и финиша
                GMarkerGoogle markerG = new GMarkerGoogle(new PointLatLng(latStart, lngStart), GMarkerGoogleType.green);
                markerG.ToolTip = new GMapRoundedToolTip(markerG);
                markerG.ToolTipMode = MarkerTooltipMode.Always;
                markerG.ToolTipText = "Таганрог, улица 1-ая Котельная, 71";
                AtoA.Markers.Add(markerG);
                gmap.Overlays.Clear();


                string[] temp = File.ReadAllLines(@"Date/АА.txt");
                for (int i = 0; i < temp.Length; i++)
                {
                    string[] OneStringWithCoor = temp[i].Split(new char[] { ';' });
                    WayAtoA.Add(new CPoint(Convert.ToDouble(OneStringWithCoor[0]), Convert.ToDouble(OneStringWithCoor[1])));
                }

                List<PointLatLng> WayAtoAPoints = new List<PointLatLng>();
                for (int i = 0; i < WayAtoA.Count; i++)
                {
                    CPoint pointnew = new CPoint(WayAtoA[i].x, WayAtoA[i].y);
                    WayAtoAPoints.Add(new PointLatLng(pointnew.x, pointnew.y));
                }



                // Очищаем все маршруты
                AtoA.Routes.Clear();

                // Создаем маршрут на основе списка контрольных точек
                GMapRoute r = new GMapRoute(WayAtoAPoints, "Route");

                // Указываем, что данный маршрут должен отображаться
                r.IsVisible = true;

                // Устанавливаем цвет маршрута
                r.Stroke.Color = Color.YellowGreen;

                // Добавляем маршрут
                AtoA.Routes.Add(r);

                // Добавляем в компонент, список маркеров и маршрутов
                gmap.Overlays.Add(AtoA);
            }
        }

        // Маршрутизация
        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            Marshruts form = new Marshruts(myAPI);
            form.ShowDialog();
            Close();
        }


        // Панорама
        private void button7_Click(object sender, EventArgs e)
        {
            Hide();
            Panorama form = new Panorama(myAPI);
            form.ShowDialog();
            Close();
        }
    }
}