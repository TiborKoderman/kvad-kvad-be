using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

public class CounterService(AppDbContext context)
{
    public async Task<int> Increment()
    {
        var counter = await context.KeyValues.FirstOrDefaultAsync(x => x.Key == "Counter");
        if (counter == null)
        {
            counter = new KeyValue { Key = "Counter", Value = "1" };
            context.KeyValues.Add(counter);
        }
        else
        {
            int currentValue = int.Parse(counter.Value);
            counter.Value = (currentValue + 1).ToString();
        }
        await context.SaveChangesAsync();

        return int.Parse(counter.Value);
    }

    public async Task<int> Decrement()
    {
        var counter = await context.KeyValues.FirstOrDefaultAsync(x => x.Key == "Counter");
        if (counter == null)
        {
            counter = new KeyValue { Key = "Counter", Value = "0" };
            context.KeyValues.Add(counter);
        }
        else
        {
            int currentValue = int.Parse(counter.Value);
            counter.Value = (currentValue - 1).ToString();
        }
        await context.SaveChangesAsync();

        return int.Parse(counter.Value);
    }

    public async Task<int> Reset()
    {
        var counter = await context.KeyValues.FirstOrDefaultAsync(x => x.Key == "Counter");
        if (counter == null)
        {
            counter = new KeyValue { Key = "Counter", Value = "0" };
            context.KeyValues.Add(counter);
        }
        else
        {
            counter.Value = "0";
        }
        await context.SaveChangesAsync();

        return int.Parse(counter.Value);
    }

    public async Task<int> GetCounter()
    {
        var counter = await context.KeyValues.FirstAsync(x => x.Key == "Counter");
        return int.Parse(counter.Value);
    }
}