namespace lotto;

/// <summary>
/// Generate minimum number of lottery picks to use all the available numbers
/// </summary>
public class Lotto
{
  /// <summary>
  /// Executes the lottery number for the specified lottery parameters
  /// </summary>
  /// <param name="args"></param>
  /// <returns>0 - if successful, not 0 if error</returns>
  /// <exception cref="ArgumentException"></exception>
  public static int Execute(string[] args)
  {
    int rc = 0;

    try
    {
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
    }
    catch
    {
      Usage();
      rc = 1;
    }

    return rc;
  }

  private static void Usage()
  {
    Console.Error.WriteLine("Usage lotto count:range [count:range [count:range] ...]");
  }

  private static List<Pool> ParsePools(string[] args)
  {
    var pools = new List<Pool>();
    var idx = 1;
    for (idx = 0; idx < args.Length; ++idx)
    {
        var picksBalls = args[idx].Split(':');
        if (picksBalls.Length != 2)
          throw new ArgumentException($"Invalid Pool Definition <{args[idx]}>");
        pools.Add(new Pool(ushort.Parse(picksBalls[0]), ushort.Parse(picksBalls[1])));
    }

    return pools;
  }

  private static ushort GetMinBoards(List<Pool> pools)
  {
    ushort minBoards = 0;
    foreach (var pool in pools)
    {
      var min = pool.MinBoards();
      if (min > minBoards)
        minBoards = min;
    }

    return minBoards;
  }

  private static void PrintBoard(int idx, List<ushort> board)
  {
    Console.Write($"{idx,2:D1}: ");
    foreach (var num in board)
    {
      Console.Write($"{num,3:D2}");
    }
    Console.WriteLine();
  }
}