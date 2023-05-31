using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nodes64.Models;
using Nodes64Api.Functions;
using Nodes64Api.Models;

namespace Nodes64Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NodeController : ControllerBase
    {
        private ConnectionStrings connectionString;

        public NodeController(IOptions<ConnectionStrings> connString)
        {
            connectionString = connString.Value;
        }

        [HttpGet("{ttype}/{nodeId}/{hours}")]
        public async Task<IActionResult> GetICMPNodes(string ttype, string nodeId, int hours)
        {
            if (nodeId == null || nodeId.Length == 0 || ttype.Length == 0 || ttype == null || hours <= 0)
                return BadRequest();

            List<Marker>? markerList = new List<Marker>();
            Node node = new Node(connectionString);

            List<Node>? nl = await node.GetNodes(nodeId);
            if (nl == null || nl.Count == 0)
                return null;

            Node fromList = nl.ElementAt(0);

            node.id = fromList.id;
            node.latitude = fromList.latitude;
            node.longitude = fromList.longitude;
            node.node_id = fromList.node_id;
            node.node_name = fromList.node_name;
            node.node_name_public = fromList.node_name_public;
            markerList = await node.GetAllNodesAsMarker(ttype, nodeId, hours);

            if (markerList == null)
            {
                return BadRequest("Return Null");
            }

            return Ok(markerList);
        }

        [HttpGet("{ttype}/{srcdst}")]
        public async Task<IActionResult> GetTopTalkerNodes(string ttype, string srcdst)
        {
            if (srcdst == null || srcdst.Length == 0 || ttype.Length == 0 || ttype == null)
                return BadRequest();

            List<Talker>? talkers = new List<Talker>();

            Node node = new Node(connectionString);

            talkers = await node.GetTop10Talker(ttype, srcdst);

            if (talkers == null)
            {
                return BadRequest("Return Null");
            }

            return Ok(talkers);
        }

        [HttpGet("{nodeId}")]
        public async Task<IActionResult> GetSearchNode(string nodeId)
        {
            if (nodeId == null || nodeId.Length == 0)
                return BadRequest();

            List<Node>? nodes = new List<Node>();

            Node node = new Node(connectionString);

            nodes = await node.GetNodes(nodeId);

            foreach (var item in nodes)
            {
                item.node_name = System.Net.WebUtility.HtmlDecode(item.node_name);
                item.latitude = "";
                item.longitude = "";
            }

            if (nodes == null)
            {
                return BadRequest("Return Null");
            }

            return Ok(nodes);
        }

        [HttpGet("GetAllNodes")]
        public async Task<IActionResult> SelectAllNodes()
        {
            List<Node>? nodes = new List<Node>();
            List<Marker>? markerList = new List<Marker>();

            Calculation c = new Calculation();
            Node node = new Node(connectionString);

            nodes = await node.GetAllNodes();

            foreach (var item in nodes)
            {
                MarkerLatLng mlatlng = new MarkerLatLng();
                MarkerLatLng nodeOriginal = c.GetOriginalLatLng(item.latitude, item.longitude);
                mlatlng = c.generateRandomPosition(nodeOriginal, 250);

                Marker marker = new Marker();
                marker.MarkerLatLng = mlatlng;
                marker.PolyOption = new List<PolylineOptions>();
                marker.NodeId = item.node_id;
                marker.MarkerIcon = "./assets/node_pin.svg";
                marker.NodeIdName = item.node_name_public ? System.Net.WebUtility.HtmlDecode(item.node_name) : "";
                marker.NodeIdNamePublic = item.node_name_public;
                var nodeName = marker.NodeIdNamePublic ? $"{marker.NodeIdName} ({marker.NodeId})" : marker.NodeId;
                marker.MarkerInfos = $"<div class=\"info-box-row\">{nodeName}</div>";
                markerList.Add(marker);
                item.node_name = System.Net.WebUtility.HtmlDecode(item.node_name);

                item.latitude = mlatlng.lat.ToString().Replace(",", ".");
                item.longitude = mlatlng.lng.ToString().Replace(",", ".");
            }

            if (markerList == null)
            {
                return BadRequest("Return Null");
            }

            return Ok(markerList);
        }
    }
}

