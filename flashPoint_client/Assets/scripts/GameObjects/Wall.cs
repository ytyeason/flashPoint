

public class Wall:EdgeObstacle
{
    private WallStatus status;

    public Wall (int id):base(id)
    {
        this.id = id;
    }

    public void SetStatus(WallStatus s)
    {
        status = s;
    }

    public WallStatus GetStatus()
    {
        return status;
    }
}
