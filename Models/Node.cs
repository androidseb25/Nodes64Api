using System;
using Newtonsoft.Json;
using Nodes64Api.Functions;
using Nodes64Api.Models;
using Nodes64Api.Models.Database;

namespace Nodes64.Models
{
    public class Node : DBBase
    {
        public Node(ConnectionStrings connStrings) : base(connStrings) { }
        public Node() { }

        public int id { get; set; }
        public string node_id { get; set; }
        public string node_name { get; set; }
        public bool node_name_public { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int trCount { get; set; }
        public int icmpCount { get; set; }

        public async Task<List<Marker>?> GetAllNodesAsMarker(string ttype, string nodeId, int hours)
        {
            decimal defaultMeterRange = 250;
            List<Marker> markerList = new List<Marker>();
            Marker marker = new Marker(ConnString!);
            Calculation c = new Calculation();

            LatLng? myNodeLatLng = new LatLng(this.latitude, this.longitude);

            if (myNodeLatLng == null)
                return null;

            MarkerLatLng mlatlng = new MarkerLatLng();
            MarkerLatLng nodeOriginal = c.GetOriginalLatLng(myNodeLatLng.latitude, myNodeLatLng.longitude);

            mlatlng = c.generateRandomPosition(nodeOriginal, defaultMeterRange);

            marker.MarkerLatLng = mlatlng;
            marker.PolyOption = new List<PolylineOptions>();
            marker.NodeId = nodeId;
            marker.NodeIdName = this.node_name_public ? System.Net.WebUtility.HtmlDecode(this.node_name) : "";
            marker.NodeIdNamePublic = this.node_name_public;
            var nodeName = marker.NodeIdNamePublic ? $"{marker.NodeIdName} ({marker.NodeId})"  : marker.NodeId;
            marker.MarkerInfos = $"<div class=\"info-box-row\">Node: {nodeName}</div>";
            markerList.Add(marker);

            if (ttype == "icmpv4" || ttype == "icmpv6")
            {
                List<Tasks_ICMP> icmpList = await marker.GetAllICMPTask(ttype, nodeId, hours);

                foreach (Tasks_ICMP icmp in icmpList)
                {
                    PolylineOptions poly = new PolylineOptions();
                    poly.path.Add(mlatlng);

                    MarkerLatLng pathlatlng = new MarkerLatLng();
                    MarkerLatLng original = c.GetOriginalLatLng(icmp.latitude, icmp.longitude);

                    pathlatlng = c.generateRandomPosition(original, defaultMeterRange);

                    poly.path.Add(pathlatlng);
                    ThresholdsFunction thf = new ThresholdsFunction();
                    Thresholds th = thf.GetThreadhold(icmp.rtt_avg);
                    Thresholds thPKGLss = thf.GetThreadholdPackageLoss(icmp.packet_loss);
                    poly.strokeColor = thf.Color(th);
                    poly.strokeOpacity = (decimal)0.8;
                    poly.geodesic = true;

                    marker.PolyOption.Add(poly);

                    Marker mEndPoint = new Marker();
                    mEndPoint.MarkerLatLng = pathlatlng;
                    mEndPoint.MarkerInfos =
                        $"<div class=\"info-box-row\">Distanz zwischen den Nodes: <b>{c.DistanceBetween(mlatlng, pathlatlng)}km</b></div>" +
                        $"<div class=\"info-box-row\">Avg RTT: <b>{Math.Round(icmp.rtt_avg, 2)}ms</b></div>" +
                        $"<div class=\"info-box-row\">Packetlost: <b class=\"{thPKGLss}\">{Math.Round(icmp.packet_loss * 100, 0)}%</b></div>" +
                        $"<div class=\"info-box-row\">Status: <b class=\"{th}\">{thf.Text(th)}</b></div>" +
                        $"<div class=\"info-box-row\">Zeit: <b>{icmp.report_time.ToString("dd.MM.yyyy HH:mm:ss")}</b></div>" +
                        $"<div class=\"info-box-row\"></div>";
                    mEndPoint.MarkerInfosMS = $"<div class=\"info-box-row\">Avg RTT: <b>{Math.Round(icmp.rtt_avg, 2)}ms</b></div>";
                    mEndPoint.MarkerIcon = "./assets/node_pin.svg";
                    mEndPoint.NodeId = icmp.src_node;
                    mEndPoint.PolyOption = new List<PolylineOptions>();
                    PolylineOptions polyReversed = new PolylineOptions();
                    polyReversed.path.Add(pathlatlng);
                    polyReversed.path.Add(mlatlng);
                    polyReversed.strokeColor = thf.Color(th);
                    polyReversed.strokeOpacity = (decimal)0.8;
                    polyReversed.geodesic = true;
                    mEndPoint.PolyOption.Add(polyReversed);
                    markerList.Add(mEndPoint);
                }
            }
            else
            {
                List<Tasks_TraceRoute> trList = await marker.GetAllTracerouteTask(ttype, nodeId, hours);

                foreach (Tasks_TraceRoute tr in trList)
                {
                    PolylineOptions poly = new PolylineOptions();
                    poly.path.Add(mlatlng);

                    MarkerLatLng pathlatlng = new MarkerLatLng();
                    MarkerLatLng original = c.GetOriginalLatLng(tr.latitude, tr.longitude);

                    pathlatlng = c.generateRandomPosition(original, defaultMeterRange);
                    poly.path.Add(pathlatlng);
                    ThresholdsFunction thf = new ThresholdsFunction();
                    Thresholds th = thf.GetThreadhold(tr.rtt_avg);
                    Thresholds thPKGLss = thf.GetThreadholdPackageLoss(tr.packet_loss);
                    poly.strokeColor = thf.Color(th);
                    poly.strokeOpacity = (decimal)0.8;
                    poly.geodesic = true;

                    marker.PolyOption.Add(poly);

                    Marker mEndPoint = new Marker();
                    mEndPoint.MarkerLatLng = pathlatlng;
                    mEndPoint.MarkerInfos =
                        $"<div class=\"info-box-row\">Distanz zwischen den Nodes: <b>{c.DistanceBetween(mlatlng, pathlatlng)}km</b></div>" +
                        $"<div class=\"info-box-row\">Avg RTT: <b>{Math.Round(tr.rtt_avg, 2)}ms</b></div>" +
                        $"<div class=\"info-box-row\">Hops: <b>{tr.hops}</b></div>" +
                        $"<div class=\"info-box-row\">Packetlost: <b class=\"{thPKGLss}\">{Math.Round(tr.packet_loss * 100, 0)}%</b></div>" +
                        $"<div class=\"info-box-row\">Status: <b class=\"{th}\">{thf.Text(th)}</b></div>" +
                        $"<div class=\"info-box-row\">Zeit: <b>{tr.report_time.ToString("dd.MM.yyyy HH:mm:ss")}</b></div>";
                    mEndPoint.MarkerInfosMS = $"<div class=\"info-box-row\">Avg RTT: <b>{Math.Round(tr.rtt_avg, 2)}ms</b></div>";
                    mEndPoint.MarkerIcon = "./assets/node_pin.svg";
                    mEndPoint.NodeId = tr.src_node;
                    mEndPoint.PolyOption = new List<PolylineOptions>();
                    PolylineOptions polyReversed = new PolylineOptions();
                    polyReversed.path.Add(pathlatlng);
                    polyReversed.path.Add(mlatlng);
                    polyReversed.strokeColor = thf.Color(th);
                    polyReversed.strokeOpacity = (decimal)0.8;
                    polyReversed.geodesic = true;
                    mEndPoint.PolyOption.Add(polyReversed);
                    markerList.Add(mEndPoint);
                }
            }
            return markerList;
        }

        public async Task<List<Talker>?> GetTop10Talker(string ttype, string srcdstnode, int showLast24 = 0)
        {
            try
            {
                string table = ttype == "traceroute" ? "tasks_traceroute" : "tasks_icmp";
                string srcdst = srcdstnode == "src" ? "src_node" : "dst_node";
                string last24h = showLast24 == 1 ? "and tasks.report_time >= DATE_SUB(NOW(),INTERVAL 24 HOUR)" : "";

                string sql = @$"select COUNT(*) as taskcount, tasks.{srcdst} as node, task_type as type, n.node_name, n.node_name_public from {table} tasks
left join nodes n on n.node_id = tasks.{srcdst}
where
CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0
{last24h}
group by tasks.{srcdst}, type
order by taskcount DESC
LIMIT 10 ";
                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<Talker> talkers = JsonConvert.DeserializeObject<List<Talker>>(jsonData);

                foreach (var item in talkers)
                {
                    item.node_name = System.Net.WebUtility.HtmlDecode(item.node_name);
                }

                return talkers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public async Task<List<Node>?> GetNodes(string nodeId)
        {
            try
            {
                string sql = @$"select * from nodes n
where (n.node_id LIKE '{nodeId}%' or case when n.node_name_public > 0 then n.node_name LIKE '%{nodeId}%' end) and 
CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0";

                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<Node> nodes = JsonConvert.DeserializeObject<List<Node>>(jsonData);
                return nodes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public async Task<List<Node>?> GetAllNodes()
        {
            try
            {
//                string sql = @$"SELECT *, (
//	SELECT Count(*) FROM tasks_icmp ti WHERE ti.src_node = n.node_id
//) as icmpCount, (
//	SELECT Count(*) FROM tasks_traceroute tr WHERE tr.src_node = n.node_id
//) as trCount
//from nodes n where 
//CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0
//having 
//trCount > 0 and icmpCount > 0";

                string sql = $@"SELECT *, (
    SELECT Count(*) FROM tasks_icmp ti WHERE ti.src_node = n.node_id and ti.report_time >= DATE_SUB(NOW(),INTERVAL 6 HOUR)
) as icmpCount, (
    SELECT Count(*) FROM tasks_traceroute tr WHERE tr.src_node = n.node_id and tr.report_time >= DATE_SUB(NOW(),INTERVAL 6 HOUR)
) as trCount
from nodes n where 
CHAR_LENGTH(n.latitude) > 0 and CHAR_LENGTH(n.longitude) > 0
having 
trCount >= 15 and icmpCount >= 30";

                dynamic dTasks = await SelectFromSql(sql);
                var jsonData = JsonConvert.SerializeObject(dTasks);
                List<Node> nodes = JsonConvert.DeserializeObject<List<Node>>(jsonData);
                return nodes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}

