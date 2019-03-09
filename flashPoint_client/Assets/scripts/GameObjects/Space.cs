

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
	private List<Space> neighbours = new List<Space>();

    public Space(int id, SpaceKind kind, int x, int y, SpaceStatus status)
    {
        this.id = id;
        this.kind = kind;
        this.xPOS = x;
        this.yPOS = y;
        this.status = status;
    }

	public void addNeighbours(List<Space> in_neighbours){
		neighbours = in_neighbours;
	}

	//more method in M5 to be added


}
