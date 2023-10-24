using System.Collections;
using System.IO.Pipes;

namespace lotto;

public class Pool : IEnumerator<List<ushort>>
{
  private static Random _random = new Random();
  private List<ushort> _available;
  public ushort Picks { get; }
  public ushort Balls { get; }
  
  
  public Pool(ushort picks, ushort balls)
  {
    Picks = picks;
    Balls = balls;
    _available = LoadAvailable(balls);
    Current = new List<ushort>();
  }

  private List<ushort> LoadAvailable(ushort balls, List<ushort>? exclude = null)
  {
    var available = new List<ushort>();
    ushort ball = 0;
    while (ball < balls)
    {
      ++ball;
      if (exclude==null || !exclude.Contains(ball))
        available.Add(ball);
    }

    return available;
  }

  public ushort MinBoards()
  {
    double boards = Balls;
    boards = Math.Ceiling(boards / Picks);
    return (ushort)boards;
  }

  private void GetNextBoard()
  {
    Current.Clear();
    if (_available.Count < Picks)
    {
      Current.AddRange(_available);
      _available = LoadAvailable(Balls, Current);
    }

    while (Current.Count < Picks)
    {
      int pick = _random.Next(0, _available.Count - 1);
      Current.Add(_available[pick]);
      _available.RemoveAt(pick);
      if (!_available.Any())
      {
        _available = LoadAvailable(Balls, Current);
      }
    }
    
    Current.Sort();
  }

  public bool MoveNext()
  {
    GetNextBoard();
    return true;
  }

  public void Reset()
  {
  }

  public List<ushort> Current { get; }

  object IEnumerator.Current => Current;

  public void Dispose()
  {
  }
}