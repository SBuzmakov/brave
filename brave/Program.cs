using System;

namespace brave
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            bool isPlaying = true;
            int gameScreenLength = 50;
            int gameScreenHeigth = 20;
            int gameGroundHeigth = 3;
            int playerPositionY = gameScreenHeigth - gameGroundHeigth - 1;
            int playerPositionX = 10;
            char[,] map = CreateObjects(gameScreenLength, gameScreenHeigth, gameGroundHeigth);



            while (isPlaying)
            {
                DrawObjects(map, gameScreenLength, gameScreenHeigth, gameGroundHeigth, playerPositionX, playerPositionY);

                ConsoleKeyInfo charKey = Console.ReadKey();

                switch (charKey.Key)
                {
                    case ConsoleKey.D:
                        map = GenerateNewObjects(map, gameGroundHeigth, playerPositionX, playerPositionY);
                        break;
                    case ConsoleKey.Enter:
                        Shoot(map, playerPositionX, playerPositionY);
                        break;
                }
                
                Console.Clear();
            }           
        }

        static char[,] GenerateNewObjects(char [,] objects, int ground, int playerPositionX, int playerPositionY)
        {
            if (objects[playerPositionY, playerPositionX + 1] != '#')
            {
                Random random = new Random();
                int erasingXNumber = 2;

                for (int y = 0; y < objects.GetLength(0) - ground; y++)
                {
                    for (int x = 0; x < objects.GetLength(1) - 1; x++)
                    {
                        objects[y, x] = objects[y, x + 1];
                    }
                }

                for (int y = 0; y < objects.GetLength(0) - ground; y++)
                {
                    for (int x = objects.GetLength(1) - 1; x < objects.GetLength(1); x++)
                    {
                        int randomCreatedObject = random.Next(0, 2);
                        objects[y, x] = ' ';

                        if (randomCreatedObject == 1)
                        {
                            objects[y, x] = '#';
                        }
                    }
                }

                objects[playerPositionY, playerPositionX - erasingXNumber] = ' ';
            }

            return objects;
        }

        static char[,] CreateObjects(int length, int heigth, int ground)
        {
            char[,] objects = new char[heigth, length];

            for (int y = 0; y < objects.GetLength(0) - ground; y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    objects[y, x] = ' ';
                }
            }

            for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    objects[y, x] = '#';
                }

                Console.WriteLine();
            }

            return objects;
        }
        
        static void DrawObjects(char[,] objects,int length, int heigth, int ground, int playerPositionX, int playerPositionY)
        {
            Console.BackgroundColor = ConsoleColor.Blue;

            for (int y = 0; y < objects.GetLength(0) - ground; y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    Console.Write(objects[y, x]);
                }

                Console.WriteLine();
            }

            Console.BackgroundColor = ConsoleColor.DarkGray;

            for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    Console.Write(objects[y, x]);
                }

                Console.WriteLine();
            }

            Console.ResetColor(); 
            
            DrawPlayer(objects, playerPositionX, playerPositionY);
        }

        static void DrawPlayer(char[,] objects, int positonX, int positionY)
        {
            objects[positionY, positonX] = '>';
        }

        static void Shoot(char[,] objects, int positionX, int positionY)
        {

        }
    }
}
