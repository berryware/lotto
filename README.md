## Generating Lottery Numbers

I started buying lottery tickets when Powerball was nearing a payout of $2B. I bought five quick picks (machine generated numbers) and noticed that there were many duplicated numbers across the five picks. I even had two picks that started with the same two numbers. After the drawing, I found that I had zero matching numbers and decided that there must be a better way to generate the numbers and at least get more matching numbers.

**OBVIOUS DISCLAIMERS:**
- There is nothing here that changes the odds of winning.
    - Powerball is 1 in 292,201,338
    - MegaMillions is 1 in 302,575,350.
- All random number generators are not really random.
- This algorithm just generates picks with non-repeating numbers.
- This algorithm does make me feel better because I can circle more matching numbers

**TERMINOLOGY**
The Mega Millions description of how to play defines each pick as a "board", and each "board" consists of two pools of number. Pool 1 consists of 5 non-repeating numbers from 1 to 70. Pool 2 is 1 number from 1 to 25.

**REQUIREMENTS**
- The program should support multiple lotteries
- The arguments should be pool definitions in the form of "selections:max_num".
    - Powerball = "5:69 1:26"
    - Mega Millions = "5:70 1:25"
- The program should figure out the minimum number of boards required to include all the numbers.

**CLASSES**
Lotto is responsible for parsing the command-line arguments, creating the pools, and printing out each board. The main loop is as follows:

```csharp
      if (args.Length < 2)
        throw new ArgumentException("Wrong Number of Arguments");
      
      var pools = ParsePools(args);
      var minBoards = GetMinBoards(pools);
      var board = new List<ushort>();
      for (int i = 0; i < minBoards; i++)
      {
        foreach (var pool in pools)
        {
          pool.MoveNext();
          board.AddRange(pool.Current);
        }
        PrintBoard(i+1, board);
        board.Clear();
      }
```

`ParsePools(args)` parses the command-line arguments and returns a `List<Pool>`. `GetMinBoards(pools)` gets the minimum number of boards for each pool and returns the greatest. Next it iterates minBoards times and gets the next pick for each pool and adds it to the board. You may have noticed that `Pool` is an `IEnumerator`. `Pool` implements the `IEnumerator<List<ushort>>` interface.

```csharp
public class Pool : IEnumerator<List<ushort>>
{
  private static Random _random = new Random();
  private List<ushort> _available;
  public ushort Selections { get; }
  public ushort MaxNumber { get; }
  
  
  public Pool(ushort selections, ushort maxNumber)
  {
    Selections = selections;
    MaxNumber = maxNumber;
    _available = LoadAvailable(maxNumber);
    Current = new List<ushort>();
  }
}
```

The `Pool` constructor takes as arguments, the number of selections that must be made for the pool and the maximum number that can be chosen. `LoadAvailable` adds integers 1..MaxNumber into a list for use in the board. Keeping the numbers in an available list allows the program not to have to remember the selections that were already made. A random number is generated as an index into the list and that item is used and removed. When the available list is empty, we call `LoadAvailable` again and immediately remove any numbers that are in the current unfinished board so that duplicates are avoided. When all the selections are made for the current pool, the selections are sorted.

```csharp
  private void GetNextBoard()
  {
    Current.Clear();
    if (_available.Count < Selections)
    {
      Current.AddRange(_available);
      _available = LoadAvailable(MaxNumber, Current);
    }

    while (Current.Count < Selections)
    {
      int pick = _random.Next(0, _available.Count - 1);
      Current.Add(_available[pick]);
      _available.RemoveAt(pick);
      if (!_available.Any())
      {
        _available = LoadAvailable(MaxNumber, Current);
      }
    }
    
    Current.Sort();
  }
```

The last thing to remember is to call ceiling when calculating the minimum number of boards so that you don't miss some numbers because of a rounding error.

```csharp
  public ushort MinBoards()
  {
    double boards = Math.Ceiling((double)MaxNumber / Selections);
    return (ushort)boards;
  }
```

Running the program for PowerBall is as follows:

```
lotto 5:69 1:26
```
which returns
```
 1:  02 16 35 42 53 25
 2:  24 36 39 41 51 11
 3:  06 32 46 56 65 17
 4:  33 34 47 61 66 15
 5:  04 17 22 25 57 01
 6:  11 38 50 59 62 09
 7:  05 07 09 37 49 06
 8:  12 13 28 44 45 18
 9:  03 08 14 26 40 03
10:  19 31 52 63 68 20
11:  01 15 21 29 67 08
12:  10 43 48 54 64 19
13:  20 23 27 30 58 24
14:  18 51 55 60 69 21
15:  04 14 39 56 62 22
16:  26 27 49 53 57 23
17:  12 21 22 36 38 13
18:  01 02 25 31 46 12
19:  09 13 19 44 63 16
20:  03 20 23 33 48 14
21:  08 35 37 47 66 07
22:  05 17 29 45 50 04
23:  06 11 40 42 54 02
24:  41 52 58 65 67 05
25:  07 10 16 28 64 10
26:  15 24 30 59 61 26
```

I hope you enjoyed this coding tangent I went on. If you happen to win the lottery using any numbers generated by lotto, I accept gratuities. The code is available [here](https://github.com/berryware/lotto) 