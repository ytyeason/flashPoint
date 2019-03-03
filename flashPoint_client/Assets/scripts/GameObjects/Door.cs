

public class Door : EdgeObstacle
{
    private DoorStatus status;
    public Door(int id) : base(id)
    {
        
    }
    
    public void SetStatus(DoorStatus s)
    {
        status = s;
    }

    public DoorStatus GetStatus()
    {
        return status;
    }
}
