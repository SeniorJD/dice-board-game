﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRectangle {
    private int x;
    private int y;
    private int width;
    private int height;
    private int x2;
    private int y2;

    private GridPoint[] points;

    public GridRectangle(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;

        this.x2 = x + width;
        this.y2 = y - height;

        this.points = new GridPoint[width * height];
        FillPoints();
    }

    private void FillPoints()
    {
        int index = 0;
        for (int i = x; i < x2; i++)
        {
            for (int j = y; j > y2; j--)
            {
                points[index] = new GridPoint(i, j);
                index++;
            }
        }
    }

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }
    }

    public int Width
    {
        get
        {
            return width;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
    }

    public int X2
    {
        get
        {
            return x2;
        }
    }

    public int Y2
    {
        get
        {
            return y2;
        }
    }

    public bool Intersects(GridRectangle r2)
    {
        foreach(GridPoint p1 in points)
        {
            foreach (GridPoint p2 in r2.points)
            {
                if (p1.X == p2.X && p1.Y == p2.Y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool Aligns(GridRectangle r2)
    {
        foreach (GridPoint p1 in points)
        {
            foreach (GridPoint p2 in r2.points)
            {
                if (p1.Y == p2.Y)
                {
                    if (p1.X == p2.X + 1 || p1.X == p2.X - 1)
                    {
                        return true;
                    }
                } else if (p1.X == p2.X)
                {
                    if (p1.Y == p2.Y + 1 || p1.Y == p2.Y - 1)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool Contains(GridPoint point)
    {
        return x <= point.X && x2 > point.X && y >= point.Y && y2 < point.Y;
    }

    public int GetSquare()
    {
        return points.Length;
    }

    public override string ToString()
    {
        return base.ToString() + ": {" + X + ", " + Y + ", " + X2 + ", " + Y2 + "} : " + Width + "x" + Height;
    }

    public int GetTileIndexFor(int i, int j)
    {
        if (width == 1)
        {
            if (height == 1)
            {
                return 15;
            }
            if (j == y)
            {
                return 6;
            }
            if (j - 1 == y2)
            {
                return 9;
            }
            return 4;
        }
        if (height == 1)
        {
            if (i == x)
            {
                return 2;
            }
            if (i + 1 == x2)
            {
                return 13;
            }
            return 11;
        }

        if (i == x)
        {
            if (j == y)
            {
                return 5;
            }
            if (j - 1 == y2)
            {
                return 1;
            }
            return 3;
        }
        if (i + 1 == x2)
        {
            if (j == y)
            {
                return 12;
            }
            if (j - 1 == y2)
            {
                return 8;
            }
            return 7;
        }
        if (j == y)
        {
            return 10;
        }
        if (j - 1 == y2)
        {
            return 0;
        }
        return 14;
    }
    public class GridPoint
    {
        int x, y;

        public GridPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }
    }
}
