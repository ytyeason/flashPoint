
public class Victim: POI
{
    public VictimStatus status;
    
    public Victim()
    {
        status = VictimStatus.Lost;//temp initialize
    }

    public void markRescued()
    {
        status = VictimStatus.Rescued;
    }
    
}
