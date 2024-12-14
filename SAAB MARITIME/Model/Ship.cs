namespace SAAB_Maritime.Model
{
    internal class Ship : Vessel 
    {

        VesselTrackingCalculator vtc;

        private int _ID;
        private int _MMSI;
        private string _radio_Callsign;



        private List<Position> _positions;

        private Position _currentPos;

        private string _name;
        private string _model;
        private string _startdestination;
        private string _enddestination;

        private DateTime _startDate;
        private DateTime _lastDataDate;
        private string _ETA;

        private bool _needNewList = false;

        private bool arrival = false;


        public Ship(int id, int mmsi, string radio, List<Position> positions, string name, string model, DateTime startdate, string eta, string start, string end)
        {
            _ID = id;
            _MMSI = mmsi;
            _radio_Callsign = radio;
            _positions = positions;

            _positions.Sort(); // sort the data
            _currentPos = positions[0];
            _positions.RemoveAt(0); // removing the first index

            _name = name;
            _model = model;
            _startDate = startdate;
            _ETA = eta;
            _startdestination = start;
            _enddestination = end;
            vtc = new VesselTrackingCalculator();
            _lastDataDate = DateTime.Now; // feature
        }

        //calculates and updates ship position
        public Position Update()
        {
            if (_positions != null)
            {
                if (_positions.Count > 0)
                {
                    _positions.Sort();
                    _currentPos = VesselTrackingCalculator.NextPosition(_currentPos, _positions[0]);
                    _positions.RemoveAt(0);
                }
                else
                {
                    _needNewList = true;
                }
            }
            return _currentPos;
        }

        //returns true if ships need to get a new position list.
        public bool needPos()
        {
            return _needNewList;
        }

        //sets _needNewList to false.
        public void noNeed()
        {
            _needNewList = false;
        }

        public void setNextPositions(List<Position> l)
        {
            _positions = l;
        }

        public Position CurrentPos() { return _currentPos; }
        public int MMSI(){ return _MMSI;}
        public string RadioCallsign() { return _radio_Callsign; }
        public string VesselName() { return _name; }
        public string VesselModel(){ return _model; }
        public string StartDestination(){ return _startdestination; }
        public string EndDestination(){ return _enddestination; }
        public string ETA(){ return _ETA; }
        public DateTime StartDate(){ return _startDate; }
        public DateTime LastDataDate() { return _lastDataDate; }

        public bool GetArrival { get { return arrival; } set {  arrival = value; } }

        public int CompareTo(Vessel other)
        {
            return this._currentPos.GetDateTime().CompareTo(other.CurrentPos().GetDateTime());
        }


    }
}
