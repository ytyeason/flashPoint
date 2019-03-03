

using System.Collections.Generic;

public class Space
{
    public int id;
    public SpaceKind kind;
    public int xPOS;
    public int yPOS;
    public SpaceStatus status;
    
    private Dictionary<int, Wall> WallList = new Dictionary<int, Wall>();
    private Dictionary<int, Door> DoorList = new Dictionary<int, Door>();

    public Space(int id, SpaceKind kind, int x, int y, SpaceStatus status)
    {
        this.id = id;
        this.kind = kind;
        this.xPOS = x;
        this.yPOS = y;
        this.status = status;
    }
    
    //more method in M5 to be added
    
    
}
