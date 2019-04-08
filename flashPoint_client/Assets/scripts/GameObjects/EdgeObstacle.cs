using System;

[Serializable]
public abstract class EdgeObstacle
{
    public int id;

    public EdgeObstacle(int id)
    {
        this.id = id;
    }
}