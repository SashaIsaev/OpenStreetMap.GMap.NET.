using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

// Библиотеки для карты
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace WindowsFormsApp1
{
    public partial class Panorama : Form
    {
        string myAPI = "";
        public Panorama(string API)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            myAPI = API;
        }

        private void Panorama_Load(object sender, EventArgs e)
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

            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            gmap.MouseClick += new MouseEventHandler(map_MouseClick);
        }

        double lat, lng;
        GMapOverlay panoramaoverlay = new GMapOverlay("marker");

        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lat = gmap.FromLocalToLatLng(e.X, e.Y).Lat;
                lng = gmap.FromLocalToLatLng(e.X, e.Y).Lng;
               
                GMarkerCross markerforPanorama = new GMarkerCross(new PointLatLng(lat, lng));
                panoramaoverlay.Markers.Add(markerforPanorama);
                gmap.Overlays.Add(panoramaoverlay);

                // Загрузить картинку места по координатам и 3 трекбарам
                LoadStreetImage(lat, lng, trackBar1.Value, trackBar2.Value, trackBar3.Value);
            }
        }


        // Загрузка изображения
        public void LoadStreetImage(double lat, double lng, int heading, int pitch, int fov)
        {
            string zapros = "https://maps.googleapis.com/maps/api/streetview?size={0}x{1}&location={2},{3}&heading={4}&pitch={5}&sensor=false&fov={6}&key=" + myAPI;
            var request = WebRequest.Create(String.Format(zapros, pictureBox1.Width, pictureBox1.Height, lat, lng, heading, pitch, fov));

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                pictureBox1.Image = Bitmap.FromStream(stream);
            }
        }

        // Обработка сдвигов трекбаров
        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            LoadStreetImage(lat, lng, trackBar1.Value, trackBar2.Value, trackBar3.Value);

        }
        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            LoadStreetImage(lat, lng, trackBar1.Value, trackBar2.Value, trackBar3.Value);

        }
        private void trackBar3_Scroll_1(object sender, EventArgs e)
        {
            LoadStreetImage(lat, lng, trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }
        private void Panorama_Resize(object sender, EventArgs e)
        {
            LoadStreetImage(lat, lng, trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }
    }
}