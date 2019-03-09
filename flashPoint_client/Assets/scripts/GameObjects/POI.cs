
public class POI : GameUnit
{
    public POIStatus stat;
	public POIKind type;

    public POI()
    {
        
    }

    public bool reveal()
    {
        return false;
    }

    public bool removeFromBoard()
    {
        return false;
    }
}
