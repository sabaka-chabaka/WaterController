using WaterController;

var hot = new Valve();
var cold = new Valve();

var controller = new Controller(hot, cold, 50f);
controller.Control();

public class Controller(Valve hotValve, Valve coldValve, float recommendedTemp)
{
    private float _outTemp;
    
    public static float CalculateMixTemperature(Valve valve1, Valve valve2)
    {
        float totalOpen = valve1.Open + valve2.Open;

        if (totalOpen <= 0) 
        {
            return 0; 
        }

        float weightedTemperature = (valve1.Open * valve1.Temperature) + (valve2.Open * valve2.Temperature);
    
        return weightedTemperature / totalOpen;
    }


    public void Control()
    {
        while (true)
        {
            _outTemp = CalculateMixTemperature(hotValve, coldValve);
            
            if (_outTemp > recommendedTemp)
            {
                hotValve.Open -= 0.001f;
                coldValve.Open += 0.001f;
            }

            if (_outTemp < recommendedTemp)
            {
                coldValve.Open -= 0.001f;
                hotValve.Open += 0.001f;
            }
        }
    }
}