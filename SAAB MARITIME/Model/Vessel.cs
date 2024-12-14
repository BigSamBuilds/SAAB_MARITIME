namespace SAAB_Maritime.Model
{
    interface Vessel : IComparable<Vessel>
    {
        Position Update();
        void setNextPositions(List<Position> l);
        bool needPos();
        void noNeed();
        int MMSI();
        string RadioCallsign();
        string VesselName();
        string VesselModel();
        DateTime LastDataDate(); // feature
        string StartDestination(); // feature
        string EndDestination(); // feature
        Position CurrentPos();
        string ETA();
        DateTime StartDate();
        new int CompareTo(Vessel other);
        bool GetArrival { get; set; }

    }
}
