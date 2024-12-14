namespace SAAB_Maritime.Model
{
    internal class VesselTrackingCalculator
    { 
        public static Position NextPosition(Position c, Position n)
        {
            if (n == null) return c;
            Position nextPosition;

            TimeSpan ts = n.GetDateTime().Subtract(c.GetDateTime());
            double interval = ts.TotalSeconds;
            if (interval <= 1) return n;

            double deltaX = (n.GetLongitude() - c.GetLongitude())/interval;
            double deltaY = (n.GetLatitude() - c.GetLatitude())/interval;
            DateTime dateTime = c.GetDateTime();
            nextPosition = new Position(c.GetLongitude() + deltaX, c.GetLatitude() + deltaY, dateTime.AddSeconds(1), c.GetCourse(), c.GetSpeed());
            return nextPosition;
        }
    }
}
