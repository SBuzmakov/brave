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
            int gameGroundHeigth = 4;
            int playerPositionY = gameScreenHeigth - gameGroundHeigth - 1;
            int playerPositionX = 10;
            int displayPositionX = 0;
            int displayPositionY = 0;
            char[,] map = CreateObjects(gameScreenLength, gameScreenHeigth, gameGroundHeigth);



            while (isPlaying)
            {

                DrawObjects(map, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayPositionX, displayPositionY);

                ConsoleKeyInfo charKey = Console.ReadKey();

                switch (charKey.Key)
                {
                    case ConsoleKey.RightArrow:
                        map = GenerateNewObjects(map, gameGroundHeigth, playerPositionX, playerPositionY);
                        break;
                    case ConsoleKey.Enter:
                        Shoot(ref map, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth,displayPositionX, displayPositionY);
                        break;
                    case ConsoleKey.Spacebar:
                        Jump(ref map, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY);
                        break;
                    default:
                        break;
                }               
            }           
        }

        static char[,] GenerateNewObjects(char [,] objects, int ground, int playerPositionX, int playerPositionY)
        {
            if (objects[playerPositionY, playerPositionX + 1] != '#')
            {
                Random random = new Random();
                int erasingXNumber = 1;

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
                    objects[y, x] = 'M';
                }
            }

            return objects;
        }
        
        static void DrawObjects(char[,] objects,int length, int heigth, int ground, ref int playerPositionX, ref int playerPositionY, int displayX, int displayY)
        {
            System.Threading.Thread.Sleep(0);


            Console.SetCursorPosition(displayX, displayY);

            SetPlayerPosition(ref objects, playerPositionX, playerPositionY);

            Console.BackgroundColor = ConsoleColor.Blue;

            for (int y = 0; y < objects.GetLength(0) - ground; y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    Console.Write(objects[y, x]);
                }

                Console.WriteLine();
            }

            Console.BackgroundColor = ConsoleColor.DarkGreen;

            for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    Console.Write(objects[y, x]);
                }

                Console.WriteLine();
            }

            Console.ResetColor(); 
            
        }

        static void SetPlayerPosition(ref char[,] objects, int positonX, int positionY)
        {
            objects[positionY, positonX] = '>';
        }

        static void Shoot(ref char[,] objects, int positionX, int positionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY)
        {
            int projectileRange = 20;
            int projectileX = positionX + 1;
            objects[positionY, projectileX] = '=';

            for (int x = 0; x < projectileRange; x++)
            {
                if (objects[positionY, projectileX] == '#')
                {
                    objects[positionY, projectileX] = ' ';
                    break;
                }

                objects[positionY, projectileX] = '=';
                objects[positionY, projectileX - 1] = ' ';
                projectileX++;

                DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref positionX, ref positionY, displayX, displayY);
              
            }

            objects[positionY, projectileX - 1] = ' ';
        }

        static void Jump(ref char[,] objects, int positionX, int positionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY)
        {
            int heigthJump = 5;

            for(int y = 0; y < heigthJump; y++)
            {
                System.Threading.Thread.Sleep(50);

                if (objects[positionY - 1, positionX] == ' ')
                {
                    positionY--;


                    objects[positionY + 1, positionX] = ' ';


                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref positionX, ref positionY, displayX, displayY);
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:

                            Shoot(ref objects, positionX, positionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY);

                            break;
                        case ConsoleKey.RightArrow:

                            objects = GenerateNewObjects(objects, gameGroundHeigth, positionX, positionY);

                            break;
                        default:
                            break;
                    }
                }
            }

            for (int y = 0; y < heigthJump; y++)
            {
                System.Threading.Thread.Sleep(50);

                if (objects[positionY + 1, positionX] == ' ')
                {
                    positionY++;


                    objects[positionY - 1, positionX] = ' ';


                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref positionX, ref positionY, displayX, displayY);
                }


                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:

                            Shoot(ref objects, positionX, positionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY);

                            break;
                        case ConsoleKey.RightArrow:

                            objects = GenerateNewObjects(objects, gameGroundHeigth, positionX, positionY);

                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
