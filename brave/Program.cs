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
            char symbolSky = ' ';
            char symbolPlayer = '>';
            char symbolPlatform = 'T';
            char symbolGrass = 'M';
            char symbolPit = '~';
            char symbolHealItem = '+';
            char symbolBullet = '=';
            char symbolCreateItem = 'c';
            char symbolSharp = '#';
            char symbolDeadPlayer = '_';
            char symbolPunch = '-';
            const ConsoleKey PressKeyY = ConsoleKey.Y;
            const ConsoleKey PressKeyN = ConsoleKey.N;

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

                char[,] objects = CreateInitialObjects(gameScreenLength, gameScreenHeigth, gameGroundHeigth, symbolSky, symbolGrass);

                while (lifeValue > 0 && isPlaying)
                {
                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayPositionX, displayPositionY,
                         barValueMax, ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer,
                         symbolSky);

                    ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX,
                        displayPositionY, barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                         ref createPlatformValue, ref isPlaying, symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp, symbolPlayer, symbolDeadPlayer, symbolBullet,
                         symbolHealItem, symbolCreateItem, symbolPunch);

                    FallWithoutFooting(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY,
                         barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                         ref createPlatformValue, ref isPlaying, symbolPlayer, symbolDeadPlayer, symbolSky, symbolSharp, symbolPit, symbolHealItem, symbolCreateItem, symbolBullet,
                          symbolPlatform, symbolGrass, symbolPunch);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Game Over :(");
                Console.ReadKey(true);
                Console.Clear();
                Console.Write($"Your SCORE : {scoreValue} \nPlay again? \tY/N");
                ConsoleKeyInfo charKey = Console.ReadKey(true);

                switch (charKey.Key)
                {
                    case PressKeyY:
                        isPlayingAgain = true;
                        break;
                    case PressKeyN:
                        isPlayingAgain = false;
                        break;
                    default:
                        break;
                }
            }
        }

        static void ConsiderProgress(ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel)
        {
            int maxChanceGeneratePit = 100;
            int difficultRisePits = 10;
            int difficultRiseScore = 150;

            scoreValue++;

            if (chanceGeneratePit != maxChanceGeneratePit && scoreValue / difficultRiseScore > difficultLevel)
            {
                difficultLevel++;
                chanceGeneratePit += difficultRisePits;
            }
        }

        static void GenerateNewObjects(char[,] objects, int ground, int playerPositionX, int playerPositionY, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel,
            char symbolSky, char symbolPlatform, char symbolGrass, char symbolPit,  char symbolSharp)
        {
            int maxPlayerPositionX = 15;
            int chanceGenerateObjectRange = 100;
            int chanceGenerateSharp = 8;
            int chanceGeneratePlatform = 2;
            int chanceExtendPlatform = 90;

            if (playerPositionX == maxPlayerPositionX && objects[playerPositionY, playerPositionX + 1] != symbolPlatform || objects[playerPositionY, playerPositionX + 1] != symbolGrass)
            {
                Random random = new Random();
                int randomCreatedPit = random.Next(0, chanceGenerateObjectRange);

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

                for (int y = 0; y < objects.GetLength(0) - ground; y++)
                {
                    for (int x = objects.GetLength(1) - 1; x < objects.GetLength(1); x++)
                    {
                        int randomCreatedObject = random.Next(0, chanceGenerateObjectRange);                       
                        objects[y, x] = symbolSky;

                        if (chanceGenerateSharp > randomCreatedObject)
                        {
                            objects[y, x] = symbolSharp;
                        }

                        if (chanceGeneratePlatform > randomCreatedObject || (objects[y, x - 1] == symbolPlatform && randomCreatedObject < chanceExtendPlatform))
                        {
                            objects[y, x] = symbolPlatform;
                        }                        
                    }
                }

                for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
                {
                    for (int x = objects.GetLength(1) - 1; x < objects.GetLength(1); x++)
                    {
                        int randomCreatedObject = random.Next(0, chanceGenerateObjectRange);
                        objects[y, x] = symbolGrass;

                        if (randomCreatedObject < chanceGeneratePit)
                        {
                            objects[y, x] = symbolPit;
                        }
                    }
                }

                ConsiderProgress(ref scoreValue, ref chanceGeneratePit, ref difficultLevel);
            }                       
        }


        static void MovePlayer(char[,] objects, int ground, ref int playerPositionX, ref int playerPositionY, int heigth, ref int lifeValue, int length, int displayX,
            int displayY, int barValueMax, int lifeValueMax, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue,
            int directionX, char symbolPlatform, char symbolGrass, char symbolPit, char symbolSky, char symbolSharp)
        {
            int maxPlayerPositionX = 15;
            char collisionChar = objects[playerPositionY, playerPositionX + directionX];

            if (directionX > 0 && playerPositionX == maxPlayerPositionX && objects[playerPositionY, playerPositionX + directionX] != symbolGrass &&
                objects[playerPositionY, playerPositionX + directionX] != symbolPlatform)
            {
                GenerateNewObjects(objects, ground, playerPositionX, playerPositionY, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, symbolSky, symbolPlatform,
                    symbolGrass, symbolPit, symbolSharp);               
            }
            else if (playerPositionX + directionX > 0 && objects[playerPositionY, playerPositionX + directionX] != symbolGrass &&
                objects[playerPositionY, playerPositionX + directionX] != symbolPlatform)
            {
                playerPositionX += directionX;
            }

            if (playerPositionY > heigth - ground - 1)
            {
                objects[playerPositionY, playerPositionX - directionX] = symbolPit;
            }
            else
            {
                objects[playerPositionY, playerPositionX - directionX] = symbolSky;
            }

            ReactCollision(collisionChar, ref lifeValue, ref bulletsValue, ref createPlatformValue);
        }

        static char[,] CreateInitialObjects(int length, int heigth, int ground, char symbolSky, char symbolGrass)
        {
            char[,] objects = new char[heigth, length];

            for (int y = 0; y < objects.GetLength(0) - ground; y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    objects[y, x] = symbolSky;
                }
            }

            for (int y = objects.GetLength(0) - ground; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    objects[y, x] = symbolGrass;
                }
            }

            return objects;
        }

        static void DrawObjects(char[,] objects, int length, int heigth, int ground, ref int playerPositionX, ref int playerPositionY, int displayX, int displayY, int barValueMax,
            ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, char symbolPlayer,
            char symbolDeadPlayer, char symbolSky)
        {
            DrawInterfacePanel(0, heigth, barValueMax, ref lifeValue, lifeValueMax, ConsoleColor.Red, ConsoleColor.DarkGray, ref scoreValue, ref difficultLevel, ref bulletsValue,
                ref createPlatformValue, symbolSky);

            Console.SetCursorPosition(displayX, displayY);

            SetPlayerPosition(objects, ref playerPositionX, ref playerPositionY, ref lifeValue, symbolPlayer, symbolDeadPlayer);

            for (int y = 0; y < objects.GetLength(0); y++)
            {
                for (int x = 0; x < objects.GetLength(1); x++)
                {
                    DrawColoredSymbol(objects[y, x], ref playerPositionY, heigth, ground, symbolPlayer);
                }

                Console.WriteLine();
            }
        }

        static void SetPlayerPosition(char[,] objects, ref int playerPositonX, ref int playerPositionY, ref int lifeValue, char symbolPlayer, char symbolDeadPlayer)
        {
            objects[playerPositionY, playerPositonX] = symbolPlayer;

            if (lifeValue == 0)
            {
                objects[playerPositionY, playerPositonX] = symbolDeadPlayer;
            }
        }

        static void DoPunch(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY,
            int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, char symbolPlayer, 
            char symbolDeadPlayer, char symbolSky, char symbolSharp, char symbolPunch, char symbolHealItem, char symbolBullet, char symbolCreateItem)
        {
            int punchX = playerPositionX + 1;
            int sharpScore = 10;
            int punchSpeed = 70;

            if (objects[playerPositionY, punchX] == symbolSky || objects[playerPositionY, punchX] == symbolSharp)
            {
                if (objects[playerPositionY, punchX] == symbolSky)
                {
                    objects[playerPositionY, punchX] = symbolPunch;

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky);

                    Thread.Sleep(punchSpeed);

                    objects[playerPositionY, punchX] = symbolSky;
                }

                if (objects[playerPositionY, punchX] == symbolSharp)
                {
                    scoreValue += sharpScore;
                    objects[playerPositionY, punchX] = symbolPunch;

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky);

                    Thread.Sleep(70);

                    objects[playerPositionY, punchX] = GenerateDrop( symbolSky,  symbolHealItem,  symbolBullet,  symbolCreateItem);
                }
            }
        }

        static void CreatePlatform(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX,
            int displayY, int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, 
            char symbolPlayer, char symbolDeadPlayer, char symbolSky, char symbolSharp, char symbolPunch, char symbolPlatform)
        {
            int punchX = playerPositionX + 1;
            int createdPlatforms = 3;
            int createSpeed = 70;

            if (createPlatformValue > 0 && (objects[playerPositionY, punchX] == symbolSky || objects[playerPositionY, punchX] == symbolSharp))
            {
                createPlatformValue--;

                if (objects[playerPositionY, punchX] == symbolSky)
                {
                    objects[playerPositionY, punchX] = symbolPunch;

                    for (int i = 0; i < createdPlatforms; i++)
                    {
                        ++punchX;
                        objects[playerPositionY, punchX] = symbolPlatform;
                    }

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky);

                    Thread.Sleep(createSpeed);

                    objects[playerPositionY, punchX - createdPlatforms] = symbolSky;
                }
            }
        }

        static void Shoot(char[,] objects, int playerPositionX, int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX, int displayY,
            int barValueMax, ref int lifeValue, int lifeValueMax, ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, char symbolPlayer,
            char symbolDeadPlayer, char symbolSky, char symbolSharp, char symbolBullet, char symbolPlatform, char symbolHealItem, char symbolCreateItem)
        {
            int projectileRange = 15;
            int projectileX = playerPositionX + 1;
            int sharpScore = 10;

            if ( bulletsValue > 0 && (objects[playerPositionY, projectileX] == symbolSky || objects[playerPositionY, projectileX] == symbolSharp))
            {
                bulletsValue--;
                objects[playerPositionY, projectileX] = symbolBullet;

                for (int x = 0; x < projectileRange; x++)
                {
                    if (objects[playerPositionY, projectileX] == symbolSharp)
                    {
                        scoreValue += sharpScore;

                        objects[playerPositionY, projectileX] = GenerateDrop( symbolSky,  symbolHealItem,  symbolBullet,  symbolCreateItem);

                        break;
                    }

                    if (objects[playerPositionY, projectileX] == symbolPlatform)
                    {
                        break;
                    }

                    objects[playerPositionY, projectileX] = symbolBullet;
                    objects[playerPositionY, projectileX - 1] = symbolSky;
                    projectileX++;

                    DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax,
                        ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky);
                }

                objects[playerPositionY, projectileX - 1] = symbolSky;
            }
        }

        static void Jump(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth, int displayX,
            int displayY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue, ref int chanceGeneratePit, ref int difficultLevel,
            ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying, char symbolPlayer, char symbolDeadPlayer, char symbolPlatform, char symbolGrass, char symbolPit,
            char symbolSky, char symbolSharp, char symbolHealItem, char symbolCreateItem, char symbolBullet, char symbolPunch)
        {
            if (canJump)
            {
                canJump = false;
                int heigthJump = 5;

                for (int y = 0; y < heigthJump; y++)
                {
                    if (playerPositionY > 0 && objects[playerPositionY - 1, playerPositionX] != symbolPlatform && objects[playerPositionY - 1, playerPositionX] != symbolGrass)
                    {
                        char collisionChar = objects[playerPositionY - 1, playerPositionX];
                        playerPositionY--;
                        objects[playerPositionY + 1, playerPositionX] = symbolSky;

                        if (playerPositionY >= gameScreenHeigth - gameGroundHeigth - 1)
                        {
                            objects[playerPositionY + 1, playerPositionX] = symbolPit;
                        }

                        DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax, 
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky );

                        ReactCollision(collisionChar, ref lifeValue, ref bulletsValue, ref createPlatformValue);
                    }

                    ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                        ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying,
                         symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp, symbolPlayer, symbolDeadPlayer, symbolBullet, symbolHealItem, symbolCreateItem, symbolPunch);
                }

                FallWithoutFooting(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying,
                    symbolPlayer, symbolDeadPlayer, symbolSky, symbolSharp, symbolPit, symbolHealItem, symbolCreateItem, symbolBullet, symbolPlatform, symbolGrass, symbolPunch);

                canJump = true;
            }

        }

        static void FallWithoutFooting(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth,
            int displayX, int displayY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue, ref int chanceGeneratePit,
             ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying, char symbolPlayer, char symbolDeadPlayer, char symbolSky, char symbolSharp,
             char symbolPit, char symbolHealItem, char symbolCreateItem, char symbolBullet, char symbolPlatform, char symbolGrass, char symbolPunch)
        {
            while (playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolSky 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolSharp
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolPit 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolHealItem 
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolCreateItem
                || playerPositionY < gameScreenHeigth - 1 && objects[playerPositionY + 1, playerPositionX] == symbolBullet)
            {
                char collisionChar = objects[playerPositionY + 1, playerPositionX];
                playerPositionY++;
                objects[playerPositionY - 1, playerPositionX] = symbolSky;

                ReactCollision(collisionChar, ref lifeValue, ref bulletsValue, ref createPlatformValue);

                if (playerPositionY > gameScreenHeigth - gameGroundHeigth)
                {
                    objects[playerPositionY - 1, playerPositionX] = symbolPit;
                }

                if (playerPositionY == gameScreenHeigth - 1)
                {
                    lifeValue = 0;
                }

                DrawObjects(objects, gameScreenLength, gameScreenHeigth, gameGroundHeigth, ref playerPositionX, ref playerPositionY, displayX, displayY, barValueMax, ref lifeValue,
                    lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky);

                ControlPlaying(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayX, displayY, barValueMax,
                    ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue, ref createPlatformValue, ref isPlaying,
                    symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp, symbolPlayer, symbolDeadPlayer, symbolBullet, symbolHealItem, symbolCreateItem, symbolPunch);
            }
        }

        static void DrawInterfacePanel(int positionX, int positionY, int valueBarMax, ref int valueFactual, int valueFactualMax, ConsoleColor colorValue, ConsoleColor colorEmpty,
            ref int scoreValue, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, char symbolSky)
        {
            string bar = "";
            string emptyBar = "";
            int valueRatio = valueFactualMax / valueBarMax;

            for (int i = 0; i < valueBarMax; i++)
            {
                if(i >= valueFactual / valueRatio)
                {
                    emptyBar += symbolSky;

                }
                else
                {
                    bar += symbolSky;
                }
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

        static void DrawColoredSymbol(char symbol, ref int playerPositionY, int gameScreenHeigth, int gameGroundHeigth, char symbolPlayer)
        {
            const char symbolSky = ' ';
            const char symbolAlivePlayer = '>';
            const char symbolDeadPlayer = '_';
            const char symbolPunch = '-';
            const char symbolSharp = '#';
            const char symbolGrass = 'M';
            const char symbolPlatform = 'T';
            const char symbolPit = '~';
            const char symbolBullet = '=';
            const char symbolCreateItem = 'c';
            const char symbolHealItem = '+';

            switch (symbol)
            {
                case symbolSky:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.DarkCyan, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolAlivePlayer:
                case symbolDeadPlayer:
                case symbolPunch:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolSharp:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.White, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolGrass:

                    ChangeColor(ConsoleColor.Green, ConsoleColor.DarkGreen, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolPlatform:

                    ChangeColor(ConsoleColor.DarkYellow, ConsoleColor.Yellow, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolPit:

                    ChangeColor(ConsoleColor.Black, ConsoleColor.DarkYellow, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolBullet:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.DarkYellow, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolCreateItem:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.Yellow, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                case symbolHealItem:

                    ChangeColor(ConsoleColor.DarkCyan, ConsoleColor.Green, ref playerPositionY, gameScreenHeigth, gameGroundHeigth, symbol, symbolPlayer);

                    break;
                default:
                    break;
            }

            Console.Write(symbol);
            Console.ResetColor();
        }

        static void ChangeColor(ConsoleColor backgroundColor, ConsoleColor symbolColor, ref int playerPositionY, int gameScreenHeigth, int gameGroundHeigth, char charSymbol,
            char symbolPlayer)
        {
            Console.ForegroundColor = symbolColor;

            if (playerPositionY > gameScreenHeigth - gameGroundHeigth - 1 && charSymbol == symbolPlayer)
            {
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = backgroundColor;
            }
        }

        static int IncreaseLifeValue(int lifeValue, int changeableLifeValue)
        {
            return lifeValue += changeableLifeValue;
        }

        static void ControlPlaying(char[,] objects, ref int playerPositionX, ref int playerPositionY, int gameScreenLength, int gameScreenHeigth, int gameGroundHeigth,
            int displayPositionX, int displayPositionY, int barValueMax, ref int lifeValue, int lifeValueMax, ref bool canJump, ref int scoreValue,
            ref int chanceGeneratePit, ref int difficultLevel, ref int bulletsValue, ref int createPlatformValue, ref bool isPlaying, char symbolPlatform, char symbolGrass,
            char symbolPit, char symbolSky, char symbolSharp, char symbolPlayer, char symbolDeadPlayer, char symbolBullet, char symbolHealItem, char symbolCreateItem, 
            char symbolPunch)
        {
            const ConsoleKey PressKeyD = ConsoleKey.D;
            const ConsoleKey PressKeyA = ConsoleKey.A;
            const ConsoleKey PressEnter = ConsoleKey.Enter;
            const ConsoleKey PressSpacebar = ConsoleKey.Spacebar;
            const ConsoleKey PressKeyL = ConsoleKey.L;
            const ConsoleKey PressKeyK = ConsoleKey.K;
            const ConsoleKey PressKeyR = ConsoleKey.R;
            int directionXRight = 1;
            int directionXLeft = -1;

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo charKey = Console.ReadKey(true);

                switch (charKey.Key)
                {
                    case PressKeyD:

                        MovePlayer( objects, gameGroundHeigth, ref playerPositionX, ref playerPositionY, gameScreenHeigth, ref lifeValue, gameScreenLength,
                            displayPositionX, displayPositionY, barValueMax, lifeValueMax, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                            ref createPlatformValue, directionXRight, symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp);

                        break;
                    case PressKeyA:

                        MovePlayer(objects, gameGroundHeigth, ref playerPositionX, ref playerPositionY, gameScreenHeigth, ref lifeValue, gameScreenLength,
                            displayPositionX, displayPositionY, barValueMax, lifeValueMax, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                            ref createPlatformValue, directionXLeft, symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp);

                        break;
                    case PressEnter:

                        Shoot(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY, barValueMax,
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky,
                            symbolSharp, symbolBullet, symbolPlatform, symbolHealItem, symbolCreateItem);

                        break;
                    case PressSpacebar:

                        Jump(objects, ref playerPositionX, ref playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY,
                            barValueMax, ref lifeValue, lifeValueMax, ref canJump, ref scoreValue, ref chanceGeneratePit, ref difficultLevel, ref bulletsValue,
                            ref createPlatformValue, ref isPlaying, symbolPlayer, symbolDeadPlayer, symbolPlatform, symbolGrass, symbolPit, symbolSky, symbolSharp, symbolHealItem, 
                            symbolCreateItem, symbolBullet, symbolPunch);

                        break;
                    case PressKeyL:

                        DoPunch(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX, displayPositionY, barValueMax,
                            ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, symbolDeadPlayer, symbolSky, 
                            symbolSharp, symbolPunch, symbolHealItem, symbolBullet, symbolCreateItem);

                        break;
                    case PressKeyK:

                        CreatePlatform(objects, playerPositionX, playerPositionY, gameScreenLength, gameScreenHeigth, gameGroundHeigth, displayPositionX,
                            displayPositionY, barValueMax, ref lifeValue, lifeValueMax, ref scoreValue, ref difficultLevel, ref bulletsValue, ref createPlatformValue, symbolPlayer, 
                            symbolDeadPlayer, symbolSky, symbolSharp, symbolPunch, symbolPlatform);

                        break;
                    case PressKeyR:
                        isPlaying = false;
                        break;
                    default:
                        break;
                }
            }
        }

        static char GenerateDrop(char symbolSky, char symbolHealItem, char symbolBullet, char symbolCreateItem)
        {
            char droppedItem = symbolSky;
            int valueDropCreateItem = 2;
            int chanceDropRange = 20;           
            Random random = new Random();
            int droppedItemNumber = random.Next(0, chanceDropRange);

            if(droppedItemNumber == 0)
            {
                droppedItem = symbolHealItem;
            }
            else if(droppedItemNumber == 1)
            {
                droppedItem = symbolBullet;
            }
            else if (droppedItemNumber == valueDropCreateItem)
            {
                droppedItem = symbolCreateItem;
            }

            return droppedItem;
        }

        static void ReactCollision(char collisionChar, ref int lifeValue, ref int bulletsValue, ref int createPlatformValue)
        {
            int sharpDamage = -10;
            int healItemRestores = 10;
            int bulletItemRestores = 5;
            int lifeValueMax = 100;
            const char symbolSharp = '#';
            const char symbolHealItem = '+';
            const char symbolBullet = '=';
            const char symbolCreateItem = 'c';

            switch (collisionChar)
            {
                case symbolSharp:

                    lifeValue = IncreaseLifeValue(lifeValue, sharpDamage);

                    break;
                case symbolHealItem:

                    if(lifeValue < lifeValueMax) lifeValue = IncreaseLifeValue(lifeValue, healItemRestores);

                    break;
                case symbolBullet:
                    bulletsValue += bulletItemRestores;
                    break;
                case symbolCreateItem:
                    createPlatformValue ++;
                    break;
                default:
                    break;
            }
        }
    }
}

