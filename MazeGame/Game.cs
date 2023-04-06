﻿using System.Numerics;
using Figgle;

namespace SimpleMazeGame;

public class Game
{
    public char Wall = '#';
    public char Path = ' ';
    public char Border = '│';
    public char Player = '■';
    public char Exit = '▒';
    
    public ConsoleColor PlayerColor = ConsoleColor.Red;
    public ConsoleColor ExitColor = ConsoleColor.Green;
    public ConsoleColor WallColor = ConsoleColor.Yellow;
    public ConsoleColor PathColor = ConsoleColor.Black;
    public ConsoleColor BorderColor = ConsoleColor.Black;
    
    
    private int width = 20;
    private int height = 20;
    private int seed = 0;
    
    private static bool[,] maze;
    private static Vector2 finishPos;
    
    private Vector2 playerStart = new Vector2(1, 1);
    private Vector2 playerPosition = new Vector2(1, 1);
    private InputProvider inputProvider;
    private LevelGenerator levelGenerator;
    private LevelSystem levelSystem;
    
    private bool gameOver = false;
    
    public Game(InputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }
    
    public Game(int width, int height, int seed, InputProvider inputProvider, LevelGenerator levelGenerator)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
        this.inputProvider = inputProvider;
        this.levelGenerator = levelGenerator;
    }
    
    public Game(int width, int height, int seed, InputProvider inputProvider, LevelGenerator levelGenerator, LevelSystem levelSystem)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
        this.inputProvider = inputProvider;
        this.levelGenerator = levelGenerator;
        this.levelSystem = levelSystem;
    }
    
    public Game(int width, int height, int seed, InputProvider inputProvider, LevelGenerator levelGenerator, Vector2 playerStart)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
        this.inputProvider = inputProvider;
        this.levelGenerator = levelGenerator;
        this.playerStart = playerStart;
    }
    
    

    public void Run()
    {
        Console.WriteLine(FiggleFonts.Standard.Render("Maze Game"));
        Console.WriteLine("Press any key to start");
        Console.ReadKey();
        
        StartLevel();
        
        inputProvider.onResetPressed += (() =>
        {
            StartLevel();
        });
        
        while (!gameOver)
        {
            Console.Clear();
            // draw
            DrawMaze(maze, playerPosition, finishPos);
            // update
            UpdateInput();
            
            // check if player is on exit
            if (playerPosition == finishPos)
            {
                gameOver = true;
            }
        }
        
        Finish();
    }

    private void Finish()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            Console.SetWindowSize(80, 25);
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(FiggleFonts.Standard.Render("You Win!"));
        Console.WriteLine("Congratulations! You won!");
        Console.WriteLine("Press any key to exit");
        levelSystem.LevelUp();
        Console.ReadKey();
    }

    private void StartLevel()
    {
        int currentLevel = levelSystem.GetLevel();
        maze = levelGenerator.GetLevel(currentLevel <= 10 ? 10 : width + currentLevel, currentLevel <= 10 ? 10 : height + currentLevel, seed + currentLevel);
        finishPos = levelGenerator.GetFinishPos();
        playerPosition = playerStart;
        gameOver = false;
    }

    private void UpdateInput()
    {
        Vector2 input = inputProvider.GetInput();
        
        if (input != Vector2.Zero)
        {
            bool canPass = IsPath(playerPosition + input);
            if (canPass)
            {
                playerPosition += input;
            }
        }
    }
    
    private bool IsPath(Vector2 pos)
    {
        // check if the pos not the wall
        bool isWall = maze[(int) pos.Y, (int) pos.X];
        return !isWall;
    }
    
    private  void DrawMaze(bool[,] walls, Vector2 playerPos, Vector2 exitPos)
    {
        // if platform is windows 
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // set the console window size to the maze size
#pragma warning disable CA1416 // im tired of this warnings
            Console.SetWindowSize(width * 2, height + 1);
            // Console.SetBufferSize(width * 3, height * 3);
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // if the current cell is the player cell
                if (x == playerPosition.X && y == playerPosition.Y)
                {
                    WriteToConsoleWithColor(PlayerColor, Player.ToString());
                }
                // if the current cell is the exit cell
                else if (x == exitPos.X && y == exitPos.Y)
                {
                    WriteToConsoleWithColor(ExitColor, Exit.ToString());
                }
                else if (walls[y, x])
                {
                    WriteToConsoleWithColor(WallColor, Wall.ToString());
                }
                else
                {
                    WriteToConsoleWithColor(PathColor, Path.ToString());
                }

                WriteToConsoleWithColor(BorderColor, Border.ToString());
            }
            Console.WriteLine();
        }
    }

    private void WriteToConsoleWithColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
    
    
}