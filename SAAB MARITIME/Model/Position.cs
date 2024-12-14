namespace SAAB_Maritime.Model
{
    internal class Position : IComparable<Position>
    {
        private double _longitude;
        private double _latitude;
        private DateTime _timeStamp;
        private double _course;
        private double _speed;

        public Position(double longitude, double latitude, DateTime timeStamp, double course, double speed)
        {
            _longitude = longitude;
            _latitude = latitude;
            _timeStamp = timeStamp;
            _course = course;
            _speed = speed;
        }

        public double GetLongitude() {  return _longitude; }
        public double GetLatitude() { return _latitude; }
        public DateTime GetDateTime() { return _timeStamp; }

        public double GetCourse() { return _course; }
        public double GetSpeed() { return _speed; }

        public int CompareTo(Position other)
        {
            return this.GetDateTime().CompareTo(other.GetDateTime());
        }

    }
}
