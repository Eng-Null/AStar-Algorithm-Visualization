namespace WPF.Enum;

public enum AStarSet
{
    Start = 0,
    End = 1,
    Undefined = 4,
    Path = 5,
    Obstacle = 6,
    Maze = 7,

    //Road Tiles
    RoadH = 12,

    RoadV = 13,

    RoadTL = 14,
    RoadTR = 15,
    RoadBL = 16,
    RoadBR = 17,

    RoadBLF = 18,
    RoadTBL = 19,
    RoadTBR = 20,
    RoadTLR = 21,

    RoadTBLR = 22,

    RoadT = 23,
    RoadB = 24,
    RoadL = 25,
    RoadR = 26,

    //River Tiles
    RiverH = 27,

    RiverV = 28,

    RiverTL = 29,
    RiverTR = 30,
    RiverBL = 31,
    RiverBR = 32,
}