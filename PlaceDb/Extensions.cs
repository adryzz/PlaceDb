namespace PlaceDb
{
    public static class Extensions
    {
        public static int ToInt(this char c)
        {
            return 0b0000_1111 & (byte) c;
        }
    }
}