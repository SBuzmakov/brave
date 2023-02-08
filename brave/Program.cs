using System;
using System.Threading;
using System.Text;

namespace brave
{
    class Program
    {       
        static void Main(string[] args)
        {
            bool isPlayingAgain = true;
            Console.CursorVisible = false;

            while (isPlayingAgain)
            {                
                int gameScreenLength = 50;
                int gameScreenHeigth = 20;
                int gameGroundHeigth = 8;
                int playerPositionY = gameScreenHeigth - gameGroundHeigth - 1;
                int playerPositionX = 15;
                int displayPositionX = 0;
                int displayPositionY = 0;
                int lifeValue = 100;
                int lifeValueMax = 100;
                int barValueMax = 20;
                bool canJump = true;
                int scoreValue = 0;
                int chanceGeneratePit = 10;
                int difficultLevel = 1;
                int bulletsValue = 5;
                int createPlatformValue = 1;
                bool isPlaying = true;

                char[,] objects = CreateInitialObjects(gameScreenLength, gameScreenHeigth, gameGroundHeigth);

                while (lifeValue > 0 && isPlaying)
                {

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayPositionX, displayPositionY,
                         barValueMax, ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                    ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX,
                        displayPositionY, barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                         ref createPlatformValue, ref isPlaying);

                    FallWithoutFooting(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY,
                         barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                         ref createPlatformValue, ref isPlaying);

                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Game Over :(");
                Console.ReadKey(true);
                Console.Clear();
                Console.Write("Play again? \tY/N");
                ConsoleKeyInfo charKey = Console.ReadKey(true);

                switch (charKey.Key)
                {
                    case ConsoleKey.Y:
                        isPlayingAgain = true;
                        break;
                    case ConsoleKey.N:
                        isPlayingAgain = false;
                        break;
                    default:
                        break;
                }
            }
        }

        static void GenerateNewObjects(char[,] objects, int ground, int playerPositionX, int playerPositionY, int heigth, ref int lifeValue, int length, int displayX, int displayY,
            int barValueMax, int lifeValueMax, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            int maxPlayerPositionX = 15;
            int chanceGenerateObjectRange = 100;
            int chanceGenerateSharp = 8;
            int chanceGeneratePlatform = 2;
            int chanceExtendPlatform = 90;
            int difficultRiseScore = 150;
            int difficultRisePits = 10;

            scoreValue++;

            if(chanceGeneratePit != 100 && scoreValue / difficultRiseScore > difficultLevel)
            {
                difficultLevel++;
                chanceGeneratePit += difficultRisePits;
            }

            if (playerPositionX == maxPlayerPositionX && objects[playerPositionY, playerPositionX + 1] != 'T' || objects[playerPositionY, playerPositionX + 1] != 'M')
            {
                Random random = new Random();
                int erasingXNumber = 1;
                int randomCreatedPit = random.Next(0, chanceGenerateObjectRange);

                ReactCollision(objects[playerPositionY, playerPositionX + 1], ref lifeValue, ref bulletsValue, ref createPlatformValue);

                for (int y = 0; y < objects.GetLength(0) - ground; y++)
                {
                    for (int x = 0; x < objects.GetLength(1) - 1; x++)
                    {
                        objects[y, x] = objects[y, x + 1];
                    }
                }

                for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
                {
                    for (int x = 0; x < objects.GetLength(1) - 1; x++)
                    {
                        objects[y, x] = objects[y, x + 1];
                    }
                }

                objects[playerPositionY, playerPositionX - erasingXNumber] = ' ';


                for (int y = 0; y < objects.GetLength(0) - ground; y++)
                {
                    for (int x = objects.GetLength(1) - 1; x < objects.GetLength(1); x++)
                    {
                        int randomCreatedObject = random.Next(0, chanceGenerateObjectRange);                       
                        objects[y, x] = ' ';

                        if (chanceGenerateSharp > randomCreatedObject)
                        {
                            objects[y, x] = '#';
                        }

                        if (chanceGeneratePlatform > randomCreatedObject || (objects[y, x - 1] == 'T' && randomCreatedObject < chanceExtendPlatform))
                        {
                            objects[y, x] = 'T';
                        }                        
                    }
                }


                for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
                {
                    for (int x = objects.GetLength(1) - 1; x < objects.GetLength(1); x++)
                    {
                        int randomCreatedObject = random.Next(0, chanceGenerateObjectRange);
                        objects[y, x] = 'M';

                        if (randomCreatedObject < chanceGeneratePit)
                        {
                            objects[y, x] = '~';
                        }
                    }
                }
            }                       
        }

        static void MovePlayerLeft(char[,] objects, ref int playerPositionX, ref int playerPositionY, int screenHeigth, int groundHeigth, ref int lifeValue, ref int bulletsValue,
            ref int createPlatformValue)
        {
            if (playerPositionX > 1 && objects[playerPositionY, playerPositionX - 1] != 'M' && objects[playerPositionY, playerPositionX - 1] != 'T')
            {
                ReactCollision(objects[playerPositionY, playerPositionX - 1], ref lifeValue, ref bulletsValue, ref createPlatformValue);

                playerPositionX--;

                if (playerPositionY > screenHeigth - groundHeigth - 1)
                {
                    objects[playerPositionY, playerPositionX + 1] = '~';
                }
                else
                {
                    objects[playerPositionY, playerPositionX + 1] = ' ';
                }
            }
        }

        static void MovePlayerRight(char[,] objects, int ground, ref int playerPositionX, ref int playerPositionY, int heigth, ref int lifeValue, int length, int displayX,
            int displayY, int barValueMax, int lifeValueMax, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            int maxPlayerPositionX = 15;

            if (playerPositionX < maxPlayerPositionX && objects[playerPositionY, playerPositionX + 1] != 'M' && objects[playerPositionY, playerPositionX + 1] != 'T')
            {
                ReactCollision(objects[playerPositionY, playerPositionX + 1], ref lifeValue, ref bulletsValue, ref createPlatformValue);

                playerPositionX++;

                if (playerPositionY > heigth - ground - 1)
                {
                    objects[playerPositionY, playerPositionX - 1] = '~';
                }
                else
                {
                    objects[playerPositionY, playerPositionX - 1] = ' ';
                }
            }
            
            if(playerPositionX == maxPlayerPositionX && objects[playerPositionY, playerPositionX + 1] != 'M' && objects[playerPositionY, playerPositionX + 1] != 'T')
            {
                GenerateNewObjects(objects, ground, playerPositionX, playerPositionY, heigth, ref lifeValue, length, displayX, displayY, barValueMax, lifeValueMax,
                     ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                if (playerPositionY > heigth - ground - 1)
                {
                    objects[playerPositionY, playerPositionX - 1] = '~';
                }
                else
                {
                    objects[playerPositionY, playerPositionX - 1] = ' ';
                }
            }        
        }

        static char[,] CreateInitialObjects(int length, int heigth, int ground)
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

        static void DrawObjects(char[,] objects, int length, int heigth, int ground, ref int playerPositionX, ref int playerPositionY, int displayX, int displayY, int barValueMax,
            ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {

            DrawInterfacePanel(0, heigth, barValueMax, ref lifeValue, lifeValueMax, ConsoleColor.Red, ConsoleColor.DarkGray, ref scoreValue, ref difficultLevel, ref bulletsValue,
                ref createPlatformValue);

            Console.SetCursorPosition(displayX, displayY);

            SetPlayerPosition(objects, ref playerPositionX, ref playerPositionY, ref lifeValue);

            for (int y = 0; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    DrawColoredSymbol(objects[y, x], ref playerPositionY, heigth, ground);
                }

                Console.WriteLine();
            }
        }

        static void SetPlayerPosition(char[,] objects, ref int playerPositonX, ref int playerPositionY, ref int lifeValue)
        {
            objects[playerPositionY, playerPositonX] = '>';

            if (lifeValue == 0)
            {
                objects[playerPositionY, playerPositonX] = '_';
            }
        }

        static void DoPunch(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY,
            int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            int punchX = playerPositionX + 1;
            int sharpScore = 10;

            if (objects[playerPositionY, punchX] == ' ' || objects[playerPositionY, punchX] == '#')
            {
                if (objects[playerPositionY, punchX] == ' ')
                {
                    objects[playerPositionY, punchX] = '-';

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                    Thread.Sleep(70);

                    objects[playerPositionY, punchX] = ' ';
                }

                if (objects[playerPositionY, punchX] == '#')
                {
                    scoreValue += sharpScore;
                    objects[playerPositionY, punchX] = '-';

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                    Thread.Sleep(70);

                    objects[playerPositionY, punchX] = GenerateDrop();

                }
            }
        }

        static void CreatePlatform(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY,
            int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            int punchX = playerPositionX + 1;
            int createdPlatforms = 3;

            if (createPlatformValue > 0 && (objects[playerPositionY, punchX] == ' ' || objects[playerPositionY, punchX] == '#'))
            {
                createPlatformValue--;

                if (objects[playerPositionY, punchX] == ' ')
                {
                    objects[playerPositionY, punchX] = '-';

                    for (int i = 0; i < createdPlatforms; i++)
                    {
                        ++punchX;
                        objects[playerPositionY, punchX] = 'T';
                    }

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                    Thread.Sleep(70);

                    objects[playerPositionY, punchX - createdPlatforms] = ' ';
                }
            }
        }

        static void Shoot(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY,
            int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            int projectileRange = 15;
            int projectileX = playerPositionX + 1;
            int sharpScore = 10;

            if ( bulletsValue > 0 && (objects[playerPositionY, projectileX] == ' ' || objects[playerPositionY, projectileX] == '#'))
            {
                bulletsValue--;
                objects[playerPositionY, projectileX] = '=';

                for (int x = 0; x < projectileRange; x++)
                {
                    if (objects[playerPositionY, projectileX] == '#')
                    {
                        scoreValue += sharpScore;

                        objects[playerPositionY, projectileX] = GenerateDrop();

                        break;
                    }

                    if (objects[playerPositionY, projectileX] == 'T')
                    {
                        break;
                    }

                    objects[playerPositionY, projectileX] = '=';
                    objects[playerPositionY, projectileX - 1] = ' ';
                    projectileX++;

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                        ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                }

                objects[playerPositionY, projectileX - 1] = ' ';
            }
        }

        static void Jump(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX,
            int displayY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel,
            ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying)
        {
            if (canJump)
            {
                canJump = false;
                int heigthJump = 5;

                for (int y = 0; y < heigthJump; y++)
                {
                    if (playerPositionY > 0 && objects[playerPositionY - 1, playerPositionX] != 'T' && objects[playerPositionY - 1, playerPositionX] != 'M')
                    {
                        
                        ReactCollision(objects[playerPositionY - 1, playerPositionX], ref lifeValue, ref bulletsValue, ref createPlatformValue);

                        playerPositionY--;
                        objects[playerPositionY + 1, playerPositionX] = ' ';

                        if (playerPositionY >= gameScreenHeigth - gameGroundHeigth - 1)
                        {
                            objects[playerPositionY + 1, playerPositionX] = '~';
                        }

                        DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax, 
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue );
                    }

                    ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                        ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying);

                }

                FallWithoutFooting(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying);

                canJump = true;
            }

        }

        static void FallWithoutFooting(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth,
            int displayX, int displayY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue, ref int chanceGeneratePit,
             ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying)
        {
            while (playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == ' ' 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == '#'
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == '~' 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == '+' 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == 'c'
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == '=')
            {

                ReactCollision(objects[playerPositionY + 1, playerPositionX], ref lifeValue, ref bulletsValue, ref createPlatformValue);

                playerPositionY++;

                objects[playerPositionY - 1, playerPositionX] = ' ';

                if (playerPositionY > gameScreenHeigth - gameGroundHeigth)
                {
                    objects[playerPositionY - 1, playerPositionX] = '~';
                }

                if (playerPositionY == gameScreenHeigth - 1)
                {
                    lifeValue = 0;
                }

                DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax, ref lifeValue,
                    lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying);

            }
        }

        static void DrawInterfacePanel(int positionX, int positionY, int valueBarMax, ref int valueFactual, int valueFactualMax, ConsoleColor colorValue, ConsoleColor colorEmpty,
            ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue)
        {
            string bar = "";
            string emptyBar = "";
            int valueRatio = valueFactualMax / valueBarMax;

            for (int i = 0; i < valueFactual / valueRatio; i++)
            {
                bar += " ";
            }

            for (int i = valueFactual / valueRatio; i < valueBarMax; i++)
            {
                emptyBar += " ";
            }

            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine($"Health:\t\t\t\t\tScore: {scoreValue}");
            Console.BackgroundColor = colorValue;
            Console.Write(bar);
            Console.BackgroundColor = colorEmpty;
            Console.Write(emptyBar);
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"\t\t\t Level: {difficultLevel}");
            Console.WriteLine($" bullets | {bulletsValue} ");
            Console.WriteLine($" create  | {createPlatformValue} ");
            Console.ResetColor();
            Console.Write("Control :\n\tD - move right\t\tA - move left\t\tSpacebar - jump\n\tEnter - shoot\t\tL - hit\t\tK - create platform\n\tR - restart");
        }

        static void DrawColoredSymbol(char symbol, ref int playerPositionY, int gameScreenHeigth, int gameGroundHeigth)
        {
            switch (symbol)
            {
                case ' ':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case '>':
                case '_':
                case '-':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    if (playerPositionY > gameScreenHeigth - gameGroundHeigth - 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    break;
                case '#':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 'M':
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case 'T':
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case '~':
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case '=':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case 'c':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case '+':
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    break;
            }

            Console.Write(symbol);

            Console.ResetColor();
        }

        static int ChangeLifeValue(int lifeValue, int changeableLifeValue)
        {
            return lifeValue += changeableLifeValue;
        }

        static void ControlPlaying(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth,
            int displayPositionX, int displayPositionY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue,
            ref int chanceGeneratePit, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo charKey = Console.ReadKey(true);

                switch (charKey.Key)
                {
                    case ConsoleKey.D:

                        MovePlayerRight( objects, gameGroundHeigth, ref playerPositionX, ref playerPositionY, gameScreenHeigth, ref lifeValue, gameScreenLength,
                            displayPositionX, displayPositionY, barValueMax, lifeValueMax, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                            ref createPlatformValue);

                        break;
                    case ConsoleKey.A:

                        MovePlayerLeft(objects, ref playerPositionX, ref playerPositionY, gameScreenHeigth, gameGroundHeigth,ref lifeValue, ref bulletsValue,
                            ref createPlatformValue);

                        break;
                    case ConsoleKey.Enter:

                        Shoot(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY, barValueMax,
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                        break;
                    case ConsoleKey.Spacebar:

                        Jump(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY,
                            barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                            ref createPlatformValue, ref isPlaying);

                        break;
                    case ConsoleKey.L:

                        DoPunch(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY, barValueMax,
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                        break;
                    case ConsoleKey.K:

                        CreatePlatform(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX,
                            displayPositionY, barValueMax, ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue);

                        break;
                    case ConsoleKey.R:
                        isPlaying = false;
                        break;
                    default:
                        break;
                }
            }
        }

        static char GenerateDrop()
        {
            char droppedItem = ' ';

            int chanceDropRange = 20;
            
            Random random = new Random();

            int droppedItemNumber = random.Next(0, chanceDropRange);

            if(droppedItemNumber == 0)
            {
                droppedItem = '+';
            }
            else if(droppedItemNumber == 1)
            {
                droppedItem = '=';
            }
            else if (droppedItemNumber == 2)
            {
                droppedItem = 'c';
            }

            return droppedItem;
        }

        static void ReactCollision(char collisionChar, ref int lifeValue, ref int bulletsValue, ref int createPlatformValue)
        {
            int sharpDamage = -10;
            int healItemRestores = 10;
            int bulletItemRestores = 5;
            int lifeValueMax = 100;

            switch (collisionChar)
            {
                case '#':
                    lifeValue = ChangeLifeValue(lifeValue, sharpDamage);
                    break;
                case '+':
                    if(lifeValue < lifeValueMax)
                    lifeValue = ChangeLifeValue(lifeValue, healItemRestores);
                    break;
                case '=':
                    bulletsValue += bulletItemRestores;
                    break;
                case 'c':
                    createPlatformValue ++;
                    break;
                default:
                    break;
            }
        }
    }
}

