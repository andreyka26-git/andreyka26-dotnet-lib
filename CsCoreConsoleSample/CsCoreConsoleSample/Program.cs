public class Solution
{
    public uint reverseBits(uint n)
    {
        var offset = 31;
        uint res = 0;

        while (n != 0)
        {
            if (n % 2 == 1)
                res = (uint)(res | (1 << offset));

            n = n / 2;
            offset--;
        }

        return res;
    }
}