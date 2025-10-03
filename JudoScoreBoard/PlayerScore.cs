namespace JudoScoreBoard;

public class PlayerScore
{
    public  int IpponScore { get; private set; }
    public int WazariScore { get; private set;}
    public int YukoScore { get;  private set;}
    public int ShidoScore { get;  private set;}

    public void IncreaseYukoScore()
    {
        if (IpponScore == 0)
        {
            YukoScore++;
        }
    }

    public void DecreaseYukoScore()
    {
        if (IpponScore == 0)
        {
            if (YukoScore > 0)
            {
                YukoScore--;
            }
        }
    }
    
    public void IncreaseIpponScore()
    {
        if (IpponScore == 0)
        {
            IpponScore++;
        }
    }

    public void DecreaseIpponScore()
    {
        if (IpponScore == 1)
        {
            IpponScore--;
        }
    }

    public void IncreaseWazariScore()
    {
        if (WazariScore == 0 && IpponScore == 0)
        {
            WazariScore++;
        }
        else if (WazariScore == 1 && IpponScore == 0)
        {
            WazariScore = 0;
            IncreaseIpponScore();
        }
    }

    public void DecreaseWazariScore()
    {
        if (WazariScore == 1)
        {
            WazariScore--;
        }
    }
    
    public void IncreaseShidoScore()
    {
        if (ShidoScore > 2)
        {
            return;
        }

        ShidoScore++;
    }
    
    public void DecreaseShidoScore()
    {
        if (ShidoScore <= 0)
        {
            return;
        }
        
        ShidoScore--;
    }
}