namespace JudoScoreBoard;

public class PlayerScore
{
    public PlayerScore()
    {
        IpponScore = 0;
        WazariScore = 0;
        YukoScore = 0;
        ShidoScore = 0;
    }

    public  int IpponScore { get; private set; }
    public int WazariScore { get; private set;}
    public int YukoScore { get;  private set;}
    public int ShidoScore { get;  private set;}

    public async Task IncreaseYukoScore()
    {
        if (IpponScore == 0)
        {
            YukoScore++;
        }

        // await CheckWinner();
    }

    public async Task DecreaseYukoScore()
    {
        if (IpponScore == 0)
        {
            if (YukoScore > 0)
            {
                YukoScore--;
                // await CheckWinner();
            }
        }
    }
    
    public async Task IncreaseIpponScore()
    {
        if (IpponScore == 0)
        {
            IpponScore++;

            // await CheckWinner();
        }
    }

    public async Task DecreaseIpponScore()
    {
        if (IpponScore == 1)
        {
            IpponScore--;
            // await CheckWinner();
        }
    }

    public async Task IncreaseWazariScore()
    {
        if (WazariScore == 0 && IpponScore == 0)
        {
            WazariScore++;
        }
        else if (WazariScore == 1 && IpponScore == 0)
        {
            WazariScore = 0;
            await IncreaseIpponScore();
        }

        // await CheckWinner();
    }

    public async Task DecreaseWazariScore()
    {
        if (WazariScore == 1)
        {
            WazariScore--;
            // await CheckWinner();
        }
    }
    
    public async Task IncreaseShidoScore()
    {
        if (ShidoScore > 2)
        {
            return;
        }

        ShidoScore++;
    }
    
    public async Task DecreaseShidoScore()
    {
        if (ShidoScore <= 0)
        {
            return;
        }
    
        if (ShidoScore == 3)
        {
           
        }

        ShidoScore--;
    }
}