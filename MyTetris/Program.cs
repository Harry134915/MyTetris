//namespace MyTetris
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("This is my Tetris!");
//        }
//    }
//}

using System;
using System.Threading;

class Tetris
{
    static int width = 10;       // 游戏宽度
    static int height = 20;      // 游戏高度
    static int[,] field = new int[height, width]; // 游戏区域

    // 七种俄罗斯方块
    static int[][,] tetrominoes = new int[][,]
    {
        new int[,] { {1,1,1,1} },                 // I
        new int[,] { {1,1},{1,1} },               // O
        new int[,] { {0,1,0},{1,1,1} },           // T
        new int[,] { {1,0,0},{1,1,1} },           // L
        new int[,] { {0,0,1},{1,1,1} },           // J
        new int[,] { {0,1,1},{1,1,0} },           // S
        new int[,] { {1,1,0},{0,1,1} },           // Z
    };

    static int[,] current;
    static int posX, posY;
    static Random rand = new Random();

    static void Main()
    {
        Console.CursorVisible = false;
        NewPiece();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.LeftArrow) Move(-1, 0);
                if (key == ConsoleKey.RightArrow) Move(1, 0);
                if (key == ConsoleKey.DownArrow) Drop();
                if (key == ConsoleKey.UpArrow) Rotate();
            }

            if (!Move(0, 1))  // 方块下移失败 → 固定
            {
                MergePiece();
                ClearLines();
                NewPiece();
                if (!IsValidMove(posX, posY, current))  // 游戏结束
                {
                    Console.Clear();
                    Console.WriteLine("Game Over!");
                    break;
                }
            }

            Draw();
            Thread.Sleep(300);
        }
    }

    static void NewPiece()
    {
        current = tetrominoes[rand.Next(tetrominoes.Length)];
        posX = width / 2 - current.GetLength(1) / 2;
        posY = 0;
    }

    static bool Move(int dx, int dy)
    {
        if (IsValidMove(posX + dx, posY + dy, current))
        {
            posX += dx;
            posY += dy;
            return true;
        }
        return false;
    }

    static void Drop()
    {
        while (Move(0, 1)) { }
    }

    static void Rotate()
    {
        int w = current.GetLength(1);
        int h = current.GetLength(0);
        int[,] rotated = new int[w, h];

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                rotated[x, h - y - 1] = current[y, x];

        if (IsValidMove(posX, posY, rotated))
            current = rotated;
    }

    static bool IsValidMove(int newX, int newY, int[,] shape)
    {
        int h = shape.GetLength(0);
        int w = shape.GetLength(1);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (shape[y, x] == 0) continue;
                int fx = newX + x;
                int fy = newY + y;
                if (fx < 0 || fx >= width || fy < 0 || fy >= height) return false;
                if (field[fy, fx] != 0) return false;
            }
        }
        return true;
    }

    static void MergePiece()
    {
        int h = current.GetLength(0);
        int w = current.GetLength(1);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (current[y, x] != 0)
                    field[posY + y, posX + x] = 1;
    }

    static void ClearLines()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            bool full = true;
            for (int x = 0; x < width; x++)
                if (field[y, x] == 0) { full = false; break; }

            if (full)
            {
                for (int ny = y; ny > 0; ny--)
                    for (int x = 0; x < width; x++)
                        field[ny, x] = field[ny - 1, x];
                for (int x = 0; x < width; x++) field[0, x] = 0;
                y++; // 重新检查这一行
            }
        }
    }

    static void Draw()
    {
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool occupied = field[y, x] != 0;
                // 临时画上当前方块
                int relY = y - posY, relX = x - posX;
                if (relY >= 0 && relY < current.GetLength(0) &&
                    relX >= 0 && relX < current.GetLength(1) &&
                    current[relY, relX] != 0)
                    occupied = true;

                Console.Write(occupied ? "■" : "  ");
            }
            Console.WriteLine();
        }
    }
}

