namespace System
{
    internal static class BlobStorageSystemExtensions
    {
        internal static bool IsSuccessStatusCode(this int @this)
        {
            return @this >= 200 && @this <= 299;
        }
    }
}