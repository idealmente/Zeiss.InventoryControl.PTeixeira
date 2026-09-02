namespace Zeiss.InventoryControl.PTeixeira.Helpers;

public static class CoreHelper
{
    // First approach for 6 number generation, but since the requirement was to be able be generated across multiple instances(BE instances assumed) the logic was moved to the DB for a distributed env
    public static string GenerateUniqueProductID()
    {
        Random generator = new Random();
        String r = generator.Next(0, 1000000).ToString("D6");

        return r;
    }
}