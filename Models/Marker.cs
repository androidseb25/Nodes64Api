using System;
using Newtonsoft.Json;
using Nodes64.Models;
using Nodes64Api.Functions;
using Nodes64Api.Models.Database;

namespace Nodes64Api.Models
{
	public class Marker : DBBase
    {
        public Marker(ConnectionStrings connStrings) : base(connStrings) { }
        public Marker() { }

        public MarkerLatLng? MarkerLatLng { get; set; }
		public string? MarkerInfos { get; set; }
        public string? MarkerInfosMS { get; set; }
        public string? MarkerIcon { get; set; } = "./assets/person_pin.svg";
        public List<PolylineOptions>? PolyOption { get; set; }
        public string? NodeId { get; set; }
        public string? NodeIdName { get; set; }
        public bool NodeIdNamePublic { get; set; }

        public async Task<LatLng?> GetMyNodeLatLng(string nodeId)
        {
            try
            {
                string sql = @$"select n.latitude, n.longitude from nodes n
where n.node_id = '{nodeId}'";
                Console.WriteLine(sql);
                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<LatLng> icmpList = JsonConvert.DeserializeObject<List<LatLng>>(jsonData);
                if (icmpList.Count > 0)
                    return icmpList[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        public async Task<List<Tasks_ICMP>> GetAllICMPTask(string ttype, string nodeId, int lastHour = 1)
        {
            try
            {
                string sql = @$"select icmp.*, n.latitude, n.longitude, n.node_name, n.node_name_public from tasks_icmp icmp
left join nodes n on n.node_id = icmp.src_node
where icmp.task_type = '{ttype}'
and icmp.dst_node = '{nodeId}'
and icmp.report_time >= DATE_SUB(NOW(),INTERVAL {lastHour} HOUR)
and CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0";
                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<Tasks_ICMP> icmpList = JsonConvert.DeserializeObject<List<Tasks_ICMP>>(jsonData);
                return icmpList == null ? new List<Tasks_ICMP>() : icmpList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return new List<Tasks_ICMP>();
        }

        public async Task<List<Tasks_TraceRoute>> GetAllTracerouteTask(string ttype, string nodeId, int lastHour = 1)
        {
            try
            {
                string sql = @$"select tr.*, n.latitude, n.longitude, n.node_name, n.node_name_public from tasks_traceroute tr
left join nodes n on n.node_id = tr.src_node
where tr.task_type = '{ttype}'
and tr.dst_node = '{nodeId}'
and tr.report_time >= DATE_SUB(NOW(),INTERVAL {lastHour} HOUR)
and CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0
";
                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<Tasks_TraceRoute> trList = JsonConvert.DeserializeObject<List<Tasks_TraceRoute>>(jsonData);
                return trList == null ? new List<Tasks_TraceRoute>() : trList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return new List<Tasks_TraceRoute>();
        }
    }

    public class MarkerLatLng
    {
        public MarkerLatLng() { }

        public MarkerLatLng(decimal lat, decimal lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }

    public class LatLng
    {
        public LatLng() { }

        public LatLng(string lat, string lng)
        {
            this.latitude = lat;
            this.longitude = lng;
        }

        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class PolylineOptions
    {
        public PolylineOptions() { }

        /**
         * Indicates whether this Polyline handles mouse events. Defaults to true.
         */
        public bool clickable { get; set; } = false;
        /**
         * If set to true, the user can drag this shape over the map.
         * The geodesic property defines the mode of dragging. Defaults to false.
         */
        public bool draggable { get; set; } = false;
        /**
         * If set to true, the user can edit this shape by dragging the control
         * points shown at the vertices and on each segment. Defaults to false.
         */
        public bool editable { get; set; } = false;
        /**
         * When true, edges of the polygon are interpreted as geodesic and will
         * follow the curvature of the Earth. When false, edges of the polygon are
         * rendered as straight lines in screen space. Note that the shape of a
         * geodesic polygon may appear to change when dragged, as the dimensions are
         * maintained relative to the surface of the earth. Defaults to false.
         */
        public bool geodesic { get; set; } = true;
        /**
         * The ordered sequence of coordinates of the Polyline.
         * This path may be specified using either a simple array of LatLngs, or an
         * MVCArray of LatLngs. Note that if you pass a simple array, it will be
         * converted to an MVCArray Inserting or removing LatLngs in the MVCArray
         * will automatically update the polyline on the map.
         */
        public List<MarkerLatLng> path { get; set; } = new List<MarkerLatLng>();
        /**
         * The stroke color. All CSS3 colors are supported except for extended
         * named colors.
         */
        public string strokeColor { get; set; } = "#dc3545";
        /** The stroke opacity between 0.0 and 1.0. */
        public decimal strokeOpacity { get; set; } = 1;
        /** The stroke width in pixels. */
        public decimal strokeWeight { get; set; } = 2;
        /** Whether this polyline is visible on the map. Defaults to true. */
        public bool visible { get; set; } = true;
        /** The zIndex compared to other polys. */
        public decimal zIndex { get; set; } = 1;
    }
}

