using WaterController;

var hot = new Valve { Temperature = 70f };
var cold = new Valve { Temperature = 15f };

var controller = new Controller(hot, cold, 50f);
await controller.Control();

public class Controller(Valve hotValve, Valve coldValve, float recommendedTemp)
{
    private float _outTemp;
    private readonly Random _random = new();
    
    private readonly float _baseHotTemp = hotValve.Temperature;
    private readonly float _baseColdTemp = coldValve.Temperature;

    public static float CalculateMixTemperature(Valve valve1, Valve valve2)
    {
        float flow1 = valve1.Open * valve1.Pressure;
        float flow2 = valve2.Open * valve2.Pressure;
        float totalFlow = flow1 + flow2;

        if (totalFlow <= 0) 
        {
            return 0; 
        }

        float weightedTemperature = (flow1 * valve1.Temperature) + (flow2 * valve2.Temperature);
    
        return weightedTemperature / totalFlow;
    }

    public async Task Control()
    {
        hotValve.Open = 0.5f;
        coldValve.Open = 0.5f;
        
        while (true)
        {
            await Randomize();
            _outTemp = CalculateMixTemperature(hotValve, coldValve);
            
            if (_outTemp > recommendedTemp)
            {
                hotValve.Open = Math.Max(0f, hotValve.Open - 0.05f);
                coldValve.Open = Math.Min(1f, coldValve.Open + 0.05f);
            }
            else if (_outTemp < recommendedTemp)
            {
                coldValve.Open = Math.Max(0f, coldValve.Open - 0.05f);
                hotValve.Open = Math.Min(1f, hotValve.Open + 0.05f);
            }
            
            Console.WriteLine($"Target: {recommendedTemp}°C | Out: {_outTemp:F1}°C | Error: {Math.Abs(recommendedTemp - _outTemp):F1}°C | HP: {hotValve.Pressure:F2} CP: {coldValve.Pressure:F2}");
        }
    }

    private async Task Randomize()
    {
        await Task.Delay(100);
        
        hotValve.Temperature = _baseHotTemp + (float)(_random.NextDouble() * 6 - 3);
        coldValve.Temperature = _baseColdTemp + (float)(_random.NextDouble() * 6 - 3);

        hotValve.Pressure = 1.0f + (float)(_random.NextDouble() * 0.4 - 0.2);
        coldValve.Pressure = 1.0f + (float)(_random.NextDouble() * 0.4 - 0.2);
    }
}
