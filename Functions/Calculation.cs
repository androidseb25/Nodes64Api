using GeoCoordinatePortable;
using Nodes64Api.Models;

namespace Nodes64Api.Functions
{
    public class Calculation
    {
        public Calculation() { }

        public decimal DistanceBetween(MarkerLatLng start, MarkerLatLng end)
        {
            var sCoord = new GeoCoordinate((double)start.lat, (double)start.lng);
            var eCoord = new GeoCoordinate((double)end.lat, (double)end.lng);

            return Math.Round((decimal)sCoord.GetDistanceTo(eCoord) / 1000);
        }

        public MarkerLatLng generateRandomPosition(MarkerLatLng center, decimal radius)
        {
            double x0 = (double)center.lng;
            double y0 = (double)center.lat;
            // Convert Radius from meters to degrees.
            double rd = (double)(radius / 111300);

            Random r = new Random();

            double u = r.Next(0, 150);
            double v = r.Next(0, 150);

            var w = rd * Math.Sqrt(u);
            var t = 2 * Math.PI * v;
            var x = w * Math.Cos(t);
            var y = w * Math.Sin(t);

            var xp = x / Math.Cos(y0);

            MarkerLatLng newLatLng = new MarkerLatLng((decimal)(y + y0), (decimal)(xp + x0));

            // Resulting point.
            return newLatLng;
        }

        public MarkerLatLng GetOriginalLatLng(string latitude, string longitude)
        {
            #if DEBUG
                return new MarkerLatLng(decimal.Parse(latitude.Replace(".", ",")), decimal.Parse(longitude.Replace(".", ",")));
            #else
                return new MarkerLatLng(decimal.Parse(latitude), decimal.Parse(longitude));
            #endif
        }
    }
}

