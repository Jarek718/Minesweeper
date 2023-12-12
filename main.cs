using System;

class Program 
{
  private static char filledBox = '■';
  private static char emptyBox = '☐';
  private static char mine = '☀';
  private static char deathMine = '☠';
  private static char Flag = '⚐';
  private static char correctFlag = '⚑';

  private  static char[,] backgroundMap;
  private  static char[,] foregroundMap;

  private static int[,] minedPositions;
  
  private static int remaingTiles;
  private static int remaingFlags;
  
  public static void Main (string[] args)
  {
    //Console.OutputEncoding = Encoding.UTF8;
    
    Console.WriteLine("\tWelcome to Minesweeper!\n\t\tBy: Jarosław Rybak\n\nTo repeat help options type:\n !help for help options\n");
    string diff = difficulty();
    if("B" == diff)
    {
      builMap(9, 9, 10);
    }
    else if("I" == diff)
    {
      builMap(16, 16, 40);
    }
    else if("E" == diff)
    {
      builMap(30, 16, 99);
    }
    else
    {
      throw new Exception("Invalid difficulty: This should not happen, "+diff);
    }

    run();
  }

  private static void run()
  {
    while(true)
    {
      printMap(foregroundMap);
      Console.WriteLine(" ");
      int[] input = playerSelection();
      revealBox(input[0], input[1]);
    }
  }

  private static int[] playerSelection()
  {
    Console.WriteLine("Please enter a column and a row: ");
    string playerCoordinates = Console.ReadLine();

    playerCoordinates = playerCoordinates.Replace(" ", "");

    if(playerCoordinates[0]== '!')
    {
      optioins(playerCoordinates);
    }
    else if(playerCoordinates.Length == 2)
    {
      playerCoordinates = playerCoordinates.Insert(1,"0");
    }

    while(!(char.IsLetter(playerCoordinates[0]) && 
            checkDigits(playerCoordinates.Substring(1,2)) && 
            int.Parse(playerCoordinates.Substring(1,2)) < backgroundMap.GetLength(0) && 
            (char.ToUpper(playerCoordinates[0]) - 65) < backgroundMap.GetLength(1)))
    {
      Console.WriteLine("Please enter a column and a row. \nExample: A01");;
      playerCoordinates = Console.ReadLine();
      playerCoordinates = playerCoordinates.Replace(" ", "");
      if(playerCoordinates[0]== '!')
      {
        optioins(playerCoordinates);
      }
      else if(playerCoordinates.Length == 2)
      {
        playerCoordinates = playerCoordinates.Insert(1,"0");
      }
    }

    Console.WriteLine("\nYou selected: " + playerCoordinates[0].ToString() + playerCoordinates.Substring(1,2));

    int[] playerCoordinatesArray = {int.Parse(playerCoordinates.Substring(1,2)), char.ToUpper(playerCoordinates[0]) - 65};

    return playerCoordinatesArray;
  }

  public static void optioins(string input)
  {
    input =  input.ToUpper();
    if(input.Contains("!HELP"))
    {
      Console.WriteLine("\n !help for help options\n !quit to exit the game\n !flag X00 to flag/unflag a tile\n !map to see the map\n !clear to clear all active flags");
    }
    else if(input.Contains("!QUIT"))
    {
      Console.WriteLine("\nThank you for playing!");
      Environment.Exit(0);
    }
    else if(input.Contains("!FLAG"))
    {
      flagTile(input);
    }
    else if(input.Contains("!MAP"))
    {
      printMap(foregroundMap);
    }
    else
    {
      Console.WriteLine("\nInvalid option: " + input+ "\n");
    }
  }
  
  private static void builMap(int rows, int cols, int mines)
  {
    backgroundMap =  new char[rows , cols];
    foregroundMap =  new char[rows , cols];

    for(int i = 0; i < backgroundMap.GetLength(0); i++)
    {
      for(int j = 0; j < backgroundMap.GetLength(1); j++)
      {
        backgroundMap[i,j] = emptyBox;
        foregroundMap[i,j] = filledBox;
      }
    }
    populateMines(rows, cols, mines);
    remaingFlags = mines;
    remaingTiles = rows * cols - mines;
  }

  private static void populateMines(int rows, int cols, int mines)
  {
    Random rng = new Random();
    minedPositions = new int[mines,2];
    
    for(int i = 0; i<mines; i++)
    {
      int rngRow = rng.Next(rows);
      int rngCol = rng.Next(cols);

      //Needs to be redone for infinte loops and all slots filled with mines conditions
      while(backgroundMap[rngRow,rngCol] == mine)
      {
        rngRow = rng.Next(rows);
        rngCol = rng.Next(cols);
      }
      
      backgroundMap[rngRow,rngCol] = mine;
      minedPositions[i,0] = rngRow;
      minedPositions[i,1] = rngCol;
      populateNumbers(rngRow,rngCol);
    }
  }

  private static void populateNumbers(int rngRow, int rngCol)
  {
    for (int i = rngRow - 1; i < rngRow + 2; i++)
    {
      for (int j = rngCol - 1; j < rngCol + 2; j++)
      {
        if(i>=0 && j>=0 && i<backgroundMap.GetLength(0) && j<backgroundMap.GetLength(1))
        {
          if(backgroundMap[i,j]!=mine)
            if(backgroundMap[i,j]==emptyBox)
              backgroundMap[i,j] = '1';
            else
              backgroundMap[i,j]++;
        }
      }
    }
  }

  private static void revealBox(int row, int col)
  {
    if(foregroundMap[row,col] == filledBox)
    {
      foregroundMap[row,col] = backgroundMap[row, col];
      remaingTiles--;
      
      if(foregroundMap[row,col] == mine)
      {
        revealAllMines();
        foregroundMap[row,col] = deathMine;
        defeat();
      }
      else if(foregroundMap[row,col] == emptyBox)
      {
        //reveal around it (Complete)
        for (int i = row - 1; i < row + 2; i++)
        {
          for (int j = col - 1; j < col + 2; j++)
          {
            if(i>=0 && j>=0 && i<backgroundMap.GetLength(0) && j<backgroundMap.GetLength(1))
            {
              revealBox(i,j);
            }
          }
        }
      }
    }
    if(remaingTiles <= 0)
    {
      victory();
    }
  }

  private static void revealAllMines()
  {
    for(int i = 0; i < minedPositions.GetLength(0); i++)
    {
      if(foregroundMap[minedPositions[i,0],minedPositions[i,1]] == Flag)
        foregroundMap[minedPositions[i,0],minedPositions[i,1]] = correctFlag;
      else
        foregroundMap[minedPositions[i,0],minedPositions[i,1]] = backgroundMap[minedPositions[i,0], minedPositions[i,1]];
    }
  }

  private static void flagTile(string input)
  {
    input = input.Substring(5);
    if(input.Length == 2)
    {
      input = input.Insert(1,"0");
    }
    if((char.IsLetter(input[0]) && 
      checkDigits(input.Substring(1,2)) && 
      int.Parse(input.Substring(1,2)) < backgroundMap.GetLength(0) && 
      (char.ToUpper(input[0]) - 65) < backgroundMap.GetLength(1)))
    {
      if(foregroundMap[int.Parse(input.Substring(1,2)), char.ToUpper(input[0]) - 65] == filledBox && remaingFlags > 0)
      {
        foregroundMap[int.Parse(input.Substring(1,2)), char.ToUpper(input[0]) - 65] = Flag;
        Console.WriteLine("\nYou Flagged: " + input[0].ToString() + input.Substring(1,2));
        remaingFlags--;
      }
      else if(foregroundMap[int.Parse(input.Substring(1,2)), char.ToUpper(input[0]) - 65] == Flag)
      {
        foregroundMap[int.Parse(input.Substring(1,2)), char.ToUpper(input[0]) - 65] = filledBox;
        Console.WriteLine("\nYou Unflagged: " + input[0].ToString() + input.Substring(1,2));
        remaingFlags++;
      }
      else
      {
        Console.WriteLine("\nYou can't flag this tile!: "+input+" \nor you no more flags remaining.");
      }

      Console.WriteLine("\nYou have "+remaingFlags+" flags remaining\n");
    }
  }

  private static bool checkDigits(string playerCoordinateRow)
  {
    foreach(char ch in playerCoordinateRow)
    {
      if(!char.IsDigit(ch))
        return false;
    }
    
    return true;
  }

  private static string difficulty()
  {
    Console.WriteLine("Please select a diffuclty: \n\tBeginner: 9 Rows by 9 Columns and 10 Mines\n\tIntermediate: 16 Rows by 16 Columns and 40 Mines\n\tExpert: 30 Rows by 16 Columns and 99 Mines");
    string diff = Console.ReadLine().ToUpper();
    diff = diff[0].ToString();

    while(!(diff == "B" || diff == "I" || diff == "E"))
    {
      Console.WriteLine("Please select a valid diffuclty: \n\tBeginner: 9 Rows by 9 Columns and 10 Mines\n\tIntermediate: 16 Rows by 16 Columns and 40 Mines\n\tExpert: 30 Rows by 16 Columns and 99 Mines");
      diff = Console.ReadLine().ToUpper();
      diff = diff[0].ToString();
    }

    return diff[0].ToString();
  }
  
  private static void victory()
  {
    Console.WriteLine("\t\tGame Over\n\t\t  You Win!\nWindow will close in 10 seconds");
    printMap(foregroundMap);
    System.Threading.Thread.Sleep(10*1000);
    System.Environment.Exit(0);
  }

  private static void defeat()
  {
    Console.WriteLine("\t\tGame Over\n\t\t  You Lose!\nWindow will close in 10 seconds");
    printMap(foregroundMap);
    System.Threading.Thread.Sleep(10*1000);
    System.Environment.Exit(0);
  }
  
  public static void printMap(char[,] map)
  {
    string fullMap="     ";
    for(int i = 0; i < map.GetLength(1); i++)
    {
      fullMap += (char)(i+65);
      fullMap += " ";
    }
    fullMap += "\n";
    
    for(int i = 0; i < map.GetLength(0); i++)
    {
      string rowInput = ""+i;
      if(i.ToString().Length < 2)
        rowInput = "0"+i;
      string row = rowInput+" : ";
      for(int j = 0; j < map.GetLength(1); j++)
      {
        row += map[i,j];
        row += " ";
      }
      fullMap += row+"\n";
    }

     Console.WriteLine (fullMap);
  }
}