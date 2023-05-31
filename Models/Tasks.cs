using System;
namespace Nodes64.Models
{
    public class Tasks_ICMP
    {
        public Tasks_ICMP() { }

        public int id { get; set; }
        public string task_id { get; set; }
        public string task_icmp_id { get; set; }
        public string task_type { get; set; }
        public string src_node { get; set; }
        public string dst_node { get; set; }
        public string dst_ip { get; set; }
        public string latitude { get; set; } = "";
        public string longitude { get; set; } = "";
        public string node_name { get; set; } = "";
        public bool node_name_public { get; set; } = false;
        public decimal rtt_avg { get; set; }
        public decimal rtt_min { get; set; }
        public decimal rtt_max { get; set; }
        public decimal jitter { get; set; }
        public decimal packet_loss { get; set; }
        public DateTime report_time { get; set; }
    }

    public class Tasks_TraceRoute
    {
        public Tasks_TraceRoute() { }

        public int id { get; set; }
        public string task_id { get; set; }
        public string task_type { get; set; }
        public string task_traceroute_id { get; set; }
        public string src_node { get; set; }
        public string dst_node { get; set; }
        public string dst_ip { get; set; }
        public string trace_info { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string node_name { get; set; } = "";
        public bool node_name_public { get; set; } = false;
        public int hops { get; set; }
        public decimal rtt_avg { get; set; }
        public decimal rtt_min { get; set; }
        public decimal rtt_max { get; set; }
        public decimal jitter { get; set; }
        public decimal packet_loss { get; set; }
        public DateTime report_time { get; set; }
    }
}

