using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;

namespace lotto.test;

public class LottoTest
{
  [Fact]
  public void PoolSingleBallTest()
  {
    var pool = new Pool(70, 70);
    Assert.True(pool.MoveNext());
    var board = pool.Current;
    
    Assert.Equal(70, board.Count);

    bool[] ballPresent = new bool[70];
    foreach (var num in board)
    {
      ballPresent[num-1] = true;
    }

    for(int i=0; i<ballPresent.Length; i++)
    {
      Assert.True(ballPresent[i],$"num {i+1} is not present");
    }
  }
  
  [Fact]
  public void PoolMultiBallTest()
  {
    var pool = new Pool(5, 70);
    bool[] ballPresent = new bool[70];

    for (int j = 0; j < 14; ++j)
    {
      Assert.True(pool.MoveNext());
      var board = pool.Current;
      Assert.Equal(5, board.Count);

      foreach (var num in board)
      {
        ballPresent[num-1] = true;
      }
    }

    for(int i=0; i<ballPresent.Length; i++)
    {
      Assert.True(ballPresent[i],$"num {i+1} is not present");
    }
  }

}